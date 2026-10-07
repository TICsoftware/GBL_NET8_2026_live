namespace GBL_BusinessLogic.Entity
{
    public class WorkWithUsEnquiry
    {
        public int EnquiryId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Expertise { get; set; }
        public string? Address { get; set; }
        public string? Designation { get; set; }
        public string? ResumePath { get; set; }
        public string? Message { get; set; }
        public bool NotRobot { get; set; }
        public string? IPAddress { get; set; }
    }
}
