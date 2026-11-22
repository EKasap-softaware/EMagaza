using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Services;

public class AddressService(IAddressRepository addresses) : IAddressService
{
    private const int MaxAddresses = 5;

    public Task<IReadOnlyList<Address>> GetAsync(string userId) => addresses.GetByUserAsync(userId);

    public async Task AddAsync(string userId, string title, string fullName, string phone, string city, string line)
    {
        if ((await addresses.GetByUserAsync(userId)).Count >= MaxAddresses)
            throw new DomainException($"En fazla {MaxAddresses} adres kaydedebilirsin.");

        await addresses.AddAsync(new Address(userId, title, fullName, phone, city, line));
    }

    public Task DeleteAsync(string userId, string addressId) => addresses.DeleteAsync(userId, addressId);
}
