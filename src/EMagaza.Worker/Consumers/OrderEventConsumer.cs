using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Models;
using EMagaza.Application.Notifications;
using MassTransit;

namespace EMagaza.Worker.Consumers;

/// <summary>'order-events' kuyruğundaki her olay için bildirim ve e-posta kaydı oluşturur.</summary>
public class OrderEventConsumer(
    INotificationRepository notifications,
    IEmailLogRepository emails,
    ILogger<OrderEventConsumer> logger) : IConsumer<OrderEvent>
{
    public async Task Consume(ConsumeContext<OrderEvent> context)
    {
        var orderEvent = context.Message;
        var (notification, email) = OrderMessageBuilder.Build(orderEvent);

        // İkisi de upsert: mesaj tekrar gelse bile tek kayıt oluşur.
        await notifications.UpsertAsync(notification);
        await emails.UpsertAsync(email);

        logger.LogInformation("İşlendi: {Kind} · sipariş {OrderNumber} ({Email})", orderEvent.Kind, orderEvent.OrderNumber, orderEvent.Email);
    }
}
