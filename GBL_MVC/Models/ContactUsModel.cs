using System;
using System.ComponentModel.DataAnnotations;

public class ContactUsModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    [Required]
    [StringLength(200)]
    public string CompanyName { get; set; }

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string EmailAddress { get; set; }

    [Required]
    public int CountryId { get; set; }

    [Required]
    [StringLength(200)]
    public string SiteLocation { get; set; }

    [StringLength(500)]
    public string Query { get; set; }

    public DateTime CreatedDate { get; set; }

    public int Status { get; set; }

    
    public string? IPAddress { get; set; }

    public bool? Consent { get; set; }
}