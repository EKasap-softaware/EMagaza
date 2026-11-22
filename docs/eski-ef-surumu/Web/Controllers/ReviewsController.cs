using EMagaza.Application.Abstractions;
using EMagaza.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
namespace EMagaza.Web.Controllers;

public class ReviewsController(IReviewRepository reviews, ILogger<ReviewsController> log) : Controller
{
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, string author, int rating, string comment)
    {
        try
        {
            await reviews.AddAsync(Review.Create(productId, author, rating, comment));
            TempData["Toast"] = "Yorumun kaydedildi";
        }
        catch (ArgumentException ex) { TempData["Toast"] = ex.Message; }          // doğrulama (puan, boş alan)
        catch (Exception ex)                                                        // Mongo erişilemiyor
        {
            log.LogError(ex, "Yorum kaydedilemedi");
            TempData["Toast"] = "Yorum şu an kaydedilemedi, sonra tekrar dene";
        }
        return RedirectToAction("Detail", "Shop", new { id = productId });
    }
}
