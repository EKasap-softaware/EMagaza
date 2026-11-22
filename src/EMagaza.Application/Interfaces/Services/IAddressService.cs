using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Services;

public interface IAddressService
{
    Task<IReadOnlyList<Address>> GetAsync(string userId);
    Task AddAsync(string userId, string title, string fullName, string phone, string city, string line);
    Task DeleteAsync(string userId, string addressId);
}
