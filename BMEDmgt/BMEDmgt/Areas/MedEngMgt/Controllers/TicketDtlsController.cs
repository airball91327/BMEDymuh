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
    [Authorize]
    public class TicketDtlsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/TicketDtls
        public ActionResult Index(string id)
        {
            List<TicketDtl> dtls = db.TicketDtls.Where(t => t.TicketDtlNo == id).ToList();
            ViewData["Total"] = dtls.Sum(t => t.Cost);
            return PartialView(dtls);
        }

        // GET: MedEngMgt/TicketDtls/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketDtl ticketDtl = db.TicketDtls.Find(id);
            if (ticketDtl == null)
            {
                return HttpNotFound();
            }
            return View(ticketDtl);
        }

        // GET: MedEngMgt/TicketDtls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/TicketDtls/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TicketDtlNo,SeqNo,ObjName,Qty,Unite,Price,Cost")] TicketDtl ticketDtl)
        {
            if (ModelState.IsValid)
            {
                db.TicketDtls.Add(ticketDtl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticketDtl);
        }

        // GET: MedEngMgt/TicketDtls/Edit/5
        public ActionResult Edit(string id, int seq)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketDtl ticketDtl = db.TicketDtls.Find(id, seq);
            if (ticketDtl == null)
            {
                return HttpNotFound();
            }
            return View(ticketDtl);
        }

        // POST: MedEngMgt/TicketDtls/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TicketDtlNo,SeqNo,ObjName,Qty,Unite,Price,Cost")] TicketDtl ticketDtl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketDtl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticketDtl);
        }

        // GET: MedEngMgt/TicketDtls/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketDtl ticketDtl = db.TicketDtls.Find(id);
            if (ticketDtl == null)
            {
                return HttpNotFound();
            }
            return View(ticketDtl);
        }

        // POST: MedEngMgt/TicketDtls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TicketDtl ticketDtl = db.TicketDtls.Find(id);
            db.TicketDtls.Remove(ticketDtl);
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
