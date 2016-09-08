using ShopRosKvartal.Models.SiteTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace ShopRosKvartal.LibraryHelperClasses.SMTPServer
{
    public static class MailSender
    {
        public static bool SendRmail(ToolsSMTPSetting smtpSetting, string code, string title, string body, string email)
        {
            bool result = false;

            //
            var message = new MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(smtpSetting.EmailFrom);
            message.Subject = title + " " + "Интернет магазин - \"РосКвартал\"";
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = smtpSetting.UserName,
                    Password = smtpSetting.Password
                };
                smtp.Credentials = credential;
                smtp.Host = smtpSetting.Host;
                smtp.Port = smtpSetting.Port;
                smtp.EnableSsl = smtpSetting.EnableSsl;
                try
                {
                    smtp.Send(message);
                    result=true;
                }
                catch
                {
                    result = false;
                }
                
            }
            return result;
        }
    }
}