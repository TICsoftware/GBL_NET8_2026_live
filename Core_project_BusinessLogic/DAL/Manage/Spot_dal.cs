using System;
using System.Data;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.DAL;

public class Spot_dal : DBHelper
{
    public Spot_dal(IConfiguration configuration)
   : base(configuration) //  call base class constructor 
    {
    }


    public DataTable GetAll_Spot_DAL()
    {
        SqlParameter[] sqlParams = new SqlParameter[0]; // no input params
        return GetDataSet("GetAll_Spot_Details", sqlParams).Tables[0];
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
    public DataTable Add_Spot_DAL(Add_Spot_RHS t)
    {
        SqlParameter[] sqlParams =
        {
            new SqlParameter("@Language_Master_ID", t.Language_Master_ID),
            new SqlParameter("@Geography_ID", t.Geography_ID),
            new SqlParameter("@spot_template_master_id", t.spot_template_master_id),
            new SqlParameter("@Title", t.Title),
            new SqlParameter("@Description", t.Description),

            new SqlParameter("@Thumbnail_Img_Media_Id", t.Thumbnail_Img_Media_Id),
            new SqlParameter("@thumbnail_Alt_Text", t.thumbnail_Alt_Text),

            new SqlParameter("@background_Img_Media_Id", t.background_Img_Media_Id),
            new SqlParameter("@background_Alt_Text", t.background_Alt_Text),

            new SqlParameter("@icon_Img_Media_Id", t.icon_Img_Media_Id),
            new SqlParameter("@icon_Alt_Text", t.icon_Alt_Text),

            new SqlParameter("@Spot_Intro", t.Spot_Intro),
            new SqlParameter("@Spot_content", t.Spot_content),

            new SqlParameter("@Files_Media_Id", t.Files_Media_Id),

            new SqlParameter("@External_Url", t.External_Url),
            new SqlParameter("@Isexternal", t.Isexternal),

            new SqlParameter("@Status", t.Status),

            new SqlParameter("@Created_UserID", t.Created_UserID),

            new SqlParameter("@sequence", t.sequence)
        };

        return GetDataSet("spot_RHS_details_Insert", sqlParams).Tables[0];
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

            new SqlParameter("@spot_template_master_id", t.spot_template_master_id),

            new SqlParameter("@Title", t.Title),

            new SqlParameter("@Description",  t.Description ),

            new SqlParameter("@Thumbnail_Img_Media_Id", t.Thumbnail_Img_Media_Id ),

            new SqlParameter("@thumbnail_Alt_Text", t.thumbnail_Alt_Text ),

            new SqlParameter("@background_Img_Media_Id", t.background_Img_Media_Id),

            new SqlParameter("@background_Alt_Text", t.background_Alt_Text),

            new SqlParameter("@icon_Img_Media_Id", t.icon_Img_Media_Id ),

            new SqlParameter("@icon_Alt_Text", t.icon_Alt_Text ),

            new SqlParameter("@Spot_Intro", t.Spot_Intro ),

            new SqlParameter("@Spot_content", t.Spot_content ),

            new SqlParameter("@Files_Media_Id", t.Files_Media_Id ),

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







}
