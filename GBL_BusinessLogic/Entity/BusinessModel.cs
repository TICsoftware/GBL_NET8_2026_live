using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class BusinessModel
    {
        public ContentViewModel? Content { get; set; }
       // public List<ComponentGroup> Components { get; set; } = new();
        //Media
        public List<ArticleModel> Section_List { get; set; } = new(); 

         public List<ComponentModel> Subsections_Components { get; set; } = new();
  
    }


}
