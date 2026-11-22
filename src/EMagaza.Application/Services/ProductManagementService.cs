using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Application.Models;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Services;

public class ProductManagementService(IProductRepository products) : IProductManagementService
{
    public async Task<Product> CreateAsync(ProductInput input)
    {
        var product = new Product(input.Name, input.Brand, input.Category, input.Type, input.ColorName, input.ColorHex, input.Price);
        Apply(product, input);
        await products.AddAsync(product);
        return product;
    }

    public async Task<Product> UpdateAsync(string id, ProductInput input)
    {
        var product = await products.GetByIdAsync(id) ?? throw new NotFoundException("Ürün bulunamadı.");
        Apply(product, input);
        await products.UpdateAsync(product);
        return product;
    }

    public async Task DeleteAsync(string id)
    {
        if (await products.GetByIdAsync(id) is null) throw new NotFoundException("Ürün bulunamadı.");
        await products.DeleteAsync(id);   // geçmiş siparişler ürün bilgisinin kopyasını taşıdığı için bozulmaz
    }

    private static void Apply(Product product, ProductInput input)
    {
        product.UpdateDetails(input.Name, input.Brand, input.Category, input.Type, input.ColorName, input.ColorHex,
            input.Price, input.OldPrice, input.Description, input.Material, input.IsNew);
        product.ReplaceSizes(input.Sizes.Select(s => (s.Size, s.Stock)));
    }
}
