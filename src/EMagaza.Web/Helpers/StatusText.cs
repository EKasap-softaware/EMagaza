using EMagaza.Domain.Enums;

namespace EMagaza.Web.Helpers;

public static class StatusText
{
    public static string Label(OrderStatus status) => status switch
    {
        OrderStatus.Paid => "Ödendi",
        OrderStatus.Preparing => "Hazırlanıyor",
        OrderStatus.Shipped => "Kargoda",
        OrderStatus.Delivered => "Teslim edildi",
        OrderStatus.Cancelled => "İptal edildi",
        _ => status.ToString()
    };

    /// <summary>Yönetici butonunda gösterilen bir sonraki adım.</summary>
    public static string NextAction(OrderStatus status) => status switch
    {
        OrderStatus.Paid => "Hazırlamaya al",
        OrderStatus.Preparing => "Kargoya ver",
        OrderStatus.Shipped => "Teslim edildi",
        _ => string.Empty
    };
}
