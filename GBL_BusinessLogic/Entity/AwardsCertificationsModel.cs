using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class AwardsCertificationsModel
    {
        public ContentViewModel? Content { get; set; }
        public List<AwardsSectionModel> Sections { get; set; } = new();
    }

    public class AwardsSectionModel
    {
        public int ContId { get; set; }
        public string Title { get; set; } = "";
        public string Intro { get; set; } = "";
        public string PageName { get; set; } = "";
        public int Sequence { get; set; }
        public bool IsCertifications { get; set; }
        public List<AwardsGroupModel> Groups { get; set; } = new();
    }

    public class AwardsGroupModel
    {
        public int ContId { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; } = "";
        public string Intro { get; set; } = "";
        public string PageName { get; set; } = "";
        public int Sequence { get; set; }
        public List<ComponentModel> Items { get; set; } = new();
    }
}
