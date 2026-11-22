using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Application.Models;
using EMagaza.Domain.Enums;

namespace EMagaza.Application.Services;

public class DashboardService(
    IOrderRepository orders,
    IReviewRepository reviews,
    IEmailLogRepository emails,
    IUserRepository users,
    IProductRepository products,
    INotificationRepository notifications,
    IQueueMonitor queue) : IDashboardService
{
    private const int LowStockThreshold = 3;

    public async Task<DashboardSummary> GetSummaryAsync()
    {
        var allOrders = await orders.GetRecentAsync(500);
        var catalog = (await products.SearchAsync(ProductQuery.Everything)).Items;

        return new DashboardSummary
        {
            RecentOrders = allOrders.Take(20).ToList(),
            OrderCount = allOrders.Count,
            Revenue = allOrders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.Total),
            RecentReviews = await reviews.GetRecentAsync(8),
            RecentEmails = await emails.GetRecentAsync(12),
            UserCount = await users.CountAsync(),
            NotificationCount = await notifications.CountAsync(),
            Queue = await queue.GetStatusAsync(),
            LowStock = catalog
                .SelectMany(p => p.Sizes.Where(s => s.Stock <= LowStockThreshold).Select(s => new LowStockItem(p, s)))
                .OrderBy(x => x.Size.Stock)
                .Take(15)
                .ToList()
        };
    }
}
