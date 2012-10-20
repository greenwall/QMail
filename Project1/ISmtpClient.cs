using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace qmail
{
    public interface ISmtpClient : IDisposable
    {

        //
        // Summary:
        //     Sends the specified message to an SMTP server for delivery.
        //
        // Parameters:
        //   message:
        //     A System.Net.Mail.MailMessage that contains the message to send.
        int Send(MailMessage message);

        //
        // Summary:
        //     Sends the specified e-mail message to an SMTP server for delivery. The message
        //     sender, recipients, subject, and message body are specified using System.String
        //     objects.
        //
        // Parameters:
        //   from:
        //     A System.String that contains the address information of the message sender.
        //
        //   recipients:
        //     A System.String that contains the addresses that the message is sent to.
        //
        //   subject:
        //     A System.String that contains the subject line for the message.
        //
        //   body:
        //     A System.String that contains the message body.
        //void Send(string from, string recipients, string subject, string body);

    }
}
