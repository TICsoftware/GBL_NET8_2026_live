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

public class CareerController : Controller
{
    private readonly ILogger<CareerController> _logger;
    private readonly Careers_BAL _bal;

    public CareerController(ILogger<CareerController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bal = new Careers_BAL(configuration);
    }


    public IActionResult Index(string title)
    {
        try
        {
            var data = _bal.GetCareers_BAL(title, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Careers :", ex);
            return View(new CareersModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }


    public IActionResult LearningDevelopment(string title)
    {
        try
        {
            var data = _bal.GetLearningDevelopment_BAL(title, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/LearningDevelopment :", ex);
            return View(new CareersModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }













}
