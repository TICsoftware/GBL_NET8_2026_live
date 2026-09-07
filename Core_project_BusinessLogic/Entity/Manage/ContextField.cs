using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class ContextField
    {
        public int id { get; set; }

        public int? post_type_id { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Range(0, 1, ErrorMessage = "Please select a valid status.")]
        public int? status { get; set; }

        [Required(ErrorMessage = "Field type is required.")]
        [Range(1, 8, ErrorMessage = "Please select a valid field type.")]
        public int? field_type_id { get; set; }

        [Required(ErrorMessage = "Field name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Field name must be 2-100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s\-_()]+$", ErrorMessage = "Field name contains invalid characters.")]
        public string? name { get; set; }

        [Required(ErrorMessage = "Field key is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Field key must be 2-100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9_#-]+$", ErrorMessage = "Field key can contain letters, numbers, #, _ and -.")]
        public string? name_key { get; set; }

        [Required(ErrorMessage = "Display order is required.")]
        [Range(1, 9999, ErrorMessage = "Display order must be between 1 and 9999.")]
        public int? order_id { get; set; }

        [Required(ErrorMessage = "Field category is required.")]
        [Range(0, 1, ErrorMessage = "Please select a valid field category.")]
        public int? is_block { get; set; }   // NEW COLUMN
        public string? field_validation { get; set; }
        public string? validation_script { get; set; }

        [Required(ErrorMessage = "Required option is required.")]
        [Range(0, 1, ErrorMessage = "Please select a valid required option.")]
        public int? is_required { get; set; }
        public string? custom_validation { get; set; }
        public string? place_holder { get; set; }
        public string? help_text { get; set; }
        public string? field_options { get; set; }
        public int? allow_multiple_option { get; set; }
        public string? html_content { get; set; }
        public string? datepicker_type { get; set; }
        public string? custom_table { get; set; }
        public string? table_key_column { get; set; }
        public string? table_value_column { get; set; }
        public int? show_in_listview { get; set; }
        public string? extra_attribute { get; set; }
        public string? wrapper_classes { get; set; }
        public string? css_classes { get; set; }

        public int? Created_UserID { get; set; }
        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }
        public int? Published_UserID { get; set; }
        public DateTime? Published_Date { get; set; }
        public int? DeActivated_UserID { get; set; }
        public DateTime? DeActivated_Date { get; set; }
    }
}
