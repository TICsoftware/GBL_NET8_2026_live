using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
public class maincomponentmasterFields_BAL
{
    private readonly MaincomponentmasterFields_DAL dal;

    public maincomponentmasterFields_BAL(IConfiguration config)
    {
        dal = new MaincomponentmasterFields_DAL(config);
    }

   public ComponentMasterFieldAssign Load(int componentMasterId)
{
    var selected = dal.GetSelectedFields(componentMasterId) ?? new List<int>();
    var all = dal.GetAllFields() ?? new List<ComponentMasterField>();

    return new ComponentMasterFieldAssign
    {
        component_master_id = componentMasterId,
        SelectedFieldIds = selected,
        AllFields = all
    };
}

    public void Save(int componentMasterId, List<int> fieldIds, int userId)
    {
        string csv = string.Join(",", fieldIds);
        dal.Save(componentMasterId, csv, userId);
    }
}

}