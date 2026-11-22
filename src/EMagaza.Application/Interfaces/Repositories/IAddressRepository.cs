using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Repositories;

public interface IAddressRepository
{
    Task<IReadOnlyList<Address>> GetByUserAsync(string userId);
    Task AddAsync(Address address);
    Task DeleteAsync(string userId, string addressId);
}
