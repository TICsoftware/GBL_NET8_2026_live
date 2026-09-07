using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace Core_project_BusinessLogic.BAL
{
    public class MainComponentField_BAL
    {
        private readonly MainComponentField_DAL dal;

        public MainComponentField_BAL(IConfiguration config)
        {
            dal = new MainComponentField_DAL(config);
        }

        public List<ComponentMasterField> GetAll()
        {
            DataTable dt = dal.GetAll_DAL();
            List<ComponentMasterField> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new ComponentMasterField
                {
                    id = Convert.ToInt32(row["id"]),
                    
                    status = row["status"] as int?,
                    field_type_id = row["field_type_id"] as int?,
                    name = row["name"].ToString(),
                    name_key = row["name_key"].ToString(),
                    order_id = row["order_id"] as int?,
                    field_validation = row["field_validation"].ToString(),
                    validation_script = row["validation_script"].ToString(),
                    is_required = row["is_required"] as int?,
                    custom_validation = row["custom_validation"].ToString(),
                    place_holder = row["place_holder"].ToString(),
                    help_text = row["help_text"].ToString(),
                    field_options = row["field_options"].ToString(),
                    allow_multiple_option = row["allow_multiple_option"] as int?,
                    html_content = row["html_content"].ToString(),
                    datepicker_type = row["datepicker_type"].ToString(),
                    custom_table = row["custom_table"].ToString(),
                    table_key_column = row["table_key_column"].ToString(),
                    table_value_column = row["table_value_column"].ToString(),
                    show_in_listview = row["show_in_listview"] as int?,
                    extra_attribute = row["extra_attribute"].ToString(),
                    wrapper_classes = row["wrapper_classes"].ToString(),
                    css_classes = row["css_classes"].ToString()
                });
            }

            return list;
        }

        public ComponentMasterField GetById(int id)
        {
            var dt = dal.GetById_DAL(id);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];

           return new ComponentMasterField
{
    id = Convert.ToInt32(row["id"]),   
    status = row["status"] as int?,
    field_type_id = row["field_type_id"] as int?,
    name = row["name"].ToString(),
    name_key = row["name_key"].ToString(),
    order_id = row["order_id"] as int?,
    field_validation = row["field_validation"].ToString(),
    validation_script = row["validation_script"].ToString(),
    is_required = row["is_required"] as int?,
    custom_validation = row["custom_validation"].ToString(),
    place_holder = row["place_holder"].ToString(),
    help_text = row["help_text"].ToString(),
    field_options = row["field_options"].ToString(),
    allow_multiple_option = row["allow_multiple_option"] as int?,
    html_content = row["html_content"].ToString(),
    datepicker_type = row["datepicker_type"].ToString(),
    custom_table = row["custom_table"].ToString(),
    table_key_column = row["table_key_column"].ToString(),
    table_value_column = row["table_value_column"].ToString(),
    show_in_listview = row["show_in_listview"] as int?,
    extra_attribute = row["extra_attribute"].ToString(),
    wrapper_classes = row["wrapper_classes"].ToString(),
    css_classes = row["css_classes"].ToString(),
};
        }

        public void Insert(ComponentMasterField m) => dal.Insert_DAL(m);

        public void Update(ComponentMasterField m) => dal.Update_DAL(m);

        public void Deactivate(int id, int userId) => dal.Deactivate_DAL(id, userId);
    }
}
