using Microsoft.AspNetCore.Mvc;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
namespace GBL_MVC.Controllers
{
    public class pagearticleController : Controller
    {
        private readonly IConfiguration objconfig;
        public pagearticleController(IConfiguration configuration)
        {
            // HttpContext.Session.SetString("userid", "1");
            objconfig = configuration;
        }

        public IActionResult Index(string title)
        {
            try
            {
                Page_Manage_BAL _bal = new Page_Manage_BAL(objconfig);
                if(string.IsNullOrWhiteSpace(title))
                {
                    title="contact";
                }
                var data = _bal.GetPageData_BAL(title, 1, 1);
                return View(data);
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/pagearticle/Index :", ex);
                return View(new PageViewModel());
            }
            finally
            {
                // _bal.Dispose();
            }
        }
        public IActionResult article(string title)
        {
            try
            {
                ViewBag.pagetitle= title;
                Page_Manage_BAL _bal = new Page_Manage_BAL(objconfig);
                var data = _bal.GetPageData_BAL(title, 1, 1);
                return View(data);
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/pagearticle/article :" + title, ex);
                return View(new PageViewModel());
            }
            finally
            {
                // _bal.Dispose();
            }
        }

        public IActionResult Error()
        {
            Response.StatusCode = 404;
            return View();
        }

    }
}