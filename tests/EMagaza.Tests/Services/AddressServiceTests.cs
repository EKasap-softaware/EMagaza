using EMagaza.Application.Services;
using EMagaza.Domain.Exceptions;
using EMagaza.Tests.Fakes;

namespace EMagaza.Tests.Services;

public class AddressServiceTests
{
    private readonly FakeAddressRepository _repository = new();
    private readonly AddressService _service;

    public AddressServiceTests() => _service = new AddressService(_repository);

    private Task AddHome(string userId = "u1") =>
        _service.AddAsync(userId, "Ev", "Ayşe Yılmaz", "05551234567", "İstanbul", "Moda Cd. 1");

    [Fact]
    public async Task Add_SavesAddressForTheUser()
    {
        await AddHome();

        Assert.Single(await _service.GetAsync("u1"));
        Assert.Empty(await _service.GetAsync("u2"));
    }

    [Fact]
    public async Task Add_InvalidPhone_Throws() =>
        await Assert.ThrowsAsync<DomainException>(() => _service.AddAsync("u1", "Ev", "Ayşe", "123", "İstanbul", "Moda"));

    [Fact]
    public async Task Add_MoreThanFive_Throws()
    {
        for (var i = 0; i < 5; i++) await AddHome();

        await Assert.ThrowsAsync<DomainException>(() => AddHome());
    }

    [Fact]
    public async Task Delete_OnlyRemovesOwnAddress()
    {
        await AddHome("u1");
        var id = (await _service.GetAsync("u1")).Single().Id;

        await _service.DeleteAsync("u2", id);   // başkası silmeye çalışıyor
        Assert.Single(await _service.GetAsync("u1"));

        await _service.DeleteAsync("u1", id);
        Assert.Empty(await _service.GetAsync("u1"));
    }
}
