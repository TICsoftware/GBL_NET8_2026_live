using Microsoft.AspNetCore.Mvc;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using Core_project_BusinessLogic;
using Microsoft.Extensions.Configuration;
using GBL_MVC.Helpers;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers
{
    [Authorize]
[SessionAuthorize]
    public class GeographyMasterController : Controller
    {
        private readonly GeographyMaster_BAL _bal;

        public GeographyMasterController(IConfiguration config)
        {
            _bal = new GeographyMaster_BAL(config);
        }

        public IActionResult Index()
        {
            var list = _bal.GetAll();
            return View(list);
        }
        [HttpGet]
        public IActionResult AddGeography()
        {
             ViewBag.Languages = _bal.GetLanguages();
            return View(new GeographyMaster());
        }

    

[HttpPost]
public IActionResult AddGeography(GeographyMaster model)
{
    if (ModelState.IsValid)
    {
        model.Created_UserID = Convert.ToInt32(User.GetUserId()); // your logged user
        int result = _bal.Add(model);

        // DUPLICATE CHECK
       if (result == -1)
{
    ModelState.AddModelError("Country_Name", "Geography already exists!");
    ViewBag.Languages = _bal.GetLanguages();
    return View(model);
}

        // ✅ SUCCESS
        TempData["AlertMessage"] = "Geography Added Successfully!";
        return RedirectToAction("Index");
    }

    ViewBag.Languages = _bal.GetLanguages();
    return View(model);
}


        public IActionResult EditGeography(string Id)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));
            System.Diagnostics.Debug.WriteLine("REAL ID = " + realId);

            var model = _bal.GetById(realId);

            if (model == null)
            {
                System.Diagnostics.Debug.WriteLine("MODEL IS NULL");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("MODEL.ID = " + model.ID);
            }
              ViewBag.Languages = _bal.GetLanguages();
            ModelState.Remove("ID");
            return View(model);
        }

        [HttpPost]
        public IActionResult EditGeography(GeographyMaster model)
        {
            if (ModelState.IsValid)
            {
                
                model.Updated_UserID = Convert.ToInt32(User.GetUserId()); // logged user
                _bal.Update(model);
                TempData["AlertMessage"] = "Geography updated!";
                return RedirectToAction("Index");
            }
            TempData["AlertMessage"] = "Validation failed.";
             ViewBag.Languages = _bal.GetLanguages();
            return View(model);
        }

        public IActionResult Deactivate(string Id)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));
            _bal.Deactivate(realId, Convert.ToInt32(User.GetUserId()));
            return RedirectToAction("Index");
        }
    }
}
