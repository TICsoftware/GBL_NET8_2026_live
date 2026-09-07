using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.Entity;

namespace GBL_MVC.ViewComponents
{
    public class FooterMenuViewComponent : ViewComponent
    {
        private readonly Menu_BAL _bal;

        public FooterMenuViewComponent(IConfiguration configuration)
        {
            _bal = new Menu_BAL(configuration);
        }

        public IViewComponentResult Invoke()
        {
            try
            {
                var footerMenus = _bal.GetFrontendFooterMenus() ?? new List<MenuFooter>();

                // Keep your existing Footer.cshtml in Views/Shared
                return View("~/Views/Shared/Footer.cshtml", footerMenus);
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/FooterMenuViewComponent :", ex);

                // Return an empty list if an error occurs
                return View(
                    "~/Views/Shared/Footer.cshtml",
                    new List<MenuFooter>()
                );
            }
        }
    }
}