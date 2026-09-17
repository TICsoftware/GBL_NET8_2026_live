using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.DAL;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class BasePageBAL : Page_Manage_DAL
    {
        protected readonly IConfiguration _configuration;

        public BasePageBAL(IConfiguration configuration)
            : base(configuration)
        {
            _configuration = configuration;
        }

        protected ContentViewModel MapContent(DataRow row)
        {
            string baseurl = _configuration["AppSettings:BaseUrl"]?.TrimEnd('/') ?? "";
            var image = row.Field<string>("masthead_image_path") ?? "";
            var canUrl = row.Field<string>("pageurl") ?? "";

            return new ContentViewModel
            {
                ContId = row.Field<int?>("cont_id") ?? 0,
                ContTitle = row.Field<string>("cont_title") ?? "",
                Cont_intro = row.Field<string>("cont_intro") ?? "",
                Cont_hmpg_intro = row.Field<string>("cont_hmpg_intro") ?? "",
                Content = row.Table.Columns.Contains("content")
                    ? row.Field<string>("content") ?? ""
                    : "",
                PageName = row.Field<string>("cont_pagename") ?? "",
                MastheadImage = image,
                MobileMastheadImage = row.Field<string>("mobile_masthead_image_path") ?? "",
                Template_Master_ID = row.Field<int?>("Template_Master_ID") ?? 0,
                BreadcrumPath = row.Field<string>("BreadcrumPath") ?? "",
                cont_window_title = row.Field<string>("cont_window_title") ?? "",
                cont_metadesc = row.Field<string>("cont_metadesc") ?? "",
                cont_metatag = row.Field<string>("cont_metatag") ?? "",
                page_schema = row.Field<string>("page_schema") ?? "",
                Hmpg_thumbnail = row.Field<string>("Hmpg_thumbnail") ?? "",
                Hmpg_thumbnail_alt_text = row.Field<string>("Hmpg_thumbnail_alt_text") ?? "",
                Masthead_image_Alt_text = row.Field<string>("Masthead_alt_text") ?? "",
                CanonicalUrl = Config_Application_Website.GetMetaUrl(baseurl, canUrl),
                ParentLink = row.Field<string>("parentLink") ?? "",
                
                cont_meta_image = Config_Application_Website.GetMetaUrl(baseurl, image),
                cont_displaydate = row.Table.Columns.Contains("cont_displaydate")
                    ? row.Field<DateTime?>("cont_displaydate")
                    : null,
                ByLine = row.Table.Columns.Contains("cont_ByLine")
                    ? row.Field<string>("cont_ByLine") ?? ""
                    : (row.Table.Columns.Contains("ByLine") ? row.Field<string>("ByLine") ?? "" : ""),
                Publication = row.Table.Columns.Contains("Cont_Publication")
                    ? row.Field<string>("Cont_Publication") ?? ""
                    : (row.Table.Columns.Contains("Publication") ? row.Field<string>("Publication") ?? "" : "")
            };
        }

        protected List<ComponentGroup> GetGroupedComponents(DataTable table)
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
                        IsBlock = group.Key.IsBlock
                    }).ToList()

                }).OrderBy(x => x.Fields.First().sequence).ToList();
        }

        protected List<ComponentModel> MapComponents(List<ComponentGroup> data, int sequence)
        {
            return Config_Application_Website.MapComponent(data, sequence, (group, dict) =>
            {
                var thumbnail = Config_Application_Website.GetPath(
                    group,
                    "Component Thumbnail image",
                    "component thumbnail image",
                    "Component thumbnail",
                    "Component Thumbnail",
                    "thumbnail image",
                    "product image",
                    "Product image");

                // Fallback: first image-like media on the component (skips file-upload fields)
                if (string.IsNullOrWhiteSpace(thumbnail))
                {
                    thumbnail = Config_Application_Website.GetFirstImagePath(
                        group,
                        excludeFieldNames: new[]
                        {
                            "component fileupload1", "Component File Upload 1", "component file upload 1",
                            "component fileupload2", "Component File Upload 2", "component file upload 2",
                            "File Upload", "file upload"
                        });
                }

                return new ComponentModel
                {
                    GroupId = group.GroupId,
                    Title = Config_Application_Website.GetValue(dict, "Title", "Component Title"),
                    Intro = Config_Application_Website.GetValue(dict, "Intro", "Component Intro"),
                    HmpgIntro = Config_Application_Website.GetValue(dict, "Landing intro", "Component Landing intro"),
                    DisplayTitle = Config_Application_Website.GetValue(dict, "Component Display title"),
                    BlockDisplayTitle = Config_Application_Website.GetValue(dict, "Display title"),
                    Content = Config_Application_Website.GetValue(dict, "Content", "Component Content"),
                    ComponentThumbnail = thumbnail,
                    ComponentThumbnailAltText = Config_Application_Website.GetValue(dict, "Component thumbnail image alt"),
                    Componentbackground = Config_Application_Website.GetPath(group, "Component Background image"),
                    ComponentbackgroundAltText = Config_Application_Website.GetValue(dict, "Component background image alt"),
                    ThumbnailImage = Config_Application_Website.GetPath(group, "Thumbnail Image"),
                    ThumbnailAltText = Config_Application_Website.GetValue(dict, "thumbnail image alt"),
                    Url = Config_Application_Website.GetValue(dict, "Url", "Component URL"),
                    Url_Text = Config_Application_Website.GetValue(dict, "Url text", "Component URL text"),
                    component_Url2 = Config_Application_Website.GetValue(dict, "component url 2", "Component URL 2"),
                    component_Url_Text2 = Config_Application_Website.GetValue(dict, "component url text2", "Component URL text 2"),
                    Video_path = Config_Application_Website.GetPath(group, "Video"),
                    Video_poster = Config_Application_Website.GetPath(group, "Video poster"),
                    Icon_Image = Config_Application_Website.GetPath(group, "Icon image"),
                    Component_Icon_Image = Config_Application_Website.GetPath(group, "component icon image"),
                    Component_Icon2_Image = Config_Application_Website.GetPath(group, "component icon 2"),
                    Component_Icon3_Image = Config_Application_Website.GetPath(group, "component icon image 3"),
                    Component_Icon_alt_Image = Config_Application_Website.GetPath(group, "component icon image alt"),
                    Popup_Content = Config_Application_Website.GetValue(dict, "popup content"),
                    Popup_Display_Title = Config_Application_Website.GetValue(dict, "Popup Display title"),
                    Sequence = Config_Application_Website.GetIntValue(dict, "Sequence"),
                    IsBlock = group.Fields.First().IsBlock,
                    Section_title = Config_Application_Website.GetValue(dict, "Block section Title", "Block Title", "Section title"),
                    Component_Button_Title1 = Config_Application_Website.GetValue(dict, "component button title 1", "Component Button Title 1"),
                    Component_Button_Title2 = Config_Application_Website.GetValue(dict, "component button title 2", "Component Button Title 2"),
                    Component_right_image = Config_Application_Website.GetPath(group, "Component right image"),
                    Component_Right_image_alt = Config_Application_Website.GetValue(dict, "Component Right image alt"),
                    Component_Designation = Config_Application_Website.GetValue(dict, "component designation"),
                    Component_Designation2 = Config_Application_Website.GetValue(dict, "Component designation 2"),
                    Designation = Config_Application_Website.GetValue(dict, "Designation"),
                    Thumbnail_color_image = Config_Application_Website.GetPath(group, "block thumbnail color image"),
                    MediafilePath = Config_Application_Website.GetPath(group, "File Upload"),
                    Component_FileUpload1 = Config_Application_Website.GetPath(group, "component fileupload1", "Component File Upload 1", "component file upload 1"),
                    Component_FileUpload2 = Config_Application_Website.GetPath(group, "component fileupload2", "Component File Upload 2", "component file upload 2"),
                    FileUploadTitle1 = Config_Application_Website.GetValue(dict, "fileupload title1", "File Upload Title 1", "file upload title 1"),
                    FileUploadTitle2 = Config_Application_Website.GetValue(dict, "fileupload title2", "File Upload Title 2", "file upload title 2"),
                    Component_LHS_thumbnail = Config_Application_Website.GetPath(group, "component LHS thumbnail"),
                    Component_LHS_thumbnail_image_alt = Config_Application_Website.GetPath(group, "Component LHS thumbnail image alt"),
                    bg_class = Config_Application_Website.GetValue(dict, "bg class", "block bg class"),
                    Component_LHS_icon1 = Config_Application_Website.GetPath(group, "LHS component icon image1"),
                    Component_LHS_icon2 = Config_Application_Website.GetPath(group, "LHS component icon image2"),
                    Component_RHS_icon1 = Config_Application_Website.GetPath(group, "RHS component icon image1"),
                    Component_RHS_icon2 = Config_Application_Website.GetPath(group, "RHS component icon image2"),
                    year = Config_Application_Website.GetValue(dict, "Year"),
                    Component_Video_path = Config_Application_Website.GetPath(group, "component video"),
                };
            })
            .OrderBy(x => x.Sequence)
            .ToList();

        }
    }
}
