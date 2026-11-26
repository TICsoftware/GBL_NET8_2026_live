using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.DAL;
using GBL_BusinessLogic.Entity;
using GBL_BusinessLogic;

namespace GBL_BusinessLogic.BAL
{
    public class Homepage_BAL : Homepage_DAL
    {
        private readonly IConfiguration _configuration;
        public Homepage_BAL(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        // private readonly Homepage_DAL _dal;
        // private readonly IConfiguration _configuration;
        // public Homepage_BAL(IConfiguration configuration)
        // {
        //     _dal = new Homepage_DAL(configuration);
        //     _configuration = configuration;
        // }

        public HomePageModel GetHomepage_BAL(int languageId, int geographyId)
        {
            var model = new HomePageModel();
            var ds = GetHomepage_DAL(languageId, geographyId);

            // Content
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Home_Content = MapContent(ds.Tables[0].Rows[0]);
            }

            // Components
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Home_Components = groupedData;

                model.Banners = MapComponentCommon(groupedData, 1);
                model.Belief_List = MapComponentCommon(groupedData, 2);
                model.Philosophy_List = MapComponentCommon(groupedData, 3);
                model.TranslatingProducts_List = MapComponentCommon(groupedData, 4);
                model.AtAGlance_List = MapComponentCommon(groupedData, 5);
                model.SoilToTable_List = MapComponentCommon(groupedData, 6);
                model.GiveBack_List = MapComponentCommon(groupedData, 7);
                model.HealthierTomorrow_List = MapComponentCommon(groupedData, 8);
                model.Performance_List = MapComponentCommon(groupedData, 9);
                model.Latest_List = MapComponentCommon(groupedData, 10);
                model.GlobalStandards_List = MapComponentCommon(groupedData, 12);
            }

            BindInvestorHighlights(model, languageId, geographyId);
            BindStories(model, languageId, geographyId);

            return model;
        }

        /// <summary>
        /// Our Performance second box uses the same latest quarter as /investors.
        /// The third box uses the latest annual report.
        /// Latest at GBL uses the newest press release and newest media coverage.
        /// </summary>
        private void BindInvestorHighlights(HomePageModel model, int languageId, int geographyId)
        {
            try
            {
                using var investors = new InvestorRelations_BAL(_configuration);
                var data = investors.GetInvestorRelations_BAL("investors", languageId, geographyId);
                model.LatestQuarter = data.LatestQuarter;
                model.LatestAnnualReport = data.LatestAnnualReport;
                model.LatestPress = data.WhatsNewPress.FirstOrDefault();
                model.LatestMedia = data.WhatsNewMedia.FirstOrDefault();
            }
            catch
            {
                // Homepage still renders when investor or media lists are unavailable.
            }

            if (model.LatestAnnualReport == null || string.IsNullOrWhiteSpace(model.LatestAnnualReport.Title))
            {
                try
                {
                    using var annualBal = new Investors_BAL(_configuration);
                    foreach (var name in new[] { "annual-report", "annual-reports" })
                    {
                        var page = annualBal.GetAnnualReports_BAL(name, languageId, geographyId, 1);
                        var top = page.SectionArticles_List?
                            .FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.Title));
                        if (top == null) continue;
                        model.LatestAnnualReport = top;
                        break;
                    }
                }
                catch
                {
                    // Annual report list is optional on the homepage.
                }
            }
        }

        private static readonly string[] StoryPageNames =
        {
            "stories-of-change",
            "stories",
            "our-stories",
            "story-of-change",
            "Stories-of-Change"
        };

        private void BindStories(HomePageModel model, int languageId, int geographyId)
        {
            try
            {
                using var pages = new Page_Manage_BAL(_configuration);
                using var media = new Media_BAL(_configuration);

                foreach (var name in StoryPageNames)
                {
                    DataSet ds;
                    try
                    {
                        ds = pages.GetContentComponentData_DAL(name, languageId, geographyId);
                    }
                    catch
                    {
                        continue;
                    }

                    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                        continue;

                    var parent = ds.Tables[0].Rows[0];
                    var parentId = parent.Table.Columns.Contains("cont_id") && parent["cont_id"] != DBNull.Value
                        ? Convert.ToInt32(parent["cont_id"])
                        : 0;
                    if (parentId <= 0)
                        continue;

                    var title = parent.Table.Columns.Contains("cont_title")
                        ? parent["cont_title"]?.ToString()
                        : "";
                    if (!string.IsNullOrWhiteSpace(title))
                        model.StoriesTitle = title;

                    var stories = LoadStoryArticles(pages, media, parentId, languageId);
                    if (stories.Count == 0)
                        continue;

                    model.Stories_List = stories;
                    return;
                }
            }
            catch
            {
                // Stories stay empty when the content parent cannot be resolved.
            }
        }

        private static List<ArticleModel> LoadStoryArticles(
            Page_Manage_BAL pages,
            Media_BAL media,
            int parentId,
            int languageId)
        {
            try
            {
                var listed = media.GetPressReleases_page_wise_BAL(parentId, 1, 12);
                if (listed.SectionArticles_List.Count > 0)
                    return OrderStories(listed.SectionArticles_List);
            }
            catch
            {
                // Listing SP may not apply to this content parent.
            }

            try
            {
                var sub = pages.GetSubSections_AllComponents_DAL(parentId, languageId);
                if (sub != null && sub.Tables.Count > 0 && sub.Tables[0].Rows.Count > 0)
                {
                    var articles = Config_Application_Website.MapArticleList(sub.Tables[0]);
                    if (articles.Count > 0)
                        return OrderStories(articles);
                }
            }
            catch
            {
                // Subsection SP may not be deployed for this parent.
            }

            return new List<ArticleModel>();
        }

        private static List<ArticleModel> OrderStories(List<ArticleModel> articles)
        {
            return articles
                .Where(a => a.ContId > 0 && !string.IsNullOrWhiteSpace(a.Title))
                .OrderBy(a => a.Sequence <= 0 ? int.MaxValue : a.Sequence)
                .ThenBy(a => a.ContId)
                .ToList();
        }


        private ContentViewModel MapContent(DataRow row)
        {
            string baseurl = _configuration["AppSettings:BaseUrl"]?.TrimEnd('/') ?? "";
            var Image = row.Field<string>("masthead_image_path") ?? "";
            var canUrl = row.Field<string>("pageurl") ?? "";

            return new ContentViewModel
            {
                ContId = row.Field<int?>("cont_id") ?? 0,
                ContTitle = row.Field<string>("cont_title") ?? "",
                Cont_intro = row.Field<string>("cont_intro") ?? "",
                Cont_hmpg_intro = row.Field<string>("cont_hmpg_intro") ?? "",
                PageName = row.Field<string>("cont_pagename") ?? "",
                MastheadImage = row.Field<string>("masthead_image_path") ?? "",
                MobileMastheadImage = row.Field<string>("mobile_masthead_image_path") ?? "",
                Template_Master_ID = row.Field<int?>("Template_Master_ID") ?? 0,
                BreadcrumPath = row.Field<string>("BreadcrumPath") ?? "",
                cont_window_title = row.Field<string>("cont_window_title") ?? "",
                cont_metadesc = row.Field<string>("cont_metadesc") ?? "",
                cont_metatag = row.Field<string>("cont_metatag") ?? "",
                page_schema = row.Field<string>("page_schema") ?? "",
                CanonicalUrl = Config_Application_Website.GetMetaUrl(baseurl, canUrl),
                cont_meta_image = Config_Application_Website.GetMetaUrl(baseurl, Image)
            };
        }


        private List<ComponentGroup> GetGroupedComponents(DataTable table)
        {
            return table.AsEnumerable()
            .GroupBy(x => new
            {
                GroupId = x.Field<Guid>("context_group_id").ToString(),
                Sequence = Convert.ToInt32(x["component_sequence"]),
                IsBlock = Convert.ToInt32(x["is_block"])
            })
                .Select(group => new ComponentGroup
                {
                    GroupId = group.Key.GroupId,

                    Fields = group.Select(row => new ComponentField
                    {
                        GroupId = group.Key.GroupId,
                        FieldName = row.Field<string>("field_name") ?? "",
                        FieldKey = row.Field<string>("name_key") ?? "",
                        FieldValue = row.Field<string>("field_value") ?? "",
                        ImagePath = row.Field<string>("component_image_path") ?? "",
                        sequence = group.Key.Sequence,
                        IsBlock = group.Key.IsBlock,
                    }).ToList()
                })
                .OrderBy(g => g.Fields.FirstOrDefault()?.sequence ?? 0).ToList();
        }



        private List<HomeCommonModel> MapComponentCommon(List<ComponentGroup> data, int sequence)
        {
            return Config_Application_Website.MapComponent(data, sequence, (group, dict) => new HomeCommonModel
            {
                GroupId = group.GroupId,
                Title = Config_Application_Website.GetValue(dict, "Title", "Component Title"),
                Intro = Config_Application_Website.GetValue(dict, "Intro", "Component Intro"),
                 Landing_Intro = Config_Application_Website.GetValue(dict, "Landing_Intro", "Landing intro"),
                Lhs_Intro = Config_Application_Website.GetValue(dict, "Component LHS intro"),
                Rhs_Intro = Config_Application_Website.GetValue(dict, "Component RHS intro"),
                DisplayTitle = Config_Application_Website.GetValue(dict, "Component Display title"),
                BlockDisplayTitle = Config_Application_Website.GetValue(dict, "Display title"),
                Content = Config_Application_Website.GetValue(dict, "Content", "Component Content"),
                ComponentThumbnail = Config_Application_Website.GetPath(group, "Component Thumbnail image"),
                ComponentThumbnailAltText = Config_Application_Website.GetValue(dict, "Component thumbnail image alt"),
                Component_Background_image = Config_Application_Website.GetPath(group, "Component Background image"),
                Component_background_image_alt = Config_Application_Website.GetValue(dict, "Component background image alt"),
                ThumbnailImage = Config_Application_Website.GetPath(group, "Thumbnail Image"),
                ThumbnailAltText = Config_Application_Website.GetValue(dict, "thumbnail image alt"),
                Url = ResolveComponentUrl(group, dict),
                Url_Text = Config_Application_Website.GetValue(dict, "Url text", "Component URL text"),
                component_Url2 = Config_Application_Website.GetValue(dict, "component url 2"),
                component_Url_Text2 = Config_Application_Website.GetValue(dict, "component url text2"),
                //Url = Config_Application_Website.GetValue(dict, "Url"),
                //Url_Text = Config_Application_Website.GetValue(dict, "Url text"),
                Video_path = Config_Application_Website.GetPath(group, "Video"),
                Video_poster = Config_Application_Website.GetPath(group, "Video poster"),
                 component_Video_path = Config_Application_Website.GetPath(group, "component video"),
                Icon_Image = Config_Application_Website.GetPath(group, "Icon image"),
                Icon_Image2 = Config_Application_Website.GetPath(group, "iconimage2"),
                background_image = Config_Application_Website.GetPath(group, "Background image"),
                Popup_Content = Config_Application_Website.GetValue(dict, "popup content"),
                Popup_Display_Title = Config_Application_Website.GetValue(dict, "Popup Display title"),
                Sequence = Config_Application_Website.GetIntValue(dict, "Sequence"),
                //Sequence = group.Fields.First().sequence,
                IsBlock = group.Fields.First().IsBlock,
                Section_title = Config_Application_Website.GetValue(dict, "Block Title", "Section title"),
                banner_image_webp = Config_Application_Website.GetPath(group, "banner image webp"),
                banner_mobile_image = Config_Application_Website.GetPath(group, "banner mobile image"),
                Component_right_image = Config_Application_Website.GetPath(group, "Component right image"),
                Component_right_image_alt = Config_Application_Website.GetPath(group, "Component Right image alt"),
                component_icon_image = Config_Application_Website.GetPath(group, "component icon image"),
                component_icon_image_alt = Config_Application_Website.GetValue(dict, "component icon image alt"),
                Designation = Config_Application_Website.GetValue(dict, "Designation"),
                Block_Company_Name=Config_Application_Website.GetValue(dict, "Block Company Name"),
                  bg_class = Config_Application_Website.GetValue(dict, "bg class"),
            });
        }

        /// <summary>
        /// Text URL fields win. A media picker stored on the same URL field is used when the value is only a media id.
        /// </summary>
        private static string ResolveComponentUrl(ComponentGroup group, Dictionary<string, ComponentField> dict)
        {
            var text = Config_Application_Website.GetValue(dict, "Url", "Component URL", "Block URL", "Block Url");
            if (IsUsableHref(text))
                return text.Trim();

            var path = Config_Application_Website.GetPath(group, "Url", "Component URL", "Block URL", "Block Url");
            return string.IsNullOrWhiteSpace(path) ? "" : path;
        }

        private static bool IsUsableHref(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var text = value.Trim();
            if (text.Contains('<'))
                return false;

            return !int.TryParse(text, out _);
        }




        public PageViewModel GetContentComponentById_BAL(int contentId, string Group_Id)
        {
            PageViewModel model = new PageViewModel();

            DataSet ds = GetContentComponentById_DAL(contentId, Group_Id);

            // 🔹 1st Table → Components
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var componentRows = ds.Tables[0].AsEnumerable();

                model.Components = componentRows
                    .GroupBy(x => x["context_group_id"].ToString())
                    .Select(g => new ComponentGroup
                    {
                        GroupId = g.Key,
                        Fields = g.Select(x => new ComponentField
                        {
                            GroupId = x["context_group_id"].ToString(),
                            FieldName = x["field_name"].ToString(),
                            FieldKey = x["name_key"].ToString(),
                            FieldValue = x["field_value"].ToString(),
                            ImagePath = x["component_image_path"].ToString()
                        }).ToList()
                    }).ToList();
            }


            return model;
        }




    }
}