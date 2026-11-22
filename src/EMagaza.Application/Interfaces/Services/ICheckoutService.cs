using EMagaza.Application.Carts;
using EMagaza.Application.Models;
using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Services;

public interface ICheckoutService
{
    Task<Order> PlaceOrderAsync(Cart cart, ShippingInfo shipping, PaymentCard card, string userId);
}
