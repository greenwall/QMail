using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;

using log4net;

using qmail.Data;

namespace qmail
{
    /// <summary>
    /// QueueSmtpClient corresponds to the standard SmtpClient, but writes mail messages to the database according to the given configuration.
    /// The Windows service QMailService will transmit mails from here asyncronously.
    /// </summary>
    public class QueueSmtpClient : ISmtpClient
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly MailDao dao;

        public QueueSmtpClient()
        {
            dao = new MailDao();
        }

        public int Send(MailMessage message)
        {
            dao.EnqueueMail(message);
            return 0;
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
