
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.Entity;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class CmsMenuAdminController : Controller
    {

        private readonly MenuTreebuilder_bal _bal;
        private readonly CmsMenuTemplate_BAL bal;

        public CmsMenuAdminController(IConfiguration config)
        {
            _bal = new MenuTreebuilder_bal(config);
            bal = new CmsMenuTemplate_BAL(config);
        }


        public IActionResult List()
        {
            var menus = _bal.GetMenus();

            ViewBag.AllMenus = menus;   // ⭐ ADD THIS

            return View(menus);
        }
        //-------------------------------------------------
        // GET — Add / Edit
        //-------------------------------------------------
        public IActionResult AddEdit(int id = 0)
        {
            ViewBag.Templates = _bal.GetTemplateList();
            ViewBag.ParentMenus = _bal.GetMenus();

            if (id == 0)
                return View(new CmsMenu());

            var menu = _bal.GetMenuById(id);

            return View(menu);
        }

        [HttpPost]
        public IActionResult AddEdit(CmsMenu model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Templates = _bal.GetTemplateList();
                ViewBag.ParentMenus = _bal.GetMenus();
                return View(model);
            }

            _bal.SaveMenu(model);

            return RedirectToAction("List");
        }

        public IActionResult GenerateMenu()
        {
            _bal.GenerateMenuHtml();

            TempData["msg"] = "Menu HTML Generated Successfully";

            return RedirectToAction("List");
        }
        public IActionResult TemplateAddEdit(int id = 0)
        {
            if (id == 0)
                return View(new CmsMenuTemplate());

            var template = bal.GetTemplateById(id);

            return View(template);
        }

        [HttpPost]
        public IActionResult TemplateAddEdit(CmsMenuTemplate model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bal.SaveTemplate(model);

            return RedirectToAction("TemplateList");
        }

        public IActionResult TemplateList()
        {
            var templates = bal.GetTemplates();

            return View(templates);
        }

    }
}