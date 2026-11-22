using EMagaza.Application.Carts;
using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Services;

public class CartService(IProductRepository products, ICouponService coupons) : ICartService
{
    public async Task AddItemAsync(Cart cart, string productId, string size, int quantity)
    {
        var product = await products.GetByIdAsync(productId) ?? throw new NotFoundException("Ürün bulunamadı.");
        if (product.StockOf(size) <= 0) throw new DomainException("Lütfen stokta olan bir beden seçin.");

        var room = Math.Min(product.StockOf(size), Cart.MaxPerLine) - cart.QuantityOf(productId, size);
        if (room <= 0) throw new DomainException($"'{product.Name}' ({size}) için daha fazla ekleyemezsin.");

        cart.Add(product, size, Math.Clamp(quantity, 1, room));
        await RefreshCouponAsync(cart);
    }

    public async Task SetQuantityAsync(Cart cart, string productId, string size, int quantity)
    {
        var product = await products.GetByIdAsync(productId);
        if (product is not null) quantity = Math.Min(quantity, product.StockOf(size));

        cart.SetQuantity(productId, size, quantity);
        await RefreshCouponAsync(cart);
    }

    public async Task ApplyCouponAsync(Cart cart, string code)
    {
        if (cart.Lines.Count == 0) throw new DomainException("Önce sepete ürün ekle.");

        var application = await coupons.ResolveAsync(code, cart.Subtotal);
        cart.ApplyCoupon(application.Code, application.Discount);
    }

    public async Task RefreshCouponAsync(Cart cart)
    {
        if (cart.CouponCode is null) return;

        try
        {
            var application = await coupons.ResolveAsync(cart.CouponCode, cart.Subtotal);
            cart.ApplyCoupon(application.Code, application.Discount);
        }
        catch (DomainException)
        {
            cart.ClearCoupon();   // sepet değişti ve kupon artık geçerli değil
        }
    }
}
