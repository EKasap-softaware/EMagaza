using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Application.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EMagaza.Infrastructure.Messaging;

/// <summary>Olayı 'order-events' kuyruğuna gönderir. Kuyruk yoksa MassTransit oluşturur; Worker kapalıyken de mesaj bekler.</summary>
public sealed class MassTransitOrderEventPublisher(IBus bus, ILogger<MassTransitOrderEventPublisher> logger) : IOrderEventPublisher
{
    private static readonly TimeSpan SendTimeout = TimeSpan.FromSeconds(3);

    public async Task PublishAsync(OrderEvent orderEvent)
    {
        try
        {
            using var cts = new CancellationTokenSource(SendTimeout);   // RabbitMQ kapalıysa müşteri beklemesin
            var endpoint = await bus.GetSendEndpoint(new Uri($"queue:{OrderEvent.QueueName}"));
            await endpoint.Send(orderEvent, cts.Token);
        }
        catch (Exception ex)
        {
            // Ödeme ve sipariş zaten kaydedildi; kuyruk hatası müşteriye yansımasın.
            logger.LogError(ex, "Olay kuyruğa bırakılamadı: {Kind} · sipariş {OrderNumber}", orderEvent.Kind, orderEvent.OrderNumber);
        }
    }
}
