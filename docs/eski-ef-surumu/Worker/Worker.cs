using System.Text;
using System.Text.Json;
using EMagaza.Application.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace EMagaza.Worker;

/// <summary>
/// Kuyruğu dinleyen arka plan servisi (consumer). Web uygulamasından BAĞIMSIZ bir süreçtir:
/// Web çökse de, bu kapalıyken bile mesajlar kuyrukta bekler ve Worker açılınca işlenir.
/// </summary>
public class Worker(IConfiguration cfg, ILogger<Worker> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName = cfg["RabbitMq:Host"] ?? "localhost",
            Port = int.Parse(cfg["RabbitMq:Port"] ?? "5672")
        };

        // RabbitMQ henüz ayakta değilse (docker compose sırası) pes etme, tekrar dene.
        IConnection conn;
        while (true)
        {
            try { conn = await factory.CreateConnectionAsync(ct); break; }
            catch (Exception ex) when (!ct.IsCancellationRequested)
            {
                log.LogWarning("RabbitMQ'ya bağlanılamadı ({Msg}); 3 sn sonra tekrar", ex.Message);
                await Task.Delay(3000, ct);
            }
        }

        await using var _ = conn;
        await using var ch = await conn.CreateChannelAsync(cancellationToken: ct);
        await ch.QueueDeclareAsync(OrderPlacedEvent.QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
        await ch.BasicQosAsync(0, 1, false, ct);   // aynı anda 1 mesaj: yavaş işleyiciye mesaj yığılmaz

        var consumer = new AsyncEventingBasicConsumer(ch);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var evt = JsonSerializer.Deserialize<OrderPlacedEvent>(Encoding.UTF8.GetString(ea.Body.Span))!;
                await SendConfirmationEmailAsync(evt);
                await ch.BasicAckAsync(ea.DeliveryTag, false, ct);   // ACK: "işlendi, kuyruktan sil"
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Mesaj işlenemedi");
                // requeue:false → bozuk mesaj sonsuz döngüye girmesin. (Gerçekte dead-letter kuyruğuna gider.)
                await ch.BasicNackAsync(ea.DeliveryTag, false, requeue: false, ct);
            }
        };
        await ch.BasicConsumeAsync(OrderPlacedEvent.QueueName, autoAck: false, consumer, ct);   // autoAck:false → mesaj ACK'lenene kadar kaybolmaz

        log.LogInformation("Worker hazır, '{Queue}' kuyruğu dinleniyor", OrderPlacedEvent.QueueName);
        await Task.Delay(Timeout.Infinite, ct).ContinueWith(_ => { });
    }

    // Gerçek e-posta yerine outbox/ klasörüne dosya yazıyoruz: SMTP kurmadan çıktıyı görebilesin.
    async Task SendConfirmationEmailAsync(OrderPlacedEvent e)
    {
        var dir = cfg["OutboxDir"] ?? "outbox";
        Directory.CreateDirectory(dir);
        var text = $"Kime: {e.Email}\nKonu: Siparişin alındı (#{e.OrderId})\n\nMerhaba {e.CustomerName},\n{e.Total:N0} ₺ tutarındaki siparişin alındı. Teşekkürler!\n";
        await File.WriteAllTextAsync(Path.Combine(dir, $"order-{e.OrderId}.txt"), text);
        log.LogInformation("E-posta gönderildi → {Email} (sipariş #{Id})", e.Email, e.OrderId);
    }
}
