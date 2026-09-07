using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class ContactusModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();

        /// <summary>Seq 1 — page intro (Our Footprint)</summary>
        public List<ComponentModel> Intro_List { get; set; } = new();

        /// <summary>Seq 2 — Toll free customer service (header + language cards)</summary>
        public List<ComponentModel> TollFree_List { get; set; } = new();

        /// <summary>Seq 3 — Enquiry CTA + form heading</summary>
        public List<ComponentModel> Enquiry_CTA_List { get; set; } = new();
    }
}
