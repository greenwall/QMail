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

namespace qmail.Test
{
    class TestQMail
    {
        static void Main(string[] args)
        {
            BasicConfigurator.Configure();

            TestFilteringSmtpClient t = new TestFilteringSmtpClient();
            t.TestCasingMailAdress();

            TestQMail p = new TestQMail();
            //p.TestSendMail();
            //p.TestSendMailDirectly();
            p.TestReadMailsFromDropFolder();
            //p.TestArchiveMails();
            
            //p.TestMailData();            

//            p.TestWork();
//            p.TestWork();

            // GMail has a maximum of 500 mails per day.
            //for (int n = 0; n < 1000; n++)
/*
            for (int n = 0; n < 10; n++)
            {
                p.TestEnqueueMail();
                System.Threading.Thread.Sleep(1000);
            }
*/
            Console.ReadLine();
        }

        private void TestArchiveMails()
        {
            QMail qmail = new QMail();
            qmail.ArchiveMails();

        }

        private void TestWork()
        {
            QMail qmail = new QMail();
            qmail.Work();

        }

        private void TestSendMail()
        {
            SendMail("torben.espersen@cybercom.com", "torben.espersen@gmail.com, ams.torben.espersen@cybercom.com", "test", "test");
        }

        private void TestReadMailsFromDropFolder()
        {
            DirectoryInfo dropFolder = new DirectoryInfo(@"c:\temp\maildrop\");
            DirectoryInfo sentFolder = new DirectoryInfo(@"c:\temp\sentmail\");
            FileInfo[] files = dropFolder.GetFiles("*.eml");
            foreach (FileInfo file in files)
            {
                Console.WriteLine(file.FullName);


                Rebex.Mail.MailMessage message = new Rebex.Mail.MailMessage();
                message.Load(file.FullName);

                Console.WriteLine("s={0}, to={1}, from={2}", message.Subject, message.To, message.From);
/*
                // Send mail using Rebex.
                SmtpConfiguration smtpConfiguration = new SmtpConfiguration();
                smtpConfiguration.ServerName = "smtp.gmail.com";
                smtpConfiguration.ServerPort = 587;
                smtpConfiguration.UserName = "test.qmail.20120314";
                smtpConfiguration.Password = "DetteErEnTest";
                smtpConfiguration.Security = SmtpSecurity.Secure;
                //<smtp host="smtp.gmail.com" port="587" ssl="True" username="test.qmail.20120314" password="DetteErEnTest"/>
                Smtp.Send(message, smtpConfiguration);
*/

                MailMessage mm = MailMessageConverter.Convert(message);
                ISmtpClient s1 = new BasicSmtpClient();
                ISmtpClient s2 = new FilteringSmtpClient(s1);
                s2.Send(mm);


                file.MoveTo(sentFolder.FullName+"\\"+file.Name);

            }
        }

        private void TestSendMailDirectly()
        {
            string from = "torben.espersen@cybercom.com";
            string to = "torben.espersen@gmail.com";
            string subject = "test";
            string body = "test";

            try
            {
                MailMessage msg = new MailMessage(from, to, subject, body);
                msg.CC.Add("torben.espersen@cybercom.com");
                msg.CC.Add("ams.torben.espersen@cybercom.com");

                msg.Bcc.Add("torben.espersen@cybercom.com");
                msg.Bcc.Add("ams.torben.espersen@cybercom.com");


                //ISmtpClient s1 = new GmailSmtpClient("test.qmail.20120314", "DetteErEnTest");

                //ISmtpClient s2 = new FilteringSmtpClient(s1, "torben.espersen@gmail.com");
                //s.Host = smtpServer;
                //ISmtpClient s1 = new BasicSmtpClient();
                //ISmtpClient s2 = new FilteringSmtpClient(s1);
                SmtpClient s = new SmtpClient();
                //s.PickupDirectoryLocation = @"c:\mail-drop";
                s.Send(msg);
            }
            catch (Exception ex)
            {
                //Logger.Warn("Failed to send notification e-mail about new user to " + to);
                Console.Write(ex);
                //throw ex;
            }
        }


        public void SendMail(string from, string to, string subject, string body)
        {
            try
            {
                MailMessage msg = new MailMessage(from, to, subject, body);
                msg.CC.Add("torben.espersen@cybercom.com");
                msg.CC.Add("ams.torben.espersen@cybercom.com");

                msg.Bcc.Add("torben.espersen@cybercom.com");
                msg.Bcc.Add("ams.torben.espersen@cybercom.com");
                

                //ISmtpClient s = new GmailSmtpClient("test.qmail.20120314", "DetteErEnTest");

                //ISmtpClient s2 = new FilteringSmtpClient(s1, "torben.espersen@gmail.com");
                //s.Host = smtpServer;
                ISmtpClient s = new BasicSmtpClient();
                //ISmtpClient s = new FilteringSmtpClient(s1);
                s.Send(msg);
            }
            catch (Exception ex)
            {
                //Logger.Warn("Failed to send notification e-mail about new user to " + to);
                Console.Write(ex);
                //throw ex;
            }
        }

        private void TestEnqueueMail()
        {
            ISmtpClient smtpClient = new QueueSmtpClient();

            MailMessage msg = new MailMessage("me@gmail.com", "you@gmail.com");
            msg.Subject = "subject";
            msg.Body = "Body";
            smtpClient.Send(msg);
        }

        private void TestMailData()
        {

            QMail qmail = new QMail();
            //qmail.ReadMarkAndSend();
            qmail.Work();
            Console.WriteLine(Timers.TimersToString());
        }



/*
        public TestQMail()
        {
            string temp = ConfigurationManager.AppSettings["SMTP_SERVER"];
            if (temp != null)
            {
                smtpServer = temp;
            }
            else
            {
                //Logger.Error(1, "No SMTP server defined i configfile. Service is stopped");
            }
            Console.WriteLine("smtpServer=" + smtpServer);
        }
*/


        private SqlConnection GetConnection()
        {
            string connectionString = ConnectionString("MAIL_DB");

            return new SqlConnection(connectionString);
        }


        public long GetNextMail()
        {
            long id = -1;

            using (SqlConnection connection = GetConnection())
            {
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "SELECT top 1 id, [to], [from], subject, body, created FROM tblMail where marked is null order by created asc, id asc ";
                //cmd.CommandType = CommandType.Text;
                cmd.Connection = connection;

                connection.Open();

                using (reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("id={3}, to={0}, from={1}, subject={2}", reader["to"], reader["from"], reader["subject"], reader["id"]);
                            id = (long)reader["id"];
                        }
                    }
                }
            }
            return id;
        }

        private StoredMailMessage MarkNextMail()
        {
            return null;
        }

        private StoredMailMessage ReadMail(SqlDataReader reader)
        {
            long id = (long)reader["id"];
            string to = (string)reader["to"];
            string from = (string)reader["from"];
            
            StoredMailMessage message = new StoredMailMessage(id, from, to);
            message.Subject = (string)reader["subject"];
            message.Body = (string)reader["body"];
            return message;
        }

        public void MarkMailForSending(long id)
        {

            using (SqlConnection connection = GetConnection())
            {
                connection.Open();

                string updateSql = "update tblMail set marked=@Marked where id=@Id and marked is null"; // "UPDATE Employees " + "SET LastName = @LastName " + "WHERE FirstName = @FirstName";
                using (SqlCommand cmd = new SqlCommand(updateSql, connection))
                {

                    cmd.Parameters.Add("@Marked", SqlDbType.DateTime);
                    cmd.Parameters.Add("@Id", SqlDbType.BigInt);

                    cmd.Parameters["@Marked"].Value = DateTime.Now;
                    cmd.Parameters["@Id"].Value = id;

                    int rowsUpdated = cmd.ExecuteNonQuery();

                    if (rowsUpdated != 1)
                    {
                        throw new Exception("Email "+id+" was already marked for sending. No retransmission.");
                    }
                }
            }
        }

        public static string ConnectionString(string key)
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[key];
            if (connectionStringSettings == null || string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
            {
                throw new ConfigurationErrorsException("The connectionStrings section is missing the " + key + " connection string");
            }
            return connectionStringSettings.ConnectionString;
        }

    }
}
