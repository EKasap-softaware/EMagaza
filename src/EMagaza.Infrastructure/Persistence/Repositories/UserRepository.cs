using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Repositories;

public class UserRepository(IMongoDbContext context)
    : MongoRepositoryBase<User>(context.Users), IUserRepository
{
    public Task<User?> GetByIdAsync(string id) => FindByIdAsync(id);

    public Task<User?> GetByEmailAsync(string email) =>
        Collection.AsQueryable().FirstOrDefaultAsync(u => u.Email == email)!;

    public async Task<IReadOnlyList<User>> GetAllAsync() =>
        await Collection.AsQueryable().OrderBy(u => u.CreatedAt).ToListAsync();

    public async Task AddAsync(User user)
    {
        try
        {
            await InsertAsync(user);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            // Eşzamanlı iki kayıtta da e-posta tekilliğini unique indeks garanti eder.
            throw new DomainException("Bu e-posta zaten kayıtlı.");
        }
    }

    public async Task UpdateRoleAsync(string id, string role)
    {
        if (!IsValidId(id)) return;
        await Collection.UpdateOneAsync(u => u.Id == id, Builders<User>.Update.Set(u => u.Role, role));
    }

    public async Task UpdatePasswordHashAsync(string id, string passwordHash)
    {
        if (!IsValidId(id)) return;
        await Collection.UpdateOneAsync(u => u.Id == id, Builders<User>.Update.Set(u => u.PasswordHash, passwordHash));
    }

    public Task<long> CountAsync() => Collection.CountDocumentsAsync(FilterDefinition<User>.Empty);

    public Task<long> CountByRoleAsync(string role) => Collection.CountDocumentsAsync(u => u.Role == role);
}
