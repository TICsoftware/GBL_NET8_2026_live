using System.Security.Cryptography.X509Certificates;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GBL_MVC.Filters;
using GBL_MVC.Helpers;

namespace GBL_MVC.Controllers.Manage
{
    [Authorize]
    [SessionAuthorize]
    public class TagMasterController : Controller
    {
        private readonly IConfiguration objconfig;
        public TagMasterController(IConfiguration configuration)
        {
            objconfig = configuration;
        }

        public IActionResult Index(List_Model_Tags Model, string Command)
        {
            using TagMaster_BAL _bal = new TagMaster_BAL(objconfig);
            List<Options_List> list_languages = [];
            List<TagMaster> objtags = new();
            Model.List_tags = [];
            Model.Languages = [];
            try
            {
                if (Command == null)
                {
                    Model.Status = 1;
                    Model.language_id = 1;
                    objtags = _bal.GetAllTags_BAL(Model.Status, Model.language_id, out list_languages);
                }
                else
                {
                    objtags = _bal.GetAllTags_BAL(Model.Status, Model.language_id, out list_languages);
                }
                if (list_languages != null && list_languages.Count > 0)
                {
                    foreach (var item in list_languages)
                    {
                        Model.Languages.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
                    }
                }
                if (objtags != null && objtags.Count > 0)
                {
                    foreach (var item in objtags)
                    {
                        Model.List_tags.Add(new Model_Tag_Master
                        {
                            ID = item.ID,
                            Language_Master_ID = item.Language_Master_ID,
                            Tag_Name = item.Tag_name,
                            Language_Name = item.Languauge_Name,
                            Status = item.Status,
                            Created_Date = item.Created_Date?.ToString("dd-MMM-yyyy"),
                            Updated_Date = item.Updated_Date?.ToString("dd-MMM-yyyy")
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Post: ", ex);
                ModelState.AddModelError("", "Something went wrong. Please try again");
            }
            finally
            {
                _bal.Dispose();
            }
            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Model_Tag_Master model)
        {
            int result=0;
            using TagMaster_BAL _bal = new TagMaster_BAL(objconfig);
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["AlertMessage"] = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault() ?? "Please enter valid tag details.";
                    return RedirectToAction("Index");
                }


                TagMaster obj = new()
                {
                    Tag_name = model.Tag_Name?.Trim(),
                    Language_Master_ID = model.Language_Master_ID,
                    Status = 1
                };

                int Id = _bal.AddTag(obj, Convert.ToInt32(User.GetUserId()), out result);
                if (result == 2)
                {
                    TempData["AlertMessage"] = "Tag " +  model.Tag_Name + " added successfully.";
                }
                else if (result == 0)
                {
                    TempData["AlertMessage"] = "Tag already exists with deactivated status.";
                }
                else if (result == 0)
                {
                    TempData["AlertMessage"] = "Tag already exists with active status.";
                }
                else
                {
                    TempData["AlertMessage"] = "Something went wrong. Please try again.";
                }
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Add: ", ex);
                TempData["AlertMessage"] = "Something went wrong. Please try again";
            }
            finally
            {
                _bal.Dispose();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            using TagMaster_BAL _bal = new TagMaster_BAL(objconfig);
            try
            {
                int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));
                TagMaster obj = _bal.GetTagById_BAL(realId);
                if (obj == null || obj.ID == 0)
                {
                    return Json(new { success = false, message = "Tag not found." });
                }

                return Json(new
                {
                    success = true,
                    id = obj.ID,
                    tagName = obj.Tag_name,
                    languageMasterId = obj.Language_Master_ID,
                    status = obj.Status
                });
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Get Edit: ", ex);
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
            finally
            {
                _bal.Dispose();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Model_Tag_Master model)
        {
            using TagMaster_BAL _bal = new TagMaster_BAL(objconfig);
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        message = ModelState.Values.SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .FirstOrDefault() ?? "Please enter valid tag details."
                    });
                }

                if (model.ID <= 0)
                {
                    return Json(new { success = false, message = "Invalid tag." });
                }

                TagMaster obj = new()
                {
                    ID = model.ID,
                    Tag_name = model.Tag_Name?.Trim(),
                    Language_Master_ID = model.Language_Master_ID,
                    Status = model.Status
                };

                _bal.UpdateTag(obj, Convert.ToInt32(User.GetUserId()));
                return Json(new { success = true, message = "Tag updated successfully." });
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Edit: ", ex);
                return Json(new { success = false, message = "Something went wrong. Please try again" });
            }
            finally
            {
                _bal.Dispose();
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update_status(string Id, int status)
        {
            using TagMaster_BAL _bal = new TagMaster_BAL(objconfig);
            try
            {
                int realId = Convert.ToInt32(CryptoEngine.Decrypt(Id));
                if (realId <= 0)
                {
                    return Json(new { success = false, message = "Invalid tag." });
                }

                if (status != 0 && status != 1)
                {
                    return Json(new { success = false, message = "Invalid status." });
                }

                _bal.UpdateTagStatus(realId, status, Convert.ToInt32(User.GetUserId()));
                string actionText = status == 1 ? "activated" : "deactivated";
                return Json(new { success = true, message = "Tag " + actionText + " successfully." });
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Update_status: ", ex);
                return Json(new { success = false, message = "Something went wrong. Please try again" });
            }
            finally
            {
                _bal.Dispose();
            }
        }
    }
}