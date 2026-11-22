using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Repositories;

public class FavoriteRepository(IMongoDbContext context)
    : MongoRepositoryBase<Favorite>(context.Favorites), IFavoriteRepository
{
    public async Task<IReadOnlyList<Favorite>> GetByUserAsync(string userId) =>
        await Collection.AsQueryable().Where(f => f.UserId == userId).OrderByDescending(f => f.CreatedAt).ToListAsync();

    public Task<bool> ExistsAsync(string userId, string productId) =>
        Collection.AsQueryable().AnyAsync(f => f.UserId == userId && f.ProductId == productId);

    // Upsert: çift tıklamada ikinci kayıt oluşmaz (ayrıca (UserId, ProductId) tekil indeksi var).
    public Task AddAsync(string userId, string productId) =>
        Collection.UpdateOneAsync(
            f => f.UserId == userId && f.ProductId == productId,
            Builders<Favorite>.Update.SetOnInsert(f => f.CreatedAt, DateTime.UtcNow),
            new UpdateOptions { IsUpsert = true });

    public Task RemoveAsync(string userId, string productId) =>
        Collection.DeleteOneAsync(f => f.UserId == userId && f.ProductId == productId);
}
