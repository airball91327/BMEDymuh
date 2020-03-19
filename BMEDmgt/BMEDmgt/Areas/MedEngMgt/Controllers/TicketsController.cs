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
using BMEDmgt.Filters;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/Tickets
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection fm)
        {
            string ticketno = fm["qtyTICKET"];
            string vendornam = fm["qtyVENDORNAM"];
            List<Ticket> ts = new List<Ticket>();
            db.Tickets.ToList()
                .ForEach(t => {
                    if (!string.IsNullOrEmpty(ticketno))
                    {
                        if (t.TicketNo == ticketno)
                            ts.Add(t);
                    }
                    if (!string.IsNullOrEmpty(vendornam))
                    {
                        if (!string.IsNullOrEmpty(t.VendorName))
                        {
                            if (t.VendorName.Contains(vendornam))
                                ts.Add(t);
                        }                        
                    }
                        });
            
            return PartialView("List", ts);
        }

        public ActionResult List()
        {
            //List<Ticket> ts = db.RepairCosts.
            //    Join(db.Tickets, r => r.TicketDtl.TicketDtlNo, t => t.TicketNo,
            //    (r, t) => t).ToList();

            return PartialView(db.Tickets.ToList());
        }

        // GET: MedEngMgt/Tickets/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: MedEngMgt/Tickets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/Tickets/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TicketNo,TicDate,VendorId,VendorName,TotalAmt,TaxAmt,Note,ScrapValue,ApplyDate,CancelDate")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticket);
        }

        // GET: MedEngMgt/Tickets/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Ticket ticket = db.Tickets.Find(id);
            decimal total = db.TicketDtls.Where(t => t.TicketDtlNo == id).DefaultIfEmpty()
                .Sum(t => t.Cost);
            
            ticket.ScrapValue = ticket.TotalAmt - Convert.ToInt32(total);

            return View(ticket);
        }

        // POST: MedEngMgt/Tickets/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit([Bind(Include = "TicketNo,TicDate,VendorId,VendorName,TotalAmt,TaxAmt,Note,ScrapValue,ApplyDate,CancelDate")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                decimal total = db.TicketDtls.Where(t => t.TicketDtlNo == ticket.TicketNo).DefaultIfEmpty()
                .Sum(t => t.Cost);

                ticket.ScrapValue = ticket.TotalAmt - Convert.ToInt32(total);
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                string msg = "";
                foreach (var error in ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors))
                {
                    msg += error.ErrorMessage + Environment.NewLine;
                }
                throw new Exception(msg);
            }
            return View(ticket);
        }

        // GET: MedEngMgt/Tickets/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: MedEngMgt/Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public string GetTickeSeq()
        {
            string result = "";
            int seq = 0;
            string yyymm = Convert.ToString((DateTime.Now.Year - 1911) * 100 + DateTime.Now.Month);
            int cnt = db.Ticket_seq_tmps.Where(t => t.YYYMM == yyymm).Count();
            Ticket_seq_tmp tmp;
            if (cnt > 0)
            {
                tmp = db.Ticket_seq_tmps.Find(yyymm);
                seq = Convert.ToInt32(tmp.TICKET_SEQ) + 1;
                result = Convert.ToString(seq);
                tmp.TICKET_SEQ = Convert.ToString(seq);
                db.Entry(tmp).State = EntityState.Modified;
            }
            else
            {
                tmp = new Ticket_seq_tmp();
                seq = Convert.ToInt32(yyymm) * 1000 + 1;
                tmp.YYYMM = Convert.ToString(yyymm);
                tmp.TICKET_SEQ = Convert.ToString(seq);
                result = Convert.ToString(seq);
                db.Ticket_seq_tmps.Add(tmp);
            }
            db.SaveChanges();

            return result;
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
