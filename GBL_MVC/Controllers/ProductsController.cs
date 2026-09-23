using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Models;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic;
using GBL_MVC.Helpers;

namespace GBL_MVC.Controllers;

public class ProductsController : Controller
{
    private readonly ILogger<ProductsController> _logger;


    public ProductsController(ILogger<ProductsController> logger, IConfiguration configuration)
    {
        _logger = logger;   
    }

    public IActionResult Index()
    {
        return View();
    }

public IActionResult Index_html()
    {
        return View();
    }

    public IActionResult Inside_html()
    {
        return View();
    }
}
