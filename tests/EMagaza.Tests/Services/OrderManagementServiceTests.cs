using EMagaza.Application.Models;
using EMagaza.Application.Services;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Enums;
using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Fakes;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Services;

public class OrderManagementServiceTests
{
    private readonly FakeOrderRepository _orders = new();
    private readonly FakeProductRepository _products = new();
    private readonly FakeCouponRepository _coupons = new();
    private readonly FakeOrderEventPublisher _events = new();
    private readonly SpyPaymentGateway _gateway = new();
    private readonly OrderManagementService _service;
    private readonly Order _order;

    public OrderManagementServiceTests()
    {
        _products.Products.Add(TestData.Shirt(stockM: 4));   // sipariş 1 adet M almıştı: stok 4 kalmıştı
        _service = new OrderManagementService(_orders, _products, _gateway, new CouponService(_coupons), _events);

        _order = TestData.PaidOrder();
        _orders.AddAsync(_order).Wait();
    }

    private string OrderId => _order.Id;

    [Fact]
    public async Task Advance_WalksThroughAllStatuses_PublishingEachEvent()
    {
        await _service.AdvanceAsync(OrderId);
        await _service.AdvanceAsync(OrderId, "TK999");
        await _service.AdvanceAsync(OrderId);

        Assert.Equal(
            new[] { OrderEventKinds.Preparing, OrderEventKinds.Shipped, OrderEventKinds.Delivered },
            _events.Published.Select(e => e.Kind));
        Assert.Equal("TK999", _events.Published[1].TrackingNumber);
    }

    [Fact]
    public async Task Advance_AfterDelivered_Throws()
    {
        for (var i = 0; i < 3; i++) await _service.AdvanceAsync(OrderId);

        await Assert.ThrowsAsync<DomainException>(() => _service.AdvanceAsync(OrderId));
    }

    [Fact]
    public async Task Advance_UnknownOrder_Throws() =>
        await Assert.ThrowsAsync<NotFoundException>(() => _service.AdvanceAsync("yok"));

    [Fact]
    public async Task Cancel_RestoresStock_RefundsPayment_PublishesEvent()
    {
        var cancelled = await _service.CancelAsync(OrderId, "u1", isAdmin: false);

        Assert.Equal(OrderStatus.Cancelled, cancelled.Status);
        Assert.Equal(5, _products.Products[0].StockOf("M"));
        Assert.Equal("tx", Assert.Single(_gateway.Refunded));
        Assert.Equal(OrderEventKinds.Cancelled, Assert.Single(_events.Published).Kind);
    }

    [Fact]
    public async Task Cancel_ReleasesCouponUsage()
    {
        var coupon = new Coupon("SON1", DiscountKind.Fixed, 100, usageLimit: 1);
        coupon.TryUse();
        _coupons.Coupons.Add(coupon);
        _order.ApplyDiscount("SON1", 100);

        await _service.CancelAsync(OrderId, "u1", isAdmin: false);

        Assert.Equal(0, coupon.UsedCount);
    }

    [Fact]
    public async Task Cancel_AfterShipping_Throws_AndChangesNothing()
    {
        await _service.AdvanceAsync(OrderId);
        await _service.AdvanceAsync(OrderId);   // Kargoda
        _events.Published.Clear();

        await Assert.ThrowsAsync<DomainException>(() => _service.CancelAsync(OrderId, "u1", isAdmin: false));

        Assert.Equal(4, _products.Products[0].StockOf("M"));
        Assert.Empty(_gateway.Refunded);
        Assert.Empty(_events.Published);
    }

    [Fact]
    public async Task Cancel_SomeoneElsesOrder_LooksLikeNotFound() =>
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CancelAsync(OrderId, "baskasi", isAdmin: false));

    [Fact]
    public async Task Cancel_AdminCanCancelAnyOrder()
    {
        var cancelled = await _service.CancelAsync(OrderId, "admin-id", isAdmin: true);

        Assert.Equal(OrderStatus.Cancelled, cancelled.Status);
    }

    [Fact]
    public async Task Cancelled_Order_CannotBeAdvanced()
    {
        await _service.CancelAsync(OrderId, "u1", isAdmin: false);

        await Assert.ThrowsAsync<DomainException>(() => _service.AdvanceAsync(OrderId));
    }
}
