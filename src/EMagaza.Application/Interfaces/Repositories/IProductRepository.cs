using EMagaza.Application.Models;
using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<PagedResult<Product>> SearchAsync(ProductQuery query);
    Task<Product?> GetByIdAsync(string id);
    Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<string> ids);
    Task<IReadOnlyList<Product>> GetRelatedAsync(Product product, int count);
    Task<IReadOnlyList<string>> GetCategoriesAsync();
    Task<IReadOnlyList<string>> GetTypesAsync(string? category);

    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(string id);

    /// <summary>Atomik: ilgili bedenin stoğu yetiyorsa düşer. İki kişi aynı anda son ürünü alamaz.</summary>
    Task<bool> TryDecreaseStockAsync(string id, string size, int quantity);
    Task IncreaseStockAsync(string id, string size, int quantity);

    /// <summary>Ürünün puan toplamını ve yorum sayısını atomik artırır.</summary>
    Task AddRatingAsync(string id, int rating);
}
