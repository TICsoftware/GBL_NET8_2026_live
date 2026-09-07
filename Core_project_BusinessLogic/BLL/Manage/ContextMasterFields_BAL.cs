using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
public class ContextMasterFields_BAL
{
    private readonly ContextMasterFields_DAL dal;

    public ContextMasterFields_BAL(IConfiguration config)
    {
        dal = new ContextMasterFields_DAL(config);
    }

   public ContextMasterFieldAssign Load(int contextMasterId)
{
    var selected = dal.GetSelectedFields(contextMasterId) ?? new List<int>();
    var all = dal.GetAllFields() ?? new List<ContextField>();

    return new ContextMasterFieldAssign
    {
        context_master_id = contextMasterId,
        SelectedFieldIds = selected,
        AllFields = all
    };
}

    public void Save(int contextMasterId, List<int> fieldIds, int userId)
    {
        string csv = string.Join(",", fieldIds);
        dal.Save(contextMasterId, csv, userId);
    }
}

}