using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class InvestorsModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ArticleModel> SectionArticles_List { get; set; } = new();
        public int TotalCount { get; set; }
    }
}
