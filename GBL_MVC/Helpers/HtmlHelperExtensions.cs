using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static string CleanParagraphTags(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            return System.Net.WebUtility.HtmlDecode(input)
                .Replace("<p>", "")
                .Replace("</p>", "");
        }
    }
}