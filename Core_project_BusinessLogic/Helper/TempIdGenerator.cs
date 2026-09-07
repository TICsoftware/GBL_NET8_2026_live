using System;

namespace Core_project_BusinessLogic.Helpers
{
    public static class TempIdGenerator
    {
        // Generates a URL-safe temporary ID
        public static string GenerateTempId()
        {
            return Guid.NewGuid().ToString("N"); 
            // Example: e3c1b9f45a9b4f7e9e3c0f21a7a2b123
        }
    }
}
