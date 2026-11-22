using EMagaza.Application.Abstractions;
using EMagaza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace EMagaza.Infrastructure.Persistence;

public class ProductRepository(StoreDbContext db) : IProductRepository
{
    public Task<List<Product>> SearchAsync(string? category, string? text)
    {
        var q = db.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(category)) q = q.Where(p => p.Category == category);
        if (!string.IsNullOrWhiteSpace(text)) q = q.Where(p => p.Name.Contains(text) || p.Description.Contains(text));
        return q.OrderBy(p => p.Name).ToListAsync();
    }
    public Task<Product?> GetAsync(int id) => db.Products.FirstOrDefaultAsync(p => p.Id == id);
    public Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids) => db.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
    public Task<List<string>> GetCategoriesAsync() => db.Products.Select(p => p.Category).Distinct().OrderBy(c => c).ToListAsync();
}

public class OrderRepository(StoreDbContext db) : IOrderRepository
{
    public async Task AddAsync(Order order) => await db.Orders.AddAsync(order);
    public Task<Order?> GetAsync(int id) => db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);
    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
