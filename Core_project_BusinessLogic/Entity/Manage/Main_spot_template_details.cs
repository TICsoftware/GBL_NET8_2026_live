using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_project_BusinessLogic.Entity.Manage
{
    public class Main_spot_template_details
    {
        public int ID { get; set; }
        public int? Language_Master_ID { get; set; }

        public int? main_spot_template_reference_id { get; set; }
        public string? Main_Spot_Template_Reference { get; set; }

        public int? spot_reference_id { get; set; }
        public string? spot_reference_name { get; set; }

        public string? Title { get; set; }

        public string? Intro { get; set; }

        public string? Spot_Content { get; set; }

        public int? Thumbnail_Image_Media_Id { get; set; }

        public string? Thumbnail_Image { get; set; }

        public string? Thumbnail_Alt { get; set; }

        public int? Background_Image_Media_Id { get; set; }

        public string? Background_Image { get; set; }

        public string? Background_Alt { get; set; }

        public string? Logo_Image { get; set; }

        public int? Logo_Image_Media_Id { get; set; }

        public string? Url { get; set; }

        public bool Is_External { get; set; }

        public string? Button_One_Title { get; set; }

        public string? Button_Two_Title { get; set; }

        public string? Link_Url_Button_One { get; set; }

        public bool Botton_One_Is_External { get; set; }

        public string? Link_Url_Button_Two { get; set; }

        public bool Botton_Two_Is_External { get; set; }

        public string? Video_Path_Media_Id { get; set; }
        public string? Video_Path { get; set; }

        public int? Video_Preview_Image_Media_Id { get; set; }

        public string? Video_Preview_Image { get; set; }

        public int? Upload_File_Media_Id { get; set; }


        public string? Upload_File { get; set; }

        public DateTime? Display_Date { get; set; }

        public int? Sequence { get; set; }
        public int? Status { get; set; }

        public string? Content_Status { get; set; }

        // Audit
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