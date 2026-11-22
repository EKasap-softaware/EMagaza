namespace EMagaza.Domain.Entities;

/// <summary>Sipariş anındaki ürün görüntüsü: ad ve fiyat kopyalanır, ürün sonradan değişse de sipariş bozulmaz.</summary>
public class OrderItem
{
    public string ProductId { get; private set; } = string.Empty;
    public string ProductName { get; private set; } = string.Empty;
    public string Brand { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string ColorName { get; private set; } = string.Empty;
    public string ColorHex { get; private set; } = "#999999";
    public string Size { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public decimal LineTotal => UnitPrice * Quantity;

    private OrderItem() { }

    public OrderItem(Product product, string size, int quantity)
    {
        ProductId = product.Id;
        ProductName = product.Name;
        Brand = product.Brand;
        Type = product.Type;
        ColorName = product.ColorName;
        ColorHex = product.ColorHex;
        Size = size;
        UnitPrice = product.Price;
        Quantity = quantity;
    }
}
