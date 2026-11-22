using EMagaza.Infrastructure.Persistence.Mongo;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace EMagaza.Infrastructure.Persistence.Migrations;

/// <summary>Uygulama açılırken henüz uygulanmamış migration'ları sürüm sırasıyla çalıştırır.</summary>
public class MigrationRunner(IMongoDbContext context, IEnumerable<IMigration> migrations, ILogger<MigrationRunner> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await RunPendingAsync();
        }
        catch (Exception ex)
        {
            // Mongo kapalıysa veya bir migration patlarsa uygulama açılmaya devam eder; migration bir sonraki açılışta tekrar denenir.
            logger.LogError(ex, "Migration'lar çalıştırılamadı");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task RunPendingAsync()
    {
        var ordered = migrations.OrderBy(m => m.Version).ToList();
        var duplicate = ordered.GroupBy(m => m.Version).FirstOrDefault(g => g.Count() > 1);
        if (duplicate is not null) throw new InvalidOperationException($"{duplicate.Key} numaralı birden fazla migration var.");

        // Aynı sürümün iki kez kaydedilmesini veritabanı engeller (birden fazla web örneği aynı anda açılsa bile).
        await context.AppliedMigrations.Indexes.CreateOneAsync(new CreateIndexModel<AppliedMigration>(
            Builders<AppliedMigration>.IndexKeys.Ascending(m => m.Version), new CreateIndexOptions { Unique = true }));

        var applied = (await context.AppliedMigrations.AsQueryable().Select(m => m.Version).ToListAsync()).ToHashSet();

        foreach (var migration in ordered.Where(m => !applied.Contains(m.Version)))
        {
            logger.LogInformation("Migration {Version} uygulanıyor: {Description}", migration.Version, migration.Description);
            await migration.UpAsync(context);

            try
            {
                await context.AppliedMigrations.InsertOneAsync(new AppliedMigration(migration.Version, migration.Description));
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                // Başka bir örnek aynı migration'ı az önce kaydetti; sorun yok.
            }
        }
    }
}
