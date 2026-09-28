using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class Industry_Public_DAL : DBHelper
    {
        public Industry_Public_DAL(IConfiguration configuration) : base(configuration) { }

        public Industry_Public_Page GetIndex(string pageName, int languageId, Industry_Public_Filter filter)
        {
            SqlParameter[] p = BuildFilterParams(pageName, languageId, filter);
            DataSet ds = GetDataSet("Industry_Public_GetIndex", p);
            var page = new Industry_Public_Page
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                IndustryId = filter.IndustryId,
                ApplicationId = filter.ApplicationId,
                CategoryId = filter.CategoryId
            };

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                page.Content = MapContent(ds.Tables[0].Rows[0]);

            if (ds.Tables.Count > 1)
                page.Industries = MapIndustries(ds.Tables[1]);

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

        public (List<Industry_Public_Item> Data, int Total) GetPaged(Industry_Public_Filter filter)
        {
            SqlParameter[] p = BuildFilterParams(null, 0, filter);
            DataSet ds = GetDataSet("Industry_Public_GetPaged", p);
            var list = ds.Tables.Count > 0 ? MapIndustries(ds.Tables[0]) : new List<Industry_Public_Item>();
            int total = 0;
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            return (list, total);
        }

        public Industry_Inside_Page GetInside(string pageName, int languageId, int categoryId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 6;
            SqlParameter[] p =
            {
                new("@PageName", string.IsNullOrWhiteSpace(pageName) ? DBNull.Value : pageName.Trim()),
                new("@LanguageId", languageId > 0 ? languageId : 1),
                new("@CategoryId", categoryId > 0 ? categoryId : DBNull.Value),
                new("@Page", page),
                new("@PageSize", pageSize)
            };

            DataSet ds = GetDataSet("Industry_Public_GetInside", p);
            var model = new Industry_Inside_Page
            {
                PageNumber = page,
                PageSize = pageSize
            };

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                model.Industry = MapInsideIndustry(ds.Tables[0].Rows[0]);

            if (ds.Tables.Count > 1)
                model.Categories = MapLookups(ds.Tables[1]);

            if (ds.Tables.Count > 2)
                model.Products = MapInsideProducts(ds.Tables[2], ds.Tables.Count > 4 ? ds.Tables[4] : null);

            if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
                model.TotalRecords = Convert.ToInt32(ds.Tables[3].Rows[0]["TotalCount"]);

            if (ds.Tables.Count > 5 && ds.Tables[5].Rows.Count > 0)
                model.ActiveCategoryId = ColInt(ds.Tables[5].Rows[0], "ActiveCategoryId") ?? 0;
            else if (categoryId > 0)
                model.ActiveCategoryId = categoryId;
            else if (model.Categories.Count > 0)
                model.ActiveCategoryId = model.Categories[0].Id;

            return model;
        }

        public (List<Industry_Inside_Product> Data, int Total) GetInsideProducts(Industry_Inside_Filter filter)
        {
            filter ??= new Industry_Inside_Filter();
            if (filter.PageNumber < 1) filter.PageNumber = 1;
            if (filter.PageSize < 1) filter.PageSize = 6;
            if (filter.LanguageId < 1) filter.LanguageId = 1;

            SqlParameter[] p =
            {
                new("@IndustryId", filter.IndustryId),
                new("@CategoryId", filter.CategoryId > 0 ? filter.CategoryId : DBNull.Value),
                new("@Page", filter.PageNumber),
                new("@PageSize", filter.PageSize),
                new("@LanguageId", filter.LanguageId)
            };

            DataSet ds = GetDataSet("Industry_Public_GetInsideProducts", p);
            var list = ds.Tables.Count > 0 ? MapInsideProducts(ds.Tables[0], ds.Tables.Count > 2 ? ds.Tables[2] : null) : new List<Industry_Inside_Product>();
            int total = 0;
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            return (list, total);
        }

        private static SqlParameter[] BuildFilterParams(string? pageName, int languageId, Industry_Public_Filter filter)
        {
            filter ??= new Industry_Public_Filter();
            if (filter.PageNumber < 1) filter.PageNumber = 1;
            if (filter.PageSize < 1) filter.PageSize = 10;
            if (languageId > 0) filter.LanguageId = languageId;
            if (filter.LanguageId < 1) filter.LanguageId = 1;

            var list = new List<SqlParameter>
            {
                new("@IndustryId", filter.IndustryId.HasValue && filter.IndustryId.Value > 0 ? filter.IndustryId.Value : DBNull.Value),
                new("@ApplicationId", filter.ApplicationId.HasValue && filter.ApplicationId.Value > 0 ? filter.ApplicationId.Value : DBNull.Value),
                new("@CategoryId", filter.CategoryId.HasValue && filter.CategoryId.Value > 0 ? filter.CategoryId.Value : DBNull.Value),
                new("@Page", filter.PageNumber),
                new("@PageSize", filter.PageSize),
                new("@LanguageId", filter.LanguageId > 0 ? filter.LanguageId : 1)
            };

            if (pageName != null)
            {
                list.Insert(0, new("@PageName", string.IsNullOrWhiteSpace(pageName) ? DBNull.Value : pageName.Trim()));
            }

            return list.ToArray();
        }

        private static Industry_Public_Content MapContent(DataRow r)
        {
            return new Industry_Public_Content
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

        private static List<Industry_Public_Item> MapIndustries(DataTable dt)
        {
            var list = new List<Industry_Public_Item>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Industry_Public_Item
                {
                    IndustryId = ColInt(r, "IndustryId") ?? 0,
                    IndustryName = ColStr(r, "IndustryName"),
                    Industry_pagename = ColStr(r, "Industry_pagename"),
                    Intro = ColStr(r, "Intro"),
                    ThumbnailUrl = ColStr(r, "ThumbnailUrl"),
                    ThumbnailAlt = ColStr(r, "ThumbnailAlt"),
                    DisplayOrder = ColInt(r, "DisplayOrder") ?? 0
                });
            }
            return list;
        }

        private static List<Industry_Public_Lookup> MapLookups(DataTable dt)
        {
            var list = new List<Industry_Public_Lookup>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new Industry_Public_Lookup
                {
                    Id = Convert.ToInt32(r["Id"]),
                    Name = ColStr(r, "Name")
                });
            }
            return list;
        }

        private static Industry_Inside_Detail MapInsideIndustry(DataRow r)
        {
            return new Industry_Inside_Detail
            {
                IndustryId = ColInt(r, "IndustryId") ?? 0,
                IndustryName = ColStr(r, "IndustryName"),
                Industry_pagename = ColStr(r, "Industry_pagename"),
                Intro = ColStr(r, "Intro"),
                BannerUrl = ColStr(r, "BannerUrl"),
                BannerAlt = ColStr(r, "BannerAlt"),
                WindowTitle = ColStr(r, "Window_Title"),
                MetaTitle = ColStr(r, "Meta_Title"),
                MetaDescription = ColStr(r, "Meta_Description")
            };
        }

        private static List<Industry_Inside_Product> MapInsideProducts(DataTable products, DataTable? applications)
        {
            var appMap = new Dictionary<int, List<string>>();
            if (applications != null)
            {
                foreach (DataRow r in applications.Rows)
                {
                    var productId = ColInt(r, "ProductId") ?? 0;
                    var name = ColStr(r, "ApplicationName");
                    if (productId <= 0 || string.IsNullOrWhiteSpace(name)) continue;
                    if (!appMap.TryGetValue(productId, out var names))
                    {
                        names = new List<string>();
                        appMap[productId] = names;
                    }
                    if (!names.Contains(name, StringComparer.OrdinalIgnoreCase))
                        names.Add(name);
                }
            }

            var list = new List<Industry_Inside_Product>();
            foreach (DataRow r in products.Rows)
            {
                var item = new Industry_Inside_Product
                {
                    ProductId = ColInt(r, "ProductId") ?? 0,
                    ProductName = ColStr(r, "ProductName"),
                    Product_pagename = ColStr(r, "Product_pagename"),
                    Intro = ColStr(r, "Intro"),
                    ThumbnailUrl = ColStr(r, "ThumbnailUrl"),
                    ThumbnailAlt = ColStr(r, "ThumbnailAlt")
                };
                if (appMap.TryGetValue(item.ProductId, out var names))
                    item.Applications = names;
                list.Add(item);
            }
            return list;
        }

        private static string? ColStr(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? r[col].ToString() : null;

        private static int? ColInt(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : null;
    }
}
