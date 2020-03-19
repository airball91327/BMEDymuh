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
    public class LabelsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/Labels
        public ActionResult Index()
        {
            return View(db.Labels.ToList());
        }

        // GET: MedEngMgt/Labels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Label label = db.Labels.Find(id);
            if (label == null)
            {
                return HttpNotFound();
            }
            label.RtpName = db.AppUsers.Find(label.Rtp) == null ? "" : db.AppUsers.Find(label.Rtp).FullName;
            return View(label);
        }

        // GET: MedEngMgt/Labels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/Labels/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LabelId,LabelName,Year,Status,Rtp,Rtt")] Label label)
        {
            if (ModelState.IsValid)
            {
                label.Rtp = WebSecurity.CurrentUserId;
                label.Rtt = DateTime.Now;
                db.Labels.Add(label);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(label);
        }

        // GET: MedEngMgt/Labels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Label label = db.Labels.Find(id);
            if (label == null)
            {
                return HttpNotFound();
            }
            return View(label);
        }

        // POST: MedEngMgt/Labels/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LabelId,LabelName,Year,Status,Rtp,Rtt")] Label label)
        {
            if (ModelState.IsValid)
            {
                label.Rtp = WebSecurity.CurrentUserId;
                label.Rtt = DateTime.Now;
                db.Entry(label).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(label);
        }

        // GET: MedEngMgt/Labels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Label label = db.Labels.Find(id);
            if (label == null)
            {
                return HttpNotFound();
            }
            label.RtpName = db.AppUsers.Find(label.Rtp) == null ? "" : db.AppUsers.Find(label.Rtp).FullName;
            return View(label);
        }

        // POST: MedEngMgt/Labels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Label label = db.Labels.Find(id);
            db.Labels.Remove(label);
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
