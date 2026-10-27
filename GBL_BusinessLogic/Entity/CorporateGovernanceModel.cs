using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class CorporateGovernanceModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ArticleModel> Policies_List { get; set; } = new();
        public List<ArticleModel> CodeOfConduct_List { get; set; } = new();
        public List<CommitteeModel> Committees_List { get; set; } = new();
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
