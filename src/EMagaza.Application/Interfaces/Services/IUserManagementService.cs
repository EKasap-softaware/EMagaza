namespace EMagaza.Application.Interfaces.Services;

public interface IUserManagementService
{
    Task ChangeRoleAsync(string actingUserId, string targetUserId, string newRole);
}
