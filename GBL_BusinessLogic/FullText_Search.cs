using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GBL_BusinessLogic
{
    public class FullText_Search
    {
        public static string FullText_Search_String(string strKeyword, int FullText_Search_Operators_ID, bool IsRelaventKeyWord)
        {
            if (string.IsNullOrEmpty(strKeyword.Trim()))
            {
                return "";
            }

            string mString = Convert_Search_String_AddPluse(strKeyword, IsRelaventKeyWord);

            if (mString.Contains(" -"))
            {
                mString = mString.Insert(mString.IndexOf(" -"), ")");
                mString = "(" + mString;
            }

            if (FullText_Search_Operators_ID == 1)
            {
                return mString.Replace("+", " NEAR ").Replace(" -", " AND NOT ");
            }
            else if (FullText_Search_Operators_ID == 2)
            {
                return mString.Replace("+", " AND ").Replace(" -", " AND NOT ");
            }
            else if (FullText_Search_Operators_ID == 3)
            {
                return mString.Replace("+", " OR ").Replace(" -", " AND NOT ");
            }
            else
            {
                return mString.Replace("+", " NEAR ").Replace(" -", " AND NOT ");
            }

        }


        private static string Convert_Search_String_AddPluse(string str, bool IsRelaventKeyWord)
        {

            //Dim parts() As String = Regex.Split(Remove_NoiseWords(str.Replace("&", "")), "(""[^""]+"")")
            string[] parts = Regex.Split(str.Replace("&", " ").Trim(), "(\"[^\"]+\")");
            System.Collections.ArrayList ignoreParts = new System.Collections.ArrayList();
            Regex re = new Regex("(\"[^\"]+\")");
            string Result = "";
            string strMinusSeprator = "TyLd8NyI459Y";

            foreach (string strParts in parts)
            {
                if (Regex.IsMatch(strParts, "(\"[^\"]+\")"))
                {
                    if ((string.IsNullOrEmpty(Result.Trim())))
                    {
                        Result += "&";
                    }
                    else
                    {
                        Result += " &";
                    }

                    ignoreParts.Add(strParts);

                }
                else
                {
                    Result += " " + Regex.Replace(strParts, "\\s+-\\s+", " ");

                    Result = Regex.Replace(Result, "\\s+\\-", strMinusSeprator);


                    Result = Regex.Replace(Result, "\\-\\s+", " ");

                    Result = Regex.Replace(Result, "\\s+\\+", "+");

                    Result = Regex.Replace(Result, "\\+\\s+", "+");
                    //comma
                    Result = Regex.Replace(Result, "\\s+\\,", ",");

                    Result = Regex.Replace(Result, "\\,\\s+", ",");

                    Result = Regex.Replace(Result, ",", "+");
                    //end comma

                    //Remove special charector
                    //Result = Regex.Replace(Result, "[^\w\\-]", " ")
                    //Result = Regex.Replace(Result, "[^A-Za-z0-9\-/&]", " ")


                    //end Remove special charector
                    Result = Regex.Replace(Result, "-", " ");
                    //Result = Regex.Replace(Result, strMinusSeprator, " -")

                    Result = Regex.Replace(Result, "[<>.,;:'\"\\[\\]{}|~`!@#$%^*()=?/\\\\]", " ");


                    //Commented for avoid seperation of charector in hindi -- need to check in other languages also.
                    //Result = Regex.Replace(Result, "\\b(\\w)\\b", " ");


                    //remove white space
                    Result = Regex.Replace(Result, "\\s+", " ");

                    //remove space before "(" and ")"
                    Result = Result.Replace("( ", "(").Replace(" )", ")");

                    Result = Regex.Replace(Result, "\\s+", "+");
                    //remove+- and ---- as -
                    Result = Regex.Replace(Result, "\\-\\+", "-");

                    Result = Regex.Replace(Result, "\\+\\-", "-");

                    Result = Regex.Replace(Result, "\\-+", "-");
                    //end remove+- and ---- as -

                    Result = Regex.Replace(Result, "\\++", "+");


                }


            }

            Result = Regex.Replace(Result, strMinusSeprator, " -");
            Result = Regex.Replace(Result, "\\-\\+", "-");
            Result = Regex.Replace(Result, "\\+\\-", "-");
            Result = Regex.Replace(Result, "\\-+", "-");
            return MeargeString(ignoreParts, Result, IsRelaventKeyWord);

        }


        private static string MeargeString(System.Collections.ArrayList arr, string str, bool IsRelaventKeyWord)
        {
            int intValue = 0;
            MatchCollection matches = Regex.Matches(str, "&");
            //Dim strFinalValue As String = ""
            // Added for part word "word*"
            if ((!IsUnicode(str)))
            {
                //str = Regex.Replace(str, "(\w+)", """$1*""")
                //str = Regex.Replace(str, "(\\w\\w\\w+)", "\"$1*\"");

                /*str = Regex.Replace(str, "(\\w\\w\\w+)", "\"$1\"");
                str = Regex.Replace(str, "(\\b\\w\\w\\b)", "\"$1\"");
                str = Regex.Replace(str, "(\\b\\w\\b)", "\"$1\"");*/
                if (IsRelaventKeyWord)
                {
                    str = Regex.Replace(str, "(\\w\\w\\w+)", "\"$1*\"");
                    str = Regex.Replace(str, "(\\b\\w\\w\\b)", "\"$1*\"");
                    str = Regex.Replace(str, "(\\b\\w\\b)", "\"$1*\"");
                }
                else
                {
                    str = Regex.Replace(str, "(\\w\\w\\w+)", "\"$1\"");
                    str = Regex.Replace(str, "(\\b\\w\\w\\b)", "\"$1\"");
                    str = Regex.Replace(str, "(\\b\\w\\b)", "\"$1\"");
                }
            }
            /*else
            {
                str = Regex.Replace(str, "(\\w\\w\\w+)", "\"$1\"");
                //str = Regex.Replace(str, "(\\b\\w\\w\\b)", "\"$1\"");
                str = Regex.Replace(str, "(\\b\\w\\b)", "\"$1\"");
            }*/
            //End for part word "word*" 


            Match m = Regex.Match(str, "&");

            while (m.Success == true)
            {
                //str = str.Remove(m.Index, 1).Insert(m.Index, arr[intValue]);
                str = str.Remove(m.Index, 1).Insert(m.Index, arr[intValue].ToString());
                intValue = intValue + 1;
                m = Regex.Match(str, "&");
            }

            if ((str.StartsWith("-") | str.StartsWith("+")))
            {
                str = str.Remove(0, 1);
            }

            if ((str.EndsWith("-") | str.EndsWith("+")))
            {
                str = str.Remove(str.Length - 1, 1);
            }

            str = str.Replace("+ -", " -");
            if ((string.IsNullOrEmpty(str.Trim())))
            {
                str = "\"\"";
            }
            return str;
        }



        public static string Remove_All_Tags(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return System.Text.RegularExpressions.Regex
                .Replace(input, "<.*?>", string.Empty);
        }

        private static bool IsUnicode(string s)
        {
            char[] items = s.ToCharArray();
            for (int i = 0; i <= items.Length - 1; i++)
            {
                //if (Convert.ToUInt16(Strings.AscW(items[i])) > 255)
                if (Convert.ToUInt16(items[i]) > 255)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public static class FullText_Search_Operators
    {
        public static int NEAR = 1;
        public static int AND = 2;
        public static int OR = 3;

        public static int NEAR_INFLECTIONAL = 4;
        public static int AND_INFLECTIONAL = 5;
        public static int OR_INFLECTIONAL = 6;
    }
}