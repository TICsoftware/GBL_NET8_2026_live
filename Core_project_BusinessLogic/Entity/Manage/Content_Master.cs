using System;
using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic;

public class Content_Master
{
    public int id { get; set; }

    [Required]
    public string title { get; set; } = string.Empty;
    public int? root_parent_id { get; set; }
    public int? parent_id { get; set; }
    public int? article_id { get; set; }
    public int? Template_Master_ID { get; set; }
    public string? Template_name { get; set; }
    public int? Template_type { get; set; }
    public int? Content_Type_ID { get; set; }
    public string? hmpg_title { get; set; }
    public string? intro { get; set; }
    public string? hmpg_intro { get; set; }
    public string? content { get; set; }

    [Required]
    public string pagename { get; set; } = string.Empty;
    public DateTime? displaydate { get; set; }
    public string? window_title { get; set; }
    public int? sequence { get; set; }
    public string? external_url { get; set; }
    public int? IsExternal { get; set; }
    public string? metatag { get; set; }
    public string? metadesc { get; set; }
    public string? page_schema { get; set; }
    public DateTime? metaexpiry { get; set; }
    public int? status { get; set; }
    public bool? IsInProcess { get; set; }
    public int? language_master_id { get; set; }
    public int? lang_groupid { get; set; }
    public int? Language_root_parent_id { get; set; }
    public int? Language_subsection_id { get; set; }
    public int? Geography_ID { get; set; }
    public string? ByLine { get; set; }
    public string? Publication { get; set; }
    public int? companies_id { get; set; }
    public int? isSearch { get; set; }
    public int? top_icon { get; set; }
    public string? search_url { get; set; }
    public string? page_url { get; set; }
    public string? breadcrumb_title { get; set; }
    public int? loc_id { get; set; }
    public int? business_id { get; set; }
    public int? Thumb_image_id { get; set; }
    public string? Thumb_image { get; set; }
    public string? Thumb_image_alttext { get; set; }
    public int? Small_Icon_Thumb_image_id { get; set; }
    public string? Small_Icon_Thumb_image { get; set; }
    public string? Small_Icon_alttext { get; set; }
    public int? Masthead_image_id { get; set; }
    public string? Masthead_image { get; set; }
    public int? Mobile_Masthead_image_id { get; set; }
    public string? Mobile_Masthead_image { get; set; }
    public string? Masthead_image_alttext { get; set; }
    public int? Background_image_id { get; set; }
    public string? Background_image { get; set; }
    public string? Background_image_Alttext { get; set; }
    public int? Attach_file_id { get; set; }
    public string? Attach_file { get; set; }
    public string? Spot_temp_id { get; set; }
    public int? reprocess_id { get; set; }

}

public class Options_List
{
    public int? id { get; set; }
    public int? parent_id { get; set; }
    public string? title { get; set; }
}

public class CMS_pageload
{
    public List<Options_List>? Sections { get; set; }
    //public List<Options_List>? SubSections { get; set; }
    public List<Options_List>? Languages { get; set; }
    public List<Options_List>? geographies { get; set; }
    public List<Options_List>? Templates { get; set; }
    public List<Options_List>? Language_sections { get; set; }
    public List<Options_List>? Subsections { get; set; }
    public List<Options_List>? sect_Articles { get; set; }
    public List<Options_List>? Language_subSections { get; set; }
}



public class SubSections_Articles
{
    public List<Options_List>? Subsections { get; set; }
    public List<Options_List>? sect_Articles { get; set; }

}


public class CMS_Page_detail
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Parent_title { get; set; }
    public string? Language { get; set; }
    public DateTime Created_date { get; set; }
    public DateTime? Updated_date { get; set; }
    public int Status { get; set; }
    public int IsReprocessed {get;set;}
    public int Reprocess_Id{get;set;}
    

}

public class Manage_List_CMS_page
{
    public int Sections_no_of_pages { get; set; }
    public List<CMS_Page_detail>? sections { get; set; }
    public int? Articles_no_of_pages { get; set; }
    public List<CMS_Page_detail>? articles { get; set; }
}


