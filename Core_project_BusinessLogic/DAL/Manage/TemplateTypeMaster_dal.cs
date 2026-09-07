using System;
using System.Data;
using Core_project_BusinessLogic.Entity.Manage;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.DAL;

public class TemplateTypeMaster_dal : DBHelper
{
    public TemplateTypeMaster_dal(IConfiguration configuration)
   : base(configuration) //  call base class constructor 
    {
    }


    public DataTable GetAll_Template_Type_Master_DAL()
    {
        SqlParameter[] sqlParams = new SqlParameter[0]; // no input params
        return GetDataSet("GetAllTemplate_Type_Masters", sqlParams).Tables[0];
    }
    // ================================
    // 👉 Get All Template Types (NEW)
    // ================================
    public List<TemplateTypeMaster> GetAll()
    {
        SqlParameter[] sqlParams = new SqlParameter[0];

        DataTable dt = GetDataSet("GetAllTemplate_Type_Masters", sqlParams).Tables[0];

        List<TemplateTypeMaster> list = new();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(new TemplateTypeMaster
            {
                ID = Convert.ToInt32(row["ID"]),
                Template_Type = row["Template_Type"].ToString()
            });
        }

        return list;
    }
    // ======================================
    // 🔹 Get TemplateMaster by ID
    // ======================================
    public DataTable GetById_Template_Type_Master_DAL(int id)
    {
        SqlParameter[] sqlParams =
        {
                new SqlParameter("@ID", id)
            };

        return GetDataSet("GetTemplate_Type_MasterById", sqlParams).Tables[0];
    }

    // ======================================
    // 🔹 Insert New TemplateMaster
    // ======================================
    public DataTable Add_Template_Type_Master_DAL(Add_Template_Type t)
    {
        SqlParameter[] sqlParams =
        {

                new SqlParameter("@Template_Type", t.Template_Type.Trim() ),
                new SqlParameter("@Design_Layout", t.Design_Layout.Trim() ),
                new SqlParameter("@Status", t.Status) ,
                new SqlParameter("@Created_UserID", t.Created_UserID )
            };

        return GetDataSet("Template_Type_Master_Insert", sqlParams).Tables[0];
    }

    // ======================================
    // 🔹 Update TemplateMaster
    // ======================================
    public void Update_Template_Type_Master_DAL(Add_Template_Type t)
    {
        SqlParameter[] sqlParams =
        {
                new SqlParameter("@ID", t.ID),
                new SqlParameter("@Template_Type", t.Template_Type?.Trim() ),
                new SqlParameter("@Design_Layout", t.Design_Layout?.Trim() ),
                new SqlParameter("@Status", t.Status ),
                new SqlParameter("@Updated_UserID", t.Updated_UserID )
            };

        SQLInsert_Update_Delete_Data("Update_Template_Type_Master", sqlParams);
    }

    // ======================================
    // 🔹 Delete TemplateMaster
    // ======================================
    public void Delete_Template_Type_Master_DAL(int id)
    {
        SqlParameter[] sqlParams =
        {
                new SqlParameter("@ID", id)
            };

        SQLInsert_Update_Delete_Data("DeleteTemplateTypeMaster", sqlParams);
    }

    public List<int> GetComponentByBlockLayout(int Block_layout_id)
    {
        SqlParameter[] p =
        {
            new("@Block_layout_id", Block_layout_id)
        };

        DataTable dt = GetDataSet("sp_Block_component_reference_ByBlock", p).Tables[0];
        List<int> ids = new();

        foreach (DataRow r in dt.Rows)
            ids.Add(Convert.ToInt32(r["component_master_id"]));

        return ids;
    }

    public void InsertBlockComp(int Block_layout_id, int component_master_id)
    {
        SqlParameter[] p =
        {
            new("@Block_layout_id", Block_layout_id),
            new("@component_master_id", component_master_id),
            new("@Status", 1)
        };

        SQLInsert_Update_Delete_Data("sp_Block_component_reference_Insert", p);
    }

    public void DeleteByBlockComp(int Block_layout_id)
    {
        SqlParameter[] p =
        {
            new("@Block_layout_id", Block_layout_id)
        };

        SQLInsert_Update_Delete_Data("sp_Block_component_reference_Delete", p);
    }




}
