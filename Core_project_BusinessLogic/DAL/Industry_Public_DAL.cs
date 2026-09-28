using System;
using System.Collections.Generic;
using System.Data;
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

        private static string? ColStr(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? r[col].ToString() : null;

        private static int? ColInt(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : null;
    }
}
