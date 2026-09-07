using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BLL;
using Core_project_BusinessLogic.Entity.Manage;
using GBL_MVC.Models.Manage_Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers.Manage
{  [Authorize]
[SessionAuthorize]
    public class SpotTemplateMasterController : Controller
    {
        private readonly SpotTemplateMaster_bal _bal;

        public SpotTemplateMasterController(IConfiguration configuration)
        {
            _bal = new SpotTemplateMaster_bal(configuration);
        }


        private void LoadDropdowns()
        {
            ViewBag.TemplateTypes = _bal.GetTemplateTypes();
            ViewBag.Languages = _bal.GetLanguages();
        }
        public IActionResult Index()
        {
            var templates = _bal.GetAll_SpotTemplates_Master_bal();
            return View(templates);
        }

        public IActionResult Add(string? id)
        {
            LoadDropdowns();

            SpotTemplate_Model model = new SpotTemplate_Model();

            if (!string.IsNullOrEmpty(id))
            {
                int spotId = Convert.ToInt32(CryptoEngine.Decrypt(id));
                var entity = _bal.Get_SpotTemplate_MasterById_bal(spotId);

                if (entity == null)
                {
                    TempData["AlertMessage"] = "No record found with the given ID.";
                    return RedirectToAction("Index");
                }

                model.ID = entity.ID;
                model.Language_Master_ID = entity.Language_Master_ID;
                model.Template_Type_ID = entity.Template_Type_ID;
                model.Name = entity.Name;
                model.Status = entity.Status;
                model.Created_UserID = entity.Created_UserID;
                model.Spot_Template_Master_Save_Action = 2; // Edit
            }
            else
            {
                model.Spot_Template_Master_Save_Action = 1; // Add
            }

            ModelState.Remove("ID");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(SpotTemplate_Model model, string id)
        {
            if (!ModelState.IsValid)
            {
                TempData["AlertMessage"] = "Validation failed. Please check your input.";
                LoadDropdowns();
                return View(model);
            }
 
            try
            {
                var entity = new Add_Spot_Template_Master
                {
                    ID = model.ID,
                    Template_Type_ID = model.Template_Type_ID,
                    Language_Master_ID = model.Language_Master_ID,
                    Name = model.Name,
                    Status = model.Status > 0 ? model.Status : 2,
                    Created_UserID = model.Created_UserID > 0 ? model.Created_UserID : 1
                };

                if (model.ID > 0)
                {
                    _bal.UpdateSpotTemplateMaster_bal(entity);
                    TempData["AlertMessage"] = "Block Layout updated successfully!";
                }
                else
                {
                    _bal.AddSpotTemplateMaster_bal(entity);
                    TempData["AlertMessage"] = "Block Layout added successfully!";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["AlertMessage"] = "An unexpected error occurred. " + ex.Message;

                LoadDropdowns();
                return View(model);
            }
        }



        public IActionResult Delete(int id)
        {
            _bal.DeleteSpotTemplateMaster_bal(id);
            TempData["AlertMessage"] = "Template deleted successfully!";
            return RedirectToAction("Index");
        }




    }
}