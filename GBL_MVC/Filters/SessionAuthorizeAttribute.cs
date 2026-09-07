using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GBL_MVC.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            // ❌ If session expired
            if (string.IsNullOrEmpty(session.GetString("UserId")))
            {
                context.Result = new RedirectToActionResult("Login", "Manage", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}