using System;
using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class ContextMaster
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Language is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Language is required.")]
        public int? Language_Master_ID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be 3-100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s\.\,\-_()\./&]{3,100}$", ErrorMessage = "Title contains invalid characters.")]
        public string Title { get; set; }

       // [Required(ErrorMessage = "Design Layout is required.")]
        [StringLength(5000, MinimumLength = 5, ErrorMessage = "Design Layout must be 5-5000 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s\.\,\-_()\./&:;!\?#%+=@\{\}\[\]<>""'\r\n]{5,5000}$", ErrorMessage = "Design Layout contains invalid characters.")]
        public string? Design_Layout { get; set; }

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
