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
    public class Page_Manage_DAL : DBHelper
    {
        public Page_Manage_DAL(IConfiguration configuration) : base(configuration) //  call base class constructor 
        {
        }

        public DataSet GetContentComponentData_DAL(string pagename, int LanguageID, int GeographyID)
        {
            SqlParameter[] sqlParams =
            {
                new ("@pagename", string.IsNullOrWhiteSpace( pagename) ? null : pagename.Trim()),
                new ("@LanguageID", LanguageID >0 ? LanguageID : null),
                new ("@GeographyID", GeographyID >0 ? GeographyID : null),
            };

            return GetDataSet("sp_GetContentComponentData", sqlParams);
        }

        public DataSet Get_Blogs_List_DAL(int contentId, int page, int pageSize)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@ContentId", contentId),
                new SqlParameter("@Page", page),
                new SqlParameter("@PageSize", pageSize)
            };

            return GetDataSet("Get_Blogs_List", sqlParams);
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


        public DataSet GetContentComponentListPaged_DAL(string pagename, int LanguageID, int GeographyID)
        {
            SqlParameter[] sqlParams =
            {
                new ("@pagename", string.IsNullOrWhiteSpace( pagename) ? null : pagename.Trim()),
                new ("@LanguageID", LanguageID >0 ? LanguageID : null),
                new ("@GeographyID", GeographyID >0 ? GeographyID : null),
            };

            return GetDataSet("GetContentComponentListPaged", sqlParams);
        }

        public DataSet GetContentComponentsPaging_DAL(int ContentID, int PageNumber, int PageSize)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@Cont_id", ContentID),
                new SqlParameter("@LanguageID", 1),
                new SqlParameter("@GeographyID", 1),
                new SqlParameter("@PageNumber", PageNumber),
                new SqlParameter("@PageSize", PageSize)
            };

            return GetDataSet("GetContentComponentsPaging", sqlParams);
        }


        public DataSet GetEventsContent_DAL(string pagename, int LanguageID, int GeographyID)
        {
            SqlParameter[] sqlParams =
            {
                new ("@pagename", string.IsNullOrWhiteSpace( pagename) ? null : pagename.Trim()),
                new ("@LanguageID", LanguageID >0 ? LanguageID : null),
                new ("@GeographyID", GeographyID >0 ? GeographyID : null),
            };

            return GetDataSet("GetEventsContent", sqlParams);
        }





    }
}