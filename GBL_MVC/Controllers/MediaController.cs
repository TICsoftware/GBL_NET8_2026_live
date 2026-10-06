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

public class MediaController : Controller
{
    private readonly ILogger<MediaController> _logger;
    private readonly Media_BAL _bal;
    private readonly PartialViewRenderer _partialViewRenderer;


    public MediaController(ILogger<MediaController> logger, IConfiguration configuration, PartialViewRenderer partialViewRenderer)
    {
        _logger = logger;
        _bal = new Media_BAL(configuration);
        _partialViewRenderer = partialViewRenderer;
    }

    public IActionResult Index()
    {
        return View();
    }



    public IActionResult PressReleases(string title)
    {
        try
        {
            var data = _bal.GetPressRelease_BAL(title, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/PressReleases :", ex);
            return View(new MediaModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

    public IActionResult MediaCoverage(string title)
    {
        try
        {
            var data = _bal.GetMediaCoverage_BAL(title, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/PressReleases :", ex);
            return View(new MediaModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }



    public IActionResult PressReleasesInside(string title)
    {
        try
        {
            var data = _bal.GetPressRelease_Inside_BAL(title, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/PressReleases :", ex);
            return View(new AboutModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }



    [HttpGet]
    public async Task<IActionResult> LoadPressReleases(int contentId, int pageNumber = 1, int pageSize = 12, string? topic = null, string? month = null, int? year = null)
    {
        int? tagId = int.TryParse(topic, out int parsedTagId)
            ? parsedTagId
            : null;

        int? selectedMonth = int.TryParse(month, out int parsedMonth)
            ? parsedMonth
            : null;

        // Fetch paginated articles with filters
        var result = _bal.GetPressReleases_page_wise_BAL(contentId, pageNumber, pageSize, year, selectedMonth, tagId);

        // Render the partial view as HTML
        var html = await _partialViewRenderer.RenderPartialToStringAsync(
            this,
            "_press_release_list",
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


     [HttpGet]
    public async Task<IActionResult> LoadMediaCoverage(int contentId, int pageNumber = 1, int pageSize = 12, string? topic = null, string? month = null, int? year = null)
    {
        int? tagId = int.TryParse(topic, out int parsedTagId)
            ? parsedTagId
            : null;

        int? selectedMonth = int.TryParse(month, out int parsedMonth)
            ? parsedMonth
            : null;

        // Fetch paginated articles with filters
        var result = _bal.GetPressReleases_page_wise_BAL(contentId, pageNumber, pageSize, year, selectedMonth, tagId);

        // Render the partial view as HTML
        var html = await _partialViewRenderer.RenderPartialToStringAsync(
            this,
            "_media_coverage_list",
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









}
