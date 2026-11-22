using EMagaza.Application.Exceptions;
using EMagaza.Application.Models;
using EMagaza.Application.Services;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Enums;
using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Fakes;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Services;

public class CheckoutServiceTests
{
    private readonly FakeProductRepository _products = new();
    private readonly FakeOrderRepository _orders = new();
    private readonly FakeOrderEventPublisher _events = new();
    private readonly SpyPaymentGateway _gateway = new();
    private readonly FakeCouponRepository _coupons = new();
    private readonly CheckoutService _service;

    public CheckoutServiceTests()
    {
        _products.Products.Add(TestData.Shirt());
        _coupons.Coupons.Add(new Coupon("HOS10", DiscountKind.Percent, 10));
        _coupons.Coupons.Add(new Coupon("SON1", DiscountKind.Fixed, 100, usageLimit: 1));
        _service = new CheckoutService(_products, _orders, _gateway, new CouponService(_coupons), _events);
    }

    [Fact]
    public async Task Coupon_ReducesTotal_AndIsRecordedOnOrder()
    {
        var cart = TestData.CartWith(_products.Products[0], "M", 2);   // 1800 TL
        cart.ApplyCoupon("HOS10", 180);

        var order = await _service.PlaceOrderAsync(cart, TestData.Shipping, TestData.GoodCard, "u1");

        Assert.Equal("HOS10", order.CouponCode);
        Assert.Equal(180, order.DiscountAmount);
        Assert.Equal(1620, order.Total);
        Assert.Equal(1620, Assert.Single(_gateway.Charged));
        Assert.Equal(1, _coupons.Coupons[0].UsedCount);
    }

    [Fact]
    public async Task Coupon_DiscountIsRecalculated_NotTrustedFromCart()
    {
        var cart = TestData.CartWith(_products.Products[0], "M", 2);
        cart.ApplyCoupon("HOS10", 1500);   // sepet kurcalandı: gerçek indirim 180

        var order = await _service.PlaceOrderAsync(cart, TestData.Shipping, TestData.GoodCard, "u1");

        Assert.Equal(180, order.DiscountAmount);
    }

    [Fact]
    public async Task Coupon_WithExhaustedLimit_FailsAndRestoresStock()
    {
        _coupons.Coupons[1].TryUse();   // tek hak zaten kullanıldı
        var cart = TestData.CartWith(_products.Products[0], "M", 1);
        cart.ApplyCoupon("SON1", 100);

        await Assert.ThrowsAsync<DomainException>(() => _service.PlaceOrderAsync(cart, TestData.Shipping, TestData.GoodCard, "u1"));

        Assert.Equal(5, StockOfM);
        Assert.Empty(_gateway.Charged);
    }

    [Fact]
    public async Task DeclinedCard_ReleasesClaimedCoupon()
    {
        var cart = TestData.CartWith(_products.Products[0], "M", 1);
        cart.ApplyCoupon("SON1", 100);

        await Assert.ThrowsAsync<PaymentDeclinedException>(() => _service.PlaceOrderAsync(cart, TestData.Shipping, TestData.DeclinedCard, "u1"));

        Assert.Equal(0, _coupons.Coupons[1].UsedCount);   // hak geri verildi
    }

    private int StockOfM => _products.Products[0].StockOf("M");

    [Fact]
    public async Task Success_ReservesStock_ChargesTotal_SavesPaidOrder_PublishesEvent()
    {
        var order = await _service.PlaceOrderAsync(TestData.CartWith(_products.Products[0], "M", 2), TestData.Shipping, TestData.GoodCard, "u1");

        Assert.Equal(3, StockOfM);
        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal("4242", order.PaymentLast4);
        Assert.Equal(order.Total, Assert.Single(_gateway.Charged));

        var published = Assert.Single(_events.Published);
        Assert.Equal(OrderEventKinds.Paid, published.Kind);
        Assert.Equal(order.Number, published.OrderNumber);
        Assert.Equal("u1", published.UserId);
    }

    [Fact]
    public async Task Price_ComesFromCatalog_NotFromCart()
    {
        var cart = TestData.CartWith(_products.Products[0], "M", 1);
        cart.Lines[0] = cart.Lines[0] with { UnitPrice = 1 };   // sepet kurcalandı

        var order = await _service.PlaceOrderAsync(cart, TestData.Shipping, TestData.GoodCard, "u1");

        Assert.Equal(900, order.Subtotal);
    }

    [Fact]
    public async Task EmptyCart_Throws() =>
        await Assert.ThrowsAsync<DomainException>(() => _service.PlaceOrderAsync(new(), TestData.Shipping, TestData.GoodCard, "u1"));

    [Fact]
    public async Task OutOfStockSize_Throws_WithoutCharging()
    {
        var cart = TestData.CartWith(_products.Products[0], "L", 1);

        await Assert.ThrowsAsync<DomainException>(() => _service.PlaceOrderAsync(cart, TestData.Shipping, TestData.GoodCard, "u1"));

        Assert.Empty(_gateway.Charged);
        Assert.Empty(_orders.Orders);
        Assert.Empty(_events.Published);
    }

    [Fact]
    public async Task DeclinedCard_RestoresStock_SavesNothing()
    {
        var cart = TestData.CartWith(_products.Products[0], "M", 2);

        await Assert.ThrowsAsync<PaymentDeclinedException>(() => _service.PlaceOrderAsync(cart, TestData.Shipping, TestData.DeclinedCard, "u1"));

        Assert.Equal(5, StockOfM);
        Assert.Empty(_orders.Orders);
        Assert.Empty(_events.Published);
    }

    [Fact]
    public async Task DatabaseFailsAfterCharge_RefundsPaymentAndRestoresStock()
    {
        _orders.FailOnAdd = true;
        var cart = TestData.CartWith(_products.Products[0], "M", 2);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.PlaceOrderAsync(cart, TestData.Shipping, TestData.GoodCard, "u1"));

        Assert.Single(_gateway.Refunded);
        Assert.Equal(5, StockOfM);
        Assert.Empty(_events.Published);
    }

    [Fact]
    public async Task InvalidShipping_Throws_BeforeTouchingStockOrCard()
    {
        var invalid = TestData.Shipping with { Phone = "1" };
        var cart = TestData.CartWith(_products.Products[0], "M", 1);

        await Assert.ThrowsAsync<DomainException>(() => _service.PlaceOrderAsync(cart, invalid, TestData.GoodCard, "u1"));

        Assert.Equal(5, StockOfM);
        Assert.Empty(_gateway.Charged);
    }
}
