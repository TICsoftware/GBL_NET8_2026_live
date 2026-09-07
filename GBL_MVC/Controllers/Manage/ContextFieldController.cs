using Microsoft.AspNetCore.Mvc;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
using GBL_MVC.Helpers;
namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class ContextFieldController : Controller
    {
        private readonly ContextField_BAL bal;
        private readonly ContextMasterFields_BAL _bal;
        public ContextFieldController(IConfiguration config)
        {
            bal = new ContextField_BAL(config);
        }

        public IActionResult Index(int page = 1)
        {
            const int pageSize = 10;
            var all = bal.GetAll();
            var total = all.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling((double)total / pageSize));
            page = Math.Min(Math.Max(page, 1), totalPages);

            var list = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View(list);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(ContextField model)
        {
            if (ModelState.IsValid)
            {
                model.Created_UserID = Convert.ToInt32(User.GetUserId()); // Example
                bal.Insert(model);
                TempData["msg"] = "Field created successfully";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Edit(string id)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(id));
            var model = bal.GetById(realId);
            ModelState.Remove("id");
            ModelState.Remove("is_block");   // IMPORTANT
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ContextField model)
        {
            if (ModelState.IsValid)
            {
                model.Updated_UserID = Convert.ToInt32(User.GetUserId());
                bal.Update(model);
                TempData["msg"] = "Field updated";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Deactivate(string id)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(id));
            bal.Deactivate(realId, Convert.ToInt32(User.GetUserId()));
            TempData["AlertMessage"] = "Context field deactivated";
            return RedirectToAction("Index");
        }

    }
}
