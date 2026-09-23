using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class Product_Master_DAL : DBHelper
    {
        public Product_Master_DAL(IConfiguration configuration) : base(configuration) { }

        public (List<Product_Master_Entity> Data, int Total) GetPaged(string search, int page, int pageSize)
        {
            SqlParameter[] p =
            {
                new("@Search", string.IsNullOrEmpty(search) ? DBNull.Value : search),
                new("@Page", page),
                new("@PageSize", pageSize)
            };

            DataSet ds = GetDataSet("Product_Master_GetPaged", p);
            List<Product_Master_Entity> list = new();

            foreach (DataRow r in ds.Tables[0].Rows)
                list.Add(MapRow(r));

            int total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            return (list, total);
        }

        public Product_Master_Entity GetById(int id)
        {
            SqlParameter[] p = { new("@ID", id) };
            DataSet ds = GetDataSet("Product_Master_GetById", p);
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null!;

            var entity = MapRow(ds.Tables[0].Rows[0]);
            entity.IndustryIds = ReadIdList(ds, 1);
            entity.ApplicationIds = ReadIdList(ds, 2);
            entity.SubcategoryIds = ReadIdList(ds, 3);
            entity.PackagingIds = ReadIdList(ds, 4);
            return entity;
        }

        public bool NameExists(string name, int? languageId, int excludeId)
        {
            SqlParameter[] p =
            {
                new("@ProductName", name ?? string.Empty),
                new("@Language_Master_Id", languageId.HasValue ? languageId.Value : DBNull.Value),
                new("@ProductId", excludeId)
            };
            DataTable dt = GetDataSet("Product_Master_NameExists", p).Tables[0];
            return dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["IsExists"]) == 1;
        }

        public int Insert(Product_Master_Entity m)
        {
            SqlParameter[] p =
            {
                new("@ProductName", (object?)m.ProductName ?? DBNull.Value),
                new("@Product_pagename", (object?)m.Product_pagename ?? DBNull.Value),
                new("@Intro", (object?)m.Intro ?? DBNull.Value),
                new("@Content", (object?)m.Content ?? DBNull.Value),
                new("@Technical_Overview", (object?)m.Technical_Overview ?? DBNull.Value),
                new("@Thumbnail_Image_media_id", m.Thumbnail_Image_media_id.HasValue ? m.Thumbnail_Image_media_id.Value : DBNull.Value),
                new("@Banner_Image_media_id", m.Banner_Image_media_id.HasValue ? m.Banner_Image_media_id.Value : DBNull.Value),
                new("@SafetyDataSheet_media_id", m.SafetyDataSheet_media_id.HasValue ? m.SafetyDataSheet_media_id.Value : DBNull.Value),
                new("@Language_Master_Id", m.Language_Master_Id.HasValue ? m.Language_Master_Id.Value : DBNull.Value),
                new("@Sequence", m.Sequence.HasValue ? m.Sequence.Value : DBNull.Value),
                new("@Status", m.Status ?? 1),
                new("@Create_UserId", m.Create_UserId.HasValue ? m.Create_UserId.Value : DBNull.Value)
            };

            return SqlInsertReturnIdentity_withSP("Product_Master_Insert", "@NewID", p);
        }

        public void Update(Product_Master_Entity m)
        {
            SqlParameter[] p =
            {
                new("@ProductId", m.ProductId),
                new("@ProductName", (object?)m.ProductName ?? DBNull.Value),
                new("@Product_pagename", (object?)m.Product_pagename ?? DBNull.Value),
                new("@Intro", (object?)m.Intro ?? DBNull.Value),
                new("@Content", (object?)m.Content ?? DBNull.Value),
                new("@Technical_Overview", (object?)m.Technical_Overview ?? DBNull.Value),
                new("@Thumbnail_Image_media_id", m.Thumbnail_Image_media_id.HasValue ? m.Thumbnail_Image_media_id.Value : DBNull.Value),
                new("@Banner_Image_media_id", m.Banner_Image_media_id.HasValue ? m.Banner_Image_media_id.Value : DBNull.Value),
                new("@SafetyDataSheet_media_id", m.SafetyDataSheet_media_id.HasValue ? m.SafetyDataSheet_media_id.Value : DBNull.Value),
                new("@Language_Master_Id", m.Language_Master_Id.HasValue ? m.Language_Master_Id.Value : DBNull.Value),
                new("@Sequence", m.Sequence.HasValue ? m.Sequence.Value : DBNull.Value),
                new("@Update_UserId", m.Update_UserId.HasValue ? m.Update_UserId.Value : DBNull.Value)
            };

            SQLInsert_Update_Delete_Data("Product_Master_Update", p);
        }

        public void SaveMappings(int productId, Product_Master_Entity m, int? userId)
        {
            SqlParameter[] p =
            {
                new("@ProductId", productId),
                new("@IndustryIds", ToCsv(m.IndustryIds)),
                new("@ApplicationIds", ToCsv(m.ApplicationIds)),
                new("@SubcategoryIds", ToCsv(m.SubcategoryIds)),
                new("@PackagingIds", ToCsv(m.PackagingIds)),
                new("@Create_UserId", userId.HasValue ? userId.Value : DBNull.Value)
            };
            SQLInsert_Update_Delete_Data("Product_Master_SaveMappings", p);
        }

        public void ChangeStatus(int id, int status, int? updateUserId)
        {
            SqlParameter[] p =
            {
                new("@ID", id),
                new("@Update_UserId", updateUserId.HasValue ? updateUserId.Value : DBNull.Value)
            };
            SQLInsert_Update_Delete_Data(status == 1 ? "Product_Master_Activate" : "Product_Master_Deactivate", p);
        }

        public void Delete(int id)
        {
            SqlParameter[] p = { new("@ID", id) };
            SQLInsert_Update_Delete_Data("Product_Master_Delete", p);
        }

        public void UpdateSequence(List<Product_Master_Entity> list)
        {
            foreach (var item in list)
            {
                SqlParameter[] p =
                {
                    new("@ID", item.ProductId),
                    new("@Sequence", item.Sequence)
                };
                SQLInsert_Update_Delete_Data("Product_Master_UpdateSequence", p);
            }
        }

        public List<ProductLookupItem> GetActiveIndustries() =>
            ReadLookups("Product_Master_GetIndustries");

        public List<ProductLookupItem> GetActiveApplications() =>
            ReadLookups("Product_Master_GetApplications");

        public List<ProductLookupItem> GetActivePackaging() =>
            ReadLookups("Product_Packaging_Master_GetActive");

        public List<ProductLookupItem> GetSubcategoriesByIndustries(List<int> industryIds)
        {
            SqlParameter[] p = { new("@IndustryIds", ToCsv(industryIds)) };
            DataTable dt = GetDataSet("Product_Master_GetSubcategoriesByIndustries", p).Tables[0];
            return MapLookups(dt);
        }

        private List<ProductLookupItem> ReadLookups(string spName)
        {
            DataTable dt = GetDataSet(spName).Tables[0];
            return MapLookups(dt);
        }

        private static List<ProductLookupItem> MapLookups(DataTable dt)
        {
            var list = new List<ProductLookupItem>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new ProductLookupItem
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = ColStr(r, "Name")
                });
            }
            return list;
        }

        private static Product_Master_Entity MapRow(DataRow r)
        {
            return new Product_Master_Entity
            {
                ProductId = Convert.ToInt32(r["ProductId"]),
                ProductName = ColStr(r, "ProductName"),
                Product_pagename = ColStr(r, "Product_pagename"),
                Sequence = ColInt(r, "Sequence"),
                Status = ColInt(r, "Status") ?? 1,
                Language_Master_Id = ColInt(r, "Language_Master_Id"),
                LanguageName = ColStr(r, "LanguageName"),
                Thumbnail_Image_media_id = ColInt(r, "Thumbnail_Image_media_id"),
                Banner_Image_media_id = ColInt(r, "Banner_Image_media_id"),
                SafetyDataSheet_media_id = ColInt(r, "SafetyDataSheet_media_id"),
                Thumbnail_Image_Url = ColStr(r, "Thumbnail_Image_Url"),
                Banner_Image_Url = ColStr(r, "Banner_Image_Url"),
                SafetyDataSheet_Url = ColStr(r, "SafetyDataSheet_Url"),
                Intro = ColStr(r, "Intro"),
                Content = ColStr(r, "Content"),
                Technical_Overview = ColStr(r, "Technical_Overview")
            };
        }

        private static List<int> ReadIdList(DataSet ds, int tableIndex)
        {
            var list = new List<int>();
            if (ds.Tables.Count <= tableIndex) return list;
            foreach (DataRow r in ds.Tables[tableIndex].Rows)
            {
                if (r["Id"] != DBNull.Value)
                    list.Add(Convert.ToInt32(r["Id"]));
            }
            return list;
        }

        private static object ToCsv(List<int>? ids) =>
            ids == null || ids.Count == 0 ? DBNull.Value : string.Join(",", ids);

        private static string? ColStr(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? r[col].ToString() : null;

        private static int? ColInt(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : null;
    }
}
