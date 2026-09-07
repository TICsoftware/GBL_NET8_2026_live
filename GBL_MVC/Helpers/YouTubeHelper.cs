using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace GBL_MVC.Helpers
{
    public class YouTubeHelper
    {
        public static string GetThumbnail(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            string videoId = ExtractVideoId(url);

            if (string.IsNullOrWhiteSpace(videoId))
                return string.Empty;

            return $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
        }

        private static string ExtractVideoId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            try
            {
                // Standard YouTube URL
                if (url.Contains("youtube.com"))
                {
                    var uri = new Uri(url);
                    var query = HttpUtility.ParseQueryString(uri.Query);

                    return query["v"] ?? string.Empty;
                }

                // Short URL (youtu.be)
                var match = Regex.Match(url, @"youtu\.be\/([^?&]+)");
                return match.Success ? match.Groups[1].Value : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}