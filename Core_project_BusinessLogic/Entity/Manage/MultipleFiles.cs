namespace Core_project_BusinessLogic
{

    public class CMS_File_details
    {
        public string? Title { get; set; }
        public int ID { get; set; }
        public string? Pagename { get; set; }
        public int Sequence { get; set; }
        public string? Filepdf { get; set; }
        public DateTime UploadDate { get; set; }
        public int? cont_parent_id { get; set; }
        public int? Lang_groupid { get; set; }
        public int? Language_ID { get; set; }
        public int? Template_Master_ID { get; set; }
        public int Content_Type_ID { get; set; }
        public int? Geography_ID { get; set; }
        public int? Created_UserID { get; set; }
        public DateTime? Created_Date { get; set; }
        public int? Published_UserID { get; set; }
        public DateTime? Published_Date { get; set; }
        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }
        public string? MappedList { get; set; }
        public string? TagList { get; set; }
        public string? Uploadfilename { get; set; }
        public string? UploadFileExtension{get;set;}
        public string? UploadFileSize{get;set;}
    }
}