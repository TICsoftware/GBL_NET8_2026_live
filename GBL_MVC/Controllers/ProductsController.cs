using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Controllers;

public class ProductsController : Controller
{
    private const int DefaultPageSize = 9;
    private readonly ILogger<ProductsController> _logger;
    private readonly Product_Public_BAL _bal;

    public ProductsController(ILogger<ProductsController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _bal = new Product_Public_BAL(configuration);
    }

    /// <summary>
    /// Products listing. Filter query params from /industries (and deep links) use encrypted ids:
    /// ?industry=...&amp;application=...&amp;category=...
    /// </summary>
    public IActionResult Index(string[]? industry, string[]? application, string[]? category)
    {
        try
        {
            var filter = BuildFilter(
                DecryptIds(industry),
                DecryptIds(application),
                DecryptIds(category),
                1);
            var model = _bal.GetIndex("products", 1, filter);
            return View(model);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Products/Index :", ex);
            return View(new Product_Public_Page());
        }
    }

    public IActionResult Index_html()
    {
        return View();
    }

    public IActionResult Inside_html()
    {
        return View();
    }

    public IActionResult Inside(string? title)
    {
        try
        {
            var pageName = (title ?? string.Empty).Trim().Trim('/');
            if (string.IsNullOrWhiteSpace(pageName))
                return Redirect("/products");

            var model = _bal.GetInside(pageName, 1);
            // Public product page: Published only (status = 2)
            if (model.Product == null || model.Product.ProductId <= 0 || model.Product.Status != 2)
                return Redirect("/products");

            return View(model);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Products/Inside :", ex);
            return Redirect("/products");
        }
    }

    [HttpPost]
    public IActionResult LoadProducts(Product_Public_Filter filter)
    {
        try
        {
            filter ??= new Product_Public_Filter();
            filter.PageNumber = 1;
            if (filter.PageSize < 1) filter.PageSize = DefaultPageSize;
            filter.LanguageId = 1;
            var data = _bal.GetPaged(filter);
            return Json(ToListResult(data, 1, filter.PageSize));
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Products/LoadProducts : Post :", ex);
            return Json(new { success = false, message = "Something went wrong. Please try again." });
        }
    }

    [HttpPost]
    public IActionResult LoadMore(Product_Public_Filter filter)
    {
        try
        {
            filter ??= new Product_Public_Filter();
            if (filter.PageNumber < 1) filter.PageNumber = 1;
            if (filter.PageSize < 1) filter.PageSize = DefaultPageSize;
            filter.LanguageId = 1;
            var data = _bal.GetPaged(filter);
            return Json(ToListResult(data, filter.PageNumber, filter.PageSize));
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Products/LoadMore : Post :", ex);
            return Json(new { success = false, message = "Something went wrong. Please try again." });
        }
    }

    private static Product_Public_Filter BuildFilter(int[]? industry, int[]? application, int[]? category, int page)
    {
        return new Product_Public_Filter
        {
            IndustryIds = JoinIds(industry),
            ApplicationIds = JoinIds(application),
            CategoryIds = JoinIds(category),
            PageNumber = page,
            PageSize = DefaultPageSize,
            LanguageId = 1
        };
    }

    private static int[] DecryptIds(string[]? values)
    {
        if (values == null || values.Length == 0)
            return Array.Empty<int>();

        var list = new List<int>();
        foreach (var raw in values)
        {
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            var value = raw.Trim();
            try
            {
                var plain = CryptoEngine.Decrypt(value);
                if (int.TryParse(plain, out var encryptedId) && encryptedId > 0)
                {
                    if (!list.Contains(encryptedId))
                        list.Add(encryptedId);
                    continue;
                }
            }
            catch
            {
                // Fall through to plain int parsing for backward compatibility.
            }

            if (int.TryParse(value, out var plainId) && plainId > 0 && !list.Contains(plainId))
                list.Add(plainId);
        }

        return list.ToArray();
    }

    private static string? JoinIds(int[]? ids)
    {
        if (ids == null || ids.Length == 0) return null;
        var values = ids.Where(id => id > 0).Distinct().ToArray();
        return values.Length == 0 ? null : string.Join(",", values);
    }

    private static object ToListResult(Product_Public_Page data, int page, int pageSize)
    {
        return new
        {
            success = true,
            items = (data.Products ?? new List<Product_Public_Item>()).Select(p => new
            {
                productId = p.ProductId,
                productName = p.ProductName,
                product_pagename = p.Product_pagename,
                thumbnailUrl = p.ThumbnailUrl,
                thumbnailAlt = p.ThumbnailAlt,
                industries = p.Industries ?? new List<string>(),
                industryNames = p.IndustryNames
            }),
            total = data.TotalRecords,
            page,
            pageSize,
            industryFilters = data.IndustryFilters,
            applicationFilters = data.ApplicationFilters,
            categoryFilters = data.CategoryFilters
        };
    }
}
