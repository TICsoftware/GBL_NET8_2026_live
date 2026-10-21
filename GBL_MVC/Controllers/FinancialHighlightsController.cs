using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Models;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic;
using GBL_MVC.Helpers;

namespace GBL_MVC.Controllers;

public class FinancialHighlightsController : Controller
{
    private readonly ILogger<FinancialHighlightsController> _logger;


    public FinancialHighlightsController(ILogger<FinancialHighlightsController> logger, IConfiguration configuration)
    {
        _logger = logger;   
    }

    public IActionResult Index()
    {
        return View();
    }


}
