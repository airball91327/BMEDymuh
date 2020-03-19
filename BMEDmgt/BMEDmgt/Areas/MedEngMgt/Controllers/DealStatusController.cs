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
    [Authorize]
    public class DealStatusController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/DealStatus
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") == true)
            {
                // Save log. 
                SystemLog log = new SystemLog();
                log.LogClass = "系統管理者紀錄";
                log.LogTime = DateTime.UtcNow.AddHours(8);
                log.UserId = WebSecurity.CurrentUserId;
                log.Action = "處理狀況選單";
                db.SystemLogs.Add(log);
                if (TryUpdateModel(log))
                {
                    db.SaveChanges();
                }
            }
            return View(db.DealStatus.ToList());
        }

        // GET: MedEngMgt/DealStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DealStatus dealStatus = db.DealStatus.Find(id);
            if (dealStatus == null)
            {
                return HttpNotFound();
            }
            return View(dealStatus);
        }

        // GET: MedEngMgt/DealStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/DealStatus/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Flg")] DealStatus dealStatus)
        {
            if (ModelState.IsValid)
            {
                db.DealStatus.Add(dealStatus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dealStatus);
        }

        // GET: MedEngMgt/DealStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DealStatus dealStatus = db.DealStatus.Find(id);
            if (dealStatus == null)
            {
                return HttpNotFound();
            }
            return View(dealStatus);
        }

        // POST: MedEngMgt/DealStatus/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Flg")] DealStatus dealStatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dealStatus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dealStatus);
        }

        // GET: MedEngMgt/DealStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DealStatus dealStatus = db.DealStatus.Find(id);
            if (dealStatus == null)
            {
                return HttpNotFound();
            }
            return View(dealStatus);
        }

        // POST: MedEngMgt/DealStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DealStatus dealStatus = db.DealStatus.Find(id);
            db.DealStatus.Remove(dealStatus);
            if (TryUpdateModel(dealStatus))
            {
                db.SaveChanges();
            }
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
