using Microsoft.AspNetCore.Mvc;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers;

public class ResourceCircularityController : Controller
{
    private readonly ResourceCircularity_BAL _bal;

    public ResourceCircularityController(IConfiguration configuration)
    {
        _bal = new ResourceCircularity_BAL(configuration);
    }

    public IActionResult Index(string title)
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title) ? "resource-circularity" : title;
            var data = _bal.GetResourceCircularity_BAL(pageName, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/ResourceCircularity :", ex);
            return View(new ResourceCircularityModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }
}
