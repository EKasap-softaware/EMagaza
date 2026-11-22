using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;

namespace EMagaza.Infrastructure.Persistence.Migrations;

/// <summary>
/// E-posta ve bildirim kayıtları (sipariş, olay türü) çiftiyle tekil olur: kuyruk aynı mesajı iki kez teslim etse de tek kayıt oluşur.
/// Eski sürümde e-posta indeksi sadece OrderId üzerindeydi; yeni olay türleriyle çakıştığı için kaldırılır.
/// </summary>
public class M002OrderEventUniqueIndexes : IMigration
{
    private const string LegacyEmailIndex = "OrderId_1";

    public int Version => 2;
    public string Description => "E-posta/bildirim için (OrderId, Kind) tekil indeksleri; eski OrderId indeksini kaldır";

    public async Task UpAsync(IMongoDbContext context)
    {
        var existing = await (await context.EmailLogs.Indexes.ListAsync()).ToListAsync();
        if (existing.Any(i => i["name"] == LegacyEmailIndex))
            await context.EmailLogs.Indexes.DropOneAsync(LegacyEmailIndex);

        await context.EmailLogs.Indexes.CreateOneAsync(new CreateIndexModel<EmailLog>(
            Builders<EmailLog>.IndexKeys.Ascending(l => l.OrderId).Ascending(l => l.Kind),
            new CreateIndexOptions { Unique = true }));

        await context.Notifications.Indexes.CreateOneAsync(new CreateIndexModel<Notification>(
            Builders<Notification>.IndexKeys.Ascending(n => n.OrderId).Ascending(n => n.Kind),
            new CreateIndexOptions { Unique = true }));
    }
}
