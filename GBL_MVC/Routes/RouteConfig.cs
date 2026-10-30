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
                    name: "learning-and-development",
                    pattern: "careers/learning-and-development",
                    defaults: new { controller = "Career", action = "LearningDevelopment", title = "learning-and-development" }
            );

            app.MapControllerRoute(
                   name: "social",
                   pattern: "sustainability/social",
                   defaults: new { controller = "Career", action = "LearningDevelopment", title = "social" }
           );
    app.MapControllerRoute(
                   name: "diversity-and-inclusion",
                   pattern: "careers/diversity-and-inclusion",
                   defaults: new { controller = "Career", action = "LearningDevelopment", title = "diversity-and-inclusion" }
           );
 app.MapControllerRoute(
                   name: "wellbeing-and-benefits",
                   pattern: "careers/wellbeing-and-benefits",
                   defaults: new { controller = "Career", action = "LearningDevelopment", title = "wellbeing-and-benefits" }
           );
app.MapControllerRoute(
                   name: "circular-products",
                   pattern: "sustainability/circular-products",
                   defaults: new { controller = "Career", action = "LearningDevelopment", title = "circular-products" }
           );
           app.MapControllerRoute(
                   name: "regenerative-processes",
                   pattern: "sustainability/regenerative-processes",
                   defaults: new { controller = "Career", action = "LearningDevelopment", title = "regenerative-processes" }
           );

                   app.MapControllerRoute(
                   name: "health-and-safety",
                   pattern: "sustainability/health-and-safety",
                   defaults: new { controller = "sustainability", action = "LearningDevelopment", title = "health-and-safety" }
           );
            app.MapControllerRoute(
                   name: "careers",
                   pattern: "careers",
                   defaults: new { controller = "Career", action = "Index", title = "careers" }
           );

            app.MapControllerRoute(
                    name: "work-with-us",
                    pattern: "careers/work-with-us",
                    defaults: new { controller = "Career", action = "WorkWithUs", title = "work-with-us" }
            );

            app.MapControllerRoute(
                               name: "contact-us",
                               pattern: "contact-us",
                               defaults: new { controller = "Contact", action = "ContactUs", title = "contact-us" }
                       );


            app.MapControllerRoute(
                    name: "work-with-us-submit",
                    pattern: "Career/SubmitWorkWithUs",
                    defaults: new { controller = "Career", action = "SubmitWorkWithUs" }
            );

            app.MapControllerRoute(
                            name: "sustainability",
                            pattern: "sustainability",
                            defaults: new { controller = "Sustainability", action = "Index", title = "sustainability" }
                       );

            app.MapControllerRoute(
                name: "carbon-light-circularity",
                pattern: "sustainability/carbon-light-circularity",
                defaults: new { controller = "Sustainability", action = "CarbonLightCircularity", title = "carbon-light-circularity" }
            );

            app.MapControllerRoute(
                name: "resource-circularity",
                pattern: "sustainability/resource-circularity",
                defaults: new { controller = "ResourceCircularity", action = "Index", title = "resource-circularity" }
            );

            app.MapControllerRoute(
                 name: "sustainability-reports",
                 pattern: "sustainability/sustainability-reports",
                 defaults: new { controller = "Sustainability", action = "SustainabilityReports", title = "sustainability-reports" }
            );


            app.MapControllerRoute(
                 name: "press-release",
                 pattern: "media/press-release",
                 defaults: new { controller = "Media", action = "PressReleases", title = "press-release" }
            );

            app.MapControllerRoute(
                name: "investor-load-pdfs",
                pattern: "Investors/LoadPdfs",
                defaults: new { controller = "Investors", action = "LoadPdfs" }
            );

            app.MapControllerRoute(
                name: "investor-shareholding-pattern",
                pattern: "investor-relations/shareholding-pattern",
                defaults: new
                {
                    controller = "Investors",
                    action = "Pdflisting",
                    title = "shareholding-pattern"
                }
            );
   app.MapControllerRoute(
                name: "investor-disclosures",
                pattern: "investor-relations/disclosures",
                defaults: new
                {
                    controller = "Investors",
                    action = "Pdflisting",
                    title = "disclosures"
                }
            );
 app.MapControllerRoute(
                name: "postal-ballot",
                pattern: "investor-relations/postal-ballot",
                defaults: new
                {
                    controller = "Investors",
                    action = "Pdflisting",
                    title = "postal-ballot"
                }
            );

            app.MapControllerRoute(
                name: "investor-subsidiaries",
                pattern: "investor-relations/subsidiaries",
                defaults: new
                {
                    controller = "Investors",
                    action = "Pdflisting",
                    title = "subsidiaries"
                }
            );

            app.MapControllerRoute(
                name: "investor-policies",
                pattern: "investor-relations/policies",
                defaults: new
                {
                    controller = "Investors",
                    action = "Pdflisting",
                    title = "policies"
                }
            );

            app.MapControllerRoute(
                name: "investor-code-of-conduct",
                pattern: "investor-relations/code-of-conduct",
                defaults: new
                {
                    controller = "Investors",
                    action = "Pdflisting",
                    title = "code-of-conduct"
                }
            );

            app.MapControllerRoute(
                name: "corporate-governance",
                pattern: "investor-relations/corporate-governance",
                defaults: new
                {
                    controller = "CorporateGovernance",
                    action = "Index",
                    title = "corporate-governance"
                }
            );

            // Sectioned IR pages: CMS children become tabs (first 2 = articles + year filter).
            // Point additional URLs at CorporateGovernance/Index to reuse this layout.
            app.MapControllerRoute(
                name: "investor-general-meeting",
                pattern: "investor-relations/general-meeting/{*title}",
                defaults: new { controller = "CorporateGovernance", action = "Index" }
            );

            app.MapControllerRoute(
                name: "Notice-of-meeting",
                pattern: "investor-relations/notice-of-meeting",
                defaults: new
                {
                    controller = "CorporateGovernance",
                    action = "Index",
                    title = "notice-of-meeting"
                }
            );
            app.MapControllerRoute(
                name: "annual-reports-load",
                pattern: "AnnualReports/LoadReports",
                defaults: new { controller = "AnnualReports", action = "LoadReports" }
            );

            app.MapControllerRoute(
                name: "investor-annual-report",
                pattern: "investor-relations/annual-reports",
                defaults: new
                {
                    controller = "AnnualReports",
                    action = "Index",
                    title = "annual-reports"
                }
            );

            app.MapControllerRoute(
                name: "investor-annual-report-singular",
                pattern: "investor-relations/annual-report",
                defaults: new
                {
                    controller = "AnnualReports",
                    action = "Index",
                    title = "annual-report"
                }
            );

            app.MapControllerRoute(
                name: "investor-quarterly-reports",
                pattern: "investor-relations/quarterly-reports",
                defaults: new
                {
                    controller = "QuarterlyReports",
                    action = "Index",
                    title = "quarterly-reports"
                }
            );

            app.MapControllerRoute(
                name: "quarterly-reports",
                pattern: "quarterly-reports",
                defaults: new
                {
                    controller = "QuarterlyReports",
                    action = "Index",
                    title = "quarterly-reports"
                }
            );

                 

            app.MapControllerRoute(
               name: "media-coverage",
               pattern: "media/media-coverage",
               defaults: new { controller = "Media", action = "MediaCoverage", title = "media-coverage" }
          );



            app.MapControllerRoute(
                 name: "press-release-inside",
                 pattern: "media/press-release/{title?}",
                 defaults: new { controller = "Media", action = "PressReleasesInside" }
            );

            app.MapControllerRoute(
               name: "media-coverage-inside",
               pattern: "media/media-coverage/{title?}",
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
                name: "industries",
                pattern: "industries",
                defaults: new { controller = "Industries", action = "Index", title = "industries" }
            );

            app.MapControllerRoute(
                name: "industries-we-serve",
                pattern: "industries-we-serve",
                defaults: new { controller = "Industries", action = "Index", title = "industries-we-serve" }
            );

            app.MapControllerRoute(
                name: "industries-inside-load-products",
                pattern: "IndustriesInside/LoadProducts",
                defaults: new { controller = "IndustriesInside", action = "LoadProducts" }
            );

            app.MapControllerRoute(
                name: "industries-inside-load-more",
                pattern: "IndustriesInside/LoadMore",
                defaults: new { controller = "IndustriesInside", action = "LoadMore" }
            );

            app.MapControllerRoute(
                name: "industries-inside",
                pattern: "IndustriesInside/{title?}",
                defaults: new { controller = "IndustriesInside", action = "Index" }
            );

            app.MapControllerRoute(
                name: "products-listing",
                pattern: "products",
                defaults: new { controller = "Products", action = "Index" }
            );

            app.MapControllerRoute(
                name: "products-load-products",
                pattern: "Products/LoadProducts",
                defaults: new { controller = "Products", action = "LoadProducts" }
            );

            app.MapControllerRoute(
                name: "products-load-more",
                pattern: "Products/LoadMore",
                defaults: new { controller = "Products", action = "LoadMore" }
            );

            app.MapControllerRoute(
                name: "products-index-html",
                pattern: "Products/Index_html",
                defaults: new { controller = "Products", action = "Index_html" }
            );

            app.MapControllerRoute(
                name: "products-inside-html",
                pattern: "Products/Inside_html",
                defaults: new { controller = "Products", action = "Inside_html" }
            );

            app.MapControllerRoute(
                name: "products-index",
                pattern: "Products/Index",
                defaults: new { controller = "Products", action = "Index" }
            );

            app.MapControllerRoute(
                name: "products-inside",
                pattern: "Products/{title}",
                defaults: new { controller = "Products", action = "Inside" }
            );

            // Submit must be registered before enquiry/{title?} so "submit" is not treated as a product page name
            app.MapControllerRoute(
                name: "enquiry-submit",
                pattern: "enquiry/submit",
                defaults: new { controller = "Enquiry", action = "SubmitEnquiry" }
            );

            app.MapControllerRoute(
                name: "enquiry",
                pattern: "enquiry/{title?}",
                defaults: new { controller = "Enquiry", action = "Index" }
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