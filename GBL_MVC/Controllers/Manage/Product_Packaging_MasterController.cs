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
    public class Product_Packaging_MasterController : Controller
    {
        private readonly Product_Packaging_Master_BAL _bal;

        public Product_Packaging_MasterController(IConfiguration config)
        {
            _bal = new Product_Packaging_Master_BAL(config);
        }

        public IActionResult Index(string search = "", int page = 1)
        {
            Product_Packaging_Master_Entity entity = new()
            {
                SearchText = search,
                PageNumber = page,
                PageSize = 10
            };

            var result = _bal.GetPaged(entity);
            ViewBag.List = result.Item1;
            entity.TotalRecords = result.Item2;
            ViewBag.Languages = new SelectList(_bal.GetLanguages(), "ID", "Language_Name");
            return View(entity);
        }

        [HttpGet]
        public IActionResult GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid request." });

            var data = _bal.GetById(id);
            if (data == null)
                return NotFound(new { message = "Record not found." });

            return Ok(data);
        }

        [HttpPost]
        public IActionResult AddAjax([FromBody] Product_Packaging_Master_Entity model)
        {
            if (model == null)
                return BadRequest(new { message = "Invalid request." });

            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError(nameof(model.Name), "Name is required.");

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
        public IActionResult UpdateAjax([FromBody] Product_Packaging_Master_Entity model)
        {
            if (model == null)
                return BadRequest(new { message = "Invalid request." });

            if (model.product_packaging_MasterId <= 0)
                ModelState.AddModelError(nameof(model.product_packaging_MasterId), "Invalid record id.");
            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError(nameof(model.Name), "Name is required.");

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
        public IActionResult ChangeStatus([FromBody] ProductStatusModel model)
        {
            if (model == null || model.Id == 0)
                return BadRequest(new { message = "Invalid data" });

            try
            {
                _bal.ChangeStatus(model.Id, model.Status, GetCurrentUserId());
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Delete([FromBody] ProductDeleteModel model)
        {
            if (model == null || model.Id == 0)
                return BadRequest(new { message = "Invalid data" });

            try
            {
                _bal.Delete(model.Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult UpdateSequence([FromBody] List<Product_Packaging_Master_Entity> list)
        {
            if (list == null || list.Count == 0)
                return BadRequest(new { message = "Invalid data" });

            try
            {
                _bal.UpdateSequence(list);
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
