using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using Microsoft.Data.SqlClient;
using GBL_BusinessLogic.Entity;

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


        public DataSet Get_PressRelease_page_wise_DAL(int contentId, int page, int pageSize, int? year = null, int? month = null, int? tagId = null)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@ContentId", contentId),
                new SqlParameter("@PageNumber", page),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@Year", (object?)year ?? DBNull.Value),
                new SqlParameter("@Month", (object?)month ?? DBNull.Value),
                new SqlParameter("@TagId", (object?)tagId ?? DBNull.Value)
            };

            return GetDataSet("Get_PressReleases_Articles_List", sqlParams);
        }

        public DataSet Get_SustainabilityReports_page_wise_DAL(int contentId, int page, int pageSize, int? year = null, int? month = null, int? tagId = null)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@ContentId", contentId),
                new SqlParameter("@PageSize", pageSize),
                new SqlParameter("@PageNumber", page)
            };

            return GetDataSet("Get_Sustainability_Reports_Articles_List", sqlParams);
        }

        public DataSet Get_Investor_Pdf_page_wise_DAL(int contentId, int page, int pageSize)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@ContentId", contentId),
                new SqlParameter("@PageNumber", page),
                new SqlParameter("@PageSize", pageSize)
            };

            return GetDataSet("Get_Investor_Pdf_Articles_List", sqlParams);
        }

        public DataSet Get_Annual_Reports_page_wise_DAL(int contentId, int page, int pageSize)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@ContentId", contentId),
                new SqlParameter("@PageNumber", page),
                new SqlParameter("@PageSize", pageSize)
            };

            return GetDataSet("Get_Annual_Reports_Articles_List", sqlParams);
        }



        public DataTable ContactUs_Insert_DAL(ContactEntity model)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@Name", string.IsNullOrWhiteSpace(model.Name) ? (object)DBNull.Value : model.Name),
                new SqlParameter("@CompanyName", string.IsNullOrWhiteSpace(model.CompanyName) ? (object)DBNull.Value : model.CompanyName),
                new SqlParameter("@PhoneNumber", string.IsNullOrWhiteSpace(model.PhoneNumber) ? (object)DBNull.Value : model.PhoneNumber),
                new SqlParameter("@EmailAddress", string.IsNullOrWhiteSpace(model.EmailAddress) ? (object)DBNull.Value : model.EmailAddress),
                new SqlParameter("@CountryId", model.CountryId), 
                new SqlParameter("@SiteLocation",  string.IsNullOrWhiteSpace(model.SiteLocation)                        ? (object)DBNull.Value : model.SiteLocation),
                new SqlParameter("@Query", string.IsNullOrWhiteSpace(model.Query)  ? (object)DBNull.Value : model.Query),
                new SqlParameter("@IPAddress", string.IsNullOrWhiteSpace(model.IPAddress) ? (object)DBNull.Value : model.IPAddress),
                new SqlParameter("@Consent", model.Consent.HasValue ? (object)model.Consent.Value : DBNull.Value)
            };

            return GetDataSet("ContactUs_Insert", sqlParams).Tables[0];
        }


 public DataTable GetStateMaster()
        {
            return GetDataSet("GetStateMaster").Tables[0];
        }


        public DataTable GetCountryMaster()
        {
            return GetDataSet("GetCountryMaster").Tables[0];
        }

    }
}