using Microsoft.AspNetCore.Mvc.Rendering;

namespace GBL_MVC
{
    public class UploadFiles
    {
        public int Language_master_id { get; set; }
        public List<SelectListItem>? Languages { get; set; }
        public int? Content_Type_ID { get; set; }
        public int Template_id { get; set; }
        public List<SelectListItem>? Templates { get; set; }
        public int Geography_ID { get; set; }
        public List<SelectListItem>? Geographies { get; set; }
        public List<SelectListItem>? Sections { get; set; }
        public int Section_id { get; set; }
        public List<SelectListItem>? subSections { get; set; }
        public int Subsection_id { get; set; }
        public List<SelectListItem>? Language_sections { get; set; }
        public int Language_section_id { get; set; }
        public List<SelectListItem>? Language_subSections { get; set; }
        public int Language_subSection_id { get; set; }
        public int article_id { get; set; }
        public List<SelectListItem>? Articles { get; set; }
        public List<SelectedFiles>? FilesList { get; set; }
        public List<SelectListItem>? Mapping_sections { get; set; }
        public int[]? SelectedMapping_sections { get; set; }
        public List<SelectListItem>? Tagging_sections { get; set; }
        public int[]? SelectedTagging_sections { get; set; }
    }

    public class SelectedFiles
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime? Filedate { get; set; }
        public string? Filepagename { get; set; }
        public int Sequence { get; set; }
    }
}