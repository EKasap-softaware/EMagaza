using EMagaza.Domain.Entities;
using EMagaza.Domain.Enums;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Tests.Domain;

public class CouponTests
{
    private static readonly DateTime Now = new(2026, 11, 22, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Percent_IsCalculatedFromSubtotal() =>
        Assert.Equal(150, new Coupon("p15", DiscountKind.Percent, 15).DiscountFor(1000, Now));

    [Fact]
    public void Fixed_NeverExceedsSubtotal() =>
        Assert.Equal(80, new Coupon("f", DiscountKind.Fixed, 200).DiscountFor(80, Now));

    [Fact]
    public void Code_IsNormalizedToUpperCase() =>
        Assert.Equal("YAZ10", new Coupon(" yaz10 ", DiscountKind.Percent, 10).Code);

    [Fact]
    public void BelowMinimumSubtotal_Throws() =>
        Assert.Throws<DomainException>(() => new Coupon("min", DiscountKind.Fixed, 50, minSubtotal: 500).DiscountFor(499, Now));

    [Fact]
    public void Expired_Throws() =>
        Assert.Throws<DomainException>(() => new Coupon("old", DiscountKind.Fixed, 50, expiresAt: Now.AddDays(-1)).DiscountFor(1000, Now));

    [Fact]
    public void UsageLimit_BlocksAfterLastUse()
    {
        var coupon = new Coupon("one", DiscountKind.Fixed, 50, usageLimit: 1);

        Assert.True(coupon.TryUse());
        Assert.False(coupon.TryUse());
        Assert.Throws<DomainException>(() => coupon.DiscountFor(1000, Now));
    }

    [Fact]
    public void ReleaseUse_GivesTheRightBack()
    {
        var coupon = new Coupon("one", DiscountKind.Fixed, 50, usageLimit: 1);
        coupon.TryUse();
        coupon.ReleaseUse();

        Assert.True(coupon.TryUse());
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData("x", 0)]
    [InlineData("x", 101)]
    public void InvalidDefinition_Throws(string code, decimal value) =>
        Assert.Throws<DomainException>(() => new Coupon(code, DiscountKind.Percent, value));
}
