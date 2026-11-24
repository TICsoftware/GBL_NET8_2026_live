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

        public Search search_display(string keyword, int page)
        {
            Search model = new Search();

            model.txttitle = keyword;
            model.keyword = keyword;
            model.currentpage = page;

            string strKeywordFinal = FullText_Search.FullText_Search_String(keyword, FullText_Search_Operators.NEAR, true);

            DataSet dsSearchDisplay = GetSearch__DAL(strKeywordFinal, page, articlepagecount);

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
                cat = string.IsNullOrWhiteSpace(category) || !SuggestCategories.ContainsKey(category) ? "All" : category
            };

            if (result.q.Length < 2) return result;

            string strKeywordFinal = FullText_Search.FullText_Search_String(result.q, FullText_Search_Operators.NEAR, true);
            DataSet ds = GetSearch__DAL(strKeywordFinal, 1, SuggestFetchSize);

            if (ds == null || ds.Tables.Count == 0) return result;

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var matches = new List<SearchSuggestItem>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string url = dr["PageUrl"]?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(url) || !seen.Add(url)) continue;

                string itemCat = CategoryForUrl(url);
                if (result.cat != "All" && itemCat != result.cat) continue;

                string title = FullText_Search.Remove_All_Tags(dr["cont_title"]?.ToString() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(title)) title = dr["cont_pagename"]?.ToString() ?? url;

                string section = dr.Table.Columns.Contains("ComponentName") ? dr["ComponentName"]?.ToString() : "";

                matches.Add(new SearchSuggestItem
                {
                    title = System.Net.WebUtility.HtmlDecode(title),
                    url = url,
                    type = url.IndexOf(".pdf", StringComparison.OrdinalIgnoreCase) >= 0 ? "PDF" : "Page",
                    path = BreadcrumbForUrl(url, section),
                    cat = itemCat
                });
            }

            result.total = matches.Count;
            result.items = matches.Take(take).ToList();
            return result;
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