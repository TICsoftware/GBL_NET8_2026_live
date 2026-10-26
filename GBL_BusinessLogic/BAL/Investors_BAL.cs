using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class Investors_BAL : BasePageBAL
    {
        public const int DefaultPageSize = 12;
        public const int AnnualReportPageSize = 8;

        public Investors_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public InvestorsModel GetPdfListing_BAL(string pagename, int languageId, int geographyId, int pageSize = DefaultPageSize)
        {
            var model = new InvestorsModel();
            var pageName = NormalizePageName(pagename);
            var ds = GetContentComponentData_DAL(pageName, languageId, geographyId);

            if (ds == null || ds.Tables.Count == 0)
            {
                return model;
            }

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (pageSize < 1) pageSize = DefaultPageSize;

            var contentId = model.Content?.ContId ?? 0;
            if (contentId > 0)
            {
                var pageData = GetPdfListing_page_wise_BAL(contentId, 1, pageSize);
                model.SectionArticles_List = pageData.SectionArticles_List;
                model.TotalCount = pageData.TotalCount;
            }
            else if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                var articles = Config_Application_Website.MapArticleList(ds.Tables[2]);
                model.TotalCount = articles.Count;
                model.SectionArticles_List = articles
                    .OrderBy(a => a.Sequence)
                    .ThenByDescending(a => a.Title, StringComparer.OrdinalIgnoreCase)
                    .Take(pageSize)
                    .ToList();
            }

            return model;
        }

        public InvestorsModel GetPdfListing_page_wise_BAL(int contId, int page, int pageSize)
        {
            return MapPagedArticles(Get_Investor_Pdf_page_wise_DAL(contId, page, pageSize), page, pageSize, DefaultPageSize);
        }

        public InvestorsModel GetAnnualReports_BAL(string pagename, int languageId, int geographyId, int pageSize = AnnualReportPageSize)
        {
            var model = new InvestorsModel();
            var pageName = NormalizePageName(pagename);
            var ds = GetContentComponentData_DAL(pageName, languageId, geographyId);

            if (ds == null || ds.Tables.Count == 0)
            {
                return model;
            }

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (pageSize < 1) pageSize = AnnualReportPageSize;

            var contentId = model.Content?.ContId ?? 0;
            if (contentId > 0)
            {
                var pageData = GetAnnualReports_page_wise_BAL(contentId, 1, pageSize);
                model.SectionArticles_List = pageData.SectionArticles_List;
                model.TotalCount = pageData.TotalCount;
            }
            else if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                var articles = Config_Application_Website.MapArticleList(ds.Tables[2]);
                model.TotalCount = articles.Count;
                model.SectionArticles_List = articles
                    .OrderByDescending(a => a.DisplayDate.HasValue)
                    .ThenByDescending(a => a.DisplayDate)
                    .ThenBy(a => a.Sequence)
                    .Take(pageSize)
                    .ToList();
            }

            return model;
        }

        public InvestorsModel GetAnnualReports_page_wise_BAL(int contId, int page, int pageSize)
        {
            return MapPagedArticles(Get_Annual_Reports_page_wise_DAL(contId, page, pageSize), page, pageSize, AnnualReportPageSize);
        }

        private static InvestorsModel MapPagedArticles(DataSet ds, int page, int pageSize, int defaultPageSize)
        {
            var model = new InvestorsModel();

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = defaultPageSize;

            if (ds == null || ds.Tables.Count == 0)
            {
                return model;
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                model.SectionArticles_List = Config_Application_Website.MapArticleList(ds.Tables[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                model.TotalCount = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            }

            return model;
        }

        private static string NormalizePageName(string? pagename)
        {
            if (string.IsNullOrWhiteSpace(pagename)) return string.Empty;
            return pagename.Trim().TrimStart('/');
        }
    }
}
