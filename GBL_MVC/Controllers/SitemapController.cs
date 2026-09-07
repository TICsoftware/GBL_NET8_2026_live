using Microsoft.AspNetCore.Mvc;
using GBL_BusinessLogic.BAL;
using GBL_BusinessLogic.DAL;
using System.Data;
using System.Text;
using System.Xml;

namespace GBL_MVC.Controllers;

public class SitemapController : Controller
{
     private readonly ILogger<SitemapController> _logger;
    private readonly IConfiguration _configuration;

    public SitemapController(
        ILogger<SitemapController> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [Route("sitemap.xml")]
    public IActionResult Index()
    {
         DataTable dt = new ContentRepository(_configuration)
            .GetContentForSitemap();


        XmlWriterSettings settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(false),
            Indent = true,
            OmitXmlDeclaration = false
        };

        using MemoryStream ms = new MemoryStream();

        using (XmlWriter writer = XmlWriter.Create(ms, settings))
        {
            writer.WriteStartDocument();

            writer.WriteProcessingInstruction(
                "xml-stylesheet",
                "type=\"text/xsl\" href=\"/Content/sitemap.xsl\"");

            writer.WriteStartElement(
                "urlset",
                "http://www.sitemaps.org/schemas/sitemap/0.9");

            writer.WriteAttributeString(
                "xmlns",
                "xsi",
                null,
                "http://www.w3.org/2001/XMLSchema-instance");

            writer.WriteAttributeString(
                "xsi",
                "schemaLocation",
                "http://www.w3.org/2001/XMLSchema-instance",
                "http://www.sitemaps.org/schemas/sitemap/0.9 " +
                "http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            string baseUrl = $"{Request.Scheme}://{Request.Host}";

            foreach (DataRow row in dt.Rows)
            {
                string path = "";

                if (!string.IsNullOrWhiteSpace(Convert.ToString(row["cont_pdf"])))
                {
                    path = Convert.ToString(row["cont_pdf"]) ?? "";
                }
                else if (!string.IsNullOrWhiteSpace(Convert.ToString(row["cont_search_url"])))
                {
                    path = Convert.ToString(row["cont_search_url"]) ?? "";
                }
                else
                {
                    path = Convert.ToString(row["pagelink"]) ?? "";
                }

                if (string.IsNullOrWhiteSpace(path))
                    continue;

                string url;

                if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    url = path;
                }
                else
                {
                    if (!path.StartsWith("/"))
                        path = "/" + path;

                    url = baseUrl + path;
                }

                DateTime lastModified;

                if (!DateTime.TryParse(Convert.ToString(row["LastModified"]), out lastModified))
                {
                    lastModified = DateTime.Now;
                }

                writer.WriteStartElement("url");

                writer.WriteElementString("loc", url);
                writer.WriteElementString(
                    "lastmod",
                    lastModified.ToString("yyyy-MM-dd"));

                writer.WriteElementString("changefreq", "weekly");
                writer.WriteElementString("priority", "1.0");

                writer.WriteEndElement(); // url
            }

            writer.WriteEndElement(); // urlset
            writer.WriteEndDocument();
        }

        return File(ms.ToArray(), "application/xml");
    }
}