using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Application.Options;
using EMagaza.Domain.Constants;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace EMagaza.Application.Services;

public class AuthService(IUserRepository users, IPasswordHasher hasher, IOptions<AuthOptions> options) : IAuthService
{
    private const int MinPasswordLength = 6;

    public async Task<User> RegisterAsync(string displayName, string email, string password)
    {
        email = Normalize(email);
        if (string.IsNullOrWhiteSpace(displayName)) throw new DomainException("Ad zorunlu.");
        if (!email.Contains('@') || !email.Contains('.')) throw new DomainException("Geçerli bir e-posta girin.");
        EnsureValidPassword(password);
        if (await users.GetByEmailAsync(email) is not null) throw new DomainException("Bu e-posta zaten kayıtlı.");

        var role = email.Equals(options.Value.AdminEmail, StringComparison.OrdinalIgnoreCase) ? Roles.Admin : Roles.Customer;
        var user = new User(displayName, email, hasher.Hash(password), role);
        await users.AddAsync(user);
        return user;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await users.GetByEmailAsync(Normalize(email));
        return user is not null && hasher.Verify(password ?? string.Empty, user.PasswordHash) ? user : null;
    }

    public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await users.GetByIdAsync(userId) ?? throw new NotFoundException("Kullanıcı bulunamadı.");
        if (!hasher.Verify(currentPassword ?? string.Empty, user.PasswordHash)) throw new DomainException("Mevcut şifre hatalı.");
        EnsureValidPassword(newPassword);

        await users.UpdatePasswordHashAsync(user.Id, hasher.Hash(newPassword));
    }

    private static void EnsureValidPassword(string? password)
    {
        if ((password ?? string.Empty).Length < MinPasswordLength)
            throw new DomainException($"Şifre en az {MinPasswordLength} karakter olmalı.");
    }

    private static string Normalize(string? email) => (email ?? string.Empty).Trim().ToLowerInvariant();
}
