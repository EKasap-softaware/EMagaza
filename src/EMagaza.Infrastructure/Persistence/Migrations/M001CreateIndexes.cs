using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;

namespace EMagaza.Infrastructure.Persistence.Migrations;

public class M001CreateIndexes : IMigration
{
    public int Version => 1;
    public string Description => "Koleksiyon indekslerini oluştur";

    public async Task UpAsync(IMongoDbContext context)
    {
        await context.Products.Indexes.CreateOneAsync(new CreateIndexModel<Product>(
            Builders<Product>.IndexKeys.Ascending(p => p.Category).Ascending(p => p.Type)));

        await context.Reviews.Indexes.CreateOneAsync(new CreateIndexModel<Review>(
            Builders<Review>.IndexKeys.Ascending(r => r.ProductId)));

        await context.Orders.Indexes.CreateOneAsync(new CreateIndexModel<Order>(
            Builders<Order>.IndexKeys.Ascending(o => o.UserId)));

        // Aynı e-postayla iki kullanıcıyı veritabanı seviyesinde engeller.
        await context.Users.Indexes.CreateOneAsync(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email), new CreateIndexOptions { Unique = true }));

        await context.Notifications.Indexes.CreateOneAsync(new CreateIndexModel<Notification>(
            Builders<Notification>.IndexKeys.Ascending(n => n.UserId)));
    }
}
