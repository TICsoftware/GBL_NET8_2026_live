using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class CaseStudiesViewModel
    {
        public List<ComponentModel> Case_Studies_Component_List { get; set; } = new();
        public List<ArticleModel> Case_Studies_List { get; set; } = new();
    }
}