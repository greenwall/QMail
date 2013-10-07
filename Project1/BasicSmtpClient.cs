using System;
using System.Net.Mail;
using System.Configuration;

using log4net;

namespace qmail
{
    public class BasicSmtpClient : ISmtpClient
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string host = null;
        private readonly int port = 22;
        private readonly string username = null;
        private readonly string password = null;
        private readonly bool enableSsl = false;
        private readonly SmtpClient smtpClient = null;

        public BasicSmtpClient()
        {
            QMailConfigurationSection config = (QMailConfigurationSection)ConfigurationManager.GetSection("qmailConfiguration");
            SmtpElement smtp = config.Smtp;

            if (!string.IsNullOrEmpty(smtp.Host))
            {

                host = smtp.Host; // ConfigHelper.RequiredString("qmail.smtp.host");
                port = smtp.Port; // ConfigHelper.DefaultInt("qmail.smtp.port", port);

                username = smtp.Username; // ConfigHelper.DefaultString("qmail.smtp.username", username);
                password = smtp.Password; // DefaultString("qmail.smtp.password", password);

                enableSsl = smtp.Ssl; // ConfigHelper.DefaultBool("qmail.smtp.ssl", enableSsl);

                smtpClient = new SmtpClient(host);
                smtpClient.Port = port;
                if (username != null)
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
                }
                smtpClient.EnableSsl = enableSsl;
            }
            else
            {
                // Use default configuration.
                smtpClient = new SmtpClient();
            }
        }

        public int Send(MailMessage message)
        {
            try
            {
                smtpClient.Send(message);
                string cc = string.Join(",", message.CC);
                string bcc = string.Join(",", message.Bcc);
                log.Info("Mail sent to:" + message.To + " cc:" + cc + " bcc:" + bcc);
                return 1;
            }
            catch (Exception ex)
            {
                string cc = string.Join(",", message.CC);
                string bcc = string.Join(",", message.Bcc);
                log.Error("Failed sending mail to:"+ message.To + " cc:" + cc + " bcc:" + bcc, ex);
                throw ex;
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
            smtpClient.Dispose();
        }
    }
}
