using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Models;
using EMagaza.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMagaza.Web.Controllers;

public class HomeController(IProductRepository products, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Index()
    {
        try
        {
            var model = new HomeViewModel
            {
                New = (await products.SearchAsync(new ProductQuery(NewOnly: true, Sort: ProductSort.Newest, PageSize: 4))).Items,
                Sale = (await products.SearchAsync(new ProductQuery(SaleOnly: true, PageSize: 4))).Items,
                Best = (await products.SearchAsync(new ProductQuery(PageSize: 8))).Items
            };
            return View(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ana sayfa verisi okunamadı");
            ViewBag.Error = true;
            return View(new HomeViewModel());
        }
    }
}
