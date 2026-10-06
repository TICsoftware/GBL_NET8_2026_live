using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Models;
using GBL_MVC.Classes;
using Core_project_BusinessLogic;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc.Rendering;
using GBL_MVC;
namespace GBL_MVC.Controllers;

public class SustainabilityController : Controller
{
    private readonly ILogger<SustainabilityController> _logger;
    private readonly Sustainability_BAL _bal;
    private readonly PartialViewRenderer _partialViewRenderer;


    public SustainabilityController(ILogger<SustainabilityController> logger, IConfiguration configuration, PartialViewRenderer partialViewRenderer)
    {
        _logger = logger;
        _bal = new Sustainability_BAL(configuration);
        _partialViewRenderer = partialViewRenderer;
    }

    public IActionResult Index()
    {
        return View();
    }



    public IActionResult SustainabilityReports(string title)
    {
        try
        {
            var data = _bal.GetSustainabilityReports_BAL(title, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/SustainabilityReports :", ex);
            return View(new SustainabilityModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }


    [HttpGet]
    public async Task<IActionResult> LoadSustainabilityReports(int contentId, int pageNumber = 1, int pageSize = 10)
    {
        try
        {
            var result = _bal.GetSustainabilityReports_page_wise_BAL(contentId, pageNumber, pageSize);

            var html = await _partialViewRenderer.RenderPartialToStringAsync(
                this,
                "_sustainability_reports_list",
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
            FileLogger.LogError("/LoadSustainabilityReports :", ex);

            return StatusCode(500, new
            {
                message = "Unable to load sustainability reports."
            });
        }
    }










}
