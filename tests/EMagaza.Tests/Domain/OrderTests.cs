using EMagaza.Domain.Entities;
using EMagaza.Domain.Enums;
using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Domain;

public class OrderTests
{
    [Theory]
    [InlineData("", "a@b.com", "05551234567", "İst", "adres")]
    [InlineData("Ali", "", "05551234567", "İst", "adres")]
    [InlineData("Ali", "a@b.com", "123", "İst", "adres")]
    [InlineData("Ali", "a@b.com", "05551234567", "", "adres")]
    [InlineData("Ali", "a@b.com", "05551234567", "İst", "")]
    public void InvalidShipping_Throws(string name, string email, string phone, string city, string address) =>
        Assert.Throws<DomainException>(() => new Order("u1", name, email, phone, city, address));

    [Fact]
    public void Number_HasExpectedFormat_AndTotalsIncludeShipping()
    {
        var order = TestData.NewOrder().WithId("507f1f77bcf86cd799439011");
        order.AddItem(TestData.Shirt(), "M", 2);
        order.ApplyShippingFee(59.9m);

        Assert.Matches(@"^EM-\d{6}-39011$", order.Number);
        Assert.Equal(1800, order.Subtotal);
        Assert.Equal(1859.9m, order.Total);
        Assert.Equal(2, order.ItemCount);
    }

    [Fact]
    public void Status_MovesForwardOnly_WithoutSkipping()
    {
        var order = TestData.PaidOrder();

        Assert.Throws<DomainException>(() => order.MoveTo(OrderStatus.Shipped));
        order.MoveTo(OrderStatus.Preparing);
        Assert.Throws<DomainException>(() => order.MoveTo(OrderStatus.Paid));
        order.MoveTo(OrderStatus.Shipped, "TK123");
        order.MoveTo(OrderStatus.Delivered);

        Assert.Equal("TK123", order.TrackingNumber);
        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public void Shipping_WithoutTrackingNumber_GeneratesOne()
    {
        var order = TestData.PaidOrder();
        order.MoveTo(OrderStatus.Preparing);
        order.MoveTo(OrderStatus.Shipped);

        Assert.StartsWith("TK", order.TrackingNumber);
    }

    [Theory]
    [InlineData(OrderStatus.Paid, true)]
    [InlineData(OrderStatus.Preparing, true)]
    [InlineData(OrderStatus.Shipped, false)]
    [InlineData(OrderStatus.Delivered, false)]
    public void CanCancel_OnlyBeforeShipping(OrderStatus status, bool expected)
    {
        var order = TestData.PaidOrder();
        if (status >= OrderStatus.Preparing) order.MoveTo(OrderStatus.Preparing);
        if (status >= OrderStatus.Shipped) order.MoveTo(OrderStatus.Shipped);
        if (status >= OrderStatus.Delivered) order.MoveTo(OrderStatus.Delivered);

        Assert.Equal(expected, order.CanCancel);
    }

    [Fact]
    public void Cancel_SetsStatus_AndCannotBeUndone()
    {
        var order = TestData.PaidOrder();
        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Throws<DomainException>(() => order.Cancel());
        Assert.Throws<DomainException>(() => order.MoveTo(OrderStatus.Preparing));
    }

    [Fact]
    public void MoveTo_Cancelled_IsNotAllowed_UseCancelInstead() =>
        Assert.Throws<DomainException>(() => TestData.PaidOrder().MoveTo(OrderStatus.Cancelled));

    [Fact]
    public void Discount_IsSubtractedFromTotal()
    {
        var order = TestData.NewOrder();
        order.AddItem(TestData.Shirt(), "M", 2);   // 1800
        order.ApplyDiscount("HOS10", 180);
        order.ApplyShippingFee(0);

        Assert.Equal(1620, order.Total);
        Assert.Equal("HOS10", order.CouponCode);
    }

    [Fact]
    public void Discount_CannotExceedSubtotal()
    {
        var order = TestData.NewOrder();
        order.AddItem(TestData.Shirt(), "M", 1);

        Assert.Throws<DomainException>(() => order.ApplyDiscount("X", 901));
    }

    [Fact]
    public void MarkPaid_EmptyOrder_Throws() =>
        Assert.Throws<DomainException>(() => TestData.NewOrder().MarkPaid("Visa", "4242", "tx"));
}
