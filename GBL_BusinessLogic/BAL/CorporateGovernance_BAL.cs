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
        public const int PdfPageSize = 500;
        public const int PdfVisibleCount = 12;

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
            DataSet? parentDs = null;

            foreach (var name in GetPageNames(pagename))
            {
                try
                {
                    parentDs = GetContentComponentData_DAL(name, languageId, geographyId);
                }
                catch
                {
                    continue;
                }

                if (parentDs == null || parentDs.Tables.Count == 0 || parentDs.Tables[0].Rows.Count == 0)
                {
                    continue;
                }

                model.Content = MapContent(parentDs.Tables[0].Rows[0]);
                if ((model.Content?.ContId ?? 0) > 0)
                {
                    break;
                }
            }

            var parentId = model.Content?.ContId ?? 0;
            var sections = new List<GovernanceSectionModel>();
            try
            {
                sections = LoadChildSections(parentId, parentDs, languageId, geographyId);
            }
            catch
            {
                sections = new List<GovernanceSectionModel>();
            }

            sections = MergeKnownSections(pagename, model.Content?.PageName, sections, languageId, geographyId);

            if (sections.Count == 0 && IsCorporateGovernancePage(pagename, model.Content?.PageName))
            {
                sections = LoadLegacyCorporateGovernanceSections(languageId, geographyId);
            }

            AttachParentArticlesIfTabsEmpty(parentId, parentDs, sections);

            RefreshSectionYears(sections);
            AssignYearFilters(sections);
            model.Sections = sections;
            return model;
        }

        public static string FinancialYearKey(ArticleModel? article)
        {
            if (article == null) return "";
            var fromTitle = FinancialYearKeyFromText(article.Title);
            if (!string.IsNullOrWhiteSpace(fromTitle)) return fromTitle;
            return FinancialYearKey(article.DisplayDate);
        }

        public static string FinancialYearKey(DateTime? date)
        {
            if (!date.HasValue) return "";
            // Indian FY: April YYYY – March YYYY+1, shown as YYYY-YYYY+1
            var start = date.Value.Month >= 4 ? date.Value.Year : date.Value.Year - 1;
            return start + "-" + (start + 1);
        }

        public static string FinancialYearKeyFromText(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";

            var match = Regex.Match(
                text,
                @"(?:FY\s*)?(?<start>(?:19|20)\d{2})\s*[-–/]\s*(?<end>(?:19|20)?\d{2})",
                RegexOptions.IgnoreCase);
            if (!match.Success) return "";

            if (!int.TryParse(match.Groups["start"].Value, out var start)) return "";
            var endRaw = match.Groups["end"].Value;
            int end;
            if (endRaw.Length == 2)
            {
                if (!int.TryParse(endRaw, out var shortEnd)) return "";
                end = (start / 100) * 100 + shortEnd;
                if (end < start) end += 100;
            }
            else if (!int.TryParse(endRaw, out end))
            {
                return "";
            }

            return start + "-" + end;
        }

        public static string FinancialYearLabel(string? key)
        {
            if (string.IsNullOrWhiteSpace(key)) return "";
            var parts = key.Split('-');
            if (parts.Length >= 2)
            {
                return "FY " + parts[0] + " - " + parts[1];
            }

            return key;
        }

        private List<GovernanceSectionModel> LoadChildSections(int parentId, DataSet? parentDs, int languageId, int geographyId)
        {
            var children = MapChildPages(LoadChildTable(parentId, parentDs));
            var sections = new List<GovernanceSectionModel>();
            var usedAnchors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var child in children)
            {
                var section = BuildSection(child, languageId, geographyId, usedAnchors);
                if (section.Articles.Count == 0 && section.Committees.Count == 0)
                {
                    continue;
                }

                sections.Add(section);
            }

            return sections;
        }

        private List<GovernanceSectionModel> LoadLegacyCorporateGovernanceSections(int languageId, int geographyId)
        {
            var usedAnchors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var sections = new List<GovernanceSectionModel>();

            var policies = BuildSectionFromPageNames("Policies", "policies", PoliciesPageNames, languageId, geographyId, usedAnchors);
            if (policies.Articles.Count > 0 || policies.Committees.Count > 0) sections.Add(policies);

            var code = BuildSectionFromPageNames("Code of Conduct", "code-of-conduct", CodeOfConductPageNames, languageId, geographyId, usedAnchors);
            if (code.Articles.Count > 0 || code.Committees.Count > 0) sections.Add(code);

            var committees = BuildSectionFromPageNames("Committees of the Board", "committees", CommitteesPageNames, languageId, geographyId, usedAnchors);
            if (committees.Articles.Count > 0 || committees.Committees.Count > 0) sections.Add(committees);

            return sections;
        }

        private GovernanceSectionModel BuildSectionFromPageNames(
            string title,
            string anchor,
            IEnumerable<string> pageNames,
            int languageId,
            int geographyId,
            HashSet<string> usedAnchors)
        {
            var child = new ArticleModel
            {
                Title = title,
                PageName = pageNames.FirstOrDefault() ?? anchor
            };

            foreach (var name in pageNames)
            {
                var ds = GetContentComponentData_DAL(name, languageId, geographyId);
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    continue;
                }

                var content = MapContent(ds.Tables[0].Rows[0]);
                if (content.ContId <= 0) continue;

                child.ContId = content.ContId;
                child.Title = FirstNonEmpty(content.ContHmpgTitle, content.ContTitle, title);
                child.PageName = FirstNonEmpty(content.PageName, name);
                break;
            }

            var section = BuildSection(child, languageId, geographyId, usedAnchors, anchor);
            if (string.IsNullOrWhiteSpace(section.Title)) section.Title = title;
            return section;
        }

        private GovernanceSectionModel BuildSection(
            ArticleModel child,
            int languageId,
            int geographyId,
            HashSet<string> usedAnchors,
            string? preferredAnchor = null)
        {
            var pageNames = GetPageNames(child.PageName).ToList();
            var articles = child.ContId > 0
                ? LoadPdfArticlesById(child.ContId)
                : new List<ArticleModel>();

            if (articles.Count == 0 && pageNames.Count > 0)
            {
                articles = LoadPdfArticles(pageNames, languageId, geographyId);
            }

            var committees = LoadCommittees(pageNames, languageId, geographyId);
            if (committees.Count == 0 && child.ContId > 0 && !string.IsNullOrWhiteSpace(child.PageName))
            {
                committees = LoadCommittees(new[] { child.PageName }, languageId, geographyId);
            }

            var years = BuildYearList(articles);
            var section = new GovernanceSectionModel
            {
                ContId = child.ContId,
                Title = FirstNonEmpty(child.Title, child.PageName, "Section"),
                PageName = child.PageName ?? "",
                Anchor = UniqueAnchor(preferredAnchor ?? child.PageName, child.ContId, usedAnchors),
                Articles = articles,
                Committees = committees,
                Years = years,
                SelectedYear = years.FirstOrDefault()?.Value ?? ""
            };

            return section;
        }

        private void AttachParentArticlesIfTabsEmpty(int parentId, DataSet? parentDs, List<GovernanceSectionModel> sections)
        {
            if (sections.Count == 0) return;
            if (sections.Take(2).Any(s => s.Articles.Count > 0)) return;

            var articles = LoadPdfArticlesById(parentId);
            if (articles.Count == 0 && parentDs != null && parentDs.Tables.Count > 2 && parentDs.Tables[2].Rows.Count > 0)
            {
                try
                {
                    articles = OrderPdfArticlesByDisplayDate(
                        Config_Application_Website.MapArticleList(parentDs.Tables[2]));
                }
                catch
                {
                    articles = new List<ArticleModel>();
                }
            }

            articles = articles
                .Where(a =>
                    !string.IsNullOrWhiteSpace(a.MediafilePath)
                    || !string.IsNullOrWhiteSpace(a.Url)
                    || !string.IsNullOrWhiteSpace(a.ExternalUrl))
                .ToList();

            if (articles.Count == 0) return;

            sections[0].Articles = articles;
            sections[0].Years = BuildYearList(articles);
            sections[0].SelectedYear = sections[0].Years.FirstOrDefault()?.Value ?? "";
        }

        private static void RefreshSectionYears(List<GovernanceSectionModel> sections)
        {
            foreach (var section in sections)
            {
                section.Years = BuildYearList(section.Articles);
                if (string.IsNullOrWhiteSpace(section.SelectedYear)
                    || section.Years.All(y => y.Value != section.SelectedYear))
                {
                    section.SelectedYear = section.Years.FirstOrDefault()?.Value ?? "";
                }
            }
        }

        private static void AssignYearFilters(List<GovernanceSectionModel> sections)
        {
            for (var i = 0; i < sections.Count && i < 2; i++)
            {
                var section = sections[i];
                if (section.IsCommitteeSection) continue;
                section.ShowYearFilter = section.Articles.Count > 0 && section.Years.Count > 0;
            }
        }

        private DataTable? LoadChildTable(int parentId, DataSet? parentDs)
        {
            if (parentId > 0)
            {
                try
                {
                    var ds = Get_Investor_Section_Children_DAL(parentId);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        return ds.Tables[0];
                    }
                }
                catch
                {
                    // SP may not be deployed yet.
                }

                try
                {
                    var ds = Get_Quarterly_Reports_Years_DAL(parentId);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        return ds.Tables[0];
                    }
                }
                catch
                {
                    // Optional fallback if quarterly SP is present.
                }
            }

            if (parentDs != null && parentDs.Tables.Count > 2 && parentDs.Tables[2].Rows.Count > 0)
            {
                return parentDs.Tables[2];
            }

            return null;
        }

        private static List<ArticleModel> MapChildPages(DataTable? table)
        {
            if (table == null || table.Rows.Count == 0)
            {
                return new List<ArticleModel>();
            }

            return table.AsEnumerable()
                .Select(row =>
                {
                    var title = FirstNonEmpty(
                        GetColumn(row, "cont_hmpg_title"),
                        GetColumn(row, "cont_title"));
                    var contId = table.Columns.Contains("cont_id") && row["cont_id"] != DBNull.Value
                        ? Convert.ToInt32(row["cont_id"])
                        : 0;

                    DateTime? displayDate = null;
                    if (table.Columns.Contains("cont_displaydate") && row["cont_displaydate"] != DBNull.Value)
                    {
                        displayDate = Convert.ToDateTime(row["cont_displaydate"]);
                    }

                    var sequence = 0;
                    if (table.Columns.Contains("cont_sequence") && row["cont_sequence"] != DBNull.Value)
                    {
                        sequence = Convert.ToInt32(row["cont_sequence"]);
                    }

                    return new ArticleModel
                    {
                        ContId = contId,
                        Title = StripHtml(title),
                        PageName = GetColumn(row, "cont_pagename"),
                        DisplayDate = displayDate,
                        Sequence = sequence
                    };
                })
                .Where(c => c.ContId > 0 || !string.IsNullOrWhiteSpace(c.PageName))
                .OrderBy(c => c.Sequence > 0 ? c.Sequence : 999999)
                .ThenByDescending(c => c.DisplayDate.HasValue)
                .ThenByDescending(c => c.DisplayDate)
                .ThenByDescending(c => c.ContId)
                .ToList();
        }

        private List<ArticleModel> LoadPdfArticlesById(int contentId, int depth = 0)
        {
            if (contentId <= 0) return new List<ArticleModel>();

            try
            {
                var paged = Get_Investor_Pdf_page_wise_DAL(contentId, 1, PdfPageSize);
                if (paged != null && paged.Tables.Count > 0 && paged.Tables[0].Rows.Count > 0)
                {
                    return OrderPdfArticlesByDisplayDate(
                        Config_Application_Website.MapArticleList(paged.Tables[0]));
                }
            }
            catch
            {
                // SP may not be deployed for this environment.
            }

            if (depth >= 2) return new List<ArticleModel>();

            var nested = new List<ArticleModel>();
            foreach (var child in MapChildPages(LoadChildTable(contentId, null)))
            {
                if (child.ContId <= 0 || child.ContId == contentId) continue;
                nested.AddRange(LoadPdfArticlesById(child.ContId, depth + 1));
            }

            return OrderPdfArticlesByDisplayDate(nested);
        }

        private List<ArticleModel> LoadPdfArticles(IEnumerable<string> pageNames, int languageId, int geographyId)
        {
            foreach (var name in pageNames)
            {
                DataSet? ds;
                try
                {
                    ds = GetContentComponentData_DAL(name, languageId, geographyId);
                }
                catch
                {
                    continue;
                }

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    continue;
                }

                var content = MapContent(ds.Tables[0].Rows[0]);
                if (content.ContId <= 0) continue;

                var articles = LoadPdfArticlesById(content.ContId);
                if (articles.Count > 0) return articles;

                if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
                {
                    try
                    {
                        return OrderPdfArticlesByDisplayDate(
                            Config_Application_Website.MapArticleList(ds.Tables[2]));
                    }
                    catch
                    {
                        // Table 2 may be components rather than articles.
                    }
                }
            }

            return new List<ArticleModel>();
        }

        private List<CommitteeModel> LoadCommittees(IEnumerable<string> pageNames, int languageId, int geographyId)
        {
            foreach (var name in pageNames)
            {
                if (string.IsNullOrWhiteSpace(name)) continue;

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

                var title = groups
                    .Where(g => (g.Fields.FirstOrDefault()?.IsBlock ?? 0) == 0)
                    .SelectMany(g => g.Fields)
                    .Where(f =>
                        f.FieldName.Equals("Component Title", StringComparison.OrdinalIgnoreCase)
                        || (f.FieldKey ?? "").Contains("main_component_title", StringComparison.OrdinalIgnoreCase))
                    .Select(f => StripHtml(f.FieldValue))
                    .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v))
                    ?? "";

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

        private static List<DropdownModel> BuildYearList(IEnumerable<ArticleModel> articles)
        {
            return articles
                .Select(FinancialYearKey)
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(key => key)
                .Select(key => new DropdownModel
                {
                    Value = key,
                    Text = FinancialYearLabel(key)
                })
                .ToList();
        }

        private static List<ArticleModel> OrderPdfArticlesByDisplayDate(IEnumerable<ArticleModel> articles)
        {
            return articles
                .OrderByDescending(a => FinancialYearKey(a))
                .ThenByDescending(a => a.DisplayDate.HasValue)
                .ThenByDescending(a => a.DisplayDate)
                .ThenByDescending(a => a.ContId)
                .ToList();
        }

        private List<GovernanceSectionModel> MergeKnownSections(
            string? requested,
            string? resolvedPageName,
            List<GovernanceSectionModel> sections,
            int languageId,
            int geographyId)
        {
            var known = GetKnownTabs(requested, resolvedPageName);
            if (known.Count == 0) return sections;

            var usedAnchors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var remaining = sections.ToList();
            var merged = new List<GovernanceSectionModel>();

            foreach (var tab in known)
            {
                var existing = remaining.FirstOrDefault(s =>
                    SectionMatchesTab(s, tab.Anchor, tab.Title));

                if (existing != null)
                {
                    remaining.Remove(existing);
                    existing.Anchor = tab.Anchor;
                    if (string.IsNullOrWhiteSpace(existing.Title)) existing.Title = tab.Title;
                    usedAnchors.Add(existing.Anchor);
                    merged.Add(existing);
                    continue;
                }

                var loaded = BuildSectionFromPageNames(
                    tab.Title,
                    tab.Anchor,
                    tab.PageNames,
                    languageId,
                    geographyId,
                    usedAnchors);
                loaded.Title = FirstNonEmpty(loaded.Title, tab.Title);
                loaded.Anchor = tab.Anchor;
                usedAnchors.Add(tab.Anchor);
                merged.Add(loaded);
            }

            foreach (var extra in remaining)
            {
                extra.Anchor = UniqueAnchor(extra.Anchor, extra.ContId, usedAnchors);
                merged.Add(extra);
            }

            return merged;
        }

        private static bool SectionMatchesTab(GovernanceSectionModel section, string anchor, string title)
        {
            if (section.Anchor.Equals(anchor, StringComparison.OrdinalIgnoreCase)) return true;
            if (PageNameEndsWith(section.PageName, anchor)) return true;

            var haystack = ((section.Title ?? "") + " " + (section.PageName ?? "") + " " + (section.Anchor ?? "")).ToLowerInvariant();
            var needle = (anchor + " " + title).ToLowerInvariant();

            if (needle.Contains("committee") && haystack.Contains("committee")) return true;
            if (needle.Contains("polic") && haystack.Contains("polic")) return true;
            if (needle.Contains("code") && haystack.Contains("code") && haystack.Contains("conduct")) return true;
            if ((needle.Contains("agm") || needle.Contains("egm") || needle.Contains("eogm"))
                && (haystack.Contains("agm") || haystack.Contains("egm") || haystack.Contains("eogm")))
            {
                if (needle.Contains("outcome") && haystack.Contains("outcome")) return true;
                if (needle.Contains("board") && haystack.Contains("board")) return false;
                if (!needle.Contains("outcome") && !haystack.Contains("outcome") && !haystack.Contains("board")) return true;
            }
            if (needle.Contains("board") && needle.Contains("outcome") && haystack.Contains("board") && haystack.Contains("outcome")) return true;
            if (needle.Contains("board") && (needle.Contains("meeting") || needle.Contains("meetings"))
                && haystack.Contains("board") && haystack.Contains("meeting")) return true;

            if (!string.IsNullOrWhiteSpace(title)
                && (section.Title ?? "").Replace(" ", "").Equals(title.Replace(" ", ""), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static bool PageNameEndsWith(string? pageName, string slug)
        {
            var value = NormalizePageName(pageName);
            return value.Equals(slug, StringComparison.OrdinalIgnoreCase)
                || value.EndsWith("/" + slug, StringComparison.OrdinalIgnoreCase);
        }

        private static List<KnownTab> GetKnownTabs(params string?[] pageNames)
        {
            var blob = string.Join(" ", pageNames.Where(n => !string.IsNullOrWhiteSpace(n))).ToLowerInvariant();

            if (blob.Contains("outcome-of-meeting"))
            {
                return new List<KnownTab>
                {
                    new("AGM/EOGM", "agm-eogm-outcome", TabPageNames("agm-eogm-outcome", "outcome-of-meeting")),
                    new("Board Outcome", "board-outcome", TabPageNames("board-outcome", "outcome-of-meeting"))
                };
            }

            if (blob.Contains("notice-of-meeting"))
            {
                return new List<KnownTab>
                {
                    new("AGM / EGM", "agm-eogm", TabPageNames("agm-eogm", "notice-of-meeting")),
                    new("Board Meetings", "board-meetings", TabPageNames("board-meetings", "notice-of-meeting"))
                };
            }

            if (blob.Contains("corporate-governance"))
            {
                return new List<KnownTab>
                {
                    new("Policies", "policies", PoliciesPageNames.Concat(TabPageNames("policies", "corporate-governance")).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()),
                    new("Code of Conduct", "code-of-conduct", CodeOfConductPageNames.Concat(TabPageNames("code-of-conduct", "corporate-governance")).Distinct(StringComparer.OrdinalIgnoreCase).ToArray()),
                    new("Committees of the Board", "committees", CommitteesPageNames.Concat(TabPageNames("committees-of-the-board", "corporate-governance")).Concat(new[] { "committees" }).Distinct(StringComparer.OrdinalIgnoreCase).ToArray())
                };
            }

            return new List<KnownTab>();
        }

        private static string[] TabPageNames(string slug, string parentSlug)
        {
            return new[]
            {
                slug,
                parentSlug + "/" + slug,
                "general-meeting/" + slug,
                "general-meeting/" + parentSlug + "/" + slug,
                "investor-relations/" + slug,
                "investor-relations/" + parentSlug + "/" + slug,
                "investor-relations/general-meeting/" + slug,
                "investor-relations/general-meeting/" + parentSlug + "/" + slug
            };
        }

        private readonly record struct KnownTab(string Title, string Anchor, string[] PageNames);

        private static IEnumerable<string> GetPageNames(string? pagename)
        {
            var requested = NormalizePageName(pagename);
            if (string.IsNullOrWhiteSpace(requested))
            {
                yield break;
            }

            var names = new List<string>();
            void Add(string? value)
            {
                var name = NormalizePageName(value);
                if (string.IsNullOrWhiteSpace(name)) return;
                if (!names.Exists(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    names.Add(name);
                }
            }

            Add(requested);

            var parts = requested.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < parts.Length; i++)
            {
                Add(string.Join("/", parts.Skip(i)));
            }

            var last = parts[^1];
            Add(last);
            Add("investor-relations/" + last);
            Add("general-meeting/" + last);
            Add("investor-relations/general-meeting/" + last);

            if (parts.Length >= 2)
            {
                var lastTwo = string.Join("/", parts.Skip(parts.Length - 2));
                Add(lastTwo);
                Add("investor-relations/" + lastTwo);
            }

            foreach (var name in names)
            {
                yield return name;
            }
        }

        private static bool IsCorporateGovernancePage(string? requested, string? resolved)
        {
            return ContainsPageToken(requested, "corporate-governance")
                || ContainsPageToken(resolved, "corporate-governance");
        }

        private static bool ContainsPageToken(string? value, string token)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            return value.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string UniqueAnchor(string? pageName, int contId, HashSet<string> used)
        {
            var slug = (pageName ?? "").Trim().Trim('/');
            if (!string.IsNullOrWhiteSpace(slug) && slug.Contains('/'))
            {
                slug = slug.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
            }

            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = "section-" + contId;
            }

            slug = Regex.Replace(slug.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
            if (string.IsNullOrWhiteSpace(slug))
            {
                slug = "section-" + contId;
            }

            var unique = slug;
            var n = 2;
            while (!used.Add(unique))
            {
                unique = slug + "-" + n;
                n++;
            }

            return unique;
        }

        private static string GetColumn(DataRow row, string column)
        {
            if (row?.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
            {
                return "";
            }

            return row[column]?.ToString()?.Trim() ?? "";
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
