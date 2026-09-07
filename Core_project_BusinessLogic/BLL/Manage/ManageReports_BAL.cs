using System.Data;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic;

public class ManageReports_BAL : ManageReports_DAL
{
    public ManageReports_BAL(IConfiguration configuration)
        : base(configuration)
    {
    }


    public int Report_Category_Manage_BAL(Report_Category_Master obj, int userid, int manage_status)
    {
        int Id = 0;
        DataTable dt = new();
        try
        {
            dt = Report_Category_Manage_DAL(obj, userid, manage_status);
            if (dt.Rows[0]["Id"] != DBNull.Value)
            {
                Id = Convert.ToInt32(dt.Rows[0]["Id"].ToString());
            }
            return Id;
        }
        catch (System.Exception)
        {
            throw;
        }

    }


    public List<Report_Category_Master> Report_categories_List_BAL(int parent_id)
    {
        List<Report_Category_Master> obj = new();
        DataTable dt = new();
        try
        {
            dt = Report_categories_List_DAL(parent_id);
            obj.Add(new Report_Category_Master { Title = "Select", Parent_id = 0 });
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    obj.Add(new Report_Category_Master
                    {
                        Title = row["Title"].ToString(),
                        Parent_id = Convert.ToInt32(row["Parent_Id"])
                    });
                }
            }

            return obj;
        }
        catch (System.Exception)
        {
            throw;
        }

    }

}