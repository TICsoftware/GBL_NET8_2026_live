using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;


namespace Core_project_BusinessLogic.DAL
{
    public class TagMaster_DAL : DBHelper
    {
        public TagMaster_DAL(IConfiguration config) : base(config) { }
        public DataSet GetAll(int status, int language_id)
        {
            DataSet ds = GetDataSet("sp_GetAllTagMaster","@status", status.ToString(),"@language_id", language_id.ToString());     

            return ds;
        }

        public DataTable GetById(int id)
        {
            SqlParameter[] prms = { new SqlParameter("@ID", id) };
            DataTable dt = GetDataSet("sp_GetTagMasterById", prms).Tables[0]; 
            return dt;
        }

        public DataTable Add(TagMaster g, int userid)
        {
            SqlParameter[] prms =
            {
                new SqlParameter("@Tag_name", g.Tag_name), 
                new SqlParameter("@Language_Master_ID", g.Language_Master_ID), 
                new SqlParameter("@Status", g.Status),
                new SqlParameter("@UserID", userid)
            }; 
            return GetDataSet("sp_InsertTagMaster",  prms).Tables[0]; 

        }

        public void Update(TagMaster g, int userid)
        {
            SqlParameter[] prms =
            {
                new SqlParameter("@ID", g.ID),
                new SqlParameter("@Tag_name", g.Tag_name), 
                new SqlParameter("@Language_Master_ID", g.Language_Master_ID), 
                new SqlParameter("@Status", g.Status),
                new SqlParameter("@UserID", userid)
            };

            SQLInsert_Update_Delete_Data("sp_UpdateTagMaster", prms);
        }

        public void UpdateTagStatus_DAL(int id, int status, int userId)
        {
            SqlParameter[] prms =
            {
                new SqlParameter("@ID", id),
                 new SqlParameter("@status", status.ToString()),
                new SqlParameter("@UserID", userId)
            };

            SQLInsert_Update_Delete_Data("sp_UpdateTagMasterStatus", prms);
        }

        
    }
}
