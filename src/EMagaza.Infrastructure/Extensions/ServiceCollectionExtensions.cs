using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Infrastructure.Payments;
using EMagaza.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EMagaza.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>Web uygulamasının tüm altyapı bağımlılıkları.</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoPersistence(configuration);
        services.AddMongoMigrations();

        services.AddRabbitMessaging(configuration);
        services.AddOrderEventPublishing();

        services.AddSingleton<ICardValidator, CardValidator>();
        services.AddSingleton<IPaymentGateway, SimulatedPaymentGateway>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        return services;
    }
}
