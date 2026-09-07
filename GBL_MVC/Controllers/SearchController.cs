using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly Search_BAL _bal;

        public SearchController(ILogger<SearchController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _bal = new Search_BAL(configuration);
        }

        public IActionResult Index(string Id)
        {
            Search model = new Search();

            if (!string.IsNullOrWhiteSpace(Id))
            {
                try
                {
                    model.txttitle = Id.ToString();
                    model = _bal.search_display(Id.Replace("of ", ""), 1);
                }
                catch (Exception ex)
                {
                    FileLogger.LogError("/Search/Index :", ex);
                    return View(new Search());
                }
            }
            return View(model);

        }


        public ActionResult LoadMoreSearch(string keyword, int page)
        {
            try
            {
                Search model = _bal.search_display(keyword, page);

                if (model == null ||
                    model.objsearchdisplay == null ||
                    model.objsearchdisplay.Count == 0)
                {
                    return Content("");
                }

                return PartialView("_LoadMoreSearch", model.objsearchdisplay);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;

                return Content("Error : " + ex.Message);
            }
        }


    }
}