using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class ResourceCircularityModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();

        /// <summary>component_sequence 1 — Infoghraphic section RC</summary>
        public List<ComponentModel> Infographic_List { get; set; } = new();

        /// <summary>component_sequence 2 — Circular energy model RC</summary>
        public List<ComponentModel> Circular_Energy_Model_List { get; set; } = new();

        /// <summary>component_sequence 3 — Energy consumption world chart</summary>
        public List<ComponentModel> Energy_Chart_World_List { get; set; } = new();

        /// <summary>component_sequence 4 — Energy consumption India chart</summary>
        public List<ComponentModel> Energy_Chart_India_List { get; set; } = new();

        /// <summary>component_sequence 5 — Energy consumption GBL chart</summary>
        public List<ComponentModel> Energy_Chart_Gbl_List { get; set; } = new();

        /// <summary>component_sequence 6 — Title intro RC (fossil fuels)</summary>
        public List<ComponentModel> Fossil_Fuels_List { get; set; } = new();

        /// <summary>component_sequence 7 — Low-emission operations</summary>
        public List<ComponentModel> Low_Emission_Operations_List { get; set; } = new();

        /// <summary>component_sequence 8 — Conserving water saving lives</summary>
        public List<ComponentModel> Conserving_Water_List { get; set; } = new();
    }
}
