using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class ContextReference_DAL : DBHelper
    {
        public ContextReference_DAL(IConfiguration config) : base(config) { }

        public List<ContextReference> GetAll()
        {
            var list = new List<ContextReference>();
            DataTable dt = GetDataSet("GetAllContextReferences", new SqlParameter[0]).Tables[0];

            foreach (DataRow r in dt.Rows)
                list.Add(Map(r));

            return list;
        }
 public List<ContextReference> GetAll_bytemplateid(int templateid)
        {
            var list = new List<ContextReference>();
            SqlParameter[] p = { new SqlParameter("@templateid", templateid) };
            DataTable dt = GetDataSet("GetContextReferenceBytempId", p).Tables[0];
           
            foreach (DataRow r in dt.Rows)
                list.Add(Map(r));

            return list;
        }

        public ContextReference GetById(int id)
        {
            SqlParameter[] p = { new SqlParameter("@ID", id) };
            DataTable dt = GetDataSet("GetContextReferenceById", p).Tables[0];
            if (dt.Rows.Count == 0) return null;
            return Map(dt.Rows[0]);
        }
    public bool CheckIfDataExists(int referenceId)
    {
                 SqlParameter[] p = {
                  new("@referenceId", referenceId)
     };

    DataTable dt = GetDataSet("CheckContextDataExists", p).Tables[0];

    return dt.Rows.Count > 0;
}
        public int Add(ContextReference model)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@Language_Master_ID", model.Language_Master_ID ?? (object)DBNull.Value),
                new SqlParameter("@Context_Master_ID", model.Context_Master_ID ?? (object)DBNull.Value),
                new SqlParameter("@Template_Master_ID", model.Template_Master_ID ?? (object)DBNull.Value),
                new SqlParameter("@Reference_Title", model.Reference_Title ?? (object)DBNull.Value),
                new SqlParameter("@Status", model.Status ?? (object)1),
                new SqlParameter("@Created_UserID", model.Created_UserID ?? (object)1)
            };

            // If GetDataSet returns the inserted id row, you can parse it; otherwise use ExecuteNonQuery style in your helper.
            var dt = GetDataSet("InsertContextReference", p).Tables[0];
            if (dt.Rows.Count > 0 && dt.Columns.Contains("NewId"))
                return Convert.ToInt32(dt.Rows[0]["NewId"]);
            return 0;
        }

        public void Update(ContextReference model)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", model.ID),
                new SqlParameter("@Language_Master_ID", model.Language_Master_ID ?? (object)DBNull.Value),
                new SqlParameter("@Context_Master_ID", model.Context_Master_ID ?? (object)DBNull.Value),
                 new SqlParameter("@Template_Master_ID", model.Template_Master_ID ?? (object)DBNull.Value),
                new SqlParameter("@Reference_Title", model.Reference_Title ?? (object)DBNull.Value),
                new SqlParameter("@Status", model.Status ?? (object)1),
                new SqlParameter("@Updated_UserID", model.Updated_UserID ?? (object)1)
            };

            SQLInsert_Update_Delete_Data("UpdateContextReference", p);
        }

        public void Deactivate(int id, int userId)
        {
            SqlParameter[] p = { new SqlParameter("@ID", id), new SqlParameter("@UserID", userId) };
            SQLInsert_Update_Delete_Data("DeactivateContextReference", p);
        }

        private ContextReference Map(DataRow r)
        {
            return new ContextReference
            {
                ID = r["ID"] == DBNull.Value ? 0 : Convert.ToInt32(r["ID"]),
                Language_Master_ID = r.Table.Columns.Contains("Language_Master_ID") && r["Language_Master_ID"] != DBNull.Value ? (int?)Convert.ToInt32(r["Language_Master_ID"]) : null,
                Context_Master_ID = r.Table.Columns.Contains("Context_Master_ID") && r["Context_Master_ID"] != DBNull.Value ? (int?)Convert.ToInt32(r["Context_Master_ID"]) : null,
                Template_Master_ID = r.Table.Columns.Contains("Template_Master_ID") && r["Template_Master_ID"] != DBNull.Value ? (int?)Convert.ToInt32(r["Template_Master_ID"]) : null,
                Reference_Title = r.Table.Columns.Contains("Reference_Title") && r["Reference_Title"] != DBNull.Value ? r["Reference_Title"].ToString() : string.Empty,
                  Title = r.Table.Columns.Contains("Title") && r["Title"] != DBNull.Value ? r["Title"].ToString() : string.Empty,
                Status = r.Table.Columns.Contains("Status") && r["Status"] != DBNull.Value ? (int?)Convert.ToInt32(r["Status"]) : null,
                Created_UserID = r.Table.Columns.Contains("Created_UserID") && r["Created_UserID"] != DBNull.Value ? (int?)Convert.ToInt32(r["Created_UserID"]) : null,
                Created_Date = r.Table.Columns.Contains("Created_Date") && r["Created_Date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["Created_Date"]) : null,
                Updated_UserID = r.Table.Columns.Contains("Updated_UserID") && r["Updated_USERID"] != DBNull.Value ? (int?)Convert.ToInt32(r["Updated_USERID"]) : null,
                Updated_Date = r.Table.Columns.Contains("Updated_Date") && r["Updated_Date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["Updated_Date"]) : null,
                Published_UserID = r.Table.Columns.Contains("Published_UserID") && r["Published_USERID"] != DBNull.Value ? (int?)Convert.ToInt32(r["Published_USERID"]) : null,
                Published_Date = r.Table.Columns.Contains("Published_Date") && r["Published_Date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["Published_Date"]) : null,
                DeActivated_UserID = r.Table.Columns.Contains("DeActivated_UserID") && r["DeActivated_USERID"] != DBNull.Value ? (int?)Convert.ToInt32(r["DeActivated_USERID"]) : null,
                DeActivated_Date = r.Table.Columns.Contains("DeActivated_Date") && r["DeActivated_Date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["DeActivated_Date"]) : null
            };
        }
public DataTable GetLayoutData(int referenceId)
{
    SqlParameter[] p =
    {
        new SqlParameter("@ReferenceId", referenceId)
    };

    return GetDataSet("GetResolvedLayoutByReference", p).Tables[0];
}

public string GetHtmlLayoutByReferenceId(int referenceId)
{
    SqlParameter[] p =
    {
        new SqlParameter("@ReferenceId", referenceId)
    };

    DataTable dt = GetDataSet("GetHtmlLayoutByReferenceId", p).Tables[0];

    return dt.Rows.Count > 0
        ? dt.Rows[0]["Design_layout"]?.ToString()
        : string.Empty;
}


    }
}
