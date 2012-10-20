using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

using log4net;

namespace qmail
{
    public class GmailSmtpClient : ISmtpClient
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string username;
        private readonly string password;

        public GmailSmtpClient(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public int Send(MailMessage message)
        {
            try
            {
                //MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(username, password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(message);
                log.Debug("Mail sent to:" + message.To);
                return 1;
            }
            catch (Exception ex)
            {
                log.Error("Failed sending mail to " + message.To, ex);
                return 0;
            }
        }
/*
        public void Send(string from, string recipients, string subject, string body)
        {
            MailMessage msg = new MailMessage(from, recipients, subject, body);
            Send(msg);
        }
*/
        public void Dispose()
        {
        }
    }
}
