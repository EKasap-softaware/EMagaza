namespace EMagaza.Domain.Entities;

/// <summary>Gönderilen e-postanın kaydı (gerçek SMTP yerine). Aynı sipariş için her olay türüne bir kayıt.</summary>
public class EmailLog : Entity
{
    public string OrderId { get; private set; } = string.Empty;
    public string Kind { get; private set; } = string.Empty;
    public string To { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public DateTime SentAt { get; private set; } = DateTime.UtcNow;

    private EmailLog() { }

    public EmailLog(string orderId, string kind, string to, string subject, string body)
    {
        OrderId = orderId;
        Kind = kind;
        To = to;
        Subject = subject;
        Body = body;
    }
}
