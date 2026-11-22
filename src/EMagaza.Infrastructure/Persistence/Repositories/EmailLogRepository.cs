using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Repositories;

public class EmailLogRepository(IMongoDbContext context)
    : MongoRepositoryBase<EmailLog>(context.EmailLogs), IEmailLogRepository
{
    // Upsert: kayıt yoksa ekler, varsa günceller. Kuyruk "en az bir kez" teslim eder; tekrar gelen mesaj çift kayıt üretmez.
    public Task UpsertAsync(EmailLog log) =>
        Collection.UpdateOneAsync(
            l => l.OrderId == log.OrderId && l.Kind == log.Kind,
            Builders<EmailLog>.Update
                .Set(l => l.To, log.To)
                .Set(l => l.Subject, log.Subject)
                .Set(l => l.Body, log.Body)
                .Set(l => l.SentAt, log.SentAt),
            new UpdateOptions { IsUpsert = true });

    public async Task<IReadOnlyList<EmailLog>> GetRecentAsync(int count) =>
        await Collection.AsQueryable().OrderByDescending(l => l.SentAt).Take(count).ToListAsync();
}
