using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers;

public class InvestorRelationsController : Controller
{
    private readonly ILogger<InvestorRelationsController> _logger;
    private readonly InvestorRelations_BAL _bal;

    public InvestorRelationsController(ILogger<InvestorRelationsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bal = new InvestorRelations_BAL(configuration);
    }

    public IActionResult Index(string title)
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title)
                ? "investor-relations"
                : title.Trim().TrimStart('/');

            var data = _bal.GetInvestorRelations_BAL(pageName, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/InvestorRelations :", ex);
            return View(new InvestorRelationsModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }
}
