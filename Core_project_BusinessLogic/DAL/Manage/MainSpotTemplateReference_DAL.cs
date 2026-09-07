using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.DAL
{
    public class MainSpotTemplateReference_DAL : DBHelper
    {
        public MainSpotTemplateReference_DAL(IConfiguration config) : base(config) { }

        public List<MainSpotTemplateReference> GetAll()
        {
            DataTable dt = GetDataSet("sp_main_spot_template_reference_List").Tables[0];
            return dt.AsEnumerable().Select(Map).ToList();
        }

        public MainSpotTemplateReference GetById(int id)
        {
            SqlParameter[] p = { new SqlParameter("@ID", id) };
            DataTable dt = GetDataSet("sp_main_spot_template_reference_ByID", p).Tables[0];
            if (dt.Rows.Count == 0) return null;
            return Map(dt.Rows[0]);
        }

        public int Insert(MainSpotTemplateReference t)
        {
            SqlParameter[] p =
            {
            new SqlParameter("@Language_Master_ID", t.Language_Master_ID),
            new SqlParameter("@Reference_Title", t.Reference_Title),
            new SqlParameter("@Template_master_id", t.Template_master_id),
            new SqlParameter("@Main_spot_template_master_id", t.Main_spot_template_master_id),
            new SqlParameter("@Status", "1"),
            new SqlParameter("@Created_UserID", "1")
        };
            return SqlInsertReturnIdentity_withSP("sp_main_spot_template_reference_Insert", "@NewID", p);
        }

        public void Update(MainSpotTemplateReference t)
        {
            SqlParameter[] p =
            {
            new SqlParameter("@Reference_ID", t.Reference_ID),
            new SqlParameter("@Language_Master_ID", t.Language_Master_ID),
            new SqlParameter("@Reference_Title", t.Reference_Title),
            new SqlParameter("@Template_master_id", t.Template_master_id),
            new SqlParameter("@Main_spot_template_master_id", t.Main_spot_template_master_id),
            new SqlParameter("@Status", "1"),
            new SqlParameter("@Updated_UserID", "1")
        };
            SQLInsert_Update_Delete_Data("sp_main_spot_template_reference_Update", p);
        }

        public void Deactivate(int id, int userId)
        {
            SqlParameter[] p =
            {
            new SqlParameter("@Reference_ID", id),
            new SqlParameter("@DeActivated_UserID", userId)
        };

            SQLInsert_Update_Delete_Data("sp_main_spot_template_reference_Deactivate", p);
        }

        private MainSpotTemplateReference Map(DataRow r)
        {
            return new MainSpotTemplateReference
            {
                Reference_ID = Convert.ToInt32(r["Reference_ID"]),
                Language_Master_ID = r["Language_Master_ID"] as int?,
                Reference_Title = r["Reference_Title"].ToString(),
                Template_master_id = r["Template_master_id"] as int?,
                Main_spot_template_master_id = r["Main_spot_template_master_id"] as int?,
                Status = r["Status"] as int?,
                Created_UserID = r["Created_UserID"] as int?,
                Created_Date = r["Created_Date"] as DateTime?,
                Updated_UserID = r["Updated_UserID"] as int?,
                Updated_Date = r["Updated_Date"] as DateTime?,
                Published_UserID = r["Published_UserID"] as int?,
                Published_Date = r["Published_Date"] as DateTime?,
                DeActivated_UserID = r["DeActivated_UserID"] as int?,
                DeActivated_Date = r["DeActivated_Date"] as DateTime?
            };
        }
    }

}
