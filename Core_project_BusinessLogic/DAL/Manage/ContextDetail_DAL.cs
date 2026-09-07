using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class ContextDetail_DAL : DBHelper
    {
        public ContextDetail_DAL(IConfiguration config) : base(config) { }

        public List<ContextFieldDefinition> GetFieldsForReference(int referenceId)
        {
            SqlParameter[] p = { new SqlParameter("@context_reference_id", referenceId) };
            DataTable dt = GetDataSet("GetFieldsForReference", p).Tables[0];

            List<ContextFieldDefinition> list = new();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new ContextFieldDefinition
                {
                    id = Convert.ToInt32(r["id"]),
                    field_type_id = Convert.ToInt32(r["field_type_id"]),
                    name = r["name"].ToString(),
                    name_key = r["name_key"].ToString(),
                    is_required = r["is_required"] as int?,
                    field_options = r["field_options"].ToString(),
                    place_holder = r["place_holder"].ToString(),
                    help_text = r["help_text"].ToString(),
                    field_validation = r["field_validation"].ToString()
                });
            }
            return list;
        }
        public List<ContextFieldWithValue> GetFieldsWithValuesByTemplateReference(int ctrId, int is_block)
        {
            SqlParameter[] p =
            {
        new ("@context_template_reference_id", ctrId),
          new("@is_block",is_block ),
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


        public List<ContextFieldDefinition> GetFieldsByTemplateReference(int ctrId, int is_block)
        {
            SqlParameter[] p =
            {
        new ("@context_template_reference_id", ctrId),
        new ("@is_block", is_block)
    };

            DataTable dt = GetDataSet("GetFieldsByTemplateReference", p).Tables[0];

            return dt.AsEnumerable().Select(MapField).ToList();
        }


        public List<ContextFieldDefinition> GetFieldsByTemplateReferencepre(int ctrId)
        {
            SqlParameter[] p =
            {
        new ("@context_template_reference_id", ctrId),

    };

            DataTable dt = GetDataSet("GetFieldsByTemplateReferencepre", p).Tables[0];

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

                // image_previewname = r.Table.Columns.Contains("File_path_name")
                //  ? r["File_path_name"]?.ToString()
                // : null, 

                // Thumbnail_Img_Media_Id = r.Table.Columns.Contains("Thumbnail_Img_Media_Id")
                //     && r["Thumbnail_Img_Media_Id"] != DBNull.Value
                //     ? (int?)Convert.ToInt32(r["Thumbnail_Img_Media_Id"])
                //     : null,

                is_required = r.Table.Columns.Contains("is_required")
                    && r["is_required"] != DBNull.Value
                    ? (int?)Convert.ToInt32(r["is_required"])
                    : null,
                // NEW COLUMN
                is_block = r.Table.Columns.Contains("is_block") && r["is_block"] != DBNull.Value
               ? (Convert.ToBoolean(r["is_block"]) ? 1 : 0)
                : 0,
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
                    : null,


                custom_validation = r.Table.Columns.Contains("custom_validation")
                    ? r["custom_validation"]?.ToString()
                    : null
            };
        }

        public void InsertDetail(ContextDetail d, string? temp_cont_id = null)
        {
            List<SqlParameter> p =
            [
                new SqlParameter("@Language_Master_ID", d.Language_Master_ID),
                new SqlParameter("@context_master_id", d.context_master_id),
                new SqlParameter("@context_template_reference_id", d.context_template_reference_id),
                new SqlParameter("@context_field_id", d.context_field_id),
                new SqlParameter("@content", d.content ?? (object)DBNull.Value),
                new SqlParameter("@File_path", d.File_path ?? (object)DBNull.Value),
                new SqlParameter("@Created_UserID", d.Created_UserID ?? 1),
                new SqlParameter("@context_group_id", d.context_group_id),  //  NEW               
            ];
            if (!string.IsNullOrWhiteSpace(temp_cont_id))
            {
                p.Add(new SqlParameter("@cont_temp_id", temp_cont_id));
            }
            if (d.cont_id > 0)
            {
                p.Add(new SqlParameter("@cont_id", d.cont_id));
            }
            SQLInsert_Update_Delete_Data("InsertContextDetail", p.ToArray());
        }

        public void InsertDetailsaved(ContextDetail d, string? temp_cont_id = null)
        {
            List<SqlParameter> p =
            [
                new SqlParameter("@Language_Master_ID", d.Language_Master_ID),
                new SqlParameter("@context_master_id", d.context_master_id),
                new SqlParameter("@context_template_reference_id", d.context_template_reference_id),
                new SqlParameter("@context_field_id", d.context_field_id),
                new SqlParameter("@content", d.content ?? (object)DBNull.Value),
                new SqlParameter("@File_path", d.File_path ?? (object)DBNull.Value),
                new SqlParameter("@Created_UserID", d.Created_UserID ?? 1),
                new SqlParameter("@context_group_id", d.context_group_id),  //  NEW  
                new SqlParameter("@cont_id", d.cont_id),
            ];

            SQLInsert_Update_Delete_Data("InsertContextDetail", p.ToArray());
        }

        public List<ContextDetail> GetDetailsByCTR(int ctrId)
        {
            SqlParameter[] p = {
        new("@ctr_id", ctrId)
    };

            DataTable dt = GetDataSet("GetContextDetailsByCTRId", p).Tables[0];

            List<ContextDetail> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new ContextDetail
                {
                    context_field_id = Convert.ToInt32(r["context_field_id"]),
                    content = r["content"]?.ToString(),
                    File_path = r["File_path"]?.ToString(),
                    context_group_id = Guid.Parse(r["context_group_id"].ToString())
                });
            }

            return list;
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

        public List<ContextFieldWithValue> GetFieldsByGroup_temp(int ctrId, string groupId, int is_block)
        {
            SqlParameter[] p =
            {
                new("@ctrId", ctrId),
                new("@groupId", groupId),
                new("@is_block", is_block)
            };

            DataTable dt = GetDataSet("GetFieldsByGroup_temp", p).Tables[0];
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
                // NEW COLUMN
                is_block = r.Table.Columns.Contains("is_block") && r["is_block"] != DBNull.Value
               ? (Convert.ToBoolean(r["is_block"]) ? 1 : 0)
                : 0,
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

                File_path_name = r.Table.Columns.Contains("File_path_name")
                    ? r["File_path_name"]?.ToString()
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

        public void UpdateDetail_temp(ContextFieldWithValue v)
        {
            SqlParameter[] p =
            {
        new("@detail_id", v.detail_id),
        new("@content", v.content ?? (object)DBNull.Value),
        new("@file_path", v.File_path ?? (object)DBNull.Value),
        new("@Updated_UserID", 1)
    };

            SQLInsert_Update_Delete_Data("UpdateContextDetail_temp", p);
        }

        public void DeleteData_temp_DAL(int CTR_Id, string group_id)
        {
            SQLInsert_Update_Delete_Data("DeleteData_temp", "@context_group_id", group_id, "@CTR_Id", CTR_Id.ToString());
        }


        public DataSet Context_Detail_List_For_Addcontent(int templateId, int language_id, string temp_id)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@template_master_id", templateId),
                new SqlParameter("@language_id", language_id),
                new SqlParameter("@temp_id", temp_id)
            };

            return GetDataSet("Context_Detail_List_For_Add_content", p);
        }

        public DataSet Context_Detail_Temp_List(int templateId, int language_id, int cont_id)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@template_master_id", templateId),
                new SqlParameter("@language_id", language_id),
                new SqlParameter("@cont_id", cont_id)
            };

            return GetDataSet("Context_Detail_Temp_List", p);
        }


        public DataSet Context_Detail_List_For_content_GetAll(int cont_id, int templateId, int language_id, int status = 1)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@template_master_id", templateId),
                new SqlParameter("@language_id", language_id),
                new SqlParameter("@cont_id", cont_id.ToString()),
                new SqlParameter("@status", status)
            };

            return GetDataSet("Context_Detail_List_For_content", p);
        }

        public void Copy_Context_Details_temp(int cont_id)
        {
            SQLInsert_Update_Delete_Data("Copy_context_details_temp", "@cont_id", cont_id.ToString());
        }
        public void Copy_Context_Details_Reprocess(int cont_id)
        {
            SQLInsert_Update_Delete_Data("Copy_context_details_reprocess", "@cont_id", cont_id.ToString());
        }

        public DataSet Context_Detail_Reprocess_List(int templateId, int language_id, int cont_id)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@template_master_id", templateId),
                new SqlParameter("@language_id", language_id),
                new SqlParameter("@cont_id", cont_id)
            };
            return GetDataSet("Context_Detail_Reprocess_List", p);

        }


        public void InsertDetail_Reprocessed(ContextDetail d, string? temp_cont_id = null)
        {
            List<SqlParameter> p =
            [
                new SqlParameter("@Language_Master_ID", d.Language_Master_ID),
                new SqlParameter("@context_master_id", d.context_master_id),
                new SqlParameter("@context_template_reference_id", d.context_template_reference_id),
                new SqlParameter("@context_field_id", d.context_field_id),
                new SqlParameter("@content", d.content ?? (object)DBNull.Value),
                new SqlParameter("@File_path", d.File_path ?? (object)DBNull.Value),
                new SqlParameter("@Created_UserID", d.Created_UserID ?? 1),
                new SqlParameter("@context_group_id", d.context_group_id),  //  NEW  
                new SqlParameter("@cont_id", d.cont_id),
            ];

            SQLInsert_Update_Delete_Data("InsertContextDetail_Reprocess", p.ToArray());
        }


        public List<ContextFieldWithValue> GetFieldsByGroup_Reprocess(int ctrId, string groupId, int is_block)
        {
            SqlParameter[] p =
            {
                new("@ctrId", ctrId),
                new("@groupId", groupId),
                new("@is_block", is_block)
            };

            DataTable dt = GetDataSet("GetFieldsByGroup_Reprocess", p).Tables[0];
            List<ContextFieldWithValue> list = new();
            return dt.AsEnumerable().Select(MapFieldWithValue).ToList();


        }

        public void DeleteData_Reprocess_DAL(int CTR_Id, string group_id, int userid)
        {
            SQLInsert_Update_Delete_Data("DeleteData_Reprocess", "@context_group_id", group_id, "@CTR_Id", CTR_Id.ToString(),"@userid", userid.ToString());
        }
     
        public void UpdateDetail_Reprocess(ContextFieldWithValue v, int userid)
        {
            SqlParameter[] p =
            {
        new("@detail_id", v.detail_id),
        new("@content", v.content ?? (object)DBNull.Value),
        new("@file_path", v.File_path ?? (object)DBNull.Value),
        new("@Updated_UserID", userid)
    };
  //updated for reprocess spname
            SQLInsert_Update_Delete_Data("UpdateContextDetail_reprocess", p);
        }

    }
}
