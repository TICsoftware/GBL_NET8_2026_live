using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.DAL
{
    public class WorkWithUsEnquiry_DAL : DBHelper
    {
        public WorkWithUsEnquiry_DAL(IConfiguration configuration) : base(configuration)
        {
        }

        public DataTable AddWorkWithUsEnquiry_DAL(WorkWithUsEnquiry model)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@FullName", string.IsNullOrWhiteSpace(model.FullName) ? (object)DBNull.Value : model.FullName),
                new SqlParameter("@Email", string.IsNullOrWhiteSpace(model.Email) ? (object)DBNull.Value : model.Email),
                new SqlParameter("@Expertise", string.IsNullOrWhiteSpace(model.Expertise) ? (object)DBNull.Value : model.Expertise),
                new SqlParameter("@Address", string.IsNullOrWhiteSpace(model.Address) ? (object)DBNull.Value : model.Address),
                new SqlParameter("@Designation", string.IsNullOrWhiteSpace(model.Designation) ? (object)DBNull.Value : model.Designation),
                new SqlParameter("@ResumePath", string.IsNullOrWhiteSpace(model.ResumePath) ? (object)DBNull.Value : model.ResumePath),
                new SqlParameter("@Message", string.IsNullOrWhiteSpace(model.Message) ? (object)DBNull.Value : model.Message),
                new SqlParameter("@NotRobot", model.NotRobot),
                new SqlParameter("@IPAddress", string.IsNullOrWhiteSpace(model.IPAddress) ? (object)DBNull.Value : model.IPAddress),
            };

            return GetDataSet("sp_AddWorkWithUsEnquiry", sqlParams).Tables[0];
        }
    }
}
