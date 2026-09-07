using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;


namespace Core_project_BusinessLogic.DAL
{
    public class GeographyMaster_DAL : DBHelper
    {
        public GeographyMaster_DAL(IConfiguration config) : base(config) {}
public List<GeographyMaster> GetAll()
        {
            DataTable dt = GetDataSet("sp_GetAllGeographyMaster").Tables[0];
            List<GeographyMaster> list = new List<GeographyMaster>();

            foreach (DataRow row in dt.Rows)
                list.Add(Map(row));

            return list;
        }

        public GeographyMaster GetById(int id)
        {
            SqlParameter[] prms = { new SqlParameter("@ID", id) };
            DataTable dt = GetDataSet("sp_GetGeographyMasterById", prms).Tables[0];

            if (dt.Rows.Count == 0)
                return null;

            return Map(dt.Rows[0]);
        }

        public int Add(GeographyMaster g)
        {
            SqlParameter[] prms =
            {
                new SqlParameter("@Language_Master_ID", g.Language_Master_ID),
                new SqlParameter("@Country_Name", g.Country_Name),
                new SqlParameter("@Status", g.Status),
                new SqlParameter("@Created_UserID", g.Created_UserID)
            };

           
    int newId = SqlInsertReturnIdentity_withSP("sp_InsertGeographyMaster", "@NewID", prms);

    return newId; // will return -1 if duplicate
            
            
        }

        public void Update(GeographyMaster g)
        {
            SqlParameter[] prms =
            {
                new SqlParameter("@ID", g.ID),
                new SqlParameter("@Language_Master_ID", g.Language_Master_ID),
                new SqlParameter("@Country_Name", g.Country_Name),
                new SqlParameter("@Status", g.Status),
                new SqlParameter("@Updated_UserID", g.Updated_UserID)
            };

            SQLInsert_Update_Delete_Data("sp_UpdateGeographyMaster", prms);
        }

        public void Deactivate(int id, int userId)
        {
            SqlParameter[] prms =
            {
                new SqlParameter("@ID", id),
                new SqlParameter("@DeActivated_UserID", userId)
            };

            SQLInsert_Update_Delete_Data("sp_DeactivateGeographyMaster", prms);
        }

        private GeographyMaster Map(DataRow row)
        {

           /* foreach (DataColumn col in row.Table.Columns)
    {
        System.Diagnostics.Debug.WriteLine(col.ColumnName + " = " + row[col]);
        string idnew= row[col].ToString();
    } */
            return new GeographyMaster
            {
                ID = Convert.ToInt32(row["ID"]),
                Language_Master_ID = row["Language_Master_ID"] as int?,
                Country_Name = row["Country_Name"].ToString(),
                Status = row["Status"] as int?,

                Created_UserID = row["Created_UserID"] as int?,
                Created_Date = row["Created_Date"] as DateTime?,

                Updated_UserID = row["Updated_UserID"] as int?,
                Updated_Date = row["Updated_Date"] as DateTime?,

                Published_UserID = row["Published_UserID"] as int?,
                Published_Date = row["Published_Date"] as DateTime?,

                DeActivated_UserID = row["DeActivated_UserID"] as int?,
                DeActivated_Date = row["DeActivated_Date"] as DateTime?
            };
        }
    }
}
