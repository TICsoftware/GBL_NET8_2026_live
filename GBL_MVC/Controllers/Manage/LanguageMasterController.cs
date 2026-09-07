using Microsoft.AspNetCore.Mvc;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using GBL_MVC.Helpers;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers
{
    [Authorize]
[SessionAuthorize]
    public class LanguageMasterController : Controller
    {
        private readonly LanguageMaster_BAL _bal;

        public LanguageMasterController(IConfiguration config)
        {
            _bal = new LanguageMaster_BAL(config);
        }

        public IActionResult Index()
        {
            var list = _bal.GetAll();
            return View(list);
        }

        [HttpGet]
        public IActionResult AddLanguage()
        {
            return View(new LanguageMaster());
        }

        [HttpPost]
        public IActionResult AddLanguage(LanguageMaster model)
        {
            if (ModelState.IsValid)
            {
                model.Created_UserID = Convert.ToInt32(User.GetUserId()); // your logged user
                _bal.Add(model);
                TempData["AlertMessage"] = "Language Added Successfully!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult EditLanguage(string Id)
        {
             int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));

            var data = _bal.GetById(realId);
            ModelState.Remove("ID");
            return View(data);
        }

        [HttpPost]
        public IActionResult EditLanguage(LanguageMaster model)
        {
            if (ModelState.IsValid)
            {
                model.Updated_UserID = Convert.ToInt32(User.GetUserId()); // your logged user
                _bal.Update(model);
                TempData["AlertMessage"] = "Language Updated!";
                return RedirectToAction("Index");
            }
            TempData["AlertMessage"] = "Validation failed.";
            return View(model);
        }
  public IActionResult Deactivate(string Id)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));
            _bal.Deactivate(realId, Convert.ToInt32(User.GetUserId()));
             TempData["AlertMessage"] = "Language Deactivated!";
            return RedirectToAction("Index");
        }

    }
}
