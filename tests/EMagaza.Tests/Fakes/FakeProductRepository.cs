using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Models;
using EMagaza.Domain.Entities;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Fakes;

public class FakeProductRepository : IProductRepository
{
    public List<Product> Products { get; } = new();
    public Dictionary<string, (int Sum, int Count)> Ratings { get; } = new();

    public Task<PagedResult<Product>> SearchAsync(ProductQuery query)
    {
        var page = Math.Max(1, query.Page);
        var items = Products.Skip((page - 1) * query.PageSize).Take(query.PageSize).ToList();
        return Task.FromResult(new PagedResult<Product>(items, Products.Count, page, query.PageSize));
    }

    public Task<Product?> GetByIdAsync(string id) => Task.FromResult(Products.FirstOrDefault(p => p.Id == id));
    public Task<IReadOnlyList<Product>> GetRelatedAsync(Product product, int count) => Task.FromResult<IReadOnlyList<Product>>(Array.Empty<Product>());
    public Task<IReadOnlyList<string>> GetCategoriesAsync() => Task.FromResult<IReadOnlyList<string>>(Products.Select(p => p.Category).Distinct().ToList());
    public Task<IReadOnlyList<string>> GetTypesAsync(string? category) => Task.FromResult<IReadOnlyList<string>>(Products.Select(p => p.Type).Distinct().ToList());

    // Gerçek veritabanı gibi KOPYA döner: servis kendi kopyasında stok düşer, "veritabanı" ayrıca atomik güncellenir.
    public Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<string> ids) =>
        Task.FromResult<IReadOnlyList<Product>>(Products.Where(p => ids.Contains(p.Id)).Select(Clone).ToList());

    public Task AddAsync(Product product)
    {
        product.WithId(Guid.NewGuid().ToString("N")[..24]);
        Products.Add(product);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Product product)
    {
        var index = Products.FindIndex(p => p.Id == product.Id);
        if (index >= 0) Products[index] = product;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string id)
    {
        Products.RemoveAll(p => p.Id == id);
        return Task.CompletedTask;
    }

    public Task AddRatingAsync(string id, int rating)
    {
        var current = Ratings.GetValueOrDefault(id);
        Ratings[id] = (current.Sum + rating, current.Count + 1);
        return Task.CompletedTask;
    }

    public Task<bool> TryDecreaseStockAsync(string id, string size, int quantity)
    {
        var sizeStock = Products.First(p => p.Id == id).Sizes.FirstOrDefault(s => s.Size == size);
        if (sizeStock is null || sizeStock.Stock < quantity) return Task.FromResult(false);

        sizeStock.Decrease(quantity);
        return Task.FromResult(true);
    }

    public Task IncreaseStockAsync(string id, string size, int quantity)
    {
        Products.First(p => p.Id == id).Sizes.First(s => s.Size == size).Increase(quantity);
        return Task.CompletedTask;
    }

    private static Product Clone(Product p)
    {
        var copy = new Product(p.Name, p.Brand, p.Category, p.Type, p.ColorName, p.ColorHex, p.Price);
        foreach (var s in p.Sizes) copy.AddSize(s.Size, s.Stock);
        return copy.WithId(p.Id);
    }
}
