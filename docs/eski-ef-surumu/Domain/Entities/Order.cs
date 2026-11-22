namespace EMagaza.Domain.Entities;

public enum OrderStatus { Received = 0, Paid = 1, Shipped = 2, Cancelled = 3 }

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Address { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Received;
    public List<OrderItem> Items { get; set; } = new();

    public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    // Sipariş anındaki ad/fiyat kopyalanır: ürün fiyatı sonra değişse de eski sipariş bozulmasın.
    public string ProductName { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
