using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Repositories;

public class NotificationRepository(IMongoDbContext context)
    : MongoRepositoryBase<Notification>(context.Notifications), INotificationRepository
{
    // SetOnInsert: kayıt zaten varsa (örneğin okunmuşsa) olduğu gibi bırakılır.
    public Task UpsertAsync(Notification n) =>
        Collection.UpdateOneAsync(
            x => x.OrderId == n.OrderId && x.Kind == n.Kind,
            Builders<Notification>.Update
                .SetOnInsert(x => x.UserId, n.UserId)
                .SetOnInsert(x => x.OrderNumber, n.OrderNumber)
                .SetOnInsert(x => x.Title, n.Title)
                .SetOnInsert(x => x.Message, n.Message)
                .SetOnInsert(x => x.IsRead, false)
                .SetOnInsert(x => x.CreatedAt, n.CreatedAt),
            new UpdateOptions { IsUpsert = true });

    public async Task<IReadOnlyList<Notification>> GetByUserAsync(string userId, int count) =>
        await Collection.AsQueryable()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(count)
            .ToListAsync();

    public Task<Notification?> GetByOrderAsync(string userId, string orderId, string kind) =>
        Collection.AsQueryable()
            .FirstOrDefaultAsync(n => n.UserId == userId && n.OrderId == orderId && n.Kind == kind)!;

    public Task<long> CountUnreadAsync(string userId) =>
        Collection.CountDocumentsAsync(n => n.UserId == userId && !n.IsRead);

    public Task<long> CountAsync() => Collection.CountDocumentsAsync(FilterDefinition<Notification>.Empty);

    public Task MarkAllReadAsync(string userId) =>
        Collection.UpdateManyAsync(n => n.UserId == userId && !n.IsRead, Builders<Notification>.Update.Set(n => n.IsRead, true));
}
