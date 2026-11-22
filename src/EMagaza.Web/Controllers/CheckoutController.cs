using EMagaza.Application.Interfaces.Services;
using EMagaza.Application.Models;
using EMagaza.Domain.Exceptions;
using EMagaza.Web.Extensions;
using EMagaza.Web.Models;
using EMagaza.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMagaza.Web.Controllers;

[Authorize]
public class CheckoutController(
    ICartStore cartStore,
    ICheckoutService checkout,
    IAddressService addresses,
    ILogger<CheckoutController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart = cartStore.Get();
        if (cart.Lines.Count == 0) return RedirectToAction("Index", "Cart");

        ViewBag.Cart = cart;
        ViewBag.Addresses = await addresses.GetAsync(User.GetUserId()!);
        return View(new CheckoutForm { CustomerName = User.Identity?.Name ?? string.Empty, Email = User.GetEmail() ?? string.Empty });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutForm form)
    {
        var cart = cartStore.Get();
        if (cart.Lines.Count == 0) return RedirectToAction("Index", "Cart");
        ViewBag.Cart = cart;

        if (!ModelState.IsValid) return await RedisplayAsync(form);

        try
        {
            var order = await checkout.PlaceOrderAsync(
                cart,
                new ShippingInfo(form.CustomerName, form.Email, form.Phone, form.City, form.Address),
                new PaymentCard(form.CardNumber, form.CardHolder, form.Expiry, form.Cvc),
                User.GetUserId()!);

            cart.Clear();
            cartStore.Save(cart);
            return RedirectToAction("Detail", "Orders", new { id = order.Id, placed = true });
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Sipariş oluşturulamadı");
            ModelState.AddModelError(string.Empty, "Beklenmeyen bir hata oluştu, kartından para çekilmedi. Lütfen tekrar dene.");
        }
        return await RedisplayAsync(form);
    }

    // Hata sayfasında kart numarası ve CVC geri yazılmaz.
    private async Task<IActionResult> RedisplayAsync(CheckoutForm form)
    {
        ViewBag.Addresses = await addresses.GetAsync(User.GetUserId()!);
        form.CardNumber = string.Empty;
        form.Cvc = string.Empty;
        ModelState.Remove(nameof(form.CardNumber));
        ModelState.Remove(nameof(form.Cvc));
        return View(form);
    }
}
