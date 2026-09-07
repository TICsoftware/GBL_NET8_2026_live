using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class MenuHeader
    {
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }
        public string? ParentMenuName { get; set; }

        public int MenuCategory { get; set; }
        public string? MenuName { get; set; }
        public string? Url { get; set; }
        public string? Target { get; set; }
        public string? MenuType { get; set; }

        public int? ColumnNo { get; set; }
        public int Sequence { get; set; }

        public int FeatureImageId { get; set; }
        public string? FeatureImage { get; set; }

        public int ThumbImageId { get; set; }
        public string? ThumbImage { get; set; }

        public string? FeatureTitle { get; set; }
        public string? FeatureDescription { get; set; }
        public string? FeatureButtonText { get; set; }
        public string? FeatureButtonUrl { get; set; }
        public string? FeatureButtonTarget { get; set; }

        public string? CssClass { get; set; }
        public int Status { get; set; }
    }


    public class MenuFooter
    {
        public int FooterId { get; set; }

        public int? ParentFooterId { get; set; }
        public string? ParentFooterName { get; set; }

        // Footer Type: CompanyInfo, Column, Link, Social, Copyright
        public string? FooterType { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? Url { get; set; }
        public string? Target { get; set; }

        // Images
        public int FooterImageId { get; set; }
        public string? FooterImage { get; set; }

        public int FooterThumbImageId { get; set; }
        public string? FooterThumbImage { get; set; }

        // Display / Styling
        public string? IconClass { get; set; }
        public string? ColumnNo { get; set; }

        public int Sequence { get; set; }

        public int Status { get; set; }
    }


}