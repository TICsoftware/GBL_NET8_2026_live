using System.Diagnostics;
using Core_project_BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GBL_MVC.Classes;
using GBL_MVC.Helpers;

namespace GBL_MVC.Controllers.Manage
{
    public class ManageFilesController : Controller
    {
        private readonly ILogger<ManageController> _logger;
        private readonly IConfiguration objconfig;
        private readonly IWebHostEnvironment _env;


        public ManageFilesController(ILogger<ManageController> logger, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _logger = logger;
            objconfig = configuration;
            _env = environment;
        }


        [HttpGet]
        public IActionResult Upload(UploadFiles Model)
        {
            Model = new()
            {
                Language_master_id = 1,
                Section_id = 0,
                Subsection_id = 0
            };
            try
            {
                pageload(Model, 1);
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Get", ex);
            }
            return View(Model);
        }



        [HttpPost]
        public IActionResult Upload(UploadFiles Model, List<IFormFile> files)
        {
            using ContentManager objBal = new(objconfig);
            List<CMS_File_details> objFile = [];
            int groupid = 0, status = 0;
            string Mapped_sections = "", Tags = "";
            string result_message = "";
            try
            {
                if (files == null || files.Count == 0)
                {
                    files = Request.Form.Files.ToList();
                }

                if (files.Count == 0)
                {
                    return Json(new { success = false, message = "Please select a file to upload." });
                }

                string relativeFolder = objconfig["FolderPaths:PDF_path"] ?? "/uploads/pdf/";
                string basePath = Path.Combine(
                    _env.WebRootPath,
                    relativeFolder.Trim('/').Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(basePath);

                int parentId = Model.Subsection_id > 0 ? Model.Subsection_id : Model.Section_id;
                if (Model.Language_master_id == 1)
                {
                    if (Model.Content_Type_ID == Content_Types.Section)
                    {
                        groupid = parentId;
                    }
                    else
                    {
                        groupid = Model.article_id;
                    }
                }
                else
                {
                    if (Model.Content_Type_ID == Content_Types.Section)
                    {
                        groupid = parentId;
                    }
                    else
                    {
                        groupid = Model.article_id;
                    }
                    parentId = Model.Language_subSection_id > 0 ? Model.Language_subSection_id : Model.Language_section_id;
                }


                for (int i = 0; i < files.Count; i++)
                {
                    IFormFile file = files[i];
                    if (file == null || file.Length == 0)
                    {
                        continue;
                    }

                    SelectedFiles? selected = Model.FilesList != null && i < Model.FilesList.Count
                        ? Model.FilesList[i]
                        : null;

                    string cleanName = Helper.CleanFileName(file.FileName);
                    string fullPath = Path.Combine(basePath, cleanName);
                    if (!System.IO.File.Exists(fullPath))
                    {
                        using FileStream stream = new(fullPath, FileMode.Create);
                        file.CopyTo(stream);
                    }

                    string pageName = string.IsNullOrWhiteSpace(selected?.Filepagename)
                        ? Path.GetFileNameWithoutExtension(cleanName)
                        : selected.Filepagename.Replace(" ", "-");

                    if (Model.SelectedMapping_sections != null && Model.SelectedMapping_sections.Length > 0)
                    {
                        Mapped_sections = string.Join(",", Model.SelectedMapping_sections);
                    }

                    if (Model.SelectedTagging_sections != null && Model.SelectedTagging_sections.Length > 0)
                    {
                        Tags = string.Join(",", Model.SelectedTagging_sections);
                    }

                    objFile.Add(new CMS_File_details
                    {
                        Uploadfilename = cleanName,
                        UploadFileExtension = Path.GetExtension(cleanName),
                        UploadFileSize = file.Length.ToString(),
                        Title = selected.Title.ToString(),
                        Pagename = pageName,
                        Sequence = selected is { Sequence: > 0 } ? selected.Sequence : 1,
                        Filepdf = relativeFolder.TrimEnd('/') + "/" + cleanName,
                        UploadDate = selected?.Filedate ?? DateTime.Now,
                        cont_parent_id = parentId,
                        Language_ID = Model.Language_master_id,
                        Template_Master_ID = Model.Template_id,
                        Content_Type_ID = Model.Content_Type_ID ?? 0,
                        Geography_ID = Model.Geography_ID,
                        Created_Date = DateTime.Now,
                        Lang_groupid = groupid,
                        MappedList = Mapped_sections,
                        TagList = Tags
                    });
                }
                status = objBal.Add_Files_bal(objFile, Convert.ToInt32(User.GetUserId()), out result_message);
                TempData["alert_message"] = "Files uploaded successfully.";
                if(string.IsNullOrWhiteSpace(result_message))
                {
                    result_message ="Files uploaded successfully.";
                }
                return Json(new { success = true, message = result_message, count = objFile.Count });
            }
            catch (Exception ex)
            {
                FileLogger.LogError("Post", ex);
                return Json(new { success = false, message = "File upload failed." });
            }
        }

        private void pageload(UploadFiles Model, int status)
        {
            using ContentManager objBal = new(objconfig);
            try
            {
                CMS_pageload cmsbal = new();
                if (Model.Content_Type_ID == Core_project_BusinessLogic.Content_Types.Section)
                {
                    cmsbal = objBal.CMS_Pageload_Get_BAL(Model.Language_master_id, 0, status);
                }
                else
                {
                    cmsbal = objBal.CMS_Pageload_Get_BAL(Model.Language_master_id, 0, status);
                }

                Model.Languages = [];
                if (cmsbal.Languages != null && cmsbal.Languages.Count > 0)
                {
                    foreach (var item in cmsbal.Languages)
                    {
                        Model.Languages.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
                    }
                }

                Model.Geographies = [];
                if (cmsbal.geographies != null && cmsbal.geographies.Count > 0)
                {
                    foreach (var item in cmsbal.geographies)
                    {
                        Model.Geographies.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
                    }
                }
                Model.Templates = [];
                if (cmsbal.Templates != null && cmsbal.Templates.Count > 0)
                {
                    foreach (var item in cmsbal.Templates)
                    {
                        Model.Templates.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
                    }

                }


                Model.Sections = [];
                if (cmsbal.Sections != null && cmsbal.Sections.Count > 0)
                {
                    foreach (var item in cmsbal.Sections)
                    {
                        Model.Sections.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
                    }
                }
                Model.subSections = [];
                if (cmsbal.Subsections != null && cmsbal.Subsections.Count > 0)
                {
                    Model.subSections = Helper.BuildHierarchy(cmsbal.Subsections, Model.Section_id);
                }


                Model.Language_sections = [];
                if (cmsbal.Language_sections != null && cmsbal.Language_sections.Count > 0)
                {
                    foreach (var item in cmsbal.Language_sections)
                    {
                        Model.Language_sections.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
                    }
                }

                Model.Articles = [];
                if (cmsbal.sect_Articles != null && cmsbal.sect_Articles.Count > 0)
                {
                    foreach (var item in cmsbal.sect_Articles)
                    {
                        Model.Articles.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
                    }
                }

                Model.Language_subSections = [new SelectListItem { Value = "0", Text = "Select" }];
                if (Model.Language_master_id > 1 && Model.Language_section_id > 0)
                {
                    cmsbal.Language_subSections = new();
                    cmsbal.Language_subSections = objBal.Language_Subsections_Get_BAL(Model.Language_section_id, Model.Language_master_id);

                    if (cmsbal.Language_subSections != null && cmsbal.Language_subSections.Count > 0)
                    {
                        Model.Language_subSections = Helper.BuildHierarchy(cmsbal.Language_subSections, Model.Language_section_id);
                    }
                }

                Model.Mapping_sections = [];
                if (cmsbal.Mapping_Sections != null && cmsbal.Mapping_Sections.Count > 0)
                {
                    foreach (var item in cmsbal.Mapping_Sections)
                    {
                        Model.Mapping_sections.Add(new SelectListItem
                        {
                            Text = item.title,
                            Value = item.id.ToString(),
                            Selected = Model.SelectedMapping_sections != null && Model.SelectedMapping_sections.Contains(item.id ?? 0)
                        });
                    }
                }


                Model.Tagging_sections = [];
                if (cmsbal.Tagging_Sections != null && cmsbal.Tagging_Sections.Count > 0)
                {
                    foreach (var item in cmsbal.Tagging_Sections)
                    {
                        Model.Tagging_sections.Add(new SelectListItem
                        {
                            Text = item.title,
                            Value = item.id.ToString(),
                            Selected = Model.SelectedTagging_sections != null && Model.SelectedTagging_sections.Contains(item.id ?? 0)
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                objBal.Dispose();
            }

        }

    }

}