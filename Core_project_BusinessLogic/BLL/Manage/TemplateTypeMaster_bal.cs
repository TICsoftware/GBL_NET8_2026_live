using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity.Manage;

namespace Core_project_BusinessLogic.BLL;

public class TemplateTypeMaster_bal
{
    private readonly TemplateTypeMaster_dal _dal;
    public TemplateTypeMaster_bal(IConfiguration configuration)
    {
        _dal = new TemplateTypeMaster_dal(configuration);
    }


    public List<Add_Template_Type> GetAllTemplatesType_bal()
    {
        List<Add_Template_Type> list = new List<Add_Template_Type>();
        DataTable dt = _dal.GetAll_Template_Type_Master_DAL();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(MapTemplate(row));
        }

        return list;
    }

    // ======================================
    // 🔹 2. Get Template by ID
    // ======================================
    public Add_Template_Type GetTemplateTypeById_bal(int id)
    {
        DataTable dt = _dal.GetById_Template_Type_Master_DAL(id);

        if (dt.Rows.Count > 0)
        {
            return MapTemplate(dt.Rows[0]);
        }

        return null;
    }

    // ======================================
    // 🔹 3. Insert New Template
    // ======================================
    public int AddTemplateType_bal(Add_Template_Type template)
    {
        DataTable dt = _dal.Add_Template_Type_Master_DAL(template);

        if (dt.Rows.Count > 0 && dt.Columns.Contains("NewID"))
        {
            return Convert.ToInt32(dt.Rows[0]["NewID"]);
        }

        return 0;
    }

    // ======================================
    // 🔹 4. Update Template
    // ======================================
    public void UpdateTemplateType_bal(Add_Template_Type template)
    {
        _dal.Update_Template_Type_Master_DAL(template);
    }

    // ======================================
    // 🔹 5. Delete Template
    // ======================================
    public void DeleteTemplateType_bal(int id)
    {
        _dal.Delete_Template_Type_Master_DAL(id);
    }

    // ======================================

    public List<int> GetComponentIds(int Block_layout_id)
    {
        return _dal.GetComponentByBlockLayout(Block_layout_id);
    }

    public void SaveMappings(int Block_Layout_id, List<int> ComponentIds)
    {
        // delete old mappings
        _dal.DeleteByBlockComp(Block_Layout_id);

        // insert new
        foreach (var tid in ComponentIds)
        {
            _dal.InsertBlockComp(Block_Layout_id, tid);
        }
    }



    // 🔹 Helper to map DataRow to TemplateMaster entity
    // ======================================
    private Add_Template_Type MapTemplate(DataRow row)
    {
        return new Add_Template_Type
        {
            ID = Convert.ToInt32(row["ID"]),
            Template_Type = row["Template_Type"].ToString(),
            Design_Layout = row["Design_Layout"].ToString(),
            Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : (int?)null,
            Created_UserID = row["Created_UserID"] != DBNull.Value ? Convert.ToInt32(row["Created_UserID"]) : (int?)null,
            Created_Date = row["Created_Date"] != DBNull.Value ? Convert.ToDateTime(row["Created_Date"]) : (DateTime?)null,
            Updated_UserID = row["Updated_UserID"] != DBNull.Value ? Convert.ToInt32(row["Updated_UserID"]) : (int?)null,
            Updated_Date = row["Updated_Date"] != DBNull.Value ? Convert.ToDateTime(row["Updated_Date"]) : (DateTime?)null,
            Published_UserID = row["Published_UserID"] != DBNull.Value ? Convert.ToInt32(row["Published_UserID"]) : (int?)null,
            Published_Date = row["Published_Date"] != DBNull.Value ? Convert.ToDateTime(row["Published_Date"]) : (DateTime?)null,
        };
    }

}
