using GBL_BusinessLogic.Entity;

namespace GBL_MVC.Helpers
{
    /// <summary>
    /// Breadcrumb helper kept for call-site compatibility.
    /// Sustainability uses the same CMS BreadcrumPath as businesses — no page-specific overrides.
    /// </summary>
    public static class BreadcrumbHelper
    {
        public static string ForSustainability(string? requestPath, ContentViewModel? content, string? fallbackHtml)
        {
            if (!string.IsNullOrWhiteSpace(fallbackHtml))
                return fallbackHtml;

            if (!string.IsNullOrWhiteSpace(content?.BreadcrumPath))
                return content.BreadcrumPath;

            return "";
        }
    }
}
