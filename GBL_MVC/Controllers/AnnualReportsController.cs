using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers;

public class AnnualReportsController : Controller
{
    private readonly ILogger<AnnualReportsController> _logger;
    private readonly Investors_BAL _bal;
    private readonly PartialViewRenderer _partialViewRenderer;

    public AnnualReportsController(
        ILogger<AnnualReportsController> logger,
        IConfiguration configuration,
        PartialViewRenderer partialViewRenderer)
    {
        _logger = logger;
        _bal = new Investors_BAL(configuration);
        _partialViewRenderer = partialViewRenderer;
    }

    public IActionResult Index(string title)
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title)
                ? "annual-reports"
                : title.Trim().TrimStart('/');

            var data = _bal.GetPdfListing_BAL(pageName, 1, 1, Investors_BAL.AnnualReportPageSize);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/AnnualReports :", ex);
            return View(new InvestorsModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

    [HttpGet]
    public async Task<IActionResult> LoadReports(
        int contentId,
        int pageNumber = 1,
        int pageSize = Investors_BAL.AnnualReportPageSize)
    {
        try
        {
            if (pageSize < 1) pageSize = Investors_BAL.AnnualReportPageSize;

            var result = _bal.GetPdfListing_page_wise_BAL(contentId, pageNumber, pageSize);

            var html = await _partialViewRenderer.RenderPartialToStringAsync(
                this,
                "_annual_report_list",
                result.SectionArticles_List
            );

            return Json(new
            {
                html,
                totalCount = result.TotalCount,
                pageNumber,
                pageSize
            });
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/AnnualReports/LoadReports :", ex);
            return StatusCode(500, new { message = "Unable to load annual reports." });
        }
        finally
        {
            _bal.Dispose();
        }
    }
}
