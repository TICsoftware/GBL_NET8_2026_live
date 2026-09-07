using Microsoft.AspNetCore.Mvc;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.AspNetCore.Authorization;
using GBL_MVC.Filters;
using System.Net;
using Core_project_BusinessLogic;
using GBL_MVC.Helpers;

namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class DynamicDataReprocessController : Controller
    {
        private readonly ContextDetail_BAL _detailBal;
        private readonly ContextTemplateReference_BAL _ctrBal;
        private readonly ContextMaster_BAL _contextBal;

        public DynamicDataReprocessController(IConfiguration config)
        {
            _detailBal = new ContextDetail_BAL(config);
            _ctrBal = new ContextTemplateReference_BAL(config);
            _contextBal = new ContextMaster_BAL(config);
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
                    _detailBal.Save_Reprocess(new ContextDetail
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
        public IActionResult EditComponent(string encId, string gid, string cont_id, string returnUrl)
        {
            if (string.IsNullOrEmpty(encId))
                return BadRequest();

            int ctrId = Convert.ToInt32(CryptoEngine.Decrypt(encId));

            //  LOAD ONLY ONE COMPONENT INSTANCE
            var model = _detailBal.LoadFieldsForEditByGroup_Reprocess(ctrId, gid, 0);
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
            var model = _detailBal.LoadFieldsForEditByGroup_Reprocess(ctrId, gid, 1);
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
                var fields = _detailBal.LoadFieldsForEditByGroup_Reprocess(referenceId, groupId, is_block);

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
                    string? value = Request.Form.ContainsKey(key)
                        ? Request.Form[key].ToString()?.Trim()
                        : null;

                    string? filePath = null;

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
                        _detailBal.UpdateDetail_Reprocess_DAL(f, Convert.ToInt32(User.GetUserId()));
                    }
                    else
                    {
                        _detailBal.Save_Reprocess(
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


        [HttpPost]
        public IActionResult DeleteData(string CTR_Id, string gid)
        {
            try
            {
                int Id = 0;
                if (string.IsNullOrEmpty(CTR_Id) && string.IsNullOrEmpty(gid))
                    return BadRequest();

                Id = Convert.ToInt32(CryptoEngine.Decrypt(CTR_Id));
                _detailBal.DeleteField_Reprocess(Id, gid, Convert.ToInt32(User.GetUserId()));
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