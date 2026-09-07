using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.DAL
{
    public class LanguageMaster_DAL : DBHelper
    {
        public LanguageMaster_DAL(IConfiguration config) : base(config) { }

        public List<LanguageMaster> GetAllLanguages()
        {
            DataTable dt = GetDataSet("sp_GetAllLanguageMaster").Tables[0];
            var list = new List<LanguageMaster>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(Map(row));
            }
            return list;
        }

        public LanguageMaster GetLanguageById(int id)
        {
            SqlParameter[] prms =
            {
                new SqlParameter("@ID", id)
            };

            DataTable dt = GetDataSet("sp_GetLanguageMasterById", prms).Tables[0];

            if (dt.Rows.Count == 0)
                return null;

            return Map(dt.Rows[0]);
        }
             public List<LanguageMaster> GetAllActive()
        {
            var list = new List<LanguageMaster>();
            DataTable dt = GetDataSet("sp_LanguageMaster_List").Tables[0];
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new LanguageMaster
                {
                    ID = Convert.ToInt32(r["ID"]),
                    Language_Name = r["Language_Name"].ToString()
                });
            }
            return list;
        }
        public int AddLanguage(LanguageMaster lm)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@Language_Name", lm.Language_Name),
                new SqlParameter("@Status", lm.Status),
                new SqlParameter("@Language_Sequence", lm.Language_Sequence),
                new SqlParameter("@Created_UserID", "1")
            };

            return SqlInsertReturnIdentity_withSP("sp_InsertLanguageMaster", "@NewID", p);
        }

        public void UpdateLanguage(LanguageMaster lm)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", lm.ID),
                new SqlParameter("@Language_Name", lm.Language_Name),
                new SqlParameter("@Status", lm.Status),
                new SqlParameter("@Language_Sequence", lm.Language_Sequence),
                new SqlParameter("@Updated_UserID", "1")
            };

            SQLInsert_Update_Delete_Data("sp_UpdateLanguageMaster", p);
        }

        public void DeactivateLanguage(int id, int userId)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", id),
                new SqlParameter("@Updated_UserID", userId)
            };
            SQLInsert_Update_Delete_Data("sp_DeactivateLanguageMaster", p);
        }

        private LanguageMaster Map(DataRow row)
        {
            return new LanguageMaster
            {
                ID = Convert.ToInt32(row["ID"]),
                Language_Name = row["Language_Name"].ToString(),
                Status = row["Status"] as int?,
                Language_Sequence = row["Language_Sequence"] as int?,
                Created_UserID = row["Created_UserID"] as int?,
                Created_Date = row["Created_Date"] as DateTime?,
                Updated_UserID = row["Updated_UserID"] as int?,
                Updated_Date = row["Updated_Date"] as DateTime?,
                Published_UserID = 1 as int?,
                Published_Date = row["Published_Date"] as DateTime?
            };
        }
    }
}
