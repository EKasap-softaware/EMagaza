using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Domain.Constants;
using EMagaza.Domain.Exceptions;
using EMagaza.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMagaza.Web.Controllers;

[Authorize]
public class OrdersController(IOrderRepository orders, IOrderManagementService orderManagement) : Controller
{
    public async Task<IActionResult> Index() => View(await orders.GetByUserAsync(User.GetUserId()!));

    public async Task<IActionResult> Detail(string id, bool placed = false)
    {
        var order = await orders.GetByIdAsync(id);

        // Başkasının siparişi için 404 döner; varlığı bile açıklanmaz. Yönetici hepsini görebilir.
        if (order is null || (order.UserId != User.GetUserId() && !User.IsInRole(Roles.Admin))) return NotFound();

        ViewBag.Placed = placed && order.UserId == User.GetUserId();
        return View(order);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(string id)
    {
        try
        {
            await orderManagement.CancelAsync(id, User.GetUserId()!, User.IsInRole(Roles.Admin));
            TempData["Toast"] = "Siparişin iptal edildi, ödemen iade edilecek";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            TempData["Toast"] = ex.Message;
        }
        return RedirectToAction(nameof(Detail), new { id });
    }
}
