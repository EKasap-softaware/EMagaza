using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Repositories;

public class ReviewRepository(IMongoDbContext context)
    : MongoRepositoryBase<Review>(context.Reviews), IReviewRepository
{
    public Task AddAsync(Review review) => InsertAsync(review);

    public async Task<IReadOnlyList<Review>> GetByProductAsync(string productId) =>
        await Collection.AsQueryable()
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<IReadOnlyList<Review>> GetRecentAsync(int count) =>
        await Collection.AsQueryable()
            .OrderByDescending(r => r.CreatedAt)
            .Take(count)
            .ToListAsync();
}
