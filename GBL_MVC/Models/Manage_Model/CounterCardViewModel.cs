using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Models.Manage_Model
{
    public class CounterCardViewModel
    {
        public int Id { get; set; }   // For DB (Edit case)

        public string Title { get; set; }

        public int Value { get; set; }

        public string Postfix { get; set; }

        public int DisplayOrder { get; set; }  // Important for sorting
    }

    public class OurStoryViewModel
    {
        public string MainTitle { get; set; }
        public string Description { get; set; }

        public List<CounterCardViewModel> CounterCards { get; set; } = new();
    }


}