using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Routes
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(this WebApplication app)
        {
            // ✅ Custom routes first



            app.MapControllerRoute(
                    name: "aboutus",
                    pattern: "about-us",
                    defaults: new { controller = "About", action = "AboutUs", title = "about-us" }
                );


            app.MapControllerRoute(
                 name: "press-release-inside",
                 pattern: "media/press-release/{title?}",
                 defaults: new { controller = "Media", action = "PressReleasesInside" }
            );


            app.MapControllerRoute(
                    name: "careers",
                    pattern: "careers",
                    defaults: new { controller = "Careers", action = "Index", title = "careers" }
            );


            app.MapControllerRoute(
                name: "LoadMoreSearch",
                pattern: "Search/LoadMoreSearch",
                defaults: new { controller = "Search", action = "LoadMoreSearch" }
            );

            app.MapControllerRoute(
                name: "search",
                pattern: "search/{id?}",
                defaults: new { controller = "Search", action = "Index" }
            );


            app.MapControllerRoute(
                name: "contactus",
                pattern: "contact-us",
                defaults: new { controller = "Contactus", action = "Index", title = "contact-us" }
            );



            app.MapControllerRoute(
                name: "legal-disclaimer",
                pattern: "disclaimer",
                defaults: new { controller = "pagearticle", action = "article", id = "disclaimer" }
            );

            app.MapControllerRoute(
                name: "privacy-policy",
                pattern: "privacy-policy",
                defaults: new { controller = "pagearticle", action = "article", id = "privacy-policy" }
            );

            app.MapControllerRoute(
             name: "terms-of-use",
             pattern: "terms-of-use",
             defaults: new { controller = "pagearticle", action = "article", id = "terms-of-use" }
         );
            app.MapControllerRoute(
              name: "sitemap",
              pattern: "sitemap",
              defaults: new { controller = "pagearticle", action = "article", id = "sitemap" }
          );


            app.MapControllerRoute(
                name: "Error",
                pattern: "Error",
                defaults: new { controller = "pagearticle", action = "Error" }
            );
            app.MapControllerRoute(
                          name: "logout",
                          pattern: "manage/logout",
                          defaults: new { controller = "Manage", action = "Logout" }
                      );
            // ✅ Area / Admin route (before default)
            app.MapControllerRoute(
                name: "manage",
                pattern: "Manage/{action=Login}/{id?}",
                defaults: new { controller = "Manage" }
            );

            // ✅ Default route LAST
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );
        }
    }
}