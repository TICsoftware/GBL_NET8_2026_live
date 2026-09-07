using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Models.Manage_Model
{
    public class Main_spot_template_details_Model
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Language")]
        public int? Language_Master_ID { get; set; }

        [Required]
        [Display(Name = "Main Spot Template reference")]
        public int? main_spot_template_reference_id { get; set; }

        [Required]
        [Display(Name = "Spot Reference")]
        public int? spot_reference_id { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string? Title { get; set; }

        [Display(Name = "Intro")]
        public string? Intro { get; set; }

        [Display(Name = "Spot Content")]
        public string? Spot_Content { get; set; }

        // Thumbnail
        [Display(Name = "Thumbnail Image")]
        public int? Thumbnail_Image_Media_Id { get; set; }

        public string? Thumbnail_Image { get; set; }

        [Display(Name = "Thumbnail Alt")]
        public string? Thumbnail_Alt { get; set; }

        // Background
        [Display(Name = "Background Image")]
        public int? Background_Image_Media_Id { get; set; }

        public string? Background_Image { get; set; }

        [Display(Name = "Background Alt")]
        public string? Background_Alt { get; set; }



        // Logo
        [Display(Name = "Logo Image")]
        public string? Logo_Image { get; set; }

        public int? Logo_Image_Media_Id { get; set; }

        // External URL
        [Display(Name = "URL")]
        public string? Url { get; set; }

        public bool Is_External { get; set; }

        // Buttons
        [Display(Name = "Button One Title")]
        public string? Button_One_Title { get; set; }

        [Display(Name = "Button Two Title")]
        public string? Button_Two_Title { get; set; }

        [Display(Name = "Button One URL")]
        public string? Link_Url_Button_One { get; set; }

        public bool Botton_One_Is_External { get; set; }

        [Display(Name = "Button Two URL")]
        public string? Link_Url_Button_Two { get; set; }

        public bool Botton_Two_Is_External { get; set; }

        // Video
        [Display(Name = "Video")]
        public string? Video_Path_Media_Id { get; set; }
        public string? Video_Path { get; set; }

        [Display(Name = "Video Preview Image")]
        public int? Video_Preview_Image_Media_Id { get; set; }

        public string? Video_Preview_Image { get; set; }


        [Display(Name = "Upload File")]
        public int? Upload_File_Media_Id { get; set; }




        public string? Upload_File { get; set; }

        [Display(Name = "Display Date")]
        public DateTime? Display_Date { get; set; }

        public int? Sequence { get; set; }
        public int? Status { get; set; }

        // Audit
        public int? Created_UserID { get; set; }
        public DateTime? Created_Date { get; set; }

        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }

        public int? Published_UserID { get; set; }
        public DateTime? Published_Date { get; set; }

        public int? DeActivated_UserID { get; set; }
        public DateTime? DeActivated_Date { get; set; }

        // Custom action param (kept from your base model)
        public int Spot_RHS_Save_Action { get; set; }

    }
}