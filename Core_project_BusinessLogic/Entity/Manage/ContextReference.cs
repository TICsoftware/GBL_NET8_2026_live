using System;

namespace Core_project_BusinessLogic.Entity
{
    public class ContextReference
    {
        public int ID { get; set; }
        public int? Language_Master_ID { get; set; }
        public int? Context_Master_ID { get; set; }
         public int? Template_Master_ID { get; set; }
         public string Title { get; set; }
        public string Reference_Title { get; set; }
        public bool HasData { get; set; }
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
