namespace EMagaza.Domain.Entities;

/// <summary>Kullanıcının bildirim kutusundaki satır. Worker, kuyruktan aldığı olaya göre oluşturur.</summary>
public class Notification : Entity
{
    public string UserId { get; private set; } = string.Empty;
    public string OrderId { get; private set; } = string.Empty;
    public string OrderNumber { get; private set; } = string.Empty;
    public string Kind { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Notification() { }

    public Notification(string userId, string orderId, string orderNumber, string kind, string title, string message)
    {
        UserId = userId;
        OrderId = orderId;
        OrderNumber = orderNumber;
        Kind = kind;
        Title = title;
        Message = message;
    }
}
