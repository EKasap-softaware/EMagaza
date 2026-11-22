using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence.Migrations;
using MongoDB.Driver;

namespace EMagaza.Infrastructure.Persistence.Mongo;

/// <summary>EF Core'daki DbContext'in karşılığı: tüm koleksiyonlara tek noktadan erişim.</summary>
public interface IMongoDbContext
{
    IMongoCollection<Product> Products { get; }
    IMongoCollection<Order> Orders { get; }
    IMongoCollection<Review> Reviews { get; }
    IMongoCollection<User> Users { get; }
    IMongoCollection<EmailLog> EmailLogs { get; }
    IMongoCollection<Notification> Notifications { get; }
    IMongoCollection<AppliedMigration> AppliedMigrations { get; }
    IMongoCollection<Coupon> Coupons { get; }
    IMongoCollection<Favorite> Favorites { get; }
    IMongoCollection<Address> Addresses { get; }
}
