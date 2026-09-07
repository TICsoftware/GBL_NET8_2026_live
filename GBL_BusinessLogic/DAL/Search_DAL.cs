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

        public DataSet GetSearch__DAL(string keyword, int page = 1, int pageCount = 10)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@Keyword", string.IsNullOrWhiteSpace(keyword) ? DBNull.Value : keyword.Trim()),
                new SqlParameter("@Page", page),
                new SqlParameter("@PageCount", pageCount)
            };

            return GetDataSet("sp_Search_ContentComponent", sqlParams);
        }


    }
}