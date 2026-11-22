using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Services;

public interface IOrderManagementService
{
    /// <summary>Siparişi bir sonraki duruma geçirir ve müşteriye gidecek olayı yayınlar.</summary>
    Task<Order> AdvanceAsync(string orderId, string? trackingNumber = null);

    /// <summary>Siparişi iptal eder: stok geri verilir, ödeme iade edilir, kupon hakkı serbest kalır, müşteriye bildirim gider.</summary>
    Task<Order> CancelAsync(string orderId, string requesterId, bool isAdmin);
}
