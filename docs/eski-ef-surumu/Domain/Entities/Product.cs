namespace EMagaza.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = "";
    public string Emoji { get; set; } = "📦";      // görsel yerine emoji: dış bağımlılık yok
    public string Color { get; set; } = "#6366f1"; // kart arka plan rengi

    /// <summary>İş kuralı: stoktan düşme kuralı entity'nin kendisinde yaşar.</summary>
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (quantity > Stock)
            throw new InvalidOperationException($"'{Name}' için yeterli stok yok (kalan: {Stock}).");
        Stock -= quantity;
    }
}
