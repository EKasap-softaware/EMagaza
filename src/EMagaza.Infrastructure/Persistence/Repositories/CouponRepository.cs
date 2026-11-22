using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Repositories;

public class CouponRepository(IMongoDbContext context)
    : MongoRepositoryBase<Coupon>(context.Coupons), ICouponRepository
{
    public Task<Coupon?> GetByCodeAsync(string code) =>
        Collection.AsQueryable().FirstOrDefaultAsync(c => c.Code == code)!;

    // Tek komutta "limit dolmadıysa say": iki kişi son hakkı aynı anda alamaz.
    public async Task<bool> TryClaimAsync(string code)
    {
        var result = await Collection.UpdateOneAsync(
            c => c.Code == code && c.IsActive && (c.UsageLimit == 0 || c.UsedCount < c.UsageLimit),
            Builders<Coupon>.Update.Inc(c => c.UsedCount, 1));

        return result.ModifiedCount == 1;
    }

    public Task ReleaseAsync(string code) =>
        Collection.UpdateOneAsync(c => c.Code == code && c.UsedCount > 0, Builders<Coupon>.Update.Inc(c => c.UsedCount, -1));
}
