using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using GBL_MVC.Filters;
using GBL_MVC.Helpers;

namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class Catalog_masterController : Controller
    {
        private readonly Catalog_Master_BAL _bal;

        public Catalog_masterController(IConfiguration config)
        {
            _bal = new Catalog_Master_BAL(config);
        }

        public IActionResult Index(string type = "Industry", string search = "", int page = 1)
        {
            CatalogMasterEntity entity = new()
            {
                MasterType = type,
                SearchText = search,
                PageNumber = page,
                PageSize = 10
            };

            var result = _bal.GetPaged(entity);
            ViewBag.List = result.Item1;
            entity.TotalRecords = result.Item2;

            if (IsMapping(type))
            {
                ViewBag.Industries = _bal.GetLookup("Industry");
                ViewBag.Subcategories = _bal.GetLookup("Subcategory");
            }

            return View(entity);
        }

        [HttpPost]
        public IActionResult AddAjax([FromBody] CatalogMasterEntity model)
        {
            if (model == null)
                return BadRequest(new { message = "Invalid request." });

            if (string.IsNullOrWhiteSpace(model.MasterType))
                ModelState.AddModelError(nameof(model.MasterType), "Master type is required.");

            ClearValidationForType(model);

            if (!ModelState.IsValid)
                return BadRequest(new { message = GetValidationMessage() });

            try
            {
                model.Create_UserId = GetCurrentUserId();
                model.Status ??= 1;
                _bal.Save(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateAjax([FromBody] CatalogMasterEntity model)
        {
            if (model == null)
                return BadRequest(new { message = "Invalid request." });

            if (model.ID <= 0)
                ModelState.AddModelError(nameof(model.ID), "Invalid record id.");
            if (string.IsNullOrWhiteSpace(model.MasterType))
                ModelState.AddModelError(nameof(model.MasterType), "Master type is required.");

            ClearValidationForType(model);

            if (!ModelState.IsValid)
                return BadRequest(new { message = GetValidationMessage() });

            try
            {
                model.Update_UserId = GetCurrentUserId();
                _bal.Save(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ChangeStatus([FromBody] CatalogStatusModel model)
        {
            if (model == null || model.Id == 0 || string.IsNullOrEmpty(model.Type))
                return BadRequest(new { message = "Invalid data" });

            try
            {
                _bal.ChangeStatus(model.Id, model.Type, model.Status, GetCurrentUserId());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Deactivate([FromBody] CatalogDeleteModel model)
        {
            if (model == null || model.Id == 0 || string.IsNullOrEmpty(model.Type))
                return BadRequest(new { message = "Invalid data" });

            try
            {
                _bal.Deactivate(model.Id, model.Type, GetCurrentUserId());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateSequence([FromBody] List<CatalogMasterEntity> list)
        {
            if (list == null || list.Count == 0)
                return BadRequest(new { message = "Invalid data" });

            var type = list.FirstOrDefault()?.MasterType;
            if (string.IsNullOrWhiteSpace(type))
                return BadRequest(new { message = "Master type is required." });

            _bal.UpdateSequence(list, type);
            return Ok();
        }

        private void ClearValidationForType(CatalogMasterEntity model)
        {
            if (IsMapping(model.MasterType))
            {
                ModelState.Remove(nameof(model.Name));
                if (!model.IndustryId.HasValue || model.IndustryId <= 0)
                    ModelState.AddModelError(nameof(model.IndustryId), "Industry is required.");
                if (!model.Category_Master_Id.HasValue || model.Category_Master_Id <= 0)
                    ModelState.AddModelError(nameof(model.Category_Master_Id), "Subcategory is required.");
            }
            else if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Name is required.");
            }
        }

        private string GetValidationMessage() =>
            string.Join(" ", ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).Where(m => !string.IsNullOrWhiteSpace(m)));

        private int? GetCurrentUserId()
        {
            var raw = User.GetUserId();
            return int.TryParse(raw, out var id) ? id : null;
        }

        private static bool IsMapping(string? type) =>
            string.Equals(type, "Mapping", StringComparison.OrdinalIgnoreCase);
    }
}
