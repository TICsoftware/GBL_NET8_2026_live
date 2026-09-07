using Microsoft.AspNetCore.Mvc;

namespace GBL_MVC.Controllers
{    
     public class ManageReportsController : Controller
    {
         private readonly IConfiguration objconfig;
        public ManageReportsController(IConfiguration configuration)
        {             
            objconfig = configuration;
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            cms_Report_Category Model = new();
            return View(Model);
        }
        public IActionResult AddCategory(cms_Report_Category Model)
        { 
            return View(Model);
        }
    }
}