using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Repositories;

public class AddressRepository(IMongoDbContext context)
    : MongoRepositoryBase<Address>(context.Addresses), IAddressRepository
{
    public async Task<IReadOnlyList<Address>> GetByUserAsync(string userId) =>
        await Collection.AsQueryable().Where(a => a.UserId == userId).OrderBy(a => a.CreatedAt).ToListAsync();

    public Task AddAsync(Address address) => InsertAsync(address);

    // userId şartı: başkasının adresini id tahmin ederek silmek mümkün olmasın.
    public async Task DeleteAsync(string userId, string addressId)
    {
        if (!IsValidId(addressId)) return;
        await Collection.DeleteOneAsync(a => a.Id == addressId && a.UserId == userId);
    }
}
