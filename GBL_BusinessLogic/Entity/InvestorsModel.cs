using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class InvestorsModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ArticleModel> SectionArticles_List { get; set; } = new();
        public List<DropdownModel> Year_List { get; set; } = new();
        public string SelectedYear { get; set; } = "";
        public int TotalCount { get; set; }
    }
}
