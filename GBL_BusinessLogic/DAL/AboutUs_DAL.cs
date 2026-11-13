using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GBL_BusinessLogic.DAL
{
    public class AboutUs_DAL : Page_Manage_DAL
    {
        public AboutUs_DAL(IConfiguration configuration) : base(configuration)
        {
        }

        public new DataSet GetLegacyPageData_DAL(string pagename, int LanguageID, int GeographyID)
        {
            SqlParameter[] sqlParams =
            {
                new ("@pagename", string.IsNullOrWhiteSpace(pagename) ? null : pagename.Trim()),
                new ("@LanguageID", LanguageID > 0 ? LanguageID : null),
                new ("@GeographyID", GeographyID > 0 ? GeographyID : null),
            };

            return GetDataSet("sp_GetLegacyPageData", sqlParams);
        }

        public new DataSet GetLeadershipPageData_DAL(string pagename, int LanguageID, int GeographyID)
        {
            SqlParameter[] sqlParams =
            {
                new ("@pagename", string.IsNullOrWhiteSpace(pagename) ? null : pagename.Trim()),
                new ("@LanguageID", LanguageID > 0 ? LanguageID : null),
                new ("@GeographyID", GeographyID > 0 ? GeographyID : null),
            };

            return GetDataSet("sp_GetLeadershipPageData", sqlParams);
        }

        public new DataSet GetVisionPageData_DAL(string pagename, int LanguageID, int GeographyID)
        {
            SqlParameter[] sqlParams =
            {
                new ("@pagename", string.IsNullOrWhiteSpace(pagename) ? null : pagename.Trim()),
                new ("@LanguageID", LanguageID > 0 ? LanguageID : null),
                new ("@GeographyID", GeographyID > 0 ? GeographyID : null),
            };

            return GetDataSet("sp_GetVisionPageData", sqlParams);
        }
    }
}
