using EMagaza.Application.Carts;

namespace EMagaza.Application.Interfaces.Services;

public interface ICartService
{
    Task AddItemAsync(Cart cart, string productId, string size, int quantity);
    Task SetQuantityAsync(Cart cart, string productId, string size, int quantity);
    Task ApplyCouponAsync(Cart cart, string code);

    /// <summary>Sepet değişince kupon hâlâ geçerli mi diye bakar; değilse kaldırır.</summary>
    Task RefreshCouponAsync(Cart cart);
}
