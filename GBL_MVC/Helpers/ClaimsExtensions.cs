using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GBL_MVC.Helpers
{
    public static class ClaimsExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        // ✅ Get Role from claims (NEW)
        public static string GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

        // ✅ Map UserTypeId → Role
        public static string MapRole(int userTypeId)
        {
            return userTypeId switch
            {
                1 => "Admin",
                2 => "Editor",
                3 => "Viewer",
                4 => "Publisher",
                _ => "User"
            };
        }


    }


}