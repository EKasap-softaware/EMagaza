using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Services;

/// <summary>
/// Bir siparişin stok rezervasyonları. MongoDB'de (tek sunucuda) çok belgeli transaction olmadığı için
/// hata durumunda düşülen stoklar <see cref="ReleaseAsync"/> ile geri verilir (telafi).
/// </summary>
internal sealed class StockReservation(IProductRepository products)
{
    private readonly List<(string ProductId, string Size, int Quantity)> _reserved = new();

    public async Task ReserveAsync(Product product, string size, int quantity)
    {
        product.DecreaseStock(size, quantity);   // bellekteki kopyada hızlı ön kontrol

        if (!await products.TryDecreaseStockAsync(product.Id, size, quantity))
            throw new DomainException($"'{product.Name}' ({size}) için yeterli stok yok.");

        _reserved.Add((product.Id, size, quantity));
    }

    public async Task ReleaseAsync()
    {
        foreach (var (productId, size, quantity) in _reserved)
            await products.IncreaseStockAsync(productId, size, quantity);
        _reserved.Clear();
    }
}
