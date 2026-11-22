using EMagaza.Application.Formatting;
using EMagaza.Application.Models;
using EMagaza.Domain.Entities;

namespace EMagaza.Application.Notifications;

/// <summary>Bir sipariş olayından bildirim ve e-posta içeriğini üretir. Saf fonksiyon: kaydetme işi tüketicinin.</summary>
public static class OrderMessageBuilder
{
    public static (Notification Notification, EmailLog Email) Build(OrderEvent e)
    {
        var (title, message, subject) = e.Kind switch
        {
            OrderEventKinds.Paid => (
                "Ödemen alındı",
                $"{e.OrderNumber} numaralı siparişin için {Fmt.Money(e.Total)} tutarındaki ödemen başarıyla alındı. Siparişin hazırlanmak üzere sıraya alındı.",
                $"Siparişin alındı ({e.OrderNumber})"),
            OrderEventKinds.Preparing => (
                "Siparişin hazırlanıyor",
                $"{e.OrderNumber} numaralı siparişin özenle paketleniyor.",
                $"Siparişin hazırlanıyor ({e.OrderNumber})"),
            OrderEventKinds.Shipped => (
                "Siparişin kargoya verildi",
                $"{e.OrderNumber} numaralı siparişin yola çıktı. Kargo takip no: {e.TrackingNumber}.",
                $"Siparişin kargoda ({e.OrderNumber})"),
            OrderEventKinds.Delivered => (
                "Siparişin teslim edildi",
                $"{e.OrderNumber} numaralı siparişin teslim edildi. Keyfini çıkar!",
                $"Siparişin teslim edildi ({e.OrderNumber})"),
            OrderEventKinds.Cancelled => (
                "Siparişin iptal edildi",
                $"{e.OrderNumber} numaralı siparişin iptal edildi. {Fmt.Money(e.Total)} tutarındaki ödemen kartına iade edilecek.",
                $"Siparişin iptal edildi ({e.OrderNumber})"),
            _ => throw new ArgumentException($"Bilinmeyen olay türü: {e.Kind}", nameof(e))
        };

        var notification = new Notification(e.UserId, e.OrderId, e.OrderNumber, e.Kind, title, message);
        var email = new EmailLog(e.OrderId, e.Kind, e.Email, subject, $"Merhaba {e.CustomerName}, {message}");
        return (notification, email);
    }
}
