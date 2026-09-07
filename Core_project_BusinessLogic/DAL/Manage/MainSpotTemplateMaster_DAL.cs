using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.DAL
{
    public class MainSpotTemplateMaster_DAL : DBHelper
    {
        public MainSpotTemplateMaster_DAL(IConfiguration config) : base(config) { }


        public List<MainSpotTemplateMaster> GetAll()
        {
            DataTable dt = GetDataSet("sp_MainSpotTemplateMaster_List_WithTemplates").Tables[0];
            return dt.AsEnumerable().Select(Map).ToList();
        }

        public List<MainSpotTemplateMaster> GetPaged(string search, int page, int size, out int total)
        {
            SqlParameter[] p = {
        new("@Search", (object?)search ?? DBNull.Value),
        new("@Page", page),
        new("@PageSize", size)
    };

            DataSet ds = GetDataSet("sp_MainSpotTemplateMaster_List_Paged", p);
            total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);

            return ds.Tables[0].AsEnumerable().Select(r => new MainSpotTemplateMaster
            {
                ID = (int)r["ID"],
                Name = r["Name"].ToString(),
                Language_Name = r["Language_Name"].ToString(),
                TemplateNames = r["TemplateTitles"]?.ToString(),
                Status = r["Status"] as int?
            }).ToList();
        }
        public MainSpotTemplateMaster GetById(int id)
        {
            SqlParameter[] p = { new SqlParameter("@ID", id) };
            DataTable dt = GetDataSet("sp_MainpreSpotTemplateMaster_ByID", p).Tables[0];
            if (dt.Rows.Count == 0) return null;
            return Map(dt.Rows[0]);
        }

        public int Insert(MainSpotTemplateMaster t)
        {
            SqlParameter[] p =
            {
            new SqlParameter("@Language_Master_ID", t.Language_Master_ID),
            new SqlParameter("@Name", t.Name),
           // new SqlParameter("@Template_Type_ID", t.Template_Type_ID),
            new SqlParameter("@Status", "1"),
            new SqlParameter("@Created_UserID", "1"),
            new SqlParameter("@Design_Layout", t.Design_Layout)
        };
            return SqlInsertReturnIdentity_withSP("sp_MainSpotTemplateMaster_Insert", "@NewID", p);
        }

        public void Update(MainSpotTemplateMaster t)
        {
            SqlParameter[] p =
            {
            new SqlParameter("@ID", t.ID),
            new SqlParameter("@Language_Master_ID", t.Language_Master_ID),
            new SqlParameter("@Name", t.Name),
           // new SqlParameter("@Template_Type_ID", t.Template_Type_ID),
            new SqlParameter("@Status", "1"),
            new SqlParameter("@Updated_UserID", "1"),
            new SqlParameter("@Design_Layout", t.Design_Layout)
        };
            SQLInsert_Update_Delete_Data("sp_MainSpotTemplateMaster_Update", p);
        }


        public void Insert(int mainSpotTemplateId, int templateId)
        {
            SqlParameter[] p =
            {
            new("@main_spot_template_master_id", mainSpotTemplateId),
            new("@template_master_id", templateId),
            new("@Status", 1)
        };

            SQLInsert_Update_Delete_Data("sp_MainComponentTemplateRef_Insert", p);
        }

        public void DeleteByMainSpot(int mainSpotTemplateId)
        {
            SqlParameter[] p =
            {
            new("@main_spot_template_master_id", mainSpotTemplateId)
              };

            SQLInsert_Update_Delete_Data("sp_MainComponentTemplateRef_DeleteByMainSpot", p);
        }

        public List<int> GetTemplatesByMainSpot(int mainSpotTemplateId)
        {
            SqlParameter[] p =
            {
            new("@main_spot_template_master_id", mainSpotTemplateId)
        };

            DataTable dt = GetDataSet("sp_MainComponentTemplateRef_ByMainSpot", p).Tables[0];
            List<int> ids = new();

            foreach (DataRow r in dt.Rows)
                ids.Add(Convert.ToInt32(r["template_master_id"]));

            return ids;
        }

        public void Deactivate(int id, int userId)
        {
            SqlParameter[] p =
            {
            new SqlParameter("@ID", id),
            new SqlParameter("@DeActivated_UserID", userId)
        };

            SQLInsert_Update_Delete_Data("sp_MainSpotTemplateMaster_Deactivate", p);
        }

        private MainSpotTemplateMaster Map(DataRow r)
        {
            return new MainSpotTemplateMaster
            {
                ID = Convert.ToInt32(r["ID"]),
                Language_Master_ID = r["Language_Master_ID"] as int?,
                Language_Name = r["Language_Name"].ToString(),
                Name = r["Name"].ToString(),
                //Status = r["Status"] as int?,
                // Created_UserID = r["Created_UserID"] as int?,
                // Created_Date = r["Created_Date"] as DateTime?,
                // Updated_UserID = r["Updated_UserID"] as int?,
                // Updated_Date = r["Updated_Date"] as DateTime?,
                // Published_UserID = r["Published_UserID"] as int?,
                // Published_Date = r["Published_Date"] as DateTime?,
                // DeActivated_UserID = r["DeActivated_UserID"] as int?,
                // DeActivated_Date = r["DeActivated_Date"] as DateTime?,
                Design_Layout = r["Design_Layout"].ToString()
            };
        }
    }

}
