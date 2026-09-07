using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class MainBlockmasterFields_DAL : DBHelper
    {
        public MainBlockmasterFields_DAL(IConfiguration config) : base(config) { }
        public List<int> GetSelectedFields(int componentMasterId)
        {
            SqlParameter[] p =
            {
                new("@main_block_master_id", componentMasterId)
            };

            DataTable dt = GetDataSet("GetmainblockmasterFields", p).Tables[0];

            if (dt == null || dt.Rows.Count == 0)
                return new List<int>();

            return dt.AsEnumerable()
                .Select(r => Convert.ToInt32(r["main_block_fields_id"]))
                .ToList();
        }
        public List<BlockMasterField> GetAllFields()
        {
            SqlParameter[] p = new SqlParameter[0];
            DataTable dt = GetDataSet("GetAllmainBlockFields_assign", p).Tables[0];

            if (dt == null || dt.Rows.Count == 0)
                return new List<BlockMasterField>();

            return dt.AsEnumerable()
                .Select(r => new BlockMasterField
                {
                    id = Convert.ToInt32(r["id"]),
                    name = r["name"].ToString()
                }).ToList();
        }

        public void Save(int main_block_master_id, string csvFieldIds, int createdUserId)
        {
            SqlParameter[] p =
            {
            new("@main_block_master_id", main_block_master_id),
            new("@field_ids", csvFieldIds),
            new("@Created_UserID", createdUserId)
        };

            SQLInsert_Update_Delete_Data("SaveBlockMasterFields", p);
        }
    }
}
