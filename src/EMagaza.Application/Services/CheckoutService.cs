using EMagaza.Application.Carts;
using EMagaza.Application.Exceptions;
using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Application.Models;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Services;

public class CheckoutService(
    IProductRepository products,
    IOrderRepository orders,
    IPaymentGateway payments,
    ICouponService coupons,
    IOrderEventPublisher events) : ICheckoutService
{
    public async Task<Order> PlaceOrderAsync(Cart cart, ShippingInfo shipping, PaymentCard card, string userId)
    {
        if (cart.Lines.Count == 0) throw new DomainException("Sepet boş.");

        var order = new Order(userId, shipping.CustomerName, shipping.Email, shipping.Phone, shipping.City, shipping.Address);
        var catalog = (await products.GetByIdsAsync(cart.Lines.Select(l => l.ProductId))).ToDictionary(p => p.Id);
        var reservation = new StockReservation(products);
        string? claimedCoupon = null;
        string? transactionId = null;

        try
        {
            foreach (var line in cart.Lines)
            {
                if (!catalog.TryGetValue(line.ProductId, out var product))
                    throw new NotFoundException($"Ürün bulunamadı: {line.Name}");

                await reservation.ReserveAsync(product, line.Size, line.Quantity);
                order.AddItem(product, line.Size, line.Quantity);   // fiyat sepetten değil veritabanından
            }

            // Kupon sepetteki tutara güvenilmeden yeniden doğrulanır ve kullanım hakkı atomik alınır.
            if (cart.CouponCode is not null)
            {
                var application = await coupons.ResolveAsync(cart.CouponCode, order.Subtotal);
                await coupons.ClaimAsync(application.Code);
                claimedCoupon = application.Code;
                order.ApplyDiscount(application.Code, application.Discount);
            }

            order.ApplyShippingFee(ShippingPolicy.FeeFor(order.Subtotal - order.DiscountAmount));

            var payment = await payments.ChargeAsync(order.Total, card, userId);
            if (!payment.Approved) throw new PaymentDeclinedException(payment.Error ?? "Ödeme reddedildi.");
            transactionId = payment.TransactionId;

            order.MarkPaid(payment.Brand ?? "Kart", payment.Last4 ?? "????", payment.TransactionId!);
            await orders.AddAsync(order);
        }
        catch
        {
            await reservation.ReleaseAsync();
            if (claimedCoupon is not null) await coupons.ReleaseAsync(claimedCoupon);
            if (transactionId is not null) await payments.RefundAsync(transactionId);
            throw;
        }

        // Sipariş kalıcı olduktan sonra haber ver: kuyruk çökerse sipariş yine geçerli.
        await events.PublishAsync(OrderEvent.From(order, OrderEventKinds.Paid));
        return order;
    }
}
