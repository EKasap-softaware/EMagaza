using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Services;

public interface IFavoriteService
{
    Task<IReadOnlyList<Product>> GetProductsAsync(string userId);
    Task<bool> IsFavoriteAsync(string userId, string productId);

    /// <summary>Favoride değilse ekler, favorideyse çıkarır. Sonuçta favori olup olmadığını döner.</summary>
    Task<bool> ToggleAsync(string userId, string productId);
}
