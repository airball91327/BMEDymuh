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
    public class KeepResultsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/KeepResults
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") == true)
            {
                // Save log. 
                SystemLog log = new SystemLog();
                log.LogClass = "系統管理者紀錄";
                log.LogTime = DateTime.UtcNow.AddHours(8);
                log.UserId = WebSecurity.CurrentUserId;
                log.Action = "保養結果選單";
                db.SystemLogs.Add(log);
                if (TryUpdateModel(log))
                {
                    db.SaveChanges();
                }
            }
            return View(db.KeepResults.ToList());
        }

        // GET: MedEngMgt/KeepResults/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeepResult keepResult = db.KeepResults.Find(id);
            if (keepResult == null)
            {
                return HttpNotFound();
            }
            return View(keepResult);
        }

        // GET: MedEngMgt/KeepResults/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/KeepResults/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Flg")] KeepResult keepResult)
        {
            if (ModelState.IsValid)
            {
                db.KeepResults.Add(keepResult);
                if (TryUpdateModel(keepResult))
                {
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(keepResult);
        }

        // GET: MedEngMgt/KeepResults/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeepResult keepResult = db.KeepResults.Find(id);
            if (keepResult == null)
            {
                return HttpNotFound();
            }
            return View(keepResult);
        }

        // POST: MedEngMgt/KeepResults/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Flg")] KeepResult keepResult)
        {
            if (ModelState.IsValid)
            {
                db.Entry(keepResult).State = EntityState.Modified;
                if (TryUpdateModel(keepResult))
                {
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(keepResult);
        }

        // GET: MedEngMgt/KeepResults/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeepResult keepResult = db.KeepResults.Find(id);
            if (keepResult == null)
            {
                return HttpNotFound();
            }
            return View(keepResult);
        }

        // POST: MedEngMgt/KeepResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KeepResult keepResult = db.KeepResults.Find(id);
            db.KeepResults.Remove(keepResult);
            if (TryUpdateModel(keepResult))
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
