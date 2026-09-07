using System.ComponentModel.DataAnnotations;

namespace GBL_MVC.Models
{
    public class ContactUsEnquiryModel
    {
        [Required(ErrorMessage = "Please enter your full name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z][a-zA-Z\s.]*$", ErrorMessage = "Only alphabets, spaces and '.' are allowed")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Please enter your designation")]
        [StringLength(100, ErrorMessage = "Designation cannot exceed 100 characters")]
        public string? Designation { get; set; }

        [Required(ErrorMessage = "Please enter your organisation name")]
        [StringLength(200, ErrorMessage = "Organisation name cannot exceed 200 characters")]
        public string? Organisation { get; set; }

        [Required(ErrorMessage = "Please enter your work email")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        [RegularExpression(@"^\+?[1-9][\d\s-]{7,14}$", ErrorMessage = "Enter a valid phone number")]
        public string? Phone { get; set; }


        [Required(ErrorMessage = "Please select your state")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select your state")]
        public int StateId { get; set; }

        public string State { get; set; }

        public int CityId { get; set; }

        [Required(ErrorMessage = "Please enter your city")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }


        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string? Message { get; set; }

        public string? IPAddress { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept the consent.")]
        public bool Consent { get; set; }


        public int CategoryId { get; set; }

        public string CategoryTitle { get; set; }
    }
}
