using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class QuestionnaireMsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/QuestionnaireMs
        public ActionResult Index()
        {
            var lst = db.QuestionnaireMs.ToList();
            lst.ForEach(q =>
            {
                q.RtpName = db.AppUsers.Find(q.Rtp).FullName;
            });
               
            return View(lst);
        }

        // GET: MedEngMgt/QuestionnaireMs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionnaireM questionnaireM = db.QuestionnaireMs.Find(id);
            if (questionnaireM == null)
            {
                return HttpNotFound();
            }
            return PartialView(questionnaireM);
        }

        // GET: MedEngMgt/QuestionnaireMs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/QuestionnaireMs/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VerId,Qname,Memo,Flg,Rtp,Rtt")] QuestionnaireM questionnaireM)
        {
            if (ModelState.IsValid)
            {
                questionnaireM.Rtp = WebSecurity.CurrentUserId;
                questionnaireM.Rtt = DateTime.Now;
                db.QuestionnaireMs.Add(questionnaireM);
                db.SaveChanges();

                return RedirectToAction("List", new { id = questionnaireM.VerId });
            }

            return View(questionnaireM);
        }

        // GET: MedEngMgt/QuestionnaireMs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionnaireM questionnaireM = db.QuestionnaireMs.Find(id);
            if (questionnaireM == null)
            {
                return HttpNotFound();
            }
            return View(questionnaireM);
        }

        // POST: MedEngMgt/QuestionnaireMs/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VerId,Qname,Memo,Flg,Rtp,Rtt")] QuestionnaireM questionnaireM)
        {
            if (ModelState.IsValid)
            {
                questionnaireM.Rtp = WebSecurity.CurrentUserId;
                questionnaireM.Rtt = DateTime.Now;
                db.Entry(questionnaireM).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(questionnaireM);
        }

        // GET: MedEngMgt/QuestionnaireMs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionnaireM questionnaireM = db.QuestionnaireMs.Find(id);
            if (questionnaireM == null)
            {
                return HttpNotFound();
            }
            return View(questionnaireM);
        }

        // POST: MedEngMgt/QuestionnaireMs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuestionnaireM questionnaireM = db.QuestionnaireMs.Find(id);
            db.QuestionnaireMs.Remove(questionnaireM);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
