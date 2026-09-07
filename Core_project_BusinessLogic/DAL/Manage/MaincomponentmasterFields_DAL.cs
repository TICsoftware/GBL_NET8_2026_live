using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
public class MaincomponentmasterFields_DAL : DBHelper
{
    public MaincomponentmasterFields_DAL(IConfiguration config) : base(config) {}
public List<int> GetSelectedFields(int componentMasterId)
{
    SqlParameter[] p =
    {
        new("@main_component_master_id", componentMasterId)
    };

    DataTable dt = GetDataSet("GetmaincomponentmasterFields", p).Tables[0];

    if (dt == null || dt.Rows.Count == 0)
        return new List<int>();

    return dt.AsEnumerable()
        .Select(r => Convert.ToInt32(r["main_component_fields_id"]))
        .ToList();
}
public List<ComponentMasterField> GetAllFields()
{
    SqlParameter[] p = new SqlParameter[0];
    DataTable dt = GetDataSet("GetAllmainComponentFields_assign", p).Tables[0];

    if (dt == null || dt.Rows.Count == 0)
        return new List<ComponentMasterField>();

    return dt.AsEnumerable()
        .Select(r => new ComponentMasterField
        {
            id = Convert.ToInt32(r["id"]),
            name = r["name"].ToString()
        }).ToList();
}

    public void Save(int componentMasterId, string csvFieldIds, int createdUserId)
    {
        SqlParameter[] p =
        {
            new("@main_component_master_id", componentMasterId),
            new("@field_ids", csvFieldIds),
            new("@Created_UserID", createdUserId)
        };

        SQLInsert_Update_Delete_Data("SaveComponentMasterFields", p);
    }
}
}
