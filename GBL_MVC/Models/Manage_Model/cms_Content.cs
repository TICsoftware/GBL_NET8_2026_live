using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GBL_MVC;

public class cms_Content
{
    [BindNever]
    [Key]
    public int Id { get; set; }

    public string? Id_encrypt_val { get; set; }

    [Required(ErrorMessage = "Please enter title")]
    [StringLength(2000, ErrorMessage = "Title cannot exceed 2000 characters.")]
    [RegularExpression(@"^[^`\^~#<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the title")]
    public string Title { get; set; } = string.Empty;

    public int? Section_id { get; set; }
    public int? Subsection_id { get; set; }
    public int? Language_section_id { get; set; }
    public int? Language_subsection_id { get; set; }

    [Required(ErrorMessage = "Please select template")]
    public int? Template_Master_ID { get; set; }

    [Required(ErrorMessage = "Please select template type")]
    public int? template_type { get; set; }

    public string? Template_name { get; set; }

    [Required(ErrorMessage = "Please select Content type")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select Content type")]
    public int? Content_Type_ID { get; set; }

    [StringLength(2000, ErrorMessage = "Landing page title cannot exceed 2000 characters.")]
    [RegularExpression(@"^[^`\^~#<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the landing page title")]
    public string? Hmpg_title { get; set; }

    [ValidateNever]
    public string? Intro { get; set; }

    [ValidateNever]
    public string? Hmpg_intro { get; set; }

    [ValidateNever]
    public string? Content { get; set; }

    [Required(ErrorMessage = "Page name is required.")]
    [StringLength(120, ErrorMessage = "Please ensure the Page Name does not exceed 120 characters ")]
    [RegularExpression(@"^[^`~#<>{}%?&+*]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `~#<>%*&?+ in the page name")]
    public string Pagename { get; set; } = string.Empty;

    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime? Displaydate { get; set; }

    [Required(ErrorMessage = "Please enter window title.")]
    [StringLength(300, ErrorMessage = "Please ensure the window title does not exceed 300 characters ")]
    [RegularExpression(@"^[^`\^~#<>{}?]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the window title ")]
    public string? Window_title { get; set; }
    public int? Sequence { get; set; }
    public string? External_url { get; set; }
    public bool IsExternal { get; set; }
    public string? Bottom_Banner { get; set; }

    [StringLength(500, ErrorMessage = "Please ensure meta keywords does not exceed 500 characters.")]
    [RegularExpression(@"^[^`\^~#<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the meta keywords")]
    [DataType(DataType.MultilineText)]
    public string? Metatag { get; set; }

    [DataType(DataType.MultilineText)]
    [StringLength(500, ErrorMessage = "Please ensure the meta description does not exceed 500 characters.")]
    [RegularExpression(@"^[^`\^~#<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the meta description")]
    public string? Metadesc { get; set; }
    
    [ValidateNever]
    public string? PageSchema { get; set; }


    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime? Metaexpiry { get; set; }
    public bool? status { get; set; }
    public bool? IsInProcess { get; set; }
    public int? Language_master_id { get; set; }
    public int? Lang_groupid { get; set; }
    public string? Spot_template_id { get; set; }   // tempId
    public int? article_id { get; set; }
    public int? Geography_ID { get; set; }

    [StringLength(500, ErrorMessage = "Please ensure By Line does not exceed 500 characters.")]
    [RegularExpression(@"^[^`\^~#<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the By Line")]
    public string? ByLine { get; set; }

    [StringLength(500, ErrorMessage = "Please ensure Publication does not exceed 500 characters.")]
    [RegularExpression(@"^[^`\^~#<>{}]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the Publication")]
    public string? Publication { get; set; }
    public int? Companies_id { get; set; }
    public bool IsSearch { get; set; }
    public string? Search_url { get; set; }
    public string? Page_url { get; set; }

    [StringLength(50, ErrorMessage = "Please ensure Breadcrumb title does not exceed 50 characters.")]
    [RegularExpression(@"^[^`\^~#<>{}?]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the Breadcrumb title")]
    public string? Breadcrumb_title { get; set; }
    public int? Loc_id { get; set; }
    public int? Business_id { get; set; }

    public List<SelectListItem>? Sections { get; set; }
    public List<SelectListItem>? subSections { get; set; }
    public List<SelectListItem>? Languages { get; set; }
    public List<SelectListItem>? Geographies { get; set; }
    public List<SelectListItem>? Templates { get; set; }
    public List<SelectListItem>? Language_sections { get; set; }
    public List<SelectListItem>? Language_subSections { get; set; }
    public List<SelectListItem>? Articles { get; set; }


    public string? Thumb_image { get; set; }
    public int? Thumb_image_id { get; set; }
    public bool Thumb_image_IsDelete { get; set; }

    [RegularExpression(@"^[^`\^~#<>{}?]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the mouse over text for thumbnail")]
    public string? Thumb_image_alttext { get; set; }

    public string? Small_Icon_Thumb_image { get; set; }
    public int? Small_Icon_Thumb_image_id { get; set; }
    public bool Small_Icon_Thumb_image_IsDelete { get; set; }
    [RegularExpression(@"^[^`\^~#<>{}?]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the mouse over text for thumbnail")]
    public string? Small_Icon_alttext { get; set; }

    public string? Masthead_image { get; set; }
    public int? Masthead_image_id { get; set; }
    public bool Masthead_image_IsDelete { get; set; }
    public string? Mobile_Masthead_image { get; set; }
    public int? Mobile_Masthead_image_id { get; set; }
    public bool Mobile_Masthead_image_IsDelete { get; set; }

    [RegularExpression(@"^[^`\^~#<>{}?]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the mouse over text for masthead")]
    public string? Masthead_image_alttext { get; set; }
    public string? Background_image { get; set; }
    public int? Background_image_id { get; set; }
    public bool Background_image_IsDelete { get; set; }
    [RegularExpression(@"^[^`\^~#<>{}?]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the mouse over text for masthead")]
    public string? Background_image_Alttext { get; set; }
    public string? Attach_file { get; set; }
    public int? Attach_file_id { get; set; }
    public bool Attach_file_IsDelete { get; set; }

    public bool Display_top_icon { get; set; }

    public string? Spot_temp_id { get; set; }

    [ValidateNever]
    public string? validation_error { get; set; }

    public int? Reprocess_Id { get; set; }

}

public class Page_detail
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Parent_title { get; set; }
    public string? Language { get; set; }
    public DateTime Created_date { get; set; }
    public DateTime? Updated_date { get; set; }
    public int? Status { get; set; }
    public int? Isreprocess { get; set; }
    public int Reprocess_Id { get; set; }
}

public class CMS_pages
{
    public int? current_page { get; set; }
    public string? searchquery { get; set; }
    public int? section_id { get; set; }
    public int? subSection_id { get; set; }
    public int? language_id { get; set; }
    public List<SelectListItem>? Languages { get; set; }
    public int? Geography_ID { get; set; }
    public List<SelectListItem>? Geographies { get; set; }
    public List<SelectListItem>? Sections { get; set; }
    public List<SelectListItem>? subSections { get; set; }
    public List<Page_detail>? sections_list { get; set; }
    public int section_no_of_pages { get; set; }
    public List<Page_detail>? articles_list { get; set; }
    public int? article_no_of_pages { get; set; }
    public int Content_Type_ID { get; set; }
}


public class List_CMS_pages
{
    public int Content_Type_ID { get; set; }
    public List<Page_detail>? Contents { get; set; }
    public int No_of_pages { get; set; }
    public PagedResult Objpaging { get; set; }
}




