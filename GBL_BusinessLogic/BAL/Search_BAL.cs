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