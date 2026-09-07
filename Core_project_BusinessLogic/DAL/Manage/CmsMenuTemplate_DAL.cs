

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
public class CmsMenuTemplate_DAL : DBHelper
{
     public CmsMenuTemplate_DAL(IConfiguration config) : base(config) { }
public CmsMenuTemplate GetTemplateById(int id)
{
    SqlParameter[] p =
    {
        new SqlParameter("@TemplateId", id)
    };

    DataTable dt =
        GetDataSet("GetCmsMenuTemplateById", p).Tables[0];

    if (dt.Rows.Count == 0)
        return null;

    DataRow r = dt.Rows[0];

    return new CmsMenuTemplate
    {
        TemplateId = Convert.ToInt32(r["TemplateId"]),
        TemplateName = r["TemplateName"].ToString(),
        TemplateHtml = r["TemplateHtml"].ToString(),
        PreviewImage = r["PreviewImage"].ToString(),
        IsActive = Convert.ToBoolean(r["IsActive"])
    };
}


public void SaveTemplate(CmsMenuTemplate model)
{
    SqlParameter[] p =
    {
        new SqlParameter("@TemplateId", model.TemplateId),
        new SqlParameter("@TemplateName", model.TemplateName ?? ""),
        new SqlParameter("@TemplateHtml", model.TemplateHtml ?? ""),
        new SqlParameter("@PreviewImage",
            (object?)model.PreviewImage ?? DBNull.Value),
        new SqlParameter("@IsActive", model.IsActive)
    };

    ExecuteNonQuery("SaveCmsMenuTemplate", p);
}
public List<CmsMenuTemplate> GetTemplates()
{
    SqlParameter[] p = { };

    DataTable dt =
        GetDataSet("GetCmsMenuTemplates", p).Tables[0];

    List<CmsMenuTemplate> list = new();

    foreach (DataRow r in dt.Rows)
    {
        list.Add(new CmsMenuTemplate
        {
            TemplateId = Convert.ToInt32(r["TemplateId"]),
            TemplateName = r["TemplateName"].ToString(),
            TemplateHtml = r["TemplateHtml"].ToString(),
            PreviewImage = r["PreviewImage"].ToString(),
            IsActive = Convert.ToBoolean(r["IsActive"])
        });
    }

    return list;
}
}
}