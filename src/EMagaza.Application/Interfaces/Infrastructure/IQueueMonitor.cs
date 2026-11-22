using EMagaza.Application.Models;

namespace EMagaza.Application.Interfaces.Infrastructure;

public interface IQueueMonitor
{
    /// <summary>RabbitMQ'ya ulaşılamazsa null döner.</summary>
    Task<QueueStatus?> GetStatusAsync();
}
