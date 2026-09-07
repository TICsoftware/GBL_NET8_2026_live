using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class PageModel
    {
        public ContentViewModel Content { get; set; }

        public List<ComponentGroup> Components { get; set; } = new();

        //Key = Sequence
        public Dictionary<int, List<ComponentModel>> Sections { get; set; } = new();

        public int TotalCount { get; set; }
    }
}