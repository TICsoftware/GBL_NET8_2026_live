namespace Core_project_BusinessLogic.Entity
{
    public class Reports_Master
    {

    }


    public class Report_type_Master
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Final_msg { get; set; }
        public string? Mail_subject_line { get; set; }
        public string? Mail_content { get; set; }
        public List<int>? Categories { get; set; }
    }

    public class Report_Category_Master
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Mail_title { get; set; }
        public int? Parent_id { get; set; }
    }
}