using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers;

public class InvestorsController : Controller
{
    private readonly ILogger<InvestorsController> _logger;
    private readonly Investors_BAL _bal;
    private readonly PartialViewRenderer _partialViewRenderer;

    public InvestorsController(
        ILogger<InvestorsController> logger,
        IConfiguration configuration,
        PartialViewRenderer partialViewRenderer)
    {
        _logger = logger;
        _bal = new Investors_BAL(configuration);
        _partialViewRenderer = partialViewRenderer;
    }

    public IActionResult Pdflisting(string title)
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title)
                ? string.Empty
                : title.Trim().TrimStart('/');

            var data = _bal.GetPdfListing_BAL(pageName, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Investors/Pdflisting :", ex);
            return View(new InvestorsModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

    [HttpGet]
    public async Task<IActionResult> LoadPdfs(
        int contentId,
        int pageNumber = 1,
        int pageSize = Investors_BAL.DefaultPageSize,
        int? year = null)
    {
        try
        {
            if (pageSize < 1) pageSize = Investors_BAL.DefaultPageSize;

            var result = _bal.GetPdfListing_page_wise_BAL(contentId, pageNumber, pageSize, year);

            ViewData["PdfYear"] = year.HasValue && year.Value > 0
                ? year.Value + "-" + (year.Value + 1)
                : null;

            var html = await _partialViewRenderer.RenderPartialToStringAsync(
                this,
                "_investor_pdf_list",
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
            FileLogger.LogError("/Investors/LoadPdfs :", ex);
            return StatusCode(500, new { message = "Unable to load documents." });
        }
        finally
        {
            _bal.Dispose();
        }
    }
}
