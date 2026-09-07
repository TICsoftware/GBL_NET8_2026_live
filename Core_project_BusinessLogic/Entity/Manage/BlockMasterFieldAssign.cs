using System;

namespace Core_project_BusinessLogic.Entity
{
    public class BlockMasterFieldAssign
{
    public int block_master_id { get; set; }
    public List<int> SelectedFieldIds { get; set; } = new();
    public List<BlockMasterField> AllFields { get; set; } = new();
}
}
