using System;

namespace Core_project_BusinessLogic.Entity
{
    public class ContextMasterFieldAssign
{
    public int context_master_id { get; set; }
    public List<int> SelectedFieldIds { get; set; } = new();
    public List<ContextField> AllFields { get; set; } = new();
}
}
