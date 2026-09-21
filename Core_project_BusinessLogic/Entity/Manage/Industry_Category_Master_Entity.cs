using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class Industry_Category_Master_Entity
    {
        public const string TextBoxPattern = @"^[^~<>|/\\!@#]*$";
        public const string TextBoxPatternMessage =
            "Special characters ~ < > | / \\ ! @ # are not allowed.";

        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 500 characters.")]
        [RegularExpression(TextBoxPattern, ErrorMessage = TextBoxPatternMessage)]
        public string? Name { get; set; }

        [StringLength(300, ErrorMessage = "Page name cannot exceed 300 characters.")]
        public string? PageName { get; set; }

        [Required(ErrorMessage = "Display order is required.")]
        [Range(1, 9999, ErrorMessage = "Display order must be between 1 and 9999.")]
        public int? Sequence { get; set; }

        /// <summary>1 = Active, 0 = Inactive</summary>
        public int? Status { get; set; }

        public int? Language_Master_Id { get; set; }
        public string? LanguageName { get; set; }

        public int? Banner_Image_media_id { get; set; }
        public int? Landing_Thumbnail_Image_media_id { get; set; }
        public string? Banner_Image_Url { get; set; }
        public string? Landing_Thumbnail_Image_Url { get; set; }

        [StringLength(500, ErrorMessage = "Banner image alt cannot exceed 500 characters.")]
        public string? Banner_Image_Alt { get; set; }

        [StringLength(500, ErrorMessage = "Thumbnail image alt cannot exceed 500 characters.")]
        public string? Landing_Thumbnail_Image_Alt { get; set; }

        [StringLength(2000, ErrorMessage = "Window title cannot exceed 2000 characters.")]
        public string? Window_Title { get; set; }

        [StringLength(2000, ErrorMessage = "Meta title cannot exceed 2000 characters.")]
        public string? Meta_Title { get; set; }

        [StringLength(2000, ErrorMessage = "Meta description cannot exceed 2000 characters.")]
        public string? Meta_Description { get; set; }

        public string? Intro { get; set; }
        public string? Content { get; set; }

        public string? MasterType { get; set; }
        public int? Create_UserId { get; set; }
        public int? Update_UserId { get; set; }

        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
    }

    public class IndustryCategoryDeleteModel
    {
        public int Id { get; set; }
        public string? Type { get; set; }
    }

    public class IndustryCategoryStatusModel
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public string? Type { get; set; }
    }
}
