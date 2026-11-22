using EMagaza.Domain.Entities;
namespace EMagaza.Application.Abstractions;

// Application katmanı SADECE arayüzü bilir; EF Core'dan habersizdir (Dependency Inversion).
public interface IProductRepository
{
    Task<List<Product>> SearchAsync(string? category, string? text);
    Task<Product?> GetAsync(int id);
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids);
    Task<List<string>> GetCategoriesAsync();
}

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetAsync(int id);
    Task SaveChangesAsync();   // ürün stoğu + sipariş TEK transaction'da kaydedilir
}
