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
                model.Latest_List = MapComponentCommon(groupedData, 9);
                model.GlobalStandards_List = MapComponentCommon(groupedData, 10);
            }

            // Our team


            return model;
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
                DisplayTitle = Config_Application_Website.GetValue(dict, "Component Display title"),
                BlockDisplayTitle = Config_Application_Website.GetValue(dict, "Display title"),
                Content = Config_Application_Website.GetValue(dict, "Content", "Component Content"),
                ComponentThumbnail = Config_Application_Website.GetPath(group, "Component Thumbnail image"),
                ComponentThumbnailAltText = Config_Application_Website.GetValue(dict, "Component thumbnail image alt"),
                Component_Background_image = Config_Application_Website.GetPath(group, "Component Background image"),
                Component_background_image_alt = Config_Application_Website.GetValue(dict, "Component background image alt"),
                ThumbnailImage = Config_Application_Website.GetPath(group, "Thumbnail Image"),
                ThumbnailAltText = Config_Application_Website.GetValue(dict, "thumbnail image alt"),
                Url = Config_Application_Website.GetValue(dict, "Url", "Component URL"),
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