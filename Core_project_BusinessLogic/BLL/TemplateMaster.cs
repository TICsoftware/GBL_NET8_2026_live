using System;
using System.Collections.Generic;
using System.Data;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.BAL
{
    public class TemplateMaster_BAL
    {
        private readonly TemplateMaster_DAL _dal;
        private readonly LanguageMaster_DAL _langDal;
        public TemplateMaster_BAL(IConfiguration configuration)
        {
            _dal = new TemplateMaster_DAL(configuration);
            _langDal = new LanguageMaster_DAL(configuration);
        }

        // ======================================
        // 🔹 1. Get All Templates
        // ======================================
        public List<TemplateMaster> GetAllTemplates()
        {
            List<TemplateMaster> list = new List<TemplateMaster>();
            DataTable dt = _dal.GetAll_TemplateMaster_DAL();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapTemplate(row));
            }

            return list;
        }

        // ======================================
        // 🔹 2. Get Template by ID
        // ======================================
        public TemplateMaster GetTemplateById(int id)
        {
            TemplateMaster objtempmast = new();
            DataTable dt = _dal.GetById_TemplateMaster_DAL(id);

            if (dt.Rows.Count > 0)
            {
                return MapTemplate(dt.Rows[0]);
            }

            return objtempmast;
        }

        // ======================================
        // 🔹 3. Insert New Template
        // ======================================
        public int AddTemplate(TemplateMaster template)
        {
            DataTable dt = _dal.Add_TemplateMaster_DAL(template);

            if (dt.Rows.Count > 0)//dt.Columns.Contains("NewID")
            {
                //return Convert.ToInt32(dt.Rows[0]["NewID"]);
                return 1;
            }

            else
            {
                return 0;

            }

        }

        // ======================================
        // 🔹 4. Update Template
        // ======================================
        public void UpdateTemplate(TemplateMaster template)
        {
            _dal.Update_TemplateMaster_DAL(template);
        }

        // ======================================
        // 🔹 5. Delete Template
        // ======================================
        public void DeleteTemplate(int id)
        {
            _dal.Delete_TemplateMaster_DAL(id);
        }

        // ======================================
        // 🔹 Helper to map DataRow to TemplateMaster entity
        // ======================================
        private TemplateMaster MapTemplate(DataRow row)
        {
            return new TemplateMaster
            {
                ID = Convert.ToInt32(row["ID"]),
                Language_Master_ID = row["Language_Master_ID"] != DBNull.Value ? Convert.ToInt32(row["Language_Master_ID"]) : (int?)null,
                Name = row["Name"].ToString(),
                Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : (int?)null,
                Created_UserID = row["Created_UserID"] != DBNull.Value ? Convert.ToInt32(row["Created_UserID"]) : (int?)null,
                Created_Date = row["Created_Date"] != DBNull.Value ? Convert.ToDateTime(row["Created_Date"]) : (DateTime?)null,
                Updated_UserID = row["Updated_UserID"] != DBNull.Value ? Convert.ToInt32(row["Updated_UserID"]) : (int?)null,
                Updated_Date = row["Updated_Date"] != DBNull.Value ? Convert.ToDateTime(row["Updated_Date"]) : (DateTime?)null,
                Published_UserID = row["Published_UserID"] != DBNull.Value ? Convert.ToInt32(row["Published_UserID"]) : (int?)null,
                Published_Date = row["Published_Date"] != DBNull.Value ? Convert.ToDateTime(row["Published_Date"]) : (DateTime?)null,
                DeActivated_UserID = row["DeActivated_UserID"] != DBNull.Value ? Convert.ToInt32(row["DeActivated_UserID"]) : (int?)null,
                DeActivated_Date = row["DeActivated_Date"] != DBNull.Value ? Convert.ToDateTime(row["DeActivated_Date"]) : (DateTime?)null
            };
        }

        public List<LanguageMaster> GetLanguages() => _langDal.GetAllActive();
    }
}
