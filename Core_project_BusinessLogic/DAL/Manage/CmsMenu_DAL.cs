
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
public class CmsMenu_DAL : DBHelper
{
    public CmsMenu_DAL(IConfiguration config) : base(config) { }

    public Dictionary<int, string> GetTemplates()    
    {
        Dictionary<int, string> dict = new();

        // No parameters needed
        SqlParameter[] p = { };

        DataTable dt =
            GetDataSet("GetActiveMenuTemplates", p).Tables[0];

        foreach (DataRow r in dt.Rows)
        {
            int templateId = Convert.ToInt32(r["TemplateId"]);
            string templateHtml = r["TemplateHtml"].ToString();

            if (!dict.ContainsKey(templateId))
                dict.Add(templateId, templateHtml);
        }

        return dict;
    }
  
public List<CmsMenu> GetMenus()
{
    SqlParameter[] p = { };  // No parameters

    DataTable dt =
        GetDataSet("GetCmsMenus", p).Tables[0];

    List<CmsMenu> list = new();

    foreach (DataRow r in dt.Rows)
    {
        list.Add(new CmsMenu
        {
            MenuId = Convert.ToInt32(r["MenuId"]),

            ParentId = r["ParentId"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(r["ParentId"]),

            TemplateId = r["TemplateId"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(r["TemplateId"]),

            Title = r["Title"].ToString(),
            Url = r["Url"].ToString(),

            HeaderText = r["HeaderText"].ToString(),
            IntroText = r["IntroText"].ToString(),
            ImageUrl = r["ImageUrl"].ToString(),
            ImageLink = r["ImageLink"].ToString(),
            ImageCaption = r["ImageCaption"].ToString(),
            CustomHtml = r["CustomHtml"].ToString(),

            DisplayOrder = Convert.ToInt32(r["DisplayOrder"]),
            IsActive = Convert.ToBoolean(r["IsActive"]),
           //  Is_Active = Convert.ToInt32(r["IsActive"].ToString())
        });
    }

    return list;
}
public void SaveMenuHtml(string name, string html)
{
    SqlParameter[] p =
    {
        new SqlParameter("@MenuName", name),
        new SqlParameter("@HtmlContent", html)
    };

    // Execute non-query SP
    ExecuteNonQuery("SaveMenuHtml", p);
}

public void SaveGeneratedMenuHtml(string menuName, string html)
{
    SqlParameter[] p =
    {
        new SqlParameter("@MenuName", menuName),
        new SqlParameter("@HtmlContent", html)
    };

    ExecuteNonQuery("SaveGeneratedMenuHtml", p);
}


public string GetGeneratedMenuHtml(string menuName)
{
    SqlParameter[] p =
    {
        new SqlParameter("@MenuName", menuName)
    };

    DataTable dt = GetDataSet("GetGeneratedMenuHtml", p).Tables[0];

    if (dt.Rows.Count > 0)
        return dt.Rows[0]["HtmlContent"].ToString();

    return "";
}
    public List<MenuTemplate> GetTemplateList()
{
    SqlParameter[] p = { };

    DataTable dt =
        GetDataSet("GetMenuTemplateList", p).Tables[0];

    List<MenuTemplate> list = new();

    foreach (DataRow r in dt.Rows)
    {
        list.Add(new MenuTemplate
        {
            TemplateId = Convert.ToInt32(r["TemplateId"]),
            TemplateName = r["TemplateName"].ToString()
        });
    }

    return list;
}
public string GetMenuHtml(string name)
{
    SqlParameter[] p =
    {
        new SqlParameter("@MenuName", name)
    };

    DataTable dt =
        GetDataSet("GetMenuHtml", p).Tables[0];

    if (dt.Rows.Count > 0)
        return dt.Rows[0]["HtmlContent"].ToString();

    return "";
}

public CmsMenu GetMenuById(int id)
{
    SqlParameter[] p =
    {
        new SqlParameter("@MenuId", id)
    };

    DataTable dt =
        GetDataSet("GetCmsMenuById", p).Tables[0];

    if (dt.Rows.Count == 0)
        return null;

    DataRow r = dt.Rows[0];

    return new CmsMenu
    {
        MenuId = Convert.ToInt32(r["MenuId"]),

        ParentId = r["ParentId"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(r["ParentId"]),

        TemplateId = r["TemplateId"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(r["TemplateId"]),

        Title = r["Title"].ToString(),
        Url = r["Url"].ToString(),

        HeaderText = r["HeaderText"].ToString(),
        IntroText = r["IntroText"].ToString(),
        ImageUrl = r["ImageUrl"].ToString(),
        ImageLink = r["ImageLink"].ToString(),
        ImageCaption = r["ImageCaption"].ToString(),
        CustomHtml = r["CustomHtml"].ToString(),

        DisplayOrder = Convert.ToInt32(r["DisplayOrder"]),
        IsActive = Convert.ToBoolean(r["IsActive"])
    };
}

public void SaveMenu(CmsMenu model)
{
    SqlParameter[] p =
    {
        new SqlParameter("@MenuId", model.MenuId),

        new SqlParameter("@ParentId",
            (object?)model.ParentId ?? DBNull.Value),

        new SqlParameter("@TemplateId",
            (object?)model.TemplateId ?? DBNull.Value),

        new SqlParameter("@Title", model.Title ?? ""),

        new SqlParameter("@Url",
            (object?)model.Url ?? DBNull.Value),

        new SqlParameter("@HeaderText",
            (object?)model.HeaderText ?? DBNull.Value),

        new SqlParameter("@IntroText",
            (object?)model.IntroText ?? DBNull.Value),

        new SqlParameter("@ImageUrl",
            (object?)model.ImageUrl ?? DBNull.Value),

        new SqlParameter("@ImageLink",
            (object?)model.ImageLink ?? DBNull.Value),

        new SqlParameter("@ImageCaption",
            (object?)model.ImageCaption ?? DBNull.Value),

        new SqlParameter("@CustomHtml",
            (object?)model.CustomHtml ?? DBNull.Value),

        new SqlParameter("@DisplayOrder", model.DisplayOrder),

        new SqlParameter("@IsActive", model.IsActive)
    };

    ExecuteNonQuery("SaveCmsMenu", p);
}
}
}