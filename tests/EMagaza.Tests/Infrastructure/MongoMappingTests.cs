using EMagaza.Domain.Entities;
using EMagaza.Domain.Enums;
using EMagaza.Infrastructure.Persistence.Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace EMagaza.Tests.Infrastructure;

/// <summary>
/// Veritabanındaki belge eksik/farklı alanlara sahip olsa bile (eski veri, elle eklenen veri, seed dosyası)
/// entity'ler okunabilmeli. Gerçek MongoDB gerekmez: sürücünün eşleme kuralları BsonDocument üzerinde denenir.
/// </summary>
public class MongoMappingTests
{
    public MongoMappingTests() => MongoConventions.Register();

    private static T Read<T>(BsonDocument document) => BsonSerializer.Deserialize<T>(document);

    [Fact]
    public void Coupon_WithoutOptionalFields_CanBeRead()
    {
        var coupon = Read<Coupon>(new BsonDocument
        {
            { "Code", "HOS10" }, { "Kind", "Percent" }, { "Value", 10 }, { "MinSubtotal", 0 }, { "IsActive", true }
        });

        Assert.Equal("HOS10", coupon.Code);
        Assert.Equal(DiscountKind.Percent, coupon.Kind);
        Assert.Null(coupon.ExpiresAt);
        Assert.Equal(0, coupon.UsageLimit);
    }

    [Fact]
    public void Product_WithOnlyBasicFields_CanBeRead()
    {
        var product = Read<Product>(new BsonDocument { { "Name", "Gömlek" }, { "Category", "Erkek" }, { "Price", 900 } });

        Assert.Equal("Gömlek", product.Name);
        Assert.Empty(product.Sizes);
        Assert.Equal(0, product.ReviewCount);
    }

    [Fact]
    public void Order_WithoutNewerFields_CanBeRead()
    {
        var order = Read<Order>(new BsonDocument { { "UserId", "u1" }, { "Status", "Paid" } });

        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Null(order.CouponCode);
        Assert.Equal(0, order.DiscountAmount);
    }

    [Fact]
    public void User_Review_Notification_Address_Favorite_CanBeReadFromMinimalDocuments()
    {
        Assert.NotNull(Read<User>(new BsonDocument { { "Email", "a@b.com" } }));
        Assert.NotNull(Read<Review>(new BsonDocument { { "ProductId", "p1" } }));
        Assert.NotNull(Read<Notification>(new BsonDocument { { "UserId", "u1" } }));
        Assert.NotNull(Read<Address>(new BsonDocument { { "UserId", "u1" } }));
        Assert.NotNull(Read<Favorite>(new BsonDocument { { "UserId", "u1" } }));
    }

    [Fact]
    public void Unknown_ExtraFields_AreIgnored() =>
        Assert.NotNull(Read<Coupon>(new BsonDocument { { "Code", "X" }, { "SomethingRemoved", 1 } }));

    [Fact]
    public void StringId_IsStoredAsObjectId_AndReadBack()
    {
        var id = ObjectId.GenerateNewId();
        var coupon = Read<Coupon>(new BsonDocument { { "_id", id }, { "Code", "X" } });

        Assert.Equal(id.ToString(), coupon.Id);
    }
}
