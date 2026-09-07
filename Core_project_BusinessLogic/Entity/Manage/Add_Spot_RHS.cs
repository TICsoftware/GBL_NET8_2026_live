using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_project_BusinessLogic.Entity.Manage
{
    public class Add_Spot_RHS
    {
        public int ID { get; set; }
        public int? Language_Master_ID { get; set; }

        public int? Geography_ID { get; set; }

        public int? Template_Type_ID { get; set; }

        public string? Template_Type { get; set; }

        public int? spot_template_master_id { get; set; }
        public string? SpotTemplate { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Thumbnail_Img { get; set; }
        public int? Thumbnail_Img_Media_Id { get; set; }

        public string? thumbnail_Alt_Text { get; set; }

        public string? background_Img { get; set; }
         public int? background_Img_Media_Id { get; set; }

        public string? background_Alt_Text { get; set; }

        public string? icon_Img { get; set; }
        public int? icon_Img_Media_Id { get; set; }

        public string? icon_Alt_Text { get; set; }

        public string? Spot_Intro { get; set; }
        public string? Spot_content { get; set; }

        public string? Design_layout { get; set; }


        public string? Files { get; set; }
        public int? Files_Media_Id { get; set; }
        public string? External_Url { get; set; }
        public bool? Isexternal { get; set; }


        public int? Status { get; set; }

        public string? Content_Status { get; set; }

        public int? Created_UserID { get; set; }

        public DateTime? Created_Date { get; set; }

        public int? Updated_UserID { get; set; }

        public DateTime? Updated_Date { get; set; }

        public int? Published_UserID { get; set; }

        public DateTime? Published_Date { get; set; }

        public int? DeActivated_UserID { get; set; }

        public DateTime? DeActivated_Date { get; set; }

        public int? sequence { get; set; }





    }
}