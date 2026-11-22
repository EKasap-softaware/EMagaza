using EMagaza.Application.Abstractions;
using EMagaza.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
namespace EMagaza.Infrastructure.Mongo;

public record MongoSettings(string ConnectionString, string Database);

public class MongoReviewRepository : IReviewRepository
{
    readonly IMongoCollection<Review> _col;

    static MongoReviewRepository()
    {
        // Domain'i Mongo'ya bağlamamak için [BsonId] attribute'u yerine eşlemeyi BURADA yapıyoruz.
        if (!BsonClassMap.IsClassMapRegistered(typeof(Review)))
            BsonClassMap.RegisterClassMap<Review>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(r => r.Id).SetSerializer(new StringSerializer(BsonType.ObjectId))
                  .SetIdGenerator(StringObjectIdGenerator.Instance);
            });
    }

    public MongoReviewRepository(IMongoClient client, MongoSettings s)
    {
        // Constructor'da ağ çağrısı YAPMA: Mongo kapalıysa nesne bile oluşturulamaz ve sayfa 500 verir.
        _col = client.GetDatabase(s.Database).GetCollection<Review>("reviews");
    }

    /// <summary>Ürüne göre sorgu hızlı olsun diye indeks. Açılışta bir kez çağrılır (varsa tekrar oluşturmaz).</summary>
    public static void EnsureIndexes(IMongoClient client, MongoSettings s) =>
        client.GetDatabase(s.Database).GetCollection<Review>("reviews").Indexes
              .CreateOne(new CreateIndexModel<Review>(Builders<Review>.IndexKeys.Ascending(r => r.ProductId)));

    public Task AddAsync(Review review) => _col.InsertOneAsync(review);

    public Task<List<Review>> GetByProductAsync(int productId) =>
        _col.Find(r => r.ProductId == productId).SortByDescending(r => r.CreatedAt).ToListAsync();
}
