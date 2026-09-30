using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class Product_Packaging_Master_DAL : DBHelper
    {
        public Product_Packaging_Master_DAL(IConfiguration configuration) : base(configuration) { }

        public (List<Product_Packaging_Master_Entity> Data, int Total) GetPaged(string search, int page, int pageSize)
        {
            SqlParameter[] p =
            {
                new("@Search", string.IsNullOrEmpty(search) ? DBNull.Value : search),
                new("@Page", page),
                new("@PageSize", pageSize)
            };

            DataSet ds = GetDataSet("Product_Packaging_Master_GetPaged", p);
            List<Product_Packaging_Master_Entity> list = new();

            foreach (DataRow r in ds.Tables[0].Rows)
                list.Add(MapRow(r));

            int total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            return (list, total);
        }

        public Product_Packaging_Master_Entity GetById(int id)
        {
            SqlParameter[] p = { new("@ID", id) };
            DataTable dt = GetDataSet("Product_Packaging_Master_GetById", p).Tables[0];
            if (dt.Rows.Count == 0)
                return null!;
            return MapRow(dt.Rows[0]);
        }

        public bool NameExists(string name, int? languageId, int excludeId)
        {
            SqlParameter[] p =
            {
                new("@Name", name ?? string.Empty),
                new("@Language_Master_Id", languageId.HasValue ? languageId.Value : DBNull.Value),
                new("@ID", excludeId)
            };
            DataTable dt = GetDataSet("Product_Packaging_Master_NameExists", p).Tables[0];
            return dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["IsExists"]) == 1;
        }

        public int Insert(Product_Packaging_Master_Entity m)
        {
            SqlParameter[] p =
            {
                new("@Name", (object?)m.Name ?? DBNull.Value),
                new("@Language_Master_Id", m.Language_Master_Id.HasValue ? m.Language_Master_Id.Value : DBNull.Value),
                new("@Thumbnailimage_Id", (object?)m.Thumbnailimage_Id ?? string.Empty),
                new("@Thumbnailimage_alt", (object?)m.Thumbnailimage_alt ?? string.Empty),
                new("@Sequence", m.Sequence.HasValue ? m.Sequence.Value : DBNull.Value),
                new("@Status", m.Status ?? 1),
                new("@Create_UserId", m.Create_UserId.HasValue ? m.Create_UserId.Value : DBNull.Value)
            };

            return SqlInsertReturnIdentity_withSP("Product_Packaging_Master_Insert", "@NewID", p);
        }

        public void Update(Product_Packaging_Master_Entity m)
        {
            SqlParameter[] p =
            {
                new("@ID", m.product_packaging_MasterId),
                new("@Name", (object?)m.Name ?? DBNull.Value),
                new("@Language_Master_Id", m.Language_Master_Id.HasValue ? m.Language_Master_Id.Value : DBNull.Value),
                new("@Thumbnailimage_Id", (object?)m.Thumbnailimage_Id ?? string.Empty),
                new("@Thumbnailimage_alt", (object?)m.Thumbnailimage_alt ?? string.Empty),
                new("@Sequence", m.Sequence.HasValue ? m.Sequence.Value : DBNull.Value),
                new("@Update_UserId", m.Update_UserId.HasValue ? m.Update_UserId.Value : DBNull.Value)
            };

            SQLInsert_Update_Delete_Data("Product_Packaging_Master_Update", p);
        }

        public void ChangeStatus(int id, int status, int? updateUserId)
        {
            SqlParameter[] p =
            {
                new("@ID", id),
                new("@Update_UserId", updateUserId.HasValue ? updateUserId.Value : DBNull.Value)
            };
            SQLInsert_Update_Delete_Data(
                status == 1 ? "Product_Packaging_Master_Activate" : "Product_Packaging_Master_Deactivate",
                p);
        }

        public void Delete(int id)
        {
            SqlParameter[] p = { new("@ID", id) };
            SQLInsert_Update_Delete_Data("Product_Packaging_Master_Delete", p);
        }

        public void UpdateSequence(List<Product_Packaging_Master_Entity> list)
        {
            foreach (var item in list)
            {
                SqlParameter[] p =
                {
                    new("@ID", item.product_packaging_MasterId),
                    new("@Sequence", item.Sequence)
                };
                SQLInsert_Update_Delete_Data("Product_Packaging_Master_UpdateSequence", p);
            }
        }

        private static Product_Packaging_Master_Entity MapRow(DataRow r)
        {
            return new Product_Packaging_Master_Entity
            {
                product_packaging_MasterId = Convert.ToInt32(r["product_packaging_MasterId"]),
                Name = ColStr(r, "Name"),
                Language_Master_Id = ColInt(r, "Language_Master_Id"),
                LanguageName = ColStr(r, "LanguageName"),
                Thumbnailimage_Id = ColStr(r, "Thumbnailimage_Id"),
                Thumbnail_Image_Url = ColStr(r, "Thumbnail_Image_Url"),
                Thumbnailimage_alt = ColStr(r, "Thumbnailimage_alt"),
                Sequence = ColInt(r, "Sequence"),
                Status = ColInt(r, "Status") ?? 1
            };
        }

        private static string? ColStr(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? r[col].ToString() : null;

        private static int? ColInt(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : null;
    }
}
