using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class AwardsCertifications_BAL : BasePageBAL
    {
        public AwardsCertifications_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public AwardsCertificationsModel GetAwardsCertifications_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new AwardsCertificationsModel();
            DataSet? ds = null;

            foreach (var name in GetPageNames(pagename))
            {
                try
                {
                    ds = Get_Awards_Certifications_Page_DAL(name, languageId, geographyId);
                }
                catch
                {
                    continue;
                }

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    break;
                }
            }

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                // Fallback: root page via generic content SP, empty sections
                foreach (var name in GetPageNames(pagename))
                {
                    try
                    {
                        var contentDs = GetContentComponentData_DAL(name, languageId, geographyId);
                        if (contentDs != null && contentDs.Tables.Count > 0 && contentDs.Tables[0].Rows.Count > 0)
                        {
                            model.Content = MapContent(contentDs.Tables[0].Rows[0]);
                            break;
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                }

                return model;
            }

            model.Content = MapContent(ds.Tables[0].Rows[0]);

            var sections = new List<AwardsSectionModel>();
            if (ds.Tables.Count > 1)
            {
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    var title = FirstNonEmpty(
                        GetColumnString(row, "cont_hmpg_title"),
                        GetColumnString(row, "cont_title"));
                    var page = GetColumnString(row, "cont_pagename");
                    sections.Add(new AwardsSectionModel
                    {
                        ContId = Convert.ToInt32(row["cont_id"]),
                        Title = title,
                        Intro = FirstNonEmpty(
                            GetColumnString(row, "cont_intro"),
                            GetColumnString(row, "cont_hmpg_intro")),
                        PageName = page,
                        Sequence = ToInt(row, "cont_sequence"),
                        IsCertifications = IsCertificationsSection(title, page)
                    });
                }
            }

            var groups = new List<AwardsGroupModel>();
            if (ds.Tables.Count > 2)
            {
                foreach (DataRow row in ds.Tables[2].Rows)
                {
                    groups.Add(new AwardsGroupModel
                    {
                        ContId = Convert.ToInt32(row["cont_id"]),
                        ParentId = Convert.ToInt32(row["cont_parent_id"]),
                        Title = FirstNonEmpty(
                            GetColumnString(row, "cont_hmpg_title"),
                            GetColumnString(row, "cont_title")),
                        Intro = FirstNonEmpty(
                            GetColumnString(row, "cont_intro"),
                            GetColumnString(row, "cont_hmpg_intro")),
                        PageName = GetColumnString(row, "cont_pagename"),
                        Sequence = ToInt(row, "cont_sequence")
                    });
                }
            }

            var componentsByCont = new Dictionary<int, List<ComponentModel>>();
            if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
            {
                var grouped = GetGroupedComponents(ds.Tables[3]);
                foreach (var contId in grouped.Select(g => g.cont_id).Distinct())
                {
                    var forCont = grouped.Where(g => g.cont_id == contId).ToList();
                    componentsByCont[contId] = MapAllComponents(forCont);
                }
            }

            foreach (var group in groups)
            {
                if (componentsByCont.TryGetValue(group.ContId, out var items))
                {
                    group.Items = items
                        .OrderBy(x => x.Sequence <= 0 ? int.MaxValue : x.Sequence)
                        .ThenBy(x => x.Title)
                        .ToList();
                }
            }

            foreach (var section in sections.OrderBy(s => s.Sequence).ThenBy(s => s.ContId))
            {
                section.Groups = groups
                    .Where(g => g.ParentId == section.ContId)
                    .OrderBy(g => g.Sequence)
                    .ThenBy(g => g.ContId)
                    .ToList();
                model.Sections.Add(section);
            }

            return model;
        }

        private List<ComponentModel> MapAllComponents(List<ComponentGroup> groupedData)
        {
            var list = new List<ComponentModel>();
            if (groupedData == null || groupedData.Count == 0) return list;

            foreach (var seq in groupedData.Select(g => g.Sequence).Distinct().OrderBy(s => s))
            {
                foreach (var item in MapComponents(groupedData, seq))
                {
                    if (item.Sequence <= 0)
                        item.Sequence = seq;
                    list.Add(item);
                }
            }

            return list;
        }

        private static bool IsCertificationsSection(string title, string pageName)
        {
            var key = ((title ?? "") + " " + (pageName ?? "")).ToLowerInvariant();
            return key.Contains("certification");
        }

        private static IEnumerable<string> GetPageNames(string? pagename)
        {
            var requested = (pagename ?? "").Trim().TrimStart('/');
            var names = new List<string>();

            void Add(string? value)
            {
                var name = (value ?? "").Trim().TrimStart('/');
                if (string.IsNullOrWhiteSpace(name)) return;
                if (!names.Exists(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    names.Add(name);
            }

            Add(requested);
            var last = requested.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "";
            Add(last);
            if (!string.IsNullOrWhiteSpace(last))
            {
                Add("about-us/" + last);
                Add("sustainability/" + last);
            }
            Add("awards");
            Add("awards-and-certifications");
            Add("awards-certifications");
            Add("about-us/awards-and-certifications");
            Add("sustainability/awards");
            Add("sustainability/awards-and-certifications");
            return names;
        }

        private static int ToInt(DataRow row, string column)
        {
            if (row?.Table == null || !row.Table.Columns.Contains(column) || row[column] == DBNull.Value)
                return 0;
            return Convert.ToInt32(row[column]);
        }
    }
}
