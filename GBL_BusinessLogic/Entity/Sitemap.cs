using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class Sitemap
    {
        public int ContId { get; set; }
        public string PageName { get; set; }

        public string PageURl { get; set; }
        public string ExternalUrl { get; set; }
        public bool IsExternal { get; set; }
        public string PdfPath { get; set; }
        public DateTime? LastModified { get; set; }
    }

    public class SitemapItem
    {
        public string Url { get; set; }
        public DateTime LastModified { get; set; }
    }
}