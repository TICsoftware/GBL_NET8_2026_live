using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers;

public class QuarterlyReportsController : Controller
{
    private readonly ILogger<QuarterlyReportsController> _logger;
    private readonly QuarterlyReports_BAL _bal;

    public QuarterlyReportsController(ILogger<QuarterlyReportsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bal = new QuarterlyReports_BAL(configuration);
    }

    public IActionResult Index(string title)
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title)
                ? "quarterly-reports"
                : title.Trim().TrimStart('/');

            var data = _bal.GetQuarterlyReports_BAL(pageName, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/QuarterlyReports :", ex);
            return View(new QuarterlyReportsModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }
}
