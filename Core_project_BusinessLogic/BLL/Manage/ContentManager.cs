using System.Data;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic;

public class ContentManager : Content_DAL
{
    public ContentManager(IConfiguration configuration)
        : base(configuration)
    {
        // You can add languageMaster-specific logic here if needed

    }


    public int Add_content_BAL(Content_Master objcont, int userid, out int content_id)
    {
        DataTable dt = new DataTable();
        content_id = 0;
        int result = 0;
        try
        {
            dt = AddContent_DAL(objcont, userid);
            if (dt.Rows.Count > 0 && dt.Rows[0]["cont_id"] != DBNull.Value && dt.Rows[0]["cont_id"].ToString() != "")
            {
                content_id = Convert.ToInt32(dt.Rows[0]["cont_id"].ToString());
                result = Convert.ToInt32(dt.Rows[0]["result"].ToString());
            }
            if (!string.IsNullOrWhiteSpace(objcont.Spot_temp_id))
            {
                Context_details_Add_DAL(objcont.Spot_temp_id, content_id, objcont.status ?? 1, userid);
            }
            return result;
        }
        catch
        {
            throw;
        }
    }

    public int Update_content_BAL(Content_Master objcont, int userid)
    {
        DataTable dt = new();
        int content_id = 0;
        int result = 0;
        try
        {
            dt = UpdateContent_DAL(objcont, userid);
            if (dt.Rows.Count > 0 && dt.Rows[0]["cont_id"] != DBNull.Value && dt.Rows[0]["cont_id"].ToString() != "")
            {
                content_id = Convert.ToInt32(dt.Rows[0]["cont_id"].ToString());
                result = Convert.ToInt32(dt.Rows[0]["result"].ToString());
            }
            return result;
        }
        catch
        {
            throw;
        }
    }


    public CMS_pageload CMS_Pageload_Get_BAL(int language_id, int cont_id, int status = 2)
    {
        CMS_pageload obj = new();
        DataSet ds = new();
        try
        {
            ds = Pageload_CMS_Get_DAL(language_id, status);

            obj.Languages = [];
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    obj.Languages.Add(new Options_List { id = Convert.ToInt32(row["Id"].ToString()), title = row["Language_Name"].ToString() });
                }
            }

            if (language_id == 1)
            {
                obj.geographies = [new Options_List { id = 1, title = "Global" }];
            }
            else
            {
                obj.geographies = [new Options_List { id = 0, title = "Select" }];
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                    {
                        obj.geographies.Add(new Options_List { id = Convert.ToInt32(row["Id"].ToString()), title = row["country_Name"].ToString() });
                    }
                }
            }

            obj.Sections = [new Options_List { id = 0, title = "Select" }];
            if (ds.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[2].Rows)
                {
                    obj.Sections.Add(new Options_List { id = Convert.ToInt32(row["cont_id"].ToString()), title = row["cont_title"].ToString() });
                }
            }


            obj.Templates = [new Options_List { id = 0, title = "Select" }];
            if (ds.Tables[3].Rows.Count > 0)
            {
                foreach (DataRow row1 in ds.Tables[3].Rows)
                {
                    obj.Templates.Add(new Options_List { id = Convert.ToInt32(row1["Id"].ToString()), title = row1["template_name"].ToString() });
                }
            }
            if (language_id != 1)
            {
                obj.Language_sections = [new Options_List { id = 0, title = "Select" }];
                if (ds.Tables[4].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[4].Rows)
                    {
                        obj.Language_sections.Add(new Options_List { id = Convert.ToInt32(row["cont_id"].ToString()), title = row["cont_title"].ToString() });
                    }
                }
            }


            obj.Subsections = [new Options_List { id = 0, title = "Select", parent_id = 0 }];
            obj.sect_Articles = [new Options_List { id = 0, title = "Select" }];
            obj.Language_subSections = [new Options_List { id = 0, title = "Select", parent_id = 0 }];
            // if (cont_id != 0)
            // {
            ds = Subsections_Articles_Get_DAL(cont_id);
            //bind english subsections

            if (ds.Tables[0].Rows.Count > 0)
            {
                obj.Subsections.AddRange(
                   ds.Tables[0].AsEnumerable().Select(r => new Options_List
                   {
                       id = r.Field<int>("cont_id"),
                       title = r.Field<string>("cont_title"),
                       parent_id = r.Field<int?>("cont_parent_id")
                   })
               );
            }

            //bind english articles

            if (ds.Tables[1].Rows.Count > 0)
            {
                obj.sect_Articles.AddRange(
                   ds.Tables[1].AsEnumerable().Select(r => new Options_List
                   {
                       id = r.Field<int>("cont_id"),
                       title = r.Field<string>("cont_title"),
                       parent_id = r.Field<int?>("cont_parent_id")
                   })
               );
            }
            // }
            //bind language subsections
            /*
            if (language_id != 1)
            {
                ds = Language_Subsections_Get_DAL(cont_id, language_id);
                if (ds.Tables[0].Rows.Count > 0)
                {

                    obj.Language_subSections.AddRange(
                       ds.Tables[0].AsEnumerable().Select(r => new Options_List
                       {
                           id = r.Field<int>("cont_id"),
                           title = r.Field<string>("cont_title"),
                           parent_id = r.Field<int?>("cont_parent_id")
                       })
                   );
                }
            }*/

            return obj;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public SubSections_Articles Subsections_Articles_Get_BAL(int cont_id)
    {
        SubSections_Articles obj_section_list = new();
        DataSet ds = new();
        try
        {
            obj_section_list.Subsections = [new Options_List { id = 0, title = "Select", parent_id = 0 }];
            obj_section_list.sect_Articles = [new Options_List { id = 0, title = "Select", parent_id = 0 }];
            ds = Subsections_Articles_Get_DAL(cont_id);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    obj_section_list.Subsections.Add(new Options_List { id = Convert.ToInt32(dr["cont_id"].ToString()), parent_id = Convert.ToInt32(dr["cont_parent_id"].ToString()), title = dr["cont_title"].ToString() });
                }
            }

            if (ds.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    obj_section_list.sect_Articles.Add(new Options_List { id = Convert.ToInt32(dr["cont_id"].ToString()), title = dr["cont_title"].ToString() });
                }
            }

            return obj_section_list;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public List<Options_List> Language_Sections_Get_BAL(int language_id)
    {
        List<Options_List> objsections = [new Options_List { id = 0, title = "Select", parent_id = 0 }];
        DataSet ds = new();
        try
        {
            ds = Language_Sections_Get_DAL(language_id);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    objsections.Add(new Options_List { id = Convert.ToInt32(dr["cont_id"].ToString()), parent_id = Convert.ToInt32(dr["cont_parent_id"].ToString()), title = dr["cont_title"].ToString() });
                }
            }

            return objsections;
        }
        catch (System.Exception)
        {
            throw;
        }
    }
    public List<Options_List> Language_Subsections_Get_BAL(int cont_id, int language_id)
    {
        List<Options_List> objsections = [new Options_List { id = 0, title = "Select", parent_id = 0 }]; ;
        DataSet ds = new();
        try
        {
            ds = Language_Subsections_Get_DAL(cont_id, language_id);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    objsections.Add(new Options_List { id = Convert.ToInt32(dr["cont_id"].ToString()), parent_id = Convert.ToInt32(dr["cont_parent_id"].ToString()), title = dr["cont_title"].ToString() });
                }
            }

            return objsections;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Content_Master Section_page_details_Get_BAL(int cont_id)
    {
        Content_Master objcontent = new();
        DataSet ds = new();
        try
        {
            ds = Section_page_details_Get_DAL(cont_id);
            if (ds.Tables[0].Rows.Count > 0)
            {
                objcontent.id = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_id"].ToString());
                objcontent.language_master_id = Convert.ToInt32(ds.Tables[0].Rows[0]["language_master_id"].ToString());
                objcontent.Content_Type_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["Content_Type_ID"].ToString());
                objcontent.Template_Master_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["Template_Master_ID"] == DBNull.Value ? 0 : ds.Tables[0].Rows[0]["Template_Master_ID"].ToString());
                objcontent.Template_name = ds.Tables[0].Rows[0]["template_name"].ToString() ?? "";
                objcontent.Template_type = Convert.ToInt32(ds.Tables[0].Rows[0]["Template_Type_ID"] == DBNull.Value ? 1 : ds.Tables[0].Rows[0]["Template_Type_ID"]);
                objcontent.Geography_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["Geography_ID"] == DBNull.Value ? 1 : ds.Tables[0].Rows[0]["Geography_ID"]);
                objcontent.root_parent_id = Convert.ToInt32(ds.Tables[0].Rows[0]["Root_parent_Id"].ToString());
                objcontent.lang_groupid = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_lang_groupid"].ToString());
                if (objcontent.language_master_id == 1)
                {
                    // if (objcontent.root_parent_id == Convert.ToInt32(ds.Tables[0].Rows[0]["cont_parent_id"].ToString()))
                    // {
                    //     objcontent.parent_id = 0;
                    // }
                    // else
                    // {
                    objcontent.parent_id = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_parent_id"].ToString());
                    //}
                }
                else
                {


                    if (objcontent.root_parent_id == objcontent.lang_groupid)
                    {
                        objcontent.parent_id = 0;
                        objcontent.article_id = 0;
                    }
                    else if (objcontent.Content_Type_ID == Content_Types.Section)
                    {
                        objcontent.parent_id = objcontent.lang_groupid;
                        objcontent.article_id = 0;
                    }
                    else
                    {
                        objcontent.parent_id = Convert.ToInt32(ds.Tables[0].Rows[0]["group_id_parent_id"].ToString());
                        objcontent.article_id = objcontent.lang_groupid;
                    }
                    objcontent.Language_root_parent_id = Convert.ToInt32(ds.Tables[0].Rows[0]["Language_root_parent_id"].ToString());
                    objcontent.Language_subsection_id = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_parent_id"].ToString());
                }

                objcontent.title = ds.Tables[0].Rows[0]["cont_title"].ToString() ?? "";
                objcontent.pagename = ds.Tables[0].Rows[0]["cont_pagename"].ToString() ?? "";
                objcontent.hmpg_title = ds.Tables[0].Rows[0]["cont_hmpg_title"].ToString() ?? "";
                objcontent.breadcrumb_title = ds.Tables[0].Rows[0]["cont_breadcrumb_title"].ToString() ?? "";
                objcontent.window_title = ds.Tables[0].Rows[0]["cont_window_title"].ToString() ?? "";
                objcontent.sequence = ds.Tables[0].Rows[0]["cont_sequence"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["cont_sequence"].ToString());
                objcontent.displaydate = ds.Tables[0].Rows[0]["cont_displaydate"] == DBNull.Value ? null : Convert.ToDateTime(ds.Tables[0].Rows[0]["cont_displaydate"].ToString());
                objcontent.search_url = ds.Tables[0].Rows[0]["cont_search_url"].ToString() ?? "";
                objcontent.isSearch = Convert.ToInt32((bool)ds.Tables[0].Rows[0]["isSearch"] ? 1 : 0);
                objcontent.hmpg_intro = ds.Tables[0].Rows[0]["cont_hmpg_intro"].ToString() ?? "";
                objcontent.intro = ds.Tables[0].Rows[0]["cont_intro"].ToString() ?? "";
                objcontent.content = ds.Tables[0].Rows[0]["content"].ToString() ?? "";
                objcontent.metatag = ds.Tables[0].Rows[0]["cont_metatag"].ToString() ?? "";
                objcontent.metadesc = ds.Tables[0].Rows[0]["cont_metadesc"].ToString() ?? "";
                objcontent.page_schema = ds.Tables[0].Rows[0]["page_schema"].ToString() ?? "";
                objcontent.metaexpiry = ds.Tables[0].Rows[0]["cont_metaexpiry"] == DBNull.Value ? null : Convert.ToDateTime(ds.Tables[0].Rows[0]["cont_metaexpiry"].ToString());
                objcontent.external_url = ds.Tables[0].Rows[0]["cont_external_url"].ToString() ?? "";
                objcontent.IsExternal = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_IsExternal"].ToString());
                objcontent.ByLine = ds.Tables[0].Rows[0]["cont_ByLine"].ToString() ?? "";
                objcontent.Publication = ds.Tables[0].Rows[0]["Cont_Publication"].ToString() ?? "";
                objcontent.top_icon = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_top_icon"].ToString());
                objcontent.Thumb_image_id = ds.Tables[0].Rows[0]["Hmpg_thumbnail_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Hmpg_thumbnail_Media_id"].ToString());
                objcontent.Thumb_image_alttext = ds.Tables[0].Rows[0]["Hmpg_thumbnail_alt_text"].ToString() ?? "";
                objcontent.Small_Icon_Thumb_image_id = ds.Tables[0].Rows[0]["Small_icon_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Small_icon_Media_id"].ToString());
                objcontent.Small_Icon_alttext = ds.Tables[0].Rows[0]["Small_icon_alt_text"].ToString() ?? "";
                objcontent.Masthead_image_id = ds.Tables[0].Rows[0]["Masthead_image_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Masthead_image_Media_id"].ToString());
                objcontent.Mobile_Masthead_image_id = ds.Tables[0].Rows[0]["Mobile_Masthead_image_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Mobile_Masthead_image_Media_id"].ToString());
                objcontent.Masthead_image_alttext = ds.Tables[0].Rows[0]["Masthead_alt_text"].ToString() ?? "";
                objcontent.Background_image_id = ds.Tables[0].Rows[0]["Background_image_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Background_image_Media_id"].ToString());
                objcontent.Background_image_Alttext = ds.Tables[0].Rows[0]["Background_alt_text"].ToString() ?? "";
                objcontent.Attach_file_id = ds.Tables[0].Rows[0]["Attach_file_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Attach_file_Media_id"].ToString());

            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                objcontent.Thumb_image = ds.Tables[1].Rows[0]["Hmpg_thumbnail_Media"].ToString() ?? "";
                objcontent.Small_Icon_Thumb_image = ds.Tables[1].Rows[0]["Small_icon_Media"].ToString() ?? "";
                objcontent.Masthead_image = ds.Tables[1].Rows[0]["Masthead_image_Media"].ToString() ?? "";
                objcontent.Mobile_Masthead_image = ds.Tables[1].Rows[0]["Mobile_Masthead_image_Media"].ToString() ?? "";
                objcontent.Background_image = ds.Tables[1].Rows[0]["Background_image_Media"].ToString() ?? "";
                objcontent.Attach_file = ds.Tables[1].Rows[0]["Attach_file_Media"].ToString() ?? "";
            }
            else
            {
                objcontent.Thumb_image = "";
                objcontent.Small_Icon_Thumb_image = "";
                objcontent.Masthead_image = "";
                objcontent.Mobile_Masthead_image = "";
                objcontent.Background_image = "";
                objcontent.Attach_file = "";
            }
            return objcontent;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Manage_List_CMS_page CMS_Section_articles_List(int cont_id, string search, int language_id, int current_page)
    {
        DataSet ds = new();
        Manage_List_CMS_page obj = new();
        try
        {
            ds = List_Sections_Articles_DAL(cont_id, search, language_id, current_page);
            if (ds.Tables[0].Rows.Count > 0)
            {
                obj.sections = new();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    obj.sections.Add(
                        new CMS_Page_detail
                        {
                            Id = Convert.ToInt32(dr["cont_id"]),
                            Title = dr["cont_title"].ToString(),
                            Parent_title = dr["parent_title"].ToString(),
                            Language = dr["Language_Name"].ToString(),
                            Created_date = Convert.ToDateTime(dr["cont_createdate"].ToString()),
                            Updated_date = dr["cont_updatedt"] == DBNull.Value ? null : Convert.ToDateTime(dr["cont_updatedt"].ToString()),
                            Status = Convert.ToInt32(dr["cont_status"]),
                        });
                }
                obj.Sections_no_of_pages = Convert.ToInt32(ds.Tables[1].Rows[0]["Section_page_count"].ToString());
            }
            else
            {
                obj.sections = null;
                obj.Sections_no_of_pages = 0;
            }

            if (ds.Tables[2].Rows.Count > 0)
            {
                obj.articles = new();
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    obj.articles.Add(
                        new CMS_Page_detail
                        {
                            Id = Convert.ToInt32(dr["cont_id"]),
                            Title = dr["cont_title"].ToString(),
                            Parent_title = dr["parent_title"].ToString(),
                            Language = dr["Language_Name"].ToString(),
                            Created_date = Convert.ToDateTime(dr["cont_createdate"].ToString()),
                            Updated_date = dr["cont_updatedt"] == DBNull.Value ? null : Convert.ToDateTime(dr["cont_updatedt"].ToString()),
                            Status = Convert.ToInt32(dr["cont_status"]),
                        });
                }
                obj.Articles_no_of_pages = Convert.ToInt32(ds.Tables[3].Rows[0]["Article_page_count"].ToString());
            }
            else
            {
                obj.articles = null;
                obj.Articles_no_of_pages = 0;
            }
            return obj;
        }
        catch (System.Exception)
        {
            throw;
        }
    }


    public Manage_List_CMS_page CMS_Published_List(int cont_id, string search, int language_id, int current_page, int Content_Type_ID = 1, int pagesize = 25)
    {
        DataSet ds = new();
        Manage_List_CMS_page obj = new();
        try
        {
            ds = List_Published_DAL(cont_id, search, language_id, current_page, Content_Type_ID, pagesize);
            if (ds.Tables[0].Rows.Count > 0)
            {
                obj.sections = new();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    obj.sections.Add(
                        new CMS_Page_detail
                        {
                            Id = Convert.ToInt32(dr["cont_id"]),
                            Title = dr["cont_title"].ToString(),
                            Parent_title = dr["parent_title"].ToString(),
                            Language = dr["Language_Name"].ToString(),
                            Created_date = Convert.ToDateTime(dr["cont_createdate"].ToString()),
                            Updated_date = dr["cont_updatedt"] == DBNull.Value ? null : Convert.ToDateTime(dr["cont_updatedt"].ToString()),
                            Status = Convert.ToInt32(dr["cont_status"]),
                            IsReprocessed = dr["reprocess_Id"] == DBNull.Value ? 0 : 1
                        });
                }
                obj.Sections_no_of_pages = Convert.ToInt32(ds.Tables[1].Rows[0]["Section_page_count"].ToString());
            }

            return obj;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public void Update_Content_Status_BAL(int cont_id, int status, int userid)
    {
        try
        {
            Update_Content_Status_DAL(cont_id, status, userid);
        }
        catch
        { throw; }
    }

    public int AddContentReprocess_BAL(Content_Master objcont, int userid)
    {
        DataTable dt = new();
        int content_id = 0;
        int result = 0;
        try
        {
            dt = AddContentReprocess_DAL(objcont, userid);
            if (dt.Rows.Count > 0 && dt.Rows[0]["cont_id"] != DBNull.Value && dt.Rows[0]["cont_id"].ToString() != "")
            {
                content_id = Convert.ToInt32(dt.Rows[0]["cont_id"].ToString());
                result = Convert.ToInt32(dt.Rows[0]["result"].ToString());
            }
            return result;
        }
        catch
        {
            throw;
        }
    }

    public Manage_List_CMS_page CMS_Section_articles_RepublishedList(int cont_id, string search, int language_id, int current_page)
    {
        DataSet ds = new();
        Manage_List_CMS_page obj = new();
        try
        {
            ds = List_Sections_Articles_Republished_DAL(cont_id, search, language_id, current_page);
            if (ds.Tables[0].Rows.Count > 0)
            {
                obj.sections = new();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    obj.sections.Add(
                        new CMS_Page_detail
                        {
                            Reprocess_Id = Convert.ToInt32(dr["id"]),
                            Id = Convert.ToInt32(dr["cont_id"]),
                            Title = dr["cont_title"].ToString(),
                            Parent_title = dr["parent_title"].ToString(),
                            Language = dr["Language_Name"].ToString(),
                            Created_date = Convert.ToDateTime(dr["cont_createdate"].ToString()),
                            Updated_date = dr["cont_updatedt"] == DBNull.Value ? null : Convert.ToDateTime(dr["cont_updatedt"].ToString()),
                        });
                }
                obj.Sections_no_of_pages = Convert.ToInt32(ds.Tables[1].Rows[0]["Section_page_count"].ToString());
            }
            else
            {
                obj.sections = null;
                obj.Sections_no_of_pages = 0;
            }

            if (ds.Tables[2].Rows.Count > 0)
            {
                obj.articles = new();
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    obj.articles.Add(
                        new CMS_Page_detail
                        {
                            Reprocess_Id = Convert.ToInt32(dr["id"]),
                            Id = Convert.ToInt32(dr["cont_id"]),
                            Title = dr["cont_title"].ToString(),
                            Parent_title = dr["parent_title"].ToString(),
                            Language = dr["Language_Name"].ToString(),
                            Created_date = Convert.ToDateTime(dr["cont_createdate"].ToString()),
                            Updated_date = dr["cont_updatedt"] == DBNull.Value ? null : Convert.ToDateTime(dr["cont_updatedt"].ToString()),
                        });
                }
                obj.Articles_no_of_pages = Convert.ToInt32(ds.Tables[3].Rows[0]["Article_page_count"].ToString());
            }
            else
            {
                obj.articles = null;
                obj.Articles_no_of_pages = 0;
            }
            return obj;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public Content_Master Reprocessed_Section_page_details_Get_BAL(int id)
    {
        Content_Master objcontent = new();
        DataSet ds = new();
        try
        {
            ds = Reprocessed_Section_page_details_Get_DAL(id);
            if (ds.Tables[0].Rows.Count > 0)
            {
                objcontent.reprocess_id = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString());
                objcontent.id = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_id"].ToString());
                objcontent.language_master_id = Convert.ToInt32(ds.Tables[0].Rows[0]["language_master_id"].ToString());
                objcontent.Content_Type_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["Content_Type_ID"].ToString());
                objcontent.Template_Master_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["Template_Master_ID"] == DBNull.Value ? 0 : ds.Tables[0].Rows[0]["Template_Master_ID"].ToString());
                objcontent.Template_name = ds.Tables[0].Rows[0]["template_name"].ToString() ?? "";
                objcontent.Template_type = Convert.ToInt32(ds.Tables[0].Rows[0]["Template_Type_ID"] == DBNull.Value ? 1 : ds.Tables[0].Rows[0]["Template_Type_ID"]);
                objcontent.Geography_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["Geography_ID"] == DBNull.Value ? 1 : ds.Tables[0].Rows[0]["Geography_ID"]);
                objcontent.root_parent_id = Convert.ToInt32(ds.Tables[0].Rows[0]["Root_parent_Id"].ToString());
                objcontent.lang_groupid = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_lang_groupid"].ToString());
                if (objcontent.language_master_id == 1)
                {
                    // if (objcontent.root_parent_id == Convert.ToInt32(ds.Tables[0].Rows[0]["cont_parent_id"].ToString()))
                    // {
                    //     objcontent.parent_id = 0;
                    // }
                    // else
                    // {
                    objcontent.parent_id = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_parent_id"].ToString());
                    //}
                }
                else
                {


                    if (objcontent.root_parent_id == objcontent.lang_groupid)
                    {
                        objcontent.parent_id = 0;
                        objcontent.article_id = 0;
                    }
                    else if (objcontent.Content_Type_ID == Content_Types.Section)
                    {
                        objcontent.parent_id = objcontent.lang_groupid;
                        objcontent.article_id = 0;
                    }
                    else
                    {
                        objcontent.parent_id = Convert.ToInt32(ds.Tables[0].Rows[0]["group_id_parent_id"].ToString());
                        objcontent.article_id = objcontent.lang_groupid;
                    }
                    objcontent.Language_root_parent_id = Convert.ToInt32(ds.Tables[0].Rows[0]["Language_root_parent_id"].ToString());
                    objcontent.Language_subsection_id = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_parent_id"].ToString());
                }

                objcontent.title = ds.Tables[0].Rows[0]["cont_title"].ToString() ?? "";
                objcontent.pagename = ds.Tables[0].Rows[0]["cont_pagename"].ToString() ?? "";
                objcontent.hmpg_title = ds.Tables[0].Rows[0]["cont_hmpg_title"].ToString() ?? "";
                objcontent.breadcrumb_title = ds.Tables[0].Rows[0]["cont_breadcrumb_title"].ToString() ?? "";
                objcontent.window_title = ds.Tables[0].Rows[0]["cont_window_title"].ToString() ?? "";
                objcontent.sequence = ds.Tables[0].Rows[0]["cont_sequence"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["cont_sequence"].ToString());
                objcontent.displaydate = ds.Tables[0].Rows[0]["cont_displaydate"] == DBNull.Value ? null : Convert.ToDateTime(ds.Tables[0].Rows[0]["cont_displaydate"].ToString());
                objcontent.search_url = ds.Tables[0].Rows[0]["cont_search_url"].ToString() ?? "";
                objcontent.isSearch = Convert.ToInt32((bool)ds.Tables[0].Rows[0]["isSearch"] ? 1 : 0);
                objcontent.hmpg_intro = ds.Tables[0].Rows[0]["cont_hmpg_intro"].ToString() ?? "";
                objcontent.intro = ds.Tables[0].Rows[0]["cont_intro"].ToString() ?? "";
                objcontent.content = ds.Tables[0].Rows[0]["content"].ToString() ?? "";
                objcontent.metatag = ds.Tables[0].Rows[0]["cont_metatag"].ToString() ?? "";
                objcontent.metadesc = ds.Tables[0].Rows[0]["cont_metadesc"].ToString() ?? "";
                objcontent.page_schema = ds.Tables[0].Rows[0]["page_schema"].ToString() ?? "";
                objcontent.metaexpiry = ds.Tables[0].Rows[0]["cont_metaexpiry"] == DBNull.Value ? null : Convert.ToDateTime(ds.Tables[0].Rows[0]["cont_metaexpiry"].ToString());
                objcontent.external_url = ds.Tables[0].Rows[0]["cont_external_url"].ToString() ?? "";
                objcontent.IsExternal = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_IsExternal"].ToString());
                objcontent.ByLine = ds.Tables[0].Rows[0]["cont_ByLine"].ToString() ?? "";
                objcontent.Publication = ds.Tables[0].Rows[0]["Cont_Publication"].ToString() ?? "";
                objcontent.top_icon = Convert.ToInt32(ds.Tables[0].Rows[0]["cont_top_icon"].ToString());
                objcontent.Thumb_image_id = ds.Tables[0].Rows[0]["Hmpg_thumbnail_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Hmpg_thumbnail_Media_id"].ToString());
                objcontent.Thumb_image_alttext = ds.Tables[0].Rows[0]["Hmpg_thumbnail_alt_text"].ToString() ?? "";
                objcontent.Small_Icon_Thumb_image_id = ds.Tables[0].Rows[0]["Small_icon_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Small_icon_Media_id"].ToString());
                objcontent.Small_Icon_alttext = ds.Tables[0].Rows[0]["Small_icon_alt_text"].ToString() ?? "";
                objcontent.Masthead_image_id = ds.Tables[0].Rows[0]["Masthead_image_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Masthead_image_Media_id"].ToString());
                objcontent.Mobile_Masthead_image_id = ds.Tables[0].Rows[0]["Mobile_Masthead_image_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Mobile_Masthead_image_Media_id"].ToString());
                objcontent.Masthead_image_alttext = ds.Tables[0].Rows[0]["Masthead_alt_text"].ToString() ?? "";
                objcontent.Background_image_id = ds.Tables[0].Rows[0]["Background_image_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Background_image_Media_id"].ToString());
                objcontent.Background_image_Alttext = ds.Tables[0].Rows[0]["Background_alt_text"].ToString() ?? "";
                objcontent.Attach_file_id = ds.Tables[0].Rows[0]["Attach_file_Media_id"] == DBNull.Value ? 0 : Convert.ToInt32(ds.Tables[0].Rows[0]["Attach_file_Media_id"].ToString());

            }
            if (ds.Tables[1].Rows.Count > 0)
            {
                objcontent.Thumb_image = ds.Tables[1].Rows[0]["Hmpg_thumbnail_Media"].ToString() ?? "";
                objcontent.Small_Icon_Thumb_image = ds.Tables[1].Rows[0]["Small_icon_Media"].ToString() ?? "";
                objcontent.Masthead_image = ds.Tables[1].Rows[0]["Masthead_image_Media"].ToString() ?? "";
                objcontent.Mobile_Masthead_image = ds.Tables[1].Rows[0]["Mobile_Masthead_image_Media"].ToString() ?? "";
                objcontent.Background_image = ds.Tables[1].Rows[0]["Background_image_Media"].ToString() ?? "";
                objcontent.Attach_file = ds.Tables[1].Rows[0]["Attach_file_Media"].ToString() ?? "";
            }
            else
            {
                objcontent.Thumb_image = "";
                objcontent.Small_Icon_Thumb_image = "";
                objcontent.Masthead_image = "";
                objcontent.Mobile_Masthead_image = "";
                objcontent.Background_image = "";
                objcontent.Attach_file = "";
            }
            return objcontent;
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public int Publish_Reprocessed_Content_BAL(Content_Master objcont, int userid)
    {
        DataTable dt = new();
        int content_id = 0;
        int result = 0;
        try
        {
            dt = Publish_Reprocessed_Content(objcont, userid);
            if (dt.Rows.Count > 0 && dt.Rows[0]["cont_id"] != DBNull.Value && dt.Rows[0]["cont_id"].ToString() != "")
            {
                content_id = Convert.ToInt32(dt.Rows[0]["cont_id"].ToString());
                result = Convert.ToInt32(dt.Rows[0]["result"].ToString());
            }
            return result;
        }
        catch
        {
            throw;
        }
    }

    public int RePublish_Reprocessed_Content_BAL(int reprocess_Id, int userid, out string title, out string pagename)
    {
        DataTable dt = new();
        int content_id = 0;
        int result = 0;
        title = "";
        pagename = "";
        try
        {
            dt = RePublish_Reprocessed_Content(reprocess_Id, userid);
            if (dt.Rows.Count > 0 && dt.Rows[0]["cont_id"] != DBNull.Value && dt.Rows[0]["cont_id"].ToString() != "")
            {
                content_id = Convert.ToInt32(dt.Rows[0]["cont_id"].ToString());
                result = Convert.ToInt32(dt.Rows[0]["result"].ToString());
                title = Convert.ToString(dt.Rows[0]["cont_title"].ToString() ?? "");
                pagename = dt.Rows[0]["result"].ToString() ?? "";
            }
            return result;
        }
        catch
        {
            throw;
        }
    }

    public void Delete_Reprocessed_Content_BAL(int reprocess_Id)
    {
        try
        {
            Delete_Reprocessed_Content(reprocess_Id);
        }
        catch
        {
            throw;
        }
    }
}



