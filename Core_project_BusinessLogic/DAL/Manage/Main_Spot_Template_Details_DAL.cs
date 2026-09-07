using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.DAL
{
    public class Main_Spot_Template_Details_DAL : DBHelper
    {
        public Main_Spot_Template_Details_DAL(IConfiguration config) : base(config) { }

        public List<Main_spot_template_details> GetAll()
        {
            DataTable dt = GetDataSet("sp_main_spot_template_details_List").Tables[0];
            return dt.AsEnumerable().Select(Map).ToList();
        }

        public Main_spot_template_details GetById(int id)
        {
            SqlParameter[] p = { new SqlParameter("@ID", id) };
            DataTable dt = GetDataSet("sp_MainSpotTemplateMaster_ByID", p).Tables[0];
            if (dt.Rows.Count == 0) return null;
            return Map(dt.Rows[0]);
        }

        public int Insert(Main_spot_template_details t)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@Language_Master_ID", t.Language_Master_ID),
                new SqlParameter("@main_spot_template_reference_id", t.main_spot_template_reference_id),
                new SqlParameter("@spot_reference_id", t.spot_reference_id),

                new SqlParameter("@Title", t.Title ?? (object)DBNull.Value),
                new SqlParameter("@Intro", t.Intro ?? (object)DBNull.Value),
                new SqlParameter("@Spot_Content", t.Spot_Content ?? (object)DBNull.Value),

                new SqlParameter("@Thumbnail_Image_Media_Id", t.Thumbnail_Image_Media_Id ?? (object)DBNull.Value),
                new SqlParameter("@Thumbnail_Alt", t.Thumbnail_Alt ?? (object)DBNull.Value),

                new SqlParameter("@Background_Image_Media_Id", t.Background_Image_Media_Id ?? (object)DBNull.Value),
                new SqlParameter("@Background_Alt", t.Background_Alt ?? (object)DBNull.Value),

                new SqlParameter("@Logo_Image_Media_Id", t.Logo_Image_Media_Id ?? (object)DBNull.Value),

                new SqlParameter("@Url", t.Url ?? (object)DBNull.Value),
                new SqlParameter("@Is_External", t.Is_External),

                new SqlParameter("@Button_One_Title", t.Button_One_Title ?? (object)DBNull.Value),
                new SqlParameter("@Button_Two_Title", t.Button_Two_Title ?? (object)DBNull.Value),

                new SqlParameter("@Link_Url_Button_One", t.Link_Url_Button_One ?? (object)DBNull.Value),
                new SqlParameter("@Botton_One_Is_External", t.Botton_One_Is_External),

                new SqlParameter("@Link_Url_Button_Two", t.Link_Url_Button_Two ?? (object)DBNull.Value),
                new SqlParameter("@Botton_Two_Is_External", t.Botton_Two_Is_External),

                new SqlParameter("@Video_Path_Media_Id", t.Video_Path_Media_Id ?? (object)DBNull.Value),
                new SqlParameter("@Video_Preview_Image_Media_Id", t.Video_Preview_Image_Media_Id ?? (object)DBNull.Value),

                new SqlParameter("@Upload_File_Media_Id", t.Upload_File_Media_Id ?? (object)DBNull.Value),

                new SqlParameter("@Display_Date", t.Display_Date ?? (object)DBNull.Value),
                new SqlParameter("@Sequence", t.Sequence ?? (object)DBNull.Value),
                new SqlParameter("@Status", t.Status ?? 1),   // default 1

                new SqlParameter("@Created_UserID", t.Created_UserID ?? 1) // default user = 1
            };

            return SqlInsertReturnIdentity_withSP(
                "sp_MainSpotTemplateDetails_Insert",
                "@NewID",
                p
            );
        }




        public void Update(Main_spot_template_details t)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", t.ID),

                new SqlParameter("@Language_Master_ID", t.Language_Master_ID),
                new SqlParameter("@main_spot_template_reference_id", t.main_spot_template_reference_id),
                new SqlParameter("@spot_reference_id", t.spot_reference_id),

                new SqlParameter("@Title", t.Title ?? (object)DBNull.Value),
                new SqlParameter("@Intro", t.Intro ?? (object)DBNull.Value),
                new SqlParameter("@Spot_Content", t.Spot_Content ?? (object)DBNull.Value),

                new SqlParameter("@Thumbnail_Image_Media_Id", t.Thumbnail_Image_Media_Id ?? (object)DBNull.Value),
                new SqlParameter("@Thumbnail_Alt", t.Thumbnail_Alt ?? (object)DBNull.Value),

                new SqlParameter("@Background_Image_Media_Id", t.Background_Image_Media_Id ?? (object)DBNull.Value),
                new SqlParameter("@Background_Alt", t.Background_Alt ?? (object)DBNull.Value),

                new SqlParameter("@Logo_Image_Media_Id", t.Logo_Image_Media_Id ?? (object)DBNull.Value),

                new SqlParameter("@Url", t.Url ?? (object)DBNull.Value),
                new SqlParameter("@Is_External", t.Is_External),

                new SqlParameter("@Button_One_Title", t.Button_One_Title ?? (object)DBNull.Value),
                new SqlParameter("@Button_Two_Title", t.Button_Two_Title ?? (object)DBNull.Value),

                new SqlParameter("@Link_Url_Button_One", t.Link_Url_Button_One ?? (object)DBNull.Value),
                new SqlParameter("@Botton_One_Is_External", t.Botton_One_Is_External),

                new SqlParameter("@Link_Url_Button_Two", t.Link_Url_Button_Two ?? (object)DBNull.Value),
                new SqlParameter("@Botton_Two_Is_External", t.Botton_Two_Is_External),

                new SqlParameter("@Video_Path_Media_Id", t.Video_Path_Media_Id ?? (object)DBNull.Value),
                new SqlParameter("@Video_Preview_Image_Media_Id", t.Video_Preview_Image_Media_Id ?? (object)DBNull.Value),

                new SqlParameter("@Upload_File_Media_Id", t.Upload_File_Media_Id ?? (object)DBNull.Value),

                new SqlParameter("@Display_Date", t.Display_Date ?? (object)DBNull.Value),
                new SqlParameter("@Sequence", t.Sequence ?? (object)DBNull.Value),

                new SqlParameter("@Status", t.Status ?? 1),
                new SqlParameter("@Updated_UserID", t.Updated_UserID ?? 1)
            };

            SQLInsert_Update_Delete_Data("sp_MainSpotTemplateDetails_Update", p);
        }


        public void Deactivate(int id, int userId)
        {
            SqlParameter[] p =
            {
            new SqlParameter("@ID", id),
            new SqlParameter("@DeActivated_UserID", userId)
        };

            SQLInsert_Update_Delete_Data("sp_Main_spot_template_details_Deactivate", p);
        }


        public DataSet GetSpotLayoutById(int id)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", id)
            };

            return GetDataSet("sp_Get_Spot_Layout_ByID", p);
        }

        private Main_spot_template_details Map(DataRow r)
        {
            return new Main_spot_template_details
            {
                ID = Convert.ToInt32(r["ID"]),
                Language_Master_ID = r["Language_Master_ID"] as int?,
                main_spot_template_reference_id = r["main_spot_template_reference_id"] as int?,
                Main_Spot_Template_Reference = r["Main_Spot_Template"]?.ToString(),
                spot_reference_id = r["spot_reference_id"] as int?,
                spot_reference_name = r["spot_reference_name"]?.ToString(),

                Title = r["Title"]?.ToString(),
                Intro = r["Intro"]?.ToString(),
                Spot_Content = r["Spot_Content"]?.ToString(),

                Thumbnail_Image_Media_Id = r["Thumbnail_Image_Media_Id"] as int?,
                Thumbnail_Image = r["Thumbnail_Image"]?.ToString(),
                Thumbnail_Alt = r["Thumbnail_Alt"]?.ToString(),

                Background_Image_Media_Id = r["Background_Image_Media_Id"] as int?,
                Background_Image = r["Background_Image"]?.ToString(),
                Background_Alt = r["Background_Alt"]?.ToString(),

                Logo_Image = r["Logo_Image"]?.ToString(),
                Logo_Image_Media_Id = r["Logo_Image_Media_Id"] as int?,

                Url = r["Url"]?.ToString(),
                Is_External = r["Is_External"] != DBNull.Value && Convert.ToBoolean(r["Is_External"]),

                Button_One_Title = r["Button_One_Title"]?.ToString(),
                Button_Two_Title = r["Button_Two_Title"]?.ToString(),

                Link_Url_Button_One = r["Link_Url_Button_One"]?.ToString(),
                Botton_One_Is_External = r["Botton_One_Is_External"] != DBNull.Value && Convert.ToBoolean(r["Botton_One_Is_External"]),

                Link_Url_Button_Two = r["Link_Url_Button_Two"]?.ToString(),
                Botton_Two_Is_External = r["Botton_Two_Is_External"] != DBNull.Value && Convert.ToBoolean(r["Botton_Two_Is_External"]),

                Video_Path = r["Video_Path"]?.ToString(),
                Video_Preview_Image_Media_Id = r["Video_Preview_Image_Media_Id"] as int?,
                Video_Preview_Image = r["Video_Preview_Image"]?.ToString(),

                Upload_File_Media_Id = r["Upload_File_Media_Id"] as int?,
                Upload_File = r["Upload_File"]?.ToString(),

                Display_Date = r["Display_Date"] as DateTime?,
                Sequence = r["Sequence"] as int?,
                Status = r["Status"] as int?,
                Content_Status = r["Content_Status"]?.ToString(),

                Created_UserID = r["Created_UserID"] as int?,
                Created_Date = r["Created_Date"] as DateTime?,

                Updated_UserID = r["Updated_UserID"] as int?,
                Updated_Date = r["Updated_Date"] as DateTime?
            };
        }
    }

}
