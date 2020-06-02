using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class KeepFormatDtlsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        //
        // GET: /KeepFormat_Dtl/

        public ActionResult Index(string id = null)
        {
            if (id != null)
            {
                ViewData["fid"] = id;
                return PartialView(db.KeepFormatDtls.Where(d => d.FormatId == id).ToList());
            }
            return View(db.KeepFormatDtls.ToList());
        }

        //
        // GET: /KeepFormat_Dtl/Details/5

        public ActionResult Details(string id = null, int sno = 0)
        {
            KeepFormatDtl keepformat_dtl = db.KeepFormatDtls.Find(id, sno);
            if (keepformat_dtl == null)
            {
                return HttpNotFound();
            }
            return View(keepformat_dtl);
        }

        //
        // GET: /KeepFormat_Dtl/Create

        public ActionResult Create(string id = null)
        {
            if (id != null)
            {
                KeepFormatDtl keepformat_dtl = db.KeepFormatDtls.Where(d => d.FormatId == id).OrderByDescending(d => d.Sno)
                    .ToList().FirstOrDefault();
                if (keepformat_dtl != null)
                {
                    keepformat_dtl.Sno += 1;
                    keepformat_dtl.Descript = "";
                    return View(keepformat_dtl);
                }
                else
                {
                    keepformat_dtl = new KeepFormatDtl();
                    keepformat_dtl.FormatId = id;
                    keepformat_dtl.Sno = 1;
                    return View(keepformat_dtl);
                }
            }
            return View();
        }

        //
        // POST: /KeepFormat_Dtl/Create

        [HttpPost]
        public ActionResult Create(KeepFormatDtl keepformat_dtl)
        {
            if (ModelState.IsValid)
            {
                db.KeepFormatDtls.Add(keepformat_dtl);
                db.SaveChanges();
                return RedirectToAction("Edit", "KeepFormats", new { id = keepformat_dtl.FormatId, sno = keepformat_dtl.Sno});
            }

            return View(keepformat_dtl);
        }

        //
        // GET: /KeepFormat_Dtl/Edit/5

        public ActionResult Edit(string id = null, int sno = 0)
        {
            KeepFormatDtl keepformat_dtl = db.KeepFormatDtls.Find(id, sno);
            if (keepformat_dtl == null)
            {
                return HttpNotFound();
            }
            return View(keepformat_dtl);
        }

        //
        // POST: /KeepFormat_Dtl/Edit/5

        [HttpPost]
        public ActionResult Edit(KeepFormatDtl keepformat_dtl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(keepformat_dtl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit", "KeepFormats", new { id = keepformat_dtl.FormatId, sno = keepformat_dtl.Sno });
            }
            return View(keepformat_dtl);
        }

        //
        // GET: /KeepFormat_Dtl/Delete/5

        public ActionResult Delete(string id = null, int sno = 0)
        {
            KeepFormatDtl keepformat_dtl = db.KeepFormatDtls.Find(id, sno);
            if (keepformat_dtl == null)
            {
                return HttpNotFound();
            }
            return View(keepformat_dtl);
        }

        //
        // POST: /KeepFormat_Dtl/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(KeepFormatDtl keepformat_dtl)
        {
            KeepFormatDtl kdtl = db.KeepFormatDtls.Find(keepformat_dtl.FormatId, keepformat_dtl.Sno);
            if (kdtl != null)
            {
                db.KeepFormatDtls.Remove(kdtl);
                db.SaveChanges();
                return RedirectToAction("Edit", "KeepFormats", new { id = keepformat_dtl.FormatId, sno = keepformat_dtl.Sno });
            }
            return View(keepformat_dtl);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}