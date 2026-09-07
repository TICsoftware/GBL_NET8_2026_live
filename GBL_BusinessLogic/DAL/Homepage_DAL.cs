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
    public class Homepage_DAL : DBHelper
    {
        public Homepage_DAL(IConfiguration configuration) : base(configuration) //  call base class constructor 
        {
        }

        public DataSet GetHomepage_DAL(int LanguageID, int GeographyID)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@LanguageID", LanguageID),
                new SqlParameter("@GeographyID", GeographyID)
            };

            return GetDataSet("GetHomepage", sqlParams);
        }

        public DataSet GetContentComponentById_DAL(int ContentID, string Group_Id)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@ContentID", ContentID),
                new SqlParameter("@Group_Id", @Group_Id)
            };

            return GetDataSet("GetContentComponentById", sqlParams);
        }


    }
}