using EMagaza.Application.Abstractions;
using EMagaza.Application.Orders;
using EMagaza.Infrastructure.Messaging;
using EMagaza.Infrastructure.Mongo;
using EMagaza.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
namespace EMagaza.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection s, string sqlite,
        MongoSettings mongo, RabbitMqSettings rabbit)
    {
        // SQL (SQLite): ürünler + siparişler
        s.AddDbContext<StoreDbContext>(o => o.UseSqlite(sqlite));
        s.AddScoped<IProductRepository, ProductRepository>();
        s.AddScoped<IOrderRepository, OrderRepository>();

        // MongoDB: yorumlar. İstemci Singleton (içinde bağlantı havuzu var). Kısa zaman aşımı: Mongo kapalıysa sayfa donmasın.
        s.AddSingleton(mongo);
        s.AddSingleton<IMongoClient>(_ =>
        {
            var cfg = MongoClientSettings.FromConnectionString(mongo.ConnectionString);
            cfg.ServerSelectionTimeout = TimeSpan.FromSeconds(2);
            return new MongoClient(cfg);
        });
        s.AddScoped<IReviewRepository, MongoReviewRepository>();

        // RabbitMQ: olay yayıncısı (Singleton: bağlantıyı paylaşır)
        s.AddSingleton(rabbit);
        s.AddSingleton<IOrderEventPublisher, RabbitMqOrderEventPublisher>();

        s.AddScoped<CheckoutService>();
        return s;
    }
}
