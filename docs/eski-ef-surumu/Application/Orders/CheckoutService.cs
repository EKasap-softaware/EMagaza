using EMagaza.Application.Abstractions;
using EMagaza.Application.Carts;
using EMagaza.Domain.Entities;
namespace EMagaza.Application.Orders;

public record CheckoutRequest(string CustomerName, string Email, string Address);

public class CheckoutService(IProductRepository products, IOrderRepository orders, IOrderEventPublisher events)
{
    public async Task<Order> PlaceOrderAsync(Cart cart, CheckoutRequest request)
    {
        if (cart.Lines.Count == 0) throw new InvalidOperationException("Sepet boş.");
        if (string.IsNullOrWhiteSpace(request.CustomerName) || string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Ad ve e-posta zorunlu.");

        var dbProducts = (await products.GetByIdsAsync(cart.Lines.Select(l => l.ProductId)))
                         .ToDictionary(p => p.Id);

        var order = new Order { CustomerName = request.CustomerName, Email = request.Email, Address = request.Address };
        foreach (var line in cart.Lines)
        {
            if (!dbProducts.TryGetValue(line.ProductId, out var p))
                throw new InvalidOperationException($"Ürün bulunamadı: {line.Name}");
            p.DecreaseStock(line.Quantity); // yetersizse burada exception, hiçbir şey kaydedilmez
            // Fiyatı sepetten DEĞİL veritabanından al: kullanıcı sepeti oynayamasın.
            order.Items.Add(new OrderItem { ProductId = p.Id, ProductName = p.Name, UnitPrice = p.Price, Quantity = line.Quantity });
        }

        await orders.AddAsync(order);
        await orders.SaveChangesAsync();   // 1) önce sipariş KALICI olur

        // 2) sonra olay kuyruğa bırakılır. E-posta/stok bildirimi gibi yavaş işler müşteriyi bekletmez.
        //    Kuyruk çökerse sipariş yine de geçerlidir (publisher hatayı yutar+loglar).
        await events.PublishOrderPlacedAsync(new OrderPlacedEvent(order.Id, order.CustomerName, order.Email, order.Total));
        return order;
    }
}
