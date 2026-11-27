using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.Data.SqlClient;

namespace GBL_BusinessLogic.DAL
{
    public class Search_DAL : DBHelper
    {
        public Search_DAL(IConfiguration configuration) : base(configuration) //  call base class constructor 
        {
        }

        public DataSet GetSearch__DAL(string keyword, int page = 1, int pageCount = 10, string rawKeyword = null, string category = null)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@Keyword", string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim()),
                new SqlParameter("@Page", page),
                new SqlParameter("@PageCount", pageCount),
                new SqlParameter("@RawKeyword", string.IsNullOrWhiteSpace(rawKeyword) ? DBNull.Value : rawKeyword.Trim()),
                new SqlParameter("@Category", string.IsNullOrWhiteSpace(category) || category.Equals("All", StringComparison.OrdinalIgnoreCase) ? DBNull.Value : category.Trim())
            };

            return GetDataSet("sp_Search_ContentComponent", sqlParams);
        }

        public DataSet GetBrowse__DAL(string category, int take)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@Category", string.IsNullOrWhiteSpace(category) ? "All" : category.Trim()),
                new SqlParameter("@Take", take)
            };

            return GetDataSet("sp_Search_BrowseSection", sqlParams);
        }

        public DataSet GetPopular__DAL()
        {
            return GetDataSet("sp_Search_GetPopular");
        }

        public void LogSearch__DAL(string keyword, string category, string itemType, string itemTitle, string itemUrl, string sectionName)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@Keyword", string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim()),
                new SqlParameter("@Category", string.IsNullOrWhiteSpace(category) ? "All" : category.Trim()),
                new SqlParameter("@ItemType", string.IsNullOrWhiteSpace(itemType) ? "Keyword" : itemType.Trim()),
                new SqlParameter("@ItemTitle", string.IsNullOrWhiteSpace(itemTitle) ? DBNull.Value : itemTitle.Trim()),
                new SqlParameter("@ItemUrl", string.IsNullOrWhiteSpace(itemUrl) ? "" : itemUrl.Trim()),
                new SqlParameter("@SectionName", string.IsNullOrWhiteSpace(sectionName) ? DBNull.Value : sectionName.Trim())
            };

            ExecuteNonQuery("sp_Search_Log", sqlParams);
        }


    }
}