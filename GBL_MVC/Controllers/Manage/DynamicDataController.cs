using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.Entity;
using System.Net;
using GBL_MVC.Helpers;
using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class DynamicDataController : Controller
    {
        private readonly ContextDetail_BAL _detailBal;
        private readonly ContextTemplateReference_BAL _ctrBal;
        private readonly ContextMaster_BAL _contextBal;

        public DynamicDataController(IConfiguration config)
        {
            _detailBal = new ContextDetail_BAL(config);
            _ctrBal = new ContextTemplateReference_BAL(config);
            _contextBal = new ContextMaster_BAL(config);
        }




        [HttpGet]
        public IActionResult AddMainComponent_Temp(string encId, string returnUrl)
        {
            int UserId = Convert.ToInt32(User.GetUserId());

            if (string.IsNullOrEmpty(encId))
                return BadRequest();
            string[] split_val = WebUtility.UrlDecode(encId).ToString().Split(new string[] { "|$|" }, StringSplitOptions.None);
            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(split_val[1]));

            // 🔹 LOAD FIELDS FROM CONTEXT TEMPLATE REFERENCE  
            var fields = _detailBal.LoadFieldsByTemplateReference(ctrId, 0);
            ViewBag.Mode = "Main";
            // Only main fields
            fields = fields.Where(x => x.is_block == 0).ToList();
            var reference = _ctrBal.GetById(ctrId);
            ViewBag.temp_cont_id = split_val[0];
            ViewBag.Fields = fields;
            ViewBag.Reference = reference;
            ViewBag.ReturnUrl = WebUtility.UrlDecode(returnUrl);
            ViewBag.EncId = split_val[1];

            return PartialView("AddData_Temp");
        }




        // ================= UPDATE =================

        // ================= ADD DATA =================

        [HttpGet]
        public IActionResult AddData_temp(string encId, string returnUrl)
        {
            if (string.IsNullOrEmpty(encId))
                return BadRequest();
            string[] split_val = WebUtility.UrlDecode(encId).ToString().Split(new string[] { "|$|" }, StringSplitOptions.None);
            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(split_val[1]));

            // 🔹 LOAD FIELDS FROM CONTEXT TEMPLATE REFERENCE added
            var fields = _detailBal.LoadFieldsByTemplateReference(ctrId, 1);
            ViewBag.Mode = "Block";

            // Only main fields
            fields = fields.Where(x => x.is_block == 1).ToList();
            var reference = _ctrBal.GetById(ctrId);
            ViewBag.temp_cont_id = split_val[0];
            ViewBag.Fields = fields;
            ViewBag.Reference = reference;
            ViewBag.ReturnUrl = WebUtility.UrlDecode(returnUrl);
            ViewBag.EncId = split_val[1];

            return PartialView("AddData_Temp");
        }



        [HttpPost]
        public IActionResult Submit_temp(int referenceId, int masterId, string returnUrl, string temp_cont_id, int is_block)
        {
            bool valid_status = true;
            string validation_error = "";
            try
            {
                var fields = _detailBal.LoadFieldsByTemplateReference(referenceId, is_block);

                if (fields == null || !fields.Any())
                {
                    return Json(new { success = false, message = "No fields configured" });
                }
                foreach (var f in fields)
                {
                    string value = Request.Form[f.name_key];
                    string filePath = null;

                    // for htmleditor validation
                    string cleanValue = value;

                    if (!string.IsNullOrEmpty(value) && f.field_type_id == 7)
                    {
                        // Remove HTML tags
                        cleanValue = System.Text.RegularExpressions.Regex
                            .Replace(value, "<.*?>", string.Empty)
                            .Replace("&nbsp;", "")
                            .Trim();
                    }
                    //end


                    // =========================
                    // 🔹 REQUIRED VALIDATION
                    // =========================
                    if (f.is_required == 1)
                    {
                        if (f.field_type_id == 8)
                        {
                            if (string.IsNullOrWhiteSpace(filePath))
                            {
                                validation_error = validation_error + "</br>" + $" {f.name} is required";
                                valid_status = false;
                            }
                        }
                        else if (f.field_type_id == 7)
                        {
                            // 🔥 CKEDITOR FIX
                            if (string.IsNullOrWhiteSpace(cleanValue))
                            {
                                validation_error = validation_error + "</br>" + $" {f.name} is required";
                                valid_status = false;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                validation_error = validation_error + "</br>" + $" {f.name} is required";
                                valid_status = false;
                            }
                        }
                    }

                    // =========================
                    // 🔹 REGEX VALIDATION
                    // =========================
                    if (!string.IsNullOrEmpty(f.field_validation))
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {

                            if (!System.Text.RegularExpressions.Regex.IsMatch(value, f.field_validation))
                            {
                                if (!string.IsNullOrWhiteSpace(f.custom_validation))
                                {
                                    validation_error = validation_error + "</br>" + $" {f.name} is required";
                                    valid_status = false;
                                }
                            }
                        }
                    }
                }

                if (!valid_status)
                {
                    return Json(new { success = false, message = validation_error });
                }


                // 🔹 Group ID (only for block)
                Guid groupId = Guid.NewGuid();
 

                foreach (var f in fields)
                {
                    string value = Request.Form[f.name_key];
                    string filePath = null;

                    // =========================
                    // 🔹 IMAGE FIELD HANDLING
                    // =========================
                    if (f.field_type_id == 8)
                    {
                        filePath = Request.Form[$"{f.name_key}_existing"];
                        value = null; // image uses file path only
                    }

                    // // =========================
                    // // 🔹 REQUIRED VALIDATION
                    // // =========================
                    // if (f.is_required == 1)
                    // {
                    //     if (f.field_type_id == 8)
                    //     {
                    //         if (string.IsNullOrWhiteSpace(filePath))
                    //         {
                    //             return Json(new { success = false, message = $"{f.name} is required" });
                    //         }
                    //     }
                    //     else
                    //     {
                    //         if (string.IsNullOrWhiteSpace(value))
                    //         {
                    //             return Json(new { success = false, message = $"{f.name} is required" });
                    //         }
                    //     }
                    // }

                    // // =========================
                    // // 🔹 REGEX VALIDATION
                    // // =========================
                    // if (!string.IsNullOrEmpty(f.field_validation))
                    // {
                    //     if (!string.IsNullOrWhiteSpace(value))
                    //     {
                    //         try
                    //         {
                    //             if (!System.Text.RegularExpressions.Regex.IsMatch(value, f.field_validation))
                    //             {

                    //                 if (!string.IsNullOrWhiteSpace(f.custom_validation))
                    //                 {
                    //                     return Json(new
                    //                     {
                    //                         success = false,
                    //                         message = $"{f.custom_validation}"
                    //                     });



                    //                 }
                    //                 else
                    //                 {
                    //                     return Json(new
                    //                     {
                    //                         success = false,
                    //                         message = $"{f.name} is invalid"
                    //                     });
                    //                 }
                    //             }


                    //         }
                    //         catch
                    //         {
                    //             return Json(new
                    //             {
                    //                 success = false,
                    //                 message = $"Invalid validation rule for {f.name}"
                    //             });
                    //         }
                    //     }
                    // }

                    // =========================
                    // 🔹 SAVE
                    // =========================
                    _detailBal.Save_temp(new ContextDetail
                    {
                        context_template_reference_id = referenceId,
                        context_master_id = masterId,
                        Language_Master_ID = 1,
                        context_field_id = f.id,
                        content = value,
                        File_path = filePath,
                        Created_UserID = Convert.ToInt32(User.GetUserId()),
                        context_group_id = groupId,
                    }, temp_cont_id);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpGet]
        public IActionResult AddMainComponent(string encId, string returnUrl)
        {

            if (string.IsNullOrEmpty(encId))
                return BadRequest();
            string[] split_val = WebUtility.UrlDecode(encId).ToString().Split(new string[] { "|$|" }, StringSplitOptions.None);
            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(split_val[1]));

            // 🔹 LOAD FIELDS FROM CONTEXT TEMPLATE REFERENCE  
            var fields = _detailBal.LoadFieldsByTemplateReference(ctrId, 0);
            ViewBag.Mode = "Main";
            // Only main fields
            fields = fields.Where(x => x.is_block == 0).ToList();
            var reference = _ctrBal.GetById(ctrId);
            ViewBag.cont_id = split_val[0];
            ViewBag.Fields = fields;
            ViewBag.Reference = reference;
            ViewBag.ReturnUrl = WebUtility.UrlDecode(returnUrl);
            ViewBag.EncId = split_val[1];

            return PartialView("AddData");
        }


        [HttpGet]
        public IActionResult AddData(string encId, string returnUrl)
        {
            if (string.IsNullOrEmpty(encId))
                return BadRequest();
            string[] split_val = WebUtility.UrlDecode(encId).ToString().Split(new string[] { "|$|" }, StringSplitOptions.None);
            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(split_val[1]));

            // 🔹 LOAD FIELDS FROM CONTEXT TEMPLATE REFERENCE added
            var fields = _detailBal.LoadFieldsByTemplateReference(ctrId, 1);
            ViewBag.Mode = "Block";

            // Only main fields
            fields = fields.Where(x => x.is_block == 1).ToList();
            var reference = _ctrBal.GetById(ctrId);
            ViewBag.cont_id = split_val[0];
            ViewBag.Fields = fields;
            ViewBag.Reference = reference;
            ViewBag.ReturnUrl = WebUtility.UrlDecode(returnUrl);
            ViewBag.EncId = split_val[1];

            return PartialView("AddData");
        }

        // ================= SAVE =================

        // [HttpPost]
        // public IActionResult Submit(int referenceId, int masterId, string returnUrl, string cont_id,int is_block )
        // {

        //     var fields = _detailBal.LoadFieldsByTemplateReference(referenceId,is_block);
        //     //  ONE group ID for this component instance
        //     Guid groupId = Guid.NewGuid();
        //     foreach (var f in fields)
        //     {
        //         string value = Request.Form[f.name_key];
        //         string filePath = null;

        //         // IMAGE FIELD
        //         if (f.field_type_id == 8 )
        //         {
        //             filePath = Request.Form[$"{f.name_key}_existing"];
        //         }

        //         _detailBal.Save(new ContextDetail
        //         {
        //             context_template_reference_id = referenceId,
        //             context_master_id = masterId,
        //             Language_Master_ID = 1,
        //             context_field_id = f.id,
        //             content = value,
        //             File_path = filePath,
        //             Created_UserID = 1,
        //             context_group_id = groupId,
        //             cont_id=Convert.ToInt32(cont_id)
        //         });
        //     }

        //     // if (!string.IsNullOrEmpty(returnUrl))
        //     //     return Redirect(returnUrl);

        //     // return PartialView("AddData");
        //     return Json(new { success = true });
        // }





        [HttpPost]
        public IActionResult Submit(int referenceId, int masterId, string returnUrl, string cont_id, int is_block)
        {
            bool valid_status = true;
            string validation_error = "";
            try
            {
                var fields = _detailBal.LoadFieldsByTemplateReference(referenceId, is_block);

                if (fields == null || !fields.Any())
                {
                    return Json(new { success = false, message = "No fields configured" });
                }


                foreach (var f in fields)
                {
                    string value = Request.Form[f.name_key].ToString();
                    string filePath = null;




                    // for htmleditor validation
                    string cleanValue = value;

                    if (!string.IsNullOrEmpty(value) && f.field_type_id == 7)
                    {
                        // Remove HTML tags
                        cleanValue = System.Text.RegularExpressions.Regex
                            .Replace(value, "<.*?>", string.Empty)
                            .Replace("&nbsp;", "")
                            .Trim();
                    }
                    //end


                    // =========================
                    // 🔹 REQUIRED VALIDATION
                    // =========================
                    if (f.is_required == 1)
                    {
                        if (f.field_type_id == 8)
                        {
                            if (string.IsNullOrWhiteSpace(filePath))
                            {
                                validation_error = validation_error + "</br>" + $" {f.name} is required";
                                valid_status = false;
                            }
                        }
                        else if (f.field_type_id == 7)
                        {
                            // 🔥 CKEDITOR FIX
                            if (string.IsNullOrWhiteSpace(cleanValue))
                            {
                                validation_error = validation_error + "</br>" + $" {f.name} is required";
                                valid_status = false;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                validation_error = validation_error + "</br>" + $" {f.name} is required";
                                valid_status = false;
                            }
                        }
                    }

                    // =========================
                    // 🔹 REGEX VALIDATION
                    // =========================
                    if (!string.IsNullOrEmpty(f.field_validation))
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {

                            if (!System.Text.RegularExpressions.Regex.IsMatch(value, f.field_validation))
                            {
                                if (!string.IsNullOrWhiteSpace(f.custom_validation))
                                {
                                    validation_error = validation_error + "</br>" + $" {f.name} is required";
                                    valid_status = false;
                                }
                            }
                        }
                    }
                }

                if (!valid_status)
                {
                    return Json(new { success = false, message = validation_error });
                }

                // 🔹 Group only for block
                Guid groupId = Guid.NewGuid();

                foreach (var f in fields)
                {
                    string value = Request.Form[f.name_key];
                    string filePath = null;




                    // =========================
                    // 🔹 IMAGE FIELD
                    // =========================
                    if (f.field_type_id == 8)
                    {
                        filePath = Request.Form[$"{f.name_key}_existing"];
                        value = null;
                    }




                    // =========================
                    // 🔹 SAVE
                    // =========================
                    _detailBal.Save(new ContextDetail
                    {
                        context_template_reference_id = referenceId,
                        context_master_id = masterId,
                        Language_Master_ID = 1,
                        context_field_id = f.id,
                        content = value,
                        File_path = filePath,
                        Created_UserID = Convert.ToInt32(User.GetUserId()),
                        context_group_id = groupId,
                        cont_id = Convert.ToInt32(CryptoEngine.Decrypt(cont_id))
                    });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        //----------edit for add page -------//


        // [HttpGet]
        // public IActionResult EditComponent_temp(string encId, string gid, string returnUrl)
        // {
        //     if (string.IsNullOrEmpty(encId))
        //         return BadRequest();

        //     int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(encId));

        //     //  LOAD ONLY ONE COMPONENT INSTANCE
        //     var model = _detailBal.LoadFieldsForEditByGroup(ctrId, gid);
        //     ViewBag.Mode = "Main";

        //     // Only main fields
        //     model = model.Where(x => x.is_block == 0).ToList();
        //     var reference = _ctrBal.GetById(ctrId);

        //     ViewBag.Reference = reference;
        //     ViewBag.EncId = encId;
        //     ViewBag.GroupId = gid;
        //     ViewBag.ReturnUrl = returnUrl;

        //     return View(model);
        // }

        // [HttpGet]
        // public IActionResult EditData_temp(string encId, string gid, string returnUrl)
        // {
        //     if (string.IsNullOrEmpty(encId))
        //         return BadRequest();

        //     int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(encId));

        //     //  LOAD ONLY ONE COMPONENT INSTANCE
        //     var model = _detailBal.LoadFieldsForEditByGroup_temp(ctrId, gid);
        //     ViewBag.Mode = "Block";
        //     // Only main fields
        //     model = model.Where(x => x.is_block == 1).ToList();
        //     var reference = _ctrBal.GetById(ctrId);

        //     ViewBag.Reference = reference;
        //     ViewBag.EncId = encId;
        //     ViewBag.GroupId = gid;
        //     ViewBag.ReturnUrl = returnUrl;

        //     return View(model);
        // }

        //------end----------------//






        // ================= EDIT =================


        [HttpGet]
        public IActionResult EditComponent(string encId, string gid, string cont_id, string returnUrl)
        {
            if (string.IsNullOrEmpty(encId))
                return BadRequest();

            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(encId));

            //  LOAD ONLY ONE COMPONENT INSTANCE
            var model = _detailBal.LoadFieldsForEditByGroup_temp(ctrId, gid, 0);
            ViewBag.Mode = "Main";

            // Only main fields
            model = model.Where(x => x.is_block == 0).ToList();
            var reference = _ctrBal.GetById(ctrId);

            ViewBag.Reference = reference;
            ViewBag.EncId = encId;
            ViewBag.GroupId = gid;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ContId = cont_id;
            ViewBag.is_block = 0;
            // return PartialView(model);

            // return PartialView("EditData");

            return View(model);
        }

        [HttpGet]
        public IActionResult EditData(string encId, string gid, string cont_id, string returnUrl)
        {
            if (string.IsNullOrEmpty(encId))
                return BadRequest();

            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(encId));
            int contid = Convert.ToInt32(CryptoEngine.Decrypt(cont_id));

            //  LOAD ONLY ONE COMPONENT INSTANCE
            //  LOAD ONLY ONE COMPONENT INSTANCE
            var model = _detailBal.LoadFieldsForEditByGroup_temp(ctrId, gid, 1);
            ViewBag.Mode = "Block";
            // Only main fields
            model = model.Where(x => x.is_block == 1).ToList();
            var reference = _ctrBal.GetById(ctrId);

            ViewBag.Reference = reference;
            ViewBag.EncId = encId;
            ViewBag.GroupId = gid;
            ViewBag.ContId = cont_id;
            ViewBag.is_block = 1;
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteData(string CTR_Id, string gid)
        {
            int Id = 0;
            if (string.IsNullOrEmpty(CTR_Id) && string.IsNullOrEmpty(gid))
                return BadRequest();

            Id = Convert.ToInt32(CryptoEngine.Decrypt(CTR_Id));
            _detailBal.DeleteField_temp(Id, gid);
            return Json(new { success = true });
        }




        public IActionResult PreviewLayout(string encId)
        {
            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(encId));

            var reference = _ctrBal.GetById(ctrId);
            var layout = "";
            // 🔹 Layout
            layout = _contextBal
                .GetById(reference.Context_Master_ID)!
                .Design_Layout;

            // 🔹 Details
            var details = _ctrBal.GetDetailsByCTRpre(ctrId);

            // 🔹 Fields (IMPORTANT)
            var fields = _detailBal.LoadFieldsByTemplateReferencepre(ctrId);

            // 🔹 Render
            var html = RenderLayout(layout, details, fields);

            return Content(html, "text/html");
        }
        private string RenderLayout(
        string layout,
        List<ContextDetail> details,
        List<ContextFieldDefinition> fields)
        {
            if (string.IsNullOrEmpty(layout))
                return "";

            // 🔹 field_id → name_key
            var fieldMap = fields.ToDictionary(x => x.id, x => x.name_key);

            // =========================
            // 🔹 1. MAIN COMPONENT
            // =========================
            var mainFields = details
                .Where(x => x.context_group_id != null)
                .ToList();

            foreach (var d in mainFields)
            {
                if (!d.context_field_id.HasValue)
                    continue;

                int fieldId = d.context_field_id.Value;

                if (!fieldMap.ContainsKey(fieldId))
                    continue;

                string key = fieldMap[fieldId];

                // 🔥 FIX IMAGE / CONTENT VALUE
                string value = !string.IsNullOrEmpty(d.File_path)
                                ? d.File_path_name
                                : d.content;

                // 🔥 FIX PLACEHOLDER FORMAT
                string placeholder = key;

                layout = layout.Replace(placeholder, value ?? "");
            }

            // =========================
            // 🔹 2. BLOCK COMPONENT
            // =========================
            var blockGroups = details
                .Where(x => x.context_group_id != null)
                .GroupBy(x => x.context_group_id);

            if (layout.Contains("{{BLOCK_START}}") && layout.Contains("{{BLOCK_END}}"))
            {
                int start = layout.IndexOf("{{BLOCK_START}}");
                int end = layout.IndexOf("{{BLOCK_END}}");

                string blockTemplate = layout.Substring(
                    start + "{{BLOCK_START}}".Length,
                    end - start - "{{BLOCK_START}}".Length
                );

                string finalBlocks = "";

                foreach (var group in blockGroups)
                {
                    string blockHtml = blockTemplate;

                    foreach (var d in group)
                    {
                        if (!d.context_field_id.HasValue)
                            continue;

                        int fieldId = d.context_field_id.Value;

                        if (!fieldMap.ContainsKey(fieldId))
                            continue;

                        string key = fieldMap[fieldId];

                        string value = !string.IsNullOrEmpty(d.File_path)
                                        ? d.File_path_name
                                        : d.content;

                        string placeholder = key;

                        blockHtml = blockHtml.Replace(placeholder, value ?? "");
                    }

                    finalBlocks += blockHtml;
                }

                layout = layout.Substring(0, start)
                       + finalBlocks
                       + layout.Substring(end + "{{BLOCK_END}}".Length);
            }

            return layout;
        }



        [HttpPost]
        public IActionResult EditDataSubmit(
            int referenceId,
            int masterId,
            string encId,
            string groupId,
            string ContId,
            int is_block,
            string returnUrl)
        {
            try
            {
                if (referenceId <= 0 || string.IsNullOrWhiteSpace(groupId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                // 🔹 Load fields from temp table
                var fields = _detailBal.LoadFieldsForEditByGroup_temp(referenceId, groupId, is_block);

                if (fields == null || !fields.Any())
                {
                    return Json(new { success = false, message = "No fields found for update" });
                }

                foreach (var f in fields)
                {
                    string key = f.name_key;

                    // ============================
                    // 🔹 GET DETAIL ID FROM FORM
                    // ============================
                    string detailKey = $"{key}_detail_id";

                    if (Request.Form.ContainsKey(detailKey))
                    {
                        if (int.TryParse(Request.Form[detailKey], out int detailId))
                        {
                            f.detail_id = detailId;
                        }
                    }

                    //  SAFETY CHECK  //removed for showing new columns added
                    // if (f.detail_id == null || f.detail_id == 0)
                    // {
                    //     return Json(new
                    //     {
                    //         success = false,
                    //         message = $"Invalid detail ID for {f.name}"
                    //     });
                    // }

                    // ============================
                    // 🔹 GET VALUE (SAFE)
                    // ============================
                    string value = Request.Form.ContainsKey(key)
                        ? Request.Form[key].ToString()?.Trim()
                        : null;

                    string filePath = null;

                    // ============================
                    // 🔹 IMAGE FIELD
                    // ============================
                    if (f.field_type_id == 8)
                    {
                        string newMedia = Request.Form[$"{key}_new"];
                        string existingMedia = Request.Form[$"{key}_existing"];

                        filePath = !string.IsNullOrWhiteSpace(newMedia)
                            ? newMedia
                            : existingMedia;

                        value = null;
                    }

                    // ============================
                    // 🔹 REQUIRED VALIDATION
                    // ============================
                    if (f.is_required == 1)
                    {
                        if (f.field_type_id == 8)
                        {
                            if (string.IsNullOrWhiteSpace(filePath))
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = $"{f.name} is required"
                                });
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = $"{f.name} is required"
                                });
                            }
                        }
                    }

                    // ============================
                    // 🔹 REGEX VALIDATION
                    // ============================
                    if (!string.IsNullOrWhiteSpace(f.field_validation) &&
                        !string.IsNullOrWhiteSpace(value))
                    {
                        try
                        {
                            var regex = new System.Text.RegularExpressions.Regex(
                                f.field_validation,
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase
                            );

                            if (!regex.IsMatch(value))
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = !string.IsNullOrWhiteSpace(f.custom_validation)
                                        ? f.custom_validation
                                        : $"{f.name} is invalid"
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new
                            {
                                success = false,
                                message = $"Validation error in {f.name}: {ex.Message}"
                            });
                        }
                    }

                    // ============================
                    // 🔹 ASSIGN VALUES
                    // ============================
                    f.content = value;
                    f.File_path = filePath;

                    // ============================
                    // 🔹 DEBUG (optional)
                    // ============================
                    // Console.WriteLine($"Updating ID: {f.detail_id}, Field: {f.name}, Value: {value}");

                    // ============================
                    // 🔹 UPDATE
                    // ============================
                    if (f.detail_id > 0)
                    {
                        _detailBal.UpdateField_temp(f);
                    }
                    else
                    {
                        _detailBal.Save_temp(
                            new ContextDetail
                            {

                                context_template_reference_id = referenceId,
                                context_master_id = masterId,
                                Language_Master_ID = 1,
                                context_field_id = f.field_id,
                                content = value,
                                File_path = filePath,
                                context_group_id = Guid.Parse(groupId),
                                Created_UserID = Convert.ToInt32(User.GetUserId()),
                                cont_id = Convert.ToInt32(CryptoEngine.Decrypt(ContId))
                            },
                            ""
                        );
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }



        [HttpGet]
        public IActionResult EditData_Temp(string encId, string gid, string temp_cont_id, string returnUrl)
        {
            if (string.IsNullOrEmpty(encId))
                return BadRequest();

            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(encId));

            //  LOAD ONLY ONE COMPONENT INSTANCE
            var model = _detailBal.LoadFieldsForEditByGroup_temp(ctrId, gid, 1);
            ViewBag.Mode = "Block";
            // Only main fields
            model = model.Where(x => x.is_block == 1).ToList();
            var reference = _ctrBal.GetById(ctrId);

            ViewBag.Reference = reference;
            ViewBag.EncId = encId;
            ViewBag.GroupId = gid;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.temp_cont_id = temp_cont_id;
            ViewBag.is_block = 1;
            return View(model);
        }
        [HttpGet]
        public IActionResult EditMainComponent_Temp(string encId, string gid, string temp_cont_id, string returnUrl)
        {
            if (string.IsNullOrEmpty(encId))
                return BadRequest();

            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(encId));

            //  LOAD ONLY ONE COMPONENT INSTANCE
            var model = _detailBal.LoadFieldsForEditByGroup_temp(ctrId, gid,0);
            ViewBag.Mode = "Main";

            // Only main fields
            model = model.Where(x => x.is_block == 0).ToList();
            var reference = _ctrBal.GetById(ctrId);

            ViewBag.Reference = reference;
            ViewBag.EncId = encId;
            ViewBag.GroupId = gid;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.temp_cont_id = temp_cont_id;
            return View(model);

            //return PartialView("EditData_Temp");
        }
        [HttpPost]
        public IActionResult EditDataSubmit_temp(
                    int referenceId,
                    int masterId,
                    string encId,
                    string groupId,
                    string temp_cont_id,
                    int is_block,
                    string returnUrl)
        {
            try
            {
                if (referenceId <= 0 || string.IsNullOrWhiteSpace(groupId))
                {
                    return Json(new { success = false, message = "Invalid request data" });
                }

                // 🔹 Load fields from temp table
                var fields = _detailBal.LoadFieldsForEditByGroup_temp(referenceId, groupId, is_block);

                if (fields == null || !fields.Any())
                {
                    return Json(new { success = false, message = "No fields found for update" });
                }

                foreach (var f in fields)
                {
                    string key = f.name_key;

                    // ============================
                    // 🔹 GET DETAIL ID FROM FORM
                    // ============================
                    string detailKey = $"{key}_detail_id";

                    if (Request.Form.ContainsKey(detailKey))
                    {
                        if (int.TryParse(Request.Form[detailKey], out int detailId))
                        {
                            f.detail_id = detailId;
                        }
                    }

                    // 🔥 SAFETY CHECK
                    // if (f.detail_id == null || f.detail_id == 0)
                    // {
                    //     return Json(new
                    //     {
                    //         success = false,
                    //         message = $"Invalid detail ID for {f.name}"
                    //     });
                    // }

                    // ============================
                    // 🔹 GET VALUE (SAFE)
                    // ============================
                    string value = Request.Form.ContainsKey(key)
                        ? Request.Form[key].ToString()?.Trim()
                        : null;

                    string filePath = null;

                    // ============================
                    // 🔹 IMAGE FIELD
                    // ============================
                    if (f.field_type_id == 8)
                    {
                        string newMedia = Request.Form[$"{key}_new"];
                        string existingMedia = Request.Form[$"{key}_existing"];

                        filePath = !string.IsNullOrWhiteSpace(newMedia)
                            ? newMedia
                            : existingMedia;

                        value = null;
                    }

                    // ============================
                    // 🔹 REQUIRED VALIDATION
                    // ============================
                    if (f.is_required == 1)
                    {
                        if (f.field_type_id == 8)
                        {
                            if (string.IsNullOrWhiteSpace(filePath))
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = $"{f.name} is required"
                                });
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(value))
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = $"{f.name} is required"
                                });
                            }
                        }
                    }

                    // ============================
                    // 🔹 REGEX VALIDATION
                    // ============================
                    if (!string.IsNullOrWhiteSpace(f.field_validation) &&
                        !string.IsNullOrWhiteSpace(value))
                    {
                        try
                        {
                            var regex = new System.Text.RegularExpressions.Regex(
                                f.field_validation,
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase
                            );

                            if (!regex.IsMatch(value))
                            {
                                return Json(new
                                {
                                    success = false,
                                    message = !string.IsNullOrWhiteSpace(f.custom_validation)
                                        ? f.custom_validation
                                        : $"{f.name} is invalid"
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new
                            {
                                success = false,
                                message = $"Validation error in {f.name}: {ex.Message}"
                            });
                        }
                    }

                    // ============================
                    // 🔹 ASSIGN VALUES
                    // ============================
                    f.content = value;
                    f.File_path = filePath;

                    // ============================
                    // 🔹 DEBUG (optional)
                    // ============================
                    // Console.WriteLine($"Updating ID: {f.detail_id}, Field: {f.name}, Value: {value}");

                    // ============================
                    // 🔹 UPDATE  updated for shwoing new columns added
                    // ============================
                    // _detailBal.UpdateField_temp(f);


                    if (f.detail_id > 0)
                    {
                        _detailBal.UpdateField_temp(f);
                    }
                    else
                    {
                        _detailBal.Save_temp(
                            new ContextDetail
                            {
                                context_template_reference_id = referenceId,
                                context_master_id = masterId,
                                Language_Master_ID = 1,
                                context_field_id = f.field_id,
                                content = value,
                                File_path = filePath,
                                context_group_id = Guid.Parse(groupId),
                                Created_UserID = Convert.ToInt32(User.GetUserId()),

                            }, temp_cont_id);


                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


    }





}