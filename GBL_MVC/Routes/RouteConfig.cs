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
                  name: "financialInformation",
                  pattern: "about-us/financial-Information",
                  defaults: new { controller = "About", action = "FinancialInformation", title = "financial-Information" }
              );

                app.MapControllerRoute(
                  name: "sustainability",
                  pattern: "esg/sustainability",
                  defaults: new { controller = "ESG", action = "Sustainability", title = "sustainability" }
              );

 app.MapControllerRoute(
                  name: "ourproducts",
                  pattern: "solutions",
                  defaults: new { controller = "Product", action = "Index", title = "solutions" }
              );
            app.MapControllerRoute(
                  name: "ourproducts",
                  pattern: "solutions",
                  defaults: new { controller = "Product", action = "Index", title = "solutions" }
              );



        app.MapControllerRoute(
                      name: "technical-services",
                      pattern: "solutions/technical-services",
                      defaults: new { controller = "Product", action = "TechnicalServices", title = "technical-services" }
                  );

                  app.MapControllerRoute(
                      name: "solutions-enquiry",
                      pattern: "solutions/enquiry",
                      defaults: new { controller = "Product", action = "SolutionsCenter", title = "enquiry" }
                  );

            app.MapControllerRoute(
                          name: "products-inside",
                          pattern: "solutions/{title?}",
                          defaults: new { controller = "Product", action = "Inside" }
                      );

            app.MapControllerRoute(
                          name: "careers",
                          pattern: "careers",
                          defaults: new { controller = "Careers", action = "Index", title = "careers" }
                      );


            app.MapControllerRoute(
              name: "SubmitTechnicalSupport",
              pattern: "Product/SubmitTechnicalSupport",
              defaults: new { controller = "Product", action = "SubmitTechnicalSupport" }
          );

            app.MapControllerRoute(
              name: "SubmitSolutionsEnquiry",
              pattern: "Product/SubmitSolutionsEnquiry",
              defaults: new { controller = "Product", action = "SubmitSolutionsEnquiry" }
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
                name: "SubmitContactEnquiry",
                pattern: "Contactus/SubmitEnquiry",
                defaults: new { controller = "Contactus", action = "SubmitEnquiry" }
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