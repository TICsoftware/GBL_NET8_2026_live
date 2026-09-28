using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using GBL_MVC;

namespace GBL_MVC.Controllers;

public class ProductsController : Controller
{
    private readonly ILogger<ProductsController> _logger;
    private readonly Product_Public_BAL _bal;

    public ProductsController(ILogger<ProductsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bal = new Product_Public_BAL(configuration);
    }

    public IActionResult Index()
    {
        return RedirectToAction("Index_html");
    }

    public IActionResult Index_html()
    {
        return View();
    }

    public IActionResult Inside_html()
    {
        return View();
    }

    public IActionResult Inside(string? title)
    {
        try
        {
            var pageName = (title ?? string.Empty).Trim().Trim('/');
            if (string.IsNullOrWhiteSpace(pageName))
                return Redirect("/products");

            var model = _bal.GetInside(pageName, 1);
            if (model.Product == null || model.Product.ProductId <= 0)
                return Redirect("/products");

            return View(model);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Products/Inside :", ex);
            return Redirect("/products");
        }
    }
}
