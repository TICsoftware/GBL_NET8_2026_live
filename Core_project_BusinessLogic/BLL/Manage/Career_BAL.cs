using System.Data;
using System.Security.Cryptography;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic;

public class Career_BAL : Career_DAL
{
    public Career_BAL(IConfiguration configuration)
        : base(configuration)
    {
    }
    // You can add languageMaster-specific logic here if needed
    public int Job_careers_Insert_BAL(CareerMaster_CMS objJob, int userid)
    {
        DataTable dt = new();
        int job_id = 0;
        try
        {
            dt = Job_Career_Insert_DAL(objJob, userid);
            if (dt.Rows.Count > 0 && dt.Rows[0]["Job_Id"] != DBNull.Value && dt.Rows[0]["Job_Id"].ToString() != "")
            {
                job_id = Convert.ToInt32(dt.Rows[0]["Job_Id"].ToString());
            }

            return job_id;
        }
        catch
        {
            throw;
        }
    }

    public Job_List Job_Career_list_BAL(int status, int pageno, int pagesize = 20, string? searchtext = "")
    {
        DataSet ds = new();
        Job_List obj = new();
        try
        {
            ds = Job_Career_list_DAL(status, pageno, pagesize, searchtext);
            if (ds.Tables[0].Rows.Count > 0)
            {
                obj.jobList = [];
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    obj.jobList.Add(new CareerMaster_CMS()
                    {
                        Job_Id = Convert.ToInt32(dr["Job_Id"].ToString()),
                        Role = dr["Role"].ToString(),
                        Education = dr["Education"].ToString(),
                        Experience = dr["Experience"].ToString(),
                        Job_Description = dr["Job_Description"].ToString(),
                        Location = dr["Location"].ToString(),
                        Salary_range = dr["Salary_range"].ToString(),
                        About_the_Role = dr["About_the_Role"].ToString(),
                        Workmode = dr["Workmode"].ToString(),
                        Expiry_date = dr["Expiry_date"] == DBNull.Value ? null : Convert.ToDateTime(dr["Expiry_date"].ToString()),
                        Created_date = Convert.ToDateTime(dr["Created_Date"].ToString())
                    });
                }
                obj.TotalRecords = Convert.ToInt32(ds.Tables[1].Rows[0]["total_count"].ToString());
            }
            else
                obj.TotalRecords = 0;
            return obj;
        }
        catch
        {
            throw;
        }
    }

    public CareerMaster_CMS Job_Career_Get_BAL(int Job_Id)
    {
        DataTable dt = new();
        CareerMaster_CMS? obj = new();
        try
        {
            dt = Job_Career_Get_DAL(Job_Id);
            if (dt.Rows.Count > 0)
            {
                obj.Job_Id = Convert.ToInt32(dt.Rows[0]["Job_Id"].ToString());
                obj.Role = dt.Rows[0]["Role"].ToString();
                obj.Education = dt.Rows[0]["Education"].ToString();
                obj.Experience = dt.Rows[0]["Experience"].ToString();
                obj.Job_Description = dt.Rows[0]["Job_Description"].ToString();
                obj.Location = dt.Rows[0]["Location"].ToString();
                obj.Salary_range = dt.Rows[0]["Salary_range"].ToString();
                obj.About_the_Role = dt.Rows[0]["About_the_Role"].ToString();
                obj.Workmode = dt.Rows[0]["Workmode"].ToString();
                obj.Expiry_date = dt.Rows[0]["Expiry_date"] == DBNull.Value ? null : Convert.ToDateTime(dt.Rows[0]["Expiry_date"].ToString());
            }

            return obj;
        }
        catch
        {
            throw;
        }
    }
    public void Update_Job_Status_BAL(int Job_Id, int status)
    {
        Update_Job_Status_DAL(Job_Id, status);
    }



    public void Job_careers_Update_BAL(CareerMaster_CMS objJob, int userid)
    {
        try
        {
            Job_Career_Update_DAL(objJob, userid);
        }
        catch
        {
            throw;
        }
    }


}