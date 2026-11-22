using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Repositories;

public interface ICouponRepository
{
    Task<Coupon?> GetByCodeAsync(string code);

    /// <summary>Atomik: limit dolmadıysa kullanım sayısını artırır. İki kişi son hakkı aynı anda alamaz.</summary>
    Task<bool> TryClaimAsync(string code);
    Task ReleaseAsync(string code);
}
