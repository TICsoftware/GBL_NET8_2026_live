using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class InvestorRelations_BAL : BasePageBAL
    {
        private const string DefaultGovernanceBackground = "/Assets/images/investors/governance-img1.webp";
        private const string DefaultShareholderImage = "/Assets/images/investors/information-img.webp";
        private const string DefaultSubsidiariesImage = "/Assets/images/investors/subsidiaries-img.webp";
        private const string DefaultAnnualThumb = "/Assets/images/investors/annual-report-img.webp";

        private static readonly string[] PageNames =
        {
            "investor-relations",
            "Investor-Relations",
            "InvestorRelations"
        };

        private static readonly (string Display, string[] Slugs, string Url)[] ShareholderOrder =
        {
            ("Annual Report", new[] { "annual-report", "annual-reports" }, "/investor-relations/annual-reports"),
            ("Shareholding Pattern", new[] { "shareholding-pattern" }, "/investor-relations/shareholding-pattern"),
            ("Subsidiaries", new[] { "subsidiaries" }, "/investor-relations/subsidiaries"),
            ("Corporate governance", new[] { "corporate-governance" }, "/investor-relations/corporate-governance"),
            ("General Meeting", new[] { "general-meeting", "general-meetings" }, "/investor-relations/general-meeting"),
            ("Disclosures", new[] { "disclosures" }, "/investor-relations/disclosures"),
            ("Quarterly Reports", new[] { "quarterly-reports", "quarterly-report" }, "/investor-relations/quarterly-reports"),
            ("Postal Ballot", new[] { "postal-ballot" }, "/investor-relations/postal-ballot")
        };

        public InvestorRelations_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public InvestorRelationsModel GetInvestorRelations_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new InvestorRelationsModel();
            DataSet? parentDs = null;

            foreach (var name in GetPageNames(pagename))
            {
                parentDs = GetContentComponentData_DAL(name, languageId, geographyId);
                if (parentDs == null || parentDs.Tables.Count == 0 || parentDs.Tables[0].Rows.Count == 0)
                {
                    continue;
                }

                model.Content = MapContent(parentDs.Tables[0].Rows[0]);
                if ((model.Content?.ContId ?? 0) > 0)
                {
                    if (string.IsNullOrWhiteSpace(model.Content.MastheadImage))
                    {
                        model.Content.MastheadImage = "/Assets/images/banners/investor-banner.webp";
                    }
                    break;
                }
            }

            var contentId = model.Content?.ContId ?? 0;
            var homeDs = contentId > 0 ? SafeHomeData(contentId) : null;

            var sections = homeDs != null && homeDs.Tables.Count > 0
                ? Config_Application_Website.MapArticleList(homeDs.Tables[0])
                : new List<ArticleModel>();

            var nested = homeDs != null && homeDs.Tables.Count > 1
                ? Config_Application_Website.MapArticleList(homeDs.Tables[1])
                : new List<ArticleModel>();

            if (homeDs != null && homeDs.Tables.Count > 2)
            {
                MapWhatsNew(model, homeDs.Tables[2]);
            }

            var cgChildren = homeDs != null && homeDs.Tables.Count > 3
                ? Config_Application_Website.MapArticleList(homeDs.Tables[3])
                : new List<ArticleModel>();

            BindShareholderImage(model);
            BindGovernanceBackground(model, sections);
            model.ShareholderLinks = BuildShareholderLinks(sections);
            model.GovernanceLinks = BuildGovernanceLinks(cgChildren, sections, nested);

            MapAnnualReport(model, sections, nested);
            MapSubsidiaries(model, sections, nested);
            MapLatestQuarter(model, languageId, geographyId);
            MapInvestorContactSection(model, sections, homeDs, languageId, geographyId);

            return model;
        }

        private DataSet? SafeHomeData(int contentId)
        {
            try
            {
                return Get_Investor_Relations_Home_DAL(contentId);
            }
            catch
            {
                return null;
            }
        }

        private void MapInvestorContactSection(
            InvestorRelationsModel model,
            List<ArticleModel> sections,
            DataSet? homeDs,
            int languageId,
            int geographyId)
        {
            var section = FindSection(sections,
                "investors-contact",
                "investor-contact",
                "investorscontact",
                "investorcontact");

            if (homeDs != null && homeDs.Tables.Count > 4 && homeDs.Tables[4].Rows.Count > 0)
            {
                MapInvestorContact(model, homeDs.Tables[4]);
            }

            if (model.InvestorContact_List.Count == 0 && section != null)
            {
                foreach (var name in ContactPageNames(section.PageName))
                {
                    try
                    {
                        var ds = GetContentComponentData_DAL(name, languageId, geographyId);
                        if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                        {
                            MapInvestorContact(model, ds.Tables[1]);
                            if (model.InvestorContact_List.Count > 0) break;
                        }
                    }
                    catch
                    {
                        // Page name may not resolve until the contact subsection is published.
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(section?.Title))
            {
                model.InvestorContactTitle = section.Title;
            }
            else if (string.IsNullOrWhiteSpace(model.InvestorContactTitle))
            {
                model.InvestorContactTitle = "Investor Contact";
            }
        }

        private static IEnumerable<string> ContactPageNames(string? pageName)
        {
            var names = new List<string>();
            void Add(string? value)
            {
                var name = (value ?? "").Trim().Trim('/');
                if (string.IsNullOrWhiteSpace(name)) return;
                if (!names.Exists(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    names.Add(name);
                }
            }

            Add(pageName);
            Add(LastSegment(pageName));
            Add("investors-contact");
            Add("investor-contact");
            Add("investor-relations/investors-contact");
            Add("investor-relations/investor-contact");
            return names;
        }

        private void MapInvestorContact(InvestorRelationsModel model, DataTable table)
        {
            List<ComponentGroup> grouped;
            try
            {
                grouped = GetGroupedComponents(table);
            }
            catch
            {
                return;
            }

            if (grouped.Count == 0) return;

            var sequences = grouped
                .Select(g => g.Fields.FirstOrDefault()?.sequence ?? 0)
                .Distinct()
                .OrderBy(s => s);

            foreach (var sequence in sequences)
            {
                model.InvestorContact_List.AddRange(MapComponents(grouped, sequence));
            }

            model.InvestorContact_List = model.InvestorContact_List
                .Where(c => !string.IsNullOrWhiteSpace(c.Title) || !string.IsNullOrWhiteSpace(c.Content))
                .OrderBy(c => c.Sequence)
                .ToList();
        }

        private static void MapWhatsNew(InvestorRelationsModel model, DataTable table)
        {
            if (table == null || table.Rows.Count == 0) return;

            var items = table.AsEnumerable()
                .Select(MapWhatsNewItem)
                .Where(a => a.ContId > 0)
                .OrderByDescending(a => a.DisplayDate.HasValue)
                .ThenByDescending(a => a.DisplayDate)
                .ThenByDescending(a => a.ContId)
                .ToList();

            model.WhatsNewPress = items
                .Where(a => IsPress(a))
                .Take(2)
                .ToList();

            model.WhatsNewMedia = items
                .Where(a => IsMedia(a))
                .Take(1)
                .ToList();
        }

        private static ArticleModel MapWhatsNewItem(DataRow row)
        {
            var table = row.Table;
            var tag = table.Columns.Contains("TagNames") ? row["TagNames"]?.ToString() ?? "" : "";
            var pageUrl = (row["pageurl"]?.ToString() ?? "").Replace("/global", "");
            var pageName = row["cont_pagename"]?.ToString() ?? "";
            var external = row["cont_external_url"]?.ToString() ?? "";

            var url = external;
            var target = "_blank";
            if (string.IsNullOrWhiteSpace(url))
            {
                url = pageUrl;
                target = "_self";
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                var slug = LastSegment(pageName);
                url = IsMediaTag(tag)
                    ? "/media/media-coverage/" + slug
                    : "/media/press-release/" + slug;
                target = "_self";
            }

            if (!string.IsNullOrWhiteSpace(url) && !url.StartsWith("/") && !url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                url = "/" + url.TrimStart('/');
            }

            return new ArticleModel
            {
                ContId = row.Field<int>("cont_id"),
                ContParentId = row.Field<int>("cont_parent_id"),
                Title = FirstNonEmpty(row["cont_hmpg_title"]?.ToString(), row["cont_title"]?.ToString()),
                Intro = row["cont_intro"]?.ToString() ?? "",
                HmpgIntro = row["cont_hmpg_intro"]?.ToString() ?? "",
                PageName = pageName,
                ThumbnailImage = row["Hmpg_thumbnail"]?.ToString() ?? "",
                ThumbnailAltText = row["Hmpg_thumbnail_alt_text"]?.ToString() ?? "",
                ExternalUrl = external,
                Url = url,
                UrlTarget = target,
                DisplayDate = table.Columns.Contains("cont_displaydate") && row["cont_displaydate"] != DBNull.Value
                    ? Convert.ToDateTime(row["cont_displaydate"])
                    : null,
                Sequence = table.Columns.Contains("cont_sequence") && row["cont_sequence"] != DBNull.Value
                    ? Convert.ToInt32(row["cont_sequence"])
                    : 0,
                Tag_list = tag,
                Content = row["content"]?.ToString() ?? ""
            };
        }

        private void MapAnnualReport(InvestorRelationsModel model, List<ArticleModel> sections, List<ArticleModel> nested)
        {
            var section = FindSection(sections, "annual-report", "annual-reports");
            if (section != null)
            {
                try
                {
                    var ds = Get_Annual_Reports_page_wise_DAL(section.ContId, 1, 1);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        model.LatestAnnualReport = Config_Application_Website.MapArticleList(ds.Tables[0]).FirstOrDefault();
                    }
                }
                catch
                {
                    // SP may not be deployed; fall through to nested articles.
                }
            }

            if (model.LatestAnnualReport == null && section != null)
            {
                model.LatestAnnualReport = nested
                    .Where(a => a.ContParentId == section.ContId)
                    .OrderByDescending(a => a.DisplayDate.HasValue)
                    .ThenByDescending(a => a.DisplayDate)
                    .ThenByDescending(a => a.ContId)
                    .FirstOrDefault();
            }

            if (model.LatestAnnualReport != null && string.IsNullOrWhiteSpace(model.LatestAnnualReport.ThumbnailImage))
            {
                model.LatestAnnualReport.ThumbnailImage = DefaultAnnualThumb;
            }
        }

        private static void BindShareholderImage(InvestorRelationsModel model)
        {
            model.ShareholderImage = FirstNonEmpty(
                model.Content?.Hmpg_thumbnail,
                model.Content?.BackgroundImage,
                DefaultShareholderImage);
            model.ShareholderImageAlt = FirstNonEmpty(
                model.Content?.Hmpg_thumbnail_alt_text,
                model.Content?.Background_image_Alt_text,
                "Shareholders reviewing company documents together");
        }

        private static void BindGovernanceBackground(InvestorRelationsModel model, List<ArticleModel> sections)
        {
            var section = FindSection(sections, "corporate-governance");
            model.GovernanceBackground = FirstNonEmpty(
                section?.BackgroundImage,
                section?.MastheadImage,
                section?.ThumbnailImage,
                DefaultGovernanceBackground);
            model.GovernanceBackgroundAlt = FirstNonEmpty(
                section?.BackgroundAltText,
                section?.MastheadAltText,
                section?.ThumbnailAltText,
                "Corporate Governance");
        }

        private void MapSubsidiaries(InvestorRelationsModel model, List<ArticleModel> sections, List<ArticleModel> nested)
        {
            var section = FindSection(sections, "subsidiaries");
            if (section == null) return;

            model.SubsidiariesImage = FirstNonEmpty(section.ThumbnailImage, DefaultSubsidiariesImage);
            model.SubsidiariesImageAlt = FirstNonEmpty(section.ThumbnailAltText, "Representatives meeting across Godavari Biorefineries subsidiaries");

            List<ArticleModel> articles = new();
            try
            {
                var ds = Get_Investor_Pdf_page_wise_DAL(section.ContId, 1, 12, 0);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    articles = Config_Application_Website.MapArticleList(ds.Tables[0]);
                }
            }
            catch
            {
                articles = new List<ArticleModel>();
            }

            if (articles.Count == 0)
            {
                articles = nested
                    .Where(a => a.ContParentId == section.ContId)
                    .OrderByDescending(a => a.DisplayDate.HasValue)
                    .ThenByDescending(a => a.DisplayDate)
                    .ThenBy(a => a.Sequence)
                    .Take(12)
                    .ToList();
            }

            model.SubsidiaryLinks = articles
                .Where(a => !string.IsNullOrWhiteSpace(a.Title))
                .Select(a => new InvestorLinkModel
                {
                    Title = a.Title ?? "",
                    Url = FirstNonEmpty(a.MediafilePath, a.Url, a.ExternalUrl, "#"),
                    UrlTarget = "_blank"
                })
                .ToList();
        }

        private void MapLatestQuarter(InvestorRelationsModel model, int languageId, int geographyId)
        {
            try
            {
                using var qrBal = new QuarterlyReports_BAL(_configuration);
                var qr = qrBal.GetQuarterlyReports_BAL("quarterly-reports", languageId, geographyId);
                var year = qr.Years.FirstOrDefault();
                if (year == null || year.Quarters.Count == 0) return;

                model.LatestQuarter = year.Quarters
                    .OrderByDescending(q => QuarterNumber(q.QuarterCode))
                    .ThenByDescending(q => q.Sequence)
                    .FirstOrDefault();
            }
            catch
            {
                // Quarterly SP / page may not be available yet.
            }
        }

        private static List<InvestorLinkModel> BuildShareholderLinks(List<ArticleModel> sections)
        {
            var links = new List<InvestorLinkModel>();
            foreach (var item in ShareholderOrder)
            {
                var match = sections.FirstOrDefault(s => MatchesSection(s, item.Slugs, item.Display));
                if (match == null) continue;

                links.Add(new InvestorLinkModel
                {
                    Title = FirstNonEmpty(match.Title, item.Display),
                    Url = item.Url,
                    UrlTarget = "_self"
                });
            }

            return links;
        }

        private static List<InvestorLinkModel> BuildGovernanceLinks(
            List<ArticleModel> cgChildren,
            List<ArticleModel> sections,
            List<ArticleModel> nested)
        {
            var children = cgChildren;
            if (children.Count == 0)
            {
                var cg = FindSection(sections, "corporate-governance");
                if (cg != null)
                {
                    children = nested.Where(a => a.ContParentId == cg.ContId).ToList();
                }
            }

            return children
                .Where(a => !string.IsNullOrWhiteSpace(a.Title))
                .OrderBy(GovernanceRank)
                .ThenBy(a => a.Sequence)
                .Select(a => new InvestorLinkModel
                {
                    Title = a.Title ?? "",
                    Url = "/investor-relations/corporate-governance#" + GovernanceAnchor(a),
                    UrlTarget = "_self"
                })
                .ToList();
        }

        private static int GovernanceRank(ArticleModel item)
        {
            var hay = ((item.PageName ?? "") + " " + (item.Title ?? "")).ToLowerInvariant();
            if (hay.Contains("polic")) return 0;
            if (hay.Contains("code-of-conduct") || hay.Contains("code of conduct")) return 1;
            if (hay.Contains("committee")) return 2;
            return 10;
        }

        private static string GovernanceAnchor(ArticleModel item)
        {
            var hay = ((item.PageName ?? "") + " " + (item.Title ?? "")).ToLowerInvariant();
            if (hay.Contains("polic")) return "policies";
            if (hay.Contains("code-of-conduct") || hay.Contains("code of conduct")) return "code-of-conduct";
            if (hay.Contains("committee")) return "committees";

            var slug = LastSegment(item.PageName);
            slug = Regex.Replace((slug ?? "").ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
            return string.IsNullOrWhiteSpace(slug) ? "section-" + item.ContId : slug;
        }

        private static ArticleModel? FindSection(List<ArticleModel> sections, params string[] slugs)
        {
            return sections.FirstOrDefault(s => MatchesSection(s, slugs, slugs.FirstOrDefault() ?? ""));
        }

        private static bool MatchesSection(ArticleModel item, string[] slugs, string displayName)
        {
            var slug = LastSegment(item.PageName);
            if (slugs.Any(s => slug.Equals(s, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            var title = Regex.Replace(item.Title ?? "", @"\s+", " ").Trim();
            if (title.Equals(displayName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var compactTitle = Regex.Replace(title, @"[\s\-]+", "");
            return slugs.Any(s => compactTitle.Equals(Regex.Replace(s, @"[\s\-]+", ""), StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsPress(ArticleModel item)
        {
            return !IsMedia(item);
        }

        private static bool IsMedia(ArticleModel item)
        {
            return IsMediaTag(item.Tag_list) || (item.PageName ?? "").IndexOf("media-coverage", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsMediaTag(string? tag)
        {
            return (tag ?? "").IndexOf("media", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static int QuarterNumber(string? code)
        {
            var match = Regex.Match(code ?? "", @"Q\s*(\d)", RegexOptions.IgnoreCase);
            return match.Success && int.TryParse(match.Groups[1].Value, out var n) ? n : 0;
        }

        private static string LastSegment(string? pageName)
        {
            var value = (pageName ?? "").Trim().Trim('/');
            if (string.IsNullOrWhiteSpace(value)) return "";
            var parts = value.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0 ? value : parts[^1];
        }

        private static IEnumerable<string> GetPageNames(string? pagename)
        {
            var requested = (pagename ?? "").Trim().Trim('/');
            if (!string.IsNullOrWhiteSpace(requested))
            {
                yield return requested;
            }

            foreach (var name in PageNames)
            {
                if (!name.Equals(requested, StringComparison.OrdinalIgnoreCase))
                {
                    yield return name;
                }
            }
        }

        private static string FirstNonEmpty(params string?[] values)
        {
            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }

            return "";
        }
    }
}
