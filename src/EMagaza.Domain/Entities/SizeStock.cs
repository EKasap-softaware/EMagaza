using EMagaza.Domain.Exceptions;

namespace EMagaza.Domain.Entities;

public class SizeStock
{
    public string Size { get; private set; } = string.Empty;
    public int Stock { get; private set; }

    private SizeStock() { }

    public SizeStock(string size, int stock)
    {
        if (string.IsNullOrWhiteSpace(size)) throw new DomainException("Beden adı zorunlu.");
        if (stock < 0) throw new DomainException("Stok negatif olamaz.");
        Size = size.Trim();
        Stock = stock;
    }

    public void Decrease(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Adet 0'dan büyük olmalı.");
        if (quantity > Stock) throw new DomainException($"{Size} bedeni için yeterli stok yok (kalan: {Stock}).");
        Stock -= quantity;
    }

    public void Increase(int quantity)
    {
        if (quantity <= 0) throw new DomainException("Adet 0'dan büyük olmalı.");
        Stock += quantity;
    }
}
