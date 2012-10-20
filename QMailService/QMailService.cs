using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

using qmail;

namespace QMailService
{
    /// <summary>
    /// Wraps QMail into a Windows Service.
    /// </summary>
    public partial class QMailService : ServiceBase
    {
        private QMail qmail;
        //private Timer serviceTimer;

        public QMailService()
        {
            InitializeComponent();
/*
            if (!System.Diagnostics.EventLog.SourceExists("QMailLogSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource("QMailLogSource", "QMailLog");
            }
            eventLog = new EventLog();
            eventLog.Source = "QMailLogSource";
            eventLog.Log = "QMailLog";
 
 */
        }

        protected override void OnStart(string[] args)
        {
            qmail = new QMail();
            qmail.Start();
        }

        protected override void OnStop()
        {
//            serviceTimer.Enabled = false;
            qmail.Stop();
        }
/*
        private void ProcessMailsFromQueue(object sender, EventArgs args)
        {
            bool more = true;
            while (more)
            {
                more = qmail.ReadMarkAndSend();
            }
        }
 */
    }
}
