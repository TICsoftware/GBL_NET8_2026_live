using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class Product_Public_DAL : DBHelper
    {
        public Product_Public_DAL(IConfiguration configuration) : base(configuration) { }

        public Product_Inside_Page GetInside(string pageName, int languageId)
        {
            SqlParameter[] p =
            {
                new("@PageName", string.IsNullOrWhiteSpace(pageName) ? DBNull.Value : pageName.Trim()),
                new("@LanguageId", languageId > 0 ? languageId : 1)
            };

            DataSet ds = GetDataSet("Product_Public_GetInside", p);
            var model = new Product_Inside_Page();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                model.Product = MapProduct(ds.Tables[0].Rows[0]);

            if (ds.Tables.Count > 1)
                model.Packaging = MapPackaging(ds.Tables[1]);

            if (ds.Tables.Count > 2)
                model.Certificates = MapCertificates(ds.Tables[2]);

            if (ds.Tables.Count > 3)
                model.Subcategories = MapLookups(ds.Tables[3]);

            return model;
        }

        private static Product_Inside_Detail MapProduct(DataRow r)
        {
            return new Product_Inside_Detail
            {
                ProductId = ColInt(r, "ProductId") ?? 0,
                ProductName = ColStr(r, "ProductName"),
                Product_pagename = ColStr(r, "Product_pagename"),
                Intro = ColStr(r, "Intro"),
                Content = ColStr(r, "Content"),
                Technical_Overview = ColStr(r, "Technical_Overview"),
                Main_Application = ColStr(r, "Main_Application"),
                BannerUrl = ColStr(r, "BannerUrl"),
                BannerAlt = ColStr(r, "BannerAlt"),
                ThumbnailUrl = ColStr(r, "ThumbnailUrl"),
                ThumbnailAlt = ColStr(r, "ThumbnailAlt"),
                SafetyDataSheetUrl = ColStr(r, "SafetyDataSheetUrl")
            };
        }

        private static List<Product_Inside_Packaging> MapPackaging(DataTable dt)
        {
            var list = new List<Product_Inside_Packaging>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Product_Inside_Packaging
                {
                    Id = ColInt(r, "Id") ?? 0,
                    Name = ColStr(r, "Name"),
                    ThumbnailUrl = ColStr(r, "ThumbnailUrl"),
                    ThumbnailAlt = ColStr(r, "ThumbnailAlt")
                });
            }
            return list;
        }

        private static List<Product_Inside_Certificate> MapCertificates(DataTable dt)
        {
            var list = new List<Product_Inside_Certificate>();
            foreach (DataRow r in dt.Rows)
            {
                var title = ColStr(r, "Title");
                if (string.IsNullOrWhiteSpace(title)) continue;
                list.Add(new Product_Inside_Certificate
                {
                    Title = title,
                    Url = ColStr(r, "Url")
                });
            }
            return list;
        }

        private static List<Product_Public_Lookup> MapLookups(DataTable dt)
        {
            var list = new List<Product_Public_Lookup>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Product_Public_Lookup
                {
                    Id = ColInt(r, "Id") ?? 0,
                    Name = ColStr(r, "Name")
                });
            }
            return list;
        }

        private static string? ColStr(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? r[col].ToString() : null;

        private static int? ColInt(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : null;
    }
}
