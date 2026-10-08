using System;
using System.Data;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.DAL;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class ProductEnquiry_BAL : ProductEnquiry_DAL
    {
        private readonly IConfiguration _configuration;

        public ProductEnquiry_BAL(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public DataTable SubmitEnquiry_BAL(ProductEnquiry model)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = AddProductEnquiry_DAL(model);
                if (dt.Rows.Count > 0 && string.Equals(dt.Rows[0][0]?.ToString(), "updated", StringComparison.OrdinalIgnoreCase))
                {
                    // Email is best-effort — do not fail the enquiry if SMTP is misconfigured
                    try
                    {
                        var productLabel = string.IsNullOrWhiteSpace(model.ProductName) ? "product" : model.ProductName;
                        SendMail(MailEnquiryContent(model), "Enquiry for " + productLabel);
                    }
                    catch (Exception mailEx)
                    {
                        System.Diagnostics.Debug.WriteLine("ProductEnquiry mail failed: " + mailEx.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                if (dt.Columns.Count == 0)
                {
                    dt.Columns.Add("Result");
                    dt.Rows.Add(ex.Message);
                }
                else if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(ex.Message);
                }
                else
                {
                    dt.Rows[0][0] = ex.Message;
                }
            }

            return dt;
        }

        public void SendMail(string emailContent, string subject)
        {
            var settings = _configuration.GetSection("MailSetting");

            string host = settings["hostname"] ?? string.Empty;
            string username = settings["mailusername"] ?? string.Empty;
            string password = settings["mailpassword"] ?? string.Empty;
            string from = settings["From"] ?? username;
            string to = settings["Enquiry"] ?? settings["ContactUs"] ?? string.Empty;
            string displayName = settings["DisplayName"] ?? "Godavari Biorefineries";

            int.TryParse(settings["Port"], out int port);

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(from) ||
                string.IsNullOrWhiteSpace(to))
            {
                throw new Exception("Mail settings are missing or invalid. Set MailSetting:mailpassword and MailSetting:Enquiry.");
            }

            using var message = new MailMessage();
            message.From = new MailAddress(from, displayName);

            foreach (var email in to.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                message.To.Add(email.Trim());
            }

            message.Subject = subject ?? string.Empty;
            message.Body = emailContent ?? string.Empty;
            message.IsBodyHtml = true;

            using var smtp = new SmtpClient(host)
            {
                Port = port > 0 ? port : 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            smtp.Send(message);
        }

        public string MailEnquiryContent(ProductEnquiry obj)
        {
            var mobileDisplay = string.IsNullOrWhiteSpace(obj.CountryCode)
                ? obj.Mobile
                : (obj.CountryCode + " " + obj.Mobile);

            return
                "<h4>Dear Team,</h4>" +
                "<p>Please find below the product enquiry submitted through the website.</p>" +
                "<table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse;width:100%;'>" +
                "<tbody>" +
                "<tr><td><strong>Product</strong></td><td>" + obj.ProductName + "</td></tr>" +
                (!string.IsNullOrWhiteSpace(obj.ProductPageName)
                    ? "<tr><td><strong>Product page</strong></td><td>" + obj.ProductPageName + "</td></tr>"
                    : "") +
                "<tr><td><strong>Name</strong></td><td>" + obj.FullName + "</td></tr>" +
                "<tr><td><strong>Company Name</strong></td><td>" + obj.CompanyName + "</td></tr>" +
                "<tr><td><strong>Street Address</strong></td><td>" + obj.StreetAddress + "</td></tr>" +
                "<tr><td><strong>City</strong></td><td>" + obj.City + "</td></tr>" +
                "<tr><td><strong>State</strong></td><td>" + obj.State + "</td></tr>" +
                "<tr><td><strong>Country</strong></td><td>" + obj.Country + "</td></tr>" +
                (!string.IsNullOrWhiteSpace(obj.Phone)
                    ? "<tr><td><strong>Phone</strong></td><td>" + obj.Phone + "</td></tr>"
                    : "") +
                "<tr><td><strong>Mobile</strong></td><td>" + mobileDisplay + "</td></tr>" +
                (!string.IsNullOrWhiteSpace(obj.Fax)
                    ? "<tr><td><strong>Fax</strong></td><td>" + obj.Fax + "</td></tr>"
                    : "") +
                "<tr><td><strong>Email</strong></td><td>" + obj.Email + "</td></tr>" +
                "<tr><td><strong>Business Type</strong></td><td>" + obj.BusinessType + "</td></tr>" +
                (!string.IsNullOrWhiteSpace(obj.EnquiryDetails)
                    ? "<tr><td><strong>Enquiry Details</strong></td><td>" + obj.EnquiryDetails + "</td></tr>"
                    : "") +
                (!string.IsNullOrWhiteSpace(obj.IPAddress)
                    ? "<tr><td><strong>IP Address</strong></td><td>" + obj.IPAddress + "</td></tr>"
                    : "") +
                "</tbody></table>" +
                "<br/><p><strong>Regards,</strong><br/><strong>Godavari Biorefineries Website</strong></p>";
        }
    }
}
