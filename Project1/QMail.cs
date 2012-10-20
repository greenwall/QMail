using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Timers;

using log4net;
using log4net.Config;

using Util.Profiling;
using qmail.Test;
using qmail.Data;

namespace qmail
{
    public class QMail
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISmtpClient smtpClient;
        private readonly IMailDao dao;
        private Timer serviceTimer;
        
        private readonly int maxMailsToSend;
        private readonly int maxMinutesToRun;
        private readonly int maxMailsPerHour;
        private readonly int maxMailsPerDay;
        private readonly bool isExitOnEmptyQueue;
        private readonly int checkIntervalMinutes;
        private readonly int archiveSentMailsAfterHours;
        private readonly int deleteArchivedMailsAfterHours;

        private int mailsSent = 0;
        private DateTime startTime = DateTime.Now;

        public QMail()
        {

            QMailConfigurationSection config = (QMailConfigurationSection)ConfigurationManager.GetSection("qmailConfiguration");

//            maxMailsToSend = ConfigHelper.DefaultInt("qmail.max-mails-to-send", 0);
//            log.Debug("qmail.max-mails-to-send=" + maxMailsToSend);

//            maxMinutesToRun = ConfigHelper.DefaultInt("qmail.max-minutes-to-run", 0);
//            log.Debug("qmail.max-minutes-to-run=" + maxMinutesToRun);

            maxMailsPerHour = config.MaxMailsPerHour; // ConfigHelper.DefaultInt("qmail.max-mails-per-hour", 0);
            log.Info("qmail.max-mails-per-hour=" + maxMailsPerHour);

            maxMailsPerDay = config.MaxMailsPerDay; // ConfigHelper.DefaultInt("qmail.max-mails-per-day", 0);
            log.Info("qmail.max-mails-per-day=" + maxMailsPerDay);
            
//            isExitOnEmptyQueue = ConfigHelper.DefaultBool("qmail.exit-on-empty-queue", true);
//            log.Debug("qmail.exit-on-empty-queue=" + isExitOnEmptyQueue);

//            checkIntervalMinutes = ConfigHelper.DefaultInt("qmail.check-interval-minutes", 1);
//            checkIntervalMinutes = Math.Max(1, checkIntervalMinutes);
            checkIntervalMinutes = config.CheckIntervalMinutes;
            log.Info("qmail.check-interval-minutes=" + checkIntervalMinutes);

            //archiveSentMailsAfterHours = ConfigHelper.DefaultInt("qmail.archive-sent-mails-after-hours", 24);
            archiveSentMailsAfterHours = config.ArchiveSentMailsAfterHours;
            log.Info("qmail.archive-sent-mails-after-hours=" + archiveSentMailsAfterHours);

            deleteArchivedMailsAfterHours = config.DeleteArchivesMailsAfterHours;
            log.Info("qmail.delete-archived-mails-after-hours=" + deleteArchivedMailsAfterHours);

            if (config.Folders != null && config.Folders.PickupFolder != null && config.Folders.SentFolder != null)
            {
                dao = new FileMailDao();
            }
            else
            {
                if (config.MailTable != null && config.ArchiveMailTable != null)
                {
                    dao = new MailDao();
                }
            }

            ISmtpClient basic = new BasicSmtpClient();
            //ISmtpClient basic = new TestSmtpClient();
            ISmtpClient filter = new FilteringSmtpClient(basic);
            smtpClient = filter;


        }

        /// <summary>
        /// Sets up a timer invoking Work at every checkIntervalMinutes
        /// </summary>
        public void Start()
        {
            long interval = 60 * 1000 * checkIntervalMinutes;

            serviceTimer = new Timer();
            serviceTimer.Elapsed += new ElapsedEventHandler(Work);
            serviceTimer.Interval = interval;
            serviceTimer.Enabled = true;
            serviceTimer.Start();

        }

        public void Stop()
        {
            serviceTimer.Enabled = false;
        }

        public IEnumerable<MailsStat> MailsSince(DateTime[] sinceDates)
        {
            List<MailsStat> stats = new List<MailsStat>();
            foreach (DateTime since in sinceDates)
            {
                stats.Add(dao.MailsSince(since));
            }
            return stats;
        }

        public int CountMails()
        {
            return dao.Count();
        }

        public IEnumerable<StoredMailMessage> SearchMails(int currentPage, int pageSize, string toFilter = null, string subjectFilter = null)
        {
            return dao.SearchMails(currentPage, pageSize, toFilter, subjectFilter);
        }

        /// <summary>
        /// Invoked every checkIntervalMinutes to transmit and archive mails.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Work(object sender = null, EventArgs args = null)
        {
            bool run = true;
            while (run)
            {
                DateTime lastHour = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
                int sentLastHour = dao.CountMailsSentSince(lastHour);

                if (sentLastHour >= maxMailsPerHour)
                {
                    run = false;
                    log.Info("QMail reached maxMailsPerHour limit: " + sentLastHour); 
                }

                DateTime lastDay = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));
                int sentLastDay = dao.CountMailsSentSince(lastDay);

                if (sentLastDay >= maxMailsPerDay)
                {
                    run = false;
                    log.Info("QMail reached maxMailsPerDay limit: " + sentLastDay);
                }

                if (run)
                {
                    run = ReadMarkAndSend();
                }
/*
                if (isExitOnEmptyQueue && !moreMails)
                {
                    run = false;
                }
                if (maxMailsToSend>0 && mailsSent >= maxMailsToSend)
                {
                    run = false;
                }
                if (maxMinutesToRun>0 && elapsedMinutes()>=maxMinutesToRun) 
                {
                    run = false;
                }
*/
            }

            ArchiveMails();
            DeleteArchivedMails();
        }

        public void ArchiveMails()
        {
            DateTime archiveDate = DateTime.Now.Subtract(new TimeSpan(archiveSentMailsAfterHours, 0, 0));
            dao.ArchiveMailsOlderThan(archiveDate);
        }

        public void DeleteArchivedMails()
        {
            DateTime deleteDate = DateTime.Now.Subtract(new TimeSpan(deleteArchivedMailsAfterHours, 0, 0));
            dao.DeleteArchivedMailsOlderThan(deleteDate);
        }

        private long elapsedMinutes()
        {
            long startMillis = startTime.Ticks / 10000;
            long nowMillis = System.DateTime.Now.Ticks / 10000;

            long elapsedMillis = nowMillis - startMillis;
            return elapsedMillis / (1000 * 60);
        }

        public bool ReadMarkAndSend()
        {

            StoredMailMessage msg = dao.GetNextMail();
            if (msg != null)
            {
                using (Timers.Time("QMail.ReadMarkAndSend#mark mail"))
                {
                    dao.MarkMailForSending(msg);
                }
                try
                {
                    int mails;
                    using (Timers.Time("QMail.ReadMarkAndSend#smtp send"))
                    {
                        mails = smtpClient.Send(msg);
                        mailsSent += mails;
                    }
                    if (mails > 0)
                    {
                        using (Timers.Time("QMail.ReadMarkAndSend#mail sent"))
                        {
                            dao.MailSent(msg);
                        }
                    }
                    else
                    {
                        // Mail ignored
                        using (Timers.Time("QMail.ReadMarkAndSend#mail ignored"))
                        {
                            dao.MailIgnored(msg);
                        }
                    }
                }
                catch (SmtpException)
                {
                    dao.MailFailed(msg);
                }
                return true;
            }
            return false;
        }


    }

}
