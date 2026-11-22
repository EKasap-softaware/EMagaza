using EMagaza.Application.Models;
using EMagaza.Infrastructure.Extensions;
using EMagaza.Worker.Consumers;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMongoPersistence(builder.Configuration);
builder.Services.AddRabbitMessaging(
    builder.Configuration,
    registerConsumers: x => x.AddConsumer<OrderEventConsumer>(),
    configureEndpoints: (context, bus) => bus.ReceiveEndpoint(OrderEvent.QueueName, endpoint =>
    {
        endpoint.PrefetchCount = 1;
        endpoint.UseMessageRetry(retry =>
        {
            retry.Ignore<ArgumentException>();   // bilinmeyen olay türü: tekrar denemek anlamsız, doğrudan _error kuyruğuna
            retry.Interval(5, TimeSpan.FromSeconds(5));
        });
        endpoint.ConfigureConsumer<OrderEventConsumer>(context);
    }));

await builder.Build().RunAsync();
