using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class QuarterlyReports_BAL : BasePageBAL
    {
        private static readonly string[] PageNames =
        {
            "quarterly-reports",
            "quarterly-report",
            "investor-relations/quarterly-reports",
            "investor-relations/quarterly-report"
        };

        public QuarterlyReports_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public QuarterlyReportsModel GetQuarterlyReports_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new QuarterlyReportsModel();
            var names = PageNames
                .Prepend(NormalizePageName(pagename))
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            DataSet? parentDs = null;
            foreach (var name in names)
            {
                parentDs = GetContentComponentData_DAL(name, languageId, geographyId);
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
            var years = LoadYears(parentId, parentDs, languageId, geographyId);

            if (years.Count == 0 && parentDs != null && parentDs.Tables.Count > 1 && parentDs.Tables[1].Rows.Count > 0)
            {
                var quarters = MapQuarters(parentDs.Tables[1]);
                if (quarters.Count > 0)
                {
                    var title = StripHtml(model.Content?.ContTitle);
                    years.Add(new QuarterlyReportYearModel
                    {
                        ContId = parentId,
                        Title = string.IsNullOrWhiteSpace(title) ? "Quarterly Reports" : title,
                        Key = "current",
                        PageName = model.Content?.PageName ?? "",
                        SortYear = ParseSortYear(title)
                    });
                    years[0].Quarters = quarters;
                }
            }

            EnsureUniqueYearKeys(years);

            model.Years = years
                .OrderByDescending(y => y.SortYear)
                .ThenBy(y => y.Sequence)
                .ThenByDescending(y => y.ContId)
                .ToList();

            return model;
        }

        private List<QuarterlyReportYearModel> LoadYears(int parentId, DataSet? parentDs, int languageId, int geographyId)
        {
            var years = new List<QuarterlyReportYearModel>();
            DataTable? detailTable = null;

            if (parentId > 0)
            {
                try
                {
                    var yearDs = Get_Quarterly_Reports_Years_DAL(parentId);
                    if (yearDs != null && yearDs.Tables.Count > 0 && yearDs.Tables[0].Rows.Count > 0)
                    {
                        years = MapYears(yearDs.Tables[0]);
                    }

                    if (yearDs != null && yearDs.Tables.Count > 1 && yearDs.Tables[1].Rows.Count > 0)
                    {
                        detailTable = yearDs.Tables[1];
                    }
                }
                catch
                {
                    // SP may not be deployed yet; fall back to parent children / year pagenames.
                }
            }

            if (years.Count == 0 && parentDs != null && parentDs.Tables.Count > 2 && parentDs.Tables[2].Rows.Count > 0)
            {
                years = MapYearsFromArticles(parentDs.Tables[2]);
            }

            foreach (var year in years)
            {
                if (year.Quarters.Count == 0 && detailTable != null && detailTable.Columns.Contains("cont_id"))
                {
                    var rows = detailTable.AsEnumerable()
                        .Where(r => (r.Field<int?>("cont_id") ?? 0) == year.ContId);
                    if (rows.Any())
                    {
                        year.Quarters = MapQuarters(rows.CopyToDataTable());
                    }
                }

                if (year.Quarters.Count == 0 && !string.IsNullOrWhiteSpace(year.PageName))
                {
                    var yearDs = GetContentComponentData_DAL(year.PageName, languageId, geographyId);
                    if (yearDs != null && yearDs.Tables.Count > 1 && yearDs.Tables[1].Rows.Count > 0)
                    {
                        year.Quarters = MapQuarters(yearDs.Tables[1]);
                    }
                }
            }

            return years;
        }

        private static void EnsureUniqueYearKeys(List<QuarterlyReportYearModel> years)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var year in years)
            {
                var key = string.IsNullOrWhiteSpace(year.Key) ? "year" : year.Key;
                if (!seen.Add(key))
                {
                    key = key + "-" + year.ContId;
                    seen.Add(key);
                }
                year.Key = key;
            }
        }

        private static List<QuarterlyReportYearModel> MapYears(DataTable table)
        {
            return table.AsEnumerable()
                .Select(row =>
                {
                    var title = FirstNonEmpty(
                        row["cont_hmpg_title"]?.ToString() ?? "",
                        row["cont_title"]?.ToString() ?? "");
                    var contId = row.Field<int>("cont_id");
                    return new QuarterlyReportYearModel
                    {
                        ContId = contId,
                        Title = StripHtml(title),
                        Key = YearKey(title, contId),
                        PageName = row["cont_pagename"]?.ToString()?.Trim() ?? "",
                        Sequence = table.Columns.Contains("cont_sequence") && row["cont_sequence"] != DBNull.Value
                            ? Convert.ToInt32(row["cont_sequence"])
                            : 0,
                        SortYear = ParseSortYear(title)
                    };
                })
                .Where(y => !string.IsNullOrWhiteSpace(y.Title) || y.ContId > 0)
                .ToList();
        }

        private static List<QuarterlyReportYearModel> MapYearsFromArticles(DataTable table)
        {
            return Config_Application_Website.MapArticleList(table)
                .Select(a => new QuarterlyReportYearModel
                {
                    ContId = a.ContId,
                    Title = StripHtml(a.Title),
                    Key = YearKey(a.Title, a.ContId),
                    PageName = a.PageName ?? "",
                    Sequence = a.Sequence,
                    SortYear = ParseSortYear(a.Title)
                })
                .ToList();
        }

        private List<QuarterlyReportItemModel> MapQuarters(DataTable table)
        {
            var grouped = table.AsEnumerable()
                .Where(r => r.Table.Columns.Contains("context_group_id") && r["context_group_id"] != DBNull.Value)
                .GroupBy(r => Convert.ToString(r["context_group_id"]) ?? "")
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .Select(g => new ComponentGroup
                {
                    GroupId = g.Key,
                    Fields = g.Select(row => new ComponentField
                    {
                        GroupId = g.Key,
                        FieldName = GetColumnString(row, "field_name"),
                        FieldKey = GetColumnString(row, "name_key"),
                        FieldValue = GetColumnString(row, "field_value"),
                        ImagePath = FirstNonEmpty(
                            GetColumnString(row, "component_image_path"),
                            GetColumnString(row, "File_path"),
                            GetColumnString(row, "field_media_path")),
                        sequence = SafeInt(row, "component_sequence"),
                        IsBlock = SafeInt(row, "is_block")
                    }).ToList()
                })
                .ToList();

            return grouped
                .Select(MapQuarter)
                .Where(q =>
                    !string.IsNullOrWhiteSpace(q.Title)
                    || !string.IsNullOrWhiteSpace(q.PublishedOn)
                    || HasFile(q))
                .OrderBy(q => q.Sequence)
                .ThenBy(q => q.Title, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static QuarterlyReportItemModel MapQuarter(ComponentGroup group)
        {
            var dict = Config_Application_Website.GetFieldDictionary(group);
            var title = StripHtml(Config_Application_Website.GetValue(
                dict, "Title", "Block Title", "Block title", "Quarter"));
            var published = StripHtml(Config_Application_Website.GetValue(
                dict, "Block date", "Block Date", "Date", "Published on", "Published date"));

            var item = new QuarterlyReportItemModel
            {
                Title = title,
                PublishedOn = published,
                Sequence = Config_Application_Website.GetIntValue(dict, "Sequence"),
                FinancialResultsUrl = GetFile(group, "Financial result pdf", "Financial results pdf", "Financial result", "Financial results"),
                ResultsPresentationUrl = GetFile(group, "Result presentation pdf", "Results presentation pdf", "Result presentataion pdf", "Result presentation", "Results presentation"),
                TranscriptUrl = GetFile(group, "Earning call transcript", "Earnings call transcript", "Transcript"),
                AudioUrl = GetFile(group, "Earning call audio", "Earnings call audio", "Audio"),
                DownloadAllUrl = GetFile(group, "Download all", "Download All", "Zip")
            };

            var match = Regex.Match(title ?? "", @"^(Q\s*\d+)\s*(.*)$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                item.QuarterCode = Regex.Replace(match.Groups[1].Value, @"\s+", "").ToUpperInvariant();
                item.QuarterRange = match.Groups[2].Value.Trim();
            }

            return item;
        }

        private static string GetFile(ComponentGroup group, params string[] names)
        {
            var exact = Config_Application_Website.GetPath(group, names);
            if (!string.IsNullOrWhiteSpace(exact)) return exact;

            foreach (var field in group.Fields ?? new List<ComponentField>())
            {
                var name = field.FieldName ?? "";
                if (!names.Any(n => name.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    continue;
                }

                var path = Config_Application_Website.NormalizeMediaPath(
                    !string.IsNullOrWhiteSpace(field.ImagePath) ? field.ImagePath : field.FieldValue);
                if (!string.IsNullOrWhiteSpace(path)) return path;
            }

            return "";
        }

        private static bool HasFile(QuarterlyReportItemModel item)
        {
            return !string.IsNullOrWhiteSpace(item.FinancialResultsUrl)
                || !string.IsNullOrWhiteSpace(item.ResultsPresentationUrl)
                || !string.IsNullOrWhiteSpace(item.TranscriptUrl)
                || !string.IsNullOrWhiteSpace(item.AudioUrl)
                || !string.IsNullOrWhiteSpace(item.DownloadAllUrl);
        }

        private static string YearKey(string? title, int contId)
        {
            var slug = Regex.Replace(StripHtml(title).ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
            return string.IsNullOrWhiteSpace(slug) ? "year-" + contId : slug;
        }

        private static int ParseSortYear(string? title)
        {
            var match = Regex.Match(title ?? "", @"(20\d{2})");
            return match.Success && int.TryParse(match.Groups[1].Value, out var year) ? year : 0;
        }

        private static int SafeInt(DataRow row, string column)
        {
            var value = GetColumnString(row, column);
            return int.TryParse(value, out var n) ? n : 0;
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
