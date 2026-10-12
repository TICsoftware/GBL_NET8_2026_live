using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.DAL;
using GBL_BusinessLogic.Entity;
using GBL_BusinessLogic;
using System.Net;
using System.Net.Mail;

namespace GBL_BusinessLogic.BAL
{
    public class Contact_BAL : BasePageBAL
    {
        public Contact_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public ContactModel GetContact_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new ContactModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            // Content
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                model.Registered_Corporate_Office_List = MapComponents(groupedData, 1);
                model.Marketing_Office_List = MapComponents(groupedData, 2);
                model.Products_List = MapComponents(groupedData, 3);
                model.India_Offices_List = MapComponents(groupedData, 4);
                model.InternationalOffices_List = MapComponents(groupedData, 5);
            }

            return model;
        }



        public DataTable ContactUs_Insert_BAL(ContactEntity model)
        {
            DataTable dt = new DataTable();

            try
            {
                dt = ContactUs_Insert_DAL(model);

                if (dt.Rows.Count > 0 && int.TryParse(dt.Rows[0][0]?.ToString(), out int id) && id > 0)
                {
                    //SendMail(MailEnquiryContent(model), "Work with us enquiry from " + model.Name);
                }
            }
            catch (Exception ex)
            {
                if (dt.Columns.Count == 0)
                {
                    dt.Columns.Add("Result");
                }

                if (dt.Rows.Count == 0)
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


            using var smtp = new SmtpClient(host)
            {
                Port = port > 0 ? port : 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };

            smtp.Send(message);
        }


        public string MailEnquiryContent(ContactEntity obj)
        {
            return
                "<h4>Dear Team,</h4>" +
                "<p>Please find below the contact enquiry submitted through the website.</p>" +

                "<table border='1' cellpadding='6' cellspacing='0' " +
                "style='border-collapse:collapse;width:100%;'>" +
                "<tbody>" +

                "<tr><td><strong>Name</strong></td><td>" + System.Net.WebUtility.HtmlEncode(obj.Name) + "</td></tr>" +
                "<tr><td><strong>Company Name</strong></td><td>" + System.Net.WebUtility.HtmlEncode(obj.CompanyName) + "</td></tr>" +
                "<tr><td><strong>Phone Number</strong></td><td>" + System.Net.WebUtility.HtmlEncode(obj.PhoneNumber) + "</td></tr>" +
                "<tr><td><strong>Email Address</strong></td><td>" + System.Net.WebUtility.HtmlEncode(obj.EmailAddress) + "</td></tr>" +
                "<tr><td><strong>State ID</strong></td><td>" + obj.StateId + "</td></tr>" +
                "<tr><td><strong>Site Location</strong></td><td>" + System.Net.WebUtility.HtmlEncode(obj.SiteLocation) + "</td></tr>" +

                (!string.IsNullOrWhiteSpace(obj.Query)
                    ? "<tr><td><strong>Query</strong></td><td>" + System.Net.WebUtility.HtmlEncode(obj.Query) + "</td></tr>"
                    : "") +

                (!string.IsNullOrWhiteSpace(obj.IPAddress)
                    ? "<tr><td><strong>IP Address</strong></td><td>" + System.Net.WebUtility.HtmlEncode(obj.IPAddress) + "</td></tr>"
                    : "") +

                "<tr><td><strong>Consent</strong></td><td>" + (obj.Consent == true ? "Yes" : "No") + "</td></tr>" +

                "</tbody></table>" +

                "<br/><p><strong>Regards,</strong><br/>" +
                "<strong>Godavari Biorefineries Website</strong></p>";
        }


        public List<StateMaster> GetStateMaster_Bal()
        {
            List<StateMaster> lst = new List<StateMaster>();

            DataTable dt = GetStateMaster();

            foreach (DataRow row in dt.Rows)
            {
                StateMaster obj = new StateMaster
                {
                    StateId = Convert.ToInt32(row["StateId"]),
                    StateName = row["StateName"].ToString(),
                    Status = Convert.ToBoolean(row["Status"]),
                    CreateDate = row["CreateDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["CreateDate"])
                };

                lst.Add(obj);
            }

            return lst;
        }





    }
}
