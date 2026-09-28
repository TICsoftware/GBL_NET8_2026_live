using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class Product_Public_DAL : DBHelper
    {
        public Product_Public_DAL(IConfiguration configuration) : base(configuration) { }

        public Product_Public_Page GetIndex(string pageName, int languageId, Product_Public_Filter filter)
        {
            filter ??= new Product_Public_Filter();
            NormalizeFilter(filter, languageId);

            SqlParameter[] p =
            {
                new("@PageName", string.IsNullOrWhiteSpace(pageName) ? DBNull.Value : pageName.Trim()),
                new("@LanguageId", filter.LanguageId),
                new("@IndustryIds", string.IsNullOrWhiteSpace(filter.IndustryIds) ? DBNull.Value : filter.IndustryIds),
                new("@ApplicationIds", string.IsNullOrWhiteSpace(filter.ApplicationIds) ? DBNull.Value : filter.ApplicationIds),
                new("@CategoryIds", string.IsNullOrWhiteSpace(filter.CategoryIds) ? DBNull.Value : filter.CategoryIds),
                new("@Page", filter.PageNumber),
                new("@PageSize", filter.PageSize)
            };

            DataSet ds = GetDataSet("Product_Public_GetIndex", p);
            var page = new Product_Public_Page
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                SelectedIndustryIds = ParseIds(filter.IndustryIds),
                SelectedApplicationIds = ParseIds(filter.ApplicationIds),
                SelectedCategoryIds = ParseIds(filter.CategoryIds)
            };

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                page.Content = MapContent(ds.Tables[0].Rows[0]);

            if (ds.Tables.Count > 1)
                page.Products = MapListingProducts(ds.Tables[1], ds.Tables.Count > 6 ? ds.Tables[6] : null);

            if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                page.TotalRecords = Convert.ToInt32(ds.Tables[2].Rows[0]["TotalCount"]);

            if (ds.Tables.Count > 3)
                page.IndustryFilters = MapLookups(ds.Tables[3]);
            if (ds.Tables.Count > 4)
                page.ApplicationFilters = MapLookups(ds.Tables[4]);
            if (ds.Tables.Count > 5)
                page.CategoryFilters = MapLookups(ds.Tables[5]);

            return page;
        }

        public Product_Public_Page GetPaged(Product_Public_Filter filter)
        {
            filter ??= new Product_Public_Filter();
            NormalizeFilter(filter, filter.LanguageId);

            SqlParameter[] p =
            {
                new("@IndustryIds", string.IsNullOrWhiteSpace(filter.IndustryIds) ? DBNull.Value : filter.IndustryIds),
                new("@ApplicationIds", string.IsNullOrWhiteSpace(filter.ApplicationIds) ? DBNull.Value : filter.ApplicationIds),
                new("@CategoryIds", string.IsNullOrWhiteSpace(filter.CategoryIds) ? DBNull.Value : filter.CategoryIds),
                new("@Page", filter.PageNumber),
                new("@PageSize", filter.PageSize),
                new("@LanguageId", filter.LanguageId)
            };

            DataSet ds = GetDataSet("Product_Public_GetPaged", p);
            var page = new Product_Public_Page
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            if (ds.Tables.Count > 0)
                page.Products = MapListingProducts(ds.Tables[0], ds.Tables.Count > 2 ? ds.Tables[2] : null);

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                page.TotalRecords = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);

            if (ds.Tables.Count > 3)
                page.IndustryFilters = MapLookups(ds.Tables[3]);
            if (ds.Tables.Count > 4)
                page.ApplicationFilters = MapLookups(ds.Tables[4]);
            if (ds.Tables.Count > 5)
                page.CategoryFilters = MapLookups(ds.Tables[5]);

            return page;
        }

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
                    Name = ColStr(r, "Name"),
                    Count = ColInt(r, "Count") ?? 0
                });
            }
            return list;
        }

        private static Product_Public_Content MapContent(DataRow r)
        {
            return new Product_Public_Content
            {
                ContId = ColInt(r, "cont_id") ?? 0,
                Title = ColStr(r, "cont_title"),
                Intro = ColStr(r, "cont_intro"),
                BreadcrumbTitle = ColStr(r, "cont_breadcrumb_title"),
                WindowTitle = ColStr(r, "cont_window_title"),
                MetaTag = ColStr(r, "cont_metatag"),
                MetaDescription = ColStr(r, "cont_metadesc"),
                PageName = ColStr(r, "cont_pagename"),
                MastheadImage = ColStr(r, "MastheadImage"),
                MobileMastheadImage = ColStr(r, "MobileMastheadImage"),
                MastheadAlt = ColStr(r, "Masthead_alt_text")
            };
        }

        private static List<Product_Public_Item> MapListingProducts(DataTable products, DataTable? industries)
        {
            var names = new Dictionary<int, List<string>>();
            if (industries != null)
            {
                foreach (DataRow r in industries.Rows)
                {
                    var productId = ColInt(r, "ProductId") ?? 0;
                    var name = ColStr(r, "IndustryName");
                    if (productId <= 0 || string.IsNullOrWhiteSpace(name)) continue;
                    if (!names.TryGetValue(productId, out var list))
                    {
                        list = new List<string>();
                        names[productId] = list;
                    }
                    if (!list.Contains(name, StringComparer.OrdinalIgnoreCase))
                        list.Add(name);
                }
            }

            var items = new List<Product_Public_Item>();
            foreach (DataRow r in products.Rows)
            {
                var item = new Product_Public_Item
                {
                    ProductId = ColInt(r, "ProductId") ?? 0,
                    ProductName = ColStr(r, "ProductName"),
                    Product_pagename = ColStr(r, "Product_pagename"),
                    ThumbnailUrl = ColStr(r, "ThumbnailUrl"),
                    ThumbnailAlt = ColStr(r, "ThumbnailAlt")
                };
                if (names.TryGetValue(item.ProductId, out var industryNames))
                    item.Industries = industryNames;
                items.Add(item);
            }
            return items;
        }

        private static void NormalizeFilter(Product_Public_Filter filter, int languageId)
        {
            if (filter.PageNumber < 1) filter.PageNumber = 1;
            if (filter.PageSize < 1) filter.PageSize = 9;
            if (languageId > 0) filter.LanguageId = languageId;
            if (filter.LanguageId < 1) filter.LanguageId = 1;
            filter.IndustryIds = JoinIds(ParseIds(filter.IndustryIds));
            filter.ApplicationIds = JoinIds(ParseIds(filter.ApplicationIds));
            filter.CategoryIds = JoinIds(ParseIds(filter.CategoryIds));
        }

        private static List<int> ParseIds(string? csv)
        {
            var list = new List<int>();
            if (string.IsNullOrWhiteSpace(csv)) return list;
            foreach (var part in csv.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(part.Trim(), out var id) && id > 0 && !list.Contains(id))
                    list.Add(id);
            }
            return list;
        }

        private static string? JoinIds(List<int> ids) =>
            ids == null || ids.Count == 0 ? null : string.Join(",", ids);

        private static string? ColStr(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? r[col].ToString() : null;

        private static int? ColInt(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : null;
    }
}
