using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace qmail
{

    public class MailsStat
    {
        private readonly DateTime sinceDate;
        private readonly int created, sent, failed, archived;

        public MailsStat(DateTime since, int created, int sent, int failed, int archived)
        {
            this.sinceDate = since;
            this.created = created;
            this.sent = sent;
            this.failed = failed;
            this.archived = archived;
        }

        public DateTime SinceDate { get { return sinceDate; } }
        public int Created { get { return created; } }
        public int Sent { get { return sent; } }
        public int Failed { get { return failed; } }
        public int Archived { get { return archived; } }
    }

    interface IMailDao
    {
        StoredMailMessage GetNextMail();
        
        void MarkMailForSending(StoredMailMessage message);

        void MailSent(StoredMailMessage message);

        void MailFailed(StoredMailMessage message);

        void MailIgnored(StoredMailMessage message);

        void EnqueueMail(MailMessage message);

        void ArchiveMailsOlderThan(DateTime archiveDate);

        void DeleteArchivedMailsOlderThan(DateTime deleteDate);

        int CountMailsSentSince(DateTime sinceDate);

        int Count();

        MailsStat MailsSince(DateTime sinceDate);

        IEnumerable<StoredMailMessage> SearchMails(int currentPage, int pageSize = 10, string toFilter = null, string subjectFilter = null);
    }
}
