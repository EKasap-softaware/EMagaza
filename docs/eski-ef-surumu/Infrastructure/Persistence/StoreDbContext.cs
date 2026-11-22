using EMagaza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace EMagaza.Infrastructure.Persistence;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>(e =>
        {
            e.Property(p => p.Name).HasMaxLength(120).IsRequired();
            e.Property(p => p.Price).HasPrecision(18, 2);
            e.HasIndex(p => p.Category);
        });
        b.Entity<Order>(e =>
        {
            e.Ignore(o => o.Total);   // hesaplanan alan, kolon olmasın
            e.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId);
        });
        b.Entity<OrderItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);
    }
}
