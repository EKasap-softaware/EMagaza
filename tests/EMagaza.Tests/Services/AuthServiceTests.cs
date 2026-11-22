using EMagaza.Application.Options;
using EMagaza.Application.Services;
using EMagaza.Domain.Constants;
using EMagaza.Domain.Exceptions;
using EMagaza.Infrastructure.Security;
using EMagaza.Tests.Fakes;
using Microsoft.Extensions.Options;

namespace EMagaza.Tests.Services;

public class AuthServiceTests
{
    private readonly AuthService _auth = new(
        new FakeUserRepository(),
        new Pbkdf2PasswordHasher(),
        Options.Create(new AuthOptions { AdminEmail = "admin@emagaza.local" }));

    [Fact]
    public async Task Register_StoresHash_NotPlainPassword()
    {
        var user = await _auth.RegisterAsync("Ali", "Ali@X.com", "gizli123");

        Assert.DoesNotContain("gizli123", user.PasswordHash);
        Assert.StartsWith("v1.", user.PasswordHash);
        Assert.Equal("ali@x.com", user.Email);
        Assert.Equal(Roles.Customer, user.Role);
    }

    [Fact]
    public async Task Register_AdminEmail_GetsAdminRole() =>
        Assert.Equal(Roles.Admin, (await _auth.RegisterAsync("Yönetici", "admin@emagaza.local", "gizli123")).Role);

    [Fact]
    public async Task Register_DuplicateEmail_Throws()
    {
        await _auth.RegisterAsync("Ali", "ali@x.com", "gizli123");

        await Assert.ThrowsAsync<DomainException>(() => _auth.RegisterAsync("Veli", "ALI@x.com", "baska123"));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("")]
    public async Task Register_ShortPassword_Throws(string password) =>
        await Assert.ThrowsAsync<DomainException>(() => _auth.RegisterAsync("Ali", "ali@x.com", password));

    [Fact]
    public async Task ChangePassword_WorksWithCorrectCurrentPassword()
    {
        var user = await _auth.RegisterAsync("Ali", "ali@x.com", "eski123");

        await _auth.ChangePasswordAsync(user.Id, "eski123", "yeni4567");

        Assert.Null(await _auth.LoginAsync("ali@x.com", "eski123"));
        Assert.NotNull(await _auth.LoginAsync("ali@x.com", "yeni4567"));
    }

    [Fact]
    public async Task ChangePassword_WrongCurrentPassword_Throws()
    {
        var user = await _auth.RegisterAsync("Ali", "ali@x.com", "eski123");

        await Assert.ThrowsAsync<DomainException>(() => _auth.ChangePasswordAsync(user.Id, "yanlis", "yeni4567"));
    }

    [Fact]
    public async Task ChangePassword_TooShortNewPassword_Throws()
    {
        var user = await _auth.RegisterAsync("Ali", "ali@x.com", "eski123");

        await Assert.ThrowsAsync<DomainException>(() => _auth.ChangePasswordAsync(user.Id, "eski123", "123"));
    }

    [Fact]
    public async Task Login_CorrectPassword_ReturnsUser()
    {
        await _auth.RegisterAsync("Ali", "ali@x.com", "gizli123");

        Assert.NotNull(await _auth.LoginAsync("ALI@x.com", "gizli123"));
    }

    [Theory]
    [InlineData("ali@x.com", "yanlis")]
    [InlineData("yok@x.com", "gizli123")]
    public async Task Login_WrongCredentials_ReturnsNull(string email, string password)
    {
        await _auth.RegisterAsync("Ali", "ali@x.com", "gizli123");

        Assert.Null(await _auth.LoginAsync(email, password));
    }
}
