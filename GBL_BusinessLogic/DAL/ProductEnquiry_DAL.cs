using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.DAL
{
    public class ProductEnquiry_DAL : DBHelper
    {
        public ProductEnquiry_DAL(IConfiguration configuration) : base(configuration)
        {
        }

        public DataTable AddProductEnquiry_DAL(ProductEnquiry model)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@ProductName", string.IsNullOrWhiteSpace(model.ProductName) ? (object)DBNull.Value : model.ProductName),
                new SqlParameter("@ProductPageName", string.IsNullOrWhiteSpace(model.ProductPageName) ? (object)DBNull.Value : model.ProductPageName),
                new SqlParameter("@FullName", string.IsNullOrWhiteSpace(model.FullName) ? (object)DBNull.Value : model.FullName),
                new SqlParameter("@CompanyName", string.IsNullOrWhiteSpace(model.CompanyName) ? (object)DBNull.Value : model.CompanyName),
                new SqlParameter("@StreetAddress", string.IsNullOrWhiteSpace(model.StreetAddress) ? (object)DBNull.Value : model.StreetAddress),
                new SqlParameter("@City", string.IsNullOrWhiteSpace(model.City) ? (object)DBNull.Value : model.City),
                new SqlParameter("@State", string.IsNullOrWhiteSpace(model.State) ? (object)DBNull.Value : model.State),
                new SqlParameter("@Country", string.IsNullOrWhiteSpace(model.Country) ? (object)DBNull.Value : model.Country),
                new SqlParameter("@Phone", string.IsNullOrWhiteSpace(model.Phone) ? (object)DBNull.Value : model.Phone),
                new SqlParameter("@CountryCode", string.IsNullOrWhiteSpace(model.CountryCode) ? (object)DBNull.Value : model.CountryCode),
                new SqlParameter("@Mobile", string.IsNullOrWhiteSpace(model.Mobile) ? (object)DBNull.Value : model.Mobile),
                new SqlParameter("@Fax", string.IsNullOrWhiteSpace(model.Fax) ? (object)DBNull.Value : model.Fax),
                new SqlParameter("@Email", string.IsNullOrWhiteSpace(model.Email) ? (object)DBNull.Value : model.Email),
                new SqlParameter("@BusinessType", string.IsNullOrWhiteSpace(model.BusinessType) ? (object)DBNull.Value : model.BusinessType),
                new SqlParameter("@EnquiryDetails", string.IsNullOrWhiteSpace(model.EnquiryDetails) ? (object)DBNull.Value : model.EnquiryDetails),
                new SqlParameter("@AcceptTerms", model.AcceptTerms),
                new SqlParameter("@NotRobot", model.NotRobot),
                new SqlParameter("@IPAddress", string.IsNullOrWhiteSpace(model.IPAddress) ? (object)DBNull.Value : model.IPAddress),
            };

            return GetDataSet("sp_AddProductEnquiry", sqlParams).Tables[0];
        }
    }
}
