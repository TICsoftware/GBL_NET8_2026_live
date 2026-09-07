
using System.Data;
using Core_project_BusinessLogic.Entity; 
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic;

public class Career_DAL : DBHelper
{
    public Career_DAL(IConfiguration configuration)
   : base(configuration) //  call base class constructor 
    {
    }

    protected DataTable Job_Career_Insert_DAL(CareerMaster_CMS objJobs, int userid)
    {
        DataTable dt = new();
        var Sqlparam = new List<SqlParameter>();

        try
        {
            Sqlparam.Add(new SqlParameter("@Role", objJobs.Role ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Education", objJobs.Education ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Experience", objJobs.Experience ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Job_Description", objJobs.Job_Description ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Location", objJobs.Location ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Salary_range", objJobs.Salary_range ?? (object)DBNull.Value)); 
            Sqlparam.Add(new SqlParameter("@About_the_Role", objJobs.About_the_Role ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Workmode", objJobs.Workmode ?? (object)DBNull.Value));
            if (objJobs.Expiry_date.HasValue)
            {
                Sqlparam.Add(new SqlParameter("@Expiry_date", objJobs.Expiry_date));
            }
            else
            {
                Sqlparam.Add(new SqlParameter("@Expiry_date", DBNull.Value));
            }
            Sqlparam.Add(new SqlParameter("@user_id", userid.ToString()));
            dt = GetDataSet("Job_careers_Insert", Sqlparam.ToArray()).Tables[0];
            return dt;
        }
        catch
        {
            throw;
        }
    }

    public DataSet Job_Career_list_DAL(int status, int pageno, int pagesize, string? searchtext = "")
    {
        DataSet ds = new();
        try
        {
            if (string.IsNullOrWhiteSpace(searchtext))
            {
                ds = GetDataSet("CMS_Job_careers_List", "@status", status.ToString(), "@pageno", pageno.ToString(),"@pagesize", pagesize.ToString());
            }
            else
            {
                ds = GetDataSet("CMS_Job_careers_List", "@status", status.ToString(), "@pageno", pageno.ToString(), "@searchkeywords", searchtext,"@pagesize", pagesize.ToString());
            }
            return ds;
        }
        catch
        {
            throw;
        }
    }

    public DataTable Job_Career_Get_DAL(int job_id)
    {
        DataTable dt = new();
        try
        {
            dt = GetDataSet("Job_Career_Get", "@job_id", job_id.ToString()).Tables[0];
            return dt;
        }
        catch
        {
            throw;
        }
    }

    public void Update_Job_Status_DAL(int Job_Id, int status)
    {
        try
        {
            SQLInsert_Update_Delete_Data("Update_Job_Status", "@Job_Id", Job_Id.ToString(), "@Status", status.ToString());
        }
        catch
        {
            throw;
        }
    }

    protected void Job_Career_Update_DAL(CareerMaster_CMS objJobs, int userid)
    {

        var Sqlparam = new List<SqlParameter>();

        try
        {
            Sqlparam.Add(new SqlParameter("@job_id", objJobs.Job_Id));
            Sqlparam.Add(new SqlParameter("@Role", objJobs.Role ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Education", objJobs.Education ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Experience", objJobs.Experience ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Job_Description", objJobs.Job_Description ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Location", objJobs.Location ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Salary_range", objJobs.Salary_range ?? (object)DBNull.Value)); 
            Sqlparam.Add(new SqlParameter("@About_the_Role", objJobs.About_the_Role ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Workmode", objJobs.Workmode ?? (object)DBNull.Value));
            if (objJobs.Expiry_date.HasValue)
            {
                Sqlparam.Add(new SqlParameter("@Expiry_date", objJobs.Expiry_date));
            }
            else
            {
                Sqlparam.Add(new SqlParameter("@Expiry_date", DBNull.Value));
            }
            Sqlparam.Add(new SqlParameter("@user_id", userid.ToString()));
            SQLInsert_Update_Delete_Data("Job_careers_Update", Sqlparam.ToArray());
        }
        catch
        {
            throw;
        }
    }

}