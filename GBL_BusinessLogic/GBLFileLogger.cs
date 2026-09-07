using System;
using System.IO;
using System.Text;

namespace GBL_BusinessLogic.Common
{
    public static class GBLFileLogger
    {
        private static readonly object _lock = new();

        //private static readonly string _logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static readonly string _logFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Logs");

        private static string FileName =>
            $"{DateTime.Now:yyyy-MM}_logs.txt";

        public static void Log(string message)
        {
            try
            {
                if (!Directory.Exists(_logFolder))
                    Directory.CreateDirectory(_logFolder);

                string filePath = Path.Combine(_logFolder, FileName);

                string logMessage =
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";

                lock (_lock)
                {
                    File.AppendAllText(filePath, logMessage, Encoding.UTF8);
                }
            }
            catch
            {
                // Never throw from logger
            }
        }

        public static void LogError(string method, Exception ex)
        {
            Log($@"{ex}");
        }

        public static void LogError(string controller, string action, string method, Exception ex)
        {
            Log($@"{ex}");
        }

        public static void LogInfo(string controller, string action, string message)
        {
            Log($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Controller: {controller}, Action: {action}, Message: {message}");
        }




    }
}