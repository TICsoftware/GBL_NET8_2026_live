using System;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core_project_BusinessLogic.BAL
{
    public class MenuRenderer_bal
{public string Render(List<CmsMenu> menus,
                         Dictionary<int, string> templates)
    {
        StringBuilder sb = new();

        sb.Append("<nav id='navMenu'><ul class='menulist-wrapper'>");

        foreach (var m in menus)
            sb.Append(RenderItem(m, templates));

        sb.Append("</ul></nav>");

        return sb.ToString();
    }

    private string RenderItem(CmsMenu m,
                              Dictionary<int, string> templates)
    {
        if (!templates.ContainsKey(m.TemplateId ?? 0))
            return "";

        string html = templates[m.TemplateId.Value];

        html = html.Replace("{{TITLE}}", m.Title ?? "")
                   .Replace("{{URL}}", m.Url ?? "#")
                   .Replace("{{HEADER}}", m.HeaderText ?? "")
                   .Replace("{{INTRO}}", m.IntroText ?? "")
                   .Replace("{{IMAGE_URL}}", m.ImageUrl ?? "")
                   .Replace("{{IMAGE_LINK}}", m.ImageLink ?? "#")
                   .Replace("{{IMAGE_CAPTION}}", m.ImageCaption ?? "")
                   .Replace("{{CUSTOM_HTML}}", m.CustomHtml ?? "");

        // CHILD ITEMS
        if (m.Children.Any())
        {
            StringBuilder child = new();

            foreach (var c in m.Children)
                child.Append($"<li><a href='{c.Url}'>{c.Title}</a></li>");

            html = html.Replace("{{CHILD_ITEMS}}", child.ToString())
                       .Replace("{{LEFT_LINKS}}", child.ToString());
        }
        else
        {
            html = html.Replace("{{CHILD_ITEMS}}", "")
                       .Replace("{{LEFT_LINKS}}", "");
        }

        return html;
    }
}
}