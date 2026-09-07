using System;
using System.Data;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.DAL;

public class SpotTemplateTypeMaster_dal : DBHelper
{
    public SpotTemplateTypeMaster_dal(IConfiguration configuration)
   : base(configuration) //  call base class constructor 
    {
    }


    public DataTable GetAll_SpotTemplate_Master_DAL()
    {
        SqlParameter[] sqlParams = new SqlParameter[0]; // no input params
        return GetDataSet("GetAllspot_template_master", sqlParams).Tables[0];
    }

    // ======================================
    // 🔹 Get TemplateMaster by ID
    // ======================================
    public DataTable GetById_SpotTemplate_Master_DAL(int id)
    {
        SqlParameter[] sqlParams =
        {
                new SqlParameter("@ID", id)
            };

        return GetDataSet("GetSpot_template_masterById", sqlParams).Tables[0];
    }

    // ======================================
    // 🔹 Insert New TemplateMaster
    // ======================================
    public DataTable Add_SpotTemplate_Master_DAL(Add_Spot_Template_Master t)
    {
        SqlParameter[] sqlParams =
        {
                new SqlParameter("@Language_Master_ID", t.Language_Master_ID ),
                new SqlParameter("@Name", t.Name) ,
                new SqlParameter("@Template_Type_ID", t.Template_Type_ID ),
                new SqlParameter("@Status", t.Status) ,
                new SqlParameter("@Created_UserID", t.Created_UserID )
            };

        return GetDataSet("spot_template_master_Insert", sqlParams).Tables[0];
    }

    // ======================================
    // 🔹 Update TemplateMaster
    // ======================================
    public void Update_SpotTemplate_Master_DAL(Add_Spot_Template_Master t)
    {
        SqlParameter[] sqlParams =
        {
                new SqlParameter("@ID", t.ID),
                new SqlParameter("@Language_Master_ID", t.Language_Master_ID ),
                new SqlParameter("@Name", t.Name?.Trim() ),
                new SqlParameter("@Template_Type_ID",  t.Template_Type_ID ),
                new SqlParameter("@Status", t.Status ),
                new SqlParameter("@Updated_UserID", t.Updated_UserID )
            };

        SQLInsert_Update_Delete_Data("Update_Spot_template_master", sqlParams);
    }

    // ======================================
    // 🔹 Delete TemplateMaster
    // ======================================
    public void Delete_SpotTemplate_Master_DAL(int id)
    {
        SqlParameter[] sqlParams =
        {
                new SqlParameter("@ID", id)
            };

        SQLInsert_Update_Delete_Data("DeleteSpot_template_master", sqlParams);
    }



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


      public List<SpotReference> GetAllActive()
        {
            var list = new List<SpotReference>();
            DataTable dt = GetDataSet("GetAllspot_template_master").Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new SpotReference
                {
                    ID = Convert.ToInt32(r["ID"]),
                    Name = r["Name"].ToString()
                });
            }
            return list;
        }



}
