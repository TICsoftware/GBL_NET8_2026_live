using System;
using System.Linq;
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
                    items = ToProductItems(data.Data),
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
                    items = ToProductItems(data.Data),
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

        /// <summary>
        /// Explicit JSON shape for tab AJAX: imageUrl = thumbnail or masthead fallback.
        /// </summary>
        private static object ToProductItems(System.Collections.Generic.List<Industry_Inside_Product> products)
        {
            return (products ?? new System.Collections.Generic.List<Industry_Inside_Product>()).Select(p =>
            {
                var thumb = (p.ThumbnailUrl ?? string.Empty).Trim();
                var banner = (p.BannerUrl ?? string.Empty).Trim();
                var imageUrl = !string.IsNullOrWhiteSpace(thumb) ? thumb : banner;
                var imageAlt = !string.IsNullOrWhiteSpace(thumb)
                    ? (p.ThumbnailAlt ?? p.ProductName)
                    : (p.BannerAlt ?? p.ProductName);
                return new
                {
                    productId = p.ProductId,
                    productName = p.ProductName,
                    product_pagename = p.Product_pagename,
                    intro = p.Intro,
                    thumbnailUrl = thumb,
                    thumbnailAlt = p.ThumbnailAlt,
                    bannerUrl = banner,
                    bannerAlt = p.BannerAlt,
                    imageUrl,
                    imageAlt,
                    applications = p.Applications ?? new System.Collections.Generic.List<string>()
                };
            }).ToList();
        }
    }
}
