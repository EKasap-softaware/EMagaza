using EMagaza.Domain.Entities;

namespace EMagaza.Application.Models;

public class DashboardSummary
{
    public IReadOnlyList<Order> RecentOrders { get; init; } = Array.Empty<Order>();
    public IReadOnlyList<Review> RecentReviews { get; init; } = Array.Empty<Review>();
    public IReadOnlyList<EmailLog> RecentEmails { get; init; } = Array.Empty<EmailLog>();
    public IReadOnlyList<LowStockItem> LowStock { get; init; } = Array.Empty<LowStockItem>();
    public int OrderCount { get; init; }
    public decimal Revenue { get; init; }
    public long UserCount { get; init; }
    public long NotificationCount { get; init; }
    public QueueStatus? Queue { get; init; }
}
