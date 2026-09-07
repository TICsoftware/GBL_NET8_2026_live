using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class ContextMaster_DAL : DBHelper
    {
        public ContextMaster_DAL(IConfiguration configuration) : base(configuration) { }

        public List<ContextMaster> GetAll()
        {
            var list = new List<ContextMaster>();
            DataTable dt = GetDataSet("sp_ContextMaster_List").Tables[0];
            foreach (DataRow r in dt.Rows) list.Add(Map(r));
            return list;
        }
public (List<ContextMasterListVM> Data, int Total) GetPaged(string search, int page, int pageSize)
{
    SqlParameter[] p =
    {
        new("@Search", string.IsNullOrEmpty(search) ? DBNull.Value : search),
        new("@Page", page),
        new("@PageSize", pageSize)
    };

    DataSet ds = GetDataSet("sp_ContextMaster_List_Paged", p);

    List<ContextMasterListVM> list = new();
    foreach (DataRow r in ds.Tables[0].Rows)
    {
        list.Add(new ContextMasterListVM
        {
            ID = Convert.ToInt32(r["ID"]),
            Title = r["Title"].ToString(),
            Language_Name = r["Language_Name"].ToString(),
            TemplateNames = r["TemplateTitles"]?.ToString(),
            Status = r["Status"] as int?,
            Created_Date = r["Created_Date"] as DateTime?
        });
    }

    int total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
    return (list, total);
}

        public ContextMaster GetById(int id)
        {
            SqlParameter[] p = { new SqlParameter("@ID", id) };
            DataTable dt = GetDataSet("sp_ContextMaster_ByID", p).Tables[0];
            if (dt.Rows.Count == 0) return null;
            return Map(dt.Rows[0]);
        }
        public ContextMaster GetById_layout(int id)
{
    SqlParameter[] p =
    {
        new SqlParameter("@ID", id)
    };

    DataTable dt = GetDataSet("GetContextMasterById", p).Tables[0];

    if (dt.Rows.Count == 0)
        return null;

    var r = dt.Rows[0];

    return new ContextMaster
    {
        ID = Convert.ToInt32(r["ID"]),
        Design_Layout = r["design_layout"]?.ToString()
    };
}

        public int Insert(ContextMaster m)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@Language_Master_ID", m.Language_Master_ID),
                new SqlParameter("@Title", m.Title ?? (object)DBNull.Value),
                new SqlParameter("@Status", m.Status),
                     new SqlParameter("@Design_Layout", m.Design_Layout ?? (object)DBNull.Value),
                new SqlParameter("@Created_UserID", m.Created_UserID)
              
            };
            return SqlInsertReturnIdentity_withSP("sp_ContextMaster_Insert", "@NewID", p);
        }

        public void Update(ContextMaster m)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", m.ID),
                new SqlParameter("@Language_Master_ID", m.Language_Master_ID),
                new SqlParameter("@Title", m.Title ?? (object)DBNull.Value),
                new SqlParameter("@Status", m.Status),
                   new SqlParameter("@Design_Layout", m.Design_Layout ?? (object)DBNull.Value),
                new SqlParameter("@Updated_UserID", m.Updated_UserID)
            };
            SQLInsert_Update_Delete_Data("sp_ContextMaster_Update", p);
        }

        public void Deactivate(int id, int userId)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", id),
                new SqlParameter("@DeActivated_UserID", userId)
            };
            SQLInsert_Update_Delete_Data("sp_ContextMaster_Deactivate", p);
        }

        private ContextMaster Map(DataRow r)
        {
            return new ContextMaster
            {
                ID = Convert.ToInt32(r["ID"]),
                Language_Master_ID = r["Language_Master_ID"] as int?,
                Title = r["Title"]?.ToString(),
                Status = r["Status"] as int?,
                Created_UserID = r["Created_UserID"] as int?,
                Created_Date = r["Created_Date"] as DateTime?,
                Updated_UserID = r["Updated_UserID"] as int?,
                Updated_Date = r["Updated_Date"] as DateTime?,
                Published_UserID = r["Published_UserID"] as int?,
                Published_Date = r["Published_Date"] as DateTime?,
                DeActivated_UserID = r["DeActivated_UserID"] as int?,
                DeActivated_Date = r["DeActivated_Date"] as DateTime?,
                Design_Layout = r["Design_Layout"].ToString()
            };
        }
    }
}
