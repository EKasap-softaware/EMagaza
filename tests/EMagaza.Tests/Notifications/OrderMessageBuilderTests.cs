using EMagaza.Application.Formatting;
using EMagaza.Application.Models;
using EMagaza.Application.Notifications;

namespace EMagaza.Tests.Notifications;

public class OrderMessageBuilderTests
{
    private static OrderEvent EventOf(string kind) =>
        new(kind, "oid", "EM-261121-ABCDE", "u1", "Ayşe", "ayse@x.com", 1859.9m, "TK123");

    [Fact]
    public void PaidNotification_ContainsOrderNumberAndAmount_ForTheRightUser()
    {
        var (notification, email) = OrderMessageBuilder.Build(EventOf(OrderEventKinds.Paid));

        Assert.Contains("EM-261121-ABCDE", notification.Message);
        Assert.Contains("1.859,90 ₺", notification.Message);
        Assert.Equal("u1", notification.UserId);
        Assert.False(notification.IsRead);
        Assert.Equal("ayse@x.com", email.To);
        Assert.Contains("EM-261121-ABCDE", email.Subject);
    }

    [Fact]
    public void ShippedNotification_ContainsTrackingNumber() =>
        Assert.Contains("TK123", OrderMessageBuilder.Build(EventOf(OrderEventKinds.Shipped)).Notification.Message);

    [Fact]
    public void CancelledNotification_MentionsRefund()
    {
        var (notification, email) = OrderMessageBuilder.Build(EventOf(OrderEventKinds.Cancelled));

        Assert.Contains("iade", notification.Message);
        Assert.Contains("EM-261121-ABCDE", email.Subject);
    }

    [Fact]
    public void UnknownKind_Throws() =>
        Assert.Throws<ArgumentException>(() => OrderMessageBuilder.Build(EventOf("???")));

    [Theory]
    [InlineData(1299, "1.299 ₺")]
    [InlineData(59.9, "59,90 ₺")]
    [InlineData(1234567.5, "1.234.567,50 ₺")]
    public void Money_UsesTurkishFormat(double amount, string expected) =>
        Assert.Equal(expected, Fmt.Money((decimal)amount));
}
