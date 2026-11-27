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

        public IActionResult Index(string Id, string cat)
        {
            Search model = new Search();

            if (!string.IsNullOrWhiteSpace(Id))
            {
                try
                {
                    model.txttitle = Id.ToString();
                    model = _bal.search_display(Id.Replace("of ", ""), 1, cat);
                }
                catch (Exception ex)
                {
                    FileLogger.LogError("/Search/Index :", ex);
                    return View(new Search());
                }
            }
            return View(model);

        }


        // Quick results for the header search overlay: GET /search/suggest?q=...&cat=Investors
        [HttpGet]
        public IActionResult Suggest(string q, string cat)
        {
            try
            {
                return Json(_bal.search_suggest(q, cat));
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/Search/Suggest :", ex);
                return Json(new SearchSuggestResult { q = q, cat = cat ?? "All" });
            }
        }

        // Sections and articles for a chip, before a keyword is typed.
        [HttpGet]
        public IActionResult Browse(string cat)
        {
            try
            {
                return Json(_bal.search_browse(cat));
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/Search/Browse :", ex);
                return Json(new SearchSuggestResult { q = "", cat = cat ?? "All" });
            }
        }

        // Most searched keywords, and the sections or articles people open.
        [HttpGet]
        public IActionResult Popular()
        {
            try
            {
                return Json(_bal.search_popular());
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/Search/Popular :", ex);
                return Json(new SearchPopularResult());
            }
        }

        // Records a search and the result that was opened.
        [HttpGet]
        public IActionResult Track(string q, string cat, string title, string url, string type, string section)
        {
            try
            {
                _bal.search_track(q, cat, title, url, type, section);
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/Search/Track :", ex);
            }

            return Json(new { ok = true });
        }

        public ActionResult LoadMoreSearch(string keyword, int page, string cat)
        {
            try
            {
                Search model = _bal.search_display(keyword, page, cat);

                if (model == null ||
                    model.objsearchdisplay == null ||
                    model.objsearchdisplay.Count == 0)
                {
                    return Content("");
                }

                ViewData["SearchKeyword"] = keyword;
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