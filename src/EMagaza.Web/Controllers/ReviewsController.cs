using EMagaza.Application.Interfaces.Services;
using EMagaza.Domain.Exceptions;
using EMagaza.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMagaza.Web.Controllers;

[Authorize]
public class ReviewsController(IReviewService reviews, ILogger<ReviewsController> logger) : Controller
{
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string productId, int rating, string comment)
    {
        try
        {
            await reviews.AddAsync(User.GetUserId()!, User.Identity!.Name!, productId, rating, comment);
            TempData["Toast"] = "Yorumun kaydedildi";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            TempData["Toast"] = ex.Message;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Yorum kaydedilemedi");
            TempData["Toast"] = "Yorum şu an kaydedilemedi, sonra tekrar dene";
        }
        return RedirectToAction("Detail", "Shop", new { id = productId });
    }
}
