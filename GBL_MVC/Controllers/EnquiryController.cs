using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using GBL_MVC.Models;
using GBL_MVC;

namespace GBL_MVC.Controllers;

public class EnquiryController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly Product_Public_BAL _productBal;

    public EnquiryController(IConfiguration configuration)
    {
        _configuration = configuration;
        _productBal = new Product_Public_BAL(configuration);
    }

    /// <summary>
    /// /enquiry/{title} — title is the product page name (e.g. 1-3-butylene-glycol)
    /// </summary>
    [HttpGet]
    public IActionResult Index(string? title)
    {
        try
        {
            var pageName = (title ?? string.Empty).Trim().Trim('/');
            if (string.IsNullOrWhiteSpace(pageName))
                return Redirect("/products");

            var productPage = _productBal.GetInside(pageName, 1);
            var product = productPage?.Product;
            if (product == null || product.ProductId <= 0)
                return Redirect("/products");

            var model = new ProductEnquiryPageModel
            {
                ProductName = product.ProductName,
                ProductPageName = product.Product_pagename ?? pageName,
                ProductUrl = "/Products/" + (product.Product_pagename ?? pageName)
            };

            return View(model);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Enquiry/Index :", ex);
            return Redirect("/products");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitEnquiry(ProductEnquiryModel model)
    {
        using var enquiryBal = new ProductEnquiry_BAL(_configuration);

        model.Phone = NormalizePhone(model.Phone);
        model.Mobile = NormalizePhone(model.Mobile);
        model.Fax = NormalizePhone(model.Fax);

        ModelState.Clear();
        TryValidateModel(model);

        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    k => k.Key,
                    v => v.Value!.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid value");

            return Json(new
            {
                status = false,
                message = "Please correct the highlighted fields and try again.",
                errors
            });
        }

        try
        {
            // Re-confirm product still exists for the submitted page name
            if (!string.IsNullOrWhiteSpace(model.ProductPageName))
            {
                var productPage = _productBal.GetInside(model.ProductPageName, 1);
                if (productPage?.Product != null && productPage.Product.ProductId > 0)
                {
                    model.ProductName = productPage.Product.ProductName ?? model.ProductName;
                    model.ProductPageName = productPage.Product.Product_pagename ?? model.ProductPageName;
                }
            }

            var entity = new ProductEnquiry
            {
                ProductName = model.ProductName,
                ProductPageName = model.ProductPageName,
                FullName = model.FullName,
                CompanyName = model.CompanyName,
                StreetAddress = model.StreetAddress,
                City = model.City,
                State = model.State,
                Country = model.Country,
                Phone = model.Phone,
                CountryCode = model.CountryCode,
                Mobile = model.Mobile,
                Fax = model.Fax,
                Email = model.Email,
                BusinessType = model.BusinessType,
                EnquiryDetails = model.EnquiryDetails,
                AcceptTerms = model.AcceptTerms,
                NotRobot = model.NotRobot,
                IPAddress = GetClientIpAddress()
            };

            var dt = enquiryBal.SubmitEnquiry_BAL(entity);
            string result = dt.Rows.Count > 0 ? dt.Rows[0][0]?.ToString() ?? string.Empty : string.Empty;

            switch (result.ToLowerInvariant())
            {
                case "updated":
                    return Json(new
                    {
                        status = true,
                        message = "Thank you for your enquiry. Our team will contact you shortly."
                    });

                case "exceeds":
                    return Json(new
                    {
                        status = false,
                        message = "You have already submitted this form 3 times in the last 5 minutes. Please try again later."
                    });

                default:
                    return Json(new
                    {
                        status = false,
                        message = string.IsNullOrWhiteSpace(result)
                            ? "Something went wrong while submitting your enquiry."
                            : result
                    });
            }
        }
        catch (Exception ex)
        {
            FileLogger.LogError("SubmitEnquiry", ex);
            return Json(new
            {
                status = false,
                message = "Something went wrong while submitting your enquiry."
            });
        }
    }

    private static string? NormalizePhone(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var trimmed = value.Trim();
        var hasPlus = trimmed.StartsWith('+');
        var digits = new string(trimmed.Where(char.IsDigit).ToArray());
        if (string.IsNullOrEmpty(digits))
            return trimmed;

        return hasPlus ? "+" + digits : digits;
    }

    private string GetClientIpAddress()
    {
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
            return forwardedFor.Split(',')[0].Trim();

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    }
}
