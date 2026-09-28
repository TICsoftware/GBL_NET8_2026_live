using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
    public class Industry_Public_BAL
    {
        private readonly Industry_Public_DAL _dal;

        public Industry_Public_BAL(IConfiguration config)
        {
            _dal = new Industry_Public_DAL(config);
        }

        public Industry_Public_Page GetIndex(string pageName, int languageId, Industry_Public_Filter filter)
        {
            var page = _dal.GetIndex(NormalizePageName(pageName), languageId, filter ?? new Industry_Public_Filter());
            foreach (var item in page.Industries)
                item.Intro = ToPlainText(item.Intro);
            return page;
        }

        public (List<Industry_Public_Item> Data, int Total) GetPaged(Industry_Public_Filter filter)
        {
            var result = _dal.GetPaged(filter ?? new Industry_Public_Filter());
            foreach (var item in result.Data)
                item.Intro = ToPlainText(item.Intro);
            return result;
        }

        public Industry_Inside_Page GetInside(string pageName, int languageId, int categoryId, int page, int pageSize)
        {
            var model = _dal.GetInside(pageName, languageId, categoryId, page, pageSize);
            foreach (var item in model.Products)
                item.Intro = ToPlainText(item.Intro);
            return model;
        }

        public (List<Industry_Inside_Product> Data, int Total) GetInsideProducts(Industry_Inside_Filter filter)
        {
            var result = _dal.GetInsideProducts(filter ?? new Industry_Inside_Filter());
            foreach (var item in result.Data)
                item.Intro = ToPlainText(item.Intro);
            return result;
        }

        private static string NormalizePageName(string? pageName)
        {
            var value = (pageName ?? string.Empty).Trim().Trim('/');
            if (string.IsNullOrWhiteSpace(value) ||
                string.Equals(value, "Industries", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "Industries/Index", System.StringComparison.OrdinalIgnoreCase))
            {
                return "industries";
            }
            return value;
        }

        private static string ToPlainText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            var text = Regex.Replace(html, "<[^>]+>", " ");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return System.Net.WebUtility.HtmlDecode(text);
        }
    }
}
