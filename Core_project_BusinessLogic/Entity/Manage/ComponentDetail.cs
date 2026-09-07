using System;

namespace Core_project_BusinessLogic.Entity
{
    public class ComponentDetail
    {
        public int ID { get; set; }
        public int? Language_Master_ID { get; set; }
        public int? Component_master_id { get; set; }
        public int? Component_reference_id { get; set; }
        public int? Component_template_reference_id { get; set; }

        public Guid Component_group_id { get; set; }
        public int? Component_field_id { get; set; }
        public string content { get; set; }
        public string File_path { get; set; }

        public int? Created_UserID { get; set; }
    }
    public class ComponentFieldWithValue
    {
        public int field_id { get; set; }
        public int Component_master_id { get; set; }

        public string name { get; set; }
        public string name_key { get; set; }
        public int field_type_id { get; set; }
        public string field_options { get; set; }
        public string place_holder { get; set; }
        public string help_text { get; set; }
        public string field_validation { get; set; }
        public int? is_required { get; set; }

        public int? detail_id { get; set; }
        public string content { get; set; }
        public string File_path { get; set; }

        public string File_path_name { get; set; }
        public string Component_group_id { get; set; }
    }

    public class ComponentFieldDefinition
    {
        public int id { get; set; }
        public int field_type_id { get; set; }
        public string name { get; set; }
        public string name_key { get; set; }

        public string image_previewname { get; set; }
        public int? Thumbnail_Img_Media_Id { get; set; }
        public int? is_required { get; set; }
        public string field_options { get; set; }
        public string place_holder { get; set; }
        public string help_text { get; set; }
        public string field_validation { get; set; }



        // =============================
        // CONTEXT DETAILS (DATA VALUES)
        // =============================

        public int? detail_id { get; set; }          // context_details.ID
        public int? field_id { get; set; }           // context_field_id

        public int? Component_reference_id { get; set; }
        public int? Component_master_id { get; set; }

        public string content { get; set; }
        public string File_path { get; set; }        // image/file path

        // For edit image logic
        public string existing_media { get; set; }
        public string new_media { get; set; }

        // =============================
        // DISPLAY / UI HELPERS
        // =============================

        public int? order_id { get; set; }
        public int? allow_multiple_option { get; set; }

        // Dropdown / checkbox parsed options
        public string[] OptionsArray { get; set; }


    }

    public class ComponentTemplate
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string LayoutName { get; set; }
    }

    public class ComponentModel
    {
        public ComponentTemplate Template { get; set; }
        public List<ComponentFieldDefinition> Fields { get; set; }
    }

}
