
using System.Collections.Generic;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Core_project_BusinessLogic.BAL
{

    public class ContextTemplateReference_BAL
    {
        private readonly ContextTemplateReference_DAL _dal;

        public ContextTemplateReference_BAL(IConfiguration config)
        {
            _dal = new ContextTemplateReference_DAL(config);
        }

        public List<ContextFieldDefinition> LoadFields(int ctrId)
            => _dal.GetFields(ctrId);

        public void Save(ContextDetail d)
            => _dal.InsertDetail(d);

        public List<ContextFieldDefinition> LoadFieldsForEdit(int ctrId)
            => _dal.GetFieldsForEdit(ctrId);

        public ContextTemplateReference GetById(int id)
        {
            return _dal.GetById(id);
        }
        public List<ContextDetail> GetDetailsByCTR(int ctrId)
        {
            return _dal.GetDetailsByCTR(ctrId);
        }

         public List<ContextDetail> GetDetailsByCTRpre(int ctrId)
        {
            return _dal.GetDetailsByCTRpre(ctrId);
        }
        public void SaveMappings(int contextId, List<int> templateIds)
        {
            // delete old mappings
            // _dal.DeleteByContext(contextId);

            // insert new
            foreach (var tid in templateIds)
            {
                _dal.Insert(contextId, tid, 1);
            }
        }
        public void UpdateLabel(int id, string label)
        {
            _dal.UpdateLabel(id, label);
        }
        public List<int> GetTemplates(int contextId)
        {
            return _dal.GetTemplatesByContext(contextId);
        }

        public List<ContextTemplateReference> GetAll(
            int? templateId,
            int? contextId,
            string label)
        {
            return _dal.GetAll(templateId, contextId, label);
        }

        public void Add(ContextTemplateReference m)
            => _dal.Insert(m);

        public void UpdateOrder(List<ComponentOrderModel> items)
        {
            _dal.UpdateOrder(items);
        }
       
        public void Delete(int id, int? contextMasterId = null)
        {
            _dal.Delete(id, contextMasterId);
        }

    }
}
