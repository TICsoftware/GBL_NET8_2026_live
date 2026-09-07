using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.DAL
{
    public class TemplateMaster_DAL : DBHelper
    {
        public TemplateMaster_DAL(IConfiguration configuration)
            : base(configuration)
        {
        }

        // ======================================
        // 🔹 Get All TemplateMaster Records
        // ======================================
        public DataTable GetAll_TemplateMaster_DAL()
        {
            SqlParameter[] sqlParams = new SqlParameter[0]; // no input params
            return GetDataSet("sp_GetAllTemplateMasters", sqlParams).Tables[0];
        }



        // ======================================
        // 🔹 Get TemplateMaster by ID
        // ======================================
        public DataTable GetById_TemplateMaster_DAL(int id)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@ID", id)
            };

            return GetDataSet("sp_GetTemplateMasterById", sqlParams).Tables[0];
        }

        // ======================================
        // 🔹 Insert New TemplateMaster
        // ======================================
        public DataTable Add_TemplateMaster_DAL(TemplateMaster t)
        {
            SqlParameter[] sqlParams =
            {
              //  new SqlParameter("@ID", 0),
                new SqlParameter("@Language_Master_ID", t.Language_Master_ID ),
                new SqlParameter("@Name", t.Name.Trim() ),
                new SqlParameter("@Status", t.Status) ,
                new SqlParameter("@Created_UserID", t.Created_UserID )
            };

            return GetDataSet("sp_InsertTemplateMaster", sqlParams).Tables[0];
        }

        // ======================================
        // 🔹 Update TemplateMaster
        // ======================================
        public void Update_TemplateMaster_DAL(TemplateMaster t)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@ID", t.ID),
                new SqlParameter("@Language_Master_ID", t.Language_Master_ID ),
                new SqlParameter("@Name", t.Name?.Trim() ),
                new SqlParameter("@Status", t.Status ),
                new SqlParameter("@Updated_UserID", t.Updated_UserID )
            };

            SQLInsert_Update_Delete_Data("sp_UpdateTemplateMaster", sqlParams);
        }
        // ======================================
        // 🔹 Delete TemplateMaster
        // ======================================
        public void Delete_TemplateMaster_DAL(int id)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@ID", id)
            };

            SQLInsert_Update_Delete_Data("sp_DeleteTemplateMaster", sqlParams);
        }
    }
}
