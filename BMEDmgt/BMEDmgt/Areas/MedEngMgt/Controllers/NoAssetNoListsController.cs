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
    public class NoAssetNoListsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/NoAssetNoLists
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") == true)
            {
                // Save log. 
                SystemLog log = new SystemLog();
                log.LogClass = "系統管理者紀錄";
                log.LogTime = DateTime.UtcNow.AddHours(8);
                log.UserId = WebSecurity.CurrentUserId;
                log.Action = "無財產設備名稱選單";
                db.SystemLogs.Add(log);
                if (TryUpdateModel(log))
                {
                    db.SaveChanges();
                }
            }
            return View(db.NoAssetNoLists.ToList());
        }

        // GET: MedEngMgt/NoAssetNoLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoAssetNoList noAssetNoList = db.NoAssetNoLists.Find(id);
            if (noAssetNoList == null)
            {
                return HttpNotFound();
            }
            return View(noAssetNoList);
        }

        // GET: MedEngMgt/NoAssetNoLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/NoAssetNoLists/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Flg")] NoAssetNoList noAssetNoList)
        {
            if (ModelState.IsValid)
            {
                db.NoAssetNoLists.Add(noAssetNoList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(noAssetNoList);
        }

        // GET: MedEngMgt/NoAssetNoLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoAssetNoList noAssetNoList = db.NoAssetNoLists.Find(id);
            if (noAssetNoList == null)
            {
                return HttpNotFound();
            }
            return View(noAssetNoList);
        }

        // POST: MedEngMgt/NoAssetNoLists/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Flg")] NoAssetNoList noAssetNoList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(noAssetNoList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(noAssetNoList);
        }

        // GET: MedEngMgt/NoAssetNoLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NoAssetNoList noAssetNoList = db.NoAssetNoLists.Find(id);
            if (noAssetNoList == null)
            {
                return HttpNotFound();
            }
            return View(noAssetNoList);
        }

        // POST: MedEngMgt/NoAssetNoLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NoAssetNoList noAssetNoList = db.NoAssetNoLists.Find(id);
            db.NoAssetNoLists.Remove(noAssetNoList);
            if (TryUpdateModel(noAssetNoList))
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
