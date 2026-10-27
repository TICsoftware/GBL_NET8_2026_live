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

        private static readonly string[] CommitteesPageNames =
        {
            "committees-of-the-board",
            "investor-relations/committees-of-the-board",
            "committees-of-the-boards"
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

                if ((model.Content?.ContId ?? 0) > 0)
                {
                    break;
                }
            }

            model.Policies_List = LoadPdfArticles(PoliciesPageNames, languageId, geographyId);
            model.CodeOfConduct_List = LoadPdfArticles(CodeOfConductPageNames, languageId, geographyId);
            model.Committees_List = LoadCommittees(CommitteesPageNames, languageId, geographyId);

            return model;
        }

        private List<CommitteeModel> LoadCommittees(IEnumerable<string> pageNames, int languageId, int geographyId)
        {
            foreach (var name in pageNames)
            {
                var ds = GetContentComponentData_DAL(name, languageId, geographyId);
                if (ds == null || ds.Tables.Count < 2 || ds.Tables[1].Rows.Count == 0)
                {
                    continue;
                }

                var grouped = GetGroupedComponents(ds.Tables[1]);
                var committees = MapCommittees(grouped);
                if (committees.Count > 0)
                {
                    return committees;
                }
            }

            return new List<CommitteeModel>();
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
                var groups = grouped
                    .Where(g => (g.Fields.FirstOrDefault()?.sequence ?? 0) == sequence)
                    .ToList();

                if (groups.Count == 0) continue;

                // is_block = 0 → Component Title (committee name)
                var title = groups
                    .Where(g => (g.Fields.FirstOrDefault()?.IsBlock ?? 0) == 0)
                    .SelectMany(g => g.Fields)
                    .Where(f =>
                        f.FieldName.Equals("Component Title", StringComparison.OrdinalIgnoreCase)
                        || (f.FieldKey ?? "").Contains("main_component_title", StringComparison.OrdinalIgnoreCase))
                    .Select(f => StripHtml(f.FieldValue))
                    .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v))
                    ?? "";

                // is_block = 1 → Title (name) + Designation (role) + Sequence
                var members = groups
                    .Where(g => (g.Fields.FirstOrDefault()?.IsBlock ?? 0) == 1)
                    .Select(g =>
                    {
                        var dict = Config_Application_Website.GetFieldDictionary(g);
                        return new CommitteeMemberModel
                        {
                            Name = StripHtml(Config_Application_Website.GetValue(dict, "Title")),
                            Role = StripHtml(Config_Application_Website.GetValue(dict, "Designation")),
                            Sequence = Config_Application_Website.GetIntValue(dict, "Sequence")
                        };
                    })
                    .Where(m => !string.IsNullOrWhiteSpace(m.Name))
                    .OrderBy(m => m.Sequence)
                    .ThenBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
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
