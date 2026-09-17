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

    public MediaController(ILogger<MediaController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bal = new Media_BAL(configuration);
    }

    public IActionResult Index()
    {
        return View();
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


    // public IActionResult PressReleases(string title)
    // {
    //     try
    //     {
    //         var data = _bal.GetPressReleases_BAL(title, 1, 1);
    //         return View(data);
    //     }
    //     catch (Exception ex)
    //     {
    //         FileLogger.LogError("/PressReleases :", ex);
    //         return View(new AboutModel());
    //     }
    //     finally
    //     {
    //         _bal.Dispose();
    //     }
    // }










}
