using EMagaza.Infrastructure.Persistence.Mongo;

namespace EMagaza.Infrastructure.Persistence.Migrations;

/// <summary>
/// Tek bir veritabanı değişikliği. Her migration bir kez çalışır; çalıştığı `migrations` koleksiyonuna yazılır.
/// Kural: uygulanmış bir migration'ı asla düzenleme, yeni bir tane ekle. Mümkün olduğunca tekrar çalışsa da zarar vermesin (idempotent).
/// </summary>
public interface IMigration
{
    int Version { get; }
    string Description { get; }
    Task UpAsync(IMongoDbContext context);
}
