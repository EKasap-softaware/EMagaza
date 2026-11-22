using EMagaza.Application.Models;
using EMagaza.Application.Services;
using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Fakes;

namespace EMagaza.Tests.Services;

public class ProductManagementServiceTests
{
    private readonly FakeProductRepository _products = new();
    private readonly ProductManagementService _service;

    public ProductManagementServiceTests() => _service = new ProductManagementService(_products);

    private static ProductInput Input(string name = "Keten Gömlek", decimal price = 1000, decimal oldPrice = 0, params SizeInput[] sizes) =>
        new(name, "LUNE", "Erkek", "shirt", "Beyaz", "#ffffff", price, oldPrice, "Açıklama", "%100 Keten", true,
            sizes.Length > 0 ? sizes : new[] { new SizeInput("M", 5), new SizeInput("L", 3) });

    [Fact]
    public async Task Create_SavesProductWithSizes()
    {
        var product = await _service.CreateAsync(Input());

        Assert.Single(_products.Products);
        Assert.False(string.IsNullOrEmpty(product.Id));
        Assert.Equal(8, product.TotalStock);
        Assert.True(product.IsNew);
    }

    [Fact]
    public async Task Update_ChangesPriceAndReplacesSizes()
    {
        var product = await _service.CreateAsync(Input());

        var updated = await _service.UpdateAsync(product.Id, Input(price: 1200, oldPrice: 1500, sizes: new SizeInput("S", 9)));

        Assert.Equal(1200, updated.Price);
        Assert.True(updated.OnSale);
        Assert.Equal(9, updated.TotalStock);
        Assert.Equal(0, updated.StockOf("M"));
    }

    [Fact]
    public async Task Update_UnknownProduct_Throws() =>
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync("yok", Input()));

    [Fact]
    public async Task Create_InvalidPrice_Throws() =>
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(Input(price: 0)));

    [Fact]
    public async Task Create_DuplicateSize_Throws() =>
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(Input(sizes: new[] { new SizeInput("M", 1), new SizeInput("M", 2) })));

    [Fact]
    public async Task Create_OldPriceNotHigherThanPrice_Throws() =>
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(Input(price: 1000, oldPrice: 900)));

    [Fact]
    public async Task Delete_RemovesProduct()
    {
        var product = await _service.CreateAsync(Input());
        await _service.DeleteAsync(product.Id);

        Assert.Empty(_products.Products);
    }

    [Fact]
    public async Task Delete_UnknownProduct_Throws() =>
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync("yok"));
}
