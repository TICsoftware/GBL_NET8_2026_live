using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers
{
    public class IndustriesController : Controller
    {
        private const int DefaultPageSize = 10;
        private readonly Industry_Public_BAL _bal;

        public IndustriesController(IConfiguration configuration)
        {
            _bal = new Industry_Public_BAL(configuration);
        }

        public IActionResult Index(string? title, int? industry, int? application, int? productCategories)
        {
            try
            {
                var pageName = ResolvePageName(title);
                var filter = BuildFilter(industry, application, productCategories, 1);
                var model = _bal.GetIndex(pageName, 1, filter);
                return View(model);
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/Industries/Index :", ex);
                return View(new Industry_Public_Page());
            }
        }

        [HttpPost]
        public IActionResult LoadIndustries(Industry_Public_Filter filter)
        {
            try
            {
                filter ??= new Industry_Public_Filter();
                filter.PageNumber = 1;
                if (filter.PageSize < 1) filter.PageSize = DefaultPageSize;
                filter.LanguageId = 1;
                var data = _bal.GetPaged(filter);
                return Json(new
                {
                    success = true,
                    items = data.Data,
                    total = data.Total,
                    page = 1,
                    pageSize = filter.PageSize
                });
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/Industries/LoadIndustries : Post :", ex);
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
        }

        [HttpPost]
        public IActionResult LoadMore(Industry_Public_Filter filter)
        {
            try
            {
                filter ??= new Industry_Public_Filter();
                if (filter.PageNumber < 1) filter.PageNumber = 1;
                if (filter.PageSize < 1) filter.PageSize = DefaultPageSize;
                filter.LanguageId = 1;
                var data = _bal.GetPaged(filter);
                return Json(new
                {
                    success = true,
                    items = data.Data,
                    total = data.Total,
                    page = filter.PageNumber,
                    pageSize = filter.PageSize
                });
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/Industries/LoadMore : Post :", ex);
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
        }

        private string ResolvePageName(string? title)
        {
            if (!string.IsNullOrWhiteSpace(title))
                return title.Trim().Trim('/');

            var path = HttpContext?.Request?.Path.Value?.Trim('/') ?? string.Empty;
            return string.IsNullOrWhiteSpace(path) ? "industries" : path;
        }

        private static Industry_Public_Filter BuildFilter(int? industry, int? application, int? category, int page)
        {
            return new Industry_Public_Filter
            {
                IndustryId = industry,
                ApplicationId = application,
                CategoryId = category,
                PageNumber = page,
                PageSize = DefaultPageSize,
                LanguageId = 1
            };
        }
    }
}
