using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_project_BusinessLogic.Entity.Manage
{
    public class MenuHeader
    {
        public int MenuId { get; set; }
        public int? ParentMenuId { get; set; }
        public string ParentMenuName { get; set; }
        public int MenuCategory { get; set; }
        public string MenuName { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public string MenuType { get; set; }
        public int? ColumnNo { get; set; }
        public int Sequence { get; set; }

        public int FeatureImageId { get; set; }
        public int ThumbImageId { get; set; }

        public string ThumbImage { get; set; }
        public string FeatureImage { get; set; }
        public string FeatureTitle { get; set; }
        public string FeatureDescription { get; set; }
        public string FeatureButtonText { get; set; }
        public string FeatureButtonUrl { get; set; }
        public string FeatureButtonTarget { get; set; }

        public string CssClass { get; set; }
        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public int MenuLevel { get; set; }

        public string? DisplayMenuName { get; set; }

    }
}