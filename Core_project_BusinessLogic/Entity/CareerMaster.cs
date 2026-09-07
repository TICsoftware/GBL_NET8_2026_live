namespace Core_project_BusinessLogic.Entity
{
    public class CareerMaster_CMS
    {
        public int Job_Id { get; set; }
        public string? Role { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public string? Job_Description { get; set; }
        public string? Location { get; set; }
        public string? Salary_range { get; set; }
        public string? About_the_Role { get; set; }
        public string? Workmode { get; set; }
        public DateTime? Expiry_date { get; set; }
        public DateTime? Created_date { get; set; }
    }

    public class Job_List
    {
        public List<CareerMaster_CMS>? jobList { get; set; }
        public int TotalRecords { get; set; }
    }
}