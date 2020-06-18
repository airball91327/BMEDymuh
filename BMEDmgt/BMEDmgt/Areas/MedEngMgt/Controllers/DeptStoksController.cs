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
using PagedList;
using WebMatrix.WebData;
using BMEDmgt.Filters;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class DeptStoksController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 100;
        // GET: MedEngMgt/DeptStoks
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection fm, int page = 1)
        {
            string stokno = fm["qtySTOKNO"];
            string dname = fm["qtyDEPTNAME"];
            string brand = fm["qtyBRAND"];
            List<DeptStok> dv = db.DeptStoks.ToList();
            if (!string.IsNullOrEmpty(stokno))
            {
                dv = dv.Where(d => d.StokNo.Contains(stokno)).ToList();
            }
            if (!string.IsNullOrEmpty(dname))
            {
                dv = dv.Where(d => d.StokNam.Contains(dname)).ToList();
            }
            if (!string.IsNullOrEmpty(brand))
            {
                dv = dv.Where(d => d.Brand == brand.ToUpper()).ToList();
            }
            if (dv.ToPagedList(page, pageSize).Count <= 0)
                return PartialView("List", dv.ToPagedList(1, pageSize));

            return PartialView("List", dv.ToPagedList(page, pageSize));
        }

        // GET: MedEngMgt/DeptStoks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeptStok deptStok = db.DeptStoks.Find(id);
            if (deptStok == null)
            {
                return HttpNotFound();
            }
            return View(deptStok);
        }

        // GET: MedEngMgt/DeptStoks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/DeptStoks/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DeptStok deptStok)
        {
            if (ModelState.IsValid)
            {
                deptStok.Rtp = WebSecurity.CurrentUserId;
                deptStok.Rtt = DateTime.Now;
                db.DeptStoks.Add(deptStok);
                db.SaveChanges();
                // Save log. 
                SystemLog log = new SystemLog();
                log.LogClass = "醫療儀器紀錄";
                log.LogTime = DateTime.UtcNow.AddHours(8);
                log.UserId = WebSecurity.CurrentUserId;
                log.Action = "庫存管理 > 新增 > " + deptStok.StokNam + "(" + deptStok.StokNo + ")";
                db.SystemLogs.Add(log);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(deptStok);
        }

        public ActionResult InStock()
        {

            return View();
        }
        [HttpPost]
        public ActionResult InStock(StokRecord stokRecord)
        {
            if (ModelState.IsValid)
            {
                DeptStok sk = db.DeptStoks.Where(d => d.StokNo == stokRecord.StokNo).ToList().FirstOrDefault();
                if (sk != null)
                {
                    if (stokRecord.InOut == "進庫")
                        sk.Qty += 1;
                    else
                        sk.Qty -= 1;
                    db.Entry(sk).State = EntityState.Modified;
                }
                stokRecord.Rtp = WebSecurity.CurrentUserId;
                stokRecord.Rtt = DateTime.Now;
                db.StokRecords.Add(stokRecord);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex);
                    return View(stokRecord);
                }

                return RedirectToAction("Index");
            }

            return View(stokRecord);
        }
        // GET: MedEngMgt/DeptStoks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeptStok deptStok = db.DeptStoks.Find(id);
            if (deptStok == null)
            {
                return HttpNotFound();
            }
            return View(deptStok);
        }

        // POST: MedEngMgt/DeptStoks/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit(DeptStok deptStok)
        {
            if (ModelState.IsValid)
            {
                deptStok.Rtp = WebSecurity.CurrentUserId;
                deptStok.Rtt = DateTime.Now;
                db.Entry(deptStok).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(deptStok);
        }

        // GET: MedEngMgt/DeptStoks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeptStok deptStok = db.DeptStoks.Find(id);
            if (deptStok == null)
            {
                return HttpNotFound();
            }
            return View(deptStok);
        }

        // POST: MedEngMgt/DeptStoks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DeptStok deptStok = db.DeptStoks.Find(id);
            db.DeptStoks.Remove(deptStok);
            db.SaveChanges();
            // Save log. 
            SystemLog log = new SystemLog();
            log.LogClass = "醫療儀器紀錄";
            log.LogTime = DateTime.UtcNow.AddHours(8);
            log.UserId = WebSecurity.CurrentUserId;
            log.Action = "庫存管理 > 刪除 > " + deptStok.StokNam + "(" + deptStok.StokNo + ")";
            db.SystemLogs.Add(log);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Choose()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Choose(FormCollection fc)
        {
            string cname = fc["qtyCNAME"];
            string stokid = fc["qtySTOKID"];
            List<DeptStok> dv = db.DeptStoks.Where(d => d.Status == "Y").ToList();
            if (!string.IsNullOrEmpty(cname))
            {
                dv = dv.Where(d => d.StokNam.Contains(cname)).ToList();
            }
            if (!string.IsNullOrEmpty(stokid))
            {
                dv = dv.Where(d => d.StokNo == stokid).ToList();
            }
            return PartialView("SelectList", dv);
        }

        public ActionResult List()
        {
            return PartialView();
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
