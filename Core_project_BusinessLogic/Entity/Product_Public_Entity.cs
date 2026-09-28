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
    }
}
