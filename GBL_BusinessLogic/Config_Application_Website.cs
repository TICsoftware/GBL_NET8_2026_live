using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic
{
    public static class Config_Application_Website
    {
        public static List<T> MapComponent<T>(
           List<ComponentGroup> data,
           int sequence,
           Func<ComponentGroup, Dictionary<string, ComponentField>, T> mapFunc)
        {
            return data
                .Where(g => g.Fields.Any() && g.Fields.First().sequence == sequence)
                .Select(group =>
                {
                    var dict = GetFieldDictionary(group);
                    return mapFunc(group, dict);
                })
                .ToList();
        }

        public static Dictionary<string, ComponentField> GetFieldDictionary(ComponentGroup group)
        {
            return group.Fields
                .GroupBy(x => x.FieldName)
                .ToDictionary(g => g.Key, g => g.First());
        }


        public static string GetPath(ComponentGroup group, string fieldName)
        {
            var field = group.Fields
                .FirstOrDefault(x =>
                    string.Equals(x.FieldName?.Trim(), fieldName.Trim(), StringComparison.OrdinalIgnoreCase)
                );

            if (field == null)
                return "";

            if (!string.IsNullOrWhiteSpace(field.ImagePath))
                return NormalizeMediaPath(field.ImagePath);

            // File uploads / some media fields store the path in FieldValue
            // (including relative paths like "uploads/thumbnail/x.webp" with no leading slash)
            return NormalizeMediaPath(field.FieldValue);
        }

        public static string NormalizeMediaPath(string? path)
        {
            var value = path?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(value) || value.Contains('<'))
                return "";

            var isUrl = value.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("//");
            var looksLikePath = value.StartsWith("/")
                || value.StartsWith("~")
                || value.Contains('/')
                || value.Contains('\\')
                || value.StartsWith("uploads", StringComparison.OrdinalIgnoreCase)
                || System.Text.RegularExpressions.Regex.IsMatch(
                    value,
                    @"\.(webp|png|jpe?g|gif|svg|pdf|docx?|xlsx?|pptx?|zip|rar|csv|txt)(\?|#|$)",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (!isUrl && !looksLikePath)
                return "";

            if (!isUrl && !value.StartsWith("/") && !value.StartsWith("~"))
                value = "/" + value.TrimStart('/', '\\').Replace('\\', '/');

            return value;
        }

        public static string GetPath(ComponentGroup group, params string[] fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                var path = GetPath(group, fieldName);
                if (!string.IsNullOrWhiteSpace(path))
                    return path;
            }
            return "";
        }

        /// <summary>
        /// First image-like media path on the component (webp/png/jpg/gif/svg), skipping excluded fields.
        /// </summary>
        public static string GetFirstImagePath(ComponentGroup group, params string[] excludeFieldNames)
        {
            if (group?.Fields == null || group.Fields.Count == 0)
                return "";

            foreach (var field in group.Fields)
            {
                var name = field.FieldName?.Trim() ?? "";
                if (excludeFieldNames != null && excludeFieldNames.Any(x =>
                        string.Equals(x?.Trim(), name, StringComparison.OrdinalIgnoreCase)))
                    continue;

                var path = NormalizeMediaPath(
                    !string.IsNullOrWhiteSpace(field.ImagePath) ? field.ImagePath : field.FieldValue);

                if (string.IsNullOrWhiteSpace(path))
                    continue;

                var ext = System.IO.Path.GetExtension(path.Split('?', '#')[0])?.ToLowerInvariant() ?? "";
                if (ext is ".pdf" or ".doc" or ".docx" or ".xls" or ".xlsx" or ".ppt" or ".pptx" or ".zip" or ".rar" or ".csv" or ".txt")
                    continue;

                if (ext is ".webp" or ".png" or ".jpg" or ".jpeg" or ".gif" or ".svg"
                    || (string.IsNullOrEmpty(ext) && path.Contains("upload", StringComparison.OrdinalIgnoreCase)))
                    return path;
            }

            return "";
        }




        public static string GetValue(Dictionary<string, ComponentField> dict, params string[] keys)
        {
            foreach (var key in keys)
            {
                var match = dict.FirstOrDefault(x =>
                    x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(match.Value?.FieldValue))
                    return match.Value.FieldValue;
            }
            return "";
        }

        public static int GetIntValue(Dictionary<string, ComponentField> dict, params string[] keys)
        {
            foreach (var key in keys)
            {
                var match = dict.FirstOrDefault(x =>
                    x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

                if (int.TryParse(match.Value?.FieldValue, out int result))
                    return result;
            }

            return 0; // default if not found
        }

        public static string GetMetaUrl(string baseUrl, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "";

            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return path;

            return $"{baseUrl?.TrimEnd('/')}/{path.TrimStart('/')}";
        }


        public static string GetArticleUrl(DataRow row)
        {
            var externalUrl = row["cont_external_url"]?.ToString();
            var mediaFile = row["MediafilePath"]?.ToString();
            var pageUrl = row["pageurl"]?.ToString();

            if (!string.IsNullOrWhiteSpace(externalUrl))
                return externalUrl;

            if (!string.IsNullOrWhiteSpace(mediaFile))
                return mediaFile;

            return pageUrl?.Replace("/global", "");
        }

        public static string GetArticleTarget(DataRow row)
        {
            var externalUrl = row["cont_external_url"]?.ToString();
            var mediaFile = row["MediafilePath"]?.ToString();
            var pageTarget = row["pageurl"]?.ToString();

            if (!string.IsNullOrWhiteSpace(externalUrl) ||
                !string.IsNullOrWhiteSpace(mediaFile))
            {
                return "_blank";
            }

            return string.Equals(pageTarget, "self", StringComparison.OrdinalIgnoreCase)
                ? "_self"
                : "_blank";
        }


        public static List<ArticleModel> MapArticleList(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return new List<ArticleModel>();

            return table.AsEnumerable()
                .Select(row => new ArticleModel
                {
                    ContId = row.Field<int>("cont_id"),
                    ContParentId = row.Field<int>("cont_parent_id"),

                    Title = string.IsNullOrWhiteSpace(row["cont_hmpg_title"]?.ToString())
                        ? row["cont_title"]?.ToString()
                        : row["cont_hmpg_title"]?.ToString(),

                    Intro = row["cont_intro"]?.ToString(),
                    HmpgIntro = row["cont_hmpg_intro"]?.ToString(),
                    PageName = row["cont_pagename"]?.ToString(),

                    ThumbnailImage = row["Hmpg_thumbnail"]?.ToString(),
                    ThumbnailAltText = row["Hmpg_thumbnail_alt_text"]?.ToString(),

                    ExternalUrl = row["cont_external_url"]?.ToString(),
                    MediafilePath = row["MediafilePath"]?.ToString(),

                    Url = GetArticleUrl(row),
                    UrlTarget = GetArticleTarget(row),

                    DisplayDate = table.Columns.Contains("cont_displaydate")
                        ? row.Field<DateTime?>("cont_displaydate")
                        : null,

                    Sequence = row.Field<int?>("cont_sequence") ?? 0
                })
                .ToList();
        }



    }
}