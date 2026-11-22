using EMagaza.Domain.Entities;

namespace EMagaza.Application.Models;

/// <summary>Kuyruğa bırakılan mesaj. Kendi kendine yeter: tüketici veritabanına sormadan bildirimi üretebilir.</summary>
public record OrderEvent(
    string Kind,
    string OrderId,
    string OrderNumber,
    string UserId,
    string CustomerName,
    string Email,
    decimal Total,
    string? TrackingNumber)
{
    public const string QueueName = "order-events";

    public static OrderEvent From(Order order, string kind) =>
        new(kind, order.Id, order.Number, order.UserId, order.CustomerName, order.Email, order.Total, order.TrackingNumber);
}
