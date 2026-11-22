using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Domain.Constants;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Services;

public class UserManagementService(IUserRepository users) : IUserManagementService
{
    public async Task ChangeRoleAsync(string actingUserId, string targetUserId, string newRole)
    {
        if (!Roles.All.Contains(newRole)) throw new DomainException("Geçersiz rol.");

        var target = await users.GetByIdAsync(targetUserId) ?? throw new NotFoundException("Kullanıcı bulunamadı.");
        if (target.Role == newRole) return;

        if (target.Id == actingUserId)
            throw new DomainException("Kendi rolünü değiştiremezsin.");

        if (target.Role == Roles.Admin && await users.CountByRoleAsync(Roles.Admin) <= 1)
            throw new DomainException("Son yönetici kaldırılamaz.");

        await users.UpdateRoleAsync(target.Id, newRole);
    }
}
