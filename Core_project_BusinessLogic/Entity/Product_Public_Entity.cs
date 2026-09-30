using System.Collections.Generic;

namespace Core_project_BusinessLogic.Entity
{
    public class Product_Inside_Page
    {
        public Product_Inside_Detail Product { get; set; } = new();
        public List<Product_Inside_Packaging> Packaging { get; set; } = new();
        public List<Product_Inside_Certificate> Certificates { get; set; } = new();
        public List<Product_Public_Lookup> Subcategories { get; set; } = new();
    }

    public class Product_Inside_Detail
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Product_pagename { get; set; }
        public string? Intro { get; set; }
        public string? Content { get; set; }
        public string? Technical_Overview { get; set; }
        public string? Main_Application { get; set; }
        public string? BannerUrl { get; set; }
        public string? BannerAlt { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ThumbnailAlt { get; set; }
        public string? SafetyDataSheetUrl { get; set; }
    }

    public class Product_Inside_Packaging
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ThumbnailAlt { get; set; }
    }

    public class Product_Inside_Certificate
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
    }

    public class Product_Public_Lookup
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Count { get; set; }
    }

    public class Product_Public_Page
    {
        public Product_Public_Content Content { get; set; } = new();
        public List<Product_Public_Item> Products { get; set; } = new();
        public List<Product_Public_Lookup> IndustryFilters { get; set; } = new();
        public List<Product_Public_Lookup> ApplicationFilters { get; set; } = new();
        public List<Product_Public_Lookup> CategoryFilters { get; set; } = new();
        public List<int> SelectedIndustryIds { get; set; } = new();
        public List<int> SelectedApplicationIds { get; set; } = new();
        public List<int> SelectedCategoryIds { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;
    }

    public class Product_Public_Content
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

    public class Product_Public_Item
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Product_pagename { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? ThumbnailAlt { get; set; }
        public List<string> Industries { get; set; } = new();
        public string IndustryNames =>
            Industries == null || Industries.Count == 0
                ? string.Empty
                : string.Join(", ", Industries);
    }

    public class Product_Public_Filter
    {
        public string? IndustryIds { get; set; }
        public string? ApplicationIds { get; set; }
        public string? CategoryIds { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 9;
        public int LanguageId { get; set; } = 1;
    }
}
