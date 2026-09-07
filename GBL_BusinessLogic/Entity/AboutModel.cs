using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class AboutModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();
        public List<ComponentGroup> Components2 { get; set; } = new();



        //About us
        public List<ComponentModel> Four_Decades_Of_Building_List { get; set; } = new();
        public List<ComponentModel> Infographic_List { get; set; } = new();
        public List<ComponentModel> What_We_Stand_For_List { get; set; } = new();
        public List<ComponentModel> A_Word_From_Our_Leadership_List { get; set; } = new();
        public List<ComponentModel> Values_That_Define_How_We_Work_List { get; set; } = new();
        public List<ComponentModel> Built_On_Trust_Proven_At_Scale_List { get; set; } = new();
        public List<ComponentModel> Legacy_Built_One_Year_At_A_Time_List { get; set; } = new();
        public List<ComponentModel> Ready_To_Build_With_Priya_Cement_List { get; set; } = new();

        //Financial Information
        public List<ArticleModel> Section_List { get; set; } = new();
        public List<ArticleModel> SectionArticles_List { get; set; } = new();


        public int TotalCount { get; set; }

    }


}