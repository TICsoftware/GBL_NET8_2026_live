using System;
using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class TagMaster
    {
        public int ID { get; set; }
  
        [Required(ErrorMessage = "Tag name is required.")]
        public string? Tag_name{get;set;}
        
        [Required(ErrorMessage = "Language is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid language.")]
        public int? Language_Master_ID { get; set; }
         
        [Required(ErrorMessage = "Status is required.")]
        [Range(0, 1, ErrorMessage = "Please select a valid status.")]
        public int Status { get; set; }

        public string? Languauge_Name  {get;set;}
        public DateTime? Created_Date { get; set; }
        public DateTime? Updated_Date { get; set; }


    }
}
