using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Services;

public class FavoriteService(IFavoriteRepository favorites, IProductRepository products) : IFavoriteService
{
    public async Task<IReadOnlyList<Product>> GetProductsAsync(string userId)
    {
        var ids = (await favorites.GetByUserAsync(userId)).OrderByDescending(f => f.CreatedAt).Select(f => f.ProductId).ToList();
        var found = (await products.GetByIdsAsync(ids)).ToDictionary(p => p.Id);

        // Silinmiş ürünler listeden düşer; favori sırası (en yeni önce) korunur.
        return ids.Where(found.ContainsKey).Select(id => found[id]).ToList();
    }

    public Task<bool> IsFavoriteAsync(string userId, string productId) => favorites.ExistsAsync(userId, productId);

    public async Task<bool> ToggleAsync(string userId, string productId)
    {
        if (await products.GetByIdAsync(productId) is null) throw new NotFoundException("Ürün bulunamadı.");

        if (await favorites.ExistsAsync(userId, productId))
        {
            await favorites.RemoveAsync(userId, productId);
            return false;
        }

        await favorites.AddAsync(userId, productId);
        return true;
    }
}
