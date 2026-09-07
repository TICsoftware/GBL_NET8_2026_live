using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Models.Manage_Model
{
    public class SpotRHS_Model
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Language")]
        public int? Language_Master_ID { get; set; }

        [Required]
        [Display(Name = "Geography")]
        public int? Geography_ID { get; set; }

        [Required]
        [Display(Name = "Spot template master")]
        public int? spot_template_master_id { get; set; }


        public int? Spot_Template_Master_ID { get; set; }


        [Required]
        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

   
        [Display(Name = "Thumbnail image")]
        public string? Thumbnail_Img { get; set; }

        public int? Thumbnail_Img_Media_Id { get; set; }

        [Display(Name = "Thumbnail alt text")]
        public string? Thumbnail_Alt_Text { get; set; }

        [Display(Name = "Background image")]
        public string? Background_Img { get; set; }

         public int? Background_Img_Media_Id { get; set; }

        [Display(Name = "Background alt text")]
        public string? Background_Alt_Text { get; set; }

        [Display(Name = "Icon")]
        public string? Icon_Img { get; set; }

        public int? Icon_Img_Media_Id { get; set; }

        [Display(Name = "Icon alt text")]
        public string? Icon_Alt_Text { get; set; }

        [Display(Name = "Spot Intro")]
        public string? Spot_Intro { get; set; }

       

        [Display(Name = "Spot Content")]
        public string? Spot_Content { get; set; }

        [Display(Name = "Files")]
        public string? Files { get; set; }

        public int? Files_Media_Id { get; set; }

        [Display(Name = "External URL")]
        public string? External_Url { get; set; }

        public bool IsExternal { get; set; }

        public int? Status { get; set; }

        public int? Created_UserID { get; set; }
        public DateTime? Created_Date { get; set; }

        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }

        public int? Published_UserID { get; set; }
        public DateTime? Published_Date { get; set; }

        public int? DeActivated_UserID { get; set; }
        public DateTime? DeActivated_Date { get; set; }

        public int? Sequence { get; set; }

        // Custom action param
        public int Spot_RHS_Save_Action { get; set; }

    }
}