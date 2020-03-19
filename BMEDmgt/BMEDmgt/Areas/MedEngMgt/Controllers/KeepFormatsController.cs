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
    [Authorize]
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
                db.Entry(keepformat).State = EntityState.Modified;
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
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}