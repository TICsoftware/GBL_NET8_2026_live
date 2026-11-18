using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class CSRModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();        
        public List<ComponentModel>  ListComponents { get; set; } = new();
        public Dictionary<int, List<ComponentModel>> Component_Dictionary { get; set; } = new();
    }
}