using EMagaza.Application.Services;
using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Fakes;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Services;

public class FavoriteServiceTests
{
    private readonly FakeFavoriteRepository _favorites = new();
    private readonly FakeProductRepository _products = new();
    private readonly FavoriteService _service;

    public FavoriteServiceTests()
    {
        _products.Products.Add(TestData.Shirt());
        _service = new FavoriteService(_favorites, _products);
    }

    [Fact]
    public async Task Toggle_AddsThenRemoves()
    {
        Assert.True(await _service.ToggleAsync("u1", "p1"));
        Assert.True(await _service.IsFavoriteAsync("u1", "p1"));

        Assert.False(await _service.ToggleAsync("u1", "p1"));
        Assert.False(await _service.IsFavoriteAsync("u1", "p1"));
    }

    [Fact]
    public async Task Favorites_BelongToTheirOwner()
    {
        await _service.ToggleAsync("u1", "p1");

        Assert.False(await _service.IsFavoriteAsync("u2", "p1"));
        Assert.Empty(await _service.GetProductsAsync("u2"));
    }

    [Fact]
    public async Task Toggle_UnknownProduct_Throws() =>
        await Assert.ThrowsAsync<NotFoundException>(() => _service.ToggleAsync("u1", "yok"));

    [Fact]
    public async Task GetProducts_SkipsDeletedProducts()
    {
        await _service.ToggleAsync("u1", "p1");
        _products.Products.Clear();

        Assert.Empty(await _service.GetProductsAsync("u1"));
    }
}
