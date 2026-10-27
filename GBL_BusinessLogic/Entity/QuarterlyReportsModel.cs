using System.Collections.Generic;

namespace GBL_BusinessLogic.Entity
{
    public class QuarterlyReportsModel
    {
        public ContentViewModel? Content { get; set; }
        public List<QuarterlyReportYearModel> Years { get; set; } = new();
    }

    public class QuarterlyReportYearModel
    {
        public int ContId { get; set; }
        public string Title { get; set; } = "";
        public string Key { get; set; } = "";
        public string PageName { get; set; } = "";
        public int Sequence { get; set; }
        public int SortYear { get; set; }
        public List<QuarterlyReportItemModel> Quarters { get; set; } = new();
    }

    public class QuarterlyReportItemModel
    {
        public string Title { get; set; } = "";
        public string QuarterCode { get; set; } = "";
        public string QuarterRange { get; set; } = "";
        public string PublishedOn { get; set; } = "";
        public int Sequence { get; set; }
        public string FinancialResultsUrl { get; set; } = "";
        public string ResultsPresentationUrl { get; set; } = "";
        public string TranscriptUrl { get; set; } = "";
        public string AudioUrl { get; set; } = "";
        public string DownloadAllUrl { get; set; } = "";
    }
}
