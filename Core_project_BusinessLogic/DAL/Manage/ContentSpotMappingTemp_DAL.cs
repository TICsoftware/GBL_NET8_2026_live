using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{

public class ContentSpotMappingTemp_DAL : DBHelper
{
    public ContentSpotMappingTemp_DAL(IConfiguration config) : base(config) { }

    public void Insert(int contId, int spotId, int spotType, int userId)
    {
        SqlParameter[] p =
        {
            new("@cont_id", contId),
            new("@spot_id", spotId),
            new("@spot_type", spotType),
            new("@created_user", userId)
        };
        SQLInsert_Update_Delete_Data("InsertContentSpotMapping", p);
    }

    public DataTable GetAll(int contId,string conttempid)
    {
        return GetDataSet("GetContentSpotMappings",
            new[] { new SqlParameter("@cont_id", contId) ,
              new("@cont_temp_id", conttempid)
                }).Tables[0];
    }

    public void Swap(int id1, int id2)
    {
        SQLInsert_Update_Delete_Data(
            "SwapContentSpotSequence",
            new[]
            {
                new SqlParameter("@currentId", id1),
                new SqlParameter("@swapWithId", id2)
            });
    }


    public void UpdateSequence(int contSpotId, int newSequence)
{
    SqlParameter[] param =
    {
        new SqlParameter("@cont_spot_temp_id", contSpotId),
        new SqlParameter("@new_sequence", newSequence)
    };

    SQLInsert_Update_Delete_Data("UpdateContentSpotSequence", param);
}
public void AddSpot(int contId,string cont_tempid, int spotId, int spotType)
{
    SqlParameter[] param =
    {
       
        new SqlParameter("@cont_id", (object?)contId ?? 0),
          new SqlParameter("@cont_tempid", (object?)cont_tempid ?? DBNull.Value),
        new SqlParameter("@spot_id", spotId),
        new SqlParameter("@spot_type", spotType)
    };

    SQLInsert_Update_Delete_Data("InsertContentSpotMapping", param);
}
public void DeleteMapping(int contSpotId, int contId)
{
    SqlParameter[] param =
    {
        new SqlParameter("@cont_spot_id", contSpotId),
        new SqlParameter("@cont_id", contId)
    };

    SQLInsert_Update_Delete_Data("DeleteContentSpotMapping", param);
}

 public void Deleteall(int? contId, string tempId)
    {
        SqlParameter[] param =
        {
            new SqlParameter("@cont_id",
                contId.HasValue ? contId.Value : (object)DBNull.Value),

            new SqlParameter("@cont_tempid",
                string.IsNullOrEmpty(tempId) ? (object)DBNull.Value : tempId)
        };

        SQLInsert_Update_Delete_Data(
            "DeleteContentSpotMapping_all",
            param
        );
    }
}

}