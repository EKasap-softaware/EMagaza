using EMagaza.Domain.Enums;

namespace EMagaza.Application.Models;

public static class OrderEventKinds
{
    public const string Paid = "paid";
    public const string Preparing = "preparing";
    public const string Shipped = "shipped";
    public const string Delivered = "delivered";
    public const string Cancelled = "cancelled";

    public static string For(OrderStatus status) => status switch
    {
        OrderStatus.Paid => Paid,
        OrderStatus.Preparing => Preparing,
        OrderStatus.Shipped => Shipped,
        OrderStatus.Delivered => Delivered,
        OrderStatus.Cancelled => Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(status))
    };
}
