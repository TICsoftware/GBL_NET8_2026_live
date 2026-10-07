using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class CareersModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();

        
        public List<ComponentModel> Learning_Development_List { get; set; } = new();
        
        public List<ComponentModel> Why_Join_Godavari_Biorefineries_List { get; set; } = new();
        public List<ComponentModel> Life_at_Godavari_List { get; set; } = new();
        public List<ComponentModel> Career_CTA_List { get; set; } = new();

        

    }
}
