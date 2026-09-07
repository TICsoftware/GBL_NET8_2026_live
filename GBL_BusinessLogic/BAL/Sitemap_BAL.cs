using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.DAL;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class Sitemap_BAL : DAL.ContentRepository
    {
        protected readonly IConfiguration _configuration;

        public Sitemap_BAL(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }


        public List<SitemapItem> GetSitemapItems()
        {
            var dt = GetContentForSitemap();
            var items = new List<SitemapItem>();

            foreach (DataRow row in dt.Rows)
            {
                items.Add(new SitemapItem
                {
                    Url = (string.IsNullOrWhiteSpace(row["cont_external_url"].ToString()) ? (string.IsNullOrWhiteSpace(row["cont_pdf"].ToString()) ? (row["pagelink"].ToString()) : row["cont_pdf"].ToString()) : row["cont_external_url"].ToString()),

                    LastModified = row.Table.Columns.Contains("LastModified")
                                    ? Convert.ToDateTime(row["LastModified"])
                                    : DateTime.UtcNow
                });
            }

            return items;
        }
    }
}