using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Models;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;

namespace GBL_MVC.Controllers;

public class SocialController : Controller
{
    private readonly ILogger<SocialController> _logger;
    private readonly Careers_BAL _bal;

    public SocialController(ILogger<SocialController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bal = new Careers_BAL(configuration);
    }

    public IActionResult Index(string title = "sustainability/social")
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title)
                ? "sustainability/social"
                : title.Trim().TrimStart('/');
            var data = _bal.GetLearningDevelopment_BAL(pageName, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Social :", ex);
            return View(new CareersModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
