using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;

namespace EMagaza.Tests.Fakes;

public class FakeFavoriteRepository : IFavoriteRepository
{
    public List<Favorite> Favorites { get; } = new();

    public Task<IReadOnlyList<Favorite>> GetByUserAsync(string userId) =>
        Task.FromResult<IReadOnlyList<Favorite>>(Favorites.Where(f => f.UserId == userId).ToList());

    public Task<bool> ExistsAsync(string userId, string productId) =>
        Task.FromResult(Favorites.Any(f => f.UserId == userId && f.ProductId == productId));

    public Task AddAsync(string userId, string productId)
    {
        if (!Favorites.Any(f => f.UserId == userId && f.ProductId == productId)) Favorites.Add(new Favorite(userId, productId));
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string userId, string productId)
    {
        Favorites.RemoveAll(f => f.UserId == userId && f.ProductId == productId);
        return Task.CompletedTask;
    }
}
