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
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class RepairCostsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/RepairCosts
        public ActionResult List(string id)
        {
            List<RepairCost> rc = db.RepairCosts.Where(c => c.DocId == id).ToList();
            rc.ForEach(r => {
                if (r.StockType == "0")
                    r.StockType = "庫存";
                else
                    r.StockType = "發票";
            });
            return PartialView(rc);
        }

        // GET: MedEngMgt/RepairCosts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairCost repairCost = db.RepairCosts.Find(id);
            if (repairCost == null)
            {
                return HttpNotFound();
            }
            return View(repairCost);
        }

        // GET: MedEngMgt/RepairCosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/RepairCosts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RepairCost repairCost)
        {
            if (ModelState.IsValid)
            {
                if (repairCost.StockType != "0")
                    repairCost.PartNo = "";
                db.RepairCosts.Add(repairCost);
                db.SaveChanges();
                //
                RepairDtl dtl = db.RepairDtls.Where(d => d.DocId == repairCost.DocId).ToList()
                                             .FirstOrDefault();
                if (dtl != null)
                {
                    dtl.Cost = db.RepairCosts.Where(k => k.DocId == repairCost.DocId)
                        .Select(k => k.TotalCost)
                        .DefaultIfEmpty(0).Sum();
                    db.Entry(dtl).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(repairCost);
        }

        // GET: MedEngMgt/RepairCosts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return PartialView();
            }
            RepairCost repairCost = new RepairCost();
            int seqno = db.RepairCosts
                .Where(c => c.DocId == id)
                .Select(c => c.SeqNo).DefaultIfEmpty().Max();
            repairCost.DocId = id;
            repairCost.SeqNo = seqno + 1;
            RepairFlow rf = db.RepairFlows.Where(f => f.DocId == id)
               .Where(f => f.Status == "?").ToList().FirstOrDefault();
            if (!(rf.Cls.Contains("工程師") || rf.Cls == "醫工經辦"))
            {
                List<RepairCost> t = db.RepairCosts.Where(c => c.DocId == id).ToList();
                return PartialView("Print", t);
            }

            return PartialView(repairCost);
        }

        // POST: MedEngMgt/RepairCosts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit(RepairCost repairCost)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int seqno = db.RepairCosts.Where(c => c.DocId == repairCost.DocId)
                        .Select(c => c.SeqNo).DefaultIfEmpty().Max();
                    repairCost.SeqNo = seqno + 1;
                    if (repairCost.StockType == "2")
                    {
                        if (string.IsNullOrEmpty(repairCost.TicketDtl.TicketDtlNo))
                        {
                            throw new Exception("發票號碼不可空白!!");
                        }
                        if (repairCost.AccountDate == null)
                        {
                            throw new Exception("發票日期不可空白!!");
                        }
                        int i = db.TicketDtls.Where(d => d.TicketDtlNo == repairCost.TicketDtl.TicketDtlNo)
                            .Select(d => d.SeqNo).DefaultIfEmpty().Max();
                        repairCost.TicketDtl.SeqNo = i + 1;
                        repairCost.TicketDtl.ObjName = repairCost.VendorName;
                        repairCost.TicketDtl.Qty = repairCost.Qty;
                        repairCost.TicketDtl.Unite = repairCost.Unite;
                        repairCost.TicketDtl.Price = repairCost.Price;
                        repairCost.TicketDtl.Cost = repairCost.TotalCost;
                        Ticket t = db.Tickets.Find(repairCost.TicketDtl.TicketDtlNo);
                        if (t == null)
                        {
                            t = new Ticket();
                            t.TicketNo = repairCost.TicketDtl.TicketDtlNo;
                            t.TicDate = repairCost.AccountDate;
                            t.ApplyDate = null;
                            t.CancelDate = null;
                            t.VendorId = repairCost.VendorId;
                            t.VendorName = repairCost.VendorName;
                            repairCost.TicketDtl.Ticket = t;
                            db.Tickets.Add(t);
                        }
                        db.TicketDtls.Add(repairCost.TicketDtl);
                    }
                    else
                    {
                        repairCost.AccountDate = DateTime.Now.Date;
                        repairCost.TicketDtl = null;
                    }
                    repairCost.Rtp = WebSecurity.CurrentUserId;
                    repairCost.Rtt = DateTime.Now;
                    if (repairCost.StockType != "0")
                        repairCost.PartNo = "";
                    db.RepairCosts.Add(repairCost);
                    db.SaveChanges();
                    //
                    RepairDtl dtl = db.RepairDtls.Where(d => d.DocId == repairCost.DocId).ToList()
                                                 .FirstOrDefault();
                    if (dtl != null)
                    {
                        dtl.Cost = db.RepairCosts.Where(k => k.DocId == repairCost.DocId)
                            .Select(k => k.TotalCost)
                            .DefaultIfEmpty(0).Sum();
                        db.Entry(dtl).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    List<RepairCost> rc = db.RepairCosts.Where(c => c.DocId == repairCost.DocId).ToList();
                    rc.ForEach(r => {
                        if (r.StockType == "0")
                            r.StockType = "庫存";
                        else
                            r.StockType = "發票";
                    });
                    return PartialView("List", rc);
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
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
        }

        [MyErrorHandler]
        public ActionResult Delete(string docid, string seqno)
        {
            try
            {
                RepairCost repairCost = db.RepairCosts.Find(docid, Convert.ToInt32(seqno));
                db.RepairCosts.Remove(repairCost);
                db.SaveChanges();
                //
                RepairDtl dtl = db.RepairDtls.Where(d => d.DocId == repairCost.DocId).ToList()
                                             .FirstOrDefault();
                if (dtl != null)
                {
                    dtl.Cost = db.RepairCosts.Where(k => k.DocId == repairCost.DocId)
                        .Select(k => k.TotalCost)
                        .DefaultIfEmpty(0).Sum();
                    db.Entry(dtl).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public ActionResult Print(string id = null)
        {
            List<RepairCost> rc = db.RepairCosts.Where(c => c.DocId == id).ToList();
            rc.ForEach(r => {
                if (r.StockType == "0")
                    r.StockType = "庫存";
                else
                    r.StockType = "發票";
            });
            return PartialView(rc);
        }

        public ActionResult PrintList(string id = null)
        {
            List<RepairCost> rc = db.RepairCosts.Where(c => c.DocId == id).ToList();
           
            return PartialView(rc);
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
