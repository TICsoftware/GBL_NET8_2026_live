using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GBL_MVC.Models
{
    public class SolutionCenterEnquiryModal : IValidatableObject
    {
        [Required(ErrorMessage = "Please enter your full name")]
        [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [RegularExpression(@"^(?=.*[a-zA-Z])[a-zA-Z][a-zA-Z\s.]*$", ErrorMessage = "Only alphabets, spaces and '.' are allowed")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,20}$", ErrorMessage = "Enter a valid mobile number")]
        public string PhoneNumber { get; set; }

        
        [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,20}$", ErrorMessage = "Enter a valid WhatsApp number")]
        public string? WhatsAppNumber { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        [StringLength(200)]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Please select gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Please select age group")]
        public string AgeGroup { get; set; }

        [Required(ErrorMessage = "Please select district")]
        public string District { get; set; }

        //[Required(ErrorMessage = "Please enter town / village")]
        [StringLength(200)]
        public string TownVillage { get; set; }

        [Required(ErrorMessage = "Please select current occupation")]
        public string CurrentOccupation { get; set; }

        [StringLength(200)]
        public string? CurrentOccupationOthers { get; set; }

        [Required(ErrorMessage = "Please select whether you own a shop/commercial space")]
        public string OwnShopOrCommercialSpace { get; set; } = "Yes";

        [Required(ErrorMessage = "Please select whether you have previously run a business")]
        public string PreviouslyRunBusiness { get; set; } = "Yes";

        [Required(ErrorMessage = "Please select whether you have space for store setup")]
        public string HaveSpaceForStoreSetup { get; set; } = "Yes";

        [StringLength(20)]
        public string? StoreSizeSqFt { get; set; }

        [StringLength(100)]
        public string? PreferredTimeForContact { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please accept the consent.")]
        public bool Consent { get; set; }



        public string OtherDistrict { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.Equals(CurrentOccupation, "Other", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(CurrentOccupationOthers))
            {
                yield return new ValidationResult(
                    "Please specify your occupation.",
                    new[] { nameof(CurrentOccupationOthers) });
            }

            if (string.Equals(HaveSpaceForStoreSetup, "Yes", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(StoreSizeSqFt))
                {
                    yield return new ValidationResult(
                        "Please enter store size in sq.ft.",
                        new[] { nameof(StoreSizeSqFt) });
                }
                else if (!System.Text.RegularExpressions.Regex.IsMatch(StoreSizeSqFt.Trim(), @"^\d+(\.\d+)?$"))
                {
                    yield return new ValidationResult(
                        "Enter a valid size in sq.ft.",
                        new[] { nameof(StoreSizeSqFt) });
                }
            }
        }
    }
}
