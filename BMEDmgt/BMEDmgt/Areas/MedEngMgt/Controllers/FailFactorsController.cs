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
    public class FailFactorsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/FailFactors
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") == true)
            {
                // Save log. 
                string logClass = "系統管理者紀錄";
                string logAction = "故障原因選單";
                var result = new SystemLogsController().SaveLog(logClass, logAction);
            }
            return View(db.FailFactors.ToList());
        }

        // GET: MedEngMgt/FailFactors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FailFactor failFactor = db.FailFactors.Find(id);
            if (failFactor == null)
            {
                return HttpNotFound();
            }
            return View(failFactor);
        }

        // GET: MedEngMgt/FailFactors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/FailFactors/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Flg")] FailFactor failFactor)
        {
            if (ModelState.IsValid)
            {
                db.FailFactors.Add(failFactor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(failFactor);
        }

        // GET: MedEngMgt/FailFactors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FailFactor failFactor = db.FailFactors.Find(id);
            if (failFactor == null)
            {
                return HttpNotFound();
            }
            return View(failFactor);
        }

        // POST: MedEngMgt/FailFactors/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Flg")] FailFactor failFactor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(failFactor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(failFactor);
        }

        // GET: MedEngMgt/FailFactors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FailFactor failFactor = db.FailFactors.Find(id);
            if (failFactor == null)
            {
                return HttpNotFound();
            }
            return View(failFactor);
        }

        // POST: MedEngMgt/FailFactors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FailFactor failFactor = db.FailFactors.Find(id);
            db.FailFactors.Remove(failFactor);
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
