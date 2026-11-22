using System.Security.Claims;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;
using EMagaza.Web.Extensions;
using EMagaza.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMagaza.Web.Controllers;

public class AccountController(IAuthService auth, IAddressService addresses) : Controller
{
    [Authorize]
    public async Task<IActionResult> Profile() => View(await BuildProfileAsync());

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword([Bind(Prefix = "Password")] ChangePasswordForm form)
    {
        if (!ModelState.IsValid)
        {
            TempData["Toast"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
            return RedirectToAction(nameof(Profile));
        }

        try
        {
            await auth.ChangePasswordAsync(User.GetUserId()!, form.CurrentPassword, form.NewPassword);
            TempData["Toast"] = "Şifren güncellendi";
        }
        catch (DomainException ex)
        {
            TempData["Toast"] = ex.Message;
        }
        return RedirectToAction(nameof(Profile));
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAddress([Bind(Prefix = "NewAddress")] AddressForm form)
    {
        if (!ModelState.IsValid)
        {
            TempData["Toast"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
            return RedirectToAction(nameof(Profile));
        }

        try
        {
            await addresses.AddAsync(User.GetUserId()!, form.Title, form.FullName, form.Phone, form.City, form.Line);
            TempData["Toast"] = "Adres kaydedildi";
        }
        catch (DomainException ex)
        {
            TempData["Toast"] = ex.Message;
        }
        return RedirectToAction(nameof(Profile));
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAddress(string id)
    {
        await addresses.DeleteAsync(User.GetUserId()!, id);
        TempData["Toast"] = "Adres silindi";
        return RedirectToAction(nameof(Profile));
    }

    private async Task<ProfileViewModel> BuildProfileAsync() => new()
    {
        DisplayName = User.Identity!.Name!,
        Email = User.GetEmail() ?? string.Empty,
        Addresses = await addresses.GetAsync(User.GetUserId()!)
    };

    [HttpGet]
    public IActionResult Register(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new RegisterForm());
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginForm());
    }

    public IActionResult Denied() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterForm form, string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (!ModelState.IsValid) return View(form);

        try
        {
            var user = await auth.RegisterAsync(form.DisplayName, form.Email, form.Password);
            await SignInAsync(user);
            return LocalRedirect(SafeReturnUrl(returnUrl));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(form);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginForm form, string? returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        if (!ModelState.IsValid) return View(form);

        var user = await auth.LoginAsync(form.Email, form.Password);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
            return View(form);
        }

        await SignInAsync(user);
        return LocalRedirect(SafeReturnUrl(returnUrl));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    private Task SignInAsync(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    // Open redirect saldırısına karşı: sadece yerel adreslere yönlendirilir.
    private string SafeReturnUrl(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
}
