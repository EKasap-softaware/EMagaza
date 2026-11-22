using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Tests.Support;

namespace EMagaza.Tests.Fakes;

public class FakeUserRepository : IUserRepository
{
    public List<User> Users { get; } = new();

    public Task AddAsync(User user)
    {
        user.WithId(Guid.NewGuid().ToString("N")[..24]);
        Users.Add(user);
        return Task.CompletedTask;
    }

    public Task<User?> GetByIdAsync(string id) => Task.FromResult(Users.FirstOrDefault(u => u.Id == id));
    public Task<User?> GetByEmailAsync(string email) => Task.FromResult(Users.FirstOrDefault(u => u.Email == email));
    public Task<IReadOnlyList<User>> GetAllAsync() => Task.FromResult<IReadOnlyList<User>>(Users.ToList());
    public Task UpdateRoleAsync(string id, string role) { Users.First(u => u.Id == id).ChangeRole(role); return Task.CompletedTask; }
    public Task UpdatePasswordHashAsync(string id, string passwordHash) { Users.First(u => u.Id == id).ChangePasswordHash(passwordHash); return Task.CompletedTask; }
    public Task<long> CountAsync() => Task.FromResult((long)Users.Count);
    public Task<long> CountByRoleAsync(string role) => Task.FromResult((long)Users.Count(u => u.Role == role));
}
