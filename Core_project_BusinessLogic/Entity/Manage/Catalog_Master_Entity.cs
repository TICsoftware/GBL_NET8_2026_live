using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class CatalogMasterEntity
    {
        public const string TextBoxPattern = @"^[^~<>|/\\!@#]*$";
        public const string TextBoxPatternMessage =
            "Special characters ~ < > | / \\ ! @ # are not allowed.";

        public int ID { get; set; }

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
        public int? IndustryId { get; set; }
        public int? Category_Master_Id { get; set; }
        public string? RelatedName { get; set; }

        public string? MasterType { get; set; }
        public int? Create_UserId { get; set; }
        public int? Update_UserId { get; set; }

        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
    }

    public class CatalogDeleteModel
    {
        public int Id { get; set; }
        public string? Type { get; set; }
    }

    public class CatalogStatusModel
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public string? Type { get; set; }
    }

    public class CatalogLookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
