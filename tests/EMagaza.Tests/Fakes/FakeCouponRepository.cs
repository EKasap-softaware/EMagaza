using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;

namespace EMagaza.Tests.Fakes;

public class FakeCouponRepository : ICouponRepository
{
    public List<Coupon> Coupons { get; } = new();

    public Task<Coupon?> GetByCodeAsync(string code) => Task.FromResult(Coupons.FirstOrDefault(c => c.Code == code));

    public Task<bool> TryClaimAsync(string code) => Task.FromResult(Coupons.FirstOrDefault(c => c.Code == code)?.TryUse() ?? false);

    public Task ReleaseAsync(string code)
    {
        Coupons.FirstOrDefault(c => c.Code == code)?.ReleaseUse();
        return Task.CompletedTask;
    }
}
