using EMagaza.Application.Abstractions;
using EMagaza.Application.Carts;
using EMagaza.Application.Orders;
using EMagaza.Domain.Entities;
using EMagaza.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
namespace EMagaza.Tests;

// ---- 1) Saf unit test: veritabanı yok, çok hızlı ----
public class CartTests
{
    [Fact]
    public void Add_SameProductTwice_MergesQuantity()
    {
        var cart = new Cart();
        cart.Add(1, "Kulaklık", 100m, "🎧");
        cart.Add(1, "Kulaklık", 100m, "🎧", 2);
        Assert.Single(cart.Lines);
        Assert.Equal(3, cart.ItemCount);
        Assert.Equal(300m, cart.Total);
    }

    [Fact]
    public void Remove_DeletesLine()
    {
        var cart = new Cart();
        cart.Add(1, "A", 10m, "x");
        cart.Remove(1);
        Assert.Empty(cart.Lines);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Add_InvalidQuantity_Throws(int qty)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new Cart().Add(1, "A", 10m, "x", qty));
}

public class ProductTests
{
    [Fact]
    public void DecreaseStock_ReducesStock()
    {
        var p = new Product { Name = "A", Stock = 5 };
        p.DecreaseStock(2);
        Assert.Equal(3, p.Stock);
    }

    [Fact]
    public void DecreaseStock_MoreThanStock_Throws()
        => Assert.Throws<InvalidOperationException>(() => new Product { Name = "A", Stock = 1 }.DecreaseStock(2));
}

// ---- 2) Entegrasyon testi: gerçek EF Core + bellek içi SQLite ----
public class CheckoutServiceTests : IDisposable
{
    readonly SqliteConnection _conn = new("Data Source=:memory:");
    readonly StoreDbContext _db;
    readonly CheckoutService _svc;
    readonly FakePublisher _events = new();

    public CheckoutServiceTests()
    {
        _conn.Open(); // bağlantı açık kaldıkça bellek DB'si yaşar
        _db = new StoreDbContext(new DbContextOptionsBuilder<StoreDbContext>().UseSqlite(_conn).Options);
        _db.Database.EnsureCreated();
        _db.Products.Add(new Product { Name = "Kulaklık", Price = 100m, Stock = 5, Category = "E" });
        _db.SaveChanges();
        _svc = new CheckoutService(new ProductRepository(_db), new OrderRepository(_db), _events);
    }

    [Fact]
    public async Task PlaceOrder_SavesOrder_AndDecreasesStock()
    {
        var cart = new Cart();
        cart.Add(1, "Kulaklık", 100m, "🎧", 2);

        var order = await _svc.PlaceOrderAsync(cart, new CheckoutRequest("Ali", "ali@x.com", "İstanbul"));

        Assert.True(order.Id > 0);
        Assert.Equal(200m, order.Total);
        Assert.Equal(3, (await _db.Products.FindAsync(1))!.Stock);
    }

    [Fact]
    public async Task PlaceOrder_PublishesOrderPlacedEvent()
    {
        var cart = new Cart();
        cart.Add(1, "Kulaklık", 100m, "🎧", 2);
        var order = await _svc.PlaceOrderAsync(cart, new CheckoutRequest("Ali", "ali@x.com", "x"));
        var evt = Assert.Single(_events.Published);
        Assert.Equal(order.Id, evt.OrderId);
        Assert.Equal(200m, evt.Total);
    }

    [Fact]
    public async Task PlaceOrder_InsufficientStock_PublishesNothing()
    {
        var cart = new Cart();
        cart.Add(1, "Kulaklık", 100m, "🎧", 99);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _svc.PlaceOrderAsync(cart, new CheckoutRequest("Ali", "ali@x.com", "x")));
        Assert.Empty(_events.Published);
    }

    [Fact]
    public async Task PlaceOrder_UsesDatabasePrice_NotCartPrice()
    {
        var cart = new Cart();
        cart.Add(1, "Kulaklık", 1m, "🎧"); // kullanıcı sepette fiyatı 1 TL yapmış gibi
        var order = await _svc.PlaceOrderAsync(cart, new CheckoutRequest("Ali", "ali@x.com", "x"));
        Assert.Equal(100m, order.Total);
    }

    [Fact]
    public async Task PlaceOrder_InsufficientStock_ThrowsAndSavesNothing()
    {
        var cart = new Cart();
        cart.Add(1, "Kulaklık", 100m, "🎧", 99);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _svc.PlaceOrderAsync(cart, new CheckoutRequest("Ali", "ali@x.com", "x")));
        Assert.Equal(0, await _db.Orders.CountAsync());
    }

    [Fact]
    public async Task PlaceOrder_EmptyCart_Throws()
        => await Assert.ThrowsAsync<InvalidOperationException>(
            () => _svc.PlaceOrderAsync(new Cart(), new CheckoutRequest("Ali", "ali@x.com", "x")));

    public void Dispose() { _db.Dispose(); _conn.Dispose(); }
}

// Sahte (fake) yayıncı: gerçek RabbitMQ yerine olayları listeye yazar.
public class FakePublisher : IOrderEventPublisher
{
    public List<OrderPlacedEvent> Published { get; } = new();
    public Task PublishOrderPlacedAsync(OrderPlacedEvent evt) { Published.Add(evt); return Task.CompletedTask; }
}

public class ReviewTests
{
    [Fact]
    public void Create_ValidReview_TrimsText()
    {
        var r = Review.Create(1, "  Ayşe ", 5, " Harika ");
        Assert.Equal("Ayşe", r.Author);
        Assert.Equal("Harika", r.Comment);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_RatingOutOfRange_Throws(int rating)
        => Assert.Throws<ArgumentOutOfRangeException>(() => Review.Create(1, "A", rating, "x"));

    [Fact]
    public void Create_EmptyComment_Throws()
        => Assert.Throws<ArgumentException>(() => Review.Create(1, "A", 5, "  "));
}
