using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class Product_Master_Entity
    {
        public const string TextBoxPattern = @"^[^~<>|/\\!@#]*$";
        public const string TextBoxPatternMessage =
            "Special characters ~ < > | / \\ ! @ # are not allowed.";

        public int ProductId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 500 characters.")]
        [RegularExpression(TextBoxPattern, ErrorMessage = TextBoxPatternMessage)]
        public string? ProductName { get; set; }

        [StringLength(300, ErrorMessage = "Page name cannot exceed 300 characters.")]
        public string? Product_pagename { get; set; }

        public string? Intro { get; set; }
        public string? Content { get; set; }
        public string? Technical_Overview { get; set; }

        public int? Thumbnail_Image_media_id { get; set; }
        public int? Banner_Image_media_id { get; set; }
        public int? SafetyDataSheet_media_id { get; set; }

        public string? Thumbnail_Image_Url { get; set; }
        public string? Banner_Image_Url { get; set; }
        public string? SafetyDataSheet_Url { get; set; }

        public int? Language_Master_Id { get; set; }
        public string? LanguageName { get; set; }

        [Required(ErrorMessage = "Display order is required.")]
        [Range(1, 9999, ErrorMessage = "Display order must be between 1 and 9999.")]
        public int? Sequence { get; set; }

        /// <summary>1 = Active, 0 = Inactive</summary>
        public int? Status { get; set; }

        public int? Create_UserId { get; set; }
        public int? Update_UserId { get; set; }

        public List<int> IndustryIds { get; set; } = new();
        public List<int> ApplicationIds { get; set; } = new();
        public List<int> SubcategoryIds { get; set; } = new();
        public List<int> PackagingIds { get; set; } = new();

        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
    }

    public class ProductLookupItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class ProductDeleteModel
    {
        public int Id { get; set; }
    }

    public class ProductStatusModel
    {
        public int Id { get; set; }
        public int Status { get; set; }
    }
}
