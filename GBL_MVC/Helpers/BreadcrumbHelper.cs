using System.Net;
using System.Text;
using GBL_BusinessLogic.Entity;

namespace GBL_MVC.Helpers
{
    /// <summary>
    /// Builds corrected breadcrumbs for /sustainability/* inside pages
    /// (CMS BreadcrumPath often duplicates Sustainability and truncates the leaf title).
    /// </summary>
    public static class BreadcrumbHelper
    {
        private sealed record Crumb(string Label, string? Url);

        /// <summary>
        /// Mid-level pillar + leaf label for known sustainability URLs.
        /// Resource Circularity is under Social per site IA for breadcrumbs.
        /// </summary>
        private static readonly Dictionary<string, (string MidLabel, string MidUrl, string LeafLabel)> SustainabilityPages =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["resource-circularity"] = ("Social", "/sustainability/social", "Resource Circularity"),
                ["circular-products"] = ("Environment", "/sustainability", "Circular Products"),
                ["regenerative-processes"] = ("Environment", "/sustainability", "Regenerative Processes"),
                ["regenerative-process"] = ("Environment", "/sustainability", "Regenerative Processes"),
                ["carbon-light-circularity"] = ("Environment", "/sustainability", "Carbon-Light Circularity"),
                ["health-and-safety"] = ("", "", "Health & Safety"),
                ["governance"] = ("", "", "Governance"),
                ["social"] = ("", "", "Social"),
                ["sustainability-reports"] = ("", "", "Sustainability Reports"),
                ["awards-and-certificates"] = ("", "", "Awards & Certificates"),
                ["awards-certifications"] = ("", "", "Awards & Certificates"),
            };

        public static string ForSustainability(string? requestPath, ContentViewModel? content, string? fallbackHtml)
        {
            var path = (requestPath ?? "").Trim().Trim('/');
            if (!path.StartsWith("sustainability", StringComparison.OrdinalIgnoreCase))
            {
                return fallbackHtml ?? "";
            }

            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                // /sustainability landing
                return BuildOl(
                    new Crumb("Home", "/"),
                    new Crumb("Sustainability", null));
            }

            var slug = parts[^1];
            var pageTitle = FirstNonEmpty(
                content?.ContHmpgTitle,
                content?.ContTitle,
                content?.BreadcrumbTitle,
                Humanize(slug));

            if (SustainabilityPages.TryGetValue(slug, out var known))
            {
                var leaf = FirstNonEmpty(known.LeafLabel, pageTitle);
                if (!string.IsNullOrWhiteSpace(known.MidLabel))
                {
                    return BuildOl(
                        new Crumb("Home", "/"),
                        new Crumb("Sustainability", "/sustainability"),
                        new Crumb(known.MidLabel, known.MidUrl),
                        new Crumb(leaf, null));
                }

                return BuildOl(
                    new Crumb("Home", "/"),
                    new Crumb("Sustainability", "/sustainability"),
                    new Crumb(leaf, null));
            }

            // Generic sustainability child: Home / Sustainability / Page Title
            return BuildOl(
                new Crumb("Home", "/"),
                new Crumb("Sustainability", "/sustainability"),
                new Crumb(pageTitle, null));
        }

        private static string BuildOl(params Crumb[] crumbs)
        {
            var sb = new StringBuilder();
            sb.Append("<ol class=\"flex items-center gap-2 text-sm\">");
            for (var i = 0; i < crumbs.Length; i++)
            {
                if (i > 0)
                    sb.Append("<li class=\"text-white/60\" aria-hidden=\"true\">/</li>");

                var c = crumbs[i];
                var label = WebUtility.HtmlEncode(c.Label);
                if (!string.IsNullOrWhiteSpace(c.Url) && i < crumbs.Length - 1)
                {
                    sb.Append("<li><a href=\"")
                      .Append(WebUtility.HtmlEncode(c.Url))
                      .Append("\" class=\"text-white/70 hover:text-white transition\">")
                      .Append(label)
                      .Append("</a></li>");
                }
                else
                {
                    sb.Append("<li class=\"text-white\">").Append(label).Append("</li>");
                }
            }
            sb.Append("</ol>");
            return sb.ToString();
        }

        private static string Humanize(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return "";
            var words = slug.Replace('-', ' ').Split(' ', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < words.Length; i++)
            {
                var w = words[i];
                if (w.Length == 0) continue;
                words[i] = char.ToUpperInvariant(w[0]) + w[1..];
            }
            return string.Join(' ', words);
        }

        private static string FirstNonEmpty(params string?[] values)
        {
            foreach (var v in values)
            {
                if (!string.IsNullOrWhiteSpace(v))
                {
                    // Strip simple HTML if CMS title has tags
                    var plain = System.Text.RegularExpressions.Regex.Replace(v, "<[^>]+>", " ").Trim();
                    plain = System.Text.RegularExpressions.Regex.Replace(plain, @"\s+", " ").Trim();
                    if (!string.IsNullOrWhiteSpace(plain)) return plain;
                }
            }
            return "";
        }
    }
}
