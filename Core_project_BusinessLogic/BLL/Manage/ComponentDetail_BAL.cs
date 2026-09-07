using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace Core_project_BusinessLogic.BAL
{
    public class ComponentDetail_BAL
    {
        private readonly ComponentDetail_DAL dal;
        private readonly IConfiguration _config;

        public ComponentDetail_BAL(IConfiguration config)
        {
            _config = config;
            dal = new ComponentDetail_DAL(config);
        }

        public ComponentModel GetComponent_bal(int referenceId)
        {
            return dal.GetComponent(referenceId);
        }




        public void Save(ContextDetail d)
        {
            dal.InsertDetail(d);
        }
        public List<ContextFieldWithValue> LoadFieldsForEditByGroup(int ctrId, string gid)
        {
            if (string.IsNullOrEmpty(gid))
                return new List<ContextFieldWithValue>();

            return dal.GetFieldsByGroup(ctrId, gid);
        }
        public List<ContextFieldWithValue> LoadFieldsForEdit(int referenceId)
        {
            return dal.GetFieldsForEdit(referenceId);
        }

        public void UpdateField(ContextFieldWithValue v)
        {
            dal.UpdateDetail(v);
        }
        public List<ContextFieldDefinition> LoadFieldsByTemplateReference(int ctrId)
        {
            return dal.GetFieldsByTemplateReference(ctrId);
        }

        public List<ContextFieldWithValue> LoadFieldsForEditByTemplateReference(int ctrId)
        {
            return dal.GetFieldsWithValuesByTemplateReference(ctrId);
        }

    }

}