namespace GBL_BusinessLogic.Entity
{
    public class ProductEnquiry
    {
        public int EnquiryId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductPageName { get; set; }
        public string? FullName { get; set; }
        public string? CompanyName { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? CountryCode { get; set; }
        public string? Mobile { get; set; }
        public string? Fax { get; set; }
        public string? Email { get; set; }
        public string? BusinessType { get; set; }
        public string? EnquiryDetails { get; set; }
        public bool AcceptTerms { get; set; }
        public bool NotRobot { get; set; }
        public string? IPAddress { get; set; }
    }

    public class ProductEnquiryPageModel
    {
        public string? ProductName { get; set; }
        public string? ProductPageName { get; set; }
        public string? ProductUrl { get; set; }
    }
}
