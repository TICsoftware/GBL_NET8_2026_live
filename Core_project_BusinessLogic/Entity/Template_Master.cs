
using System;
using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class TemplateMaster
    {
        public int ID { get; set; }

        [Display(Name = "Language")]
        [Required(ErrorMessage = "Please select a language.")]
        public int? Language_Master_ID { get; set; }

        [Display(Name = "Template Name")]
        [Required(ErrorMessage = "Please enter template name.")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "Template name must be 2-120 characters.")]
        [RegularExpression(@"^(?! )[A-Za-z0-9 .'\-&()]+(?<! )$", ErrorMessage = "Please enter only valid characters (letters, numbers, spaces, and .'-&()).")]
        public string? Name { get; set; }

        [Display(Name = "Status")]
        [Required(ErrorMessage = "Please select status.")]
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
