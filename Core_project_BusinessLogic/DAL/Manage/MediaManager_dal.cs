using System;
using System.Data;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.DAL;

public class MediaManager_dal : DBHelper
{
    public MediaManager_dal(IConfiguration configuration)
   : base(configuration) //  call base class constructor 
    {
    }


    public DataTable GetAll_Media_DAL(string sort, string type, string search, int page, int pageSize)
    {
        SqlParameter[] sqlParams =
        {
            new SqlParameter("@search", search),
            new SqlParameter("@sort", sort),
            new SqlParameter("@page", page),
            new SqlParameter("@pageSize", pageSize)
        };

        return GetDataSet("media_GetList", sqlParams).Tables[0];
    }

    // ======================================
    // 🔹 Get TemplateMaster by ID
    // ======================================
    public DataTable GetSpot_ById_DAL(int id)
    {
        SqlParameter[] sqlParams =
        {
                new SqlParameter("@ID", id)
            };

        return GetDataSet("GetSpot_ById", sqlParams).Tables[0];
    }

    // ======================================
    // 🔹 Insert New TemplateMaster
    // ======================================
    public DataTable Add_Media_DAL(MediaItem m)
    {
        SqlParameter[] sqlParams =
        {
            new SqlParameter("@media_file_name", m.media_file_name),
            new SqlParameter("@file_path", m.file_path),
            new SqlParameter("@file_type", m.file_type),
            new SqlParameter("@file_size", m.file_size),
            new SqlParameter("@status", (object?)m.status ?? DBNull.Value),
            new SqlParameter("@Created_UserID", (object?)m.Created_UserID ?? DBNull.Value),
        };

        return GetDataSet("media_Insert", sqlParams).Tables[0];
    }



    public DataTable update_media_DAL(MediaTemp m)
    {
        SqlParameter[] sqlParams =
        {
            new SqlParameter("@media_id", m.media_id),
            new SqlParameter("@file_alt_text", m.file_alt_text),
            new SqlParameter("@Updated_UserID", 1),
        };

        return GetDataSet("update_media", sqlParams).Tables[0];
    }


    public DataTable Add_Media_temp_DAL(MediaTemp m)
    {
        SqlParameter[] sqlParams =
        {
            new SqlParameter("@media_id", m.media_id),
            new SqlParameter("@file_alt_text", m.file_alt_text),
            new SqlParameter("@Created_UserID", 1),
        };

        return GetDataSet("media_mapping_temp_Insert", sqlParams).Tables[0];
    }

    // ======================================
    // 🔹 Update TemplateMaster
    // ======================================
    public void Update_Spot_DAL(Add_Spot_RHS t)
    {
        SqlParameter[] sqlParams =
        {
            new SqlParameter("@ID", t.ID),

            new SqlParameter("@Language_Master_ID", t.Language_Master_ID),

            new SqlParameter("@Geography_ID", t.Geography_ID),

            new SqlParameter("@Template_Type_ID", t.Template_Type_ID),

            new SqlParameter("@spot_template_master_id", t.spot_template_master_id),

            new SqlParameter("@Title", t.Title),

            new SqlParameter("@Description",  t.Description ),

            new SqlParameter("@Thumbnail_Img", t.Thumbnail_Img ),

            new SqlParameter("@thumbnail_Alt_Text", t.thumbnail_Alt_Text ),

            new SqlParameter("@background_Img", t.background_Img),

            new SqlParameter("@background_Alt_Text", t.background_Alt_Text),

            new SqlParameter("@icon_Img", t.icon_Img ),

            new SqlParameter("@icon_Alt_Text", t.icon_Alt_Text ),

            new SqlParameter("@Spot_Intro", t.Spot_Intro ),

            new SqlParameter("@Spot_content", t.Spot_content ),

            new SqlParameter("@Design_layout", t.Design_layout ),

            new SqlParameter("@Files", t.Files ),

            new SqlParameter("@External_Url", t.External_Url ),

            new SqlParameter("@Isexternal", t.Isexternal ),

            new SqlParameter("@Status", t.Status ),

            new SqlParameter("@Updated_UserID", t.Updated_UserID ),

            new SqlParameter("@sequence", t.sequence )
        };

        SQLInsert_Update_Delete_Data("spot_RHS_details_Update", sqlParams);
    }


    // ======================================
    // 🔹 Delete TemplateMaster
    // ======================================
    public void DeleteSpot_DAL(int id)
    {
        SqlParameter[] sqlParams =
        {
                new SqlParameter("@ID", id)
            };

        SQLInsert_Update_Delete_Data("DeleteSpot_RHS_details", sqlParams);
    }



    public DataTable Get_MediaById_DAL(int mediaId)
    {
        SqlParameter[] sqlParams =
        {
            new SqlParameter("@mediaId", mediaId)
        };

        return GetDataSet("GetMediaById", sqlParams).Tables[0];
    }


    public void DeleteMedia_DAL(int id, int updatedBy, string file_path)
    {
        SqlParameter[] sqlParams =
        {
            new SqlParameter("@ID", id),
            new SqlParameter("@file_path", file_path),
            new SqlParameter("@updatedBy", updatedBy)
        };

        SQLInsert_Update_Delete_Data("DeleteMedia", sqlParams);
    }




}
