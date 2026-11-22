using EMagaza.Application.Interfaces.Services;
using EMagaza.Domain.Exceptions;
using EMagaza.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMagaza.Web.Controllers;

[Authorize]
public class FavoritesController(IFavoriteService favorites) : Controller
{
    public async Task<IActionResult> Index() => View(await favorites.GetProductsAsync(User.GetUserId()!));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(string productId, bool fromList = false)
    {
        try
        {
            var isFavorite = await favorites.ToggleAsync(User.GetUserId()!, productId);
            TempData["Toast"] = isFavorite ? "Favorilere eklendi" : "Favorilerden çıkarıldı";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }

        return fromList ? RedirectToAction(nameof(Index)) : RedirectToAction("Detail", "Shop", new { id = productId });
    }
}
