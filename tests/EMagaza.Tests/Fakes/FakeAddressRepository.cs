using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Fakes;

public class FakeAddressRepository : IAddressRepository
{
    public List<Address> Addresses { get; } = new();

    public Task<IReadOnlyList<Address>> GetByUserAsync(string userId) =>
        Task.FromResult<IReadOnlyList<Address>>(Addresses.Where(a => a.UserId == userId).ToList());

    public Task AddAsync(Address address)
    {
        address.WithId(Guid.NewGuid().ToString("N")[..24]);
        Addresses.Add(address);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string userId, string addressId)
    {
        Addresses.RemoveAll(a => a.Id == addressId && a.UserId == userId);
        return Task.CompletedTask;
    }
}
