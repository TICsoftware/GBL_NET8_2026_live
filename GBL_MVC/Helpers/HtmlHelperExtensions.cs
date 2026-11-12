using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace GBL_MVC.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string CleanParagraphTags(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            return WebUtility.HtmlDecode(input)
                .Replace("<p>", "")
                .Replace("</p>", "");
        }

        public static string FormatGovernanceCopy(this string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return html ?? string.Empty;

            html = Regex.Replace(
                html,
                @"<p\b[^>]*>(.*?)</p>",
                match =>
                {
                    var inner = match.Groups[1].Value;
                    if (!Regex.IsMatch(inner, @"<br\s*/?>", RegexOptions.IgnoreCase))
                        return match.Value;

                    var parts = Regex.Split(inner, @"<br\s*/?>", RegexOptions.IgnoreCase)
                        .Select(part => part.Trim())
                        .Where(part => PlainText(part).Length > 0)
                        .ToList();

                    if (parts.Count < 3 || parts.Any(part => !PlainText(part).Contains(':')))
                        return match.Value;

                    return ToList(parts);
                },
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            html = GroupStatParagraphs(html);

            return Regex.Replace(
                html,
                @"<ul\b([^>]*)>",
                match =>
                {
                    var attrs = match.Groups[1].Value;
                    if (Regex.IsMatch(attrs, @"\blist-icon\b", RegexOptions.IgnoreCase))
                        return match.Value;

                    var classMatch = Regex.Match(attrs, @"class\s*=\s*([""'])(.*?)\1", RegexOptions.IgnoreCase);
                    if (!classMatch.Success)
                        return "<ul class=\"list-icon\"" + attrs + ">";

                    var quote = classMatch.Groups[1].Value;
                    var classes = ("list-icon " + classMatch.Groups[2].Value).Trim();
                    var replaced = "class=" + quote + classes + quote;
                    return "<ul" + attrs.Replace(classMatch.Value, replaced) + ">";
                },
                RegexOptions.IgnoreCase);
        }

        private static string GroupStatParagraphs(string html)
        {
            var matches = Regex.Matches(
                html,
                @"<p\b[^>]*>(.*?)</p>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (matches.Count == 0) return html;

            var builder = new StringBuilder();
            var cursor = 0;
            var index = 0;
            while (index < matches.Count)
            {
                if (!IsStatLine(matches[index].Groups[1].Value))
                {
                    index++;
                    continue;
                }

                var start = index;
                while (index < matches.Count && IsStatLine(matches[index].Groups[1].Value))
                    index++;

                if (index - start < 3)
                    continue;

                builder.Append(html, cursor, matches[start].Index - cursor);
                builder.Append("<ul class=\"list-icon\">");
                for (var item = start; item < index; item++)
                    builder.Append("<li>").Append(matches[item].Groups[1].Value.Trim()).Append("</li>");
                builder.Append("</ul>");
                cursor = matches[index - 1].Index + matches[index - 1].Length;
            }

            builder.Append(html, cursor, html.Length - cursor);
            return builder.ToString();
        }

        private static bool IsStatLine(string inner)
        {
            if (Regex.IsMatch(inner, @"<br\s*/?>", RegexOptions.IgnoreCase))
                return false;

            var text = PlainText(inner);
            return text.Length > 0 && text.Length <= 140 && text.Contains(':');
        }

        private static string ToList(IEnumerable<string> items)
        {
            var builder = new StringBuilder("<ul class=\"list-icon\">");
            foreach (var item in items)
                builder.Append("<li>").Append(item).Append("</li>");
            builder.Append("</ul>");
            return builder.ToString();
        }

        private static string PlainText(string fragment)
        {
            var text = Regex.Replace(fragment ?? string.Empty, "<.*?>", string.Empty);
            return WebUtility.HtmlDecode(text).Replace('\u00a0', ' ').Trim();
        }
    }
}