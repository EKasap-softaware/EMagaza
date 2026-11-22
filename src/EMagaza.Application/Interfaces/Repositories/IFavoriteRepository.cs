using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Repositories;

public interface IFavoriteRepository
{
    Task<IReadOnlyList<Favorite>> GetByUserAsync(string userId);
    Task<bool> ExistsAsync(string userId, string productId);
    Task AddAsync(string userId, string productId);
    Task RemoveAsync(string userId, string productId);
}
