using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Fakes;

public class FakeOrderRepository : IOrderRepository
{
    public List<Order> Orders { get; } = new();
    public bool FailOnAdd { get; set; }

    public Task AddAsync(Order order)
    {
        if (FailOnAdd) throw new InvalidOperationException("Veritabanı yazma hatası");
        order.WithId(Guid.NewGuid().ToString("N")[..24]);
        Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order) => Task.CompletedTask;
    public Task<Order?> GetByIdAsync(string id) => Task.FromResult(Orders.FirstOrDefault(o => o.Id == id));
    public Task<IReadOnlyList<Order>> GetByUserAsync(string userId) => Task.FromResult<IReadOnlyList<Order>>(Orders.Where(o => o.UserId == userId).ToList());
    public Task<IReadOnlyList<Order>> GetRecentAsync(int count) => Task.FromResult<IReadOnlyList<Order>>(Orders.Take(count).ToList());
}
