using System.Text.RegularExpressions;
using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Models;
using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Repositories;

public class ProductRepository(IMongoDbContext context)
    : MongoRepositoryBase<Product>(context.Products), IProductRepository
{
    public async Task<PagedResult<Product>> SearchAsync(ProductQuery query)
    {
        var products = Collection.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Category))
            products = products.Where(p => p.Category == query.Category);

        if (!string.IsNullOrWhiteSpace(query.Type))
            products = products.Where(p => p.Type == query.Type);

        if (!string.IsNullOrWhiteSpace(query.Size))
            products = products.Where(p => p.Sizes.Any(s => s.Size == query.Size && s.Stock > 0));

        if (query.SaleOnly)
            products = products.Where(p => p.OldPrice > 0);

        if (query.NewOnly)
            products = products.Where(p => p.IsNew);

        if (!string.IsNullOrWhiteSpace(query.Text))
        {
            var pattern = Regex.Escape(query.Text.Trim());
            products = products.Where(p =>
                Regex.IsMatch(p.Name, pattern, RegexOptions.IgnoreCase) ||
                Regex.IsMatch(p.Description, pattern, RegexOptions.IgnoreCase) ||
                Regex.IsMatch(p.Brand, pattern, RegexOptions.IgnoreCase) ||
                Regex.IsMatch(p.ColorName, pattern, RegexOptions.IgnoreCase));
        }

        var totalCount = await products.CountAsync();

        products = query.Sort switch
        {
            ProductSort.PriceAsc => products.OrderBy(p => p.Price),
            ProductSort.PriceDesc => products.OrderByDescending(p => p.Price),
            ProductSort.Newest => products.OrderByDescending(p => p.CreatedAt),
            _ => products.OrderByDescending(p => p.Popularity).ThenBy(p => p.Name)
        };

        var page = Math.Max(1, query.Page);
        var skip = (int)Math.Min((long)(page - 1) * query.PageSize, int.MaxValue);
        var items = await products.Skip(skip).Take(query.PageSize).ToListAsync();

        return new PagedResult<Product>(items, totalCount, page, query.PageSize);
    }

    public Task AddAsync(Product product) => InsertAsync(product);

    public Task UpdateAsync(Product product) => Collection.ReplaceOneAsync(p => p.Id == product.Id, product);

    public async Task DeleteAsync(string id)
    {
        if (IsValidId(id)) await Collection.DeleteOneAsync(p => p.Id == id);
    }

    public async Task AddRatingAsync(string id, int rating)
    {
        if (!IsValidId(id)) return;
        await Collection.UpdateOneAsync(
            p => p.Id == id,
            Builders<Product>.Update.Inc(p => p.RatingSum, rating).Inc(p => p.ReviewCount, 1));
    }

    public Task<Product?> GetByIdAsync(string id) => FindByIdAsync(id);

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<string> ids)
    {
        var validIds = ids.Where(IsValidId).Distinct().ToList();
        if (validIds.Count == 0) return Array.Empty<Product>();

        return await Collection.AsQueryable().Where(p => validIds.Contains(p.Id)).ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetRelatedAsync(Product product, int count) =>
        await Collection.AsQueryable()
            .Where(p => p.Category == product.Category && p.Id != product.Id)
            .OrderByDescending(p => p.Popularity)
            .Take(count)
            .ToListAsync();

    public async Task<IReadOnlyList<string>> GetCategoriesAsync() =>
        await Collection.AsQueryable().Select(p => p.Category).Distinct().OrderBy(c => c).ToListAsync();

    public async Task<IReadOnlyList<string>> GetTypesAsync(string? category)
    {
        var products = Collection.AsQueryable();
        if (!string.IsNullOrWhiteSpace(category)) products = products.Where(p => p.Category == category);

        return await products.Select(p => p.Type).Distinct().OrderBy(t => t).ToListAsync();
    }

    // LINQ sadece okuma içindir; güncelleme Builders ile yapılır.
    // Filtre "o beden VE yeterli stok" şartını tek komutta kontrol edip düşer: iki kişi aynı anda son ürünü alamaz.
    // FirstMatchingElement() = filtrede eşleşen dizi elemanı (MongoDB konumsal operatörü $).
    public async Task<bool> TryDecreaseStockAsync(string id, string size, int quantity)
    {
        if (!IsValidId(id)) return false;

        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.Id, id),
            Builders<Product>.Filter.ElemMatch(p => p.Sizes, s => s.Size == size && s.Stock >= quantity));

        var result = await Collection.UpdateOneAsync(filter, Builders<Product>.Update.Inc(p => p.Sizes.FirstMatchingElement().Stock, -quantity));
        return result.ModifiedCount == 1;
    }

    public async Task IncreaseStockAsync(string id, string size, int quantity)
    {
        if (!IsValidId(id)) return;

        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.Id, id),
            Builders<Product>.Filter.ElemMatch(p => p.Sizes, s => s.Size == size));

        await Collection.UpdateOneAsync(filter, Builders<Product>.Update.Inc(p => p.Sizes.FirstMatchingElement().Stock, quantity));
    }
}
