using EMagaza.Domain.Entities;
using Microsoft.Extensions.Options;
using EMagaza.Infrastructure.Persistence.Migrations;
using MongoDB.Driver;

namespace EMagaza.Infrastructure.Persistence.Mongo;

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbOptions> options)
    {
        MongoConventions.Register();

        var settings = MongoClientSettings.FromConnectionString(options.Value.ConnectionString);
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(3);   // varsayılan 30 sn: Mongo kapalıysa sayfa donmasın
        _database = new MongoClient(settings).GetDatabase(options.Value.Database);
    }

    public IMongoCollection<Product> Products => _database.GetCollection<Product>(CollectionNames.Products);
    public IMongoCollection<Order> Orders => _database.GetCollection<Order>(CollectionNames.Orders);
    public IMongoCollection<Review> Reviews => _database.GetCollection<Review>(CollectionNames.Reviews);
    public IMongoCollection<User> Users => _database.GetCollection<User>(CollectionNames.Users);
    public IMongoCollection<EmailLog> EmailLogs => _database.GetCollection<EmailLog>(CollectionNames.EmailLogs);
    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>(CollectionNames.Notifications);
    public IMongoCollection<AppliedMigration> AppliedMigrations => _database.GetCollection<AppliedMigration>(CollectionNames.Migrations);
    public IMongoCollection<Coupon> Coupons => _database.GetCollection<Coupon>(CollectionNames.Coupons);
    public IMongoCollection<Favorite> Favorites => _database.GetCollection<Favorite>(CollectionNames.Favorites);
    public IMongoCollection<Address> Addresses => _database.GetCollection<Address>(CollectionNames.Addresses);
}
