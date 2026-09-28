using System.Collections.Generic;

namespace Core_project_BusinessLogic.Entity
{
    public class Industry_Public_Page
    {
        public Industry_Public_Content Content { get; set; } = new();
        public List<Industry_Public_Item> Industries { get; set; } = new();
        public List<Industry_Public_Lookup> IndustryFilters { get; set; } = new();
        public List<Industry_Public_Lookup> ApplicationFilters { get; set; } = new();
        public List<Industry_Public_Lookup> CategoryFilters { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? IndustryId { get; set; }
        public int? ApplicationId { get; set; }
        public int? CategoryId { get; set; }
    }

    public class Industry_Public_Content
    {
        public int ContId { get; set; }
        public string? Title { get; set; }
        public string? Intro { get; set; }
        public string? BreadcrumbTitle { get; set; }
        public string? WindowTitle { get; set; }
        public string? MetaTag { get; set; }
        public string? MetaDescription { get; set; }
        public string? PageName { get; set; }
        public string? MastheadImage { get; set; }
        public string? MobileMastheadImage { get; set; }
        public string? MastheadAlt { get; set; }
    }

    public class Industry_Public_Item
    {
        public int IndustryId { get; set; }
        public string? IndustryName { get; set; }
        public string? Industry_pagename { get; set; }
        public string? Intro { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ThumbnailAlt { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class Industry_Public_Lookup
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class Industry_Public_Filter
    {
        public int? IndustryId { get; set; }
        public int? ApplicationId { get; set; }
        public int? CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int LanguageId { get; set; } = 1;
    }

    public class Industry_Inside_Page
    {
        public Industry_Inside_Detail Industry { get; set; } = new();
        public List<Industry_Public_Lookup> Categories { get; set; } = new();
        public List<Industry_Inside_Product> Products { get; set; } = new();
        public int ActiveCategoryId { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 6;
    }

    public class Industry_Inside_Detail
    {
        public int IndustryId { get; set; }
        public string? IndustryName { get; set; }
        public string? Industry_pagename { get; set; }
        public string? Intro { get; set; }
        public string? BannerUrl { get; set; }
        public string? BannerAlt { get; set; }
        public string? WindowTitle { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
    }

    public class Industry_Inside_Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Product_pagename { get; set; }
        public string? Intro { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ThumbnailAlt { get; set; }
        public List<string> Applications { get; set; } = new();
    }

    public class Industry_Inside_Filter
    {
        public int IndustryId { get; set; }
        public int CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int LanguageId { get; set; } = 1;
    }
}
