using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GBL_MVC;

public class cms_Reports
{

}

public class cms_Report_type
{
    public int Id { get; set; }

    [StringLength(2000, ErrorMessage = "Title cannot exceed 2000 characters.")]
    [RegularExpression(@"^[^`\^~#<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the title")]
    public string? Title { get; set; }

    [ValidateNever]
    public string? Description { get; set; }
    
    [ValidateNever]
    public string? Final_msg { get; set; }

    [StringLength(2000, ErrorMessage = "Title cannot exceed 2000 characters.")]
    [RegularExpression(@"^[^`\^~#<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the title")]
    public string? Mail_subject_line { get; set; }
    
    [ValidateNever]
    public string? Mail_content { get; set; }
    public List<int>? Categories { get; set; }
}

public class cms_Report_Category
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Mail_title { get; set; }
    public int? Parent_id { get; set; }
}