using System;
using System.Data;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.DAL;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class WorkWithUsEnquiry_BAL : WorkWithUsEnquiry_DAL
    {
        private readonly IConfiguration _configuration;

        public WorkWithUsEnquiry_BAL(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public DataTable SubmitEnquiry_BAL(WorkWithUsEnquiry model, string? resumePhysicalPath = null)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = AddWorkWithUsEnquiry_DAL(model);
                if (dt.Rows.Count > 0 && string.Equals(dt.Rows[0][0]?.ToString(), "updated", StringComparison.OrdinalIgnoreCase))
                {
                    SendMail(MailEnquiryContent(model), "Work with us enquiry from " + model.FullName, resumePhysicalPath);
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

        public void SendMail(string emailContent, string subject, string? attachmentPath = null)
        {
            var settings = _configuration.GetSection("MailSetting");

            string host = settings["hostname"] ?? string.Empty;
            string username = settings["mailusername"] ?? string.Empty;
            string password = settings["mailpassword"] ?? string.Empty;
            string from = settings["From"] ?? username;
            string to = settings["WorkWithUs"] ?? settings["ContactUs"] ?? string.Empty;
            string displayName = settings["DisplayName"] ?? "Godavari Biorefineries";

            int.TryParse(settings["Port"], out int port);

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(from) ||
                string.IsNullOrWhiteSpace(to))
            {
                throw new Exception("Mail settings are missing or invalid.");
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

            if (!string.IsNullOrWhiteSpace(attachmentPath) && System.IO.File.Exists(attachmentPath))
            {
                message.Attachments.Add(new Attachment(attachmentPath));
            }

            using var smtp = new SmtpClient(host)
            {
                Port = port > 0 ? port : 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            smtp.Send(message);
        }

        public string MailEnquiryContent(WorkWithUsEnquiry obj)
        {
            return
                "<h4>Dear Team,</h4>" +
                "<p>Please find below the Work with us enquiry submitted through the website.</p>" +
                "<table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse;width:100%;'>" +
                "<tbody>" +
                "<tr><td><strong>Full Name</strong></td><td>" + obj.FullName + "</td></tr>" +
                "<tr><td><strong>Email</strong></td><td>" + obj.Email + "</td></tr>" +
                (!string.IsNullOrWhiteSpace(obj.Expertise)
                    ? "<tr><td><strong>Function / Area of expertise</strong></td><td>" + obj.Expertise + "</td></tr>"
                    : "") +
                (!string.IsNullOrWhiteSpace(obj.Address)
                    ? "<tr><td><strong>Address</strong></td><td>" + obj.Address + "</td></tr>"
                    : "") +
                (!string.IsNullOrWhiteSpace(obj.Designation)
                    ? "<tr><td><strong>Designation</strong></td><td>" + obj.Designation + "</td></tr>"
                    : "") +
                (!string.IsNullOrWhiteSpace(obj.ResumePath)
                    ? "<tr><td><strong>Resume</strong></td><td>" + obj.ResumePath + "</td></tr>"
                    : "") +
                (!string.IsNullOrWhiteSpace(obj.Message)
                    ? "<tr><td><strong>Message</strong></td><td>" + obj.Message + "</td></tr>"
                    : "") +
                (!string.IsNullOrWhiteSpace(obj.IPAddress)
                    ? "<tr><td><strong>IP Address</strong></td><td>" + obj.IPAddress + "</td></tr>"
                    : "") +
                "</tbody></table>" +
                "<br/><p><strong>Regards,</strong><br/><strong>Godavari Biorefineries Website</strong></p>";
        }
    }
}
