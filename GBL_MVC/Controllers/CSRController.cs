 
using Microsoft.AspNetCore.Mvc; 
using GBL_BusinessLogic.BAL; 
using GBL_BusinessLogic.Entity;
namespace GBL_MVC.Controllers;

public class CSRController : Controller
{
    private readonly ILogger<CSRController> _logger;
    private readonly CSR_BAL _bal;
    private readonly PartialViewRenderer _partialViewRenderer;


    public CSRController(ILogger<CSRController> logger, IConfiguration configuration, PartialViewRenderer partialViewRenderer)
    {
        _logger = logger;
        _bal = new CSR_BAL(configuration);
        _partialViewRenderer = partialViewRenderer;
    }

    public IActionResult Index(string title)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "corporate-social-responsibility";
            }
            var data = _bal.GetPageData_GroupedComponents_BAL(title, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/CSR/Index:", ex);
            return View(new CSRModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

}