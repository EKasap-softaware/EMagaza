using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Application.Models;
using EMagaza.Web.Extensions;
using EMagaza.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMagaza.Web.Controllers;

public class ShopController(
    IProductRepository products,
    IReviewRepository reviews,
    IFavoriteService favorites,
    ILogger<ShopController> logger) : Controller
{
    public async Task<IActionResult> Index(string? category, string? type, string? size, string? q, ProductSort? sort,
        bool sale = false, bool isnew = false, int page = 1)
    {
        var query = new ProductQuery(Clean(category), Clean(type), Clean(size), Clean(q), sort ?? ProductSort.Featured, sale, isnew, page);

        return View(new ShopViewModel
        {
            Query = query,
            Categories = await products.GetCategoriesAsync(),
            Types = await products.GetTypesAsync(query.Category),
            Result = await products.SearchAsync(query)
        });
    }

    public async Task<IActionResult> Detail(string id)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null) return NotFound();

        ViewBag.Related = await products.GetRelatedAsync(product, 4);
        ViewBag.IsFavorite = User.Identity?.IsAuthenticated == true && await favorites.IsFavoriteAsync(User.GetUserId()!, id);
        try
        {
            ViewBag.Reviews = await reviews.GetByProductAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Yorumlar okunamadı");   // yorumlar gelmese de ürün sayfası açılsın
            ViewBag.ReviewsError = true;
        }
        return View(product);
    }

    public IActionResult Error() => View();

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
