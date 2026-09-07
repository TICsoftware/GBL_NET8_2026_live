using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class MainComponentField_DAL : DBHelper
    {
        public MainComponentField_DAL(IConfiguration configuration) : base(configuration) { }

        public DataTable GetAll_DAL()
        {
            return GetDataSet("GetAllMainComponentFields", new SqlParameter[0]).Tables[0];
        }

        public DataTable GetById_DAL(int id)
        {
            SqlParameter[] param = { new SqlParameter("@id", id) };
            return GetDataSet("GetComponenetFieldById", param).Tables[0];
        }

      public void Insert_DAL(ComponentMasterField f)
{
    SqlParameter[] param =
    {
        new("@status", f.status ?? (object)DBNull.Value),
        new("@field_type_id", f.field_type_id ?? (object)DBNull.Value),
        new("@name", (object?)f.name ?? DBNull.Value),
        new("@name_key", (object?)f.name_key ?? DBNull.Value),
        new("@order_id", f.order_id ?? 0),

        new("@field_validation", (object?)f.field_validation ?? DBNull.Value),
        new("@validation_script", (object?)f.validation_script ?? DBNull.Value),

        new("@is_required", f.is_required ?? 0),
        new("@custom_validation", (object?)f.custom_validation ?? DBNull.Value),

        new("@place_holder", (object?)f.place_holder ?? DBNull.Value),
        new("@help_text", (object?)f.help_text ?? DBNull.Value),

        new("@field_options", (object?)f.field_options ?? DBNull.Value),
        new("@allow_multiple_option", f.allow_multiple_option ?? 0),

        new("@html_content", (object?)f.html_content ?? DBNull.Value),
        new("@datepicker_type", (object?)f.datepicker_type ?? DBNull.Value),

        new("@custom_table", (object?)f.custom_table ?? DBNull.Value),
        new("@table_key_column", (object?)f.table_key_column ?? DBNull.Value),
        new("@table_value_column", (object?)f.table_value_column ?? DBNull.Value),

        new("@show_in_listview", f.show_in_listview ?? 0),
        new("@extra_attribute", (object?)f.extra_attribute ?? DBNull.Value),
        new("@wrapper_classes", (object?)f.wrapper_classes ?? DBNull.Value),
        new("@css_classes", (object?)f.css_classes ?? DBNull.Value),

        new("@Created_UserID", f.Created_UserID ?? 1)
    };

    GetDataSet("InsertComponentField", param);
}


      public void Update_DAL(ComponentMasterField f)
{
    SqlParameter[] param =
    {
        new("@id", f.id),

        new("@status", f.status ?? (object)DBNull.Value),
        new("@field_type_id", f.field_type_id ?? (object)DBNull.Value),
        new("@name", (object?)f.name ?? DBNull.Value),
        new("@name_key", (object?)f.name_key ?? DBNull.Value),
        new("@order_id", f.order_id ?? 0),

        new("@field_validation", (object?)f.field_validation ?? DBNull.Value),
        new("@validation_script", (object?)f.validation_script ?? DBNull.Value),

        new("@is_required", f.is_required ?? 0),
        new("@custom_validation", (object?)f.custom_validation ?? DBNull.Value),

        new("@place_holder", (object?)f.place_holder ?? DBNull.Value),
        new("@help_text", (object?)f.help_text ?? DBNull.Value),

        new("@field_options", (object?)f.field_options ?? DBNull.Value),
        new("@allow_multiple_option", f.allow_multiple_option ?? 0),

        new("@html_content", (object?)f.html_content ?? DBNull.Value),
        new("@datepicker_type", (object?)f.datepicker_type ?? DBNull.Value),

        new("@custom_table", (object?)f.custom_table ?? DBNull.Value),
        new("@table_key_column", (object?)f.table_key_column ?? DBNull.Value),
        new("@table_value_column", (object?)f.table_value_column ?? DBNull.Value),

        new("@show_in_listview", f.show_in_listview ?? 0),
        new("@extra_attribute", (object?)f.extra_attribute ?? DBNull.Value),
        new("@wrapper_classes", (object?)f.wrapper_classes ?? DBNull.Value),
        new("@css_classes", (object?)f.css_classes ?? DBNull.Value),

        new("@Updated_UserID", f.Updated_UserID ?? 1)
    };

    SQLInsert_Update_Delete_Data("UpdateComponentField", param);
}

        public void Deactivate_DAL(int id, int userId)
        {
            SqlParameter[] p =
            {
                new("@id", id),
                new("@UserID", userId)
            };

            SQLInsert_Update_Delete_Data("DeactivateComponentField", p);
        }
    }
}
