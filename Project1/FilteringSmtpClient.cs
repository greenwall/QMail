using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Net.Mail;

using log4net;

namespace qmail
{
    public class FilteringSmtpClient : ISmtpClient
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISmtpClient delegateTo;
        //private readonly string[] acceptedRecipients;
        private readonly MailAddress overridingRecipient;
        private HashSet<MailAddress> whitelist = new HashSet<MailAddress>();

        public FilteringSmtpClient(ISmtpClient delegateTo)
        {
            QMailConfigurationSection config = (QMailConfigurationSection)ConfigurationManager.GetSection("qmailConfiguration");
            FilterElement filter = config.Filter;
            
            this.delegateTo = delegateTo;
            string to = filter.OverrideTo; // ConfigHelper.DefaultString("qmail.mail.to", null);
            if (!string.IsNullOrEmpty(to))
            {
                overridingRecipient = new MailAddress(to);
            }

            ToMailAddressElementCollection mailAddresses = filter.Whitelist;

            for (int n = 0; n < mailAddresses.Count; n++) 
            {
                string mailAddress = mailAddresses[n].MailAddress;
                whitelist.Add(new MailAddress(mailAddress));
            }

        }

        public FilteringSmtpClient(ISmtpClient delegateTo, string overridingRecipient, string[] whitelist)
            : this(delegateTo)
        {
            this.overridingRecipient = new MailAddress(overridingRecipient);

            foreach (string mailAddress in whitelist)
            {
                this.whitelist.Add(new MailAddress(mailAddress));
            }
        }
/*
        public FilteringSmtpClient(ISmtpClient delegateTo, string[] acceptedRecipients, string overridingRecipient)
            : this(delegateTo, overridingRecipient)
        {
            this.acceptedRecipients = acceptedRecipients;
        }

/*
        private bool MatchingMailAddress(string accepted, MailAddress to)
        {
            string toMail = to.Address;
            //int atIndex = accepted.IndexOf('@');
            //string domainPart = accepted.Substring(atIndex + 1);
            return accepted.Equals(toMail);
        }

        private bool RecipientAccepted(MailAddress to)
        {
            if (acceptedRecipients == null || acceptedRecipients.Length == 0)
            {
                return false;
            }
            else
            {
                foreach (string acceptedRecipient in acceptedRecipients)
                {
                    if (MatchingMailAddress(acceptedRecipient, to))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool RecipientsAccepted(MailAddressCollection tos)
        {
            if (tos!=null)
            {
                bool allAccepted = true;
                foreach (MailAddress to in tos)
                {
                   // if (RecipientAccepted(to)
                }
            }
            return false;
        }

        public void Send(MailMessage message)
        {
            if (RecipientsAccepted(message.To))
            {
                delegateTo.Send(message);
            }
            else
            {
                MailMessage newMessage = new MailMessage(message.From, overridingRecipient);
                newMessage.Subject = message.Subject;
                newMessage.Body = message.Body;
                foreach (Attachment attachment in message.Attachments)
                {
                    newMessage.Attachments.Add(attachment);
                }
            }
        }
*/
        public int Send(MailMessage message)
        {
            string originalReceivers = StoredMailMessage.ReceiversToString(message);
            RemoveNonWhitelistedRecipients(message);

            if (overridingRecipient != null)
            {
                //MailMessage newMessage = OverrideRecipients(message);
                if (message.To.Count == 0)
                {
                    message.To.Add(overridingRecipient);
                }
                string filteredReceivers = StoredMailMessage.ReceiversToString(message);
                log.Info("Receivers changed from: " + originalReceivers + " to: " + filteredReceivers);              
                return delegateTo.Send(message);
            }
            else
            {
                if (message.To.Count + message.CC.Count + message.Bcc.Count > 0)
                {
                    string filteredReceivers = StoredMailMessage.ReceiversToString(message);
                    log.Info("Receivers changed from: " + originalReceivers + " to: " + filteredReceivers);
                    return delegateTo.Send(message);
                }
                else
                {
                    // No recipients left. Ignore sending.
                    log.Info("All receivers removed: " + originalReceivers + ". Mail ignored.");
                    return 0;
                }
            }
        }

        public void RemoveNonWhitelistedRecipients(MailMessage message)
        {
            // Apparently Intersect<MailAddress> does not match different casing in mail addresses.
            IEnumerable<MailAddress> to = message.To.Intersect<MailAddress>(whitelist, StoredMailMessage.MAIL_ADDRESS_COMPARER).ToList<MailAddress>();
            IEnumerable<MailAddress> cc = message.CC.Intersect<MailAddress>(whitelist, StoredMailMessage.MAIL_ADDRESS_COMPARER).ToList<MailAddress>();
            IEnumerable<MailAddress> bcc = message.Bcc.Intersect<MailAddress>(whitelist, StoredMailMessage.MAIL_ADDRESS_COMPARER).ToList<MailAddress>();

            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            foreach (MailAddress ma in to)
            {
                message.To.Add(ma);
            }

            foreach (MailAddress ma in cc)
            {
                message.CC.Add(ma);
            }

            foreach (MailAddress ma in bcc)
            {
                message.Bcc.Add(ma);
            }

        }
        
        public MailMessage OverrideRecipients(MailMessage message)
        {
            message.To.Clear();
            message.CC.Clear();
            message.Bcc.Clear();

            message.To.Add(overridingRecipient);
            return message;


/*            
            MailAddressCollection tos = message.To;
            MailAddressCollection ccs = message.CC;
            MailAddressCollection bccs = message.Bcc;

            string toEmails = String.Join<MailAddress>(",", tos);
            string ccEmails = String.Join<MailAddress>(",", ccs);
            string bccEmails = String.Join<MailAddress>(",", bccs);

//            StringBuilder builder = new StringBuilder();
//            builder.AppendLine("QMail overwrote original recipients. These were:");
//            builder.Append("To:  ").AppendLine(toEmails);
//            builder.Append("CC:  ").AppendLine(ccEmails);
//            builder.Append("BCC: ").AppendLine(bccEmails);
//            builder.AppendLine();
//            builder.Append(message.Body);
//            string newBody = builder.ToString();

            MailMessage newMessage = new MailMessage(message.From, overridingRecipient);
            newMessage.CC.Clear();            
            newMessage.Bcc.Clear();
            newMessage.Subject = message.Subject;
//            newMessage.Body = newBody;
            newMessage.Body = message.Body;
            
            newMessage.BodyEncoding = message.BodyEncoding;
            newMessage.HeadersEncoding = message.HeadersEncoding;
            newMessage.Headers.Add(message.Headers);
            newMessage.IsBodyHtml = message.IsBodyHtml;
            newMessage.Priority = message.Priority;
            //newMessage.ReplyToList.Add(message.ReplyToList;
            newMessage.Sender = message.Sender;
            newMessage.SubjectEncoding = message.SubjectEncoding;


            foreach (Attachment attachment in message.Attachments)
            {
                newMessage.Attachments.Add(attachment);
            }
            return newMessage;
*/
        }
        

        public void Dispose()
        {
            delegateTo.Dispose();
        }
    }
}
