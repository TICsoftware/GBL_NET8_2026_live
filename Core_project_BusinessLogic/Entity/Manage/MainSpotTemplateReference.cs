using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity.Manage
{
    public class MainSpotTemplateReference
    {
        public int Reference_ID { get; set; }

        [Required]
        public int? Language_Master_ID { get; set; }

        [Required]
        public string Reference_Title { get; set; }

        [Required]
        public int? Template_master_id { get; set; }

        [Required]
        public int? Main_spot_template_master_id { get; set; }
        public int? Status { get; set; }

        public int? Created_UserID { get; set; }
        public DateTime? Created_Date { get; set; }

        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }

        public int? Published_UserID { get; set; }
        public DateTime? Published_Date { get; set; }

        public int? DeActivated_UserID { get; set; }
        public DateTime? DeActivated_Date { get; set; }


    }
}
