using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using GBL_MVC.Filters;
using GBL_MVC.Helpers;

namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class Industry_Category_masterController : Controller
    {
        private readonly Industry_Category_Master_BAL _bal;

        public Industry_Category_masterController(IConfiguration config)
        {
            _bal = new Industry_Category_Master_BAL(config);
        }

        public IActionResult Index(string type = "Industry", string search = "", int page = 1)
        {
            Industry_Category_Master_Entity entity = new()
            {
                MasterType = type,
                SearchText = search,
                PageNumber = page,
                PageSize = 10
            };

            if (string.Equals(type, "Tagging", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Industries = _bal.GetActiveIndustries();
                ViewBag.Categories = _bal.GetActiveCategories();
                ViewBag.Mappings = _bal.GetMappings();
                ViewBag.Languages = new SelectList(_bal.GetLanguages(), "ID", "Language_Name");
                return View(entity);
            }

            var result = _bal.GetPaged(entity);
            ViewBag.List = result.Item1;
            entity.TotalRecords = result.Item2;
            ViewBag.Languages = new SelectList(_bal.GetLanguages(), "ID", "Language_Name");
            return View(entity);
        }

        [HttpGet]
        public IActionResult GetById(int id, string type)
        {
            if (id <= 0 || string.IsNullOrWhiteSpace(type))
                return BadRequest(new { message = "Invalid request." });

            var data = _bal.GetById(id, type);
            if (data == null)
                return NotFound(new { message = "Record not found." });

            return Ok(data);
        }

        [HttpGet]
        public IActionResult CheckNameExists(string name, int? languageId, int id = 0, string type = "")
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(type))
                return Ok(new { exists = false });

            return Ok(new { exists = _bal.NameExists(name.Trim(), languageId, id, type) });
        }

        [HttpPost]
        public IActionResult AddAjax([FromBody] Industry_Category_Master_Entity model)
        {
            if (model == null)
                return BadRequest(new { message = "Invalid request." });

            if (string.IsNullOrWhiteSpace(model.MasterType))
                ModelState.AddModelError(nameof(model.MasterType), "Master type is required.");
            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError(nameof(model.Name), "Name is required.");
            if (string.Equals(model.MasterType, "Industry", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(model.PageName))
                ModelState.AddModelError(nameof(model.PageName), "Page name is required.");

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
        public IActionResult UpdateAjax([FromBody] Industry_Category_Master_Entity model)
        {
            if (model == null)
                return BadRequest(new { message = "Invalid request." });

            if (model.ID <= 0)
                ModelState.AddModelError(nameof(model.ID), "Invalid record id.");
            if (string.IsNullOrWhiteSpace(model.MasterType))
                ModelState.AddModelError(nameof(model.MasterType), "Master type is required.");
            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError(nameof(model.Name), "Name is required.");
            if (string.Equals(model.MasterType, "Industry", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(model.PageName))
                ModelState.AddModelError(nameof(model.PageName), "Page name is required.");

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
        public IActionResult ChangeStatus([FromBody] IndustryCategoryStatusModel model)
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
        public IActionResult Delete([FromBody] IndustryCategoryDeleteModel model)
        {
            if (model == null || model.Id == 0 || string.IsNullOrEmpty(model.Type))
                return BadRequest(new { message = "Invalid data" });

            try
            {
                _bal.Delete(model.Id, model.Type);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateSequence([FromBody] List<Industry_Category_Master_Entity> list)
        {
            if (list == null || list.Count == 0)
                return BadRequest(new { message = "Invalid data" });

            var type = list.FirstOrDefault()?.MasterType;
            if (string.IsNullOrWhiteSpace(type))
                return BadRequest(new { message = "Master type is required." });

            _bal.UpdateSequence(list, type);
            return Ok();
        }

        [HttpPost]
        public IActionResult SaveTagging([FromBody] IndustryCategoryTaggingSaveModel model)
        {
            if (model == null)
                return BadRequest(new { message = "Invalid request." });

            try
            {
                _bal.SaveMappings(
                    model.IndustryIds ?? new List<int>(),
                    model.CategoryIds ?? new List<int>(),
                    GetCurrentUserId()
                );
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteMapping([FromBody] IndustryCategoryDeleteModel model)
        {
            if (model == null || model.Id <= 0)
                return BadRequest(new { message = "Invalid mapping id." });

            try
            {
                _bal.DeleteMapping(model.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateMappingSequence([FromBody] List<Industry_Subcategory_Mapping_Entity> list)
        {
            if (list == null || list.Count == 0)
                return BadRequest(new { message = "Invalid data" });

            try
            {
                _bal.UpdateMappingSequence(list);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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
    }
}
