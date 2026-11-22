using System.Text.Json;
using EMagaza.Application.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
namespace EMagaza.Infrastructure.Messaging;

public record RabbitMqSettings(string Host, int Port);

public sealed class RabbitMqOrderEventPublisher(RabbitMqSettings s, ILogger<RabbitMqOrderEventPublisher> log)
    : IOrderEventPublisher, IAsyncDisposable
{
    IConnection? _conn;
    readonly SemaphoreSlim _lock = new(1, 1);

    // Bağlantı pahalıdır: bir kez açılıp paylaşılır (Singleton). Kanal ise ucuzdur: her yayında yenisi.
    async Task<IConnection> GetConnectionAsync()
    {
        if (_conn is { IsOpen: true }) return _conn;
        await _lock.WaitAsync();
        try
        {
            if (_conn is { IsOpen: true }) return _conn;
            var f = new ConnectionFactory { HostName = s.Host, Port = s.Port, RequestedConnectionTimeout = TimeSpan.FromSeconds(3) };
            return _conn = await f.CreateConnectionAsync();
        }
        finally { _lock.Release(); }
    }

    public async Task PublishOrderPlacedAsync(OrderPlacedEvent evt)
    {
        try
        {
            var conn = await GetConnectionAsync();
            await using var ch = await conn.CreateChannelAsync();
            // durable: RabbitMQ yeniden başlasa da kuyruk silinmez
            await ch.QueueDeclareAsync(OrderPlacedEvent.QueueName, durable: true, exclusive: false, autoDelete: false);
            var body = JsonSerializer.SerializeToUtf8Bytes(evt);
            // Persistent: mesaj diske yazılır
            await ch.BasicPublishAsync("", OrderPlacedEvent.QueueName, false,
                new BasicProperties { Persistent = true, ContentType = "application/json" }, body);
            log.LogInformation("Olay kuyruğa bırakıldı: sipariş #{OrderId}", evt.OrderId);
        }
        catch (Exception ex)
        {
            // Sipariş zaten kaydedildi; kuyruk sorunu müşteriye hata olarak yansımasın.
            // (Kesin çözüm: Transactional Outbox deseni. Bkz. docs/08-rabbitmq.md)
            log.LogError(ex, "Olay kuyruğa bırakılamadı: sipariş #{OrderId}", evt.OrderId);
        }
    }

    public async ValueTask DisposeAsync() { if (_conn is not null) await _conn.DisposeAsync(); }
}
