using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util.Profiling;
using System.Net.Mail;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using log4net.Config;

//using Rebex.Mail;
using Rebex.Net;

using qmail.Test;
using qmail.Data;
using System.IO;
using System.Diagnostics;


namespace qmail.Test
{
    public class TestFilteringSmtpClient
    {

        public void TestCasingMailAdress()
        {
            string A = "Aaa@bc.de";
            string a = "aaa@bc.de";

            MailAddress mA = new MailAddress(A);
            MailAddress ma = new MailAddress(a);

            Debug.Assert(ma.Equals(mA));

            MailMessage m1 = new MailMessage();
            m1.To.Add(mA);

            HashSet<MailAddress> whitelist = new HashSet<MailAddress>();
            whitelist.Add(ma);

            // Apparently the default comparer is case sensitive on mail addresses.
            //IEnumerable<MailAddress> to = m1.To.Intersect<MailAddress>(whitelist).ToList<MailAddress>();
            IEnumerable<MailAddress> to = m1.To.Intersect<MailAddress>(whitelist, StoredMailMessage.MAIL_ADDRESS_COMPARER).ToList<MailAddress>();

            Debug.Assert(to.Count() == 1);
            Debug.Assert(to.Contains(mA));
        }

        public void TestCasing()
        {
            string A = "Aaa@bc.de";
            string a = "aaa@bc.de";

            FilteringSmtpClient client = new FilteringSmtpClient(new TestSmtpClient());

            MailMessage test = new MailMessage();
            test.To.Add(A);

            client.RemoveNonWhitelistedRecipients(test);

            Debug.Assert(test.To.Count == 1);
            Debug.Assert(test.To.Contains(new MailAddress(a)));

        }

        public void Test()
        {

            string A = "a@bc.de";
            string B = "b@bc.de";
            string C = "c@bc.de";
            string D = "d@bc.de";

//            FilteringSmtpClient client = new FilteringSmtpClient(new TestSmtpClient(), "override@bc.de", new string[] {B, A } );
            FilteringSmtpClient client = new FilteringSmtpClient(new TestSmtpClient());

            MailMessage test = new MailMessage();
            test.To.Add(A);
            test.To.Add(B);
            test.To.Add(C);

            test.CC.Add(B);
            test.CC.Add(C);

            test.Bcc.Add(C);
            test.Bcc.Add(A);

            client.RemoveNonWhitelistedRecipients(test);

            Debug.Assert(test.To.Count == 2);
            Debug.Assert(test.To.Contains(new MailAddress(A)));
            Debug.Assert(test.To.Contains(new MailAddress(B)));

            Debug.Assert(test.CC.Count == 1);
            Debug.Assert(test.CC.Contains(new MailAddress(B)));

            Debug.Assert(test.Bcc.Count == 1);
            Debug.Assert(test.Bcc.Contains(new MailAddress(A)));
        
        }

    }
}
