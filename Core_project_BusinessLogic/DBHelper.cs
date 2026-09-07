using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
 

namespace Core_project_BusinessLogic
{
    public abstract class DBHelper : IDisposable
    {
        public static string ? strCon;
       

        public DBHelper(IConfiguration configuration)
        {             
            strCon = CryptoEngine.Decrypt(configuration.GetConnectionString("constr")!); 
        }


        #region "Data Manupulation"
        /// <summary>
        /// This function is use for Insert Data, it will return Indentity Column value, I has 3 parameters first one is storeprocedure name and secound one is Outputput parameter name and third one is array of Inpur sqlParameter
        /// </summary>
        /// <param name="spName">Pass StoreProcedure name</param>
        /// <param name="outputPara">Pass Output Parameter name like @return_ID</param>
        /// <param name="parameters">Pass Array of SqlParameter</param>
        /// <returns></returns>
        protected int SqlInsertReturnIdentity_withSP(string spName, string outputPara, params SqlParameter[] parameters)
        {
            using (SqlConnection cn = new SqlConnection(strCon))
            {
                try
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(spName, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        int i = 0;
                        for (i = 0; i <= parameters.Length - 1; i++)
                        {
                            cmd.Parameters.Add(parameters[i]);
                            //cmd.Parameters(parameters(i))
                        }
                        //The output and return parameters must be created as objects
                        SqlParameter Pid = new SqlParameter(outputPara, SqlDbType.Int);
                        Pid.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(Pid);
                        cmd.ExecuteNonQuery();
                        return Convert.ToInt32(Pid.Value);
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.ErrorCode == 2627)
                        throw new Exception("Already exist");
                    else
                         throw new Exception("error", ex);
                }
                catch (Exception ex)
                {
                     throw new Exception("error", ex);
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                    {
                        cn.Close();
                    }
                }
            }
        }

        /// <summary>
        /// This function is use for Insert, Update and Delete data from Database table. It does not return anythink
        /// </summary>
        /// <param name="srtSP_Name">Pass storeprocedure name</param>
        /// <param name="parameters">Pass array of SqlParameter</param>
        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, SqlParameter[] parameters)
        {
            object obj ;
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {
                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            for (int i = 0; i <= parameters.Length - 1; i++)
                            {
                                cmd.Parameters.Add(parameters[i]);
                                //cmd.Parameters(parameters(i))
                            }
                            obj = cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        /// <summary>
        /// This function is use for Insert, Update and Delete data from Database table. It returns true false
        /// </summary>
        /// <param name="srtSP_Name">Pass storeprocedure name</param>
        /// <param name="parameters">Pass array of SqlParameter</param>
        /// <returns></returns>
        protected bool SQLInsert_Update_Delete_Data_Return_Bool(string srtSP_Name, SqlParameter[] parameters)
        {
            object obj ;
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {
                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            for (int i = 0; i <= parameters.Length - 1; i++)
                            {
                                cmd.Parameters.Add(parameters[i]);
                                //cmd.Parameters(parameters(i))
                            }
                            obj = cmd.ExecuteNonQuery();
                            return true;
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }
    private string Decrypt(string stringToDecrypt, string sEncryptionKey)
        {
            byte[] key = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[stringToDecrypt.Length];
            try
            {
                key = Encoding.UTF8.GetBytes(sEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(stringToDecrypt);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string Encrypt(string stringToEncrypt, string SEncryptionKey)
        {
            byte[] key = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray;

            try
            {
                key = Encoding.UTF8.GetBytes(SEncryptionKey.Substring(0, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
      
        protected string Encrypt_String(string strQueryString)
        {
            try
            {
                return Encrypt(strQueryString, "!#$a91?8");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected string Decrypt_String(string strQueryString)
        {
            try
            {
                return Decrypt(strQueryString.Replace(" ", "+"), "!#$a91?8");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      
        /// <summary>
        /// This function is use for Insert, Update and Delete data from Database table. It does not return anythink
        /// </summary>
        /// <param name="srtSP_Name">Pass storeprocedure name</param>
        protected void SQLInsert_Update_Delete_Data(string srtSP_Name)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        /// <summary>
        /// This function is use for Insert, Update and Delete data from Database table. It does not return anythink
        /// </summary>
        /// <param name="srtSP_Name">Pass storeprocedure name</param>
        /// <param name="dbparameter1">Pass database parameter name</param>
        /// <param name="usparameter1">Pass value which you want to insert</param>
        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
            string dbparameter3, string usparameter3)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
            string dbparameter3, string usparameter3,
            string dbparameter4, string usparameter4)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
             string dbparameter3, string usparameter3,
             string dbparameter4, string usparameter4,
             string dbparameter5, string usparameter5)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
      string dbparameter3, string usparameter3,
      string dbparameter4, string usparameter4,
      string dbparameter5, string usparameter5,
      string dbparameter6, string usparameter6)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }


        protected void SQLInsert_Update_Delete_Data(string srtSP_Name,
        string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
        string dbparameter3, string usparameter3, string dbparameter4, string usparameter4,
        string dbparameter5, string usparameter5, string dbparameter6, string usparameter6,
        string dbparameter7, string usparameter7)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }


        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
string dbparameter3, string usparameter3,
string dbparameter4, string usparameter4,
string dbparameter5, string usparameter5,
string dbparameter6, string usparameter6,
     string dbparameter7, string usparameter7,
            string dbparameter8, string usparameter8
            )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            cmd.Parameters.Add(new SqlParameter(dbparameter8, usparameter8));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }


        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
string dbparameter3, string usparameter3,
string dbparameter4, string usparameter4,
string dbparameter5, string usparameter5,
string dbparameter6, string usparameter6,
string dbparameter7, string usparameter7,
     string dbparameter8, string usparameter8,
            string dbparameter9, string usparameter9
     )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            cmd.Parameters.Add(new SqlParameter(dbparameter8, usparameter8));
                            cmd.Parameters.Add(new SqlParameter(dbparameter9, usparameter9));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }


        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
string dbparameter3, string usparameter3,
string dbparameter4, string usparameter4,
string dbparameter5, string usparameter5,
string dbparameter6, string usparameter6,
string dbparameter7, string usparameter7,
string dbparameter8, string usparameter8,
     string dbparameter9, string usparameter9,
            string dbparameter10, string usparameter10
)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            cmd.Parameters.Add(new SqlParameter(dbparameter8, usparameter8));
                            cmd.Parameters.Add(new SqlParameter(dbparameter9, usparameter9));
                            cmd.Parameters.Add(new SqlParameter(dbparameter10, usparameter10));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
string dbparameter3, string usparameter3,
string dbparameter4, string usparameter4,
string dbparameter5, string usparameter5,
string dbparameter6, string usparameter6,
string dbparameter7, string usparameter7,
string dbparameter8, string usparameter8,
string dbparameter9, string usparameter9,
string dbparameter10, string usparameter10,
string dbparameter11, string usparameter11
)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            cmd.Parameters.Add(new SqlParameter(dbparameter8, usparameter8));
                            cmd.Parameters.Add(new SqlParameter(dbparameter9, usparameter9));
                            cmd.Parameters.Add(new SqlParameter(dbparameter10, usparameter10));
                            cmd.Parameters.Add(new SqlParameter(dbparameter11, usparameter11));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
 string dbparameter3, string usparameter3,
 string dbparameter4, string usparameter4,
 string dbparameter5, string usparameter5,
 string dbparameter6, string usparameter6,
 string dbparameter7, string usparameter7,
 string dbparameter8, string usparameter8,
 string dbparameter9, string usparameter9,
 string dbparameter10, string usparameter10,
 string dbparameter11, string usparameter11,
 string dbparameter12, string usparameter12
 )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            cmd.Parameters.Add(new SqlParameter(dbparameter8, usparameter8));
                            cmd.Parameters.Add(new SqlParameter(dbparameter9, usparameter9));
                            cmd.Parameters.Add(new SqlParameter(dbparameter10, usparameter10));
                            cmd.Parameters.Add(new SqlParameter(dbparameter11, usparameter11));
                            cmd.Parameters.Add(new SqlParameter(dbparameter12, usparameter12));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }


        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
  string dbparameter3, string usparameter3,
  string dbparameter4, string usparameter4,
  string dbparameter5, string usparameter5,
  string dbparameter6, string usparameter6,
  string dbparameter7, string usparameter7,
  string dbparameter8, string usparameter8,
  string dbparameter9, string usparameter9,
  string dbparameter10, string usparameter10,
  string dbparameter11, string usparameter11,
  string dbparameter12, string usparameter12,
  string dbparameter13, string usparameter13
  )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {
                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            cmd.Parameters.Add(new SqlParameter(dbparameter8, usparameter8));
                            cmd.Parameters.Add(new SqlParameter(dbparameter9, usparameter9));
                            cmd.Parameters.Add(new SqlParameter(dbparameter10, usparameter10));
                            cmd.Parameters.Add(new SqlParameter(dbparameter11, usparameter11));
                            cmd.Parameters.Add(new SqlParameter(dbparameter12, usparameter12));
                            cmd.Parameters.Add(new SqlParameter(dbparameter13, usparameter13));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
        string dbparameter3, string usparameter3,
        string dbparameter4, string usparameter4,
        string dbparameter5, string usparameter5,
        string dbparameter6, string usparameter6,
        string dbparameter7, string usparameter7,
        string dbparameter8, string usparameter8,
        string dbparameter9, string usparameter9,
        string dbparameter10, string usparameter10,
        string dbparameter11, string usparameter11,
        string dbparameter12, string usparameter12,
        string dbparameter13, string usparameter13,
        string dbparameter14, string usparameter14
        )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {
                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            cmd.Parameters.Add(new SqlParameter(dbparameter8, usparameter8));
                            cmd.Parameters.Add(new SqlParameter(dbparameter9, usparameter9));
                            cmd.Parameters.Add(new SqlParameter(dbparameter10, usparameter10));
                            cmd.Parameters.Add(new SqlParameter(dbparameter11, usparameter11));
                            cmd.Parameters.Add(new SqlParameter(dbparameter12, usparameter12));
                            cmd.Parameters.Add(new SqlParameter(dbparameter13, usparameter13));
                            cmd.Parameters.Add(new SqlParameter(dbparameter14, usparameter14));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
        string dbparameter3, string usparameter3,
        string dbparameter4, string usparameter4,
        string dbparameter5, string usparameter5,
        string dbparameter6, string usparameter6,
        string dbparameter7, string usparameter7,
        string dbparameter8, string usparameter8,
        string dbparameter9, string usparameter9,
        string dbparameter10, string usparameter10,
        string dbparameter11, string usparameter11,
        string dbparameter12, string usparameter12,
        string dbparameter13, string usparameter13,
        string dbparameter14, string usparameter14,
        string dbparameter15, string usparameter15
        )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {
                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            cmd.Parameters.Add(new SqlParameter(dbparameter8, usparameter8));
                            cmd.Parameters.Add(new SqlParameter(dbparameter9, usparameter9));
                            cmd.Parameters.Add(new SqlParameter(dbparameter10, usparameter10));
                            cmd.Parameters.Add(new SqlParameter(dbparameter11, usparameter11));
                            cmd.Parameters.Add(new SqlParameter(dbparameter12, usparameter12));
                            cmd.Parameters.Add(new SqlParameter(dbparameter13, usparameter13));
                            cmd.Parameters.Add(new SqlParameter(dbparameter14, usparameter14));
                            cmd.Parameters.Add(new SqlParameter(dbparameter15, usparameter15));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected void SQLInsert_Update_Delete_Data(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
        string dbparameter3, string usparameter3,
        string dbparameter4, string usparameter4,
        string dbparameter5, string usparameter5,
        string dbparameter6, string usparameter6,
        string dbparameter7, string usparameter7,
        string dbparameter8, string usparameter8,
        string dbparameter9, string usparameter9,
        string dbparameter10, string usparameter10,
        string dbparameter11, string usparameter11,
        string dbparameter12, string usparameter12,
        string dbparameter13, string usparameter13,
        string dbparameter14, string usparameter14,
        string dbparameter15, string usparameter15,
        string dbparameter16, string usparameter16
        )
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {
                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            cmd.Parameters.Add(new SqlParameter(dbparameter8, usparameter8));
                            cmd.Parameters.Add(new SqlParameter(dbparameter9, usparameter9));
                            cmd.Parameters.Add(new SqlParameter(dbparameter10, usparameter10));
                            cmd.Parameters.Add(new SqlParameter(dbparameter11, usparameter11));
                            cmd.Parameters.Add(new SqlParameter(dbparameter12, usparameter12));
                            cmd.Parameters.Add(new SqlParameter(dbparameter13, usparameter13));
                            cmd.Parameters.Add(new SqlParameter(dbparameter14, usparameter14));
                            cmd.Parameters.Add(new SqlParameter(dbparameter15, usparameter15));
                            cmd.Parameters.Add(new SqlParameter(dbparameter16, usparameter16));
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException ex)
                        {
                            if (ex.ErrorCode == 2627)
                                throw new Exception("Already exist");
                            else
                                 throw new Exception("error", ex);
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }
        #endregion

        #region "Dispose method"
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }

        ~DBHelper()
        {
            // Simply call Dispose(False).
            Dispose(false);
        }
        #endregion
    
           #region "Display Methods"

        #region "Scalar Method"
        protected object ExecScalar(string srtSP_Name)
        {
            object obj;
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            obj = cmd.ExecuteScalar();
                            return obj;
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }
        protected object ExecScalar(string srtSP_Name, string dbparameter1, string usparameter1)
        {
            object obj ;
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            obj = cmd.ExecuteScalar();
                            return obj;
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }
        protected object ExecScalar(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2)
        {
            object obj ;
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            obj = cmd.ExecuteScalar();
                            return obj;
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected object ExecScalar(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
            string dbparameter3, string usparameter3)
        {
            object obj ;
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            obj = cmd.ExecuteScalar();
                            return obj;
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected object ExecScalar(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
       string dbparameter3, string usparameter3,
            string dbparameter4, string usparameter4)
        {
            object obj ;
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            obj = cmd.ExecuteScalar();
                            return obj;
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected object ExecScalar(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
string dbparameter3, string usparameter3,
    string dbparameter4, string usparameter4,
            string dbparameter5, string usparameter5)
        {
            object obj ;
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            obj = cmd.ExecuteScalar();
                            return obj;
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected object ExecScalar(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
        string dbparameter3, string usparameter3,
        string dbparameter4, string usparameter4,
        string dbparameter5, string usparameter5,
        string dbparameter6, string usparameter6)
        {
            object obj ;
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            obj = cmd.ExecuteScalar();
                            return obj;
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        protected object ExecScalar(string srtSP_Name, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2,
string dbparameter3, string usparameter3,
string dbparameter4, string usparameter4,
string dbparameter5, string usparameter5,
string dbparameter6, string usparameter6,
string dbparameter7, string usparameter7)
        {
            object obj ;
            try
            {
                using (SqlConnection cn = new SqlConnection(strCon))
                {
                    using (SqlCommand cmd = new SqlCommand(srtSP_Name, cn))
                    {

                        cn.Open();
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                            cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                            cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                            cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                            cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                            cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                            cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                            obj = cmd.ExecuteScalar();
                            return obj;
                        }
                        catch (Exception ex)
                        {
                            //cn.Close();
                             throw new Exception("error", ex);
                        }
                        finally
                        {
                            cn.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                 throw new Exception("error", ex);
            }
        }

        #endregion

        #region "Get Dataset"
        protected DataSet GetDataSet(string strSQL)
        {
            using (SqlConnection cn = new SqlConnection(strCon))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Specifies SP as Cmd object type
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            da.Dispose();
                            return ds;
                        }
                    }
                }
            }
        }
public void ExecuteNonQuery(string spName, SqlParameter[] parameters)
{
    using SqlConnection con = new SqlConnection(strCon);
    using SqlCommand cmd = new SqlCommand(spName, con);

    cmd.CommandType = CommandType.StoredProcedure;

    if (parameters != null)
        cmd.Parameters.AddRange(parameters);

    con.Open();
    cmd.ExecuteNonQuery();
}
        protected DataSet GetDataSet(string strSQL, params SqlParameter[] parameters)
        {
            using (SqlConnection cn =  new SqlConnection(strCon))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Specifies SP as Cmd object type

                    int i = 0;
                    for (i = 0; i <= parameters.Length - 1; i++)
                    {
                        cmd.Parameters.Add(parameters[i]);
                        //cmd.Parameters(parameters(i))
                    }
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            da.Dispose();
                            return ds;
                        }
                    }
                }
            }
        }
        
        protected DataSet GetDataSet(string strSQL, string dbparameter1, string usparameter1)
        {
            using (SqlConnection cn = new SqlConnection(strCon))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Specifies SP as Cmd object type
                    cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            da.Dispose();
                            return ds;
                        }
                    }
                }
            }
        }

        protected DataSet GetDataSet(string strSQL, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2)
        {
            using (SqlConnection cn = new SqlConnection(strCon))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Specifies SP as Cmd object type
                    cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                    cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            da.Dispose();
                            return ds;
                        }
                    }
                }
            }
        }

        protected DataSet GetDataSet(string strSQL, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2, string dbparameter3, string usparameter3)
        {
            using (SqlConnection cn = new SqlConnection(strCon))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Specifies SP as Cmd object type
                    cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                    cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                    cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            da.Dispose();
                            return ds;
                        }
                    }
                }
            }
        }

        protected DataSet GetDataSet(string strSQL, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2, string dbparameter3, string usparameter3, string dbparameter4, string usparameter4)
        {
            using (SqlConnection cn = new SqlConnection(strCon))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Specifies SP as Cmd object type
                    cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                    cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                    cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                    cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            da.Dispose();
                            return ds;
                        }
                    }
                }
            }
        }

        protected DataSet GetDataSet(string strSQL, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2, string dbparameter3, string usparameter3, string dbparameter4, string usparameter4, string dbparameter5, string usparameter5)
        {
            using (SqlConnection cn = new SqlConnection(strCon))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Specifies SP as Cmd object type
                    cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                    cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                    cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                    cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                    cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            da.Dispose();
                            return ds;
                        }
                    }
                }
            }
        }

        protected DataSet GetDataSet(string strSQL, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2, string dbparameter3, string usparameter3, string dbparameter4, string usparameter4, string dbparameter5, string usparameter5, string dbparameter6, string usparameter6)
        {
            using (SqlConnection cn = new SqlConnection(strCon))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Specifies SP as Cmd object type
                    cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                    cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                    cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                    cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                    cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                    cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            da.Dispose();
                            return ds;
                        }
                    }
                }
            }
        }

        protected DataSet GetDataSet(string strSQL, string dbparameter1, string usparameter1, string dbparameter2, string usparameter2, string dbparameter3, string usparameter3, string dbparameter4, string usparameter4, string dbparameter5, string usparameter5, string dbparameter6, string usparameter6, string dbparameter7, string usparameter7)
        {
            using (SqlConnection cn = new SqlConnection(strCon))
            {
                using (SqlCommand cmd = new SqlCommand(strSQL, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Specifies SP as Cmd object type
                    cmd.Parameters.Add(new SqlParameter(dbparameter1, usparameter1));
                    cmd.Parameters.Add(new SqlParameter(dbparameter2, usparameter2));
                    cmd.Parameters.Add(new SqlParameter(dbparameter3, usparameter3));
                    cmd.Parameters.Add(new SqlParameter(dbparameter4, usparameter4));
                    cmd.Parameters.Add(new SqlParameter(dbparameter5, usparameter5));
                    cmd.Parameters.Add(new SqlParameter(dbparameter6, usparameter6));
                    cmd.Parameters.Add(new SqlParameter(dbparameter7, usparameter7));
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            da.Fill(ds);
                            da.Dispose();
                            return ds;
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

    
    
    }

}