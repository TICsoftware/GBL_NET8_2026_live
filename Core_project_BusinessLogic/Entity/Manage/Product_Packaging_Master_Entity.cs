using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class Product_Packaging_Master_Entity
    {
        public const string TextBoxPattern = @"^[^~<>|/\\!@#]*$";
        public const string TextBoxPatternMessage =
            "Special characters ~ < > | / \\ ! @ # are not allowed.";

        public int product_packaging_MasterId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(250, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 250 characters.")]
        [RegularExpression(TextBoxPattern, ErrorMessage = TextBoxPatternMessage)]
        public string? Name { get; set; }

        public int? Language_Master_Id { get; set; }
        public string? LanguageName { get; set; }

        public string? Thumbnailimage_Id { get; set; }
        public string? Thumbnail_Image_Url { get; set; }

        [StringLength(250, ErrorMessage = "Thumbnail image alt cannot exceed 250 characters.")]
        public string? Thumbnailimage_alt { get; set; }

        [Required(ErrorMessage = "Display order is required.")]
        [Range(1, 9999, ErrorMessage = "Display order must be between 1 and 9999.")]
        public int? Sequence { get; set; }

        /// <summary>1 = Active, 0 = Inactive</summary>
        public int? Status { get; set; }

        public int? Create_UserId { get; set; }
        public int? Update_UserId { get; set; }

        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
    }
}
