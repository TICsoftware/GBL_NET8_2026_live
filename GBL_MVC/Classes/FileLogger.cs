
using System.Text;

namespace GBL_MVC
{
    public static class FileLogger
    {
        public static IHttpContextAccessor? HttpContextAccessor { get; set; }
        private static readonly object _lock = new();
        //private static readonly string _logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        private static readonly string _logFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "Logs");

        // Change this to "yyyy-MM" for monthly logs
        private static readonly string _fileName = DateTime.Now.ToString("yyyy-MM") + "_logs.txt";

        public static void Log(string message)
        {
            try
            {
                if (!Directory.Exists(_logFolder))
                    Directory.CreateDirectory(_logFolder);

                string filePath = Path.Combine(_logFolder, _fileName);
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";

                lock (_lock)
                {
                    File.AppendAllText(filePath, logMessage, Encoding.UTF8);
                }
            }
            catch
            {
                // NO THROW — logging should never break app
            }
        }

        public static void LogError(string method, Exception ex)
        {
            var httpContext = HttpContextAccessor?.HttpContext;

            string controller = httpContext?.Request.RouteValues["controller"]?.ToString() ?? "Unknown";
            string action = httpContext?.Request.RouteValues["action"]?.ToString() ?? "Unknown";

            Log($"Controller: {controller}, Action: {action}: " + method + " :[ERROR] " + ex.ToString());
        }
    }
}