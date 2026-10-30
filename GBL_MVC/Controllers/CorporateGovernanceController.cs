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
            var path = (Request?.Path.Value ?? "").Trim().Trim('/');
            var pageName = string.IsNullOrWhiteSpace(title)
                ? (string.IsNullOrWhiteSpace(path) ? "corporate-governance" : path)
                : title.Trim().TrimStart('/');

            if (!string.IsNullOrWhiteSpace(path)
                && path.StartsWith("investor-relations/", StringComparison.OrdinalIgnoreCase)
                && !pageName.StartsWith("investor-relations/", StringComparison.OrdinalIgnoreCase))
            {
                pageName = path;
            }

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
