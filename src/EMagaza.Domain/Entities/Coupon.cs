using EMagaza.Domain.Enums;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Domain.Entities;

public class Coupon : Entity
{
    public string Code { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DiscountKind Kind { get; private set; }
    public decimal Value { get; private set; }
    public decimal MinSubtotal { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    /// <summary>0 = sınırsız kullanım.</summary>
    public int UsageLimit { get; private set; }
    public int UsedCount { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Coupon() { }

    public Coupon(string code, DiscountKind kind, decimal value, decimal minSubtotal = 0, DateTime? expiresAt = null, int usageLimit = 0, string description = "")
    {
        if (string.IsNullOrWhiteSpace(code)) throw new DomainException("Kupon kodu zorunlu.");
        if (value <= 0) throw new DomainException("İndirim değeri 0'dan büyük olmalı.");
        if (kind == DiscountKind.Percent && value > 100) throw new DomainException("Yüzde indirimi 100'ü geçemez.");

        Code = code.Trim().ToUpperInvariant();
        Kind = kind;
        Value = value;
        MinSubtotal = minSubtotal;
        ExpiresAt = expiresAt;
        UsageLimit = usageLimit;
        Description = description;
    }

    /// <summary>Bellekte kullanım hakkı alır (veritabanı tarafında aynı iş atomik güncellemeyle yapılır).</summary>
    public bool TryUse()
    {
        if (!IsActive || (UsageLimit > 0 && UsedCount >= UsageLimit)) return false;
        UsedCount++;
        return true;
    }

    public void ReleaseUse()
    {
        if (UsedCount > 0) UsedCount--;
    }

    public bool IsAvailableAt(DateTime now) =>
        IsActive && (ExpiresAt is null || ExpiresAt > now) && (UsageLimit == 0 || UsedCount < UsageLimit);

    /// <summary>Verilen ara toplam için indirim tutarı. Kupon geçerli değilse DomainException fırlatır.</summary>
    public decimal DiscountFor(decimal subtotal, DateTime now)
    {
        if (!IsAvailableAt(now)) throw new DomainException("Bu kuponun süresi dolmuş veya kullanım limiti bitmiş.");
        if (subtotal < MinSubtotal) throw new DomainException($"Bu kupon için sepet tutarı en az {MinSubtotal:0} ₺ olmalı.");

        var amount = Kind == DiscountKind.Percent ? subtotal * Value / 100 : Value;
        return Math.Min(Math.Round(amount, 2), subtotal);
    }
}
