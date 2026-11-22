using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Application.Models;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Enums;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Services;

public class OrderManagementService(
    IOrderRepository orders,
    IProductRepository products,
    IPaymentGateway payments,
    ICouponService coupons,
    IOrderEventPublisher events) : IOrderManagementService
{
    public async Task<Order> AdvanceAsync(string orderId, string? trackingNumber = null)
    {
        var order = await orders.GetByIdAsync(orderId) ?? throw new NotFoundException("Sipariş bulunamadı.");
        if (order.Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new DomainException("Bu sipariş artık ilerletilemez.");

        order.MoveTo(order.Status + 1, trackingNumber);
        await orders.UpdateAsync(order);
        await events.PublishAsync(OrderEvent.From(order, OrderEventKinds.For(order.Status)));
        return order;
    }

    public async Task<Order> CancelAsync(string orderId, string requesterId, bool isAdmin)
    {
        var order = await orders.GetByIdAsync(orderId) ?? throw new NotFoundException("Sipariş bulunamadı.");
        if (order.UserId != requesterId && !isAdmin) throw new NotFoundException("Sipariş bulunamadı.");

        order.Cancel();   // kural Domain'de: kargolanmış sipariş iptal edilemez
        await orders.UpdateAsync(order);

        foreach (var item in order.Items)
            await products.IncreaseStockAsync(item.ProductId, item.Size, item.Quantity);

        await payments.RefundAsync(order.PaymentTransactionId);
        if (order.CouponCode is not null) await coupons.ReleaseAsync(order.CouponCode);

        await events.PublishAsync(OrderEvent.From(order, OrderEventKinds.Cancelled));
        return order;
    }
}
