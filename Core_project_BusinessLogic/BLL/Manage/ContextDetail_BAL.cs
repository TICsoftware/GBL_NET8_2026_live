using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace Core_project_BusinessLogic.BAL
{
    public class ContextDetail_BAL
    {
        private readonly ContextDetail_DAL dal;
        private readonly ContextTemplateReference_DAL _dal;
        private readonly IConfiguration _config;

        public ContextDetail_BAL(IConfiguration config)
        {
            _config = config;
            dal = new ContextDetail_DAL(config);
        }

        public List<ContextFieldDefinition> LoadFields(int referenceId)
        {
            return dal.GetFieldsForReference(referenceId);
        }
        public void Save_temp(ContextDetail d, string? temp_cont_id = null)
        {
            if (string.IsNullOrWhiteSpace(temp_cont_id))
            {
                dal.InsertDetail(d);
            }
            else
            {
                dal.InsertDetail(d, temp_cont_id);
            }
        }
        public void Save(ContextDetail d)
        {

            dal.InsertDetailsaved(d);


        }
        public List<ContextFieldWithValue> LoadFieldsForEditByGroup(int ctrId, string gid)
        {
            if (string.IsNullOrEmpty(gid))
                return new List<ContextFieldWithValue>();

            return dal.GetFieldsByGroup(ctrId, gid);
        }

        public List<ContextFieldWithValue> LoadFieldsForEditByGroup_temp(int ctrId, string gid, int is_block)
        {
            if (string.IsNullOrEmpty(gid))
                return new List<ContextFieldWithValue>();

            return dal.GetFieldsByGroup_temp(ctrId, gid, is_block);
        }

        public List<ContextFieldWithValue> LoadFieldsForEdit(int referenceId)
        {
            return dal.GetFieldsForEdit(referenceId);
        }

        public void UpdateField(ContextFieldWithValue v)
        {
            dal.UpdateDetail(v);
        }

        public void UpdateField_temp(ContextFieldWithValue v)
        {
            dal.UpdateDetail_temp(v);
        }

        public List<ContextFieldDefinition> LoadFieldsByTemplateReference(int ctrId, int is_block)
        {
            return dal.GetFieldsByTemplateReference(ctrId, is_block);
        }
        public List<ContextFieldDefinition> LoadFieldsByTemplateReferencepre(int ctrId)
        {
            return dal.GetFieldsByTemplateReferencepre(ctrId);
        }

        public List<ContextFieldWithValue> LoadFieldsForEditByTemplateReference(int ctrId, int is_block)
        {
            return dal.GetFieldsWithValuesByTemplateReference(ctrId, is_block);
        }

        public void DeleteField_temp(int CTR_Id, string group_id)
        {
            dal.DeleteData_temp_DAL(CTR_Id, group_id);
        }


        public Context_Component_Details Context_Detail_List_For_content_GetAll_BAL(int cont_id, int templateId, int language_id, int status = 1)
        {
            Context_Component_Details obj = new();
            DataSet ds = new();
            try
            {
                ds = dal.Context_Detail_List_For_content_GetAll(templateId, language_id, status);
                obj._context = [];

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        obj._context.Add(new ContextTemplateReference
                        {
                            ID = Convert.ToInt32(item["CTR_Id"].ToString()),
                            Sequence = Convert.ToInt32(item["sequence"]),
                            Status = Convert.ToInt32(item["Status"]),
                            Context_Title = item["Context_Title"].ToString(),
                            Component_Label = item["component_label"].ToString(),
                            field_id = Convert.ToInt32(item["field_Id"] == DBNull.Value ? 0 : item["field_Id"]),
                            is_block = Convert.ToInt32(item["block_fields_count"]),
                            Component_field_count=Convert.ToInt32(item["component_fields_count"]),
                        });
                    }
                }
                else
                {
                    obj._context = null;
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    obj._context_Details = [];
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds.Tables[1].Rows)
                        {
                            obj._context_Details.Add(new Tuple<int, int, string, string, int>(
                                Convert.ToInt32(item["ID"]), Convert.ToInt32(item["CTR_Id"]), item["context_group_id"].ToString() ?? "", item["Title"].ToString() ?? "", Convert.ToInt32(item["cd_sequence"])));
                        }
                    }
                }
                else
                {
                    obj._context_Details = null;
                }

                return obj;
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        public Context_Component_Details Context_Detail_List_For_Addcontent_BAL(int templateId, int language_id, string temp_id)
        {
            Context_Component_Details obj = new();
            DataSet ds = new();
            try
            {
                ds = dal.Context_Detail_List_For_Addcontent(templateId, language_id, temp_id);
                obj._context = [];

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        obj._context.Add(new ContextTemplateReference
                        {
                            ID = Convert.ToInt32(item["CTR_Id"].ToString()),
                            Sequence = Convert.ToInt32(item["sequence"]),
                            Status = Convert.ToInt32(item["Status"]),
                            Context_Title = item["Context_Title"].ToString(),
                            Component_Label = item["component_label"].ToString(),
                            group_id = item["context_group_id"].ToString(),
                            is_block = Convert.ToInt32(item["block_fields_count"]),
                            field_id = Convert.ToInt32(item["field_Id"] == DBNull.Value ? 0 : item["field_Id"]),
                            Component_field_count = Convert.ToInt32(item["component_fields_count"]),
                        });
                    }
                }
                else
                {
                    obj._context = null;
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    obj._context_Details = [];
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds.Tables[1].Rows)
                        {
                            obj._context_Details.Add(
                                new Tuple<int, int, string, string, int>(
                                    Convert.ToInt32(item["ID"]),
                                    Convert.ToInt32(item["CTR_Id"]),
                                    item["context_group_id"]?.ToString() ?? "",
                                    item["Title"]?.ToString() ?? "",
                                    item["cd_sequence"] != DBNull.Value ? Convert.ToInt32(item["cd_sequence"]) : 0
                                )
                            );
                        }
                    }
                }
                else
                {
                    obj._context_Details = null;
                }

                return obj;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public Context_Component_Details Context_Detail_Temp_List_BAL(int templateId, int language_id, int cont_id)
        {
            Context_Component_Details obj = new();
            DataSet ds = new();
            try
            {
                ds = dal.Context_Detail_Temp_List(templateId, language_id, cont_id);
                obj._context = [];

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        obj._context.Add(new ContextTemplateReference
                        {
                            ID = Convert.ToInt32(item["CTR_Id"].ToString()),
                            Sequence = Convert.ToInt32(item["sequence"]),
                            Status = Convert.ToInt32(item["Status"]),
                            Context_Title = item["Context_Title"].ToString(),
                            Component_Label = item["component_label"].ToString(),
                            group_id = item["context_group_id"].ToString(),
                            is_block = Convert.ToInt32(item["block_fields_count"]),
                            field_id = Convert.ToInt32(item["field_Id"] == DBNull.Value ? 0 : item["field_Id"]),
                            Component_field_count=Convert.ToInt32(item["component_fields_count"]),
                        });
                    }
                }
                else
                {
                    obj._context = null;
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    obj._context_Details = [];
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds.Tables[1].Rows)
                        {
                            obj._context_Details.Add(new Tuple<int, int, string, string, int>(
                                Convert.ToInt32(item["ID"]), Convert.ToInt32(item["CTR_Id"]), item["context_group_id"].ToString() ?? "", item["Title"].ToString() ?? "", Convert.ToInt32(item["cd_sequence"])));
                        }
                    }
                }
                else
                {
                    obj._context_Details = null;
                }

                return obj;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        //delete existing data from context_details_temp and copy from main table
        public void Copy_Context_Details_temp_BAL(int cont_id)
        {
            dal.Copy_Context_Details_temp(cont_id);
        }

        public void Copy_Context_Details_Reprocess_BAL(int cont_id)
        {
            dal.Copy_Context_Details_Reprocess(cont_id);
        }

        public Context_Component_Details Context_Detail_Reprocess_List_BAL(int templateId, int language_id, int cont_id)
        {
            Context_Component_Details obj = new();
            DataSet ds = new();
            try
            {
                ds = dal.Context_Detail_Reprocess_List(templateId, language_id, cont_id);
                obj._context = [];

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        obj._context.Add(new ContextTemplateReference
                        {
                            ID = Convert.ToInt32(item["CTR_Id"].ToString()),
                            Sequence = Convert.ToInt32(item["sequence"]),
                            Status = Convert.ToInt32(item["Status"]),
                            Context_Title = item["Context_Title"].ToString(),
                            Component_Label = item["component_label"].ToString(),
                            group_id = item["context_group_id"].ToString(),
                            is_block = Convert.ToInt32(item["block_fields_count"]),
                            field_id = Convert.ToInt32(item["field_Id"] == DBNull.Value ? 0 : item["field_Id"]),
                            Component_field_count = Convert.ToInt32(item["component_fields_count"]),
                        });
                    }
                }
                else
                {
                    obj._context = null;
                }

                if (ds.Tables[1].Rows.Count > 0)
                {
                    obj._context_Details = [];
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        foreach (DataRow item in ds.Tables[1].Rows)
                        {
                            obj._context_Details.Add(new Tuple<int, int, string, string, int>(
                                Convert.ToInt32(item["ID"]), Convert.ToInt32(item["CTR_Id"]), item["context_group_id"].ToString() ?? "", item["Title"].ToString() ?? "", Convert.ToInt32(item["cd_sequence"])));
                        }
                    }
                }
                else
                {
                    obj._context_Details = null;
                }

                return obj;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void Save_Reprocess(ContextDetail d, string? temp_cont_id = null)
        {
            if (string.IsNullOrWhiteSpace(temp_cont_id))
            {
                dal.InsertDetail_Reprocessed(d);
            }
            else
            {
                dal.InsertDetail_Reprocessed(d, temp_cont_id);
            }

        }
        public List<ContextFieldWithValue> LoadFieldsForEditByGroup_Reprocess(int ctrId, string gid, int is_block)
        {
            if (string.IsNullOrEmpty(gid))
                return new List<ContextFieldWithValue>();

            return dal.GetFieldsByGroup_Reprocess(ctrId, gid, is_block);
        }

        public void DeleteField_Reprocess(int CTR_Id, string group_id, int userid)
        {
            dal.DeleteData_Reprocess_DAL(CTR_Id, group_id, userid);
        }

        public void UpdateDetail_Reprocess_DAL(ContextFieldWithValue v, int userid)
        {
            dal.UpdateDetail_Reprocess(v, userid);
        }
    }

}