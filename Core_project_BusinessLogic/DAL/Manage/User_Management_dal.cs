using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core_project_BusinessLogic;
using Microsoft.Extensions.Configuration;




namespace Core_project_BusinessLogic.DAL
{
    public class User_Management_dal : DBHelper
    {

         public User_Management_dal(IConfiguration configuration)
   : base(configuration) //  call base class constructor 
    {
    }

        #region "CMS User management"

        // protected User_Master login_check_dal(string loginname, string password, string ipaddress)
        // {
        //     User_Master user_Master = new User_Master();
        //     try
        //     {
        //         using (SqlConnection cn = new SqlConnection(TIC_DbHelper.Project_Config.GetConnectionString()))
        //         {
        //             string strSPName = "login_check";
        //             SqlCommand command = new SqlCommand(strSPName, cn);
        //             command.CommandType = CommandType.StoredProcedure;  
        //             command.Parameters.Add(new SqlParameter("@loginname", loginname.Trim()));
        //             command.Parameters.Add(new SqlParameter("@password", GetMD5String(password.Trim())));
        //             command.Parameters.Add(new SqlParameter("@ipaddress", ipaddress.Trim()));

        //             cn.Open();
        //             try
        //             {
        //                 SqlDataReader reader = command.ExecuteReader();
        //                 if (reader.HasRows)
        //                 {
        //                     while (reader.Read())
        //                     {
        //                         user_Master.usr_id = Convert.ToInt32(reader["usr_id"]);
        //                         user_Master.usr_roleid = Convert.ToInt32(reader["usr_roleid"]);
        //                         user_Master.usr_login = reader["usr_login"].ToString().Trim();
        //                         //user_Master.usr_password = Convert.ToInt32(reader["usr_password"]);
        //                         //user_Master.usr_status = Convert.ToInt32(reader["usr_status"]);
        //                         user_Master.usr_firstname = reader["usr_firstname"].ToString();
        //                         user_Master.usr_lastname = reader["usr_lastname"].ToString();
        //                         user_Master.usr_emailid = reader["usr_emailid"].ToString();
        //                         user_Master.role_taskids = reader["role_taskids"].ToString();
        //                         //usr_id,usr_roleid,usr_login,usr_password,usr_status,role_taskids
        //                     }

        //                 }
        //                 return user_Master;
        //             }
        //             catch (Exception ex)
        //             {
        //                 throw ex;
        //             }
        //             finally
        //             {
        //                 cn.Close();
        //             }
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        // }
        #endregion

      



      


        protected void Manage_login_sessions_DAL(int emp_id, string email_id, string session)
        {
            SQLInsert_Update_Delete_Data("insert_user_session", "@usr_id", emp_id.ToString(), "@emp_emailid", email_id.ToString(), "@session_name", session);
        }


        protected DataSet Get_employee_session_DAL(int emp_id, string email_id)
        {
            return GetDataSet("check_user_session", "@usr_id", emp_id.ToString(), "@emp_emailid", email_id.ToString());
        }




    }
}
