using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using WebMatrix.WebData;
using BMEDmgt.Models;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class QuestSendController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        //
        // GET: /QuestSend/

        public ActionResult Index()
        {
            QuestSend sd = new QuestSend();
            sd.YYYYmm = (DateTime.Now.Year * 100 + DateTime.Now.Month).ToString();
            //List<SelectListItem> listItem = new List<SelectListItem>();
            //List<Group> gv = db.Groups.ToList();
            //SelectListItem it;
            //foreach (Group g in gv)
            //{
            //    it = new SelectListItem();
            //    it.Text = g.GrpName;
            //    it.Value = g.GroupId;
            //    listItem.Add(it);
            //}
            //ViewData["GROUP"] = new SelectList(listItem, "Value", "Text", "");
            return View(sd);
        }

        [HttpPost]
        public ActionResult Index(QuestSend questsend)
        {
            if (ModelState.IsValid)
            {
                questsend.Rtp = WebSecurity.CurrentUserId;
                questsend.Rtt = DateTime.Now;
                try
                {
                    //
                    if (questsend.GroupId != null)
                    {
                        Tmail mail = new Tmail();
                        string body = "";
                        AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                        mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                        mail.message.Subject = "醫療儀器管理資訊系統[服務滿意度調查]";
                        body += "<p>您對醫工人員醫療儀器服務的表現評價如何?</p>";
                        body += "<p>邀請您填寫「醫療儀器服務滿意度調查表」，</p>";
                        body += "<p>將您給予的分數在選單內點選，10分為很滿意，1分為不滿意</p>";
                        body += "<p><a href='http://203.75.23.13/MvcMedEngMgr/Questionnaire/New?YYYYmm=" + questsend.YYYYmm;
                        body += "'>前往填寫問卷</a></p>";
                        body += "<p>敬請各單位給予您最寶貴的意見，謝謝!!</p>";
                        mail.message.Body = body;
                        mail.message.IsBodyHtml = true;
                        var ulist = db.Departments
                        .Join(db.AppUsers, p => p.DptId, c => c.DptId,
                        (p, c) => new { 
                        c.Email
                        }).ToList();
                        foreach (var f in ulist)
                        {
                            if (f.Email != null)
                            {
                                mail.message.Bcc.Add(new System.Net.Mail.MailAddress(f.Email));
                            }                           
                        }
                        //mail.SendMail();
                        //
                        questsend.GrpName = "";//db.Groups.Find(questsend.GroupId).GrpName;
                        questsend.SendAmt = ulist.Count();
                        db.QuestSends.Add(questsend);
                        db.SaveChanges();
                    }
                    else
                    {
                        return Content("請選擇群組!!");
                    }
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }               
            }
            return PartialView("List", db.QuestSends.ToList());
        }

        public ActionResult List()
        {
            List<QuestSend> qlist = db.QuestSends.ToList();
            foreach (QuestSend q in qlist)
            {
                q.UserName = db.AppUsers.Find(q.Rtp).FullName;
            }
            return PartialView(db.QuestSends.ToList());
        }

        //
        // GET: /QuestSend/Details/5

        public ActionResult Details(string id = null)
        {
            QuestSend questsend = db.QuestSends.Find(id);
            if (questsend == null)
            {
                return HttpNotFound();
            }
            return View(questsend);
        }

        //
        // GET: /QuestSend/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /QuestSend/Create

        [HttpPost]
        public ActionResult Create(QuestSend questsend)
        {
            if (ModelState.IsValid)
            {
                db.QuestSends.Add(questsend);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(questsend);
        }

        //
        // GET: /QuestSend/Edit/5

        public ActionResult Edit(string id = null)
        {
            QuestSend questsend = db.QuestSends.Find(id);
            if (questsend == null)
            {
                return HttpNotFound();
            }
            return View(questsend);
        }

        //
        // POST: /QuestSend/Edit/5

        [HttpPost]
        public ActionResult Edit(QuestSend questsend)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questsend).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(questsend);
        }

        //
        // GET: /QuestSend/Delete/5

        public ActionResult Delete(string id = null)
        {
            QuestSend questsend = db.QuestSends.Find(id);
            if (questsend == null)
            {
                return HttpNotFound();
            }
            return View(questsend);
        }

        //
        // POST: /QuestSend/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            QuestSend questsend = db.QuestSends.Find(id);
            db.QuestSends.Remove(questsend);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}