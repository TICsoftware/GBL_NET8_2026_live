using System.Data;
using GBL_BusinessLogic.BAL;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GBL_BusinessLogic.DAL
{
    public class Media_DAL :  BasePageBAL
    {
        public Media_DAL(IConfiguration configuration) : base(configuration) //  call base class constructor 
        {
        }

        public DataSet GetLeadershipComponents_DAL(int cont_id, int LanguageID, int GeographyID)
        {
             SqlParameter[] sqlParams =
            {
                new ("@cont_id", cont_id),
                new ("@LanguageID", LanguageID >0 ? LanguageID : null),
                new ("@GeographyID", GeographyID >0 ? GeographyID : null),
            };

            return GetDataSet("GetLeadershipComponents", sqlParams);
        }

        public DataSet Get_Annual_Sustainability_Reports_Components_DAL(int LanguageID, int GeographyID)
        {
            SqlParameter[] sqlParams =
            {                 
                new ("@LanguageID", LanguageID >0 ? LanguageID : null),
                new ("@GeographyID", GeographyID >0 ? GeographyID : null),
            };

            return GetDataSet("Get_Annual_Sustainability_Reports_Components", sqlParams);
        }

    }
}