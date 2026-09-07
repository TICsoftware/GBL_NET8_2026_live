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
{
      [Authorize]
[SessionAuthorize]
    public class SpotRHSController : Controller
    {
        private readonly Spot_bal _bal;

        public SpotRHSController(IConfiguration configuration)
        {
            _bal = new Spot_bal(configuration);
        }


        private void LoadDropdowns()
        {
            ViewBag.SpotReference = _bal.GetAllSpotReference();
            ViewBag.Languages = _bal.GetLanguages();
            ViewBag.Geography = _bal.GetGeography();
        }


        public IActionResult Index()
        {
            var templates = _bal.GetAll_Spot_bal();
            return View(templates);
        }


        public IActionResult Add(string id)
        {
            LoadDropdowns();

            SpotRHS_Model model = new SpotRHS_Model();

            if (!string.IsNullOrEmpty(id))
            {
                int spotId = Convert.ToInt32(CryptoEngine.Decrypt(id));
                var entity = _bal.GetSpot_ById_bal(spotId);

                if (entity == null)
                {
                    TempData["AlertMessage"] = "No record found with the given ID.";
                    return View(model);
                }

                model = MapEntityToModel(entity);
                model.Spot_RHS_Save_Action = 2; // Edit
            }
            else
            {
                model.Spot_RHS_Save_Action = 1; // Add
            }

            ModelState.Remove("ID");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(SpotRHS_Model model, string id)
        {
            if (!ModelState.IsValid)
            {
                TempData["AlertMessage"] = "Validation failed. Please check your input.";
                LoadDropdowns();
                return View(model);
            }

            try
            {
                var entity = MapModelToEntity(model);

                if (model.ID > 0)
                {
                    _bal.UpdateSpotTemplateMaster_bal(entity);
                    TempData["AlertMessage"] = "Template updated successfully!";
                }
                else
                {
                    _bal.Add_Spot_bal(entity);
                    TempData["AlertMessage"] = "Template added successfully!";
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


        private SpotRHS_Model MapEntityToModel(Add_Spot_RHS e)
        {
            return new SpotRHS_Model
            {
                ID = e.ID,
                Language_Master_ID = e.Language_Master_ID,
                Geography_ID = e.Geography_ID,
                spot_template_master_id = e.spot_template_master_id,

                Title = e.Title,
                Description = e.Description,

                Thumbnail_Img_Media_Id = e.Thumbnail_Img_Media_Id,
                Thumbnail_Img = e.Thumbnail_Img,
                Thumbnail_Alt_Text = e.thumbnail_Alt_Text,

                Background_Img_Media_Id = e.background_Img_Media_Id,
                Background_Img = e.background_Img,
                Background_Alt_Text = e.background_Alt_Text,

                Icon_Img_Media_Id = e.icon_Img_Media_Id,
                Icon_Img = e.icon_Img,
                Icon_Alt_Text = e.icon_Alt_Text,

                Spot_Intro = e.Spot_Intro,
                Spot_Content = e.Spot_content,

                Files_Media_Id = e.Files_Media_Id,
                Files = e.Files,

                External_Url = e.External_Url,
                IsExternal = e.Isexternal ?? false,

                Status = e.Status,
                Created_UserID = e.Created_UserID,
                Updated_UserID = e.Updated_UserID ?? 0,
                Sequence = e.sequence
            };
        }

        private Add_Spot_RHS MapModelToEntity(SpotRHS_Model m)
        {
            return new Add_Spot_RHS
            {
                ID = m.ID,

                Language_Master_ID = m.Language_Master_ID,
                Geography_ID = m.Geography_ID,
                spot_template_master_id = m.spot_template_master_id,

                Title = m.Title,
                Description = m.Description,

                Thumbnail_Img_Media_Id = m.Thumbnail_Img_Media_Id,
                thumbnail_Alt_Text = m.Thumbnail_Alt_Text,

                background_Img_Media_Id = m.Background_Img_Media_Id,
                background_Alt_Text = m.Background_Alt_Text,

                icon_Img_Media_Id = m.Icon_Img_Media_Id,
                icon_Alt_Text = m.Icon_Alt_Text,

                Spot_Intro = m.Spot_Intro,
                Spot_content = m.Spot_Content,

                Files_Media_Id = m.Files_Media_Id,

                External_Url = m.External_Url,
                Isexternal = m.IsExternal,

                Status = 2,

                Created_UserID = 1,
                Updated_UserID = 1,
                Published_UserID = 1,
                DeActivated_UserID = 1,

                sequence = m.Sequence
            };
        }


        // public IActionResult Add(string id)
        // {
        //     SpotRHS_Model model = new SpotRHS_Model();

        //     if (!string.IsNullOrEmpty(id))
        //     {
        //         int spot_id = Convert.ToInt32(CryptoEngine.Decrypt(id));
        //         Add_Spot_RHS entity = _bal.GetSpot_ById_bal(spot_id);

        //         if (entity != null)
        //         {
        //             model.ID = entity.ID;

        //             model.Language_Master_ID = entity.Language_Master_ID;
        //             model.Geography_ID = entity.Geography_ID;
        //             model.spot_template_master_id = entity.spot_template_master_id;

        //             model.Title = entity.Title;
        //             model.Description = entity.Description;

        //             // Images & Alt text
        //             model.Thumbnail_Img_Media_Id = entity.Thumbnail_Img_Media_Id;
        //             model.Thumbnail_Img = entity.Thumbnail_Img;
        //             model.Thumbnail_Alt_Text = entity.thumbnail_Alt_Text;

        //             model.Background_Img_Media_Id = entity.background_Img_Media_Id;
        //             model.Background_Img = entity.background_Img;
        //             model.Background_Alt_Text = entity.background_Alt_Text;

        //             model.Icon_Img_Media_Id = entity.icon_Img_Media_Id;
        //             model.Icon_Img = entity.icon_Img;
        //             model.Icon_Alt_Text = entity.icon_Alt_Text;

        //             // Spot Content
        //             model.Spot_Intro = entity.Spot_Intro;
        //             model.Spot_Content = entity.Spot_content;


        //             model.Files_Media_Id = entity.Files_Media_Id;
        //             model.Files = entity.Files;

        //             // External info
        //             model.External_Url = entity.External_Url;
        //             model.IsExternal = entity.Isexternal ?? false;

        //             // Status
        //             model.Status = entity.Status;

        //             // Created / Updated
        //             model.Created_UserID = entity.Created_UserID;
        //             model.Updated_UserID = entity.Updated_UserID ?? 0;   // safe fallback

        //             // Sequence
        //             model.Sequence = entity.sequence;
        //         }
        //         else
        //         {
        //             TempData["AlertMessage"] = "No record found with the given ID.";
        //         }

        //     }
        //     else
        //     {
        //         model = new SpotRHS_Model
        //         {
        //             Spot_RHS_Save_Action = 1 // for "Add"
        //         };
        //     }

        //     return View(model);
        // }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult Add(SpotRHS_Model model)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var entity = new Add_Spot_RHS
        //         {

        //             ID = model.ID,

        //             Language_Master_ID = model.Language_Master_ID,
        //             Geography_ID = model.Geography_ID,
        //             spot_template_master_id = model.spot_template_master_id,

        //             Title = model.Title,
        //             Description = model.Description,

        //             Thumbnail_Img_Media_Id = model.Thumbnail_Img_Media_Id,
        //             thumbnail_Alt_Text = model.Thumbnail_Alt_Text,

        //             background_Img_Media_Id = model.Background_Img_Media_Id,
        //             background_Alt_Text = model.Background_Alt_Text,

        //             icon_Img_Media_Id = model.Icon_Img_Media_Id,
        //             icon_Alt_Text = model.Icon_Alt_Text,

        //             Spot_Intro = model.Spot_Intro,
        //             Spot_content = model.Spot_Content,

        //             Files_Media_Id = model.Files_Media_Id,

        //             External_Url = model.External_Url,
        //             Isexternal = model.IsExternal,

        //             Status = 2,

        //             Created_UserID = 1,
        //             Updated_UserID = 1,
        //             Published_UserID = 1,
        //             DeActivated_UserID = 1,

        //             sequence = model.Sequence
        //         };

        //         if (model.ID > 0)
        //         {
        //             _bal.UpdateSpotTemplateMaster_bal(entity);
        //             TempData["AlertMessage"] = "Template updated successfully!";
        //         }
        //         else
        //         {
        //             _bal.Add_Spot_bal(entity);
        //             TempData["AlertMessage"] = "Template added successfully!";
        //         }

        //         return RedirectToAction("Index");
        //     }
        //     if (!ModelState.IsValid)
        //     {
        //         var errors = ModelState.Values.SelectMany(v => v.Errors);
        //     }
        //     TempData["AlertMessage"] = "Validation failed. Please check your input.";
        //     return View(model);
        // }




        public IActionResult Edit(int id)
        {
            var template = _bal.GetSpot_ById_bal(id);
            return View(template);
        }



        public IActionResult Delete(int id)
        {
            _bal.DeleteSpot_bal(id);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            _bal.DeleteSpot_bal(id);
            return RedirectToAction("Index");
        }


    }
}