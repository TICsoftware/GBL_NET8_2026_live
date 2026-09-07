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
    public class Page_Manage_BAL : Page_Manage_DAL
    {
        private readonly IConfiguration _configuration;
        public Page_Manage_BAL(IConfiguration configuration) : base(configuration) 
        {
            _configuration = configuration;
        }

        // private readonly Page_Manage_DAL _dal;
        // private readonly IConfiguration _configuration;
        // public Page_Manage_BAL(IConfiguration configuration)
        // {
        //     _dal = new Page_Manage_DAL(configuration);
        //     _configuration = configuration;
        // }

        public PageViewModel GetPageData_BAL(string pagename, int languageId, int geographyId)
        {
            PageViewModel model = new PageViewModel();

            string baseurl = _configuration["AppSettings:BaseUrl"]?.TrimEnd('/') ?? "";

            DataSet ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            // 🔹 1st Table → Content
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                var Image = row.Field<string>("masthead_image_path") ?? "";
                var canUrl = row.Field<string>("pageurl") ?? "";

                model.Content = new ContentViewModel
                {
                    ContId = Convert.ToInt32(row["cont_id"]),
                    ContTitle = Convert.ToString(row["cont_title"]),
                    Cont_intro = Convert.ToString(row["cont_intro"]),
                    Cont_hmpg_intro = Convert.ToString(row["cont_hmpg_intro"]),
                    Content = Convert.ToString(row["content"]),
                    PageName = Convert.ToString(row["cont_pagename"]),
                    MastheadImage = Convert.ToString(row["masthead_image_path"]),
                    MobileMastheadImage = Convert.ToString(row["mobile_masthead_image_path"]),
                    Template_Master_ID = Convert.ToInt32(row["Template_Master_ID"]),
                    BreadcrumPath = Convert.ToString(row["BreadcrumPath"]) ?? string.Empty,
                    cont_window_title = row.Field<string>("cont_window_title") ?? "",
                    cont_metadesc = row.Field<string>("cont_metadesc") ?? "",
                    cont_metatag = row.Field<string>("cont_metatag") ?? "",
                    page_schema =Convert.ToString(row["page_schema"]) ?? "",
                    Hmpg_thumbnail = row.Field<string>("Hmpg_thumbnail") ?? "",
                    Hmpg_thumbnail_alt_text = row.Field<string>("Hmpg_thumbnail_alt_text") ?? "",
                    CanonicalUrl = Config_Application_Website.GetMetaUrl(baseurl, canUrl),
                    cont_meta_image = Config_Application_Website.GetMetaUrl(baseurl, Image)
                };


            }
               // Components
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;


              
               
             
            }
            // if (model.Content.Template_Master_ID == 1)
            // {
            //     // 🔹 2nd Table → Components
            //     if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            //     {
            //         var componentRows = ds.Tables[1].AsEnumerable();

            //         model.Components = componentRows
            //             .GroupBy(x => x["context_group_id"].ToString())
            //             .Select(g => new ComponentGroup
            //             {
            //                 GroupId = g.Key,
            //                 Fields = g.Select(x => new ComponentField
            //                 {
            //                     GroupId = x["context_group_id"].ToString(),
            //                     FieldName = x["field_name"].ToString(),
            //                     FieldKey = x["name_key"].ToString(),
            //                     FieldValue = x["field_value"].ToString(),
            //                     ImagePath = x["component_image_path"].ToString(),
            //                     sequence = Convert.ToInt32(x["component_sequence"].ToString())
            //                 }).ToList()
            //             }).ToList();
            //     }
            // }


            return model;
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

       private List<ContentCommonModel> MapComponentCommon(List<ComponentGroup> data, int sequence)
        {
            return Config_Application_Website.MapComponent(data, sequence, (group, dict) => new ContentCommonModel
            {
                GroupId = group.GroupId,
                Title = Config_Application_Website.GetValue(dict, "Title", "Component Title"),
                Intro = Config_Application_Website.GetValue(dict, "Intro", "Component Intro"),
                HmpgIntro = Config_Application_Website.GetValue(dict, "Landing intro", "Component Landing intro"),
                DisplayTitle = Config_Application_Website.GetValue(dict, "Component Display title"),
                BlockDisplayTitle = Config_Application_Website.GetValue(dict, "Display title"),
                Content = Config_Application_Website.GetValue(dict, "Content", "Component Content"),
                ComponentThumbnail = Config_Application_Website.GetPath(group, "Component Thumbnail image"),
                ComponentThumbnailAltText = Config_Application_Website.GetValue(dict, "Component thumbnail image alt"),
                ThumbnailImage = Config_Application_Website.GetPath(group, "Thumbnail Image"),
                ThumbnailAltText = Config_Application_Website.GetValue(dict, "thumbnail image alt"),
                Url = Config_Application_Website.GetValue(dict, "Url", "Component URL"),
                Url_Text = Config_Application_Website.GetValue(dict, "Url text", "Component URL text"),
                //Url = Config_Application_Website.GetValue(dict, "Url"),
                //Url_Text = Config_Application_Website.GetValue(dict, "Url text"),
                Video_path = Config_Application_Website.GetPath(group, "Video"),
                Video_poster = Config_Application_Website.GetPath(group, "Video poster"),
                Icon_Image = Config_Application_Website.GetPath(group, "Icon image"),
                Popup_Content = Config_Application_Website.GetValue(dict, "popup content"),
                Popup_Display_Title = Config_Application_Website.GetValue(dict, "Popup Display title"),
                Sequence = Config_Application_Website.GetIntValue(dict, "Sequence"),
                //Sequence = group.Fields.First().sequence,
                IsBlock = group.Fields.First().IsBlock,
                Section_title = Config_Application_Website.GetValue(dict, "Block Title", "Section title"),
                File_path = Config_Application_Website.GetPath(group, "File Upload"),
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