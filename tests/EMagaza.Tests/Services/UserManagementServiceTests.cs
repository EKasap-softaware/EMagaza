using EMagaza.Application.Services;
using EMagaza.Domain.Constants;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Fakes;

namespace EMagaza.Tests.Services;

public class UserManagementServiceTests
{
    private readonly UserManagementService _service;
    private readonly User _admin = new("Yönetici", "admin@x.com", "hash", Roles.Admin);
    private readonly User _customer = new("Müşteri", "musteri@x.com", "hash", Roles.Customer);

    public UserManagementServiceTests()
    {
        var users = new FakeUserRepository();
        users.AddAsync(_admin).Wait();
        users.AddAsync(_customer).Wait();
        _service = new UserManagementService(users);
    }

    [Fact]
    public async Task Admin_CanPromoteCustomer()
    {
        await _service.ChangeRoleAsync(_admin.Id, _customer.Id, Roles.Admin);

        Assert.Equal(Roles.Admin, _customer.Role);
    }

    [Fact]
    public async Task Admin_CanDemoteAnotherAdmin_WhenTwoAdminsExist()
    {
        await _service.ChangeRoleAsync(_admin.Id, _customer.Id, Roles.Admin);
        await _service.ChangeRoleAsync(_admin.Id, _customer.Id, Roles.Customer);

        Assert.Equal(Roles.Customer, _customer.Role);
    }

    [Fact]
    public async Task CannotChangeOwnRole() =>
        await Assert.ThrowsAsync<DomainException>(() => _service.ChangeRoleAsync(_admin.Id, _admin.Id, Roles.Customer));

    [Fact]
    public async Task CannotRemoveLastAdmin_EvenWhenActorHasStaleCookie() =>
        await Assert.ThrowsAsync<DomainException>(() => _service.ChangeRoleAsync("eski-cookie", _admin.Id, Roles.Customer));

    [Fact]
    public async Task InvalidRole_Throws() =>
        await Assert.ThrowsAsync<DomainException>(() => _service.ChangeRoleAsync(_admin.Id, _customer.Id, "Hacker"));

    [Fact]
    public async Task UnknownUser_Throws() =>
        await Assert.ThrowsAsync<NotFoundException>(() => _service.ChangeRoleAsync(_admin.Id, "yok", Roles.Admin));
}
