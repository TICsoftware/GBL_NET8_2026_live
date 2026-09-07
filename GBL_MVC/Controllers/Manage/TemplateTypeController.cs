using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.BAL;
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
{
  [Authorize]
[SessionAuthorize]
    public class TemplateTypeController : Controller
    {

        private readonly TemplateTypeMaster_bal _bal;
        private readonly MainSpotTemplateMaster_BAL _compBal;

        public TemplateTypeController(IConfiguration configuration)
        {
            _bal = new TemplateTypeMaster_bal(configuration);
            _compBal = new MainSpotTemplateMaster_BAL(configuration);
        }

        public IActionResult Index()
        {
            var templates = _bal.GetAllTemplatesType_bal();
            return View(templates);
        }

        public IActionResult Add(string? id)
        {
            Templatetype_Model model = new Templatetype_Model();

            if (!string.IsNullOrEmpty(id))
            {
                int spotId = Convert.ToInt32(CryptoEngine.Decrypt(id));
                Add_Template_Type entity = _bal.GetTemplateTypeById_bal(spotId);

                if (entity != null)
                {
                    model.ID = entity.ID;
                    model.Template_Type = entity.Template_Type;
                    model.Design_Layout = entity.Design_Layout;
                    model.Status = entity.Status;
                    model.Created_UserID = entity.Created_UserID.ToString();

                }
                else
                {
                    // If no record found, show an empty form
                    TempData["AlertMessage"] = "No record found with the given ID.";
                }
            }
            else
            {
                // Create mode
                model = new Templatetype_Model
                {
                    Template_Master_Save_Action = 1 // for "Add"
                };
            }

            ViewBag.ComponentMasters = _compBal.GetAll();

            ModelState.Remove("ID");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Templatetype_Model model, string id, List<int> SelectedComponentsIds)
        {
            if (ModelState.IsValid)
            {
                var entity = new Add_Template_Type
                {
                    ID = model.ID,
                    Template_Type = model.Template_Type,
                    Design_Layout = model.Design_Layout,
                    Status = 2,
                    Created_UserID = 1
                };

                if (model.ID > 0)
                {
                    _bal.UpdateTemplateType_bal(entity);
                    TempData["AlertMessage"] = "Template updated successfully!";
                }
                else
                {
                    entity.ID = _bal.AddTemplateType_bal(entity);
                    TempData["AlertMessage"] = "Template added successfully!";
                }


                _bal.SaveMappings(entity.ID, SelectedComponentsIds);

                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }
            TempData["AlertMessage"] = "Validation failed. Please check your input.";
            return View(model);
        }




        public IActionResult Edit(string? id)
        {
            Templatetype_Model model = new Templatetype_Model();

            if (!string.IsNullOrEmpty(id))
            {
                int spotId = Convert.ToInt32(CryptoEngine.Decrypt(id));
                Add_Template_Type entity = _bal.GetTemplateTypeById_bal(spotId);

                if (entity != null)
                {
                    model.ID = entity.ID;
                    model.Template_Type = entity.Template_Type;
                    model.Design_Layout = entity.Design_Layout;
                    model.Status = entity.Status;
                    model.Created_UserID = entity.Created_UserID.ToString();

                }
                else
                {
                    // If no record found, show an empty form
                    TempData["AlertMessage"] = "No record found with the given ID.";
                }


                ViewBag.ComponentMasters = _compBal.GetAll();

                ViewBag.SelectedComponents = _bal.GetComponentIds(spotId);

            }
            else
            {
                // Create mode
                model = new Templatetype_Model
                {
                    Template_Master_Save_Action = 1 // for "Add"
                };
            }



            ViewBag.EncId = id;   //  keep encrypted id for post
            ModelState.Remove("ID");
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Add_Template_Type model, List<int> TemplateIds)
        {
            if (ModelState.IsValid)
            {
                var entity = new Add_Template_Type
                {
                    ID = model.ID,
                    Template_Type = model.Template_Type,
                    Design_Layout = model.Design_Layout,
                    Status = 2,
                    Created_UserID = 1
                };

                if (model.ID > 0)
                {
                    _bal.UpdateTemplateType_bal(entity);
                    TempData["AlertMessage"] = "Block Layout updated successfully!";
                }

                _bal.SaveMappings(entity.ID, TemplateIds);

                TempData["msg"] = "Block Layout Updated Successfully!";
                return RedirectToAction("Index");
            }
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }

            TempData["AlertMessage"] = "Validation failed. Please check your input.";
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            _bal.DeleteTemplateType_bal(id);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            _bal.DeleteTemplateType_bal(id);
            return RedirectToAction("Index");
        }



        private Templatetype_Model MapEditTemplate(Add_Template_Type entity)
        {
            return new Templatetype_Model
            {
                ID = entity.ID,
                Template_Type = entity.Template_Type,
                Status = entity.Status,
            };
        }


    }
}