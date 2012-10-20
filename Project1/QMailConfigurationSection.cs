using System;
using System.Collections;
using System.Text;
using System.Configuration;
using System.Xml;

namespace qmail
{
    public class QMailConfigurationSection : ConfigurationSection
    {

/*
            maxMailsToSend = ConfigHelper.DefaultInt("qmail.max-mails-to-send", 0);
            log.Debug("qmail.max-mails-to-send=" + maxMailsToSend);

            maxMinutesToRun = ConfigHelper.DefaultInt("qmail.max-minutes-to-run", 0);
            log.Debug("qmail.max-minutes-to-run=" + maxMinutesToRun);
            
            maxMailsPerHour = ConfigHelper.DefaultInt("qmail.max-mails-per-hour", 0);
            log.Debug("qmail.max-mails-per-hour=" + maxMailsPerHour);

            maxMailsPerDay = ConfigHelper.DefaultInt("qmail.max-mails-per-day", 0);
            log.Debug("qmail.max-mails-per-day=" + maxMailsPerDay);
            
            isExitOnEmptyQueue = ConfigHelper.DefaultBool("qmail.exit-on-empty-queue", true);
            log.Debug("qmail.exit-on-empty-queue=" + isExitOnEmptyQueue);

            checkIntervalMinutes = ConfigHelper.DefaultInt("qmail.check-interval-minutes", 1);
            checkIntervalMinutes = Math.Max(1, checkIntervalMinutes);
            log.Debug("qmail.check-interval-minutes=" + checkIntervalMinutes);

            archiveSentMailsAfterHours = ConfigHelper.DefaultInt("qmail.archive-sent-mails-after-hours", 24);
            log.Debug("qmail.archive-sent-mails-after-hours=" + archiveSentMailsAfterHours);
*/

        [ConfigurationProperty("maxMailsPerHour", DefaultValue = "0", IsRequired = false)]
        [IntegerValidator(MinValue = 0)]
        public int MaxMailsPerHour
        {
            get { return (int)this["maxMailsPerHour"]; }
            set { this["maxMailsPerHour"] = value; }
        }

        [ConfigurationProperty("maxMailsPerDay", DefaultValue = "0", IsRequired = false)]
        [IntegerValidator(MinValue = 0)]
        public int MaxMailsPerDay
        {
            get { return (int)this["maxMailsPerDay"]; }
            set { this["maxMailsPerDay"] = value; }
        }

        [ConfigurationProperty("checkIntervalMinutes", DefaultValue = "1", IsRequired = false)]
        [IntegerValidator(MinValue = 1)]
        public int CheckIntervalMinutes
        {
            get { return (int)this["checkIntervalMinutes"]; }
            set { this["checkIntervalMinutes"] = value; }
        }

        [ConfigurationProperty("archiveSentMailsAfterHours", DefaultValue = "24", IsRequired = false)]
        [IntegerValidator(MinValue = 0)]
        public int ArchiveSentMailsAfterHours
        {
            get { return (int)this["archiveSentMailsAfterHours"]; }
            set { this["archiveSentMailsAfterHours"] = value; }
        }

        [ConfigurationProperty("deleteArchivedMailsAfterHours", DefaultValue = "48", IsRequired = false)]
        [IntegerValidator(MinValue = 0)]
        public int DeleteArchivesMailsAfterHours
        {
            get { return (int)this["deleteArchivedMailsAfterHours"]; }
            set { this["deleteArchivesMailsAfterHours"] = value; }
        }

        [ConfigurationProperty("mailTable")]
        public MailTableElement MailTable
        {
            get { return (MailTableElement)this["mailTable"]; }
            set { this["mailTable"] = value; }
        }

        [ConfigurationProperty("archiveMailTable")]
        public MailTableElement ArchiveMailTable
        {
            get { return (MailTableElement)this["archiveMailTable"]; }
            set { this["archiveMailTable"] = value; }
        }

        [ConfigurationProperty("smtp")]
        public SmtpElement Smtp
        {
            get { return (SmtpElement)this["smtp"]; }
            set { this["smtp"] = value; }
        }

        [ConfigurationProperty("folders")]
        public FoldersElement Folders
        {
            get { return (FoldersElement)this["folders"]; }
            set { this["folders"] = value; }
        }

        [ConfigurationProperty("filter")]
        public FilterElement Filter
        {
            get { return (FilterElement)this["filter"]; }
            set { this["filter"] = value; }
        }
    }

    public class FoldersElement : ConfigurationElement
    {
        [ConfigurationProperty("pickupFolder")]
        public string PickupFolder
        {
            get { return (string)this["pickupFolder"]; }
            set { this["pickupFolder"] = value; }
        }

        [ConfigurationProperty("sentFolder")]
        public string SentFolder
        {
            get { return (string)this["sentFolder"]; }
            set { this["sentFolder"] = value; }
        }

        [ConfigurationProperty("failedFolder")]
        public string FailedFolder
        {
            get { return (string)this["failedFolder"]; }
            set { this["failedFolder"] = value; }
        }

        [ConfigurationProperty("ignoredFolder")]
        public string IgnoredFolder
        {
            get { return (string)this["ignoredFolder"]; }
            set { this["ignoredFolder"] = value; }
        }

        [ConfigurationProperty("archiveFolder")]
        public string ArchiveFolder
        {
            get { return (string)this["archiveFolder"]; }
            set { this["archiveFolder"] = value; }
        }

        [ConfigurationProperty("createFolders", DefaultValue=true)]
        public bool CreateFolders
        {
            get { return (bool)this["createFolders"]; }
            set { this["createFolders"] = value; }
        }

        [ConfigurationProperty("processFilesByDate", DefaultValue = false)]
        public bool ProcessFilesByDate
        {
            get { return (bool)this["processFilesByDate"]; }
            set { this["processFilesByDate"] = value; }
        }
    }

    public class MailTableElement : ConfigurationElement
    {
        [ConfigurationProperty("tableName", DefaultValue="qmail", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String TableName
        {
            get { return (String)this["tableName"]; }
            set { this["tableName"] = value; }
        }

        [ConfigurationProperty("id", DefaultValue = "id", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*(){}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Id
        {
            get { return (String)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("to", DefaultValue = "to", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*(){}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String To
        {
            get { return (String)this["to"]; }
            set { this["to"] = value; }
        }

        [ConfigurationProperty("from", DefaultValue = "from", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*(){}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String From
        {
            get { return (String)this["from"]; }
            set { this["from"] = value; }
        }

        [ConfigurationProperty("subject", DefaultValue = "subject", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*(){}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Subject
        {
            get { return (String)this["subject"]; }
            set { this["subject"] = value; }
        }

        [ConfigurationProperty("body", DefaultValue = "body", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*(){}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Body
        {
            get { return (String)this["body"]; }
            set { this["body"] = value; }
        }

        [ConfigurationProperty("created", DefaultValue = "created", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*(){}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Created
        {
            get { return (String)this["created"]; }
            set { this["created"] = value; }
        }

        [ConfigurationProperty("sending", DefaultValue = "sending", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*(){}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Sending
        {
            get { return (String)this["sending"]; }
            set { this["sending"] = value; }
        }

        [ConfigurationProperty("sent", DefaultValue = "sent", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*(){}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Sent
        {
            get { return (String)this["sent"]; }
            set { this["sent"] = value; }
        }

        [ConfigurationProperty("failed", DefaultValue = "failed", IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*(){}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Failed
        {
            get { return (String)this["failed"]; }
            set { this["failed"] = value; }
        }
    }

    public class SmtpElement : ConfigurationElement
    {

        [ConfigurationProperty("host", IsRequired = true)]
//        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Host
        {
            get { return (String)this["host"]; }
            set { this["host"] = value; }
        }

        [ConfigurationProperty("port", DefaultValue="22", IsRequired = false)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("ssl", DefaultValue="false", IsRequired = false)]
        public Boolean Ssl
        {
            get { return (bool)this["ssl"]; }
            set { this["ssl"] = value; }
        }

        [ConfigurationProperty("username", IsRequired = false)]
        public String Username
        {
            get { return (String)this["username"]; }
            set { this["username"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public String Password
        {
            get { return (String)this["password"]; }
            set { this["password"] = value; }
        }
    }

    public class FilterElement : ConfigurationElement
    {

        [ConfigurationProperty("overrideTo", IsRequired = false)]
        //        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String OverrideTo
        {
            get { return (String)this["overrideTo"]; }
            set { this["overrideTo"] = value; }
        }
        
        [ConfigurationProperty("whitelist")]
        public ToMailAddressElementCollection Whitelist
        {
            get { return (ToMailAddressElementCollection)this["whitelist"]; }
        }
    }

    public class ToMailAddressElement : ConfigurationElement
    {
       [ConfigurationProperty("mailAddress", IsKey=true, IsRequired=true)]
       public string MailAddress
       {
           get { return (string)this["mailAddress"]; }
       }
    }

    public class ToMailAddressElementCollection : ConfigurationElementCollection
    {
       protected override ConfigurationElement CreateNewElement()
       {
           return new ToMailAddressElement();
       }

       protected override object GetElementKey(ConfigurationElement element)
       {
           return ((ToMailAddressElement)element).MailAddress;
       }

       public ToMailAddressElement this[int index]
       {
           get
           {
               return (ToMailAddressElement)BaseGet(index);
           }
           set
           {
               if (BaseGet(index) != null)
               {
                   BaseRemoveAt(index);
               }
               BaseAdd(index, value);
           }
       }
    }
}
