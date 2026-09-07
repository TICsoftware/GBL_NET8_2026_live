
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
    public class contextfieldmasterController : Controller
    {

        private readonly ContextMasterFields_BAL _bal;
        public contextfieldmasterController(IConfiguration config)
        {
            _bal = new ContextMasterFields_BAL(config);
        }

        [HttpGet]
        public IActionResult Assign(string id)  // id = context_master_id
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(id));
            var model = _bal.Load(realId);
            ModelState.Remove("context_master_id");
            return View(model);
        }

        [HttpPost]
        public IActionResult Assign(ContextMasterFieldAssign model)
        {
            _bal.Save(model.context_master_id, model.SelectedFieldIds, Convert.ToInt32(User.GetUserId()));

            TempData["msg"] = "Fields updated!";
            return RedirectToAction("Index", "DynamicTemplateMaster");
        }
    }
}