using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace qmail
{
    public class MailAddressMatcher
    {
        private readonly string[] acceptedRecipients;

        public MailAddressMatcher(string[] acceptedRecipients) 
        {
            this.acceptedRecipients = acceptedRecipients;
        }

        bool MatchingMailAddress(string accepted, MailAddress to)
        {
            string toMail = to.Address;
            //int atIndex = accepted.IndexOf('@');
            //string domainPart = accepted.Substring(atIndex + 1);
            return accepted.Equals(toMail);
        }

        bool RecipientAccepted(MailAddress to)
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

        bool RecipientsAccepted(MailAddressCollection tos)
        {
            if (tos!=null)
            {
                //bool allAccepted = true;
                foreach (MailAddress to in tos)
                {
                   // if (RecipientAccepted(to)
                }
            }
            return false;
        }

    }
}
