using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity.Manage;

namespace Core_project_BusinessLogic.BLL;

public class User_bal
{
    private readonly User_dal _dal;
    public User_bal(IConfiguration configuration)
    {
        _dal = new User_dal(configuration);
    }




    public int InsertUsers_bal(User template)
    {
        var dt = _dal.Insert_Users_DAL(template);

        // Return 0 if no data (you can change default)
        if (dt?.Rows.Count > 0 && dt.Columns.Contains("Status"))
        {
            return Convert.ToInt32(dt.Rows[0]["Status"]);
        }

        return 0;
    }

    public int UpdateUsers_bal(UserEdit template)
    {
        var dt = _dal.Update_Users_DAL(template);

        // Return 0 if no data (you can change default)
        if (dt?.Rows.Count > 0 && dt.Columns.Contains("Status"))
        {
            return Convert.ToInt32(dt.Rows[0]["Status"]);
        }

        return 0;
    }


    // public void InsertUsers_bal(User template)
    // {
    //     _dal.Insert_Users_DAL(template);
    // }


    public User Users_Login_bal(string username)
    {
        var dt = _dal.Users_Login_DAL(username);

        if (dt?.Rows.Count > 0 && dt.Rows[0].Field<int>("Id") > 0)
        {
            var row = dt.Rows[0];

            return new User
            {
                user_id = row.Field<int>("Id"),
                username = row.Field<string>("Username"),
                password = row.Field<string>("Password"),
                secret = row.Field<string>("SecretKey"),
                UserType_Id = row.Field<int>("UserType"),
                Is2FAEnabled = row.Field<bool>("Is2FAEnabled")
            };
        }

        return null;
    }

    // public User Users_Login_bal(User t)
    // {
    //     var dt = _dal.Users_Login_DAL(t);

    //     if (dt?.Rows.Count > 0 && dt.Rows[0].Field<int>("Id") > 0)
    //     {
    //         var row = dt.Rows[0];

    //         return new User
    //         {
    //             user_id = row.Field<int>("Id"),
    //             username = row.Field<string>("Username"),
    //             password = row.Field<string>("Password"),
    //             secret = row.Field<string>("SecretKey"),
    //             UserType_Id = row.Field<int>("UserType"),
    //             Is2FAEnabled = row.Field<bool>("Is2FAEnabled")
    //         };
    //     }

    //     return null;
    // }


    public UserEdit GetUserById_bal(int user_id)
    {
        try
        {
            var dt = _dal.GetUserById_DAL(user_id);

            if (dt == null || dt.Rows.Count == 0)
                return null;

            var row = dt.Rows[0];

            return new UserEdit
            {
                user_id = row.Field<int>("Id"),
                username = row.Field<string>("Username"),
                password = row.Field<string>("Password"),
                UserType_Id = row.Field<int>("UserType"),
                Email = row.Field<string>("Email"),
                PhoneNo = row.Field<string>("PhoneNo")
            };
        }
        catch (Exception ex)
        {
            throw; // ✅ VERY IMPORTANT (preserves stack trace)
        }
    }


    // public string Users_Login_bal(User t)
    // {
    //     string retval = "";
    //     DataTable dt = _dal.Users_Login_DAL(t);

    //     foreach (DataRow row in dt.Rows)
    //     {
    //         retval = row["SecretKey"].ToString();
    //     }

    //     return retval;
    // }




    public string GetUserSecret_bal(string username)
    {
        string retval = "";
        DataTable dt = _dal.GetUserSecret_DAL(username);

        foreach (DataRow row in dt.Rows)
        {
            retval = row["SecretKey"].ToString();
        }

        return retval;
    }


    public List<User> GetUser_bal()
    {
        List<User> accounts = new List<User>();

        DataTable dt = _dal.GetUser_DAL();

        foreach (DataRow row in dt.Rows)
        {
            User obj = new User();

            obj.user_id = Convert.ToInt32(row["Id"]);
            obj.password = row["password"].ToString();
            obj.username = row["username"].ToString();
            obj.secret = row["SecretKey"].ToString();

            obj.Is2FAEnabled = Convert.ToBoolean(row["Is2FAEnabled"]);

            if (row["LastLogin"] != DBNull.Value)
                obj.LastLogin = Convert.ToDateTime(row["LastLogin"]);

            accounts.Add(obj);
        }

        return accounts;
    }


    public void UpdateUserSecret_bal(string email, string secret)
    {
        _dal.UpdateUserSecret_dal(email, secret);
    }


    public void Disable2FA_bal(string username)
    {
        _dal.Disable2FA_dal(username);
    }

    public List<LoginLogsModel> GetLoginLogs_bal(string email)
    {
        List<LoginLogsModel> logs = new List<LoginLogsModel>();

        DataTable dt = _dal.GetLoginLogs_dal(email);

        foreach (DataRow row in dt.Rows)
        {
            LoginLogsModel obj = new LoginLogsModel();

            obj.Email = row["Email"].ToString();
            obj.IPAddress = row["IPAddress"].ToString();
            obj.LoginDate = Convert.ToDateTime(row["LoginDate"]);
            obj.Status = row["Status"].ToString();

            logs.Add(obj);
        }

        return logs;
    }


    public void InsertLoginLog_bal(string email, string ip, string status)
    {
        _dal.InsertLoginLog_DAL(email, ip, status);
    }

    public List<UserType> UserType_Drowpdown_bal()
    {
        return _dal.UserType_Drowpdown_dal();
    }


}
