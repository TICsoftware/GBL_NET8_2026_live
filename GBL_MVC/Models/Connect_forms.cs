using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using GBL_MVC.Classes;

namespace GBL_MVC;

public class ContactUs
{
    [Required(ErrorMessage = "Please enter full name")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z][a-zA-Z\s.]*$", ErrorMessage = "Only Alphabets,Space,'.' allowed.")]
    public string? FullName { get; set; }


    [Required(ErrorMessage = "Please enter email")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Enter valid email address")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(150)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please enter Mobile number")]
    [RegularExpression(@"^\+?[1-9][\d\s-]{7,14}$", ErrorMessage = "Enter valid mobile number")]
    public string? Contact { get; set; }

    [Required(ErrorMessage = "Please enter city")]
    [StringLength(100, ErrorMessage = "City cannot exceed 1000 characters")]
    public string? City { get; set; }

    [StringLength(200, ErrorMessage = "Organisation/Hospital/Lab cannot exceed 200 characters")]
    public string? Labname { get; set; }

    [Required(ErrorMessage = "Please enter message")]
    [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
    public string? Message { get; set; }

    [MaxFileSize(5 * 1024 * 1024)] // 5 MB 
    [AllowedExtensions([".pdf", ".doc", ".docx"], ErrorMessage = "Only PDF and word file is accepted")]
    public IFormFile? attachment { get; set; }

    //[Range(typeof(bool), "true", "true", ErrorMessage = "Please accept terms")]   
    public bool AcceptTerms { get; set; }

    public string? form_type { get; set; }

    public string? TempFileName { get; set; }

    public string? form_status { get; set; }

    [Required(ErrorMessage = "Please select enquiry type")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Please select enquiry type")]
    public string? Enquiry_Type { get; set; }
}

public class CaptchaResponse
{
    [JsonProperty("success")]
    public string? Success { get; set; }

    [JsonProperty("error-codes")]
    public List<string>? ErrorCodes { get; set; }
}

public class Event_Register
{
    [Required(ErrorMessage = "Please enter full name")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z][a-zA-Z\s.]*$", ErrorMessage = "Only Alphabets,Space,'.' allowed.")]
    public string? Full_name { get; set; }


    [Required(ErrorMessage = "Please enter email")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Enter valid email address")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(150)]
    public string? Email_Id { get; set; }

    [Required(ErrorMessage = "Please enter designation")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z][a-zA-Z\s.]*$", ErrorMessage = "Only Alphabets,Space,'.' allowed.")]
    public string? Designation { get; set; }

    [Required(ErrorMessage = "Please enter Mobile number")]
    [RegularExpression(@"^\+?[1-9][\d\s-]{7,14}$", ErrorMessage = "Enter valid mobile number")]
    public string? Contact { get; set; }

    public string? Event_Id { get; set; }

}