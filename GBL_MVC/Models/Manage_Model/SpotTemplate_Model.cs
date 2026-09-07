using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Models.Manage_Model
{
    public class SpotTemplate_Model
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please select language")]
        [Display(Name = "Language")]
        [DefaultValue(0)]
        public int? Language_Master_ID { get; set; }


        [Required(ErrorMessage = "Please select block layout title")]
        [Display(Name = "Block layout title")]
        [DefaultValue(0)]
        public int? Template_Type_ID { get; set; }


        // [Required(ErrorMessage = "Please select spot reference")]
        // [Display(Name = "Spot reference")]
        // [DefaultValue(0)]
        // public int? spot_template_master_Id { get; set; }
        

        [Required(ErrorMessage = "Please enter name")]
        [Display(Name = "Name")]
        public string? Name { get; set; }



        [Display(Name = "Thumbnail Image")]
        public string? Thumbnail_Image { get; set; }
        public int? Thumbnail_Image_Media_Id { get; set; }

      

        public int? Status { get; set; }

        public int? Created_UserID { get; set; }

        public DateTime? Created_Date { get; set; }

        public int? Updated_UserID { get; set; }

        public DateTime? Updated_Date { get; set; }

        public int? Published_UserID { get; set; }

        public DateTime? Published_Date { get; set; }

        public int? DeActivated_UserID { get; set; }

        public DateTime? DeActivated_Date { get; set; }

        public int Spot_Template_Master_Save_Action { get; set; }

    }
}