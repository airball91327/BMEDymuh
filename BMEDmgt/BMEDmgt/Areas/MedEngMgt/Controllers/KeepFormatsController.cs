using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Extensions;
using BMEDmgt.Models;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class KeepFormatsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        //
        // GET: /KeepFormat/

        public ActionResult Index()
        {
            return View(db.KeepFormats.ToList());
        }

        //
        // GET: /KeepFormat/Details/5

        public ActionResult Details(string id = null)
        {
            KeepFormat keepformat = db.KeepFormats.Find(Server.HtmlDecode(id));
            if (keepformat == null)
            {
                return HttpNotFound();
            }
            return View(keepformat);
        }

        //
        // GET: /KeepFormat/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /KeepFormat/Create

        [HttpPost]
        public ActionResult Create(KeepFormat keepformat)
        {
            if (ModelState.IsValid)
            {
                db.KeepFormats.Add(keepformat);
                db.SaveChanges();
                // Save log. 
                SystemLog log = new SystemLog();
                log.LogClass = "醫療儀器紀錄";
                log.LogTime = DateTime.UtcNow.AddHours(8);
                log.UserId = WebSecurity.CurrentUserId;
                log.Action = "保養格式 > 新增 > " + keepformat.FormatId;
                db.SystemLogs.Add(log);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(keepformat);
        }

        //
        // GET: /KeepFormat/Edit/5

        public ActionResult Edit(string id = null)
        {
            KeepFormat keepformat = db.KeepFormats.Find(Server.HtmlDecode(id));
            if (keepformat == null)
            {
                return HttpNotFound();
            }
            return View(keepformat);
        }

        //
        // POST: /KeepFormat/Edit/5

        [HttpPost]
        public ActionResult Edit(KeepFormat keepformat)
        {
            if (ModelState.IsValid)
            {
                var oriObj = db.KeepFormats.Find(keepformat.FormatId);
                db.Entry(oriObj).State = EntityState.Detached;
                //
                db.Entry(keepformat).State = EntityState.Modified;
                db.SaveChanges();
                // Save edit log.
                var currentObj = db.KeepFormats.Find(keepformat.FormatId);
                var result = oriObj.EnumeratePropertyDifferences<KeepFormat>(currentObj);
                SystemLog log = new SystemLog();
                log.LogClass = "醫療儀器紀錄";
                log.LogTime = DateTime.UtcNow.AddHours(8);
                log.UserId = WebSecurity.CurrentUserId;
                log.Action = "保養格式 > 編輯 > " + keepformat.FormatId;
                if (result.Count() > 0)
                {
                    foreach (string s in result)
                    {
                        log.Action += "【" + s + "】";
                    }
                }
                db.SystemLogs.Add(log);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(keepformat);
        }

        public ActionResult GetPlants(string id = null)
        {
            if (id != null)
                return Content(db.KeepFormats.Find(id).Plants);
            return Content("");
        }
        //
        // GET: /KeepFormat/Delete/5

        public ActionResult Delete(string id = null)
        {
            KeepFormat keepformat = db.KeepFormats.Find(id);
            if (keepformat == null)
            {
                return HttpNotFound();
            }
            return View(keepformat);
        }

        //
        // POST: /KeepFormat/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            KeepFormat keepformat = db.KeepFormats.Find(id);
            db.KeepFormats.Remove(keepformat);
            db.SaveChanges();
            // Save log. 
            SystemLog log = new SystemLog();
            log.LogClass = "醫療儀器紀錄";
            log.LogTime = DateTime.UtcNow.AddHours(8);
            log.UserId = WebSecurity.CurrentUserId;
            log.Action = "保養格式 > 刪除 > " + keepformat.FormatId;
            db.SystemLogs.Add(log);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}