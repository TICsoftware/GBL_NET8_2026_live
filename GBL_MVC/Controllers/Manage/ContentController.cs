using System;
using Core_project_BusinessLogic;
using GBL_MVC.Classes;
using Microsoft.AspNetCore.Mvc;
using Core_project_BusinessLogic.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;

using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
using GBL_MVC.Helpers;
namespace GBL_MVC.Controllers;

[Authorize]
[SessionAuthorize]

public class ContentController : Controller
{
    private readonly IConfiguration objconfig;
    // private int userid = 0;
    public ContentController(IConfiguration configuration)
    {
        objconfig = configuration;
    }
    [HttpPost]
    public IActionResult EncryptTemplateId(int templateId)
    {
        return Json(CryptoEngine.Encrypt(templateId.ToString()));
    }
    [HttpGet]
    public IActionResult Add()
    {
        cms_Content Model = new cms_Content();
        try
        {
            Model.Id = 0;
            Model.Language_master_id = 1;
            Model.Section_id = 0;
            Model.Subsection_id = 0;
            Model.Spot_temp_id = Guid.NewGuid().ToString("N")[..12] + DateTime.Now.ToString("ddMMyyHHss");
            //Model.Spot_temp_id = "e39c524be0022406261214";
            pageload(Model, 1);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Get", ex);
        }
        return View(Model);
    }

    [HttpPost]
    public IActionResult Add(cms_Content Modelobj, string Command, IFormCollection form)
    {
        using ContentManager objBal = new(objconfig);
        int cont_parent_id = 0, add_status = 0, cont_id = 0;
        Content_Master ContentObj = new();
        string SelectedMapping = "", Selected_tagging = "";
        try
        {
            if (ModelState.IsValid)
            {
                if (Modelobj.Content_Type_ID == Content_Types.Related_Article && Modelobj.Section_id == 0 && Modelobj.Subsection_id == 0)
                {
                    ModelState.AddModelError("", "Select section for related article");
                }
                else if (!File_validation(Modelobj))
                {
                    ModelState.AddModelError("", Modelobj.validation_error ?? "Something wend wrong. Please try again.");
                }
                else
                {
                    cont_parent_id = (Modelobj.Subsection_id != 0 ? Modelobj.Subsection_id : Modelobj.Section_id) ?? 0;
                    Modelobj.Lang_groupid = (Modelobj.article_id != 0 ? Modelobj.article_id : cont_parent_id) ?? 0;

                    ContentObj.language_master_id = Modelobj.Language_master_id;
                    ContentObj.Content_Type_ID = Modelobj.Content_Type_ID;
                    ContentObj.Template_Master_ID = Modelobj.Template_Master_ID;
                    ContentObj.Template_type = Modelobj.template_type;
                    ContentObj.Geography_ID = Modelobj.Geography_ID;
                    if (Modelobj.Language_master_id > 1)
                    {
                        ContentObj.parent_id = Modelobj.Language_subsection_id != 0 ? Modelobj.Language_subsection_id : Modelobj.Language_section_id;
                    }
                    else
                    {
                        ContentObj.parent_id = cont_parent_id;
                    }

                    ContentObj.lang_groupid = Modelobj.Lang_groupid;
                    ContentObj.root_parent_id = Modelobj.Section_id;
                    ContentObj.pagename = Modelobj.Pagename.Trim().Replace(" ", "-"); ;
                    ContentObj.title = Modelobj.Title.Trim();
                    ContentObj.hmpg_title = string.IsNullOrWhiteSpace(Modelobj.Hmpg_title) ? "" : Modelobj.Hmpg_title.Trim();
                    ContentObj.breadcrumb_title = string.IsNullOrWhiteSpace(Modelobj.Breadcrumb_title) ? "" : Modelobj.Breadcrumb_title.Trim();
                    ContentObj.window_title = Modelobj.Window_title;
                    ContentObj.sequence = Modelobj.Sequence;
                    ContentObj.hmpg_intro = Modelobj.Hmpg_intro;
                    ContentObj.intro = Modelobj.Intro;
                    ContentObj.content = Modelobj.Content;
                    ContentObj.metatag = Modelobj.Metatag;
                    ContentObj.metadesc = Modelobj.Metadesc;
                    ContentObj.page_schema = Modelobj.PageSchema;
                    ContentObj.metaexpiry = Modelobj.Metaexpiry;
                    ContentObj.external_url = string.IsNullOrWhiteSpace(Modelobj.External_url) ? "" : Modelobj.External_url.Trim();
                    ContentObj.IsExternal = Modelobj.IsExternal ? 1 : 0;
                    ContentObj.ByLine = string.IsNullOrWhiteSpace(Modelobj.ByLine) ? "" : Modelobj.ByLine.Trim();
                    ContentObj.Publication = string.IsNullOrWhiteSpace(Modelobj.Publication) ? "" : Modelobj.Publication.Trim();
                    ContentObj.isSearch = Modelobj.IsSearch ? 1 : 0;
                    ContentObj.top_icon = Modelobj.Display_top_icon ? 1 : 0;
                    ContentObj.Thumb_image_id = Modelobj.Thumb_image_id ?? null;
                    ContentObj.Thumb_image_alttext = string.IsNullOrWhiteSpace(Modelobj.Thumb_image_alttext) ? "" : Modelobj.Thumb_image_alttext.Trim();
                    ContentObj.Small_Icon_Thumb_image_id = Modelobj.Small_Icon_Thumb_image_id ?? null;
                    ContentObj.Small_Icon_alttext = string.IsNullOrWhiteSpace(Modelobj.Small_Icon_alttext) ? "" : Modelobj.Small_Icon_alttext.Trim();
                    ContentObj.Masthead_image_id = Modelobj.Masthead_image_id ?? null;
                    ContentObj.Mobile_Masthead_image_id = Modelobj.Mobile_Masthead_image_id ?? null;
                    ContentObj.Masthead_image_alttext = string.IsNullOrWhiteSpace(Modelobj.Masthead_image_alttext) ? "" : Modelobj.Masthead_image_alttext.Trim();
                    ContentObj.Background_image_id = Modelobj.Background_image_id ?? null;
                    ContentObj.Background_image_Alttext = Modelobj.Background_image_Alttext;
                    ContentObj.Attach_file_id = Modelobj.Attach_file_id ?? null;
                    ContentObj.Spot_temp_id = Modelobj.Spot_temp_id;

                    if (Modelobj.SelectedMapping_sections != null && Modelobj.SelectedMapping_sections.Length > 0)
                    {
                        ContentObj.Mapped_sections = string.Join(",", Modelobj.SelectedMapping_sections);
                    }

                    if (Modelobj.SelectedTagging_sections != null && Modelobj.SelectedTagging_sections.Length > 0)
                    {
                        ContentObj.Tags = string.Join(",", Modelobj.SelectedTagging_sections);
                    }

                    if (Modelobj.Displaydate != null)
                    {
                        ContentObj.displaydate = Modelobj.Displaydate;
                    }
                    if (Command == "Save")
                    {
                        ContentObj.status = Content_Status.Draft;
                    }
                    else
                    {
                        ContentObj.status = Content_Status.Publish;
                    }
                    add_status = objBal.Add_content_BAL(ContentObj, Convert.ToInt32(User.GetUserId()), out cont_id);

                    if (add_status == 2)
                    {
                        if (Command == "Save")
                        {
                            TempData["alert_message"] = "Content with title " + ContentObj.title.Trim() + " is saved successfully";
                            return RedirectToAction("Drafts", "Content");
                        }
                        else
                        {
                            TempData["alert_message"] = "Content with title " + ContentObj.title.Trim() + " is saved and published successfully";
                            return RedirectToAction("Index", "Content");
                        }
                    }
                    else if (add_status == 1)
                    {
                        ModelState.AddModelError("", "Content with pagename " + ContentObj.pagename.Trim() + " is already exists");
                    }
                    else if (add_status == 0)
                    {
                        ModelState.AddModelError("", "Content with pagename " + ContentObj.pagename.Trim() + " is already exists with inactive status");
                    }
                    else
                    {
                        TempData["alert_message"] = "Something went wrong. Please try again.";
                        return RedirectToAction("Drafts", "Content");
                    }
                }
            }
            else
            {
                // ModelState.AddModelError("", "Please resolve below errors and submit again");

                var errors = ModelState.Values.SelectMany(v => v.Errors)
                      .Select(e => e.ErrorMessage)
                      .ToList();
                foreach (var err in errors)
                {
                    ModelState.AddModelError("", err);
                }
            }
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Before pageload function: Post", ex);
            ModelState.AddModelError("", "Something went wrong. Please try again");
        }
        finally
        {
            objBal.Dispose();
        }
        try
        {
            pageload(Modelobj, 1);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("After pageload function: Post", ex);
            ModelState.AddModelError("", "Something went wrong. Please try again");
        }
        return View(Modelobj);
    }


    //bind sections, subsections, languages , geographies
    private void pageload(cms_Content Model, int status)
    {
        using ContentManager objBal = new(objconfig);
        try
        {
            CMS_pageload cmsbal = new();
            if (Model.Content_Type_ID == Core_project_BusinessLogic.Content_Types.Section)
            {
                if (Model.Id == 0 || Model.Section_id == 0)
                    cmsbal = objBal.CMS_Pageload_Get_BAL(Model.Language_master_id ?? 1, Model.Id, status);
                else
                    cmsbal = objBal.CMS_Pageload_Get_BAL(Model.Language_master_id ?? 1, Model.Section_id ?? 0, status);

            }
            else
            {
                cmsbal = objBal.CMS_Pageload_Get_BAL(Model.Language_master_id ?? 1, Model.Section_id ?? 0, status);
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
                cmsbal.Language_subSections = objBal.Language_Subsections_Get_BAL(Model.Language_section_id ?? 0, Model.Language_master_id ?? 1);

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

    [HttpGet]
    public IActionResult Load_sections(int cont_id)
    {

        using ContentManager objBal = new(objconfig);
        SubSections_Articles obj = new();
        List<SelectListItem> subSections = new();
        List<SelectListItem> articles = new();

        try
        {
            obj = objBal.Subsections_Articles_Get_BAL(cont_id);
            if (obj.Subsections != null && obj.Subsections.Count > 0)
            {
                subSections = Helper.BuildHierarchy(obj.Subsections, cont_id);

            }
            if (obj.sect_Articles != null && obj.sect_Articles.Count > 0)
            {
                articles = obj.sect_Articles.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.title }).ToList();
            }

            return Json(new { listSections = subSections, listArticles = articles });

        }
        catch (Exception ex)
        {
            FileLogger.LogError(" ", ex);
            return Json(new { msg = ex.Message });
        }
        finally
        {
            objBal.Dispose();
        }
    }

     [HttpGet]
    public IActionResult Load_Articles(int cont_id)
    {
        using ContentManager objBal = new(objconfig);
        List<Options_List> obj = new();
        List<SelectListItem> articles = new();

        try
        {
            obj = objBal.Articles_Get_BAL(cont_id);
            if (obj != null && obj.Count > 0)
            {
                articles = obj.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.title }).ToList();
            }
            return Json(new { listArticles = articles });

        }
        catch (Exception ex)
        {
            FileLogger.LogError(" ", ex);
            return Json(new { msg = ex.Message });
        }
        finally
        {
            objBal.Dispose();
        }
    }

    [HttpGet]
    public IActionResult Load_Language_sections(int language_id)
    {
        using ContentManager objBal = new(objconfig);
        List<SelectListItem> sectionsObj = new();
        List<SelectListItem> tagsObj = new();
        List<SelectListItem> mapping_sectionsObj = new();
        List<SelectListItem> geographiesObj = new();
        try
        {
            sectionsObj = objBal.Language_Sections_Get_BAL(language_id).Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.title }).ToList();
            tagsObj = objBal.Tag_master_Get_BAL(language_id).Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.title }).ToList();
            mapping_sectionsObj = objBal.Sections_Mapping_Get_BAL(language_id).Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.title }).ToList();
            geographiesObj =  objBal.Geographies_Get_BAL(language_id).Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.title }).ToList();
            return Json(new { listsections = sectionsObj, taglist = tagsObj, mapped_sections= mapping_sectionsObj , geographies = geographiesObj});

        }
        catch (Exception ex)
        {
            FileLogger.LogError(" ", ex);
            return Json(new { msg = ex.Message });
        }
        finally
        {
            objBal.Dispose();
        }
    }

    [HttpGet]
    public IActionResult Load_Language_subsections(int language_id, int cont_id)
    {
        using ContentManager objBal = new(objconfig);
        List<SelectListItem> subSections = new();
        try
        {
            subSections = Helper.BuildHierarchy(objBal.Language_Subsections_Get_BAL(cont_id, language_id), cont_id);

            return Json(new { listsubsections = subSections });
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Load_Language_subsections", ex);
            return Json(new { msg = ex.Message });
        }
        finally
        {
            objBal.Dispose();
        }

    }

    public IActionResult Load_tags(int language_id)
    {
        using ContentManager objBal = new(objconfig);
        List<SelectListItem> sections = new();
        try
        {
            sections = objBal.Tag_master_Get_BAL(language_id).Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.title }).ToList();
            return Json(new { listsections = sections });

        }
        catch (Exception ex)
        {
            FileLogger.LogError(" ", ex);
            return Json(new { msg = ex.Message });
        }
        finally
        {
            objBal.Dispose();
        }

    }
    //Context details to display while adding content
    public IActionResult Add_Context_List(int template_Id, int language_id, string Spot_temp_id)
    {
        List_Components? ModelObj = new();
        ContextDetail_BAL? objBal = new(objconfig);
        Context_Component_Details lst_components = new();

        try
        {
            if (template_Id > 0)
            {

                ModelObj = new List_Components();
                lst_components = objBal.Context_Detail_List_For_Addcontent_BAL(template_Id, language_id, Spot_temp_id);
                if (lst_components._context != null && lst_components._context.Count > 0)
                {
                    ModelObj.Components = new List<Component>();
                    foreach (var item in lst_components._context.OrderBy(m => m.Sequence))
                    {
                        ModelObj.Components.Add(new Component()
                        {
                            CTR_Id = item.ID,
                            title = item.Context_Title,
                            sequence = item.Sequence,
                            component_label = item.Component_Label,
                            field_Id = item.field_id,
                            group_id = item.group_id,
                            block_field_count = item.is_block,
                            Component_field_count = item.Component_field_count
                        });
                    }

                    ModelObj.Field_details = new List<Component_data>();
                    if (lst_components._context_Details != null && lst_components._context_Details.Count > 0)
                    {
                        foreach (var item in lst_components._context_Details.OrderBy(m => m.Item4))
                        {
                            ModelObj.Field_details.Add(new Component_data()
                            {
                                Id = item.Item1,
                                CTR_Id = item.Item2,
                                context_group_id = item.Item3,
                                field_Title = item.Item4,
                                sequence = item.Item5
                            });
                        }
                    }
                }
                else
                {
                    ModelObj.Components = null;
                    ModelObj.Field_details = null;
                }
                return View(ModelObj);
            }
            else
            {
                return View(null);
            }
        }
        catch (Exception ex)
        {
            FileLogger.LogError("_Add_Context_List", ex);
            return Json(new { msg = ex.Message });
        }
        finally
        {
            objBal = null;
        }
    }

    //Temp Context details to display while editing content
    public IActionResult Edit_Context_Temp(int template_Id, int language_id, string Id_encrypt_val)
    {
        List_Components? ModelObj = new();
        ContextDetail_BAL? objBal = new(objconfig);
        Context_Component_Details lst_components = new();
        int cont_id = 0;
        try
        {
            if (template_Id > 0)
            {
                cont_id = Convert.ToInt32(CryptoEngine.Decrypt(Id_encrypt_val));
                ModelObj = new List_Components();
                lst_components = objBal.Context_Detail_Temp_List_BAL(template_Id, language_id, cont_id);
                if (lst_components._context != null && lst_components._context.Count > 0)
                {
                    ModelObj.Components = new List<Component>();
                    foreach (var item in lst_components._context.OrderBy(m => m.Sequence))
                    {
                        ModelObj.Components.Add(new Component()
                        {
                            CTR_Id = item.ID,
                            title = item.Context_Title,
                            sequence = item.Sequence,
                            component_label = item.Component_Label,
                            field_Id = item.field_id,
                            group_id = item.group_id,
                            block_field_count = item.is_block,
                            Component_field_count = item.Component_field_count
                        });
                    }

                    ModelObj.Field_details = new List<Component_data>();
                    if (lst_components._context_Details != null && lst_components._context_Details.Count > 0)
                    {
                        foreach (var item in lst_components._context_Details.OrderBy(m => m.Item4))
                        {
                            ModelObj.Field_details.Add(new Component_data()
                            {
                                Id = item.Item1,
                                CTR_Id = item.Item2,
                                context_group_id = item.Item3,
                                field_Title = item.Item4,
                                sequence = item.Item5
                            });
                        }
                    }
                }
                else
                {
                    ModelObj.Components = null;
                    ModelObj.Field_details = null;
                }
                return View("Edit_Context_Temp_List", ModelObj);
            }
            else
            {
                return View(null);
            }
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Edit_Context_Temp", ex);
            return Json(new { msg = ex.Message });
        }
        finally
        {
            objBal = null;
        }
    }

    //Context details to display while editing published content
    public IActionResult Edit_Context_List(int template_Id, int language_id, string Id_encrypt_val)
    {
        List_Components? ModelObj = new();
        ContextDetail_BAL? objBal = new(objconfig);
        Context_Component_Details lst_components = new();
        int cont_id = 0;
        try
        {
            if (template_Id > 0)
            {
                cont_id = Convert.ToInt32(CryptoEngine.Decrypt(Id_encrypt_val));
                ModelObj = new List_Components();
                lst_components = objBal.Context_Detail_List_For_content_GetAll_BAL(cont_id, template_Id, language_id);
                if (lst_components._context != null && lst_components._context.Count > 0)
                {
                    ModelObj.Components = new List<Component>();
                    foreach (var item in lst_components._context.OrderBy(m => m.Sequence))
                    {
                        ModelObj.Components.Add(new Component()
                        {
                            CTR_Id = item.ID,
                            title = item.Context_Title,
                            sequence = item.Sequence,
                            component_label = item.Component_Label,
                            field_Id = item.field_id,
                            group_id = item.group_id,
                            block_field_count = item.is_block,
                            Component_field_count = item.Component_field_count
                        });
                    }

                    ModelObj.Field_details = new List<Component_data>();
                    if (lst_components._context_Details != null && lst_components._context_Details.Count > 0)
                    {
                        foreach (var item in lst_components._context_Details.OrderBy(m => m.Item4))
                        {
                            ModelObj.Field_details.Add(new Component_data()
                            {
                                Id = item.Item1,
                                CTR_Id = item.Item2,
                                context_group_id = item.Item3,
                                field_Title = item.Item4,
                                sequence = item.Item5
                            });
                        }
                    }
                }
                else
                {
                    ModelObj.Components = null;
                    ModelObj.Field_details = null;
                }
                return View(ModelObj);
            }
            else
            {
                return View(null);
            }
        }
        catch (Exception ex)
        {
            FileLogger.LogError("_Edit_Context_List", ex);
            return Json(new { msg = ex.Message });
        }
        finally
        {
            objBal = null;
        }
    }

    private bool File_validation(cms_Content Model)
    {
        bool IsValid = true;
        if (Model.template_type == 1 && Model.Template_Master_ID == 0)
        {
            Model.validation_error = "Please select template";
        }
        return IsValid;

    }


    [HttpGet]
    public IActionResult Drafts()
    {
        if (TempData["alert_message"] != null)
        {
            ViewBag.alert_message = TempData["alert_message"];
        }
        CMS_pages Model = new();
        using ContentManager objBal = new(objconfig);
        try
        {
            Model.language_id = 1;
            Model.section_id = 0;
            Model.subSection_id = 0;
            Drafts_load(Model, objBal);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Get", ex);
            ModelState.AddModelError("", "Something went wrong. Please try again");
        }
        finally
        {

            objBal.Dispose();
        }
        return View(Model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Drafts(string page_no, CMS_pages Model, IFormCollection form, string action)
    {
        if (TempData["alert_message"] != null)
        {
            ViewBag.alert_message = TempData["alert_message"];
        }

        using ContentManager objBal = new(objconfig);
        try
        {
            Drafts_load(Model, objBal);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Post", ex);
            ModelState.AddModelError("", "Something went wrong. Please try again");
        }
        finally
        {

            objBal.Dispose();
        }
        return View(Model);
    }


    private void Drafts_load(CMS_pages Model, ContentManager objBal)
    {

        CMS_pageload cmsbal = new();
        Manage_List_CMS_page cmsObj = new();
        int cont_id = 0;
        try
        {

            if (Model.language_id == 1)
            {
                cont_id = Model.subSection_id > 0 ? Model.subSection_id : Model.section_id;
            }
            else
            {
                cont_id = Model.language_subSection_id > 0 ? Model.language_subSection_id : Model.language_section_id;
            }
            cmsbal = objBal.CMS_Pageload_Get_BAL(Model.language_id, cont_id, 1);
            Model.Languages = [];
            if (cmsbal.Languages != null && cmsbal.Languages.Count > 0)
            {
                foreach (var item in cmsbal.Languages)
                {
                    Model.Languages.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
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
                Model.subSections = Helper.BuildHierarchy(cmsbal.Subsections, Model.section_id);
            }
            Model.Geographies = [];
            if (cmsbal.geographies != null && cmsbal.geographies.Count > 0)
            {
                foreach (var item in cmsbal.geographies)
                {
                    Model.Geographies.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
                }
            }
            Model.Language_sections = [];
            if (cmsbal.Language_sections != null && cmsbal.Language_sections.Count > 0)
            {
                foreach (var item in cmsbal.Language_sections)
                {
                    Model.Language_sections.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
                }
            }

            Model.Language_subSections = [];
            if (cmsbal.Language_subSections != null && cmsbal.Language_subSections.Count > 0)
            {
                foreach (var item in cmsbal.Language_subSections)
                {
                    Model.Language_subSections.Add(new SelectListItem { Text = item.title, Value = item.id.ToString() });
                }
            }

            cmsObj = objBal.CMS_Section_articles_List(cont_id, Model.searchquery ?? "", Model.language_id, Model.current_page ?? 1);

            if (cmsObj.sections != null && cmsObj.sections.Count > 0)
            {
                Model.sections_list = new List<Page_detail>();
                foreach (var item in cmsObj.sections)
                {
                    Model.sections_list.Add(
                        new Page_detail
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Language = item.Language,
                            // Language =  Model.Languages.Where(x => x.Value == item.Id.ToString())
                            //         .Select(x => x.Text)
                            //         .FirstOrDefault(),
                            Parent_title = item.Parent_title,
                            Created_date = item.Created_date,
                            Updated_date = item.Updated_date,
                            Status = item.Status
                        });
                }
                Model.section_no_of_pages = cmsObj.Sections_no_of_pages;
            }
            else
            {
                Model.sections_list = null;
                Model.section_no_of_pages = 0;
            }

            if (cmsObj.articles != null && cmsObj.articles.Count > 0)
            {
                Model.articles_list = new List<Page_detail>();
                foreach (var item in cmsObj.articles)
                {
                    Model.articles_list.Add(
                        new Page_detail
                        {
                            Id = item.Id,
                            Title = item.Title,
                            Language = item.Language,
                            // Model.Languages.Where(x => x.Value == item.Id.ToString())
                            //         .Select(x => x.Text)
                            //         .FirstOrDefault(),
                            Parent_title = item.Parent_title,
                            Created_date = item.Created_date,
                            Updated_date = item.Updated_date,
                            Status = item.Status
                        });
                }
                Model.article_no_of_pages = cmsObj.Articles_no_of_pages;
            }
            else
            {
                Model.articles_list = null;
                Model.article_no_of_pages = 0;
            }
        }
        catch
        {
            throw;
        }
    }

    public IActionResult UpdateContentStatus(string cont_id)
    {
        using ContentManager objBal = new(objconfig);
        try
        {
            if (string.IsNullOrWhiteSpace(cont_id))
                return Json(new { message = "No record found", refreshstatus = 0 });

            int content_id = Convert.ToInt32(CryptoEngine.Decrypt(cont_id));
            objBal.Update_Content_Status_BAL(content_id, Content_Status.Delete, Convert.ToInt32(User.GetUserId()));
            return Json(new { message = "Selected record deleted successfully", refreshstatus = 1 });
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Post", ex);
            return Json(new { message = "Something went wrong. Please try again", refreshstatus = 1 });
        }
    }

    public IActionResult Index()
    {
        if (TempData["alert_message"] != null)
        {
            ViewBag.alert_message = TempData["alert_message"];
        }

        return View();
    }

    [HttpGet]
    public IActionResult Edit(string Id)
    {
        if (TempData["alert_message"] != null)
        {
            ViewBag.alert_message = TempData["alert_message"];
        }
        if (string.IsNullOrWhiteSpace(Id))
        {
            TempData["alert_message"] = "Invalid page details";
            return RedirectToAction("Drafts", "Content");
        }
        using ContentManager objBal = new(objconfig);
        cms_Content Model = new cms_Content();

        try
        {

            int cont_id = Convert.ToInt32(CryptoEngine.Decrypt(Id).ToString());
            Model = Get_page_Details(cont_id);
            Model.Id_encrypt_val = Id;
            pageload(Model, 1);

            //delete existing data from context_details_temp and copy from main table
            new ContextDetail_BAL(objconfig).Copy_Context_Details_temp_BAL(cont_id);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Get", ex);
            ModelState.AddModelError("", "Something went wrong. Please try again");
        }
        finally
        {
            objBal.Dispose();
        }
        return View(Model);
    }

    [HttpPost]
    public IActionResult Edit(cms_Content Modelobj, string Command, IFormCollection form)
    {
        using ContentManager objBal = new(objconfig);
        int cont_parent_id = 0, add_status = 0;
        Content_Master ContentObj = new();
        try
        {
            ContentObj.id = Convert.ToInt32(CryptoEngine.Decrypt(Modelobj.Id_encrypt_val ?? "").ToString());
            Modelobj.Id = ContentObj.id;

            if (ModelState.IsValid)
            {
                if (Modelobj.Content_Type_ID == Content_Types.Related_Article && Modelobj.Section_id == 0 && Modelobj.Subsection_id == 0)
                {
                    ModelState.AddModelError("", "Select section for related article");
                }
                else if (!File_validation(Modelobj))
                {
                    ModelState.AddModelError("", Modelobj.validation_error ?? "Something wend wrong. Please try again.");
                }
                else
                {
                    cont_parent_id = (Modelobj.Subsection_id != 0 ? Modelobj.Subsection_id : Modelobj.Section_id) ?? 0;
                    Modelobj.Lang_groupid = (Modelobj.article_id != 0 ? Modelobj.article_id : cont_parent_id) ?? 0;

                    ContentObj.language_master_id = Modelobj.Language_master_id;
                    ContentObj.Content_Type_ID = Modelobj.Content_Type_ID;
                    ContentObj.Template_Master_ID = Modelobj.Template_Master_ID;
                    ContentObj.Template_type = Modelobj.template_type;
                    ContentObj.Geography_ID = Modelobj.Geography_ID;

                    if (Modelobj.Language_master_id > 1)
                    {
                        ContentObj.parent_id = Modelobj.Language_subsection_id != 0 ? Modelobj.Language_subsection_id : Modelobj.Language_section_id;
                    }
                    else
                    {
                        ContentObj.parent_id = cont_parent_id;
                    }

                    ContentObj.lang_groupid = Modelobj.Lang_groupid;
                    ContentObj.root_parent_id = Modelobj.Section_id;
                    ContentObj.pagename = Modelobj.Pagename.Trim().Replace(" ", "-"); ;
                    ContentObj.title = Modelobj.Title.Trim();
                    ContentObj.hmpg_title = string.IsNullOrWhiteSpace(Modelobj.Hmpg_title) ? "" : Modelobj.Hmpg_title.Trim();
                    ContentObj.breadcrumb_title = string.IsNullOrWhiteSpace(Modelobj.Breadcrumb_title) ? "" : Modelobj.Breadcrumb_title.Trim();
                    ContentObj.window_title = Modelobj.Window_title;
                    ContentObj.sequence = Modelobj.Sequence;
                    ContentObj.hmpg_intro = Modelobj.Hmpg_intro;
                    ContentObj.intro = Modelobj.Intro;
                    ContentObj.content = Modelobj.Content;
                    ContentObj.metatag = Modelobj.Metatag;
                    ContentObj.metadesc = Modelobj.Metadesc;
                    ContentObj.page_schema = Modelobj.PageSchema;
                    ContentObj.metaexpiry = Modelobj.Metaexpiry;
                    ContentObj.external_url = string.IsNullOrWhiteSpace(Modelobj.External_url) ? "" : Modelobj.External_url.Trim();
                    ContentObj.IsExternal = Modelobj.IsExternal ? 1 : 0;
                    ContentObj.ByLine = string.IsNullOrWhiteSpace(Modelobj.ByLine) ? "" : Modelobj.ByLine.Trim();
                    ContentObj.Publication = string.IsNullOrWhiteSpace(Modelobj.Publication) ? "" : Modelobj.Publication.Trim();
                    ContentObj.isSearch = Modelobj.IsSearch ? 1 : 0;
                    ContentObj.top_icon = Modelobj.Display_top_icon ? 1 : 0;
                    ContentObj.Thumb_image_id = Modelobj.Thumb_image_id ?? null;
                    ContentObj.Thumb_image_alttext = string.IsNullOrWhiteSpace(Modelobj.Thumb_image_alttext) ? "" : Modelobj.Thumb_image_alttext.Trim();
                    ContentObj.Small_Icon_Thumb_image_id = Modelobj.Small_Icon_Thumb_image_id ?? null;
                    ContentObj.Small_Icon_alttext = string.IsNullOrWhiteSpace(Modelobj.Small_Icon_alttext) ? "" : Modelobj.Small_Icon_alttext.Trim();
                    ContentObj.Masthead_image_id = Modelobj.Masthead_image_id ?? null;
                    ContentObj.Mobile_Masthead_image_id = Modelobj.Mobile_Masthead_image_id ?? null;
                    ContentObj.Masthead_image_alttext = string.IsNullOrWhiteSpace(Modelobj.Masthead_image_alttext) ? "" : Modelobj.Masthead_image_alttext.Trim();
                    ContentObj.Background_image_id = Modelobj.Background_image_id ?? null;
                    ContentObj.Background_image_Alttext = Modelobj.Background_image_Alttext;
                    ContentObj.Attach_file_id = Modelobj.Attach_file_id ?? null;
                    if (Modelobj.SelectedMapping_sections != null && Modelobj.SelectedMapping_sections.Length > 0)
                    {
                        ContentObj.Mapped_sections = string.Join(",", Modelobj.SelectedMapping_sections);
                    }

                    if (Modelobj.SelectedTagging_sections != null && Modelobj.SelectedTagging_sections.Length > 0)
                    {
                        ContentObj.Tags = string.Join(",", Modelobj.SelectedTagging_sections);
                    }
                    if (Modelobj.Displaydate != null)
                    {
                        ContentObj.displaydate = Modelobj.Displaydate;
                    }
                    if (Command == "Save")
                    {
                        ContentObj.status = Content_Status.Draft;
                    }
                    else
                    {
                        ContentObj.status = Content_Status.Publish;
                    }
                    add_status = objBal.Update_content_BAL(ContentObj, Convert.ToInt32(User.GetUserId()));

                    if (add_status == 2)
                    {
                        if (Command == "Save")
                        {
                            TempData["alert_message"] = "Content with title " + ContentObj.title.Trim() + " is saved successfully";
                            return RedirectToAction("Drafts", "Content");
                        }
                        else
                        {
                            TempData["alert_message"] = "Content with title " + ContentObj.title.Trim() + " is saved and published successfully";
                            return RedirectToAction("Drafts", "Content");
                        }
                    }
                    else if (add_status == 1)
                    {
                        ModelState.AddModelError("", "Content with pagename " + ContentObj.pagename.Trim() + " is already exists");
                    }
                    else if (add_status == 0)
                    {
                        ModelState.AddModelError("", "Content with pagename " + ContentObj.pagename.Trim() + " is already exists with inactive status");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Something went wrong. Please try again.");
                        return RedirectToAction("Drafts", "Content");
                    }
                }
            }
            else
            {
                // ModelState.AddModelError("", "Please resolve below errors and submit again");

                var errors = ModelState.Values.SelectMany(v => v.Errors)
                      .Select(e => e.ErrorMessage)
                      .ToList();
                foreach (var err in errors)
                {
                    ModelState.AddModelError("", err);
                }
            }
        }
        catch (Exception ex)
        {
            FileLogger.LogError("Before pageload function: Post", ex);
            ModelState.AddModelError("", "Something went wrong. Please try again");
        }
        finally
        {
            objBal.Dispose();
        }
        try
        {
            pageload(Modelobj, 1);
        }
        catch (Exception ex)
        {
            FileLogger.LogError("After pageload function: Post", ex);
            ModelState.AddModelError("", "Something went wrong. Please try again");
        }
        return View(Modelobj);
    }


    private cms_Content Get_page_Details(int cont_id)
    {
        using ContentManager objBal = new(objconfig);
        cms_Content objModel = new();
        Content_Master objcontent = new();
        try
        {
            objcontent = objBal.Section_page_details_Get_BAL(cont_id);
            if (objcontent != null)
            {
                objModel.Id = objcontent.id;
                objModel.Language_master_id = objcontent.language_master_id;
                objModel.Content_Type_ID = objcontent.Content_Type_ID;
                objModel.Template_Master_ID = objcontent.Template_Master_ID;
                objModel.template_type = objcontent.Template_type;
                objModel.Template_name = objcontent.Template_name;
                objModel.Geography_ID = objcontent.Geography_ID;

                if (objcontent.language_master_id == 1)
                {

                    if (objcontent.root_parent_id == objcontent.id)
                    {
                        objModel.Section_id = 0;
                        objModel.Subsection_id = 0;
                    }
                    else
                    {
                        objModel.Section_id = objcontent.root_parent_id;
                        objModel.Subsection_id = objcontent.parent_id;
                    }
                    objModel.Language_section_id = 0;
                }
                else
                {
                    if (objcontent.Language_root_parent_id == objcontent.id)
                    {
                        objModel.Language_section_id = 0;
                        objModel.Language_subsection_id = 0;
                    }
                    else if (objcontent.Language_root_parent_id == objcontent.parent_id)
                    {
                        objModel.Language_section_id = objcontent.Language_root_parent_id;
                        objModel.Language_subsection_id = 0;
                    }
                    else
                    {
                        objModel.Language_section_id = objcontent.Language_root_parent_id;
                        objModel.Language_subsection_id = objcontent.Language_subsection_id;
                    }
                    if (objcontent.Content_Type_ID == 1)
                    {
                        objModel.Section_id = objcontent.root_parent_id;
                        if (objcontent.root_parent_id == objcontent.lang_groupid)
                        {
                            objModel.Subsection_id = 0;
                        }
                        else
                        {
                            objModel.Subsection_id = objcontent.lang_groupid;
                        }
                    }
                    else
                    {
                        objModel.article_id = objcontent.lang_groupid;
                    }
                }

                objModel.Title = objcontent.title;
                objModel.Pagename = objcontent.pagename;
                objModel.Hmpg_title = objcontent.hmpg_title;
                objModel.Breadcrumb_title = objcontent.breadcrumb_title;
                objModel.Window_title = objcontent.window_title;
                objModel.Sequence = objcontent.sequence;
                objModel.Displaydate = objcontent.displaydate;
                objModel.Search_url = objcontent.search_url;
                objModel.IsSearch = objcontent.isSearch == 1 ? true : false;
                objModel.Hmpg_intro = objcontent.hmpg_intro;
                objModel.Intro = objcontent.intro;
                objModel.Content = objcontent.content;
                objModel.Metatag = objcontent.metatag;
                objModel.Metadesc = objcontent.metadesc;
                objModel.PageSchema = objcontent.page_schema;
                objModel.Metaexpiry = objcontent.metaexpiry;
                objModel.External_url = objcontent.external_url;
                objModel.IsExternal = objcontent.IsExternal == 1 ? true : false;
                objModel.ByLine = objcontent.ByLine;
                objModel.Publication = objcontent.Publication;
                objModel.Display_top_icon = objcontent.top_icon == 1 ? true : false;
                objModel.Thumb_image_id = objcontent.Thumb_image_id;
                objModel.Thumb_image_alttext = objcontent.Thumb_image_alttext;
                objModel.Small_Icon_Thumb_image_id = objcontent.Small_Icon_Thumb_image_id;
                objModel.Small_Icon_alttext = objcontent.Small_Icon_alttext;
                objModel.Masthead_image_id = objcontent.Masthead_image_id;
                objModel.Mobile_Masthead_image_id = objcontent.Mobile_Masthead_image_id;
                objModel.Masthead_image_alttext = objcontent.Masthead_image_alttext;
                objModel.Background_image_id = objcontent.Background_image_id;
                objModel.Background_image_Alttext = objcontent.Background_image_Alttext;
                objModel.Attach_file_id = objcontent.Attach_file_id;
                objModel.Thumb_image = objcontent.Thumb_image;
                objModel.Small_Icon_Thumb_image = objcontent.Small_Icon_Thumb_image;
                objModel.Masthead_image = objcontent.Masthead_image;
                objModel.Mobile_Masthead_image = objcontent.Mobile_Masthead_image;
                objModel.Background_image = objcontent.Background_image;
                objModel.Attach_file = objcontent.Attach_file;
                if (!string.IsNullOrWhiteSpace(objcontent.Mapped_sections))
                {
                    objModel.SelectedMapping_sections = objcontent.Mapped_sections
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(int.Parse)
                        .ToArray();
                }

                if (!string.IsNullOrWhiteSpace(objcontent.Tags))
                {
                    objModel.SelectedTagging_sections = objcontent.Tags
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(int.Parse)
                        .ToArray();
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
        return objModel;
    }


}