
using System;
using System.ComponentModel.DataAnnotations;


namespace Core_project_BusinessLogic.Entity
{

public class ContextMasterListVM
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Language_Name { get; set; }
    public string TemplateNames { get; set; }
    public int? Status { get; set; }
    public DateTime? Created_Date { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "Search must be between 2 and 100 characters.")]
    [RegularExpression(@"^[A-Za-z0-9\s\.\,\-_()/&]+$", ErrorMessage = "Search contains invalid characters.")]
    public string Search { get; set; }
}

}