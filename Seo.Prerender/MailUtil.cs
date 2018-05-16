using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using log4net;
namespace Seo.Prerender
{
    public class MailUtil
    {
        
        private static readonly ILog Log = LogManager.GetLogger(typeof(MailUtil).Name);
        public static void SendMail(string mailfrom, string receiver, string subject, string body, string pageName,
            string mailServer, int ServicePort)
        {

            MailMessage message = new MailMessage();
            try
            {
                message.To.Add(receiver);
                message.Subject = subject;
                message.From = new MailAddress(mailfrom);
                message.Body = body;
                message.IsBodyHtml = true;
                Log.Debug(string.Format("Page {0} sent email to [{1}]. Content:{2}", pageName, receiver, body));

                SmtpClient smtp = new SmtpClient(mailServer, ServicePort);
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                Log.Fatal(string.Format("sent email fail: [{0}].", ex.Message));
            }
        }
    }
}
