using EMagaza.Application.Abstractions;
using EMagaza.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
namespace EMagaza.Web.Controllers;

public class CartController(IProductRepository products) : Controller
{
    public IActionResult Index() => View(HttpContext.Session.GetCart());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id, int quantity = 1)
    {
        var p = await products.GetAsync(id);
        if (p is null) return NotFound();
        var cart = HttpContext.Session.GetCart();
        cart.Add(p.Id, p.Name, p.Price, p.Emoji, Math.Max(1, quantity));
        HttpContext.Session.SaveCart(cart);
        TempData["Toast"] = $"{p.Name} sepete eklendi";
        return Redirect(Request.Headers.Referer.FirstOrDefault() ?? "/");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Remove(int id)
    {
        var cart = HttpContext.Session.GetCart();
        cart.Remove(id);
        HttpContext.Session.SaveCart(cart);
        return RedirectToAction(nameof(Index));
    }
}
