# 08 · RabbitMQ (sipariş olayı → e-posta)

## Sorun ne? Neden kuyruk?
Sipariş verince e-posta gitmeli. Doğrudan gönderseydik: SMTP yavaşsa müşteri bekler, SMTP çökerse sipariş hata verir.
Kuyrukla: Web sadece **"sipariş #3 alındı"** mesajını bırakır ve hemen döner. Ayrı bir **Worker** mesajı alıp e-postayı kendi hızında gönderir.
> Benzetme: Garson siparişi mutfağa kâğıtla asar (kuyruk) ve müşteriye döner; aşçı hazır olunca kâğıdı alır. Garson aşçıyı beklemez.

```
Web (producer) ──mesaj──► [ order-placed kuyruğu ] ──► Worker (consumer) ──► outbox/order-3.txt
```
Kazanımlar: **ayrışma** (Web, Worker'ı bilmez), **dayanıklılık** (Worker kapalıyken mesaj bekler), **yük dengeleme** (birden çok Worker aynı kuyruğu paylaşır).

## Kavramlar
| Terim | Anlam |
|---|---|
| Producer | Mesajı bırakan (Web) |
| Consumer | Mesajı alan (Worker) |
| Queue | Mesajların sırada beklediği yer (`order-placed`) |
| Exchange | Mesajı kuyruklara yönlendiren dağıtıcı. Biz `""` (default) kullandık: routing key = kuyruk adı |
| ACK | "İşledim, kuyruktan sil" onayı |
| Durable / Persistent | Kuyruk / mesaj RabbitMQ yeniden başlayınca da durur |

## Kodda neresi
| Dosya | Görev |
|---|---|
| `Application/Abstractions/Reviews.cs` | `OrderPlacedEvent` (mesaj gövdesi) + `IOrderEventPublisher` |
| `Application/Orders/CheckoutService.cs` | Sipariş kaydedilir, **sonra** olay yayınlanır |
| `Infrastructure/Messaging/RabbitMq.cs` | Producer (RabbitMQ.Client 7) |
| `src/EMagaza.Worker/Worker.cs` | Consumer (BackgroundService) |
| `docker-compose.yml` | `rabbitmq` + `worker` servisleri |
Application RabbitMQ'yu bilmez, sadece `IOrderEventPublisher`'ı bilir; testte `FakePublisher` kullandık.

## Producer'ın kritik satırları
```csharp
await ch.QueueDeclareAsync("order-placed", durable: true, exclusive: false, autoDelete: false);
await ch.BasicPublishAsync("", "order-placed", false,
        new BasicProperties { Persistent = true }, body);     // body = JSON byte[]
```
- **Bağlantı Singleton, kanal her yayında yeni.** Bağlantı (TCP) pahalı, kanal ucuz.
- `QueueDeclare` her iki tarafta da var: kim önce başlarsa kuyruğu oluşturur (idempotent).
- Hata `catch` ile yutulup loglanır: **sipariş kaydedildi, e-posta gecikse de sipariş geçerli.**

## Consumer'ın kritik satırları
```csharp
await ch.BasicQosAsync(0, 1, false);                       // aynı anda 1 mesaj (prefetch)
await ch.BasicConsumeAsync(queue, autoAck: false, consumer);   // elle ACK
... işle ...
await ch.BasicAckAsync(ea.DeliveryTag, false);             // başarı → sil
await ch.BasicNackAsync(ea.DeliveryTag, false, requeue: false); // hata → at
```
**`autoAck: false` neden önemli?** `true` olsa mesaj alınır alınmaz silinir; Worker tam e-posta yazarken çökerse mesaj sonsuza dek kaybolur. Elle ACK ile: ACK gelmediyse RabbitMQ mesajı başkasına verir (**at-least-once**: bir mesaj *birden fazla* işlenebilir → işleyici idempotent olmalı).
`requeue: false` bozuk mesajın sonsuz döngüye girmesini önler; gerçekte *dead-letter queue*'ya yönlendirilir.
Worker, RabbitMQ henüz açılmamışsa 3 sn arayla yeniden dener (canlıda görüldü: "bağlanılamadı… tekrar", sonra "Worker hazır").

## Doğrulanan akış
Siparişten sonra: Web logu `Olay kuyruğa bırakıldı: sipariş #3` → Worker logu `E-posta gönderildi → test@example.com (sipariş #3)` → `outbox/order-3.txt` oluştu → `rabbitmqctl list_queues`: `order-placed  0 mesaj  1 consumer`.

## Bilinen zayıflık (mülakatta anlat)
Sipariş DB'ye yazıldı ama olay yayınlanamadan Web çökerse e-posta **hiç gitmez** (iki ayrı sistem tek transaction'da değil). Gerçek çözüm: **Transactional Outbox** — olayı aynı DB transaction'ında bir `Outbox` tablosuna yaz, ayrı süreç oradan RabbitMQ'ya aktarsın. Burada uygulamadım.

## Komutlar
```powershell
docker compose up -d rabbitmq
# Yönetim paneli: http://localhost:15673  (kullanıcı/şifre: guest/guest)
docker exec emagaza-rabbitmq-1 rabbitmqctl list_queues name messages consumers
docker compose stop worker      # siparişler ver → mesajlar birikir (panelde "Ready" artar)
docker compose start worker     # birikenler işlenir
```
## Sık hatalar
- Container içinde `localhost` kendisidir: `RabbitMq__Host=rabbitmq` (servis adı), port **5672** (5673 yalnızca host tarafı eşlemesi).
- Mesajı `Persistent` yapmadan "dayanıklı kuyruk" yeterli değil: kuyruk kalır, mesaj gider.
- ACK'i unutmak: mesajlar "Unacked"da asılı kalır, Worker yeniden başlayınca tekrar işlenir.
## Mülakat soruları
Kuyruk neden? Exchange türleri (direct/fanout/topic)? ACK/NACK? At-least-once vs at-most-once vs exactly-once? Idempotency neden? Dead-letter queue? Outbox deseni? RabbitMQ ile Kafka farkı?
