

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Models;
using GBL_MVC.Classes;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BAL;

namespace GBL_MVC.Controllers;

public class MenuDisplayController : Controller
{
     private readonly MenuTreebuilder_bal _bal;
    

    public MenuDisplayController(IConfiguration config)
    {
        _bal = new MenuTreebuilder_bal(config);
        
    }

public IActionResult Index()
{
    ViewBag.MenuHtml = _bal.GetMenuHtml();

    return View();
}
}