using Microsoft.AspNetCore.Mvc;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
using GBL_MVC.Helpers;
namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class ContextReferenceController : Controller
    {
        private readonly ContextReference_BAL _bal;
        private readonly LanguageMaster_BAL _langBal;
        private readonly ContextMaster_BAL _ctxBal;
        private readonly TemplateMaster_BAL _tempMasBal;
        private readonly IConfiguration _config;

        public ContextReferenceController(IConfiguration config)
        {
            _config = config;
            _bal = new ContextReference_BAL(config);
            _langBal = new LanguageMaster_BAL(config);    // assumes exist
            _ctxBal = new ContextMaster_BAL(config);      // assumes exist
            _tempMasBal = new TemplateMaster_BAL(config);
        }

        public IActionResult Index(string search = "", int page = 1, int pageSize = 10)
        {
            var data = _bal.GetAll();

            foreach (var r in data)
            {
                r.HasData = _bal.HasData(r.ID); // Add flag for UI
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => (x.Reference_Title ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            int total = data.Count;
            var items = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Search = search;

            return View(items);
        }



        public IActionResult _contextreferencetable(string search = "", int page = 1, int pageSize = 10)
        {
            var data = _bal.GetAll();

            foreach (var r in data)
            {
                r.HasData = _bal.HasData(r.ID); // Add flag for UI
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => (x.Reference_Title ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            int total = data.Count;
            var items = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Search = search;

            return View(items);
        }

        // Add GET (returns Add page)
        [HttpGet]
        public IActionResult Add()
        {

            ViewBag.Languages = _langBal.GetLanguages();
            ViewBag.TemplateMasters = _tempMasBal.GetAllTemplates();
            ViewBag.ContextMasters = _ctxBal.GetAll();
            return View(new ContextReference());
        }

        // Add POST - AJAX
        [HttpPost]
        public IActionResult Add(ContextReference model)
        {

            if (!ModelState.IsValid)
            {

                return Json(new { success = false, message = "Invalid data" });
            }
            model.Created_UserID = Convert.ToInt32(User.GetUserId());
            int newId = _bal.Add(model);

            return Json(new { success = true, message = "Added successfully", id = newId });
        }

        // Edit GET using encrypted id
        [HttpGet]
        public IActionResult Edit(string encId)
        {
            if (string.IsNullOrEmpty(encId)) return BadRequest();

            int id = Convert.ToInt32(Core_project_BusinessLogic.CryptoEngine.Decrypt(encId));
            var model = _bal.GetById(id);
            if (model == null) return NotFound();

            ViewBag.Languages = _langBal.GetLanguages();
            ViewBag.TemplateMasters = _tempMasBal.GetAllTemplates();
            ViewBag.ContextMasters = _ctxBal.GetAll();

            ViewBag.EncId = encId; // keep for POST
            return View(model);
        }

        // Edit POST - AJAX expects encId param in querystring or form
        [HttpPost]
        public IActionResult Edit(ContextReference model, string encId)
        {
            try
            {

                if (!string.IsNullOrEmpty(encId))
                    model.ID = Convert.ToInt32(Core_project_BusinessLogic.CryptoEngine.Decrypt(encId));

                if (!ModelState.IsValid)
                {

                    return Json(new { success = false, message = "Invalid data" });
                }
                model.Updated_UserID = Convert.ToInt32(User.GetUserId());
                _bal.Update(model);
                return Json(new { success = true, message = "Updated successfully" });


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // Deactivate (soft)
        public IActionResult Deactivate(string encId)
        {
            int id = Convert.ToInt32(Core_project_BusinessLogic.CryptoEngine.Decrypt(encId));
            _bal.Deactivate(id, Convert.ToInt32(User.GetUserId()));
            TempData["AlertMessage"] = "Deactivated successfully";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Render(string encId)
        {
            int referenceId = Convert.ToInt32(Core_project_BusinessLogic.CryptoEngine.Decrypt(encId));

            string finalHtml = _bal.BuildFinalLayout(referenceId);

            //return Content(finalHtml, "text/html");
            return Json(new { finalHtml });
        }


        [HttpGet]
        public IActionResult GetHtmlLayout(string encId)
        {
            int referenceId = Convert.ToInt32(Core_project_BusinessLogic.CryptoEngine.Decrypt(encId));

            string htmllayout = _bal.GetHtmlLayout(referenceId);
            //htmllayout=htmllayout.ToString().Replace("#title#","test").Replace("#intro#","intro test");
            return Json(new { htmllayout });
        }

    }
}
