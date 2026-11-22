using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMagaza.Web.Controllers;

[Authorize]
public class NotificationsController(INotificationRepository notifications) : Controller
{
    private const int PageSize = 50;

    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId()!;
        var list = await notifications.GetByUserAsync(userId, PageSize);
        await notifications.MarkAllReadAsync(userId);   // liste okunmamış halleriyle çizilir, sonra hepsi okundu olur
        return View(list);
    }

    /// <summary>Header'daki zil rozeti birkaç saniyede bir bunu sorar.</summary>
    [HttpGet]
    public async Task<IActionResult> Count()
    {
        try
        {
            return Json(new { unread = await notifications.CountUnreadAsync(User.GetUserId()!) });
        }
        catch
        {
            return Json(new { unread = 0 });
        }
    }

    /// <summary>Sipariş sayfası: bildirimin Worker tarafından yazılıp yazılmadığını sorar.</summary>
    [HttpGet]
    public async Task<IActionResult> ForOrder(string orderId, string kind = "paid")
    {
        var notification = await notifications.GetByOrderAsync(User.GetUserId()!, orderId, kind);
        return Json(new
        {
            found = notification is not null,
            title = notification?.Title ?? string.Empty,
            message = notification?.Message ?? string.Empty
        });
    }
}
