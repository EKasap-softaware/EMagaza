using EMagaza.Application.Services;
using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Fakes;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Services;

public class ReviewServiceTests
{
    private readonly FakeProductRepository _products = new();
    private readonly ReviewService _service;

    public ReviewServiceTests()
    {
        _products.Products.Add(TestData.Shirt());
        _service = new ReviewService(new FakeReviewRepository(), _products);
    }

    [Fact]
    public async Task Add_UpdatesProductRatingSummary()
    {
        await _service.AddAsync("u1", "Ayşe", "p1", 5, "Harika");
        await _service.AddAsync("u2", "Ali", "p1", 3, "İdare eder");

        Assert.Equal((8, 2), _products.Ratings["p1"]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task Add_InvalidRating_Throws_AndDoesNotTouchSummary(int rating)
    {
        await Assert.ThrowsAsync<DomainException>(() => _service.AddAsync("u1", "Ayşe", "p1", rating, "x"));

        Assert.Empty(_products.Ratings);
    }

    [Fact]
    public async Task Add_UnknownProduct_Throws() =>
        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddAsync("u1", "Ayşe", "yok", 5, "x"));
}
