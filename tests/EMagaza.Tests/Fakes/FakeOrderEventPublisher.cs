using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Application.Models;

namespace EMagaza.Tests.Fakes;

public class FakeOrderEventPublisher : IOrderEventPublisher
{
    public List<OrderEvent> Published { get; } = new();

    public Task PublishAsync(OrderEvent orderEvent)
    {
        Published.Add(orderEvent);
        return Task.CompletedTask;
    }
}
