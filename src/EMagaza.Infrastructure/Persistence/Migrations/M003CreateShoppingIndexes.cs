using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;

namespace EMagaza.Infrastructure.Persistence.Migrations;

public class M003CreateShoppingIndexes : IMigration
{
    public int Version => 3;
    public string Description => "Kupon (tekil kod), favori (tekil kullanıcı+ürün) ve adres indeksleri";

    public async Task UpAsync(IMongoDbContext context)
    {
        await context.Coupons.Indexes.CreateOneAsync(new CreateIndexModel<Coupon>(
            Builders<Coupon>.IndexKeys.Ascending(c => c.Code), new CreateIndexOptions { Unique = true }));

        await context.Favorites.Indexes.CreateOneAsync(new CreateIndexModel<Favorite>(
            Builders<Favorite>.IndexKeys.Ascending(f => f.UserId).Ascending(f => f.ProductId),
            new CreateIndexOptions { Unique = true }));

        await context.Addresses.Indexes.CreateOneAsync(new CreateIndexModel<Address>(
            Builders<Address>.IndexKeys.Ascending(a => a.UserId)));
    }
}
