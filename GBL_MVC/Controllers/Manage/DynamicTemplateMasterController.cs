using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.Entity;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
using GBL_MVC.Helpers;
namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
[SessionAuthorize]
    public class DynamicTemplateMasterController : Controller
    {
        private readonly ContextMaster_BAL _bal;
        private readonly ContextTemplateReference_BAL _mapBal;
        private readonly TemplateMaster_BAL _tempBal;
        public DynamicTemplateMasterController(IConfiguration config)
        {
            _bal = new ContextMaster_BAL(config);
            _mapBal = new ContextTemplateReference_BAL(config);
            _tempBal = new TemplateMaster_BAL(config);
        }
        public IActionResult Index(string search = "", int page = 1)
        {
            int pageSize = 10;
            string validatedSearch = (search ?? string.Empty).Trim();

            var result = _bal.GetPaged(validatedSearch, page, pageSize);

            ViewBag.Search = validatedSearch;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.Total / pageSize);

            return View(result.Data);
        }

        public IActionResult Add()
        {
            ViewBag.Languages = _bal.GetLanguages();
            ViewBag.TemplateMasters = _tempBal.GetAllTemplates();
            return View(new ContextMaster { Status = 1 });
        }


        [HttpPost]       
        [ValidateAntiForgeryToken] 
        public IActionResult Add(ContextMaster model, List<int> TemplateIds)      
        {
            if (ModelState.IsValid)
            {
                model.Created_UserID = Convert.ToInt32(User.GetUserId());
                int contextId = _bal.Add(model);

                // Template mapping is optional; save only when selected.
                _mapBal.SaveMappings(contextId, TemplateIds ?? new List<int>());

                TempData["AlertMessage"] = "Context added successfully";
                return RedirectToAction("Index");
            }

            ViewBag.Languages = _bal.GetLanguages();
            ViewBag.TemplateMasters = _tempBal.GetAllTemplates();
            return View(model);
        }

        [HttpGet]

        public IActionResult Edit(string Id)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));

            var model = _bal.GetById(realId);
            if (model == null) return NotFound();

            ViewBag.Languages = _bal.GetLanguages();
            ViewBag.TemplateMasters = _tempBal.GetAllTemplates();

            // load mappings
            ViewBag.SelectedTemplates = _mapBal.GetTemplates(realId);
            ViewBag.EncId = Id;   //  keep encrypted id for post
            ModelState.Remove("ID");
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ContextMaster model, List<int> TemplateIds, string EncId)
        {
            if (!string.IsNullOrEmpty(EncId))
            {
                model.ID = Convert.ToInt32(CryptoEngine.Decrypt(EncId));
            }

            if (ModelState.IsValid)
            {
                model.Updated_UserID = Convert.ToInt32(User.GetUserId());
                _bal.Update(model);

                // mapping save
              //  _mapBal.SaveMappings(model.ID, TemplateIds);

                TempData["AlertMessage"] = "Context updated successfully";
                return RedirectToAction("Index");
            }

            // reload dropdowns on validation fail
            ViewBag.Languages = _bal.GetLanguages();
            ViewBag.TemplateMasters = _tempBal.GetAllTemplates();
            ViewBag.SelectedTemplates = TemplateIds;

            return View(model);
        }



        public IActionResult Deactivate(string Id)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));
            _bal.Deactivate(realId, Convert.ToInt32(User.GetUserId()));

            TempData["AlertMessage"] = "Context deactivated";
            return RedirectToAction("Index");
        }



    }
}
