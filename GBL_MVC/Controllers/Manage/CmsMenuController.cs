
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.Entity;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
using GBL_MVC.Models.Manage_Model;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.AspNetCore.Mvc.Rendering;
using GBL_MVC.Classes;
using GBL_MVC.Helpers;
namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class CmsMenuController : Controller
    {

        private readonly MenuTreebuilder_bal _bal;
        private readonly MenuHeader_BAL bal;
        private readonly MenuFooter_BAL bal_footer;
        private readonly IConfiguration objconfig;

        public CmsMenuController(IConfiguration config)
        {
            _bal = new MenuTreebuilder_bal(config);
            bal = new MenuHeader_BAL(config);
            bal_footer = new MenuFooter_BAL(config);
            objconfig = config;
        }


        [HttpGet]
        public IActionResult AddEdit()
        {
            ViewBag.ParentMenus = bal.GetParentMenus();

            return View(new MenuHeaderModel
            {
                MenuCategory = 3,
                Target = "_self",
                MenuType = "Normal",
                Sequence = 0,
                FeatureButtonTarget = "_self",
                Status = 2
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEdit(MenuHeaderModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentMenus = bal.GetParentMenus();
                return View(model);
            }

            MenuHeader entity = new MenuHeader
            {
                MenuId = model.MenuId,
                ParentMenuId = model.ParentMenuId,
                MenuCategory = model.MenuCategory,
                MenuName = model.MenuName,
                Url = model.Url,
                Target = model.Target,
                MenuType = model.MenuType,
                Sequence = model.Sequence,

                FeatureImageId = model.FeatureImageId ?? 0,
                ThumbImageId = model.Thumb_image_id ?? 0,

                FeatureTitle = model.FeatureTitle,
                FeatureDescription = model.FeatureDescription,
                FeatureButtonText = model.FeatureButtonText,
                FeatureButtonUrl = model.FeatureButtonUrl,
                FeatureButtonTarget = model.FeatureButtonTarget,

                CssClass = model.CssClass,

                // Use the correct property from your MenuHeader entity
                Status = 2
            };

            if (model.MenuId > 0)
            {
                bal.UpdateMenu_dal(entity);
            }
            else
            {
                bal.SaveMenu(entity);
            }

            return RedirectToAction(nameof(List));
        }






        public IActionResult Edit(string id)
        {
            int menuId = Convert.ToInt32(CryptoEngine.Decrypt(id));

            var menu = bal.GetMenuById(menuId);

            if (menu == null)
                return NotFound();

            var model = new MenuHeaderModel
            {
                MenuId = menu.MenuId,
                ParentMenuId = menu.ParentMenuId,
                MenuCategory = menu.MenuCategory,
                MenuName = menu.MenuName,
                Url = menu.Url,
                Target = menu.Target,
                MenuType = menu.MenuType,
                Sequence = menu.Sequence,

                FeatureImageId = menu.FeatureImageId,
                Thumb_image_id = menu.ThumbImageId,

                FeatureImage = menu.FeatureImage,
                Thumb_image = menu.ThumbImage,

                FeatureTitle = menu.FeatureTitle,
                FeatureDescription = menu.FeatureDescription,
                FeatureButtonText = menu.FeatureButtonText,
                FeatureButtonUrl = menu.FeatureButtonUrl,
                FeatureButtonTarget = menu.FeatureButtonTarget,

                CssClass = menu.CssClass,
                Status = menu.Status
            };

            ViewBag.ParentMenus = bal.GetMenus().Where(x => x.MenuId != menuId).ToList();

            return View("AddEdit", model);
        }

        public IActionResult List(string search = "", int? Status = null, int page = 1)
        {
            int pageSize = 10;
            string validatedSearch = (search ?? string.Empty).Trim();

            var result = bal.GetPagedMenus(validatedSearch, Status, page, pageSize);

            ViewBag.Search = validatedSearch;
            ViewBag.Status = Status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(
                (double)result.Total / pageSize
            );

            return View(result.Data);
        }


        public IActionResult Deactivate(string Id)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));
            bal.Deactivate(realId, Convert.ToInt32(User.GetUserId()));

            TempData["AlertMessage"] = "Menu deactivated";
            return RedirectToAction("List");
        }




        [HttpGet]
        public IActionResult AddFooter()
        {
            ViewBag.ParentFooters = bal_footer.GetParentFooters();

            return View(new MenuFooterModel
            {
                FooterType = "Menu",
                Target = "_self",
                Sequence = 0,
                Status = 2
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFooter(MenuFooterModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ParentFooters = bal_footer.GetParentFooters();
                return View(model);
            }

            MenuFooter entity = new MenuFooter
            {
                FooterId = model.FooterId,
                ParentFooterId = model.ParentFooterId,

                FooterType = model.FooterType,
                Title = model.Title,
                Description = model.Description,
                Url = model.Url,
                Target = model.Target,

                FooterImageId = model.FooterImageId ?? 0,
                FooterThumbImageId = model.FooterThumbImageId ?? 0,

                IconClass = model.IconClass,
                ColumnNo = model.ColumnNo,
                Sequence = model.Sequence,
                Status = model.Status,

                // Set according to your logged-in user
                CreatedBy = model.CreatedBy
            };



            if (model.FooterId > 0)
            {
                bal_footer.UpdateFooter(entity);
            }
            else
            {
                bal_footer.SaveFooter(entity);
            }

            return RedirectToAction(nameof(ListFooter));
        }






        [HttpGet]
        public IActionResult EditFooter(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            int footerId = Convert.ToInt32(CryptoEngine.Decrypt(id));

            var footer = bal_footer.GetFooterById(footerId);

            if (footer == null)
                return NotFound();

            var model = new MenuFooterModel
            {
                FooterId = footer.FooterId,
                ParentFooterId = footer.ParentFooterId,

                FooterType = footer.FooterType,
                Title = footer.Title,
                Description = footer.Description,
                Url = footer.Url,
                Target = footer.Target,

                FooterImageId = footer.FooterImageId,
                FooterThumbImageId = footer.FooterThumbImageId,

                FooterImage = footer.FooterImage,
                FooterThumbImage = footer.FooterThumbImage,

                IconClass = footer.IconClass,
                ColumnNo = footer.ColumnNo,
                Sequence = footer.Sequence,
                Status = footer.Status,

                CreatedBy = footer.CreatedBy,
                CreatedDate = footer.CreatedDate,
                ModifiedBy = footer.ModifiedBy,
                ModifiedDate = footer.ModifiedDate
            };

            // Exclude the current footer from the parent dropdown
            ViewBag.ParentFooters = bal_footer
                .GetParentFooters()
                .Where(x => x.FooterId != footerId)
                .ToList();

            return View("AddFooter", model);
        }

        public IActionResult ListFooter(string search = "", int? status = null, int page = 1)
        {
            int pageSize = 10;
            string validatedSearch = (search ?? string.Empty).Trim();

            var result = bal_footer.GetPagedFooters(
                validatedSearch,
                status,
                page,
                pageSize
            );

            ViewBag.Search = validatedSearch;
            ViewBag.Status = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = result.Total > 0
                ? (int)Math.Ceiling((double)result.Total / pageSize)
                : 1;

            return View(result.Data);
        }

        public IActionResult DeactivateFooter(string Id)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));
            bal_footer.Deactivate(realId, Convert.ToInt32(User.GetUserId()));

            TempData["AlertMessage"] = "Footer deactivated";
            return RedirectToAction("ListFooter");
        }




    }
}