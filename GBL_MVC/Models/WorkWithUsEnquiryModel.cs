using System.ComponentModel.DataAnnotations;
using GBL_MVC.Classes;

namespace GBL_MVC.Models
{
    public class WorkWithUsEnquiryModel
    {
        [Required(ErrorMessage = "Please enter your full name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z][a-zA-Z\s.]*$", ErrorMessage = "Only alphabets, spaces and '.' are allowed")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(500, ErrorMessage = "Email cannot exceed 500 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Enter a valid email address")]
        public string? Email { get; set; }

        [StringLength(1000, ErrorMessage = "Expertise cannot exceed 1000 characters")]
        [RegularExpression(@"^[^`\^~<>{}]*$", ErrorMessage = "Please enter valid characters. The following characters are not accepted `^~<>{}")]
        public string? Expertise { get; set; }

        [StringLength(2000, ErrorMessage = "Address cannot exceed 2000 characters")]
        [RegularExpression(@"^[^`\^~<>{}]*$", ErrorMessage = "Please enter valid characters. The following characters are not accepted `^~<>{}")]
        public string? Address { get; set; }

        [StringLength(1000, ErrorMessage = "Designation cannot exceed 1000 characters")]
        [RegularExpression(@"^[^`\^~<>{}]*$", ErrorMessage = "Please enter valid characters. The following characters are not accepted `^~<>{}")]
        public string? Designation { get; set; }

        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions([".pdf", ".doc", ".docx"], ErrorMessage = "Only PDF and Word files are accepted")]
        public IFormFile? Resume { get; set; }

        [RegularExpression(@"^[^`\^~<>{}]*$", ErrorMessage = "Please enter valid characters. The following characters are not accepted `^~<>{}")]
        public string? Message { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please confirm you are not a robot.")]
        public bool NotRobot { get; set; }

        public string? IPAddress { get; set; }
    }
}
