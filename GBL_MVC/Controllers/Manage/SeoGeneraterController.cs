using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core_project_BusinessLogic.BLL;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using GBL_MVC.Filters;
using Microsoft.AspNetCore.Authorization;
namespace GBL_MVC.Controllers.Manage
{
  [Authorize]
[SessionAuthorize]
    public class SeoGeneraterController : Controller
    {
        private readonly ILogger<SeoGeneraterController> _logger;
        private readonly IConfiguration objconfig;
        private readonly HttpClient _httpClient;

        public SeoGeneraterController(ILogger<SeoGeneraterController> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            objconfig = configuration;
            _httpClient = httpClient;
        }



        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateSEO(string title, string content)
        {
            try
            {
                // ✅ Correct API URL (NOT swagger URL)
                var url = "https://ticaiapi.ticuat.com/docs#/SEO/generate_seo_api_generate_seo_post";

                // ✅ Get token from appsettings.json
                var token = "f42e124095a6357b1936fb02fe2bc6b241d8d28d3a0f6b960345a80f01fbbfc9";

                var requestData = new SeoRequest
                {
                    page_url = "/blogs",
                    page_type = "blog",
                    mode = "basic",
                    content = content
                };

                var json = JsonConvert.SerializeObject(requestData);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                // ✅ Clean headers and set properly
                _httpClient.DefaultRequestHeaders.Clear();

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                // ✅ API call
                var response = await _httpClient.PostAsync(url, httpContent);
                var responseString = await response.Content.ReadAsStringAsync();

                // 🔥 Show real error if API fails
                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { success = false, message = responseString });
                }

                // ✅ Parse response safely
                dynamic result = JsonConvert.DeserializeObject(responseString);

                var keywords = result?.data?.keywords ?? result?.keywords ?? "";
                var description = result?.data?.description ?? result?.description ?? "";

                return Json(new
                {
                    success = true,
                    keywords = keywords,
                    description = description
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SEO API Error");

                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> GenerateSEO(string title, string content)
        // {
        //     try
        //     {
        //         var url = "https://ticaiapi.ticuat.com/docs#/SEO/generate_seo_api_generate_seo_post";

        //         var requestData = new SeoRequest
        //         {
        //             page_url = "/about-us",
        //             page_type = "blog",
        //             mode = "basic",
        //             content = content
        //         };

        //         var json = JsonConvert.SerializeObject(requestData);
        //         var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        //         _httpClient.DefaultRequestHeaders.Authorization =
        //             new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "f42e124095a6357b1936fb02fe2bc6b241d8d28d3a0f6b960345a80f01fbbfc9");

        //         var response = await _httpClient.PostAsync(url, httpContent);
        //         var responseString = await response.Content.ReadAsStringAsync();

        //         if (!response.IsSuccessStatusCode)
        //         {
        //             return Json(new { success = false, message = responseString });
        //         }

        //         // 🔥 Parse API response
        //         dynamic result = JsonConvert.DeserializeObject(responseString);

        //         return Json(new
        //         {
        //             success = true,
        //             keywords = result?.keywords ?? "",
        //             description = result?.description ?? ""
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return Json(new { success = false, message = ex.Message });
        //     }
        // }




        private string GenerateSlug(string title)
        {
            return "/resources/insights/" + title
                .ToLower()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace(",", "");
        }


        // [HttpPost]
        // public async Task<IActionResult> GenerateSEO(string title, string content)
        // {
        //     try
        //     {
        //         var apiKey = objconfig["OpenAI:ApiKey"];

        //         using var client = new HttpClient();
        //         client.DefaultRequestHeaders.Authorization =
        //             new AuthenticationHeaderValue("Bearer", apiKey);

        //         var prompt = $@"
        //             Generate SEO for:
        //             Title: {title}
        //             Content: {content}

        //             Return in JSON:
        //             {{
        //             ""keywords"": ""comma separated keywords"",
        //             ""description"": ""max 160 characters""
        //             }}";

        //         var requestBody = new
        //         {
        //             model = "gpt-4.1-mini",
        //             messages = new[]
        //             {
        //             new { role = "user", content = prompt }
        //         }
        //         };

        //         var jsonContent = new StringContent(
        //             JsonSerializer.Serialize(requestBody),
        //             Encoding.UTF8,
        //             "application/json"
        //         );

        //         var response = await client.PostAsync(
        //             "https://api.openai.com/v1/chat/completions",
        //             jsonContent
        //         );

        //         var result = await response.Content.ReadAsStringAsync();

        //         using var doc = JsonDocument.Parse(result);
        //         var aiText = doc.RootElement
        //             .GetProperty("choices")[0]
        //             .GetProperty("message")
        //             .GetProperty("content")
        //             .GetString();

        //         // Parse AI JSON response
        //         var seo = JsonSerializer.Deserialize<SeoResult>(aiText);

        //         return Json(new
        //         {
        //             success = true,
        //             keywords = seo.keywords,
        //             description = seo.description
        //         });
        //     }
        //     catch (Exception ex)
        //     {
        //         return Json(new { success = false, message = ex.Message });
        //     }
        // }


    }



}

public class SeoRequest
{
    public string page_url { get; set; }
    public string page_type { get; set; }
    public string mode { get; set; }
    public string content { get; set; }
}