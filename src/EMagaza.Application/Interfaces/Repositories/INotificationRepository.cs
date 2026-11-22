using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    /// <summary>(sipariş, tür) başına tek bildirim: aynı mesaj iki kez işlense de çift bildirim oluşmaz.</summary>
    Task UpsertAsync(Notification notification);
    Task<IReadOnlyList<Notification>> GetByUserAsync(string userId, int count);
    Task<Notification?> GetByOrderAsync(string userId, string orderId, string kind);
    Task<long> CountUnreadAsync(string userId);
    Task<long> CountAsync();
    Task MarkAllReadAsync(string userId);
}
