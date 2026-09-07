using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity.Manage;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BLL;

public class SpotTemplateMaster_bal
{
    private readonly SpotTemplateTypeMaster_dal _dal;
    private readonly TemplateTypeMaster_dal typeDal;
    private readonly LanguageMaster_DAL _langDal;

    public SpotTemplateMaster_bal(IConfiguration configuration)
    {
        _dal = new SpotTemplateTypeMaster_dal(configuration);
        typeDal = new TemplateTypeMaster_dal(configuration);
        _langDal = new LanguageMaster_DAL(configuration);
    }


    public List<Add_Spot_Template_Master> GetAll_SpotTemplates_Master_bal()
    {
        List<Add_Spot_Template_Master> list = new List<Add_Spot_Template_Master>();
        DataTable dt = _dal.GetAll_SpotTemplate_Master_DAL();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(MapTemplate(row));
        }

        return list;
    }

    // ======================================
    // 🔹 2. Get Template by ID
    // ======================================
    public Add_Spot_Template_Master Get_SpotTemplate_MasterById_bal(int id)
    {
        DataTable dt = _dal.GetById_SpotTemplate_Master_DAL(id);

        if (dt.Rows.Count > 0)
        {
            return MapTemplate(dt.Rows[0]);
        }

        return null;
    }

    // ======================================
    // 🔹 3. Insert New Template
    // ======================================
    public int AddSpotTemplateMaster_bal(Add_Spot_Template_Master template)
    {
        DataTable dt = _dal.Add_SpotTemplate_Master_DAL(template);

        if (dt.Rows.Count > 0 && dt.Columns.Contains("NewID"))
        {
            return Convert.ToInt32(dt.Rows[0]["NewID"]);
        }

        return 0;
    }

    // ======================================
    // 🔹 4. Update Template
    // ======================================
    public void UpdateSpotTemplateMaster_bal(Add_Spot_Template_Master template)
    {
        _dal.Update_SpotTemplate_Master_DAL(template);
    }

    // ======================================
    // 🔹 5. Delete Template
    // ======================================
    public void DeleteSpotTemplateMaster_bal(int id)
    {
        _dal.Delete_SpotTemplate_Master_DAL(id);
    }

    // ======================================
    // 🔹 Helper to map DataRow to TemplateMaster entity
    // ======================================
    private Add_Spot_Template_Master MapTemplate(DataRow row)
    {
        return new Add_Spot_Template_Master
        {
            ID = Convert.ToInt32(row["ID"]),
            Language_Master_ID = row["Language_Master_ID"] != DBNull.Value ? Convert.ToInt32(row["Language_Master_ID"]) : (int?)null,
            Template_Type_ID = row["Template_Type_ID"] != DBNull.Value ? Convert.ToInt32(row["Template_Type_ID"]) : (int?)null,
            Template_Type = row["Template_Type"].ToString(),
            Name = row["Name"].ToString(),
            Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : (int?)null,
            Created_UserID = row["Created_UserID"] != DBNull.Value ? Convert.ToInt32(row["Created_UserID"]) : (int?)null,
            Created_Date = row["Created_Date"] != DBNull.Value ? Convert.ToDateTime(row["Created_Date"]) : (DateTime?)null,
            Content_Status = row["Content_Status"].ToString(),
            // Updated_UserID = row["Updated_UserID"] != DBNull.Value ? Convert.ToInt32(row["Updated_UserID"]) : (int?)null,
            // Updated_Date = row["Updated_Date"] != DBNull.Value ? Convert.ToDateTime(row["Updated_Date"]) : (DateTime?)null,
            // Published_UserID = row["Published_UserID"] != DBNull.Value ? Convert.ToInt32(row["Published_UserID"]) : (int?)null,
            // Published_Date = row["Published_Date"] != DBNull.Value ? Convert.ToDateTime(row["Published_Date"]) : (DateTime?)null,
            // DeActivated_UserID = row["DeActivated_UserID"] != DBNull.Value ? Convert.ToInt32(row["DeActivated_UserID"]) : (int?)null,
            // DeActivated_Date = row["DeActivated_Date"] != DBNull.Value ? Convert.ToDateTime(row["DeActivated_Date"]) : (DateTime?)null
        };
    }


    public List<TemplateTypeMaster> GetTemplateTypes() => typeDal.GetAll();
    public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();


}
