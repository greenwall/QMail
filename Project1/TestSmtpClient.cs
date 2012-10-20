using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using qmail;
using log4net;

namespace qmail.Test
{
    public class TestSmtpClient : ISmtpClient
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int Send(MailMessage message)
        {
            log.Info("Sending mail to:" + message.To + " from:" + message.From + " subject:" + message.Subject);
            //throw new SmtpException("TestSmtpClient throws SmtpException");
            return 1;
        }
/*
        public void Send(string from, string recipients, string subject, string body)
        {
            log.Info("Sending mail to:" + recipients + " from:" + from + " subject:" + subject);
            //throw new SmtpException("TestSmtpClient throws SmtpException");
        }
*/
        public void Dispose()
        {
        }
    }
}
