using EMagaza.Application.Carts;
using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Carts;

public class CartTests
{
    [Fact]
    public void SameProductDifferentSize_AreSeparateLines()
    {
        var cart = new Cart();
        var shirt = TestData.Shirt();
        cart.Add(shirt, "S", 1);
        cart.Add(shirt, "M", 1);

        Assert.Equal(2, cart.Lines.Count);
    }

    [Fact]
    public void SameProductSameSize_MergesQuantity()
    {
        var cart = TestData.CartWith(TestData.Shirt(), "M", 1);
        cart.Add(TestData.Shirt(), "M", 2);

        Assert.Single(cart.Lines);
        Assert.Equal(3, cart.ItemCount);
        Assert.Equal(2700, cart.Subtotal);
    }

    [Fact]
    public void Shipping_IsFreeAboveThreshold()
    {
        var cart = TestData.CartWith(TestData.Shirt(), "M", 1);
        Assert.Equal(ShippingPolicy.Fee, cart.ShippingFee);

        cart.Add(TestData.Shirt(), "M", 1);   // 1800 TL
        Assert.Equal(0, cart.ShippingFee);
        Assert.Equal(cart.Subtotal, cart.Total);
    }

    [Fact]
    public void SetQuantityToZero_RemovesLine()
    {
        var cart = TestData.CartWith(TestData.Shirt(), "M", 2);
        cart.SetQuantity("p1", "M", 0);

        Assert.Empty(cart.Lines);
    }

    [Fact]
    public void Add_WithoutSize_Throws() =>
        Assert.Throws<DomainException>(() => new Cart().Add(TestData.Shirt(), "", 1));

    [Fact]
    public void Add_NeverExceedsMaxPerLine()
    {
        var cart = TestData.CartWith(TestData.Shirt(), "M", 8);
        cart.Add(TestData.Shirt(), "M", 8);

        Assert.Equal(Cart.MaxPerLine, cart.ItemCount);
    }
}
