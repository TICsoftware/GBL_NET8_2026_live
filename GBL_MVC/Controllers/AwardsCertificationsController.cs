using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Models;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;

namespace GBL_MVC.Controllers;

public class AwardsCertificationsController : Controller
{
    private readonly ILogger<AwardsCertificationsController> _logger;
    private readonly AwardsCertifications_BAL _bal;

    public AwardsCertificationsController(
        ILogger<AwardsCertificationsController> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _bal = new AwardsCertifications_BAL(configuration);
    }

    public IActionResult Index(string title = "awards")
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title) ? "awards" : title.Trim().TrimStart('/');
            var data = _bal.GetAwardsCertifications_BAL(pageName, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/AwardsCertifications :", ex);
            return View(new AwardsCertificationsModel());
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
