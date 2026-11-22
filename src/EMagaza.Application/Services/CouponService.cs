using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Application.Models;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Services;

public class CouponService(ICouponRepository coupons) : ICouponService
{
    public async Task<CouponApplication> ResolveAsync(string code, decimal subtotal)
    {
        var coupon = await coupons.GetByCodeAsync(Normalize(code)) ?? throw new DomainException("Kupon kodu bulunamadı.");
        return new CouponApplication(coupon.Code, coupon.DiscountFor(subtotal, DateTime.UtcNow));
    }

    public async Task ClaimAsync(string code)
    {
        if (!await coupons.TryClaimAsync(Normalize(code)))
            throw new DomainException("Bu kuponun kullanım limiti doldu.");
    }

    public Task ReleaseAsync(string code) => coupons.ReleaseAsync(Normalize(code));

    private static string Normalize(string code) => (code ?? string.Empty).Trim().ToUpperInvariant();
}
