using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class CorporateGovernance_BAL : BasePageBAL
    {
        public const int PdfPageSize = 100;

        private static readonly string[] CorporateGovernancePageNames =
        {
            "corporate-governance",
            "CorporateGovernance",
            "investor-relations/corporate-governance"
        };

        private static readonly string[] PoliciesPageNames =
        {
            "policies",
            "investor-relations/policies"
        };

        private static readonly string[] CodeOfConductPageNames =
        {
            "code-of-conduct",
            "investor-relations/code-of-conduct"
        };

        public CorporateGovernance_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public CorporateGovernanceModel GetCorporateGovernance_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new CorporateGovernanceModel();
            var names = CorporateGovernancePageNames
                .Prepend(NormalizePageName(pagename))
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var name in names)
            {
                var ds = GetContentComponentData_DAL(name, languageId, geographyId);
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    continue;
                }

                model.Content = MapContent(ds.Tables[0].Rows[0]);

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                {
                    var grouped = GetGroupedComponents(ds.Tables[1]);
                    model.Committees_List = MapCommittees(grouped);
                }

                if ((model.Content?.ContId ?? 0) > 0)
                {
                    break;
                }
            }

            model.Policies_List = LoadPdfArticles(PoliciesPageNames, languageId, geographyId);
            model.CodeOfConduct_List = LoadPdfArticles(CodeOfConductPageNames, languageId, geographyId);

            return model;
        }

        private List<ArticleModel> LoadPdfArticles(IEnumerable<string> pageNames, int languageId, int geographyId)
        {
            foreach (var name in pageNames)
            {
                var ds = GetContentComponentData_DAL(name, languageId, geographyId);
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    continue;
                }

                var content = MapContent(ds.Tables[0].Rows[0]);
                if (content.ContId <= 0) continue;

                var paged = Get_Investor_Pdf_page_wise_DAL(content.ContId, 1, PdfPageSize);
                if (paged != null && paged.Tables.Count > 0 && paged.Tables[0].Rows.Count > 0)
                {
                    return Config_Application_Website.MapArticleList(paged.Tables[0]);
                }

                if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    return Config_Application_Website.MapArticleList(ds.Tables[2]);
                }
            }

            return new List<ArticleModel>();
        }

        private List<CommitteeModel> MapCommittees(List<ComponentGroup> grouped)
        {
            var sequences = grouped
                .Select(g => g.Fields.FirstOrDefault()?.sequence ?? 0)
                .Where(s => s > 0)
                .Distinct()
                .OrderBy(s => s);

            var list = new List<CommitteeModel>();

            foreach (var sequence in sequences)
            {
                var items = MapComponents(grouped, sequence);
                if (items == null || items.Count == 0) continue;

                var titleItem = items.FirstOrDefault(x => x.IsBlock == 1)
                    ?? items.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Title) && x.IsBlock != 0);

                var title = StripHtml(
                    titleItem?.Title
                    ?? titleItem?.Section_title
                    ?? titleItem?.DisplayTitle
                    ?? titleItem?.BlockDisplayTitle
                    ?? "");

                var members = items
                    .Where(x => x.IsBlock == 0 && !string.IsNullOrWhiteSpace(x.Title))
                    .Select(x => new CommitteeMemberModel
                    {
                        Name = StripHtml(x.Title),
                        Role = StripHtml(
                            !string.IsNullOrWhiteSpace(x.Designation)
                                ? x.Designation
                                : x.Component_Designation),
                        Sequence = x.Sequence
                    })
                    .OrderBy(x => x.Sequence)
                    .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (string.IsNullOrWhiteSpace(title) && members.Count == 0)
                {
                    continue;
                }

                list.Add(new CommitteeModel
                {
                    Title = title,
                    Sequence = sequence,
                    Members = members
                });
            }

            return list;
        }

        private static string NormalizePageName(string? pagename)
        {
            if (string.IsNullOrWhiteSpace(pagename)) return string.Empty;
            return pagename.Trim().TrimStart('/');
        }

        private static string StripHtml(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            var decoded = System.Net.WebUtility.HtmlDecode(value);
            decoded = Regex.Replace(decoded, "<[^>]+>", " ");
            return Regex.Replace(decoded, @"\s+", " ").Trim();
        }
    }
}
