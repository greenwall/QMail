using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.IO;
using System.Configuration;

using log4net;

namespace qmail
{
    public class FileMailDao : IMailDao
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DirectoryInfo mailFolder;
        private readonly DirectoryInfo sentMailFolder;
        private readonly DirectoryInfo failedMailFolder;
        private readonly DirectoryInfo ignoredMailFolder;
        private readonly DirectoryInfo archiveMailFolder;
        private readonly bool createFolders;
        private readonly bool processFilesByDate;

        public FileMailDao()
        {
            QMailConfigurationSection qmailConfig = (QMailConfigurationSection)ConfigurationManager.GetSection("qmailConfiguration");
            FoldersElement config = qmailConfig.Folders;

            mailFolder = new DirectoryInfo(config.PickupFolder);
            sentMailFolder = FolderOrNull(config.SentFolder);
            failedMailFolder = FolderOrNull(config.FailedFolder);
            ignoredMailFolder = FolderOrNull(config.IgnoredFolder);
            archiveMailFolder = FolderOrNull(config.ArchiveFolder);

            createFolders = config.CreateFolders;
            processFilesByDate = config.ProcessFilesByDate;

            VerifyFolders();
        }

        private DirectoryInfo FolderOrNull(string folder)
        {
            if (!String.IsNullOrEmpty(folder))
            {
                return new DirectoryInfo(folder);
            }
            return null;
        }

        private void VerifyFolders()
        {
            checkIfFolderExists(mailFolder, createFolders);
            checkIfFolderExists(sentMailFolder, createFolders);
            checkIfFolderExists(failedMailFolder, createFolders);
            checkIfFolderExists(ignoredMailFolder, createFolders);
            checkIfFolderExists(archiveMailFolder, createFolders);

        }

        private void checkIfFolderExists(DirectoryInfo folder, bool createIfNotExists)
        {
            if (folder!=null && !folder.Exists)
            {
                if (createIfNotExists)
                {
                    folder.Create();
                }
                else
                {
                    throw new Exception("QMail detected non-existing folder: " + folder.FullName + ". Setting attribute 'createFolder=\"true\" allows QMail to create non-existing folders.");
                }
            }
        }

        public IEnumerable<StoredMailMessage> SearchMails(int currentPage, int pageSize, string toFilter, string subjectFilter)
        {
            throw new NotImplementedException();
        }

        public StoredMailMessage GetNextMail()
        {
            if (processFilesByDate)
            {
                // Ensure strict ordering by date. May cause problems if we have huge amounts of files in dropfolder!
                FileSystemInfo[] files = mailFolder.GetFileSystemInfos("*.eml");
                var orderedFiles = files.OrderBy(f => f.CreationTime);
                if (orderedFiles.Any())
                {
                    FileSystemInfo first = orderedFiles.First();
                    return LoadMailFromFile(first);
                }
                return null;
            }
            else
            {
                IEnumerable<FileSystemInfo> files = mailFolder.EnumerateFileSystemInfos("*.eml");
                try
                {
                    var first = files.First();
                    return LoadMailFromFile(first);
                }
                catch
                {
                    log.Debug("No mails to send");
                }
                return null;
            }
        }

        private StoredMailMessage LoadMailFromFile(FileSystemInfo first)
        {
            // load mail
            FileInfo file = new FileInfo(first.FullName);
            string fileNameLoading = first.FullName + ".loading";
            file.MoveTo(fileNameLoading);

            var rmessage = new Rebex.Mail.MailMessage();
            rmessage.Load(fileNameLoading); 

            file.MoveTo(first.FullName + ".loaded");

            StoredMailMessage mm = new StoredMailMessage(first.Name);
            MailMessageConverter.ConvertTo(rmessage, mm);

            file.MoveTo(first.FullName + ".ready");
            return mm;
        }
             

/*        
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
*/
        public int Count()
        {
            FileSystemInfo[] files = mailFolder.GetFileSystemInfos("*.eml");
            return files.Count();
            //var orderedFiles = files.OrderBy(f => f.CreationTime);
        }

        public void MarkMailForSending(StoredMailMessage message)
        {
            FileInfo file = new FileInfo(mailFolder.FullName+"\\"+message.filename+".ready");
            file.MoveTo(mailFolder.FullName + "\\" + message.filename + ".sending");
        }

        public void MailSent(StoredMailMessage message)
        {
            log.Debug("Sent mail " + message.filename + message.ReceiversToString());

            FileInfo file = new FileInfo(mailFolder.FullName + "\\" + message.filename + ".sending");
            if (sentMailFolder != null)
            {
                file.MoveTo(sentMailFolder.FullName + "\\" + message.filename);
            }
            else
            {
                file.MoveTo(mailFolder.FullName + "\\" + message.filename + ".sent");
            }
        }

        public void MailFailed(StoredMailMessage message)
        {
            log.Error("Failed sending mail " + message.filename + message.ReceiversToString());

            FileInfo file = new FileInfo(mailFolder.FullName + "\\" + message.filename + ".sending");
            if (failedMailFolder != null)
            {
                file.MoveTo(failedMailFolder.FullName + "\\" + message.filename);
            }
            else
            {
                file.MoveTo(mailFolder.FullName + "\\" + message.filename + ".failed");
            }
        }

        public void MailIgnored(StoredMailMessage message)
        {
            log.Info("Ignored mail " + message.filename + message.ReceiversToString());

            FileInfo file = new FileInfo(mailFolder.FullName + "\\" + message.filename + ".sending");
            if (ignoredMailFolder != null)
            {
                file.MoveTo(ignoredMailFolder.FullName + "\\" + message.filename);
            }
            else
            {
                file.MoveTo(mailFolder.FullName + "\\" + message.filename + ".ignored");
            }
        }
        
        public void EnqueueMail(MailMessage message)
        {
        }

        public int CountMailsSentSince(DateTime sinceDate)
        {
            int count = 0;
//            FileSystemInfo[] files = sentMailFolder.GetFileSystemInfos("*.eml");
            IEnumerable<FileSystemInfo> sentFiles;
            if (sentMailFolder != null)
            {
                sentFiles = sentMailFolder.EnumerateFileSystemInfos("*.eml");
            }
            else
            {
                sentFiles = mailFolder.EnumerateFileSystemInfos("*.sent");
            }

            foreach (FileSystemInfo file in sentFiles)
            {
                if (file.CreationTime >= sinceDate)
                {
                    count++;
                }
            }
            return count;
            //var orderedFiles = files.OrderBy(f => f.CreationTime);
        }

        public MailsStat MailsSince(DateTime sinceDate) {
            throw new NotImplementedException();
        }

        public void ArchiveMailsOlderThan(DateTime archiveDate)
        {
            IEnumerable<FileSystemInfo> sentFiles;

            if (sentMailFolder != null)
            {
                sentFiles = sentMailFolder.EnumerateFileSystemInfos("*.eml");
            }
            else
            {
                sentFiles = mailFolder.EnumerateFileSystemInfos("*.sent");
            }

            foreach (FileSystemInfo file in sentFiles)
            {
                ArchiveFileOlderThan(file, archiveDate);
            }

            IEnumerable<FileSystemInfo> ignoredFiles;
            if (ignoredMailFolder != null)
            {
                ignoredFiles = ignoredMailFolder.EnumerateFileSystemInfos("*.eml");
            }
            else
            {
                ignoredFiles = mailFolder.EnumerateFileSystemInfos("*.ignored");
            }

            foreach (FileSystemInfo file in ignoredFiles)
            {
                ArchiveFileOlderThan(file, archiveDate);
            }        
        }

        private void ArchiveFileOlderThan(FileSystemInfo file, DateTime archiveDate)
        {
            if (file.CreationTime < archiveDate)
            {
                log.Info("Archiving mail " + file.Name);

                //FileInfo file = new FileInfo(mailFolder.FullName + "\\" + message.filename + ".sending");
                FileInfo f = new FileInfo(file.FullName);
                if (archiveMailFolder != null)
                {
                    f.MoveTo(archiveMailFolder.FullName + "\\" + file.Name);
                }
                else
                {
                    f.MoveTo(mailFolder.FullName + "\\" + file.Name + ".archived");
                }
            }            
        }

        public void DeleteArchivedMailsOlderThan(DateTime archiveDate)
        {
            IEnumerable<FileSystemInfo> archivedFiles;

            if (archiveMailFolder != null)
            {
                archivedFiles = archiveMailFolder.EnumerateFileSystemInfos("*.eml");
            }
            else
            {
                archivedFiles = mailFolder.EnumerateFileSystemInfos("*.archived");
            }

            foreach (FileSystemInfo file in archivedFiles)
            {
                if (file.CreationTime < archiveDate)
                {
                    log.Info("Deleting archived mail " + file.Name);

                    file.Delete();
                }
            }
        }
    
    }
}
