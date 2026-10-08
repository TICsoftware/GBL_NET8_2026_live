using System.ComponentModel.DataAnnotations;

namespace GBL_MVC.Models
{
    public class ProductEnquiryModel
    {
        [Required(ErrorMessage = "Product is required")]
        [StringLength(500, ErrorMessage = "Product name cannot exceed 500 characters")]
        public string? ProductName { get; set; }

        [StringLength(300, ErrorMessage = "Product page name cannot exceed 300 characters")]
        public string? ProductPageName { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z][a-zA-Z\s.]*$", ErrorMessage = "Only alphabets, spaces and '.' are allowed")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Please enter company name")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        [RegularExpression(@"^[^`\^~<>{}]*$", ErrorMessage = "Please enter valid characters")]
        public string? CompanyName { get; set; }

        [Required(ErrorMessage = "Please enter street address")]
        [StringLength(500, ErrorMessage = "Street address cannot exceed 500 characters")]
        [RegularExpression(@"^[^`\^~<>{}]*$", ErrorMessage = "Please enter valid characters")]
        public string? StreetAddress { get; set; }

        [Required(ErrorMessage = "Please enter city")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        [RegularExpression(@"^[^`\^~<>{}]*$", ErrorMessage = "Please enter valid characters")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Please enter state")]
        [StringLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        [RegularExpression(@"^[^`\^~<>{}]*$", ErrorMessage = "Please enter valid characters")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Please select country")]
        [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
        public string? Country { get; set; }

        /// <summary>Optional landline. Digits only, max 15 (international).</summary>
        [StringLength(16, ErrorMessage = "Phone cannot exceed 15 digits")]
        [RegularExpression(@"^$|^\+?[0-9]{1,15}$", ErrorMessage = "Enter a valid phone number (up to 15 digits)")]
        public string? Phone { get; set; }

        [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters")]
        [RegularExpression(@"^\+?[0-9]{1,4}$", ErrorMessage = "Enter a valid country code")]
        public string? CountryCode { get; set; }

        /// <summary>Required mobile. Digits only, max 15 (international).</summary>
        [Required(ErrorMessage = "Please enter mobile number")]
        [StringLength(16, ErrorMessage = "Mobile cannot exceed 15 digits")]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Enter a valid mobile number (7–15 digits)")]
        public string? Mobile { get; set; }

        [Required(ErrorMessage = "Please enter fax")]
        [StringLength(16, ErrorMessage = "Fax cannot exceed 15 digits")]
        [RegularExpression(@"^\+?[0-9]{1,15}$", ErrorMessage = "Enter a valid fax number (up to 15 digits)")]
        public string? Fax { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(500, ErrorMessage = "Email cannot exceed 500 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Enter a valid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter business type")]
        [StringLength(200, ErrorMessage = "Business type cannot exceed 200 characters")]
        [RegularExpression(@"^[^`\^~<>{}]*$", ErrorMessage = "Please enter valid characters")]
        public string? BusinessType { get; set; }

        [Required(ErrorMessage = "Please enter enquiry details")]
        [RegularExpression(@"^[^`\^~<>{}]*$", ErrorMessage = "Please enter valid characters")]
        public string? EnquiryDetails { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept the privacy policy and terms of use.")]
        public bool AcceptTerms { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please confirm you are not a robot.")]
        public bool NotRobot { get; set; }

        public string? IPAddress { get; set; }
    }
}
