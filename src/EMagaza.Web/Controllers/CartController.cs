using EMagaza.Application.Interfaces.Services;
using EMagaza.Domain.Exceptions;
using EMagaza.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace EMagaza.Web.Controllers;

public class CartController(ICartStore cartStore, ICartService cartService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var cart = cartStore.Get();
        await cartService.RefreshCouponAsync(cart);
        cartStore.Save(cart);
        return View(cart);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string id, string size, int quantity = 1)
    {
        var cart = cartStore.Get();
        try
        {
            await cartService.AddItemAsync(cart, id, size, quantity);
            cartStore.Save(cart);
            TempData["Toast"] = "Ürün sepete eklendi";
            TempData["ToastLink"] = Url.Action(nameof(Index));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            TempData["Toast"] = ex.Message;
        }
        return RedirectToAction("Detail", "Shop", new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetQty(string id, string size, int quantity)
    {
        var cart = cartStore.Get();
        await cartService.SetQuantityAsync(cart, id, size, quantity);
        cartStore.Save(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(string id, string size)
    {
        var cart = cartStore.Get();
        cart.Remove(id, size);
        await cartService.RefreshCouponAsync(cart);
        cartStore.Save(cart);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ApplyCoupon(string code)
    {
        var cart = cartStore.Get();
        try
        {
            await cartService.ApplyCouponAsync(cart, code);
            cartStore.Save(cart);
            TempData["Toast"] = $"{cart.CouponCode} kuponu uygulandı";
        }
        catch (DomainException ex)
        {
            TempData["Toast"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RemoveCoupon()
    {
        var cart = cartStore.Get();
        cart.ClearCoupon();
        cartStore.Save(cart);
        return RedirectToAction(nameof(Index));
    }
}
