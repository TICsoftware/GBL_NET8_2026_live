using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class ESGModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();
        public List<ComponentGroup> Components2 { get; set; } = new();



        //Sustainability
        public List<ComponentModel> Strong_Cement_List { get; set; } = new();
        public List<ComponentModel> PC_Manufactures_Responsibly_List { get; set; } = new();
        public List<ComponentModel> Independently_Verified_List { get; set; } = new();
        public List<ComponentModel> Read_The_Numbers_List { get; set; } = new();
       


        public int TotalCount { get; set; }

    }


}