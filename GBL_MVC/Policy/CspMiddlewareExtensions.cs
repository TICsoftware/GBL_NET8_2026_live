using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Policy
{
    public static class CspMiddlewareExtensions
    {
        public static IApplicationBuilder UseCspPolicy(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CspMiddleware>();
        }
    }
}