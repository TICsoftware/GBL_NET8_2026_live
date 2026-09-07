namespace Core_project_BusinessLogic.Entity.Manage
{
    public class MainSpotTemplateMaster
    {
        public int ID { get; set; }
        public int? Language_Master_ID { get; set; }

         public string? Language_Name{ get; set; }
        public string Name { get; set; }
        //public int? Template_Type_ID { get; set; }
        public int? Status { get; set; }

        public int? Created_UserID { get; set; }
        public DateTime? Created_Date { get; set; }

        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }

        public int? Published_UserID { get; set; }
        public DateTime? Published_Date { get; set; }

        public int? DeActivated_UserID { get; set; }
        public DateTime? DeActivated_Date { get; set; }

        public string? Design_Layout { get; set; }

          public List<int> SelectedTemplateIds { get; set; } = new();
          public string? TemplateNames { get; set; }
           
    }

    public class MainComponentTemplateReference
{
    public int ID { get; set; }
    public int main_spot_template_master_id { get; set; }  
    public int template_master_id { get; set; }
    public int? Status { get; set; }
}
}
