
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Core_project_BusinessLogic.Entity.Manage;

namespace Core_project_BusinessLogic;

public class Content_DAL : DBHelper
{

    public Content_DAL(IConfiguration configuration)
: base(configuration) //  call base class constructor 
    {
    }


    protected DataTable AddContent_DAL(Content_Master content, int userid)
    {
        DataTable dt = new();
        var Sqlparam = new List<SqlParameter>();

        try
        {
            Sqlparam.Add(new SqlParameter("@title", content.title.Trim()));
            Sqlparam.Add(new SqlParameter("@parent_id", content.parent_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Template_Master_ID", content.Template_Master_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Content_Type_ID", content.Content_Type_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Template_Type_ID", content.Template_type ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@hmpg_title", content.hmpg_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@intro", content.intro ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@hmpg_intro", content.hmpg_intro ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@content", content.content ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@pagename", content.pagename.Trim()));
            if (content.displaydate.HasValue)
            {
                Sqlparam.Add(new SqlParameter("@displaydate", content.displaydate.Value));
            }
            else
            {
                Sqlparam.Add(new SqlParameter("@displaydate", DBNull.Value));
            }

            Sqlparam.Add(new SqlParameter("@window_title", content.window_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@sequence", content.sequence ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@external_url", content.external_url ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@IsExternal", content.IsExternal ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@metatag", content.metatag ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@metadesc", content.metadesc ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@page_schema", content.page_schema ?? (object)DBNull.Value));
            if (content.metaexpiry.HasValue)
            {
                Sqlparam.Add(new SqlParameter("@metaexpiry", content.metaexpiry.Value));
            }
            else
            {
                Sqlparam.Add(new SqlParameter("@metaexpiry", DBNull.Value));
            }

            Sqlparam.Add(new SqlParameter("@status", content.status ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@language_master_id", content.language_master_id ?? 1));
            Sqlparam.Add(new SqlParameter("@lang_groupid", content.lang_groupid ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Geography_ID", content.Geography_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@ByLine", content.ByLine ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Publication", content.Publication ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@top_icon", content.top_icon ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@isSearch", content.isSearch ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@search_url", content.search_url ?? (object)DBNull.Value));
            // Sqlparam.Add(new SqlParameter("@page_url", content.page_url ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@breadcrumb_title", content.breadcrumb_title ?? (object)DBNull.Value));

            Sqlparam.Add(new SqlParameter("@Hmpg_thumbnail_Media_id", content.Thumb_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Hmpg_thumbnail_alt_text", content.Thumb_image_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Small_icon_Media_id", content.Small_Icon_Thumb_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Small_icon_alt_text", content.Small_Icon_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Masthead_image_Media_id", content.Masthead_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Mobile_Masthead_image_Media_id", content.Mobile_Masthead_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Masthead_alt_text", content.Masthead_image_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Background_image_Media_id", content.Background_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Background_alt_text", content.Background_image_Alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Attach_file_Media_id", content.Attach_file_id ?? (object)DBNull.Value));
            // Sqlparam.Add(new SqlParameter("@Spot_temp_id", content.Spot_temp_id ?? (object)DBNull.Value));            
            Sqlparam.Add(new SqlParameter("@userid", userid));


            dt = GetDataSet("Content_Insert", Sqlparam.ToArray()).Tables[0];
            return dt;
        }
        catch
        {
            throw;
        }
    }

    protected void Context_details_Add_DAL(string temp_cont_id, int cont_id, int status, int userid)
    {
        SQLInsert_Update_Delete_Data("Context_details_Add", "@temp_cont_id", temp_cont_id, "@cont_id", cont_id.ToString(), "@status", status.ToString(), "@userid", userid.ToString());
    }

    protected DataSet Pageload_CMS_Get_DAL(int language_id, int status = 2)
    {
        DataSet ds = new();
        try
        {
            ds = GetDataSet("Pageload_CMS_Get", "@language_id", language_id.ToString(), "@status", status.ToString());
            return ds;
        }
        catch
        { throw; }
    }

    protected DataSet Subsections_Articles_Get_DAL(int cont_id)
    {
        DataSet ds = new();
        try
        {
            ds = GetDataSet("Subsections_Articles_CMS_Get", "@cont_id", cont_id.ToString());
            return ds;
        }
        catch
        {
            throw;
        }
    }

    protected DataSet Language_Sections_Get_DAL(int language_id)
    {
        DataSet ds = new();
        try
        {
            ds = GetDataSet("Sections_Language_CMS_Get", "@language_id", language_id.ToString());
            return ds;
        }
        catch
        {
            throw;
        }
    }

    protected DataSet Language_Subsections_Get_DAL(int cont_id, int language_id)
    {
        DataSet ds = new();
        try
        {
            ds = GetDataSet("Subsections_Language_CMS_Get", "@cont_id", cont_id.ToString(), "@language_id", language_id.ToString());
            return ds;
        }
        catch
        {
            throw;
        }
    }

    protected DataSet Section_page_details_Get_DAL(int cont_id)
    {
        DataSet ds = new();
        try
        {
            ds = GetDataSet("Section_page_details_Get", "@cont_id", cont_id.ToString());
            return ds;
        }
        catch
        {
            throw;
        }
    }

    protected DataSet List_Sections_Articles_DAL(int cont_id, string search, int language_id, int current_page, int pagesize = 500)
    {
        DataSet ds = new();
        try
        {
            ds = GetDataSet("CMS_Section_articles_List", "@cont_id", cont_id.ToString(), "@language_id", language_id.ToString(), "@searchquery", search, "@current_page", current_page.ToString(), "@PageSize", pagesize.ToString());
            return ds;
        }
        catch
        {
            throw;
        }
    }

    protected DataTable UpdateContent_DAL(Content_Master content, int userid)
    {
        DataTable dt = new();
        var Sqlparam = new List<SqlParameter>();

        try
        {
            Sqlparam.Add(new SqlParameter("@cont_Id", content.id));
            Sqlparam.Add(new SqlParameter("@title", content.title.Trim()));
            Sqlparam.Add(new SqlParameter("@parent_id", content.parent_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Template_Master_ID", content.Template_Master_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Content_Type_ID", content.Content_Type_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Template_Type_ID", content.Template_type ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@hmpg_title", content.hmpg_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@intro", content.intro ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@hmpg_intro", content.hmpg_intro ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@content", content.content ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@pagename", content.pagename.Trim()));
            if (content.displaydate.HasValue)
            {
                Sqlparam.Add(new SqlParameter("@displaydate", content.displaydate.Value));
            }
            else
            {
                Sqlparam.Add(new SqlParameter("@displaydate", DBNull.Value));
            }

            Sqlparam.Add(new SqlParameter("@window_title", content.window_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@sequence", content.sequence ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@external_url", content.external_url ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@IsExternal", content.IsExternal ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@metatag", content.metatag ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@metadesc", content.metadesc ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@page_schema", content.page_schema ?? (object)DBNull.Value));
            if (content.metaexpiry.HasValue)
            {
                Sqlparam.Add(new SqlParameter("@metaexpiry", content.metaexpiry.Value));
            }
            else
            {
                Sqlparam.Add(new SqlParameter("@metaexpiry", DBNull.Value));
            }

            Sqlparam.Add(new SqlParameter("@status", content.status ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@language_master_id", content.language_master_id ?? 1));
            Sqlparam.Add(new SqlParameter("@lang_groupid", content.lang_groupid ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Geography_ID", content.Geography_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@ByLine", content.ByLine ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Publication", content.Publication ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@top_icon", content.top_icon ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@isSearch", content.isSearch ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@search_url", content.search_url ?? (object)DBNull.Value));
            // Sqlparam.Add(new SqlParameter("@page_url", content.page_url ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@breadcrumb_title", content.breadcrumb_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@business_id", content.business_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Hmpg_thumbnail_Media_id", content.Thumb_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Hmpg_thumbnail_alt_text", content.Thumb_image_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Small_icon_Media_id", content.Small_Icon_Thumb_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Small_icon_alt_text", content.Small_Icon_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Masthead_image_Media_id", content.Masthead_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Mobile_Masthead_image_Media_id", content.Mobile_Masthead_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Masthead_alt_text", content.Masthead_image_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Background_image_Media_id", content.Background_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Background_alt_text", content.Background_image_Alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Attach_file_Media_id", content.Attach_file_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@userid", userid));


            dt = GetDataSet("Content_Update", Sqlparam.ToArray()).Tables[0];
            return dt;
        }
        catch
        {
            throw;
        }
    }

    protected void Update_Content_Status_DAL(int cont_id, int status, int userid)
    {
        try
        {
        SQLInsert_Update_Delete_Data("Update_content_status", "@cont_id", cont_id.ToString(),"@status", status.ToString(), "@userid", userid.ToString());
        }
        catch
         {throw;}
    }

 

    protected DataSet List_Published_DAL(int cont_id, string search, int language_id, int current_page, int Content_Type_ID = 1, int pagesize = 25)
    {
        DataSet ds = new();
        var Sqlparam = new List<SqlParameter>();
        try
        {
            Sqlparam.Add(new SqlParameter("@cont_id", cont_id.ToString()));
            Sqlparam.Add(new SqlParameter("@language_id", language_id.ToString()));
            Sqlparam.Add(new SqlParameter("@Content_Type_ID", Content_Type_ID.ToString()));
            Sqlparam.Add(new SqlParameter("@searchquery", search));
            Sqlparam.Add(new SqlParameter("@current_page", current_page.ToString()));
            Sqlparam.Add(new SqlParameter("@PageSize", pagesize.ToString()));
            ds = GetDataSet("CMS_Published_List", Sqlparam.ToArray());
            return ds;
        }
        catch
        {
            throw;
        }
    }


    protected DataTable AddContentReprocess_DAL(Content_Master content, int userid)
    {
        DataTable dt = new();
        var Sqlparam = new List<SqlParameter>();

        try
        {
            Sqlparam.Add(new SqlParameter("@cont_Id", content.id));
            Sqlparam.Add(new SqlParameter("@title", content.title.Trim()));
            Sqlparam.Add(new SqlParameter("@parent_id", content.parent_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Template_Master_ID", content.Template_Master_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Content_Type_ID", content.Content_Type_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Template_Type_ID", content.Template_type ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@hmpg_title", content.hmpg_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@intro", content.intro ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@hmpg_intro", content.hmpg_intro ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@content", content.content ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@pagename", content.pagename.Trim()));
            if (content.displaydate.HasValue)
            {
                Sqlparam.Add(new SqlParameter("@displaydate", content.displaydate.Value));
            }
            else
            {
                Sqlparam.Add(new SqlParameter("@displaydate", DBNull.Value));
            }

            Sqlparam.Add(new SqlParameter("@window_title", content.window_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@sequence", content.sequence ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@external_url", content.external_url ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@IsExternal", content.IsExternal ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@metatag", content.metatag ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@metadesc", content.metadesc ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@page_schema", content.page_schema ?? (object)DBNull.Value));
            if (content.metaexpiry.HasValue)
            {
                Sqlparam.Add(new SqlParameter("@metaexpiry", content.metaexpiry.Value));
            }
            else
            {
                Sqlparam.Add(new SqlParameter("@metaexpiry", DBNull.Value));
            }

            Sqlparam.Add(new SqlParameter("@status", content.status ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@language_master_id", content.language_master_id ?? 1));
            Sqlparam.Add(new SqlParameter("@lang_groupid", content.lang_groupid ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Geography_ID", content.Geography_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@ByLine", content.ByLine ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Publication", content.Publication ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@top_icon", content.top_icon ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@isSearch", content.isSearch ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@search_url", content.search_url ?? (object)DBNull.Value));
            // Sqlparam.Add(new SqlParameter("@page_url", content.page_url ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@breadcrumb_title", content.breadcrumb_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@business_id", content.business_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Hmpg_thumbnail_Media_id", content.Thumb_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Hmpg_thumbnail_alt_text", content.Thumb_image_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Small_icon_Media_id", content.Small_Icon_Thumb_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Small_icon_alt_text", content.Small_Icon_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Masthead_image_Media_id", content.Masthead_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Mobile_Masthead_image_Media_id", content.Mobile_Masthead_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Masthead_alt_text", content.Masthead_image_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Background_image_Media_id", content.Background_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Background_alt_text", content.Background_image_Alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Attach_file_Media_id", content.Attach_file_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@userid", userid));


            dt = GetDataSet("Content_Reprocess_Insert", Sqlparam.ToArray()).Tables[0];
            return dt;
        }
        catch
        {
            throw;
        }
    }

     protected DataSet List_Sections_Articles_Republished_DAL(int cont_id, string search, int language_id, int current_page, int pagesize = 500)
    {
        DataSet ds = new();
        try
        {
            ds = GetDataSet("CMS_Section_articles_Republished_List", "@cont_id", cont_id.ToString(), "@language_id", language_id.ToString(), "@searchquery", search, "@current_page", current_page.ToString(), "@PageSize", pagesize.ToString());
            return ds;
        }
        catch
        {
            throw;
        }
    }


    protected DataSet Reprocessed_Section_page_details_Get_DAL(int id)
    {
        DataSet ds = new();
        try
        {
            ds = GetDataSet("Reprocessed_page_details_Get", "@id", id.ToString());
            return ds;
        }
        catch
        {
            throw;
        }
    }

     protected DataTable Publish_Reprocessed_Content(Content_Master content, int userid)
    {
        DataTable dt = new();
        var Sqlparam = new List<SqlParameter>();

        try
        {
            Sqlparam.Add(new SqlParameter("@cont_Id", content.id));
            Sqlparam.Add(new SqlParameter("@title", content.title.Trim()));
            Sqlparam.Add(new SqlParameter("@parent_id", content.parent_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Template_Master_ID", content.Template_Master_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Content_Type_ID", content.Content_Type_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Template_Type_ID", content.Template_type ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@hmpg_title", content.hmpg_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@intro", content.intro ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@hmpg_intro", content.hmpg_intro ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@content", content.content ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@pagename", content.pagename.Trim()));
            if (content.displaydate.HasValue)
            {
                Sqlparam.Add(new SqlParameter("@displaydate", content.displaydate.Value));
            }
            else
            {
                Sqlparam.Add(new SqlParameter("@displaydate", DBNull.Value));
            }

            Sqlparam.Add(new SqlParameter("@window_title", content.window_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@sequence", content.sequence ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@external_url", content.external_url ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@IsExternal", content.IsExternal ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@metatag", content.metatag ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@metadesc", content.metadesc ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@page_schema", content.page_schema ?? (object)DBNull.Value));
            if (content.metaexpiry.HasValue)
            {
                Sqlparam.Add(new SqlParameter("@metaexpiry", content.metaexpiry.Value));
            }
            else
            {
                Sqlparam.Add(new SqlParameter("@metaexpiry", DBNull.Value));
            }

            Sqlparam.Add(new SqlParameter("@status", content.status ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@language_master_id", content.language_master_id ?? 1));
            Sqlparam.Add(new SqlParameter("@lang_groupid", content.lang_groupid ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Geography_ID", content.Geography_ID ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@ByLine", content.ByLine ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Publication", content.Publication ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@top_icon", content.top_icon ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@isSearch", content.isSearch ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@search_url", content.search_url ?? (object)DBNull.Value));
            // Sqlparam.Add(new SqlParameter("@page_url", content.page_url ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@breadcrumb_title", content.breadcrumb_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@business_id", content.business_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Hmpg_thumbnail_Media_id", content.Thumb_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Hmpg_thumbnail_alt_text", content.Thumb_image_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Small_icon_Media_id", content.Small_Icon_Thumb_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Small_icon_alt_text", content.Small_Icon_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Masthead_image_Media_id", content.Masthead_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Mobile_Masthead_image_Media_id", content.Mobile_Masthead_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Masthead_alt_text", content.Masthead_image_alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Background_image_Media_id", content.Background_image_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Background_alt_text", content.Background_image_Alttext ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Attach_file_Media_id", content.Attach_file_id ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@userid", userid));

            if(content.status ==1)
            {
            dt = GetDataSet("Content_Reprocess_Save", Sqlparam.ToArray()).Tables[0];
            }
            else
            {
            dt = GetDataSet("Content_Reprocess_Republished", Sqlparam.ToArray()).Tables[0];
            }
            return dt;
        }
        catch
        {
            throw;
        }
    }

    protected DataTable RePublish_Reprocessed_Content(int reprocess_Id, int userid)
    {
        DataTable dt = new();
        try
        {
            dt = GetDataSet("Content_Republished", "@id", reprocess_Id.ToString(),"@userid", userid.ToString()).Tables[0];
            return dt;
        }
        catch
        {
            throw;
        }
    }

    protected void Delete_Reprocessed_Content(int reprocess_Id)
    {
        
        try
        {
             SQLInsert_Update_Delete_Data("Delete_Reprocessed_Content", "@id", reprocess_Id.ToString());            
        }
        catch
        {
            throw;
        }
    }
}

