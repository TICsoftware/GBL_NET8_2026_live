using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class ContactModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();


        public List<ComponentModel> Registered_Corporate_Office_List { get; set; } = new();
        public List<ComponentModel> Marketing_Office_List { get; set; } = new();
        public List<ComponentModel> Products_List { get; set; } = new();
        public List<ComponentModel> India_Offices_List { get; set; } = new();
        public List<ComponentModel> InternationalOffices_List { get; set; } = new();


    }


    public class ContactEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int StateId { get; set; }
        public string SiteLocation { get; set; }
        public string Query { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public string IPAddress { get; set; }
        public bool? Consent { get; set; }
    }

    public class StateMaster
    {
        public int StateId { get; set; }
        public string StateName { get; set; }
        public bool Status { get; set; }
        public DateTime? CreateDate { get; set; }
    }

}
