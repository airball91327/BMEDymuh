using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Extensions;
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
                string logClass = "系統管理者紀錄";
                string logAction = "無財產設備名稱選單";
                var result = new SystemLogsController().SaveLog(logClass, logAction);
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
                // Save log. 
                string logClass = "管理紀錄";
                string logAction = "無財編設備名稱選單 > 新增 > " + noAssetNoList.Title;
                var result = new SystemLogsController().SaveLog(logClass, logAction);

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
                var oriObj = db.NoAssetNoLists.Find(noAssetNoList.Id);
                db.Entry(oriObj).State = EntityState.Detached;
                //
                db.Entry(noAssetNoList).State = EntityState.Modified;
                db.SaveChanges();
                // Save log. 
                var currentObj = db.NoAssetNoLists.Find(noAssetNoList.Id);
                var logAction2 = oriObj.EnumeratePropertyDifferences<NoAssetNoList>(currentObj);
                string logClass = "管理紀錄";
                string logAction = "無財編設備名稱選單 > 編輯 > " + noAssetNoList.Title;
                var result = new SystemLogsController().SaveLog(logClass, logAction, logAction2);

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
