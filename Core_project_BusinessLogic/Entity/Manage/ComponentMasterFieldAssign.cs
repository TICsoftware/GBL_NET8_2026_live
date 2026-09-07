using System;

namespace Core_project_BusinessLogic.Entity
{
    public class ComponentMasterFieldAssign
{
    public int component_master_id { get; set; }
    public List<int> SelectedFieldIds { get; set; } = new();
    public List<ComponentMasterField> AllFields { get; set; } = new();
}
}
