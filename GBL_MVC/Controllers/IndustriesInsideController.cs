using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers
{
    public class IndustriesInsideController : Controller
    {
        private const int DefaultPageSize = 6;
        private readonly Industry_Public_BAL _bal;

        public IndustriesInsideController(IConfiguration configuration)
        {
            _bal = new Industry_Public_BAL(configuration);
        }

        public IActionResult Index(string? title, int? category)
        {
            try
            {
                var pageName = (title ?? string.Empty).Trim().Trim('/');
                if (string.IsNullOrWhiteSpace(pageName))
                    return Redirect("/industries");

                var model = _bal.GetInside(pageName, 1, category ?? 0, 1, DefaultPageSize);
                if (model.Industry == null || model.Industry.IndustryId <= 0)
                    return Redirect("/industries");

                return View(model);
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/IndustriesInside/Index :", ex);
                return Redirect("/industries");
            }
        }

        [HttpPost]
        public IActionResult LoadProducts(Industry_Inside_Filter filter)
        {
            try
            {
                filter ??= new Industry_Inside_Filter();
                filter.PageNumber = 1;
                if (filter.PageSize < 1) filter.PageSize = DefaultPageSize;
                filter.LanguageId = 1;
                var data = _bal.GetInsideProducts(filter);
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
                FileLogger.LogError("/IndustriesInside/LoadProducts : Post :", ex);
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
        }

        [HttpPost]
        public IActionResult LoadMore(Industry_Inside_Filter filter)
        {
            try
            {
                filter ??= new Industry_Inside_Filter();
                if (filter.PageNumber < 1) filter.PageNumber = 1;
                if (filter.PageSize < 1) filter.PageSize = DefaultPageSize;
                filter.LanguageId = 1;
                var data = _bal.GetInsideProducts(filter);
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
                FileLogger.LogError("/IndustriesInside/LoadMore : Post :", ex);
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
        }
    }
}
