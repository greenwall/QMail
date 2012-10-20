using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace qmail
{
    public class StoredMailMessage : MailMessage
    {
        public long id { get; private set; }

        public string filename { get; private set; }

        public StoredMailMessage(string filename)
        {
            this.filename = filename;
        }

        public StoredMailMessage(long id, string from, string to) : base (from, to)
        {
            this.id = id;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public string ReceiversToString()
        {
            return ReceiversToString(this);
        }

        public static string ReceiversToString(MailMessage message)
        {
            
            StringBuilder builder = new StringBuilder();
            string to = string.Join(",", message.To);
            string cc = string.Join(",", message.CC);
            string bcc = string.Join(",", message.Bcc);

            builder.Append("{");
            builder.Append("to=");
            builder.Append(to);
            builder.Append(";cc=");
            builder.Append(cc);
            builder.Append(";bcc=");
            builder.Append(bcc);
            builder.Append("}");

            return builder.ToString();
        }

        public static IEqualityComparer<MailAddress> MAIL_ADDRESS_COMPARER = new MailAddressComparer();

        class MailAddressComparer : IEqualityComparer<MailAddress>
        {
            public bool Equals(MailAddress x, MailAddress y)
            {
                return x.Address.ToLower().Equals(y.Address.ToLower());
            }

            public int GetHashCode(MailAddress obj)
            {
                return obj.Address.ToLower().GetHashCode();
            }
        }

    }
}
