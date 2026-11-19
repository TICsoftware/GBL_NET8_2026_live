using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class AboutUs_BAL : BasePageBAL
    {
        public AboutUs_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public AboutModel GetLegacyPage_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new AboutModel();
            var pageName = string.IsNullOrWhiteSpace(pagename) ? "our-legacy" : pagename.Trim();
            var ds = GetLegacyPageData_DAL(pageName, languageId, geographyId);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                // component_sequence 1 = Founding Family, 2 = Milestones
                model.Founding_Family_List = MapComponents(groupedData, 1);
                model.Milestones_List = MapComponents(groupedData, 2);
            }

            return model;
        }

        public AboutModel GetLeadershipPage_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new AboutModel();
            var pageName = string.IsNullOrWhiteSpace(pagename) ? "leadership" : pagename.Trim();
            var ds = GetLeadershipPageData_DAL(pageName, languageId, geographyId);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                // Preferred: seq 1 = featured Board of Directors, seq 2 = BOD leadership list
                model.Board_Of_Directors_List = MapComponents(groupedData, 1);
                model.Leadership_List = MapComponents(groupedData, 2);

                // CMS may store BOD list under the same component_sequence as the chair (both = 1).
                // Split: keep featured chair on board; move remaining blocks to leadership list.
                NormalizeLeadershipLists(model);
            }

            return model;
        }

        private static void NormalizeLeadershipLists(AboutModel model)
        {
            var hasLeadershipBlocks = model.Leadership_List?.Any(x => x.IsBlock == 1) == true;
            if (hasLeadershipBlocks)
                return;

            var seq1Blocks = (model.Board_Of_Directors_List ?? new System.Collections.Generic.List<ComponentModel>())
                .Where(x => x.IsBlock == 1)
                .OrderBy(x => x.Sequence)
                .ThenBy(x => x.Title)
                .ToList();

            if (seq1Blocks.Count <= 1)
                return;

            // Featured chair has Block section Title / Intro / Content; grid cards usually do not.
            var chair = seq1Blocks.FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x.Section_title)
                    || !string.IsNullOrWhiteSpace(x.Intro)
                    || !string.IsNullOrWhiteSpace(x.Content))
                ?? seq1Blocks.First();

            var sectionRows = (model.Board_Of_Directors_List ?? new System.Collections.Generic.List<ComponentModel>())
                .Where(x => x.IsBlock == 0)
                .ToList();

            model.Board_Of_Directors_List = sectionRows
                .Concat(seq1Blocks.Where(x => x.GroupId == chair.GroupId))
                .ToList();

            model.Leadership_List = seq1Blocks
                .Where(x => x.GroupId != chair.GroupId)
                .OrderBy(x => x.Sequence)
                .ThenBy(x => x.Title)
                .ToList();
        }

        public AboutModel GetVisionPage_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new AboutModel();
            DataSet? ds = null;

            foreach (var name in GetVisionPageNames(pagename))
            {
                ds = GetVisionPageData_DAL(name, languageId, geographyId);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    model.Content = MapContent(ds.Tables[0].Rows[0]);
                    if ((model.Content?.ContId ?? 0) > 0)
                        break;
                }
            }

            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                // Preferred CMS order on /about-us/our-vision:
                //   1 = Our Guiding Principles
                //   2 = Our Mission
                // Also resolve by component title in case sequences differ or mission
                // was stored under the guiding-principles component.
                AssignVisionComponents(model, groupedData);
            }

            return model;
        }

        private static IEnumerable<string> GetVisionPageNames(string? pagename)
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
            Add("our-vision");
            Add("about-us/our-vision");
            Add("vision-mission");
            Add("our-vision-mission");
            if (!string.IsNullOrWhiteSpace(last))
                Add("about-us/" + last);

            return names;
        }

        private void AssignVisionComponents(AboutModel model, List<ComponentGroup> groupedData)
        {
            var bySequence = MapComponentsBySequence(groupedData);

            model.Guiding_Principles_List = FindVisionComponent(
                bySequence,
                preferredSequence: 1,
                titleNeedles: new[] { "guiding principle", "guiding principles", "our guiding" },
                matchedSequence: out var guidingSeq);

            model.Our_Mission_List = FindVisionComponent(
                bySequence,
                preferredSequence: 2,
                titleNeedles: new[] { "our mission", "mission" },
                matchedSequence: out _,
                excludeSequences: guidingSeq > 0 ? new List<int> { guidingSeq } : null);

            // If mission is still empty, CMS may have put mission lines under Guiding Principles.
            NormalizeVisionLists(model);
        }

        private static List<ComponentModel> FindVisionComponent(
            Dictionary<int, List<ComponentModel>> bySequence,
            int preferredSequence,
            string[] titleNeedles,
            out int matchedSequence,
            List<int>? excludeSequences = null)
        {
            matchedSequence = 0;
            if (bySequence == null || bySequence.Count == 0)
                return new List<ComponentModel>();

            excludeSequences ??= new List<int>();

            foreach (var seq in bySequence.Keys.OrderBy(s => s))
            {
                if (excludeSequences.Contains(seq)) continue;
                var list = bySequence[seq];
                var sectionTitle = GetVisionSectionTitle(list);
                if (TitleMatchesAny(sectionTitle, titleNeedles))
                {
                    matchedSequence = seq;
                    return list;
                }
            }

            if (bySequence.TryGetValue(preferredSequence, out var preferred)
                && !excludeSequences.Contains(preferredSequence))
            {
                matchedSequence = preferredSequence;
                return preferred;
            }

            return new List<ComponentModel>();
        }

        private static string GetVisionSectionTitle(List<ComponentModel> list)
        {
            var section = list?.FirstOrDefault(x => x.IsBlock == 0)
                ?? list?.FirstOrDefault();
            if (section == null) return "";

            return FirstNonEmpty(
                section.Title,
                section.DisplayTitle,
                section.BlockDisplayTitle,
                section.Section_title);
        }

        private static bool TitleMatchesAny(string? title, string[] needles)
        {
            var hay = (title ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(hay)) return false;
            // Strip simple HTML so "Our Mission" still matches.
            hay = System.Text.RegularExpressions.Regex.Replace(hay, "<[^>]+>", " ");
            hay = System.Text.RegularExpressions.Regex.Replace(hay, @"\s+", " ").Trim();

            return needles.Any(n => hay.Contains(n, StringComparison.OrdinalIgnoreCase));
        }

        private static void NormalizeVisionLists(AboutModel model)
        {
            var hasMissionItems = model.Our_Mission_List?.Any(IsVisionMissionItem) == true
                || model.Our_Mission_List?.Any(x => x.IsBlock == 1) == true;
            if (hasMissionItems)
                return;

            var guiding = model.Guiding_Principles_List ?? new List<ComponentModel>();
            if (guiding.Count == 0) return;

            var sectionRows = guiding.Where(x => x.IsBlock == 0).ToList();
            var blocks = guiding.Where(x => x.IsBlock != 0).ToList();
            if (blocks.Count == 0)
                blocks = guiding.Where(x => !sectionRows.Contains(x)).ToList();

            var principleBlocks = blocks.Where(x => !IsVisionMissionItem(x)).ToList();
            var missionBlocks = blocks.Where(IsVisionMissionItem).ToList();

            // Heuristic only when mission-like lines are clearly mixed into guiding principles.
            if (missionBlocks.Count == 0 || principleBlocks.Count == 0)
                return;

            model.Guiding_Principles_List = sectionRows
                .Where(x => !TitleMatchesAny(GetVisionSectionTitle(new List<ComponentModel> { x }), new[] { "mission" }))
                .Concat(principleBlocks)
                .ToList();

            var missionSection = sectionRows
                .FirstOrDefault(x => TitleMatchesAny(GetVisionSectionTitle(new List<ComponentModel> { x }), new[] { "mission" }));

            model.Our_Mission_List = (missionSection != null
                    ? new List<ComponentModel> { missionSection }
                    : new List<ComponentModel>())
                .Concat(missionBlocks)
                .ToList();
        }

        private static bool IsVisionMissionItem(ComponentModel item)
        {
            if (item == null) return false;

            var title = FirstNonEmpty(item.Title, item.DisplayTitle, item.BlockDisplayTitle).Trim();
            title = System.Text.RegularExpressions.Regex.Replace(title ?? "", "<[^>]+>", " ").Trim();
            if (string.IsNullOrWhiteSpace(title)) return false;

            // Mission CMS rows are long statements, typically starting with "To ".
            if (title.StartsWith("To ", StringComparison.OrdinalIgnoreCase))
                return true;

            var hasIcon = !string.IsNullOrWhiteSpace(item.ThumbnailImage)
                || !string.IsNullOrWhiteSpace(item.ComponentThumbnail)
                || !string.IsNullOrWhiteSpace(item.Icon_Image)
                || !string.IsNullOrWhiteSpace(item.Component_Icon_Image);

            // Long text with no icon is mission-like; short titled cards with icons are principles.
            return !hasIcon && title.Length >= 60;
        }

        public AboutModel GetAboutUs_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new AboutModel();
            var pageName = string.IsNullOrWhiteSpace(pagename) ? "about-us" : pagename.Trim();
            var ds = GetContentComponentData_DAL(pageName, languageId, geographyId);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                model.Four_Decades_Of_Building_List = MapComponents(groupedData, 1);
                model.Infographic_List = MapComponents(groupedData, 2);
                model.What_We_Stand_For_List = MapComponents(groupedData, 3);
                model.A_Word_From_Our_Leadership_List = MapComponents(groupedData, 4);
                model.Values_That_Define_How_We_Work_List = MapComponents(groupedData, 5);
                model.Built_On_Trust_Proven_At_Scale_List = MapComponents(groupedData, 6);
                model.Legacy_Built_One_Year_At_A_Time_List = MapComponents(groupedData, 7);
                model.Ready_To_Build_With_Priya_Cement_List = MapComponents(groupedData, 8);
            }

            return model;
        }
    }
}
