using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Repositories;

public class OrderRepository(IMongoDbContext context)
    : MongoRepositoryBase<Order>(context.Orders), IOrderRepository
{
    public Task AddAsync(Order order) => InsertAsync(order);

    public Task UpdateAsync(Order order) => Collection.ReplaceOneAsync(o => o.Id == order.Id, order);

    public Task<Order?> GetByIdAsync(string id) => FindByIdAsync(id);

    public async Task<IReadOnlyList<Order>> GetByUserAsync(string userId) =>
        await Collection.AsQueryable()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<IReadOnlyList<Order>> GetRecentAsync(int count) =>
        await Collection.AsQueryable()
            .OrderByDescending(o => o.CreatedAt)
            .Take(count)
            .ToListAsync();
}
