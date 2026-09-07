using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.Entity;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers.Manage
{
  [Authorize]
[SessionAuthorize]
public class MenuController : Controller
{
    private readonly MenuTreebuilder_bal _bal;
     
    public MenuController(MenuTreebuilder_bal bal)
    {
        _bal = bal;
    }

    //-------------------------------------------------
    // 👉 ADMIN PAGE — ADD MENU
    //-------------------------------------------------
    public IActionResult Create()
    {
        ViewBag.Templates = _bal.GetTemplates();
        ViewBag.Menus = _bal.GetMenus();

        return View();
    }

    //-------------------------------------------------
    // 👉 GENERATE FINAL MENU HTML
    //-------------------------------------------------
    public IActionResult GenerateMenu()
    {
        // 1️⃣ Get Data
        var items = _bal.GetMenus();
        var templates = _bal.GetTemplates();

        // 2️⃣ Build Tree
        var tree = _bal.BuildTree(items);

        // 3️⃣ Generate HTML
        MenuRenderer_bal renderer = new();
        string html = renderer.Render(tree, templates);

        // 4️⃣ Save HTML to DB
        _bal.SaveMenuHtml("MainMenu", html);

        return Content("Menu Generated Successfully");
    }

    //-------------------------------------------------
    // 👉 LOAD MENU FOR SITE
    //-------------------------------------------------
    public IActionResult LoadMenu()
    {
        ViewBag.MenuHtml = _bal.GetMenuHtml("MainMenu");

        return View();
    }
}
}