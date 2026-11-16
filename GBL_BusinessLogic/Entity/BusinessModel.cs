using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class BusinessModel
    {
        public ContentViewModel? Content { get; set; }

        public List<ArticleModel> Section_List { get; set; } = new();

        /// <summary>Landing / subsection image blocks (typically sequence 1).</summary>
        public List<ComponentModel> Subsections_Components { get; set; } = new();

        /// <summary>
        /// Business inside: one list per CMS component_sequence (1, 2, 3…).
        /// Any number of components binds automatically.
        /// </summary>
        public Dictionary<int, List<ComponentModel>> Component_Lists { get; set; } = new();
    }
}
