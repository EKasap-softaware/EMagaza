using EMagaza.Domain.Enums;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Domain.Entities;

public class Order : Entity
{
    public string UserId { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Paid;
    public string PaymentBrand { get; private set; } = string.Empty;
    public string PaymentLast4 { get; private set; } = string.Empty;
    public string PaymentTransactionId { get; private set; } = string.Empty;
    public string? TrackingNumber { get; private set; }
    public decimal ShippingFee { get; private set; }
    public string? CouponCode { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();

    public decimal Subtotal => Items.Sum(i => i.LineTotal);
    public decimal Total => Subtotal - DiscountAmount + ShippingFee;
    public bool CanCancel => Status is OrderStatus.Paid or OrderStatus.Preparing;
    public int ItemCount => Items.Sum(i => i.Quantity);

    /// <summary>Müşteriye gösterilen numara: EM-yyAAgg-XXXXX. Kayıttan sonra (Id atanınca) anlamlıdır.</summary>
    public string Number => Id.Length >= 5 ? $"EM-{CreatedAt:yyMMdd}-{Id[^5..].ToUpperInvariant()}" : Id;

    private Order() { }

    public Order(string userId, string customerName, string email, string phone, string city, string address)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new DomainException("Sipariş için giriş yapılmalı.");
        if (string.IsNullOrWhiteSpace(customerName)) throw new DomainException("Ad soyad zorunlu.");
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("E-posta zorunlu.");
        if (phone is null || phone.Count(char.IsDigit) < 10) throw new DomainException("Geçerli bir telefon numarası girin.");
        if (string.IsNullOrWhiteSpace(city)) throw new DomainException("İl zorunlu.");
        if (string.IsNullOrWhiteSpace(address)) throw new DomainException("Adres zorunlu.");

        UserId = userId;
        CustomerName = customerName.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        City = city.Trim();
        Address = address.Trim();
    }

    public void AddItem(Product product, string size, int quantity)
    {
        if (quantity <= 0) throw new DomainException("Adet 0'dan büyük olmalı.");
        Items.Add(new OrderItem(product, size, quantity));
    }

    public void ApplyShippingFee(decimal fee)
    {
        if (fee < 0) throw new DomainException("Kargo ücreti negatif olamaz.");
        ShippingFee = fee;
    }

    public void ApplyDiscount(string code, decimal amount)
    {
        if (amount < 0 || amount > Subtotal) throw new DomainException("İndirim tutarı geçersiz.");
        CouponCode = code;
        DiscountAmount = amount;
    }

    public void MarkPaid(string brand, string last4, string transactionId)
    {
        if (Items.Count == 0) throw new DomainException("Boş sipariş için ödeme alınamaz.");
        Status = OrderStatus.Paid;
        PaidAt = DateTime.UtcNow;
        PaymentBrand = brand;
        PaymentLast4 = last4;
        PaymentTransactionId = transactionId;
    }

    /// <summary>Kargoya verilmeden önce iptal edilebilir. Stok ve ödeme iadesi servis katmanının işidir.</summary>
    public void Cancel()
    {
        if (!CanCancel) throw new DomainException("Kargoya verilen veya tamamlanan sipariş iptal edilemez.");
        Status = OrderStatus.Cancelled;
    }

    /// <summary>Durum yalnızca ileri gider ve adım atlanamaz: Ödendi → Hazırlanıyor → Kargoda → Teslim edildi.</summary>
    public void MoveTo(OrderStatus next, string? trackingNumber = null)
    {
        if (next == OrderStatus.Cancelled || (int)next != (int)Status + 1)
            throw new DomainException($"Sipariş '{Status}' durumundan '{next}' durumuna geçemez.");

        Status = next;
        if (next == OrderStatus.Shipped)
            TrackingNumber = string.IsNullOrWhiteSpace(trackingNumber)
                ? $"TK{Random.Shared.NextInt64(1_000_000_000, 9_999_999_999)}"
                : trackingNumber.Trim();
    }
}
