using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
    Task<Order?> GetByIdAsync(string id);
    Task<IReadOnlyList<Order>> GetByUserAsync(string userId);
    Task<IReadOnlyList<Order>> GetRecentAsync(int count);
}
