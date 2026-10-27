using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers;

public class CorporateGovernanceController : Controller
{
    private readonly ILogger<CorporateGovernanceController> _logger;
    private readonly CorporateGovernance_BAL _bal;

    public CorporateGovernanceController(ILogger<CorporateGovernanceController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bal = new CorporateGovernance_BAL(configuration);
    }

    public IActionResult Index(string title)
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title)
                ? "corporate-governance"
                : title.Trim().TrimStart('/');

            var data = _bal.GetCorporateGovernance_BAL(pageName, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/CorporateGovernance :", ex);
            return View(new CorporateGovernanceModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }
}
