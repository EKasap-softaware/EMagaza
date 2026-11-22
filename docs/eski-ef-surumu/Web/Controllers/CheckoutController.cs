using EMagaza.Application.Abstractions;
using EMagaza.Application.Orders;
using EMagaza.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace EMagaza.Web.Controllers;

public class CheckoutForm
{
    [Required(ErrorMessage = "Ad soyad zorunlu")] public string CustomerName { get; set; } = "";
    [Required, EmailAddress(ErrorMessage = "Geçerli bir e-posta girin")] public string Email { get; set; } = "";
    [Required(ErrorMessage = "Adres zorunlu")] public string Address { get; set; } = "";
}

public class CheckoutController(CheckoutService checkout, IOrderRepository orders) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var cart = HttpContext.Session.GetCart();
        if (cart.Lines.Count == 0) return RedirectToAction("Index", "Cart");
        ViewBag.Cart = cart;
        return View(new CheckoutForm());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutForm form)
    {
        var cart = HttpContext.Session.GetCart();
        ViewBag.Cart = cart;
        if (!ModelState.IsValid) return View(form);
        try
        {
            var order = await checkout.PlaceOrderAsync(cart, new CheckoutRequest(form.CustomerName, form.Email, form.Address));
            cart.Clear();
            HttpContext.Session.SaveCart(cart);
            return RedirectToAction(nameof(Done), new { id = order.Id });
        }
        catch (InvalidOperationException ex) // örn. yetersiz stok
        {
            ModelState.AddModelError("", ex.Message);
            return View(form);
        }
    }

    public async Task<IActionResult> Done(int id)
    {
        var order = await orders.GetAsync(id);
        return order is null ? NotFound() : View(order);
    }
}
