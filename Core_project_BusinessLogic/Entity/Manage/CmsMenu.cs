using System;

namespace Core_project_BusinessLogic.Entity
{
    
public class CmsMenu
{
    public int MenuId { get; set; }
    public int? ParentId { get; set; }
    public int? TemplateId { get; set; }

    public string? Title { get; set; }

     public string? SubTitle { get; set; }
    public string? Url { get; set; }

    public string? HeaderText { get; set; }
    public string? IntroText { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageLink { get; set; }
    public string? ImageCaption { get; set; }
    public string? CustomHtml { get; set; }

    public int? DisplayOrder { get; set; }
      public bool IsActive { get; set; }

       public int? Is_Active { get; set; }

    public List<CmsMenu> Children { get; set; } = new();
}

public class MenuTemplate
{
    public int? TemplateId { get; set; }
    public string? TemplateName { get; set; }
}


}