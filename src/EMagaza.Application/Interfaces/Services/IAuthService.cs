using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Services;

public interface IAuthService
{
    Task<User> RegisterAsync(string displayName, string email, string password);

    /// <summary>Başarılıysa kullanıcı, değilse null. E-posta mı şifre mi yanlış, bilerek söylenmez.</summary>
    Task<User?> LoginAsync(string email, string password);

    Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
