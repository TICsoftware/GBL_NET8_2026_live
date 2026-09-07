using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    public class MainSpotTemplateDetailsController : Controller
    {
        private readonly Main_Spot_Template_Details_BAL _bal;

        public MainSpotTemplateDetailsController(IConfiguration configuration)
        {
            _bal = new Main_Spot_Template_Details_BAL(configuration);
        }

        private void LoadDropdowns()
        {
            ViewBag.MainSpotReference = _bal.GetAllMainStopReferenceTemplate();
            ViewBag.SpotReference = _bal.GetAllSpotReference();
            ViewBag.Languages = _bal.GetLanguages();
            ViewBag.Geography = _bal.GetGeography();
        }


        public IActionResult Index()
        {
            var templates = _bal.GetAll();
            return View(templates);
        }


        public IActionResult Add(string id)
        {
            LoadDropdowns();

            Main_spot_template_details_Model model = new Main_spot_template_details_Model();

            if (!string.IsNullOrEmpty(id))
            {
                int spotId = Convert.ToInt32(CryptoEngine.Decrypt(id));
                var entity = _bal.GetById(spotId);

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
        public IActionResult Add(Main_spot_template_details_Model model, string id)
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
                    _bal.Update(entity);
                    TempData["AlertMessage"] = "Template updated successfully!";
                }
                else
                {
                    _bal.Add(entity);
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


        private Main_spot_template_details_Model MapEntityToModel(Main_spot_template_details e)
        {
            return new Main_spot_template_details_Model
            {
                ID = e.ID,
                Language_Master_ID = e.Language_Master_ID,
                main_spot_template_reference_id = e.main_spot_template_reference_id,
                spot_reference_id = e.spot_reference_id,

                Title = e.Title,
                Intro = e.Intro,
                Spot_Content = e.Spot_Content,

                // Thumbnail
                Thumbnail_Image_Media_Id = e.Thumbnail_Image_Media_Id,
                Thumbnail_Image = e.Thumbnail_Image,
                Thumbnail_Alt = e.Thumbnail_Alt,

                // Background
                Background_Image_Media_Id = e.Background_Image_Media_Id,
                Background_Image = e.Background_Image,
                Background_Alt = e.Background_Alt,

                // Logo
                Logo_Image_Media_Id = e.Logo_Image_Media_Id,
                Logo_Image = e.Logo_Image,

                // External URL
                Url = e.Url,
                Is_External = e.Is_External,

                // Buttons
                Button_One_Title = e.Button_One_Title,
                Button_Two_Title = e.Button_Two_Title,

                Link_Url_Button_One = e.Link_Url_Button_One,
                Botton_One_Is_External = e.Botton_One_Is_External,

                Link_Url_Button_Two = e.Link_Url_Button_Two,
                Botton_Two_Is_External = e.Botton_Two_Is_External,

                // Video
                Video_Path = e.Video_Path,
                Video_Preview_Image_Media_Id = e.Video_Preview_Image_Media_Id,
                Video_Preview_Image = e.Video_Preview_Image,

                // Upload File
                Upload_File_Media_Id = e.Upload_File_Media_Id,
                Upload_File = e.Upload_File,

                Display_Date = e.Display_Date,
                Sequence = e.Sequence,
                Status = e.Status,

                // Audit
                Created_UserID = e.Created_UserID,
                Created_Date = e.Created_Date,
                Updated_UserID = e.Updated_UserID,
                Updated_Date = e.Updated_Date

            };
        }

        private Main_spot_template_details MapModelToEntity(Main_spot_template_details_Model m)
        {
            return new Main_spot_template_details
            {
                ID = m.ID,
                Language_Master_ID = m.Language_Master_ID,
                main_spot_template_reference_id = m.main_spot_template_reference_id,
                spot_reference_id = m.spot_reference_id,

                Title = m.Title,
                Intro = m.Intro,
                Spot_Content = m.Spot_Content,

                // Thumbnail
                Thumbnail_Image_Media_Id = m.Thumbnail_Image_Media_Id,
                Thumbnail_Alt = m.Thumbnail_Alt,

                // Background
                Background_Image_Media_Id = m.Background_Image_Media_Id,
                Background_Alt = m.Background_Alt,

                // Logo
                Logo_Image_Media_Id = m.Logo_Image_Media_Id,

                // External URL
                Url = m.Url,
                Is_External = m.Is_External,

                // Buttons
                Button_One_Title = m.Button_One_Title,
                Button_Two_Title = m.Button_Two_Title,

                Link_Url_Button_One = m.Link_Url_Button_One,
                Botton_One_Is_External = m.Botton_One_Is_External,

                Link_Url_Button_Two = m.Link_Url_Button_Two,
                Botton_Two_Is_External = m.Botton_Two_Is_External,

                // Video
                Video_Path = m.Video_Path,
                Video_Preview_Image_Media_Id = m.Video_Preview_Image_Media_Id,

                // Upload File
                Upload_File_Media_Id = m.Upload_File_Media_Id,

                Display_Date = m.Display_Date,
                Sequence = m.Sequence,
                Status = m.Status,

                // Audit
                Created_UserID = m.Created_UserID,
                Created_Date = m.Created_Date,
                Updated_UserID = m.Updated_UserID,
                Updated_Date = m.Updated_Date
            };
        }




        public IActionResult Deactivate(string Id)
        {
            int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));
            _bal.Deactivate(realId, 1);
            TempData["msg"] = "Main spot Deactivated!";
            return RedirectToAction("Index");
        }

        public ActionResult ViewLayout(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Content("Invalid layout id");

            if (!int.TryParse(CryptoEngine.Decrypt(id), out int layoutId))
                return Content("Invalid layout id");

            DataSet ds = _bal.GetSpotLayoutById(layoutId);

            if (ds?.Tables.Count < 4)
                return Content("Layout data missing");

            string finalLayout = BuildFinalLayout(ds);

            if (string.IsNullOrWhiteSpace(finalLayout))
                return Content("Layout template missing");

            return Content(finalLayout, "text/html");
        }

        private string? BuildFinalLayout(DataSet ds)
        {
            if (ds == null || ds.Tables.Count < 4)
                return null;

            DataTable mainLayoutTable = ds.Tables[0];
            DataTable mainContentTable = ds.Tables[1];
            DataTable spotLayoutTable = ds.Tables[2];
            DataTable spotContentTable = ds.Tables[3];

            if (mainLayoutTable.Rows.Count == 0)
                return null;

            // ============================
            // MAIN LAYOUT
            // ============================
            string finalLayout = SafeValue(mainLayoutTable.Rows[0]["Design_Layout"]);

            if (string.IsNullOrWhiteSpace(finalLayout))
                return null;

            // ============================
            // MAIN CONTENT
            // ============================
            if (mainContentTable.Rows.Count > 0)
            {
                DataRow main = mainContentTable.Rows[0];

                finalLayout = finalLayout
                    .Replace("#maintemplatetitle#", SafeValue(main["Title"]))
                    .Replace("#maintemplateintro#", SafeValue(main["Intro"]))

                    .Replace("#spot_content#", SafeValue(main["Spot_Content"]))
                    .Replace("#thumbnail_image_media_id#", SafeValue(main["Thumbnail_Image_Media_Id"]))
                    .Replace("#thumbnail_alt#", SafeValue(main["Thumbnail_Alt"]))

                    .Replace("#background_image_media_id#", SafeValue(main["Background_Image_Media_Id"]))
                    .Replace("#background_alt#", SafeValue(main["background_alt"]))

                    .Replace("#url#", SafeValue(main["Url"]))
                    .Replace("#is_external#", GetTarget(main["Is_External"]))

                    .Replace("#button_one_title#", SafeValue(main["Button_One_Title"]))
                    .Replace("#button_two_title#", SafeValue(main["Button_Two_Title"]))

                    .Replace("#link_url_button_one#", SafeValue(main["Link_Url_Button_One"]))
                    .Replace("#button_one_is_external#", GetTarget(main["Botton_One_Is_External"]))

                    .Replace("#link_url_button_two#", SafeValue(main["Link_Url_Button_Two"]))
                    .Replace("#button_two_is_external#", GetTarget(main["Botton_Two_Is_External"]))

                    .Replace("#display_date#", SafeValue(main["Display_Date"]));

                  //.Replace("#logo_image_media_id#", SafeValue(main["Logo_Image_Media_Id"]))
                  //.Replace("#video_path_media_id#", SafeValue(main["Video_Path_Media_Id"]))
                  //.Replace("#video_preview_image_media_id#", SafeValue(main["Video_Preview_Image_Media_Id"]))
                  //.Replace("#upload_file_media_id#", SafeValue(main["Upload_File_Media_Id"]))
            }

            // ============================
            // RHS SPOT LAYOUT
            // ============================
            if (spotLayoutTable.Rows.Count == 0)
                return finalLayout;

            string spotTemplate = SafeValue(spotLayoutTable.Rows[0]["Design_Layout"]);

            if (string.IsNullOrWhiteSpace(spotTemplate) || spotContentTable.Rows.Count == 0)
                return finalLayout;

            int index = 1;

            foreach (DataRow row in spotContentTable.Rows)
            {
                string spotHtml = spotTemplate
                    .Replace("#title#", SafeValue(row["Title"]))
                    .Replace("#intro#", SafeValue(row["Description"]))
                    .Replace("#thumbnail_image#", SafeValue(row["Thumbnail_Img"]))
                    .Replace("#thumbnail_image_alt#", SafeValue(row["Thumbnail_Alt_Text"]))
                    .Replace("#background_Img#", SafeValue(row["background_Img"]))
                    .Replace("#background_Alt_Text#", SafeValue(row["background_Alt_Text"]))
                    .Replace("#icon_Img#", SafeValue(row["icon_Img"]))
                    .Replace("#icon_Alt_Text#", SafeValue(row["icon_Alt_Text"]))
                    .Replace("#Spot_Intro#", SafeValue(row["Spot_Intro"]))
                    .Replace("#Spot_content#", SafeValue(row["Spot_content"]))
                    .Replace("#Files#", SafeValue(row["Files"]))
                    .Replace("#External_Url#", SafeValue(row["External_Url"]));

                finalLayout = finalLayout.Replace($"#rhsspot{index}#", spotHtml);
                index++;
            }

            // Remove unused RHS placeholders
            while (finalLayout.Contains($"#rhsspot{index}#"))
            {
                finalLayout = finalLayout.Replace($"#rhsspot{index}#", string.Empty);
                index++;
            }

            return finalLayout;
        }

        // private string BuildFinalLayout(DataSet ds)
        // {
        //     // ============================
        //     // TABLE REFERENCES
        //     // ============================
        //     DataTable mainLayoutTable = ds.Tables[0];
        //     DataTable mainContentTable = ds.Tables[1];
        //     DataTable spotLayoutTable = ds.Tables[2];
        //     DataTable spotContentTable = ds.Tables[3];

        //     if (mainLayoutTable.Rows.Count == 0 || spotLayoutTable.Rows.Count == 0)
        //         return null;

        //     // ============================
        //     // MAIN LAYOUT
        //     // ============================
        //     string finalLayout = mainLayoutTable.Rows[0]["Design_Layout"]?.ToString();

        //     if (string.IsNullOrWhiteSpace(finalLayout))
        //         return null;

        //     if (mainContentTable.Rows.Count > 0)
        //     {
        //         DataRow main = mainContentTable.Rows[0];

        //         finalLayout = finalLayout
        //             .Replace("#maintemplatetitle#", SafeValue(main["Title"]))
        //             .Replace("#maintemplateintro#", SafeValue(main["Intro"]))

        //             .Replace("#spot_content#", SafeValue(main["Spot_Content"]))
        //             .Replace("#thumbnail_image_media_id#", SafeValue(main["Thumbnail_Image_Media_Id"]))
        //             .Replace("#thumbnail_alt#", SafeValue(main["Thumbnail_Alt"]))

        //             .Replace("#background_image_media_id#", SafeValue(main["Background_Image_Media_Id"]))
        //             .Replace("#background_alt#", SafeValue(main["background_alt"]))

        //             .Replace("#url#", SafeValue(main["Url"]))
        //             .Replace("#is_external#", GetTarget(main["Is_External"]))

        //             .Replace("#button_one_title#", SafeValue(main["Button_One_Title"]))
        //             .Replace("#button_two_title#", SafeValue(main["Button_Two_Title"]))

        //             .Replace("#link_url_button_one#", SafeValue(main["Link_Url_Button_One"]))
        //             .Replace("#button_one_is_external#", GetTarget(main["Botton_One_Is_External"]))

        //             .Replace("#link_url_button_two#", SafeValue(main["Link_Url_Button_Two"]))
        //             .Replace("#button_two_is_external#", GetTarget(main["Botton_Two_Is_External"]))

        //             //.Replace("#logo_image_media_id#", SafeValue(main["Logo_Image_Media_Id"]))
        //             //.Replace("#video_path_media_id#", SafeValue(main["Video_Path_Media_Id"]))
        //             //.Replace("#video_preview_image_media_id#", SafeValue(main["Video_Preview_Image_Media_Id"]))

        //             //.Replace("#upload_file_media_id#", SafeValue(main["Upload_File_Media_Id"]))
        //             .Replace("#display_date#", SafeValue(main["Display_Date"]));
        //     }

        //     // ============================
        //     // RHS SPOT LAYOUT
        //     // ============================
        //     //string spotTemplate = spotLayoutTable.Rows[0]["Design_Layout"]?.ToString();

        //     string spotTemplate = spotLayoutTable.Rows.Count > 0
        //                         ? SafeValue(spotLayoutTable.Rows[0]["Design_Layout"])
        //                         : string.Empty;

        //     if (string.IsNullOrWhiteSpace(spotTemplate))
        //         return finalLayout;

        //     var rhsHtml = new StringBuilder();
        //     int index = 1;

        //     foreach (DataRow row in spotContentTable.Rows)
        //     {
        //         string spotHtml = spotTemplate
        //             .Replace("#title#", row["Title"]?.ToString() ?? string.Empty)
        //             .Replace("#intro#", row["Description"]?.ToString() ?? string.Empty)
        //             .Replace("#thumbnail_image#", row["Thumbnail_Img"]?.ToString() ?? string.Empty)
        //             .Replace("#thumbnail_image_alt#", row["Thumbnail_Alt_Text"]?.ToString() ?? string.Empty)
        //             .Replace("#background_Img#", row["background_Img"]?.ToString() ?? string.Empty)
        //             .Replace("#background_Alt_Text#", row["background_Alt_Text"]?.ToString() ?? string.Empty)
        //             .Replace("#icon_Img#", row["icon_Img"]?.ToString() ?? string.Empty)
        //             .Replace("#icon_Alt_Text#", row["icon_Alt_Text"]?.ToString() ?? string.Empty)
        //             .Replace("#Spot_Intro#", row["Spot_Intro"]?.ToString() ?? string.Empty)
        //             .Replace("#Spot_content#", row["Spot_content"]?.ToString() ?? string.Empty)
        //             .Replace("#Files#", row["Files"]?.ToString() ?? string.Empty)
        //             .Replace("#External_Url#", row["External_Url"]?.ToString() ?? string.Empty);

        //         finalLayout = finalLayout.Replace($"#rhsspot{index}#", spotHtml);
        //         index++;
        //     }

        //     return finalLayout;
        // }

        private static string SafeValue(object? value)
        {
            if (value is null || value == DBNull.Value)
                return string.Empty;

            var text = value.ToString();

            return string.IsNullOrWhiteSpace(text) ? string.Empty : text;
        }

        private static string GetTarget(object? isExternal)
        {
            if (isExternal is null || isExternal == DBNull.Value)
                return string.Empty;

            var text = isExternal.ToString();

            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.ToLowerInvariant();

            return (text == "1" || text == "true" || text == "yes")
                ? "target=\"_blank\" rel=\"noopener noreferrer\""
                : string.Empty;
        }








    }
}