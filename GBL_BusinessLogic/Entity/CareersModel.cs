using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class CareersModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();

        /// <summary>Seq 1 — page intro</summary>
        public List<ComponentModel> Intro_List { get; set; } = new();

        /// <summary>Seq 2 — Life Inside (header + petal icons/labels)</summary>
        public List<ComponentModel> LifeInside_List { get; set; } = new();

        /// <summary>Seq 3 — Workplace culture (intro + photos)</summary>
        public List<ComponentModel> WorkplaceCulture_List { get; set; } = new();

        /// <summary>Seq 4 — Testimonials</summary>
        public List<ComponentModel> Testimonials_List { get; set; } = new();

        /// <summary>Seq 5 — Join CTA</summary>
        public List<ComponentModel> Careers_CTA_List { get; set; } = new();
    }
}
