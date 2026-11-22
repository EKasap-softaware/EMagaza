using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void DecreaseStock_OnlyAffectsThatSize()
    {
        var product = TestData.Shirt();
        product.DecreaseStock("M", 2);

        Assert.Equal(3, product.StockOf("M"));
        Assert.Equal(2, product.StockOf("S"));
    }

    [Fact]
    public void DecreaseStock_MoreThanStock_Throws() =>
        Assert.Throws<DomainException>(() => TestData.Shirt().DecreaseStock("S", 3));

    [Fact]
    public void DecreaseStock_UnknownSize_Throws() =>
        Assert.Throws<DomainException>(() => TestData.Shirt().DecreaseStock("XXL", 1));

    [Fact]
    public void WithDiscount_ComputesPercent()
    {
        var product = TestData.Shirt().WithDiscount(1200);

        Assert.True(product.OnSale);
        Assert.Equal(25, product.DiscountPercent);
    }

    [Fact]
    public void WithDiscount_LowerThanPrice_Throws() =>
        Assert.Throws<DomainException>(() => TestData.Shirt().WithDiscount(500));

    [Fact]
    public void AddSize_Duplicate_Throws() =>
        Assert.Throws<DomainException>(() => TestData.Shirt().AddSize("M", 1));

    [Fact]
    public void TotalStock_SumsAllSizes() => Assert.Equal(7, TestData.Shirt().TotalStock);
}
