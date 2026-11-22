using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Carts;

/// <summary>Veritabanı bilmeyen sepet. Aynı ürünün farklı bedenleri ayrı satırdır.</summary>
public class Cart
{
    public const int MaxPerLine = 10;

    public List<CartLine> Lines { get; set; } = new();
    public string? CouponCode { get; set; }
    public decimal Discount { get; set; }

    public decimal Subtotal => Lines.Sum(l => l.LineTotal);
    public decimal ShippingFee => ShippingPolicy.FeeFor(Subtotal - Discount);
    public decimal Total => Subtotal - Discount + ShippingFee;
    public int ItemCount => Lines.Sum(l => l.Quantity);
    public decimal RemainingForFreeShipping => Math.Max(0, ShippingPolicy.FreeThreshold - (Subtotal - Discount));

    public void ApplyCoupon(string code, decimal discount)
    {
        CouponCode = code;
        Discount = Math.Min(discount, Subtotal);
    }

    public void ClearCoupon()
    {
        CouponCode = null;
        Discount = 0;
    }

    public int QuantityOf(string productId, string size) =>
        Lines.FirstOrDefault(l => l.ProductId == productId && l.Size == size)?.Quantity ?? 0;

    public void Add(Product product, string size, int quantity)
    {
        if (quantity <= 0) throw new DomainException("Adet 0'dan büyük olmalı.");
        if (string.IsNullOrWhiteSpace(size)) throw new DomainException("Beden seçilmeli.");

        var index = IndexOf(product.Id, size);
        if (index >= 0)
        {
            Lines[index] = Lines[index] with { Quantity = Math.Min(MaxPerLine, Lines[index].Quantity + quantity) };
            return;
        }

        Lines.Add(new CartLine(product.Id, size, product.Name, product.Brand, product.Type,
            product.ColorName, product.ColorHex, product.Price, Math.Min(MaxPerLine, quantity)));
    }

    public void SetQuantity(string productId, string size, int quantity)
    {
        if (quantity <= 0)
        {
            Remove(productId, size);
            return;
        }

        var index = IndexOf(productId, size);
        if (index >= 0) Lines[index] = Lines[index] with { Quantity = Math.Min(MaxPerLine, quantity) };
    }

    public void Remove(string productId, string size) =>
        Lines.RemoveAll(l => l.ProductId == productId && l.Size == size);

    public void Clear()
    {
        Lines.Clear();
        ClearCoupon();
    }

    private int IndexOf(string productId, string size) =>
        Lines.FindIndex(l => l.ProductId == productId && l.Size == size);
}
