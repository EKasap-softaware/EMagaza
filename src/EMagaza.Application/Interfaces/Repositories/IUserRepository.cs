using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateRoleAsync(string id, string role);
    Task UpdatePasswordHashAsync(string id, string passwordHash);
    Task<long> CountAsync();
    Task<long> CountByRoleAsync(string role);
}
