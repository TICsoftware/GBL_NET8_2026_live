using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.DAL;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class Search_BAL : Search_DAL
    {
        private readonly IConfiguration _configuration;
        public Search_BAL(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }



        protected int articlepagecount = 10;

        public Search search_display(string keyword, int page, string category = null)
        {
            Search model = new Search();

            model.txttitle = keyword;
            model.keyword = keyword;
            model.currentpage = page;
            model.category = NormalizeCategory(category);

            string rawKeyword = (keyword ?? "").Trim();
            string strKeywordFinal = rawKeyword.Length == 0
                ? ""
                : FullText_Search.FullText_Search_String(rawKeyword, FullText_Search_Operators.NEAR, true);

            DataSet dsSearchDisplay = GetSearch__DAL(strKeywordFinal, page, articlepagecount, rawKeyword, model.category);

            model.objsearchdisplay = new List<Content_Search_Display>();

            if (dsSearchDisplay != null &&
                dsSearchDisplay.Tables.Count > 0 &&
                dsSearchDisplay.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsSearchDisplay.Tables[0].Rows)
                {
                    model.objsearchdisplay.Add(new Content_Search_Display
                    {
                        cont_title = Highlight(keyword, dr["cont_title"]?.ToString()),

                        content = Highlight
                        (
                            keyword,
                            trimData(FullText_Search.Remove_All_Tags(dr["content"]?.ToString()))
                        ),

                        cont_pagename = dr["cont_pagename"]?.ToString(),

                        cont_status = dr["cont_status"] != DBNull.Value
                                        ? Convert.ToInt32(dr["cont_status"])
                                        : 0,

                        PageLink = dr["PageUrl"]?.ToString(),

                        Row = dr["RowNo"] != DBNull.Value
                                ? Convert.ToInt32(dr["RowNo"])
                                : 0,

                        URL = dr["PageUrl"]?.ToString(),

                        search_URL = dr["PageUrl"]?.ToString(),

                        cont_publishdate =
                            string.IsNullOrWhiteSpace(dr["cont_publishdate"]?.ToString())
                            ? ""
                            : Convert.ToDateTime(dr["cont_publishdate"])
                                .ToString("MMMM dd, yyyy"),

                        section_name = dr.Table.Columns.Contains("ComponentName") ? dr["ComponentName"]?.ToString() : "",

                        result_type = ResultTypeOf(dr),

                        section_path = Cell(dr, "SectionPath"),

                        hmpg_intro = Highlight
                        (
                            keyword,
                            trimData(FullText_Search.Remove_All_Tags(dr["cont_hmpg_intro"]?.ToString()))
                        ),

                        intro = Highlight
                        (
                            keyword,
                            trimData(FullText_Search.Remove_All_Tags(dr["cont_intro"]?.ToString()))
                        )
                    });
                }
            }

            if (dsSearchDisplay != null &&
                dsSearchDisplay.Tables.Count > 1 &&
                dsSearchDisplay.Tables[1].Rows.Count > 0)
            {
                model.totalrecords = Convert.ToInt32(dsSearchDisplay.Tables[1].Rows[0]["TotalCount"]);

                model.count = (int)Math.Ceiling
                (
                    Convert.ToDouble(model.totalrecords) / articlepagecount
                );
            }
            else
            {
                model.totalrecords = 0;
                model.count = 0;
            }

            if (page <= 1)
                TryLog(rawKeyword, model.category, "Keyword", null, "", null);

            return model;
        }





        // ---------- Header overlay: quick results ----------

        // Site sections used by the overlay's category chips, matched on the first URL segment.
        private static readonly Dictionary<string, string[]> SuggestCategories = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "Investors",      new[] { "investors", "investor", "investor-relations", "annualreports", "annual-reports", "quarterly-reports", "quarterlyreports", "financial-highlights", "corporate-governance" } },
            { "Businesses",     new[] { "businesses", "products", "industries", "industries-we-serve", "industriesinside" } },
            { "Sustainability", new[] { "sustainability", "csr", "resource-circularity" } },
            { "Media",          new[] { "media", "awards" } },
            { "Careers",        new[] { "careers", "career", "learningdevelopment" } }
        };

        private const int SuggestFetchSize = 60;

        public SearchSuggestResult search_suggest(string keyword, string category, int take = 8)
        {
            var result = new SearchSuggestResult
            {
                q = (keyword ?? "").Trim(),
                cat = NormalizeCategory(category)
            };

            if (result.q.Length < 2) return result;

            string strKeywordFinal = FullText_Search.FullText_Search_String(result.q, FullText_Search_Operators.NEAR, true);
            DataSet ds = GetSearch__DAL(strKeywordFinal, 1, SuggestFetchSize, result.q, result.cat);

            return MapSuggest(ds, result, take);
        }

        public SearchSuggestResult search_browse(string category, int take = 8)
        {
            var result = new SearchSuggestResult
            {
                q = "",
                cat = NormalizeCategory(category)
            };

            if (result.cat == "All") return result;
            if (take < 1) take = 8;
            if (take > 20) take = 20;

            DataSet ds = GetBrowse__DAL(result.cat, take);
            return MapSuggest(ds, result, take);
        }

        public SearchPopularResult search_popular()
        {
            var result = new SearchPopularResult();
            DataSet ds = GetPopular__DAL();
            if (ds == null || ds.Tables.Count == 0) return result;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string term = Cell(dr, "Keyword");
                if (!string.IsNullOrWhiteSpace(term)) result.terms.Add(term);
            }

            if (ds.Tables.Count > 1)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    string title = Cell(dr, "ItemTitle");
                    string url = Cell(dr, "ItemUrl");
                    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(url)) continue;

                    result.featured.Add(new SearchSuggestItem
                    {
                        title = title,
                        url = url,
                        type = Cell(dr, "ItemType"),
                        path = Cell(dr, "SectionName"),
                        cat = Cell(dr, "Category")
                    });
                }
            }

            return result;
        }

        public void search_track(string keyword, string category, string title, string url, string type, string section)
        {
            string term = NormalizeTerm(keyword);
            string cat = CleanCategory(category);
            string safeUrl = SafeUrl(url);
            string itemTitle = NormalizeTerm(title);
            if (itemTitle.Length > 500) itemTitle = itemTitle.Substring(0, 500);
            string itemType = NormalizeItemType(type);
            string sectionName = NormalizeTerm(section);
            if (sectionName.Length > 300) sectionName = sectionName.Substring(0, 300);

            if (term.Length >= 2)
                TryLog(term, NormalizeCategory(cat), "Keyword", null, "", null);

            if (itemTitle.Length >= 2 && safeUrl.Length > 1 && itemType != "Keyword")
                TryLog(itemTitle, cat, itemType, itemTitle, safeUrl, sectionName);
        }

        private SearchSuggestResult MapSuggest(DataSet ds, SearchSuggestResult result, int take)
        {
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                result.total = 0;
                result.items = new List<SearchSuggestItem>();
                return result;
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var matches = new List<SearchSuggestItem>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                var item = MapSuggestItem(dr);
                if (item == null || !seen.Add(item.url)) continue;

                // Keyword search keeps the chip filter. Browsing a chip trusts the section query.
                if (!string.IsNullOrEmpty(result.q) && result.cat != "All" && item.cat != result.cat) continue;

                matches.Add(item);
            }

            int sqlTotal = matches.Count;
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0 && ds.Tables[1].Columns.Contains("TotalCount"))
                sqlTotal = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);

            result.total = Math.Max(sqlTotal, matches.Count);
            result.items = matches.Take(take).ToList();
            return result;
        }

        private SearchSuggestItem MapSuggestItem(DataRow dr)
        {
            string url = Cell(dr, "PageUrl").Trim();
            if (string.IsNullOrWhiteSpace(url)) return null;

            string title = FullText_Search.Remove_All_Tags(Cell(dr, "cont_title")).Trim();
            if (string.IsNullOrWhiteSpace(title)) title = Cell(dr, "cont_pagename");
            if (string.IsNullOrWhiteSpace(title)) title = url;

            string sectionPath = Cell(dr, "SectionPath");
            string component = Cell(dr, "ComponentName");
            string itemCat = CategoryForUrl(url);
            if (itemCat == "All")
            {
                string typeCat = ResultTypeOf(dr);
                if (typeCat == "Product" || typeCat == "Industry") itemCat = "Businesses";
            }

            return new SearchSuggestItem
            {
                title = System.Net.WebUtility.HtmlDecode(title),
                url = url,
                type = ResultTypeOf(dr),
                path = !string.IsNullOrWhiteSpace(sectionPath) ? sectionPath : BreadcrumbForUrl(url, component),
                cat = itemCat
            };
        }

        private static string FirstSegment(string url)
        {
            string path = url;
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri abs)) path = abs.AbsolutePath;
            return path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
        }

        private static string CategoryForUrl(string url)
        {
            string first = FirstSegment(url);
            foreach (var pair in SuggestCategories)
            {
                if (pair.Value.Any(v => string.Equals(v, first, StringComparison.OrdinalIgnoreCase))) return pair.Key;
            }
            return "All";
        }

        private static string BreadcrumbForUrl(string url, string section)
        {
            string path = url;
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri abs)) path = abs.AbsolutePath;

            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(p => p.IndexOf('.') < 0) // drop file names such as report.pdf
                            .Take(3)
                            .Select(Prettify)
                            .ToList();

            if (parts.Count == 0) return string.IsNullOrWhiteSpace(section) ? "Home" : section;
            return string.Join(" › ", parts);
        }

        private static string Prettify(string segment)
        {
            string s = Uri.UnescapeDataString(segment).Replace('-', ' ').Replace('_', ' ').Trim();
            if (s.Length == 0) return s;
            var words = s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                         .Select(w => w.Length > 3 || w.Length == 1 ? char.ToUpperInvariant(w[0]) + w.Substring(1).ToLowerInvariant() : w.ToLowerInvariant());
            string joined = string.Join(" ", words);
            return char.ToUpperInvariant(joined[0]) + joined.Substring(1);
        }


        private static string NormalizeCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return "All";
            foreach (var key in SuggestCategories.Keys)
            {
                if (string.Equals(key, category.Trim(), StringComparison.OrdinalIgnoreCase))
                    return key;
            }
            return "All";
        }

        private static string CleanCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category)) return "All";
            string value = category.Trim();
            return value.Length > 100 ? value.Substring(0, 100) : value;
        }

        private static string NormalizeTerm(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            string term = string.Join(" ", value.Trim().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            return term.Length > 200 ? term.Substring(0, 200) : term;
        }

        private static string NormalizeItemType(string type)
        {
            switch ((type ?? "").Trim().ToLowerInvariant())
            {
                case "product": return "Product";
                case "industry": return "Industry";
                case "section": return "Section";
                case "article": return "Article";
                case "pdf": return "PDF";
                case "page": return "Page";
                default: return "Page";
            }
        }

        private static string SafeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return "";
            url = url.Trim();
            if (url.StartsWith("//", StringComparison.Ordinal)) return "";

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri abs))
            {
                if (abs.Scheme != Uri.UriSchemeHttp && abs.Scheme != Uri.UriSchemeHttps) return "";
                url = abs.AbsolutePath;
            }

            if (!url.StartsWith("/", StringComparison.Ordinal)) return "";
            int cut = url.IndexOfAny(new[] { '?', '#' });
            if (cut >= 0) url = url.Substring(0, cut);
            if (url.Contains("..")) return "";
            return url.Length > 1000 ? url.Substring(0, 1000) : url;
        }

        private void TryLog(string keyword, string category, string itemType, string itemTitle, string itemUrl, string sectionName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword)) return;
                LogSearch__DAL(keyword, category, itemType, itemTitle, itemUrl, sectionName);
            }
            catch
            {
                // Search still works if the popular-search table has not been created yet.
            }
        }

        private static string Cell(DataRow dr, string column)
        {
            if (dr?.Table == null || !dr.Table.Columns.Contains(column) || dr[column] == DBNull.Value)
                return "";
            return dr[column]?.ToString() ?? "";
        }

        private static string ResultTypeOf(DataRow dr)
        {
            string type = Cell(dr, "ResultType");
            if (!string.IsNullOrWhiteSpace(type)) return type;
            string url = Cell(dr, "PageUrl");
            return url.IndexOf(".pdf", StringComparison.OrdinalIgnoreCase) >= 0 ? "PDF" : "Page";
        }

        private string GetURl(string pdfUrl, string Path, string extURL)
        {
            try
            {
                string strURl = "";

                if (!string.IsNullOrWhiteSpace(pdfUrl) && pdfUrl.ToLower().Contains(".pdf"))
                {

                    strURl = pdfUrl;
                }

                else if (!string.IsNullOrWhiteSpace(extURL))
                {
                    strURl = extURL;

                }

                else
                {
                    strURl = Path;
                }


                return strURl;

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {

            }

        }


        private string Highlight(string Search_Str, string InputTxt)
        {
            try
            {
                Regex expression;
                var regex = new Regex(".*" + Search_Str + ".*", RegexOptions.IgnoreCase);
                //Search_Str = Regex.Replace(Search_Str, pattern, "");
                string[] arrstr = null;
                if (!regex.IsMatch(InputTxt))
                {
                    //Search_Str = Regex.Replace(Search_Str, patt_alpha.ToLower(), " ");
                    arrstr = Search_Str.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    string re = String.Join("|", arrstr.Select(s => Regex.Escape(s)).ToArray());
                    re = @"\b(?:" + re + @")\b";
                    var text = Regex.Replace(InputTxt, re, new MatchEvaluator(ReplaceKeywords), RegexOptions.IgnoreCase);
                    return text;
                }
                else
                {
                    expression = new Regex(Search_Str, RegexOptions.IgnoreCase);
                    return expression.Replace(InputTxt, new MatchEvaluator(ReplaceKeywords));
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }



        private string trimData(string strContent)
        {
            if (strContent.Length > 300)
            {
                return strContent.Substring(0, 300);
            }
            else
            {
                return strContent;
            }

        }
        private string ReplaceKeywords(Match m)//(Match m)
        {
            try
            {
                return "<span class=highlight_txt>" + m.Value + "</span>";
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string RemoveHTML(string strHTML)
        {
            try
            {
                return Regex.Replace(strHTML, @"<(.|\n)*?>", string.Empty);
            }
            catch (Exception ex)
            {

                throw;
            }

        }







    }
}