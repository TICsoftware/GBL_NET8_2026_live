using System;
using System.Text;

namespace Core_project_BusinessLogic.Entity
{
    public class MenuRenderer
{
    public string RenderMenuold(List<CmsMenu> menus)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("<nav id='navMenu'>");
        sb.Append("<ul class='menulist-wrapper'>");

        var parents = menus.Where(x => x.ParentId == null)
                           .OrderBy(x => x.DisplayOrder);

        foreach (var p in parents)
        {
            sb.Append("<li class='menu-link'>");

            sb.Append($"<a href='{p.Url}'>{p.Title}</a>");

            var children = menus.Where(x => x.ParentId == p.MenuId);

            if (children.Any())
            {
                sb.Append("<ul class='dropdown-content'>");

                foreach (var c in children)
                {
                    sb.Append($"<li><a href='{c.Url}'>{c.Title}</a></li>");
                }

                sb.Append("</ul>");
            }

            sb.Append("</li>");
        }

        sb.Append("</ul>");
        sb.Append("</nav>");

        return sb.ToString();
    }



 public string RenderMenu(List<CmsMenu> menus,
                         Dictionary<int,string> templates)
{
    StringBuilder html = new StringBuilder();

    html.Append("<nav id='navMenu'>");
    html.Append("<ul class='menulist-wrapper'>");

    var parents = menus.Where(x => x.ParentId == null)
                       .OrderBy(x => x.DisplayOrder);

    foreach (var m in parents)
    {

        string template = templates.ContainsKey(m.TemplateId ?? 0)
            ? templates[m.TemplateId ?? 0]
            : "<li><a href='{{URL}}'>{{TITLE}}</a>{{CHILD_ITEMS}}</li>";

        string childrenHtml = RenderChildren(m.MenuId, menus);

        template = template.Replace("{{TITLE}}", m.Title ?? "")
                           .Replace("{{URL}}", m.Url ?? "#")
                           .Replace("{{SUBTITLE}}", m.SubTitle ?? "")
                           .Replace("{{INTRO}}", m.IntroText ?? "")
                           .Replace("{{IMAGE_URL}}", m.ImageUrl ?? "")
                           .Replace("{{IMAGE_LINK}}", m.ImageLink ?? "")
                           .Replace("{{CHILD_ITEMS}}", childrenHtml);

        html.Append(template);
    }

    html.Append("</ul>");
    html.Append("</nav>");

    return html.ToString();
}
   private string RenderChildren(int parentId,
                              List<CmsMenu> menus)
{
    StringBuilder sb = new StringBuilder();

    var children = menus.Where(x => x.ParentId == parentId)
                        .OrderBy(x => x.DisplayOrder)
                        .ToList();

    if (!children.Any())
        return "";

    sb.Append("<ul class='dropdown-content'>");

    foreach (var c in children)
    {
        sb.Append("<li>");

        sb.Append($"<a href='{c.Url}'>{c.Title}</a>");

        string sub = RenderChildren(c.MenuId, menus);

        if (!string.IsNullOrEmpty(sub))
            sb.Append(sub);

        sb.Append("</li>");
    }

    sb.Append("</ul>");

    return sb.ToString();
}

}
}