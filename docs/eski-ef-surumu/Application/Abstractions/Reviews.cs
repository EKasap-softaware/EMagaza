using EMagaza.Domain.Entities;
namespace EMagaza.Application.Abstractions;

public interface IReviewRepository
{
    Task AddAsync(Review review);
    Task<List<Review>> GetByProductAsync(int productId);
}

/// <summary>Sipariş oluştuğunda kuyruğa bırakılan olay (mesaj gövdesi).</summary>
public record OrderPlacedEvent(int OrderId, string CustomerName, string Email, decimal Total)
{
    public const string QueueName = "order-placed";
}

public interface IOrderEventPublisher
{
    Task PublishOrderPlacedAsync(OrderPlacedEvent evt);
}
