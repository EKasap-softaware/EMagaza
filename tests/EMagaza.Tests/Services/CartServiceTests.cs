using EMagaza.Application.Carts;
using EMagaza.Application.Services;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Enums;
using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Fakes;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Services;

public class CartServiceTests
{
    private readonly CartService _service;
    private readonly FakeCouponRepository _coupons = new();

    public CartServiceTests()
    {
        var products = new FakeProductRepository();
        products.Products.Add(TestData.Shirt());   // S:2, M:5, L:0
        _service = new CartService(products, new CouponService(_coupons));
        _coupons.Coupons.Add(new Coupon("HOS10", DiscountKind.Percent, 10));
        _coupons.Coupons.Add(new Coupon("BIN", DiscountKind.Fixed, 50, minSubtotal: 1000));
    }

    [Fact]
    public async Task ApplyCoupon_SetsDiscountOnCart()
    {
        var cart = new Cart();
        await _service.AddItemAsync(cart, "p1", "M", 2);   // 1800 TL
        await _service.ApplyCouponAsync(cart, "hos10");     // küçük harf de çalışır

        Assert.Equal("HOS10", cart.CouponCode);
        Assert.Equal(180, cart.Discount);
        Assert.Equal(1620, cart.Total);   // 1800 - 180, kargo bedava
    }

    [Fact]
    public async Task ApplyCoupon_EmptyCart_Throws() =>
        await Assert.ThrowsAsync<DomainException>(() => _service.ApplyCouponAsync(new Cart(), "HOS10"));

    [Fact]
    public async Task ApplyCoupon_UnknownCode_Throws()
    {
        var cart = TestData.CartWith(TestData.Shirt(), "M", 1);

        await Assert.ThrowsAsync<DomainException>(() => _service.ApplyCouponAsync(cart, "YOK"));
    }

    [Fact]
    public async Task Coupon_IsRemoved_WhenCartNoLongerMeetsMinimum()
    {
        var cart = new Cart();
        await _service.AddItemAsync(cart, "p1", "M", 2);   // 1800 TL
        await _service.ApplyCouponAsync(cart, "BIN");

        await _service.SetQuantityAsync(cart, "p1", "M", 1);   // 900 TL < 1000 TL minimum

        Assert.Null(cart.CouponCode);
        Assert.Equal(0, cart.Discount);
    }

    [Fact]
    public async Task AddItem_ClampsToAvailableStock()
    {
        var cart = new Cart();
        await _service.AddItemAsync(cart, "p1", "S", 5);

        Assert.Equal(2, cart.QuantityOf("p1", "S"));
    }

    [Fact]
    public async Task AddItem_OutOfStockSize_Throws() =>
        await Assert.ThrowsAsync<DomainException>(() => _service.AddItemAsync(new Cart(), "p1", "L", 1));

    [Fact]
    public async Task AddItem_WhenStockAlreadyInCart_Throws()
    {
        var cart = new Cart();
        await _service.AddItemAsync(cart, "p1", "S", 2);

        await Assert.ThrowsAsync<DomainException>(() => _service.AddItemAsync(cart, "p1", "S", 1));
    }

    [Fact]
    public async Task AddItem_UnknownProduct_Throws() =>
        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddItemAsync(new Cart(), "yok", "M", 1));

    [Fact]
    public async Task SetQuantity_NeverExceedsStock()
    {
        var cart = new Cart();
        await _service.AddItemAsync(cart, "p1", "S", 1);
        await _service.SetQuantityAsync(cart, "p1", "S", 9);

        Assert.Equal(2, cart.QuantityOf("p1", "S"));
    }
}
