using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Infrastructure.Persistence.Migrations;
using EMagaza.Infrastructure.Persistence.Mongo;
using EMagaza.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EMagaza.Infrastructure.Extensions;

public static class PersistenceServiceCollectionExtensions
{
    /// <summary>MongoDB bağlantısı ve repository'ler. Sürücü thread-safe olduğundan hepsi singleton.</summary>
    public static IServiceCollection AddMongoPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.SectionName));
        services.AddSingleton<IMongoDbContext, MongoDbContext>();

        services.AddSingleton<IProductRepository, ProductRepository>();
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddSingleton<IReviewRepository, ReviewRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IEmailLogRepository, EmailLogRepository>();
        services.AddSingleton<INotificationRepository, NotificationRepository>();
        services.AddSingleton<ICouponRepository, CouponRepository>();
        services.AddSingleton<IFavoriteRepository, FavoriteRepository>();
        services.AddSingleton<IAddressRepository, AddressRepository>();
        return services;
    }

    /// <summary>IMigration uygulayan tüm sınıfları bulur ve açılışta bekleyenleri çalıştırır.</summary>
    public static IServiceCollection AddMongoMigrations(this IServiceCollection services)
    {
        var migrationTypes = typeof(IMigration).Assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IMigration).IsAssignableFrom(t));

        foreach (var type in migrationTypes)
            services.AddSingleton(typeof(IMigration), type);

        return services.AddHostedService<MigrationRunner>();
    }
}
