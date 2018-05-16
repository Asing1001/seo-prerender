using System;
using System.Net.Mail;
using log4net;
using Seo.Prerender.Models;

namespace Seo.Prerender.Helpers
{
    internal class MailHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MailHelper).Name);
        private static SmtpClient _smtp;
        public static void SendMail(MailMessage message)
        {
            try
            {
                Log.Debug(string.Format("sent email to [{0}], Subject:{1}, Body:{2}", message.To, message.Subject, message.Body));
                _smtp.Send(message);
            }
            catch (Exception ex)
            {
                Log.Fatal(string.Format("sent email fail: [{0}].", ex.Message));
            }
        }

        public static void Initialize(MailNotifySetting mailNotifySetting)
        {
            _smtp = new SmtpClient(mailNotifySetting.MailServer, mailNotifySetting.Port);
        }
    }
}
