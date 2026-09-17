using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class MediaModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();
        public List<ComponentGroup> Components2 { get; set; } = new();

        //Media
        public List<ArticleModel> Section_List { get; set; } = new();
        public List<ArticleModel> SectionArticles_List { get; set; } = new();
        public List<ArticleModel> Related_Articles_List { get; set; } = new();


        public int TotalCount { get; set; }

    }
}
