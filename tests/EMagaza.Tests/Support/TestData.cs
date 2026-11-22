using EMagaza.Application.Carts;
using EMagaza.Application.Models;
using EMagaza.Domain.Entities;

namespace EMagaza.Tests.Support;

public static class TestData
{
    public static readonly ShippingInfo Shipping = new("Ayşe Yılmaz", "ayse@x.com", "0555 123 45 67", "İstanbul", "Moda Cd. 1");
    public static readonly PaymentCard GoodCard = new("4242 4242 4242 4242", "AYSE YILMAZ", "12/35", "123");
    public static readonly PaymentCard DeclinedCard = new("4000 0000 0000 0002", "AYSE YILMAZ", "12/35", "123");

    /// <summary>900 TL'lik gömlek: S'den 2, M'den <paramref name="stockM"/>, L tükenmiş.</summary>
    public static Product Shirt(int stockM = 5) =>
        new Product("Gömlek", "LUNE", "Erkek", "shirt", "Beyaz", "#ffffff", 900)
            .AddSize("S", 2).AddSize("M", stockM).AddSize("L", 0)
            .WithId("p1");

    public static Order NewOrder(string userId = "u1") =>
        new(userId, "Ali", "a@b.com", "05551234567", "İstanbul", "Moda");

    public static Order PaidOrder()
    {
        var order = NewOrder();
        order.AddItem(Shirt(), "M", 1);
        order.MarkPaid("Visa", "4242", "tx");
        return order.WithId("507f1f77bcf86cd799439011");
    }

    public static Cart CartWith(Product product, string size, int quantity)
    {
        var cart = new Cart();
        cart.Add(product, size, quantity);
        return cart;
    }
}
