using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_project_BusinessLogic.Entity.Manage
{
    public class Add_Spot_Template_Master
    {
        public int ID { get; set; }
        public int? Language_Master_ID{ get; set; }

        public int? Geography_ID{ get; set; }

        public string? Name { get; set; }

        public string? Thumbnail_Image { get; set; }

        public int? Template_Type_ID { get; set; }

         public string? Template_Type { get; set; }

        public int? Status { get; set; }

        public string? Content_Status{ get; set; }

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