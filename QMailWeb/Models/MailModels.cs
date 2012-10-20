using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QMailWeb.Models
{
    public class MailModel
    {
        [Required]
        [Display(Name = "Id")]
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "To")]
        public string To { get; set; }
    }

    public class Mails
    {

        public Mails()
        {
            mailList.Add(new MailModel
            {
                Id = "1",
                Subject = "Subject1",
                To = "To1"
            });

            mailList.Add(new MailModel
            {
                Id = "2",
                Subject = "Subject2",
                To = "To2"
            });
        }

        public List<MailModel> mailList = new List<MailModel>();

        public void Update(MailModel toUpdate)
        {

            foreach (MailModel model in mailList)
            {
                if (model.Id == toUpdate.Id)
                {
                    mailList.Remove(model);
                    mailList.Add(toUpdate);
                    break;
                }
            }
        }

        public void Create(MailModel toUpdate)
        {
            foreach (MailModel model in mailList)
            {
                if (model.Id == toUpdate.Id)
                {
                    throw new System.InvalidOperationException("Duplicate id: " + model.Id);
                }
            }
            mailList.Add(toUpdate);
        }
/*
        public void Remove(string usrName)
        {

            foreach (UserModel um in _usrList)
            {
                if (um.UserName == usrName)
                {
                    _usrList.Remove(um);
                    break;
                }
            }
        }
*/
        public MailModel GetMail(string id)
        {
            MailModel model = null;

            foreach (MailModel m in mailList)
            {
                if (m.Id == id)
                {
                    model = m;
                }
            }

            return model;
        }

    }
}