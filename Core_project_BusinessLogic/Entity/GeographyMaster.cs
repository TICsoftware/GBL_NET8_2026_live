using System;
using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class GeographyMaster
    {
        public int ID { get; set; }
        
        [Required(ErrorMessage = "Language is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid language.")]
        public int? Language_Master_ID { get; set; }
        
        [Required(ErrorMessage = "Country name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Country name must be 2-100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s\.\,\-']+$", ErrorMessage = "Country name contains invalid characters.")]
        public string? Country_Name { get; set; }
        
        [Required(ErrorMessage = "Status is required.")]
        [Range(0, 1, ErrorMessage = "Please select a valid status.")]
        public int? Status { get; set; }

        public int? Created_UserID { get; set; }
        public DateTime? Created_Date { get; set; }

        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }

        public int? Published_UserID { get; set; }
        public DateTime? Published_Date { get; set; }

        public int? DeActivated_UserID { get; set; }
        public DateTime? DeActivated_Date { get; set; }
    }
}
