using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class CorporateGovernanceModel
    {
        public ContentViewModel? Content { get; set; }
        public List<GovernanceSectionModel> Sections { get; set; } = new();
    }

    public class GovernanceSectionModel
    {
        public int ContId { get; set; }
        public string Title { get; set; } = "";
        public string Anchor { get; set; } = "";
        public string PageName { get; set; } = "";
        public bool ShowYearFilter { get; set; }
        public string SelectedYear { get; set; } = "";
        public List<DropdownModel> Years { get; set; } = new();
        public List<ArticleModel> Articles { get; set; } = new();
        public List<CommitteeModel> Committees { get; set; } = new();
        public bool IsCommitteeSection => Committees.Count > 0 && Articles.Count == 0;
    }

    public class CommitteeModel
    {
        public string Title { get; set; } = "";
        public int Sequence { get; set; }
        public List<CommitteeMemberModel> Members { get; set; } = new();
    }

    public class CommitteeMemberModel
    {
        public string Name { get; set; } = "";
        public string Role { get; set; } = "";
        public int Sequence { get; set; }
    }
}
