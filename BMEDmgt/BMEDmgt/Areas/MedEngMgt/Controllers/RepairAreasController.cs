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

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class RepairAreasController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/RepairAreas
        public ActionResult Index()
        {
            return View(db.RepairAreas.ToList());
        }

        // GET: MedEngMgt/RepairAreas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairArea repairArea = db.RepairAreas.Find(id);
            if (repairArea == null)
            {
                return HttpNotFound();
            }
            return View(repairArea);
        }

        // GET: MedEngMgt/RepairAreas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/RepairAreas/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AreaName")] RepairArea repairArea)
        {
            if (ModelState.IsValid)
            {
                db.RepairAreas.Add(repairArea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(repairArea);
        }

        // GET: MedEngMgt/RepairAreas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairArea repairArea = db.RepairAreas.Find(id);
            if (repairArea == null)
            {
                return HttpNotFound();
            }
            return View(repairArea);
        }

        // POST: MedEngMgt/RepairAreas/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AreaName")] RepairArea repairArea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(repairArea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(repairArea);
        }

        // GET: MedEngMgt/RepairAreas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairArea repairArea = db.RepairAreas.Find(id);
            if (repairArea == null)
            {
                return HttpNotFound();
            }
            return View(repairArea);
        }

        // POST: MedEngMgt/RepairAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RepairArea repairArea = db.RepairAreas.Find(id);
            db.RepairAreas.Remove(repairArea);
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
