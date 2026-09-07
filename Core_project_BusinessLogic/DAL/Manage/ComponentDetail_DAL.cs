using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class ComponentDetail_DAL : DBHelper
    {
        public ComponentDetail_DAL(IConfiguration config) : base(config) { }


        public ComponentModel GetComponent(int main_component_master_id)
        {
            SqlParameter[] p = { new SqlParameter("@main_component_master_id", main_component_master_id) };

            DataSet ds = GetDataSet("GetComponenetsbyId", p);

            ComponentModel component = new ComponentModel();

            // Template Table
            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow r = ds.Tables[0].Rows[0];

                component.Template = new ComponentTemplate
                {
                    Id = Convert.ToInt32(r["ID"]),
                    TemplateName = r["Name"].ToString(),
                    LayoutName = r["Design_layout"].ToString()
                };
            }

            // Fields Table
            List<ComponentFieldDefinition> fields = new List<ComponentFieldDefinition>();

            foreach (DataRow r in ds.Tables[1].Rows)
            {
                fields.Add(new ComponentFieldDefinition
                {
                    id = Convert.ToInt32(r["id"]),
                    field_type_id = Convert.ToInt32(r["field_type_id"]),
                    name = r["name"].ToString(),
                    name_key = r["name_key"].ToString(),
                    is_required = r["is_required"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["is_required"]),
                    field_options = r["field_options"].ToString(),
                    place_holder = r["place_holder"].ToString()
                });
            }

            component.Fields = fields;

            return component;
        }



        public List<ContextFieldWithValue> GetFieldsWithValuesByTemplateReference(int ctrId)
        {
            SqlParameter[] p =
            {
        new SqlParameter("@context_template_reference_id", ctrId)
    };

            DataTable dt = GetDataSet("GetFieldsWithValuesByTemplateReference", p).Tables[0];

            List<ContextFieldWithValue> list = new List<ContextFieldWithValue>();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new ContextFieldWithValue
                {
                    field_id = Convert.ToInt32(r["field_id"]),
                    name = r["name"].ToString(),
                    name_key = r["name_key"].ToString(),
                    field_type_id = r["field_type_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["field_type_id"]),
                    field_options = r["field_options"]?.ToString(),
                    place_holder = r["place_holder"]?.ToString(),
                    help_text = r["help_text"]?.ToString(),
                    is_required = r["is_required"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["is_required"]),

                    // 🔹 Existing saved data
                    detail_id = r["detail_id"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["detail_id"]),
                    content = r["content"]?.ToString(),
                    File_path = r["File_path"]?.ToString(),
                    File_path_name = r["File_path_name"]?.ToString()
                });
            }

            return list;
        }


        public List<ContextFieldDefinition> GetFieldsByTemplateReference(int ctrId)
        {
            SqlParameter[] p =
            {
        new SqlParameter("@context_template_reference_id", ctrId)
    };

            DataTable dt = GetDataSet("GetFieldsByTemplateReference", p).Tables[0];

            return dt.AsEnumerable().Select(MapField).ToList();
        }
        private ContextFieldDefinition MapField(DataRow r)
        {
            return new ContextFieldDefinition
            {
                id = Convert.ToInt32(r["id"]),
                field_type_id = r["field_type_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["field_type_id"]),
                name = r["name"]?.ToString(),
                name_key = r["name_key"]?.ToString(),

                // image_previewname = r.Table.Columns.Contains("image_previewname")
                //     ? r["image_previewname"]?.ToString()
                //     : null,

                // Thumbnail_Img_Media_Id = r.Table.Columns.Contains("Thumbnail_Img_Media_Id")
                //     && r["Thumbnail_Img_Media_Id"] != DBNull.Value
                //     ? (int?)Convert.ToInt32(r["Thumbnail_Img_Media_Id"])
                //     : null,

                is_required = r.Table.Columns.Contains("is_required")
                    && r["is_required"] != DBNull.Value
                    ? (int?)Convert.ToInt32(r["is_required"])
                    : null,

                field_options = r.Table.Columns.Contains("field_options")
                    ? r["field_options"]?.ToString()
                    : null,

                place_holder = r.Table.Columns.Contains("place_holder")
                    ? r["place_holder"]?.ToString()
                    : null,

                help_text = r.Table.Columns.Contains("help_text")
                    ? r["help_text"]?.ToString()
                    : null,

                field_validation = r.Table.Columns.Contains("field_validation")
                    ? r["field_validation"]?.ToString()
                    : null
            };
        }

        public void InsertDetail(ContextDetail d)
        {
            SqlParameter[] p =
            {
            new("@Language_Master_ID", d.Language_Master_ID),
            new("@context_master_id", d.context_master_id),
            new("@context_template_reference_id", d.context_template_reference_id),
            new("@context_field_id", d.context_field_id),
            new("@content", d.content ?? (object)DBNull.Value),
            new("@File_path", d.File_path ?? (object)DBNull.Value),
            new("@Created_UserID", d.Created_UserID ?? 1),
            new("@context_group_id", d.context_group_id),  //  NEW
        };

            SQLInsert_Update_Delete_Data("InsertContextDetail", p);
        }
        public List<ContextFieldWithValue> GetFieldsByGroup(int ctrId, string groupId)
        {
            SqlParameter[] p =
            {
        new("@ctrId", ctrId),
        new("@groupId", groupId)
    };

            DataTable dt = GetDataSet("GetFieldsByGroup", p).Tables[0];
            List<ContextFieldWithValue> list = new();
            return dt.AsEnumerable().Select(MapFieldWithValue).ToList();


        }


        private ContextFieldWithValue MapFieldWithValue(DataRow r)
        {
            return new ContextFieldWithValue
            {
                field_id = r["field_id"] == DBNull.Value ? 0 : Convert.ToInt32(r["field_id"]),
                name = r["name"]?.ToString(),
                name_key = r["name_key"]?.ToString(),

                field_type_id = r["field_type_id"] == DBNull.Value
                    ? 1
                    : Convert.ToInt32(r["field_type_id"]),

                place_holder = r.Table.Columns.Contains("place_holder")
                    ? r["place_holder"]?.ToString()
                    : "",

                help_text = r.Table.Columns.Contains("help_text")
                    ? r["help_text"]?.ToString()
                    : "",

                is_required = r.Table.Columns.Contains("is_required") && r["is_required"] != DBNull.Value
                    ? (int?)Convert.ToInt32(r["is_required"])
                    : null,

                // 🔹 Context Detail Data
                detail_id = r.Table.Columns.Contains("detail_id") && r["detail_id"] != DBNull.Value
                    ? (int?)Convert.ToInt32(r["detail_id"])
                    : null,

                content = r.Table.Columns.Contains("content")
                    ? r["content"]?.ToString()
                    : "",

                File_path = r.Table.Columns.Contains("File_path")
                    ? r["File_path"]?.ToString()
                    : "",

                context_group_id = r.Table.Columns.Contains("context_group_id")
                    ? r["context_group_id"]?.ToString()
                    : ""
            };
        }
        public List<ContextFieldWithValue> GetFieldsForEdit(int referenceId)
        {
            SqlParameter[] p = {
        new SqlParameter("@context_reference_id", referenceId)
    };

            DataTable dt = GetDataSet("GetContextDetailsForEdit", p).Tables[0];

            List<ContextFieldWithValue> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new ContextFieldWithValue
                {
                    field_id = Convert.ToInt32(r["field_id"]),
                    name = r["name"].ToString(),
                    name_key = r["name_key"].ToString(),
                    field_type_id = Convert.ToInt32(r["field_type_id"]),
                    field_options = r["field_options"].ToString(),
                    place_holder = r["place_holder"].ToString(),
                    help_text = r["help_text"].ToString(),
                    field_validation = r["field_validation"].ToString(),
                    is_required = r["is_required"] as int?,
                    detail_id = r["detail_id"] as int?,
                    content = r["content"].ToString(),
                    File_path = r["File_path"].ToString(),
                    File_path_name = r["field_media_path"].ToString()
                });
            }

            return list;
        }

        public void UpdateDetail(ContextFieldWithValue v)
        {
            SqlParameter[] p =
            {
        new("@detail_id", v.detail_id),
        new("@content", v.content ?? (object)DBNull.Value),
        new("@file_path", v.File_path ?? (object)DBNull.Value),
        new("@Updated_UserID", 1)
    };

            SQLInsert_Update_Delete_Data("UpdateContextDetail", p);
        }


    }
}
