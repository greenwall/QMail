using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using qmail;
using QMailWeb.Models;

namespace QMailWeb.Controllers
{
    public class HomeController : Controller
    {
        // The Mails class is replacement for a real data access strategy.
        private static Mails mails = new Mails();
        private int pageSize = 10;

        [HttpGet]
        public ViewResult Index()
        {
            return Statistics();
        }

        //[HttpPost]
        //[HttpGet]
        public ViewResult Mails(string searchString, int page = 1)
        {

            QMail qmail = new QMail();


            IList<StoredMailMessage> mails = qmail.SearchMails(page, pageSize, searchString).ToList<StoredMailMessage>();
            ViewBag.TotalPages = (int)Math.Ceiling((double)qmail.CountMails() / pageSize);

/*
            products = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
*/
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(mails);
        }

        [HttpGet]
        public ViewResult Statistics()
        {
            QMail qmail = new QMail();

            DateTime[] dateTimes = 
            {
                DateTime.Now.Subtract(new TimeSpan(1, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(4, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(12, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(24, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(2*24, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(7*24, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(14*24, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(30*24, 0, 0)),
                DateTime.Now.Subtract(new TimeSpan(60*24, 0, 0)) 
            };
            return View(qmail.MailsSince(dateTimes));
/*
            List<MailStatistics> statistics = new List<MailStatistics>();

            foreach (MailsStat stat in stats)
            {
                MailStatistics s = new MailStatistics();
                s.since = stat.SinceDate;
                s.stats["Created"] = stat.Created;
                s.stats["Sent"] = stat.Sent;
                s.stats["Failed"] = stat.Failed;
                s.stats["Archived"] = stat.Archived;

                statistics.Add(s);
            }

            return View(statistics.ToList());

 */
        }
/*
        public ViewResult Index3(string sortOrder, string searchString, string hours)
        {

            QMail qmail = new QMail();

            int _hours = 1;
            try
            {
                _hours = int.Parse(hours);
            }
            catch { };

            DateTime[] dateTimes = 
            {
                DateTime.Now.Subtract(new TimeSpan(1, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(4, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(12, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(24, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(2*24, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(7*24, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(14*24, 0, 0)), 
                DateTime.Now.Subtract(new TimeSpan(30*24, 0, 0)),
                DateTime.Now.Subtract(new TimeSpan(60*24, 0, 0)) 
            };
            IEnumerable<MailsStat> stats = qmail.MailsSince(dateTimes);
            return View(stats);
        }
*/
        public ViewResult _Index(string sortOrder, string searchString, string hours)
        {

            QMail qmail = new QMail();

            int _hours = 1;
            try
            {
                _hours = int.Parse(hours);
            }
            catch { };
            //ViewBag.mailsStat = qmail.MailsSince(DateTime.Now.Subtract(new TimeSpan(_hours, 0, 0)));

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date desc" : "Date";
            var _mails = from s in mails.mailList
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                _mails = _mails.Where(s => s.Subject.ToUpper().Contains(searchString.ToUpper())
                                       || s.To.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "Name desc":
                    _mails = _mails.OrderByDescending(s => s.To);
                    break;
                case "Date":
                    _mails = _mails.OrderBy(s => s.Id);
                    break;
                case "Date desc":
                    _mails = _mails.OrderByDescending(s => s.Id);
                    break;
                default:
                    _mails = _mails.OrderBy(s => s.Subject);
                    break;
            }
            return View(_mails.ToList());
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
