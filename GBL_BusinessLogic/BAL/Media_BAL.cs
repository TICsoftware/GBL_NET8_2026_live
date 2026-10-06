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
    public class Media_BAL : BasePageBAL
    {
        public Media_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public MediaModel GetPressRelease_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new MediaModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            // Content
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }


            if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                model.SectionArticles_List = Config_Application_Website.MapMediaArticleList(ds.Tables[2]);
            }

            if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
            {
                model.TotalCount = Convert.ToInt32(ds.Tables[3].Rows[0]["TotalCount"]);
            }

            if (ds.Tables.Count > 4 && ds.Tables[4].Rows.Count > 0)
            {
                model.Year_List = MapYearList(ds.Tables[4]);
            }

            if (ds.Tables.Count > 5 && ds.Tables[5].Rows.Count > 0)
            {
                model.Tag_List = MapTagList(ds.Tables[5]);
            }



            return model;
        }


        public MediaModel GetMediaCoverage_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new MediaModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            // Content
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }


            if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                model.SectionArticles_List = Config_Application_Website.MapMediaArticleList(ds.Tables[2]);
            }

            if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
            {
                model.TotalCount = Convert.ToInt32(ds.Tables[3].Rows[0]["TotalCount"]);
            }

            if (ds.Tables.Count > 4 && ds.Tables[4].Rows.Count > 0)
            {
                model.Year_List = MapYearList(ds.Tables[4]);
            }

            if (ds.Tables.Count > 5 && ds.Tables[5].Rows.Count > 0)
            {
                model.Tag_List = MapTagList(ds.Tables[5]);
            }



            return model;
        }


        public MediaModel GetPressRelease_Inside_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new MediaModel();

            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            if (ds == null || ds.Tables.Count == 0)
            {
                return model;
            }

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                model.Related_Articles_List = Config_Application_Website.MapMediaArticleList(ds.Tables[2]);
            }


            return model;
        }


        public MediaModel GetPressReleases_page_wise_BAL(int cont_id, int page, int pageSize, int? year = null, int? month = null, int? tagId = null)
        {
            var model = new MediaModel();

            var ds = Get_PressRelease_page_wise_DAL(cont_id, page, pageSize, year, month, tagId);

            if (ds == null || ds.Tables.Count == 0)
            {
                return model;
            }

            // Article list
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.SectionArticles_List = Config_Application_Website.MapMediaArticleList(ds.Tables[0]);
            }

            // Total record count
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                model.TotalCount = Convert.ToInt32(
                    ds.Tables[1].Rows[0]["TotalCount"]
                );
            }

            return model;
        }


        public static List<DropdownModel> MapTagList(DataTable dt)
        {
            List<DropdownModel> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DropdownModel
                {
                    Text = row["Tag_Name"]?.ToString() ?? "",
                    Value = row["ID"]?.ToString() ?? ""
                });
            }

            return list;
        }

        public static List<DropdownModel> MapYearList(DataTable dt)
        {
            List<DropdownModel> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DropdownModel
                {
                    Text = row["Year"]?.ToString() ?? "",
                    Value = row["Year"]?.ToString() ?? ""
                });
            }

            return list;
        }



    }
}