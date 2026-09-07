
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;


namespace Core_project_BusinessLogic.DAL
{
    public class ContextTemplateReference_DAL : DBHelper
    {
        public ContextTemplateReference_DAL(IConfiguration config) : base(config) { }
        public void UpdateLabel(int id, string label)
        {
            SqlParameter[] p =
            {
        new SqlParameter("@ID", id),
        new SqlParameter("@component_label", label)
    };

            SQLInsert_Update_Delete_Data(
                "sp_ContextTemplateReference_UpdateLabel",
                p);
        }
        public void Insert(int contextId, int templateId, int status = 1)
        {
            SqlParameter[] p =
            {
            new("@context_master_id", contextId),
            new("@template_master_id", templateId),
            new("@Status", status)
        };

            SQLInsert_Update_Delete_Data("sp_ContextTemplateRef_Insert", p);
        }

        public List<ContextDetail> GetDetailsByCTR(int ctrId)
        {
            SqlParameter[] p =
            {
               new SqlParameter("@ctr_id", ctrId)
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
                    File_path_name = r["File_path_name"]?.ToString(),
                    context_group_id = Guid.Parse(r["context_group_id"].ToString())
                });
            }

            return list;
        }


        public List<ContextDetail> GetDetailsByCTRpre(int ctrId)
        {
            SqlParameter[] p =
            {
               new SqlParameter("@ctr_id", ctrId)
            };

            DataTable dt = GetDataSet("GetContextDetailsByCTRIdpre", p).Tables[0];

            List<ContextDetail> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new ContextDetail
                {
                    context_field_id = Convert.ToInt32(r["context_field_id"]),
                    content = r["content"]?.ToString(),
                    File_path = r["File_path"]?.ToString(),
                    File_path_name = r["File_path_name"]?.ToString(),
                    context_group_id = Guid.Parse(r["context_group_id"].ToString())
                });
            }

            return list;
        }
        public List<ContextFieldDefinition> GetFields(int ctrId)
        {
            SqlParameter[] p = { new("@ctr_id", ctrId) };
            DataTable dt = GetDataSet("GetFieldsForCTR", p).Tables[0];

            var list = new List<ContextFieldDefinition>();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new ContextFieldDefinition
                {
                    id = Convert.ToInt32(r["id"]),
                    name = r["name"].ToString(),
                    name_key = r["name_key"].ToString(),
                    field_type_id = Convert.ToInt32(r["field_type_id"]),
                    context_master_id = Convert.ToInt32(r["context_master_id"])
                });
            }

            return list;
        }
        public ContextTemplateReference GetById(int id)
        {
            SqlParameter[] p =
            {
        new SqlParameter("@ID", id)
    };

            DataTable dt = GetDataSet("GetContextTemplateReferenceById", p).Tables[0];

            if (dt.Rows.Count == 0)
                return null;

            return Map(dt.Rows[0]);
        }


        private ContextTemplateReference Map(DataRow r)
        {
            return new ContextTemplateReference
            {
                ID = Convert.ToInt32(r["ID"]),
                Status = r["Status"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["Status"]),
                Context_Master_ID = (int)Convert.ToInt32(r["context_master_id"]),
                Template_Master_ID = r["template_master_id"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["template_master_id"]),
                Sequence = r.Table.Columns.Contains("sequence") && r["sequence"] != DBNull.Value
                    ? (int?)Convert.ToInt32(r["sequence"])
                    : null,

                Context_Title = r.Table.Columns.Contains("Context_Title")
                    ? r["Context_Title"]?.ToString()
                    : "",

                Template_Name = r.Table.Columns.Contains("Template_Name")
                    ? r["Template_Name"]?.ToString()
                    : ""
            };
        }
        public List<ContextTemplateReference> GetAll(
            int? templateId,
            int? contextId,
            string label)
        {
            SqlParameter[] p =
            {
                new("@template_master_id", (object)templateId ?? DBNull.Value),
                new("@context_master_id", (object)contextId ?? DBNull.Value),
                new("@component_label", (object)label ?? DBNull.Value)
            };

            DataTable dt =
                GetDataSet("sp_ContextTemplateReference_List", p).Tables[0];

            return dt.AsEnumerable().Select(MapAll).ToList();
        }



        public void Insert(ContextTemplateReference m)
        {
            SqlParameter[] p =
            {
        new SqlParameter("@context_master_id", m.Context_Master_ID),
        new SqlParameter("@template_master_id", m.Template_Master_ID),
        new SqlParameter("@sequence", m.Sequence),
        new SqlParameter("@component_label", (object?)m.Component_Label ?? DBNull.Value),
        new SqlParameter("@Status", m.Status ?? 1)
    };

            SQLInsert_Update_Delete_Data("sp_ContextTemplateReference_Insert", p);
        }
        private ContextTemplateReference MapAll(DataRow r)
        {
            return new ContextTemplateReference
            {
                ID = Convert.ToInt32(r["ID"]),
                Context_Master_ID =
                    Convert.ToInt32(r["context_master_id"]),
                Template_Master_ID =
                    r["template_master_id"] as int?,
                Sequence =
                    r["sequence"] as int?,
                Status =
                    r["Status"] as int?,

                Context_Title =
                    r["Context_Title"]?.ToString(),

                Template_Name =
                    r["Template_Name"]?.ToString(),
                Component_Label =
                    r["component_label"]?.ToString(),

                HasData =
                    Convert.ToInt32(r["HasData"]) == 1
            };
        }

        public void UpdateOrder(List<ComponentOrderModel> items)
        {
            foreach (var item in items)
            {
                SqlParameter[] p =
                {
            new("@ID", item.id),
            new("@sequence", item.sequence)
        };

                SQLInsert_Update_Delete_Data(
                    "sp_ContextTemplateReference_UpdateSequence",
                    p);
            }
        }
        public void InsertDetail(ContextDetail d)
        {


            SqlParameter[] p =
            {
            new("@context_template_reference_id", d.context_template_reference_id),
            new("@context_master_id", d.context_master_id),
            new("@context_field_id", d.context_field_id),
            new("@Language_Master_ID", d.Language_Master_ID),
            new("@content", d.content ?? (object)DBNull.Value),
            new("@File_path", d.File_path ?? (object)DBNull.Value),
            new("@Created_UserID", d.Created_UserID)
        };

            SQLInsert_Update_Delete_Data("InsertContextDetail", p);
        }

        public List<ContextFieldDefinition> GetFieldsForEdit(int ctrId)
        {
            SqlParameter[] p = { new("@ctr_id", ctrId) };
            DataTable dt = GetDataSet("GetContextDetailsByCTR", p).Tables[0];

            var list = new List<ContextFieldDefinition>();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new ContextFieldDefinition
                {
                    field_id = Convert.ToInt32(r["field_id"]),
                    name = r["name"].ToString(),
                    name_key = r["name_key"].ToString(),
                    field_type_id = Convert.ToInt32(r["field_type_id"]),
                    content = r["content"]?.ToString(),
                    File_path = r["File_path"]?.ToString(),
                    detail_id = r["detail_id"] == DBNull.Value ? null : (int?)Convert.ToInt32(r["detail_id"]),
                    context_master_id = Convert.ToInt32(r["context_master_id"])
                });
            }

            return list;
        }


        public void DeleteByContext(int contextId)
        {
            SqlParameter[] p =
            {
            new("@context_master_id", contextId)
        };

            SQLInsert_Update_Delete_Data("sp_ContextTemplateRef_DeleteByContext", p);
        }

        public void Delete(int id, int? contextMasterId = null)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", id),
                new SqlParameter("@context_master_id", (object?)contextMasterId ?? DBNull.Value)
            };

            SQLInsert_Update_Delete_Data("sp_ContextTemplateReference_Delete", p);
        }


        public List<int> GetTemplatesByContext(int contextId)
        {
            SqlParameter[] p = { new("@context_master_id", contextId) };
            DataTable dt = GetDataSet("sp_ContextTemplateRef_ByContext", p).Tables[0];

            List<int> ids = new();
            foreach (DataRow r in dt.Rows)
                ids.Add(Convert.ToInt32(r["template_master_id"]));

            return ids;
        }



    }

}