using Core_project_BusinessLogic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace GBL_MVC.Classes
{
    public static class Helper
    {

        public static List<SelectListItem> BuildHierarchy(List<Options_List> items, int? parent_id = 0)
        {
            List<SelectListItem> result = [];
            var default_item = items.FirstOrDefault(x => x.id == 0);
            if (default_item != null)
            {
                result.Add(new SelectListItem
                {
                    Value = "0",
                    Text = default_item.title
                });
            }
            var all_records = items.Where(x => x.id != 0).ToList();

            AddChildren(result, all_records, parent_id, 0);
            return result;
        }

        private static void AddChildren(List<SelectListItem> list, List<Options_List> all, int? parentId, int level)
        {
            var children = all.Where(x => x.parent_id == parentId).ToList();

            foreach (var child in children)
            {
                // prefix with "--" per level
                //string prefix = new string('-', level * 2);
                string prefix = "";
                list.Add(new SelectListItem
                {
                    Value = child.id.ToString(),
                    Text = prefix + child.title
                });

                // recursive call for next level
                AddChildren(list, all, child.id, level + 1);
            }
        }

      
        public static void Remove_temporary_files(string folderpath)
        {
            var files = Directory.GetFiles(folderpath);

            foreach (var file in files)
            {
                if (File.GetCreationTime(file) < DateTime.Now.AddHours(-2))
                {
                    File.Delete(file);
                }
            }
        }

        public static string CleanFileName(string fileName)
        {
            // Get filename without extension
            string name = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            // Replace spaces with hyphen
            name = name.Replace(" ", "-");

            // Remove special characters (keep letters, numbers, hyphen)
            name = Regex.Replace(name, @"[^a-zA-Z0-9\-]", "");

            // Replace multiple hyphens with single
            name = Regex.Replace(name, @"-+", "-");

            // Trim hyphens from start/end
            name = name.Trim('-');

            // Optional: convert to lowercase
            name = name.ToLower();

            return name + extension;
        }


        public static void SendMail(
            string displayName,
            string To,
            string CC,
            string BCC,
            string Subject,
            string Body,
            bool isHTML,
            IConfiguration objconfig,
            string attachment = ""
        )
        {
            try
            {
                using (SmtpClient smtpClient = new SmtpClient())
                using (MailMessage message = new MailMessage())
                {
                    // Get values from config
                    string username = objconfig["MailSetting:mailusername"];
                    string password = objconfig["MailSetting:mailpassword"];
                    string host = objconfig["MailSetting:hostname"];
                    int port = Convert.ToInt32(objconfig["MailSetting:Port"]);

                    if (string.IsNullOrWhiteSpace(displayName))
                    {
                        displayName = username;
                    }

                    // From
                    message.From = new MailAddress(username, displayName);

                    // To
                    foreach (var email in To.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        message.To.Add(email.Trim());
                    }

                    // CC
                    if (!string.IsNullOrWhiteSpace(CC))
                    {
                        foreach (var email in CC.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            message.CC.Add(email.Trim());
                        }
                    }

                    // BCC
                    if (!string.IsNullOrWhiteSpace(BCC))
                    {
                        foreach (var email in BCC.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            message.Bcc.Add(email.Trim());
                        }
                    }

                    // Subject & Body
                    message.Subject = Subject;
                    message.Body = Body;
                    message.IsBodyHtml = isHTML;

                    // Attachment
                    if (!string.IsNullOrWhiteSpace(attachment))
                    {
                        message.Attachments.Add(new Attachment(attachment));
                    }

                    // SMTP Settings
                    smtpClient.Host = host;
                    smtpClient.Port = port;
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;   // ✅ IMPORTANT
                    smtpClient.Credentials = new NetworkCredential(username, password);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Timeout = 200000;

                    // Send mail
                    smtpClient.Send(message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Email sending failed: " + ex.Message, ex);
            }
        }



        private static void SendMaillocal(string EmailContent, string strSubject, string EmailId)
        {
            try
            {
                var message = new MailMessage();
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);

                var credential = new NetworkCredential(
                    "ticworks2022@gmail.com",
                    "password"
                );

                message.From = new MailAddress("ticworks2022@gmail.com", "Priya Cement");
                message.To.Add("ashabbir72@gmail.com");

                message.Subject = strSubject;
                message.Body = EmailContent;
                message.IsBodyHtml = true;

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = credential;
                smtpClient.EnableSsl = true;

                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static void SendAMail(string displayName, string To, string CC, string BCC, string Subject, string Body, bool isHTML, IConfiguration objconfig, string attachment = "")
        {
            string[] tempTo, tempCC, tempBCC;
            try
            {
                using (System.Net.Mail.SmtpClient smtpClient = new())
                {
                    using (System.Net.Mail.MailMessage message = new())
                    {
                        if (string.IsNullOrWhiteSpace(displayName))
                        {
                            displayName = objconfig["MailSetting:mailusername"];
                        }
                        System.Net.Mail.MailAddress mailAddress = new(displayName + " <" + objconfig["MailSetting:mailusername"] + ">");
                        //smtpClient.Host = "smtp.office365.com";
                        //message.From = mailAddress;
                        System.Net.NetworkCredential networkCredential = new(objconfig["MailSetting:mailusername"], objconfig["MailSetting:mailpassword"]);
                        smtpClient.Host = objconfig["MailSetting:hostname"];
                        smtpClient.Port = Convert.ToInt32(objconfig["MailSetting:Port"]);
                        message.From = new System.Net.Mail.MailAddress(objconfig["MailSetting:mailusername"]);
                        tempTo = To.Split(new char[] { ',' });


                        for (int i = 0; i < tempTo.Length; i++)
                        {
                            message.To.Add(tempTo[i].ToString().Trim());
                        }


                        if (!string.IsNullOrWhiteSpace(CC))
                        {
                            tempCC = CC.Split(',');
                            foreach (var cc in tempCC)
                            {
                                var email = cc.Trim();
                                if (!string.IsNullOrEmpty(email))
                                {
                                    message.CC.Add(email);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(BCC))
                        {
                            tempBCC = BCC.Split(',');
                            foreach (var bcc in tempBCC)
                            {
                                var email = bcc.Trim();
                                if (!string.IsNullOrEmpty(email))
                                {
                                    message.Bcc.Add(email);
                                }
                            }
                        }

                        // if (CC.Trim().ToString() != "")
                        // {
                        //     tempCC = CC.Split(new char[] { ',' });
                        //     for (int i = 0; i < tempCC.Length; i++)
                        //     {
                        //         message.CC.Add(tempCC[i].ToString().Trim());
                        //     }
                        // }
                        // if (BCC.Trim().ToString() != "")
                        // {
                        //     tempBCC = BCC.Split(new char[] { ',' });
                        //     for (int i = 0; i < tempBCC.Length; i++)
                        //     {
                        //         message.Bcc.Add(tempBCC[i].ToString().Trim());
                        //     }
                        // }
                        smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                        smtpClient.EnableSsl = true;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = networkCredential;
                        message.Subject = Subject;
                        if (isHTML)
                        {
                            message.IsBodyHtml = true;
                        }


                        message.Body = Body;
                        if (!string.IsNullOrWhiteSpace(attachment))
                        {
                            message.Attachments.Add(new System.Net.Mail.Attachment(attachment));
                        }
                        smtpClient.Timeout = 200000;
                        smtpClient.Send(message);

                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
            }
        }

    }

}