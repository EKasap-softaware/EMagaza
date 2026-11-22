using EMagaza.Domain.Constants;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Domain.Entities;

public class User : Entity
{
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = Roles.Customer;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private User() { }

    public User(string displayName, string email, string passwordHash, string role)
    {
        if (string.IsNullOrWhiteSpace(displayName)) throw new DomainException("Ad zorunlu.");
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("E-posta zorunlu.");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("Şifre özeti zorunlu.");
        if (!Roles.All.Contains(role)) throw new DomainException("Geçersiz rol.");

        DisplayName = displayName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
    }

    public void ChangePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("Şifre özeti zorunlu.");
        PasswordHash = passwordHash;
    }

    public void ChangeRole(string role)
    {
        if (!Roles.All.Contains(role)) throw new DomainException("Geçersiz rol.");
        Role = role;
    }
}
