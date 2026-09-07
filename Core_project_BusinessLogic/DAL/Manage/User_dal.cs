using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core_project_BusinessLogic;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Extensions.Configuration;
using TIC_CMS_BusinessLogic.Entity;
using Microsoft.Data.SqlClient;




namespace Core_project_BusinessLogic.DAL
{
    public class User_dal : DBHelper
    {

        public User_dal(IConfiguration configuration)
  : base(configuration) //  call base class constructor 
        {
        }





        public DataTable Insert_Users_DAL(User f)
        {
            SqlParameter[] param =
            {
                new("@username", f.username ?? (object)DBNull.Value),
                new("@Password", f.password ?? (object)DBNull.Value),
                new("@SecretKey", (object?)f.secret ?? DBNull.Value),
                new("@Email", (object?)f.Email ?? DBNull.Value),
                new("@PhoneNo", (object?)f.PhoneNo ?? DBNull.Value),
                new("@CreatedBy", (object?)f.CreatedBy ?? DBNull.Value),
                new("@UserType_Id", (object?)f.UserType_Id ?? DBNull.Value),
            };

            return GetDataSet("InsertUsers", param).Tables[0];
        }


        public DataTable Update_Users_DAL(UserEdit f)
        {
            SqlParameter[] param =
            {
                new("@Id", (object?)f.user_id ?? DBNull.Value),
                new("@username", f.username ?? (object)DBNull.Value),
                new("@Password", f.password ?? (object)DBNull.Value),
                new("@Email", (object?)f.Email ?? DBNull.Value),
                new("@PhoneNo", (object?)f.PhoneNo ?? DBNull.Value),
                new("@UpdatedBy", (object?)f.CreatedBy ?? DBNull.Value),
                new("@UserType_Id", (object?)f.UserType_Id ?? DBNull.Value),
            };

            return GetDataSet("UpdateUsers", param).Tables[0];
        }


        public DataTable GetUserById_DAL(int user_id)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@Id", user_id)
            };

            return GetDataSet("GetUserById", sqlParams).Tables[0];
        }

        public DataTable Users_Login_DAL(string username)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@username", username.Trim() )
            };

            return GetDataSet("Users_Login", sqlParams).Tables[0];
        }
        
        // public DataTable Users_Login_DAL(User t)
        // {
        //     SqlParameter[] sqlParams =
        //     {
        //         new SqlParameter("@username", t.username.Trim() ),
        //         new SqlParameter("@Password", t.password.Trim() )
        //     };

        //     return GetDataSet("Users_Login", sqlParams).Tables[0];
        // }

        public DataTable GetUserSecret_DAL(string username)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@username", username.Trim() )
            };

            return GetDataSet("GetUserSecret", sqlParams).Tables[0];
        }

        public DataTable GetUser_DAL()
        {
            return GetDataSet("GetUser").Tables[0];
        }


        public void UpdateUserSecret_dal(string email, string secret)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@username", email),
                new SqlParameter("@secretkey", secret)
            };

            GetDataSet("UpdateUserSecret", param);
        }

        public void Disable2FA_dal(string username)
        {
            SqlParameter[] param =
            {
               new SqlParameter("@username", username)
            };

            GetDataSet("Disable2FA", param);
        }

        public DataTable GetLoginLogs_dal(string username)
        {
            SqlParameter[] sqlParams =
            {
                new SqlParameter("@email", username.Trim() )
            };

            return GetDataSet("GetLoginLogs", sqlParams).Tables[0];
        }

        public void InsertLoginLog_DAL(string email, string ip, string status)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@Email", email),
                new SqlParameter("@IPAddress", ip),
                new SqlParameter("@Status", status)
            };

            ExecuteNonQuery("InsertLoginLog", p);
        }


        public List<UserType> UserType_Drowpdown_dal()
        {
            DataTable dt = GetDataSet("UserTypes_Drowpdown").Tables[0];
            var list = new List<UserType>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(Map(row));
            }
            return list;
        }

        private UserType Map(DataRow row)
        {
            return new UserType
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = row["Name"].ToString()
            };
        }


    }
}
