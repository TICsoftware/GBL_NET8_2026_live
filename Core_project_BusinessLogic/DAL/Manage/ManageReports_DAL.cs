
using System.Data;
using Core_project_BusinessLogic.Entity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic;

public class ManageReports_DAL : DBHelper
{
    public ManageReports_DAL(IConfiguration configuration)
      : base(configuration) //  call base class constructor 
    {
    }

    protected DataTable Report_Category_Manage_DAL(Report_Category_Master obj, int userid, int manage_status)
    {
        DataTable dt = new();
        var Sqlparam = new List<SqlParameter>();

        try
        {
            Sqlparam.Add(new SqlParameter("@Title", obj.Title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@mail_title", obj.Mail_title ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@Parent_Id", obj.Parent_id));
            Sqlparam.Add(new SqlParameter("@user_id", userid.ToString()));
            if (manage_status == 1)
            {
                dt = GetDataSet("Report_categories_Insert", Sqlparam.ToArray()).Tables[0];
            }
            else
            {
                dt = GetDataSet("Report_categories_Update", Sqlparam.ToArray()).Tables[0];
            }
            return dt;
        }
        catch
        {
            throw;
        }
    }

    protected DataTable Report_Category_Get_DAL(string searchtext, int parent_id = 0, int status = 2)
    {
        DataTable dt = new();
        var Sqlparam = new List<SqlParameter>();
        try
        {
            if (!string.IsNullOrWhiteSpace(searchtext))
            {
                Sqlparam.Add(new SqlParameter("@searchtext", searchtext));
            }
            Sqlparam.Add(new SqlParameter("@parent_id", parent_id.ToString() ?? (object)DBNull.Value));
            Sqlparam.Add(new SqlParameter("@status", status.ToString()));
            dt = GetDataSet("Report_categories_Get", Sqlparam.ToArray()).Tables[0];
            return dt;
        }
        catch
        {
            throw;
        }
    }

    protected DataTable Report_categories_List_DAL(int parent_id)
    {
        DataTable dt = new();
        try
        {
            dt = GetDataSet("Report_categories_List","@parent_id",parent_id.ToString()).Tables[0];
            return dt;
        }
        catch
        {
            throw;
        }
    }
}
