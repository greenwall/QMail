using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;

using log4net;

using Util.Data;
using qmail;

namespace qmail.Data
{
    public class MailDao: IMailDao
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string tableName;
        private readonly string idColumn;        
        private readonly string toColumn;
        private readonly string fromColumn;
        private readonly string subjectColumn;
        private readonly string bodyColumn;
        private readonly string createdColumn;
        private readonly string sendingColumn;
        private readonly string sentColumn;
        private readonly string failedColumn;
        private readonly string archiveTableName;

        private readonly string getNextMail;
        private readonly DbTemplate dbTemplate;

        public MailDao()
        {
            QMailConfigurationSection config = (QMailConfigurationSection)ConfigurationManager.GetSection("qmailConfiguration");
            MailTableElement mailTable = config.MailTable;
            MailTableElement archiveMailTable = config.ArchiveMailTable;

            tableName = mailTable.TableName; // ConfigHelper.DefaultString("qmail.data.table", "qmail");
            archiveTableName = archiveMailTable.TableName; // ConfigHelper.DefaultString("qmail.data.archive", "archive");
            idColumn = mailTable.Id; // ConfigHelper.DefaultString("qmail.data.id", "id");
            toColumn = mailTable.To; // ConfigHelper.DefaultString("qmail.data.to", "to");
            fromColumn = mailTable.From; // ConfigHelper.DefaultString("qmail.data.from", "from");
            subjectColumn = mailTable.Subject; // ConfigHelper.DefaultString("qmail.data.subject", "subject");
            bodyColumn = mailTable.Body; // ConfigHelper.DefaultString("qmail.data.body", "body");
            createdColumn = mailTable.Created; // ConfigHelper.DefaultString("qmail.data.created", "created");
            sendingColumn = mailTable.Sending; // ConfigHelper.DefaultString("qmail.data.sending", "sending");
            sentColumn = mailTable.Sent; // ConfigHelper.DefaultString("qmail.data.sent", "sent");
            failedColumn = mailTable.Failed; // ConfigHelper.DefaultString("qmail.data.failed", "failed");

            string allColumnsAs = idColumn + " as [id], " + toColumn + " as [to], " + fromColumn + " as [from], " + subjectColumn + " as [subject], " + bodyColumn + " as [body], " + createdColumn + " as [created]";
            string allColumnsAsFromTable = allColumnsAs + " from " + tableName;
            getNextMail = "SELECT top 1 "+allColumnsAsFromTable+" where "+sendingColumn+" is null order by "+createdColumn+" asc, "+idColumn+" asc ";

            dbTemplate = new DbTemplate("qmail.database");

            verifyTables();
        }

        private void verifyTables()
        {
            // Test mail table
            dbTemplate.Count("select count(*) from " + tableName);

            // Test archive table
            try
            {
                dbTemplate.Count("select count(*) from " + archiveTableName);
            }
            catch (Exception e)
            {
                throw new Exception("Achive table named '" + archiveTableName + "' not found.", e);
            }

            // Test id
            dbTemplate.SelectOne("select top 1 " + idColumn + " as [id] from " + tableName + " order by " + idColumn, DummyReader);
        }

//        private delegate T ObjectReader<T>(SqlDataReader reader);
        private object DummyReader(SqlDataReader reader)
        {
            return null;
        }

        public int countMails()
        {
            return dbTemplate.Count("select count(*) from " + tableName);
        }

        public int Count()
        {
            return countMails();
        }


        private string AddConditions(string selectStmt, string[] conditions)
        {
            StringBuilder sb = new StringBuilder(selectStmt);
            bool first = true;
            foreach (string condition in conditions)
            {
                if (first)
                {
                    sb.Append(" WHERE ");
                    first = false;
                }
                else
                {
                    sb.Append(" AND ");
                }
                sb.Append(condition);
            }

            return sb.ToString();
        }

        public IEnumerable<StoredMailMessage> SearchMails(int currentPage, int pageSize, string toFilter = null, string subjectFilter = null)
        {
            // Page numbering starts with 1: 
            // page    first    last   (pagesize=10)
            // ---------------------
            // 1       1        10
            // 2       11       20
            // 3       21       30

            currentPage = Math.Max(1, currentPage);
            pageSize = Math.Max(1, pageSize);

            int firstRow = (currentPage-1)*pageSize+1;
            int lastRow = firstRow+pageSize-1;

            string subSelectWithRow = "SELECT  ROW_NUMBER() OVER(ORDER BY "+idColumn+") AS row, * FROM "+tableName;

            IList<string> conditions = new List<string>();
            if (!string.IsNullOrEmpty(toFilter))
            {
                conditions.Add(toColumn + " LIKE '" + toFilter+"'");
            }

            if (!string.IsNullOrEmpty(subjectFilter))
            {
                conditions.Add(subjectColumn + " LIKE '" + subjectFilter+"'");
            }

            subSelectWithRow = AddConditions(subSelectWithRow, conditions.ToArray<string>());

            string stmt = "SELECT * FROM ( "+subSelectWithRow+" ) AS "+tableName+" "+
                            "WHERE row >= @firstRow AND row <= @lastRow ORDER BY "+idColumn;
            DbParameter[] parameters = new DbParameter[] {
                new DbParameter("@FirstRow", SqlDbType.BigInt, firstRow),
                new DbParameter("@LastRow", SqlDbType.BigInt, lastRow)
            };
            //string statement = "select top "+pageSize+" * from "+tableName+" order by id asc";
            return dbTemplate.Select<StoredMailMessage>(stmt, ReadMail, parameters);
        }

        public StoredMailMessage GetNextMail()
        {
            return dbTemplate.SelectOne<StoredMailMessage>(getNextMail, ReadMail);
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

        public void MarkMailForSending(StoredMailMessage message)
        {
            long id = message.id;
            string MarkMail = "update "+tableName+" set "+sendingColumn+"=@Sending where "+idColumn+"=@Id and "+sendingColumn+" is null";
            DbParameter[] parameters = new DbParameter[] {
                new DbParameter("@Sending", SqlDbType.DateTime, DateTime.Now),
                new DbParameter("@Id", SqlDbType.BigInt, id)
            };
            int rowsUpdated = dbTemplate.Update(MarkMail, parameters);
            if (rowsUpdated != 1)
            {
                throw new Exception("Email " + id + " was already marked for sending. No retransmission.");
            }
        }

        public void MailSent(StoredMailMessage message)
        {
            long id = message.id;
            string MailSent = "update " + tableName + " set " + sentColumn + "=@Sent where " + idColumn + "=@Id";
            DbParameter[] parameters = new DbParameter[] {
                new DbParameter("@Sent", SqlDbType.DateTime, DateTime.Now),
                new DbParameter("@Id", SqlDbType.BigInt, id)
            };
            dbTemplate.Update(MailSent, parameters);
        }

        public void MailFailed(StoredMailMessage message)
        {
            long id = message.id;
            string MailSent = "update " + tableName + " set " + failedColumn + "=@Failed where " + idColumn + "=@Id";
            DbParameter[] parameters = new DbParameter[] {
                new DbParameter("@Failed", SqlDbType.DateTime, DateTime.Now),
                new DbParameter("@Id", SqlDbType.BigInt, id)
            };
            dbTemplate.Update(MailSent, parameters);
        }

        public void MailIgnored(StoredMailMessage message)
        {
            throw new NotImplementedException("MailDao.MailIgnored() not implemented.");
/*
            long id = message.id;
            string MailSent = "update " + tableName + " set " + failedColumn + "=@Failed where " + idColumn + "=@Id";
            DbParameter[] parameters = new DbParameter[] {
                new DbParameter("@Failed", SqlDbType.DateTime, DateTime.Now),
                new DbParameter("@Id", SqlDbType.BigInt, id)
            };
            dbTemplate.Update(MailSent, parameters);
 */ 
        }

        public void EnqueueMail(MailMessage message)
        {
            string to = message.To.ToString();
            string from = message.From.ToString();

            string EnqueueMail = "insert into " + tableName + " (" + toColumn + ", " + fromColumn + ", " + subjectColumn + ", " + bodyColumn + ", " + createdColumn + ") values (@To, @From, @Subject, @Body, @Created)";
            DbParameter[] parameters = new DbParameter[] {
                //new SqlParameter("@Id", SqlDbType.BigInt, id),
                new DbParameter("@To", SqlDbType.NVarChar, to),
                new DbParameter("@From", SqlDbType.NVarChar, from),
                new DbParameter("@Subject", SqlDbType.NVarChar, message.Subject),
                new DbParameter("@Body", SqlDbType.NVarChar, message.Body),
                new DbParameter("@Created", SqlDbType.DateTime, DateTime.Now)
            };
            dbTemplate.Update(EnqueueMail, parameters);
        }

        private int CountSince(DateTime sinceDate, string fromTableWhere)
        {
            string countStmt = "select count(*) from " + fromTableWhere; //tableName + " where " + createdColumn + " >= @sinceDate";
            DbParameter[] parameters = new DbParameter[] {
                new DbParameter("@sinceDate", SqlDbType.DateTime, sinceDate)
            };
            return (int)dbTemplate.Scalar(countStmt, parameters);
        }

        private int CountMailsCreatedSince(DateTime sinceDate)
        {
            return CountSince(sinceDate, tableName + " where " + createdColumn + " >= @sinceDate");
        }

        private int CountMailsFailedSince(DateTime sinceDate)
        {
            return CountSince(sinceDate, tableName + " where " + failedColumn + " >= @sinceDate");
        }

        public int CountMailsSentSince(DateTime sinceDate) {
            return CountSince(sinceDate, tableName+" where "+sentColumn+" >= @sinceDate");
        }

        private int CountMailsArchivedSince(DateTime sinceDate)
        {
            return CountSince(sinceDate, archiveTableName + " where " + sentColumn + " >= @sinceDate");
        }

        public MailsStat MailsSince(DateTime sinceDate)
        {
            int created = CountMailsCreatedSince(sinceDate);
            int sent = CountMailsSentSince(sinceDate);
            int failed = CountMailsFailedSince(sinceDate);
            int archived = CountMailsArchivedSince(sinceDate);

            return new MailsStat(sinceDate, created, sent, failed, archived);
        }

        public void ArchiveMailsOlderThan(DateTime archiveDate)
        {

            string insertIntoArchive = "insert into " + archiveTableName + " SELECT * from " + tableName + " where " + sentColumn + "<@archiveDate";
            DbParameter[] parameters = new DbParameter[] {
                new DbParameter("@archiveDate", SqlDbType.DateTime, archiveDate)
            };
            dbTemplate.Update(insertIntoArchive, parameters);

            string deleteMails = "delete from " + tableName + " where " + sentColumn + "<@archiveDate";
            parameters = new DbParameter[] {
                new DbParameter("@archiveDate", SqlDbType.DateTime, archiveDate)
            };
            dbTemplate.Update(deleteMails, parameters);
        }

        public void DeleteArchivedMailsOlderThan(DateTime deleteDate)
        {
            string deleteMails = "delete from " + archiveTableName + " where " + sentColumn + "<@deleteDate";
            DbParameter[] parameters = new DbParameter[] {
                new DbParameter("@deleteDate", SqlDbType.DateTime, deleteDate)
            };
            dbTemplate.Update(deleteMails, parameters);
        }
    
    }
}
