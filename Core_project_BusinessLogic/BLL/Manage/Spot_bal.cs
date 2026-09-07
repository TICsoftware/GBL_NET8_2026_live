using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity.Manage;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BLL;

public class Spot_bal
{
    private readonly Spot_dal _dal;
    private readonly GeographyMaster_DAL _geogDal;
    private readonly LanguageMaster_DAL _langDal;
    private readonly SpotTemplateTypeMaster_dal _spotref;

    public Spot_bal(IConfiguration configuration)
    {
        _dal = new Spot_dal(configuration);
        _langDal = new LanguageMaster_DAL(configuration);
        _geogDal = new GeographyMaster_DAL(configuration);
        _spotref = new SpotTemplateTypeMaster_dal(configuration);
    }


    public List<Add_Spot_RHS> GetAll_Spot_bal()
    {
        List<Add_Spot_RHS> list = new List<Add_Spot_RHS>();
        DataTable dt = _dal.GetAll_Spot_DAL();

        foreach (DataRow row in dt.Rows)
        {
            list.Add(MapTemplate(row));
        }

        return list;
    }

    // ======================================
    // 🔹 2. Get Template by ID
    // ======================================
    public Add_Spot_RHS GetSpot_ById_bal(int id)
    {
        DataTable dt = _dal.GetSpot_ById_DAL(id);

        if (dt.Rows.Count > 0)
        {
            return MapTemplate(dt.Rows[0]);
        }

        return null;
    }

    // ======================================
    // 🔹 3. Insert New Template
    // ======================================
    public int Add_Spot_bal(Add_Spot_RHS template)
    {
        DataTable dt = _dal.Add_Spot_DAL(template);

        if (dt.Rows.Count > 0 && dt.Columns.Contains("NewID"))
        {
            return Convert.ToInt32(dt.Rows[0]["NewID"]);
        }

        return 0;
    }

    // ======================================
    // 🔹 4. Update Template
    // ======================================
    public void UpdateSpotTemplateMaster_bal(Add_Spot_RHS template)
    {
        _dal.Update_Spot_DAL(template);
    }

    // ======================================
    // 🔹 5. Delete Template
    // ======================================
    public void DeleteSpot_bal(int id)
    {
        _dal.DeleteSpot_DAL(id);
    }

    // ======================================
    // 🔹 Helper to map DataRow to TemplateMaster entity
    // ======================================
    private Add_Spot_RHS MapTemplate(DataRow row)
    {
        return new Add_Spot_RHS
        {
            ID = Convert.ToInt32(row["ID"]),
            Language_Master_ID = row["Language_Master_ID"] != DBNull.Value ? Convert.ToInt32(row["Language_Master_ID"]) : (int?)null,
            Geography_ID = row["Geography_ID"] != DBNull.Value ? Convert.ToInt32(row["Geography_ID"]) : (int?)null,
            spot_template_master_id = row["spot_template_master_id"] != DBNull.Value ? Convert.ToInt32(row["spot_template_master_id"]) : (int?)null,

            Title = row["Title"].ToString(),
            Description = row["Description"].ToString(),
            Thumbnail_Img_Media_Id = row["Thumbnail_Img_Media_Id"] != DBNull.Value ? Convert.ToInt32(row["Thumbnail_Img_Media_Id"]) : (int?)null,
            Thumbnail_Img = row["Thumbnail_Img"].ToString(),
            thumbnail_Alt_Text = row["thumbnail_Alt_Text"].ToString(),
            background_Img_Media_Id = row["background_Img_Media_Id"] != DBNull.Value ? Convert.ToInt32(row["background_Img_Media_Id"]) : (int?)null,
            background_Img = row["background_Img"].ToString(),
            background_Alt_Text = row["background_Alt_Text"].ToString(),
            icon_Img_Media_Id = row["icon_Img_Media_Id"] != DBNull.Value ? Convert.ToInt32(row["icon_Img_Media_Id"]) : (int?)null,
            icon_Img = row["icon_Img"].ToString(),
            icon_Alt_Text = row["icon_Alt_Text"].ToString(),
            Spot_Intro = row["Spot_Intro"].ToString(),
            Spot_content = row["Spot_content"].ToString(),
            Files_Media_Id = row["Files_Media_Id"] != DBNull.Value ? Convert.ToInt32(row["Files_Media_Id"]) : (int?)null,
            Files = row["Files"].ToString(),
            External_Url = row["External_Url"].ToString(),
            Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : (int?)null,
            Isexternal = row["Isexternal"] != DBNull.Value ? Convert.ToBoolean(row["Isexternal"]) : (bool?)null,
            Created_UserID = row["Created_UserID"] != DBNull.Value ? Convert.ToInt32(row["Created_UserID"]) : (int?)null,
            Created_Date = row["Created_Date"] != DBNull.Value ? Convert.ToDateTime(row["Created_Date"]) : (DateTime?)null,
            Content_Status = row["Content_Status"].ToString(),
            sequence = row["sequence"] != DBNull.Value ? Convert.ToInt32(row["sequence"]) : (int?)null,
            SpotTemplate = row["SpotTemplate"].ToString(),
        };
    }



    public List<SpotReference> GetAllSpotReference() => _spotref.GetAllActive();
    public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();
    public List<GeographyMaster> GetGeography() => _geogDal.GetAll();
    
    

}
