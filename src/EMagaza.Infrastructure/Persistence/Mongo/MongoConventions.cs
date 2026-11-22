using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace EMagaza.Infrastructure.Persistence.Mongo;

/// <summary>
/// Eşleme kuralları tek yerde ve sınıf başına kod yazmadan: "Id" alanı otomatik _id olur,
/// string Id veritabanında ObjectId saklanır, enum'lar yazı olarak tutulur.
/// </summary>
public static class MongoConventions
{
    private static readonly object Gate = new();
    private static bool _registered;

    public static void Register()
    {
        lock (Gate)
        {
            if (_registered) return;
            _registered = true;

            ConventionRegistry.Register("EMagazaConventions", new ConventionPack
            {
                new ParameterlessConstructorConvention(),
                new StringIdStoredAsObjectIdConvention(),
                new EnumRepresentationConvention(BsonType.String),
                new IgnoreExtraElementsConvention(true)
            }, _ => true);

            // decimal varsayılan olarak yazı saklanır; para için Decimal128 gerekir.
            BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
        }
    }
}
