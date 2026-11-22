using EMagaza.Application.Models;

namespace EMagaza.Application.Interfaces.Services;

public interface ICouponService
{
    /// <summary>Kodu doğrular ve bu ara toplam için indirimi hesaplar. Geçersizse DomainException.</summary>
    Task<CouponApplication> ResolveAsync(string code, decimal subtotal);
    Task ClaimAsync(string code);
    Task ReleaseAsync(string code);
}
