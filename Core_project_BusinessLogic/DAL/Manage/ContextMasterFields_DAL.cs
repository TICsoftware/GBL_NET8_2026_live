using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
public class ContextMasterFields_DAL : DBHelper
{
    public ContextMasterFields_DAL(IConfiguration config) : base(config) {}
public List<int> GetSelectedFields(int contextMasterId)
{
    SqlParameter[] p =
    {
        new("@context_master_id", contextMasterId)
    };

    DataTable dt = GetDataSet("GetContextMasterFields", p).Tables[0];

    if (dt == null || dt.Rows.Count == 0)
        return new List<int>();

    return dt.AsEnumerable()
        .Select(r => Convert.ToInt32(r["context_field_id"]))
        .ToList();
}
public List<ContextField> GetAllFields()
{
    SqlParameter[] p = new SqlParameter[0];
    DataTable dt = GetDataSet("GetAllContextFields_assign", p).Tables[0];

    if (dt == null || dt.Rows.Count == 0)
        return new List<ContextField>();

    return dt.AsEnumerable()
        .Select(r => new ContextField
        {
            id = Convert.ToInt32(r["id"]),
            name = r["name"].ToString()
        }).ToList();
}

    public void Save(int contextMasterId, string csvFieldIds, int createdUserId)
    {
        SqlParameter[] p =
        {
            new("@context_master_id", contextMasterId),
            new("@field_ids", csvFieldIds),
            new("@Created_UserID", createdUserId)
        };

        SQLInsert_Update_Delete_Data("SaveContextMasterFields", p);
    }
}
}
