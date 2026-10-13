using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Models;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;
using System.Data;

namespace GBL_MVC.Controllers;

public class ContactController : Controller
{
    private readonly ILogger<ContactController> _logger;
    private readonly Contact_BAL _bal;
    private readonly IConfiguration _configuration;

    public ContactController(ILogger<ContactController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        _bal = new Contact_BAL(configuration);
    }



    public IActionResult ContactUs(string title)
    {
        try
        {
            var data = _bal.GetContact_BAL(title, 1, 1);
            var states = _bal.GetStateMaster_Bal();
            ViewBag.States = states;
            ViewBag.captchapublickey = _configuration["CaptchaKeys:PublicKey"];

            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/ContactUs :", ex);
            return View(new CareersModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SubmitQuery(ContactUsModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    k => k.Key,
                    v => v.Value!.Errors.FirstOrDefault()?.ErrorMessage
                         ?? "Invalid value");

            return Json(new
            {
                status = false,
                message = "Please correct the highlighted fields and try again.",
                errors
            });
        }

        try
        {
            var entity = new ContactEntity
            {
                Name = model.Name,
                CompanyName = model.CompanyName,
                PhoneNumber = model.PhoneNumber,
                EmailAddress = model.EmailAddress,
                StateId = model.StateId,
                SiteLocation = model.SiteLocation,
                Query = model.Query,
                IPAddress = GetClientIpAddress(),
                Consent = model.Consent
            };

            DataTable dt = _bal.ContactUs_Insert_BAL(entity);

            string result = dt.Rows.Count > 0 ? dt.Rows[0][0]?.ToString() ?? string.Empty : string.Empty;

            if (int.TryParse(result, out int id) && id > 0)
            {
                return Json(new
                {
                    status = true,
                    message = result
                });
            }

            FileLogger.LogError("SubmitQuery", new Exception("Contact enquiry insertion failed: " + result));

            return Json(new
            {
                status = false,
                message = "Something went wrong while submitting your enquiry."
            });
        }
        catch (Exception ex)
        {
            FileLogger.LogError("SubmitQuery", ex);

            return Json(new
            {
                status = false,
                message = "Something went wrong while submitting your enquiry."
            });
        }
    }


    private string GetClientIpAddress()
    {
        var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
            return forwardedFor.Split(',')[0].Trim();

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
