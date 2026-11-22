using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Application.Models;
using EMagaza.Domain.Constants;
using EMagaza.Domain.Exceptions;
using EMagaza.Web.Extensions;
using EMagaza.Web.Helpers;
using EMagaza.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMagaza.Web.Controllers;

[Authorize(Roles = Roles.Admin)]
public class AdminController(
    IDashboardService dashboard,
    IOrderManagementService orderManagement,
    IUserManagementService userManagement,
    IProductManagementService productManagement,
    IProductRepository products,
    IUserRepository users) : Controller
{
    public async Task<IActionResult> Index() => View(await dashboard.GetSummaryAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Advance(string id, string? tracking)
    {
        try
        {
            var order = await orderManagement.AdvanceAsync(id, tracking);
            TempData["Toast"] = $"{order.Number} → {StatusText.Label(order.Status)} (müşteriye bildirim kuyruğa bırakıldı)";
        }
        catch (DomainException ex)
        {
            TempData["Toast"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(string id)
    {
        try
        {
            var order = await orderManagement.CancelAsync(id, User.GetUserId()!, isAdmin: true);
            TempData["Toast"] = $"{order.Number} iptal edildi, stok geri verildi, ödeme iade edildi";
        }
        catch (DomainException ex)
        {
            TempData["Toast"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    // ---- Ürün yönetimi ----

    public async Task<IActionResult> Products() => View((await products.SearchAsync(ProductQuery.Everything)).Items);

    [HttpGet]
    public async Task<IActionResult> ProductEdit(string? id)
    {
        if (id is null) return View(new ProductForm());

        var product = await products.GetByIdAsync(id);
        return product is null ? NotFound() : View(ProductForm.From(product));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductEdit(ProductForm form)
    {
        if (!ModelState.IsValid) return View(form);

        try
        {
            var input = form.ToInput();
            if (string.IsNullOrEmpty(form.Id)) await productManagement.CreateAsync(input);
            else await productManagement.UpdateAsync(form.Id, input);

            TempData["Toast"] = "Ürün kaydedildi";
            return RedirectToAction(nameof(Products));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(form);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductDelete(string id)
    {
        try
        {
            await productManagement.DeleteAsync(id);
            TempData["Toast"] = "Ürün silindi";
        }
        catch (DomainException ex)
        {
            TempData["Toast"] = ex.Message;
        }
        return RedirectToAction(nameof(Products));
    }

    // ---- Kullanıcılar ----

    public async Task<IActionResult> Users() => View(await users.GetAllAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(string userId, string role)
    {
        try
        {
            await userManagement.ChangeRoleAsync(User.GetUserId()!, userId, role);
            TempData["Toast"] = "Rol güncellendi (kullanıcı için bir sonraki girişte geçerli olur)";
        }
        catch (DomainException ex)
        {
            TempData["Toast"] = ex.Message;
        }
        return RedirectToAction(nameof(Users));
    }
}
