using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class TechnicalSupportEnquiry
    {
        public int Id { get; set; }

        public int ServiceTypeId { get; set; }

        public string Name { get; set; }

        public string Designation { get; set; }

        public string CompanyName { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public int CityId { get; set; }

        public string City { get; set; }

        public int StateId { get; set; }

        public string State{ get; set; }

        public int TestTypeId { get; set; }

        public string IPAddress { get; set; }

        public bool Consent{ get; set; }


    }
}