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

public class BusinessesController : Controller
{
    private readonly ILogger<BusinessesController> _logger;
    private readonly Business_BAL _bal;
    private readonly PartialViewRenderer _partialViewRenderer;


    public BusinessesController(ILogger<BusinessesController> logger, IConfiguration configuration, PartialViewRenderer partialViewRenderer)
    {
        _logger = logger;
        _bal = new Business_BAL(configuration);
        _partialViewRenderer = partialViewRenderer;
    }

    public IActionResult Index(string title)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                title ="Businesses";
            }
            var data = _bal.GetBusiness_BAL(title, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Businesses :", ex);
            return View(new SustainabilityModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

    public IActionResult Inside(string title)
    {
         try
        {
             var data = "";
             return View(data);
        }
         catch (Exception ex)
        {
            FileLogger.LogError("/Businesses :", ex);
            return View(new SustainabilityModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

}
