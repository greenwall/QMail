using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;

using Rebex.Net;
using System.Collections.Specialized;

namespace qmail
{
    public class MailMessageConverter
    {
        public static MailMessage Convert(Rebex.Mail.MailMessage rmm) {
            MailMessageConverter converter = new MailMessageConverter();
            return converter.ToMailMessage(rmm);
        }

        public static void ConvertTo(Rebex.Mail.MailMessage rmm, MailMessage mm)
        {
            MailMessageConverter converter = new MailMessageConverter();
            converter.CopyToMailMessage(rmm, mm);
        }

        public MailMessage ToMailMessage(Rebex.Mail.MailMessage rmm)
        {
            MailMessage mm = new MailMessage();
            CopyToMailMessage(rmm, mm);
            return mm;
        }

        public void CopyToMailMessage(Rebex.Mail.MailMessage rmm, MailMessage mm)
        {

            //public AlternateViewCollection AlternateViews { get; }
            //public AttachmentCollection Attachments { get; }
            CopyToAttachmentCollection(rmm.Attachments, mm.Attachments);

            //public MailAddressCollection Bcc { get; }
            CopyToMailAddressCollection(rmm.Bcc, mm.Bcc);


            
            //public string Body { get; set; }
            //public bool IsBodyHtml { get; set; }
            if (!string.IsNullOrEmpty(rmm.BodyHtml))
            {
                // Body is html
                mm.IsBodyHtml = true;
                mm.Body = rmm.BodyHtml;
            }
            else
            {
                if (!string.IsNullOrEmpty(rmm.BodyText))
                {
                    // body is text
                    mm.IsBodyHtml = false;
                    mm.Body = rmm.BodyText;
                }
                else
                {
                    // Body is empty
                    mm.IsBodyHtml = false;
                    mm.Body = string.Empty;
                }
            }

            //public Encoding BodyEncoding { get; set; }
            mm.BodyEncoding = rmm.DefaultCharset;

            //public MailAddressCollection CC { get; }
            CopyToMailAddressCollection(rmm.CC, mm.CC);

            //public DeliveryNotificationOptions DeliveryNotificationOptions { get; set; }
            // Not supported by rebex.

            //public MailAddress From { get; set; }
            MailAddressCollection mac = ToMailAddressCollection(rmm.From);
            if (mac.Count >= 1)
            {
                mm.From = mac[0];
            }
        
            //public System.Collections.Specialized.NameValueCollection Headers { get; }
            CopyToHeaderCollection(rmm.Headers, mm.Headers);

            //public Encoding HeadersEncoding { get; set; }
            mm.HeadersEncoding = rmm.DefaultCharset;

            //public MailPriority Priority { get; set; }
            mm.Priority = ToPriority(rmm.Priority);


            //[Obsolete("ReplyTo is obsoleted for this type.  Please use ReplyToList instead which can accept multiple addresses. http://go.microsoft.com/fwlink/?linkid=14202")]
            //public MailAddress ReplyTo { get; set; }
            //public MailAddressCollection ReplyToList { get; }
            CopyToMailAddressCollection(rmm.ReplyTo, mm.ReplyToList);

            //public MailAddress Sender { get; set; }
            mm.Sender = ToMailAddress(rmm.Sender);

            //public string Subject { get; set; }
            mm.Subject = rmm.Subject;

            //public Encoding SubjectEncoding { get; set; }
            mm.SubjectEncoding = rmm.DefaultCharset;

            //public MailAddressCollection To { get; }
            CopyToMailAddressCollection(rmm.To, mm.To);
        }

        private void CopyToAttachmentCollection(Rebex.Mail.AttachmentCollection rac, AttachmentCollection ac)
        {
            foreach (Rebex.Mail.Attachment ra in rac) 
            {                
                ac.Add(ToAttachment(ra));
            }
        }

        private Attachment ToAttachment(Rebex.Mail.Attachment ra)
        {
            System.Net.Mime.ContentType contentType = ToContentType(ra.ContentType);            
            Attachment a = new Attachment(ra.GetContentStream(), contentType);
            return a;
        }

        private System.Net.Mime.ContentType ToContentType(Rebex.Mime.Headers.ContentType rct)
        {
            System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(rct.MediaType);
            ct.MediaType = rct.MediaType;
            ct.Boundary = rct.Boundary;
            ct.CharSet = rct.CharSet;
            Rebex.Mime.Headers.MimeParameterList mpl = rct.Parameters;
            foreach (string name in mpl.Names) 
            {
                ct.Parameters.Add(name, mpl[name]);
            }
            return ct;
        }



        public void CopyToHeaderCollection(Rebex.Mime.MimeHeaderCollection mhc, NameValueCollection nvc)
        {
            foreach (Rebex.Mime.MimeHeader mh in mhc) 
            {                
                // Guessing that MimeHeader.Value.ToString() returns the value of the header?
                nvc.Add(mh.Name, mh.Value.ToString());
            }
        }

        public MailPriority ToPriority(Rebex.Mail.MailPriority mp) {
            switch (mp) {
            case Rebex.Mail.MailPriority.High:
                return MailPriority.High;
            case Rebex.Mail.MailPriority.Low:
                return MailPriority.Low;
            case Rebex.Mail.MailPriority.Normal:
                return MailPriority.Normal;
            default:
                throw new Exception("Unknown mail-priority = " + mp); 
            }
        }

        public MailAddressCollection ToMailAddressCollection(Rebex.Mime.Headers.MailAddressCollection rmac)
        {
            MailAddressCollection mac = new MailAddressCollection();
            CopyToMailAddressCollection(rmac, mac);
            return mac;
        }

        public void CopyToMailAddressCollection(Rebex.Mime.Headers.MailAddressCollection rmac, MailAddressCollection mac)
        {
            foreach (Rebex.Mime.Headers.MailAddress rma in rmac) 
            {
                mac.Add(ToMailAddress(rma));
            }
        }


        public MailAddress ToMailAddress(Rebex.Mime.Headers.MailAddress rma)
        {
            if (rma != null)
            {
                MailAddress ma = new MailAddress(rma.Address, rma.DisplayName);
                return ma;
            }
            else
            {
                return null;
            }
        }
    }
}
