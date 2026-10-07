using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GBL_MVC.Models;
using GBL_MVC.Classes;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;

namespace GBL_MVC.Controllers;

public class CareerController : Controller
{
    private readonly ILogger<CareerController> _logger;
    private readonly Careers_BAL _bal;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public CareerController(
        ILogger<CareerController> logger,
        IConfiguration configuration,
        IWebHostEnvironment env)
    {
        _logger = logger;
        _configuration = configuration;
        _env = env;
        _bal = new Careers_BAL(configuration);
    }

    public IActionResult Index(string title)
    {
        try
        {
            var data = _bal.GetCareers_BAL(title, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Careers :", ex);
            return View(new CareersModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

    public IActionResult LearningDevelopment(string title)
    {
        try
        {
            var data = _bal.GetLearningDevelopment_BAL(title, 1, 1);
            return View(data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/LearningDevelopment :", ex);
            return View(new CareersModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

    public IActionResult WorkWithUs(string title)
    {
        try
        {
            var pageName = string.IsNullOrWhiteSpace(title) ? "work-with-us" : title;
            var data = _bal.GetWorkWithUs_BAL(pageName, 1, 1);
            return View("workwithus", data);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("/Career/WorkWithUs :", ex);
            return View("workwithus", new CareersModel());
        }
        finally
        {
            _bal.Dispose();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitWorkWithUs(WorkWithUsEnquiryModel model)
    {
        using var enquiryBal = new WorkWithUsEnquiry_BAL(_configuration);

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

        string? resumePhysicalPath = null;
        string? resumeDbPath = null;

        try
        {
            if (model.Resume != null && model.Resume.Length > 0)
            {
                var folder = Path.Combine(_env.WebRootPath, "uploads", "workwithus");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var extension = Path.GetExtension(model.Resume.FileName).ToLowerInvariant();
                var fileName = $"{Guid.NewGuid():N}{extension}";
                resumePhysicalPath = Path.Combine(folder, fileName);
                resumeDbPath = $"/uploads/workwithus/{fileName}";

                using (var stream = new FileStream(resumePhysicalPath, FileMode.Create))
                {
                    await model.Resume.CopyToAsync(stream);
                }
            }

            var entity = new WorkWithUsEnquiry
            {
                FullName = model.FullName,
                Email = model.Email,
                Expertise = model.Expertise,
                Address = model.Address,
                Designation = model.Designation,
                ResumePath = resumeDbPath,
                Message = model.Message,
                NotRobot = model.NotRobot,
                IPAddress = GetClientIpAddress()
            };

            var dt = enquiryBal.SubmitEnquiry_BAL(entity, resumePhysicalPath);
            string result = dt.Rows.Count > 0 ? dt.Rows[0][0]?.ToString() ?? string.Empty : string.Empty;

            switch (result.ToLowerInvariant())
            {
                case "updated":
                    return Json(new
                    {
                        status = true,
                        message = "Thank you for your interest. Our team will contact you shortly."
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
            FileLogger.LogError("SubmitWorkWithUs", ex);
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
