using System;
using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class LanguageMaster
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Language name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Language name must be 2-50 characters.")]
        [RegularExpression(@"^[A-Za-z\s\.\-']+$", ErrorMessage = "Language name contains invalid characters.")]
        public string? Language_Name { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [Range(0, 1, ErrorMessage = "Please select a valid status.")]
        public int? Status { get; set; }

        [Required(ErrorMessage = "Sequence is required.")]
        [Range(1, 9999, ErrorMessage = "Sequence must be between 1 and 9999.")]
        public int? Language_Sequence { get; set; }
        public int? Created_UserID { get; set; }
        public DateTime? Created_Date { get; set; }
        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }
        public int? Published_UserID { get; set; }
        public DateTime? Published_Date { get; set; }
    }
}
