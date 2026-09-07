using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Identity.Client;
using GBL_MVC.Classes;
using System.ComponentModel.DataAnnotations;

namespace GBL_MVC;

public class Career
{
    public string? Encrypt_job_Id { get; set; }

    [Required(ErrorMessage = "Please enter Education")]
    [StringLength(1000, ErrorMessage = "Education cannot exceed 1000 characters.")]
    [RegularExpression(@"^[^`\^~<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the Education")]
    public string? Education { get; set; }

    [Required(ErrorMessage = "Please enter Role")]
    [StringLength(500, ErrorMessage = "Role cannot exceed 500 characters.")]
    [RegularExpression(@"^[^`\^~<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the Role")]
    public string? Role { get; set; }

    [StringLength(2000, ErrorMessage = "Experience cannot exceed 2000 characters.")]
    [RegularExpression(@"^[^`\^~<>{}]+", ErrorMessage = @"Please e                                            nter valid characters. The following characters are not accepted `^~#<> in the Experience")]
    public string? Experience { get; set; }

    [ValidateNever]
    [Required(ErrorMessage = "Please enter Job Description")]
    public string? Job_Description { get; set; }

    [StringLength(1000, ErrorMessage = "Location cannot exceed 1000 characters.")]
    [RegularExpression(@"^[^`\^~<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the Location")]
    public string? Location { get; set; }

    [StringLength(200, ErrorMessage = "Salary range cannot exceed 200 characters.")]
    [RegularExpression(@"^[^`\^~<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the Salary range")]
    public string? Salary_range { get; set; }

    [ValidateNever]
    public string? About_the_Role { get; set; }

    [StringLength(200, ErrorMessage = "Work mode cannot exceed 200 characters.")]
    [RegularExpression(@"^[^`\^~<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the Work mode")]
    public string? Workmode { get; set; }

    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime? Expiry_date { get; set; }
    public DateTime? Created_date { get; set; }
}

public class CMS_job_list
{
    public string? DisplayTitle { get; set; }
    public string? Intro { get; set; }
    public string? Masthead_image { get; set; }
    public string? Image_alt_text { get; set; }
    public string? searchquery { get; set; }
    public int status { get; set; }
    public List<Career>? joblists { get; set; }
    public PagedResult? objpaging { get; set; }
}

public class Job_application
{
    [Required(ErrorMessage = "Please enter full name")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z][a-zA-Z\s.]*$", ErrorMessage = "Only Alphabets,Space,'.' allowed.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Please enter age")]
    [RegularExpression(@"^[0-9]+$",     ErrorMessage = "Only numbers allowed.")]
    public string? Age { get; set; }

    [Required(ErrorMessage = "Please select gender")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Please select enquiry type")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Please enter email")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Enter valid email address")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(150)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Please enter mobile number")]
    [RegularExpression(@"^\+?[1-9][\d\s-]{7,14}$", ErrorMessage = "Enter valid mobile number")]
    [StringLength(100)]
    public string? Contact { get; set; }

    [Required(ErrorMessage = "Please enter current salary")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z0-9\s.]+$", ErrorMessage = "Only alphabets, numbers, spaces, and '.' allowed.")]
    public string? Current_Salary { get; set; }

    [Required(ErrorMessage = "Please enter notice period")]
    [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z0-9\s]+$", ErrorMessage = "Only alphabets, numbers, and spaces allowed.")]
    public string? Notice_Period { get; set; }

    [Required(ErrorMessage = "Please enter current location")]
    public string? Location { get; set; }

    [Required(ErrorMessage = "Please select one option")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Please select enquiry type")]
    public string? Relocate_job { get; set; }

   
    [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
    public string? Message { get; set; }

    [MaxFileSize(5 * 1024 * 1024)] // 5 MB 
    [AllowedExtensions([".pdf", ".doc", ".docx"], ErrorMessage = "Only PDF and word file is accepted")]
    public IFormFile? attachment { get; set; }
   
    public string? Job_id { get; set; }
    public string? TempFileName { get; set; }
}