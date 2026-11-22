using EMagaza.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
namespace EMagaza.Web.Controllers;

public class ShopController(IProductRepository products, IReviewRepository reviews, ILogger<ShopController> log) : Controller
{
    public async Task<IActionResult> Index(string? category, string? q)
    {
        ViewBag.Categories = await products.GetCategoriesAsync();
        ViewBag.Category = category;
        ViewBag.Query = q;
        return View(await products.SearchAsync(category, q));
    }

    public async Task<IActionResult> Detail(int id)
    {
        var p = await products.GetAsync(id);
        if (p is null) return NotFound();

        // Yorumlar MongoDB'den. Mongo kapalıysa ürün sayfası YİNE açılsın (zarif bozulma).
        try { ViewBag.Reviews = await reviews.GetByProductAsync(id); }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Yorumlar okunamadı");
            ViewBag.ReviewsError = true;
        }
        return View(p);
    }

    public IActionResult Error() => View();
}
