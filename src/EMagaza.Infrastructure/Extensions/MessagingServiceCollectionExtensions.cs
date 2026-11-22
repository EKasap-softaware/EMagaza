using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Infrastructure.Messaging;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EMagaza.Infrastructure.Extensions;

public static class MessagingServiceCollectionExtensions
{
    /// <summary>MassTransit + RabbitMQ bus'ı. Web yalnızca gönderir; Worker tüketici ve endpoint ekleyerek çağırır.</summary>
    public static IServiceCollection AddRabbitMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? registerConsumers = null,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configureEndpoints = null)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        var rabbit = configuration.GetSection(RabbitMqOptions.SectionName).Get<RabbitMqOptions>() ?? new RabbitMqOptions();

        services.AddMassTransit(x =>
        {
            registerConsumers?.Invoke(x);
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbit.Host, (ushort)rabbit.Port, "/", h =>
                {
                    h.Username(rabbit.User);
                    h.Password(rabbit.Password);
                });
                configureEndpoints?.Invoke(context, cfg);
            });
        });
        return services;
    }

    /// <summary>Web tarafı: olay yayınlama ve kuyruk izleme.</summary>
    public static IServiceCollection AddOrderEventPublishing(this IServiceCollection services)
    {
        services.AddSingleton<IOrderEventPublisher, MassTransitOrderEventPublisher>();
        services.AddSingleton<IQueueMonitor, RabbitMqQueueMonitor>();
        return services;
    }
}
