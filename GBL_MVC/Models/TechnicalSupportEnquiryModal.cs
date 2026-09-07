using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Models
{
    public class TechnicalSupportEnquiryModal
    {
        public int Id { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Designation { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,20}$", ErrorMessage = "Enter a valid mobile number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(200)]
        public string EmailAddress { get; set; }


        public int CityId { get; set; }

        [Required]
        public int StateId { get; set; }

        public string State { get; set; }

        [Required(ErrorMessage = "Please enter city")]
        [StringLength(200)]
        public string City { get; set; }

        [Required]
        public int TestTypeId { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept the consent.")]
        public bool Consent { get; set; }






    }
}