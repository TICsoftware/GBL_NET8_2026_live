using System.Globalization;
using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.FileProviders;
using GBL_MVC.Policy;
using GBL_MVC.Routes;

var builder = WebApplication.CreateBuilder(args);

var enableTls12 = builder.Configuration.GetValue<string>("EnableTls12");

// 👉 Optional (legacy support - only if really needed)
if (enableTls12 == "1")
{
    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
}

// 👉 Recommended modern approach (HttpClient)
builder.Services.AddHttpClient("MyClient")
    .ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            SslProtocols = SslProtocols.Tls12
        });

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Add services to the container.
//builder.Services.AddControllersWithViews();
//.AddRazorRuntimeCompilation();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddControllersWithViews()
   .AddRazorRuntimeCompilation();//uncomment while live
}
else
{
    builder.Services
        .AddControllersWithViews()
       .AddRazorRuntimeCompilation();
}


builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Manage/Login";
        options.AccessDeniedPath = "/Manage/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddAuthorization();

//uncomment while live start
// builder.Services.AddSession(options =>
// {
//     options.IdleTimeout = TimeSpan.FromMinutes(20); // session timeout
//     options.Cookie.HttpOnly = true;
//     options.Cookie.IsEssential = true;
// });

//  builder.Services.AddAntiforgery(options =>
// {
//     options.Cookie.Name = "SecureToken";
//     options.Cookie.HttpOnly = true;
//    options.Cookie.SecurePolicy =
//         CookieSecurePolicy.Always;  
// });
//uncomment while live end

//builder.Services.AddHttpClient();


// Kestrel limit
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 52428800;
    options.AddServerHeader = false;
});

// Multipart/form-data limit (file uploads)
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800; // 50 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

//uncomment while live
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "Assets")),
    RequestPath = "/Assets"
});

// Handles 404, 403, etc. when no endpoint produces the response
app.UseStatusCodePagesWithReExecute("/Error");

var redirects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
{
    // Home page
    { "^index\\.html$", "/" },
    { "^index_new\\.html$", "/" },
    { "^404_page\\.html$", "/" },
    { "^google30c494c5d635adbc\\.html$", "/" },

    // Common pages
    { "^disclaimer\\.html$", "/disclaimer" },
    { "^privacy-policy\\.html$", "/privacy-policy" },
    { "^sitemap\\.html$", "/sitemap" },
    { "^terms-of-use\\.html$", "/terms-of-use" },

    // Technical Services
    { "^technical-services\\.html$", "/solutions/technical-services" },

    // Contact
    { "^dealership_enquiry$", "/contact-us" },
    { "^contact-us/contact\\.html$", "/contact-us" },

    // About Us
    { "^about-us/overview\\.html$", "/about-us" },

    // Products / Solutions
    { "^products/processes\\.html$", "/solutions" },
    { "^products/processes1\\.html$", "/solutions" },
    { "^products/processes2\\.html$", "/solutions" },
    { "^products/processes3\\.html$", "/solutions" },
    { "^products/processes4\\.html$", "/solutions" },
    { "^products/processes5\\.html$", "/solutions" },
    { "^products/processes6\\.html$", "/solutions" },
    { "^products/processes7\\.html$", "/solutions" },
    { "^products/processes8\\.html$", "/solutions" },

    { "^products/products-opc-and-ppc\\.html$", "/solutions" },
    { "^products/products-opc-and-ppc\\.html#OPC$", "/solutions/maxload-opc-53-grade" },
    { "^products/products-opc-and-ppc\\.html#PPC1$", "/solutions/portland-pozzolana-cement" },

    { "^products/products-maxload-bulk-cement\\.html$", "/solutions/maxload-bulk-cement" },

    // Social Responsibility
    { "^social-responsibility/overview\\.html$", "/about-us" },
    { "^social-responsibility/join-us\\.html$", "/about-us" },
    { "^social-responsibility/privacy-policy\\.html$", "/privacy-policy" },
    { "^social-responsibility/404_page\\.html$", "/about-us" },
    { "^social-responsibility/rcl-csr-policy\\.pdf$", "/about-us" },

    // Media
    { "^media/media\\.html$", "/" },
    { "^media/gallery\\.html$", "/" },
    { "^media/CorporateFilm\\.html$", "/" },
    { "^media/Print\\.html$", "/" },

    // Careers
    { "^careers/join-us\\.html$", "/contact-us" },
    { "^careers/current_opening\\.html$", "/contact-us" },
    { "^careers/current-opening\\.html$", "/contact-us" },

    // Financial Information
    { "^financial-information/financial-information\\.html$", "/" },

    // Regulatory Environment Reports
    { "^regulatory-environment-reports/index\\.html$", "/" },

    // Any other old .html URL → Home page
    { "^.*\\.html$", "/" }
};

var options = new RewriteOptions();

foreach (var rule in redirects)
{
    options.AddRedirect(rule.Key, rule.Value, StatusCodes.Status301MovedPermanently);
}

// ✅ IMPORTANT: Must be before UseRouting()
app.UseRewriter(options);
var culture = new CultureInfo("en-US");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = new[] { culture },
    SupportedUICultures = new[] { culture }
});
app.Use(async (context, next) =>
{
    context.Response.Headers["Vary"] = "Accept-Encoding";
    context.Response.Headers["Access-Control-Allow-Headers"] = "Origin, Content-Type, Accept";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["X-Permitted-Cross-Domain-Policies"] = "none";
    //uncommen for live after ssl
    context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    context.Response.Headers["Permissions-Policy"] = "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(self), payment=(), fullscreen=(self)";
    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";

        // Content Security Policy
// Content Security Policy - Allow resources from all domains
context.Response.Headers["Content-Security-Policy"] =
    "default-src * 'self' data: blob: 'unsafe-inline' 'unsafe-eval'; " +
    "script-src * 'self' data: blob: 'unsafe-inline' 'unsafe-eval'; " +
    "style-src * 'self' data: blob: 'unsafe-inline'; " +
    "img-src * 'self' data: blob:; " +
    "font-src * 'self' data:; " +
    "connect-src * 'self'; " +
    "frame-src * 'self' data: blob:; " +
    "media-src * 'self' data: blob:; " +
    "object-src * 'self' data: blob:;";

    // Remove unwanted headers
    context.Response.Headers.Remove("Server");
    context.Response.Headers.Remove("X-Powered-By");
    context.Response.Headers.Remove("X-AspNetMvc-Version");

    await next();
});

//app.UseCspPolicy();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization(); 

app.RegisterRoutes(); 

app.Run();
