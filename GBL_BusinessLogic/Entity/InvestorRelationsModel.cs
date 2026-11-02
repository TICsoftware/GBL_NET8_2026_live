using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class InvestorRelationsModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ArticleModel> WhatsNewPress { get; set; } = new();
        public List<ArticleModel> WhatsNewMedia { get; set; } = new();
        public QuarterlyReportItemModel? LatestQuarter { get; set; }
        public ArticleModel? LatestAnnualReport { get; set; }
        public List<InvestorLinkModel> ShareholderLinks { get; set; } = new();
        public List<InvestorLinkModel> GovernanceLinks { get; set; } = new();
        public string GovernanceBackground { get; set; } = "/Assets/images/investors/governance-img1.webp";
        public string GovernanceBackgroundAlt { get; set; } = "";
        public List<InvestorLinkModel> SubsidiaryLinks { get; set; } = new();
        public string SubsidiariesImage { get; set; } = "";
        public string SubsidiariesImageAlt { get; set; } = "";
        public string ShareholderImage { get; set; } = "";
        public string ShareholderImageAlt { get; set; } = "";
        public List<ComponentModel> InvestorContact_List { get; set; } = new();
        public string InvestorContactTitle { get; set; } = "Investor Contact";
    }

    public class InvestorLinkModel
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public string UrlTarget { get; set; } = "_self";
    }
}
