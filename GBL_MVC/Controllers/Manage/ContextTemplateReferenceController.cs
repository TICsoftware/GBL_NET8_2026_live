using Microsoft.AspNetCore.Mvc;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class ContextTemplateReferenceController : Controller
    {
        private readonly ContextTemplateReference_BAL _bal;
        private readonly ContextMaster_BAL _ctxBal;
        private readonly TemplateMaster_BAL _tempBal;

        public ContextTemplateReferenceController(IConfiguration config)
        {
            _bal = new ContextTemplateReference_BAL(config);
            _ctxBal = new ContextMaster_BAL(config);
            _tempBal = new TemplateMaster_BAL(config);
        }



        // ADD NEW MAPPING
        [HttpPost]
        public IActionResult Add(ContextTemplateReference model)
        {
            if (!ModelState.IsValid)
            {
                int? selectedContextId = model.Context_Master_ID > 0
                    ? model.Context_Master_ID
                    : (int?)null;

                TempData["MappingError"] = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault() ?? "Please fill all required fields.";

                return RedirectToAction("Index", new
                {
                    templateId = model.Template_Master_ID,
                    contextId = selectedContextId
                });
            }

            _bal.Add(model);

            return RedirectToAction("Index",
                new { templateId = model.Template_Master_ID, contextId = model.Context_Master_ID });
        }


        [HttpPost]
        public IActionResult AddComponent(ContextTemplateReference model)
        {
            if (model.Context_Master_ID == 0 ||
                model.Template_Master_ID == null ||
                model.Sequence == null)
            {
                return Json(new { success = false, message = "All fields required" });
            }

            _bal.Add(model);

            return Json(new
            {
                success = true,
                id = 1,
                message = "Component added successfully"
            });
        }


        // INDEX
        [HttpPost]
        public IActionResult UpdateLabel(int id, string label)
        {
            _bal.UpdateLabel(id, label);
            return Ok();
        }
        public IActionResult Index(int? templateId, int? contextId, string label, int page = 1)
        {
            const int pageSize = 10;
            var allList = _bal.GetAll(templateId, contextId, label);
            var total = allList.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling((double)total / pageSize));
            page = Math.Min(Math.Max(page, 1), totalPages);

            var list = allList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.ContextMasters = _ctxBal.GetAll();
            ViewBag.TemplateMasters = _tempBal.GetAllTemplates();
            ViewBag.SelectedTemplateId = templateId;
            ViewBag.SelectedContextId = contextId;
            ViewBag.Label = label;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(list);
        }

        [HttpPost]
        public IActionResult UpdateOrder([FromBody] List<ComponentOrderModel> items)
        {
            _bal.UpdateOrder(items);
            return Ok();
        }
        [HttpPost]
        public IActionResult Delete(int id, int? contextMasterId)
        {
            if (id <= 0)
            {
                return Json(new { success = false, message = "Invalid mapping id." });
            }

            if (contextMasterId == null || contextMasterId <= 0)
            {
                return Json(new { success = false, message = "Invalid component layout id." });
            }

            try
            {
                _bal.Delete(id, contextMasterId);
                return Json(new
                {
                    success = true,
                    message = "Mapping deactivated and related details moved to history successfully."
                });
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, message = "Something went wrong while deleting. Please try again." });
            }
        }
    }
}