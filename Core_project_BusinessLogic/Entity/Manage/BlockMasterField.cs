namespace Core_project_BusinessLogic.Entity
{
    public class BlockMasterField
    {
        public int id { get; set; }
        public int? post_type_id { get; set; }
        public int? status { get; set; }
        public int? field_type_id { get; set; }
        public string? name { get; set; }
        public string? name_key { get; set; }
        public int? order_id { get; set; }
        public string? field_validation { get; set; }
        public string? validation_script { get; set; }
        public int? is_required { get; set; }
        public string? custom_validation { get; set; }
        public string? place_holder { get; set; }
        public string? help_text { get; set; }
        public string? field_options { get; set; }
        public int? allow_multiple_option { get; set; }
        public string? html_content { get; set; }
        public string? datepicker_type { get; set; }
        public string? custom_table { get; set; }
        public string? table_key_column { get; set; }
        public string? table_value_column { get; set; }
        public int? show_in_listview { get; set; }
        public string? extra_attribute { get; set; }
        public string? wrapper_classes { get; set; }
        public string? css_classes { get; set; }

        public int? Created_UserID { get; set; }
        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }
        public int? Published_UserID { get; set; }
        public DateTime? Published_Date { get; set; }
        public int? DeActivated_UserID { get; set; }
        public DateTime? DeActivated_Date { get; set; }
    }
}
