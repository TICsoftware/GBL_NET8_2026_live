using System;

namespace Core_project_BusinessLogic.Entity
{
     public class CmsMenuTemplate
    {
        public int TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string TemplateHtml { get; set; }

        public string PreviewImage { get; set; }

        public bool IsActive { get; set; }
    }


}