using EMagaza.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Repositories;

/// <summary>Tüm Mongo repository'lerinin ortak işleri: ekleme ve Id ile getirme.</summary>
public abstract class MongoRepositoryBase<T>(IMongoCollection<T> collection) where T : Entity
{
    protected IMongoCollection<T> Collection { get; } = collection;

    protected Task InsertAsync(T entity) => Collection.InsertOneAsync(entity);

    protected async Task<T?> FindByIdAsync(string id) =>
        IsValidId(id) ? await Collection.AsQueryable().FirstOrDefaultAsync(e => e.Id == id) : null;

    /// <summary>URL'den gelen geçersiz bir id sürücüde FormatException'a yol açar; önce kontrol edilir.</summary>
    protected static bool IsValidId(string? id) => ObjectId.TryParse(id, out _);
}
