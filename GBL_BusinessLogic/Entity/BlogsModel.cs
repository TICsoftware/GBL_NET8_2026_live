using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class BlogsModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();

        // Seq 1: section heading + View More CTA (IsBlock == 0)
        //        optional component cards (IsBlock == 1) when not using child articles
        public List<ComponentModel> Latest_Trends_List { get; set; } = new();

        // Child blog posts from Tables[2]
        public List<ArticleModel> BlogPosts_List { get; set; } = new();

        // Related articles from Tables[2] (blog inside page)
        public List<ArticleModel> RelatedArticles_List { get; set; } = new();

        // Seq 1 header for related section on inside page
        public List<ComponentModel> Related_Section_List { get; set; } = new();

        public int TotalCount { get; set; }
    }
}
