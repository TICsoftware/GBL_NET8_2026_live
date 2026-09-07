using System;
using System.Security.Cryptography;
using System.Text;


namespace Core_project_BusinessLogic
{
    public static class CryptoEngine
    {

        #region "Encrypt and Decript QueryString"

        public static string Encrypt(string stringToEncrypt)
        {
            EncryptionSettings objsettings = new EncryptionSettings();
            try
            {
                using Aes aes = Aes.Create();
                aes.KeySize = 256;
                aes.Key = Convert.FromBase64String(objsettings.Key);
                aes.IV = Convert.FromBase64String(objsettings.IV);

                // aes.GenerateKey();
                // aes.GenerateIV();  
                // string keyBase64 = Convert.ToBase64String(aes.Key);
                // string ivBase64 = Convert.ToBase64String(aes.IV);

                using var encryptor = aes.CreateEncryptor();
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                using var sw = new StreamWriter(cs);
                sw.Write(stringToEncrypt); sw.Close();


                string base64 = Convert.ToBase64String(ms.ToArray());

                // Make it URL safe
                string urlSafe = base64
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "$")
                    .Replace(".", "|");

                return urlSafe;

            }
            catch (Exception ex)
            {
                throw new Exception("cryptoerror", ex); ;
            }
        }

        public static string Decrypt(string stringToDecrypt)
        {
            EncryptionSettings objsettings = new EncryptionSettings();
            try
            {


                string base64 = stringToDecrypt
                 .Replace("-", "+")
                 .Replace("_", "/")
                 .Replace("$", "=")
                 .Replace("|", ".");

                using Aes aes = Aes.Create();
                aes.KeySize = 256;
                aes.Key = Convert.FromBase64String(objsettings.Key);
                aes.IV = Convert.FromBase64String(objsettings.IV);

                using var decryptor = aes.CreateDecryptor();
                var cipherBytes = Convert.FromBase64String(base64);

                using var ms = new MemoryStream(cipherBytes);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new Exception("cryptoerror", ex);
            }
        }

        #endregion


        #region "Dispose method"
        // public void Dispose()
        // {
        //     Dispose(true);
        //     GC.SuppressFinalize(this);
        // }

        // protected virtual void Dispose(bool disposing)
        // {
        //     if (disposing)
        //     {

        //     }
        // }


        // ~CryptoEngine()
        // {
        //     // Simply call Dispose(False).
        //     Dispose(false);
        // }


        #endregion
    }

}