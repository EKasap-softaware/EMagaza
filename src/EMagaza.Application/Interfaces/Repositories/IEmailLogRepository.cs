using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Repositories;

public interface IEmailLogRepository
{
    /// <summary>(sipariş, tür) başına tek kayıt: aynı mesaj iki kez işlense de çift e-posta oluşmaz.</summary>
    Task UpsertAsync(EmailLog log);
    Task<IReadOnlyList<EmailLog>> GetRecentAsync(int count);
}
