using EMagaza.Application.Models;

namespace EMagaza.Application.Interfaces.Infrastructure;

public interface IOrderEventPublisher
{
    Task PublishAsync(OrderEvent orderEvent);
}
