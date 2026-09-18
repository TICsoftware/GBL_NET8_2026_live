using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class Catalog_Master_DAL : DBHelper
    {
        public Catalog_Master_DAL(IConfiguration configuration) : base(configuration) { }

        public (List<CatalogMasterEntity> Data, int Total) GetPaged(string type, string search, int page, int pageSize)
        {
            SqlParameter[] p =
            {
                new("@MasterType", type),
                new("@Search", string.IsNullOrEmpty(search) ? DBNull.Value : search),
                new("@Page", page),
                new("@PageSize", pageSize)
            };

            DataSet ds = GetDataSet("Catalog_master_GetPaged", p);
            List<CatalogMasterEntity> list = new();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(MapRow(r, type));
            }

            int total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            return (list, total);
        }

        public CatalogMasterEntity GetById(int id, string type)
        {
            SqlParameter[] p =
            {
                new("@ID", id),
                new("@MasterType", type)
            };

            DataTable dt = GetDataSet("Catalog_master_GetById", p).Tables[0];
            if (dt.Rows.Count == 0)
                return null!;

            return MapRow(dt.Rows[0], type);
        }

        public int Insert(CatalogMasterEntity m)
        {
            SqlParameter[] p =
            {
                new("@Name", (object?)m.Name ?? DBNull.Value),
                new("@PageName", (object?)m.PageName ?? DBNull.Value),
                new("@Sequence", m.Sequence.HasValue ? m.Sequence.Value : DBNull.Value),
                new("@Status", m.Status ?? 1),
                new("@Language_Master_Id", m.Language_Master_Id.HasValue ? m.Language_Master_Id.Value : DBNull.Value),
                new("@IndustryId", m.IndustryId.HasValue ? m.IndustryId.Value : DBNull.Value),
                new("@Category_Master_Id", m.Category_Master_Id.HasValue ? m.Category_Master_Id.Value : DBNull.Value),
                new("@Create_UserId", m.Create_UserId.HasValue ? m.Create_UserId.Value : DBNull.Value),
                new("@MasterType", m.MasterType)
            };

            return SqlInsertReturnIdentity_withSP("Catalog_master_Insert", "@NewID", p);
        }

        public void Update(CatalogMasterEntity m)
        {
            SqlParameter[] p =
            {
                new("@ID", m.ID),
                new("@Name", (object?)m.Name ?? DBNull.Value),
                new("@PageName", (object?)m.PageName ?? DBNull.Value),
                new("@Sequence", m.Sequence.HasValue ? m.Sequence.Value : DBNull.Value),
                new("@Language_Master_Id", m.Language_Master_Id.HasValue ? m.Language_Master_Id.Value : DBNull.Value),
                new("@IndustryId", m.IndustryId.HasValue ? m.IndustryId.Value : DBNull.Value),
                new("@Category_Master_Id", m.Category_Master_Id.HasValue ? m.Category_Master_Id.Value : DBNull.Value),
                new("@Update_UserId", m.Update_UserId.HasValue ? m.Update_UserId.Value : DBNull.Value),
                new("@MasterType", m.MasterType)
            };

            SQLInsert_Update_Delete_Data("Catalog_master_Update", p);
        }

        public void Activate(int id, string type, int? updateUserId)
        {
            SqlParameter[] p =
            {
                new("@ID", id),
                new("@MasterType", type),
                new("@Update_UserId", updateUserId.HasValue ? updateUserId.Value : DBNull.Value)
            };

            SQLInsert_Update_Delete_Data("Catalog_master_Activate", p);
        }

        public void Deactivate(int id, string type, int? updateUserId)
        {
            SqlParameter[] p =
            {
                new("@ID", id),
                new("@MasterType", type),
                new("@Update_UserId", updateUserId.HasValue ? updateUserId.Value : DBNull.Value)
            };

            SQLInsert_Update_Delete_Data("Catalog_master_Deactivate", p);
        }

        public void ChangeStatus(int id, string type, int status, int? updateUserId)
        {
            if (status == 1)
                Activate(id, type, updateUserId);
            else
                Deactivate(id, type, updateUserId);
        }

        public void Delete(int id, string type)
        {
            SqlParameter[] p =
            {
                new("@ID", id),
                new("@MasterType", type)
            };

            SQLInsert_Update_Delete_Data("Catalog_master_Delete", p);
        }

        public void UpdateSequence(List<CatalogMasterEntity> list, string type)
        {
            foreach (var item in list)
            {
                SqlParameter[] p =
                {
                    new("@ID", item.ID),
                    new("@Sequence", item.Sequence),
                    new("@MasterType", type)
                };

                SQLInsert_Update_Delete_Data("Catalog_master_UpdateSequence", p);
            }
        }

        public List<CatalogLookupItem> GetLookup(string lookupType)
        {
            SqlParameter[] p =
            {
                new("@LookupType", lookupType)
            };

            DataTable dt = GetDataSet("Catalog_master_GetLookup", p).Tables[0];
            List<CatalogLookupItem> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new CatalogLookupItem
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = r["Name"]?.ToString() ?? string.Empty
                });
            }

            return list;
        }

        private static CatalogMasterEntity MapRow(DataRow r, string type)
        {
            return new CatalogMasterEntity
            {
                ID = Convert.ToInt32(r["ID"]),
                Name = r.Table.Columns.Contains("Name") && r["Name"] != DBNull.Value ? r["Name"].ToString() : null,
                PageName = r.Table.Columns.Contains("PageName") && r["PageName"] != DBNull.Value ? r["PageName"].ToString() : null,
                Sequence = r.Table.Columns.Contains("Sequence") && r["Sequence"] != DBNull.Value ? Convert.ToInt32(r["Sequence"]) : null,
                Status = r.Table.Columns.Contains("Status") && r["Status"] != DBNull.Value ? Convert.ToInt32(r["Status"]) : 1,
                Language_Master_Id = r.Table.Columns.Contains("Language_Master_Id") && r["Language_Master_Id"] != DBNull.Value
                    ? Convert.ToInt32(r["Language_Master_Id"]) : null,
                IndustryId = r.Table.Columns.Contains("IndustryId") && r["IndustryId"] != DBNull.Value
                    ? Convert.ToInt32(r["IndustryId"]) : null,
                Category_Master_Id = r.Table.Columns.Contains("Category_Master_Id") && r["Category_Master_Id"] != DBNull.Value
                    ? Convert.ToInt32(r["Category_Master_Id"]) : null,
                RelatedName = r.Table.Columns.Contains("RelatedName") && r["RelatedName"] != DBNull.Value
                    ? r["RelatedName"].ToString() : null,
                MasterType = type
            };
        }
    }
}
