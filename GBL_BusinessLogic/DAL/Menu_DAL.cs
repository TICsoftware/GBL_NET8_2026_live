using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.DAL
{
    public class Menu_DAL : DBHelper
    {
        public Menu_DAL(IConfiguration configuration) : base(configuration) //  call base class constructor 
        {
        }

        public List<MenuHeader> GetFrontendHeaderMenus_dal()
        {
            DataTable dt = GetDataSet("MenuHeader_GetFrontend").Tables[0];

            List<MenuHeader> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new MenuHeader
                {
                    MenuId = r["MenuId"] == DBNull.Value ? 0 : Convert.ToInt32(r["MenuId"]),
                    ParentMenuId = r["ParentMenuId"] == DBNull.Value ? null : Convert.ToInt32(r["ParentMenuId"]),
                    ParentMenuName = r["ParentMenuName"] == DBNull.Value ? null : r["ParentMenuName"].ToString(),
                    MenuCategory = r["MenuCategory"] == DBNull.Value ? 0 : Convert.ToInt32(r["MenuCategory"]),
                    MenuName = r["MenuName"] == DBNull.Value ? null : r["MenuName"].ToString(),
                    Url = r["Url"] == DBNull.Value ? null : r["Url"].ToString(),
                    Target = r["Target"] == DBNull.Value ? "_self" : r["Target"].ToString(),
                    MenuType = r["MenuType"] == DBNull.Value ? "Normal" : r["MenuType"].ToString(),
                    ColumnNo = r["ColumnNo"] == DBNull.Value ? null : Convert.ToInt32(r["ColumnNo"]),
                    Sequence = r["Sequence"] == DBNull.Value ? 0 : Convert.ToInt32(r["Sequence"]),
                    FeatureImageId = r["FeatureImageId"] == DBNull.Value ? 0 : Convert.ToInt32(r["FeatureImageId"]),
                    FeatureImage = r["FeatureImage"] == DBNull.Value ? null : r["FeatureImage"].ToString(),
                    ThumbImageId = r["ThumbImageId"] == DBNull.Value ? 0 : Convert.ToInt32(r["ThumbImageId"]),
                    ThumbImage = r["ThumbImage"] == DBNull.Value ? null : r["ThumbImage"].ToString(),
                    FeatureTitle = r["FeatureTitle"] == DBNull.Value ? null : r["FeatureTitle"].ToString(),
                    FeatureDescription = r["FeatureDescription"] == DBNull.Value ? null : r["FeatureDescription"].ToString(),
                    FeatureButtonText = r["FeatureButtonText"] == DBNull.Value ? null : r["FeatureButtonText"].ToString(),
                    FeatureButtonUrl = r["FeatureButtonUrl"] == DBNull.Value ? null : r["FeatureButtonUrl"].ToString(),
                    FeatureButtonTarget = r["FeatureButtonTarget"] == DBNull.Value ? "_self" : r["FeatureButtonTarget"].ToString(),
                    CssClass = r["CssClass"] == DBNull.Value ? null : r["CssClass"].ToString(),
                    Status = r["Status"] == DBNull.Value ? 0 : Convert.ToInt32(r["Status"])
                });
            }

            return list;
        }



        public List<MenuFooter> GetFrontendFooterMenus_dal()
        {
            DataTable dt = GetDataSet("MenuFooter_GetFrontend").Tables[0];

            List<MenuFooter> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new MenuFooter
                {
                    FooterId = r["FooterId"] == DBNull.Value ? 0 : Convert.ToInt32(r["FooterId"]),
                    ParentFooterId = r["ParentFooterId"] == DBNull.Value ? null : Convert.ToInt32(r["ParentFooterId"]),
                    ParentFooterName = r["ParentFooterName"] == DBNull.Value ? null : r["ParentFooterName"].ToString(),
                    FooterType = r["FooterType"] == DBNull.Value ? null : r["FooterType"].ToString(),
                    Title = r["Title"] == DBNull.Value ? null : r["Title"].ToString(),
                    Description = r["Description"] == DBNull.Value ? null : r["Description"].ToString(),
                    Url = r["Url"] == DBNull.Value ? null : r["Url"].ToString(),
                    Target = r["Target"] == DBNull.Value ? "_self" : r["Target"].ToString(),
                    // Footer Image
                    FooterImageId = r["FooterImageId"] == DBNull.Value ? 0 : Convert.ToInt32(r["FooterImageId"]),
                    FooterImage = r["FooterImage"] == DBNull.Value ? null : r["FooterImage"].ToString(),
                    // Footer Thumbnail / Logo
                    FooterThumbImageId = r["FooterThumbImageId"] == DBNull.Value ? 0 : Convert.ToInt32(r["FooterThumbImageId"]),
                    FooterThumbImage = r["FooterThumbImage"] == DBNull.Value ? null : r["FooterThumbImage"].ToString(),
                    IconClass = r["IconClass"] == DBNull.Value ? null : r["IconClass"].ToString(),
                    ColumnNo = r["ColumnNo"] == DBNull.Value ? null : r["ColumnNo"].ToString(),
                    Sequence = r["Sequence"] == DBNull.Value ? 0 : Convert.ToInt32(r["Sequence"]),
                    Status = r["Status"] == DBNull.Value ? 0 : Convert.ToInt32(r["Status"])
                });
            }

            return list;
        }

    }
}