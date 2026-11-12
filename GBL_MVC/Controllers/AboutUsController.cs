using Microsoft.AspNetCore.Mvc;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers;

public class AboutUsController : Controller
{
    private readonly AboutUs_BAL _bal;

    public AboutUsController(IConfiguration configuration)
    {
        _bal = new AboutUs_BAL(configuration);
    }

    public IActionResult AboutUs(string title)
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title) ? "about-us" : title;
            var data = _bal.GetAboutUs_BAL(pageName, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/AboutUs :", ex);
            return View(new AboutModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

    public IActionResult OurLegacy(string title)
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title) ? "our-legacy" : title;
            var data = _bal.GetLegacyPage_BAL(pageName, 1, 1);
            return View("~/Views/OurLegacy/Index.cshtml", data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/OurLegacy :", ex);
            return View("~/Views/OurLegacy/Index.cshtml", new AboutModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }
}
