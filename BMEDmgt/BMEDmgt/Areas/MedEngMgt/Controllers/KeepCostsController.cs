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
    public class KeepCostsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/KeepCosts
        public ActionResult List(string id)
        {
            List<KeepCost> kc = db.KeepCosts.Where(c => c.DocId == id).ToList();
            foreach (KeepCost k in kc)
            {
                if (k.StockType == "0")
                    k.StockType = "庫存";
                else
                    k.StockType = "發票";
            }
                
            return PartialView(kc);
        }

        // GET: MedEngMgt/KeepCosts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeepCost keepCost = db.KeepCosts.Find(id);
            if (keepCost == null)
            {
                return HttpNotFound();
            }
            return View(keepCost);
        }

        // GET: MedEngMgt/KeepCosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/KeepCosts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KeepCost keepCost)
        {
            if (ModelState.IsValid)
            {
                if (keepCost.StockType != "0")
                    keepCost.PartNo = "";
                db.KeepCosts.Add(keepCost);
                db.SaveChanges();
                //
                KeepDtl dtl = db.KeepDtls.Where(d => d.DocId == keepCost.DocId).ToList()
                    .FirstOrDefault();
                if (dtl != null)
                {
                    dtl.Cost = db.KeepCosts.Where(k => k.DocId == keepCost.DocId)
                        .Select(k => k.TotalCost)
                        .DefaultIfEmpty(0).Sum();
                    db.Entry(dtl).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(keepCost);
        }

        // GET: MedEngMgt/KeepCosts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return PartialView();
            }
            KeepCost keepCost = new KeepCost();
            int seqno = db.KeepCosts
                .Where(c => c.DocId == id)
                .Select(c => c.SeqNo).DefaultIfEmpty().Max();
            keepCost.DocId = id;
            keepCost.SeqNo = seqno + 1;
            KeepFlow kf = db.KeepFlows.Where(f => f.DocId == id)
                          .Where(f => f.Status == "?").ToList().FirstOrDefault();
            if (!(kf.Cls.Contains("工程師") || kf.Cls == "醫工經辦"))
            {
                List<KeepCost> t = db.KeepCosts.Where(c => c.DocId == id).ToList();
                return PartialView("Print", t);
            }

            return PartialView(keepCost);
        }

        // POST: MedEngMgt/KeepCosts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit(KeepCost keepCost)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int seqno = db.KeepCosts.Where(c => c.DocId == keepCost.DocId)
                        .Select(c => c.SeqNo).DefaultIfEmpty().Max();
                    keepCost.SeqNo = seqno + 1;
                    if (keepCost.StockType == "2")
                    {
                        if (string.IsNullOrEmpty(keepCost.TicketDtl.TicketDtlNo))
                        {
                            throw new Exception("發票號碼不可空白!!");
                        }
                        if (keepCost.AccountDate == null)
                        {
                            throw new Exception("發票日期不可空白!!");
                        }
                        int i = db.TicketDtls.Where(d => d.TicketDtlNo == keepCost.TicketDtl.TicketDtlNo)
                            .Select(d => d.SeqNo).DefaultIfEmpty().Max();
                        keepCost.TicketDtl.SeqNo = i + 1;
                        keepCost.TicketDtl.ObjName = keepCost.VendorName;
                        keepCost.TicketDtl.Qty = keepCost.Qty;
                        keepCost.TicketDtl.Unite = keepCost.Unite;
                        keepCost.TicketDtl.Price = keepCost.Price;
                        keepCost.TicketDtl.Cost = keepCost.TotalCost;
                        Ticket t = db.Tickets.Find(keepCost.TicketDtl.TicketDtlNo);
                        if (t == null)
                        {
                            t = new Ticket();
                            t.TicketNo = keepCost.TicketDtl.TicketDtlNo;
                            t.TicDate = keepCost.AccountDate;
                            t.ApplyDate = null;
                            t.CancelDate = null;
                            t.VendorId = keepCost.VendorId;
                            t.VendorName = keepCost.VendorName;
                            keepCost.TicketDtl.Ticket = t;
                            db.Tickets.Add(t);
                        }
                        db.TicketDtls.Add(keepCost.TicketDtl);
                    }
                    else
                    {
                        keepCost.AccountDate = DateTime.Now.Date;
                        keepCost.TicketDtl = null;
                    }
                    keepCost.Rtp = WebSecurity.CurrentUserId;
                    keepCost.Rtt = DateTime.Now;
                    if (keepCost.StockType != "0")
                        keepCost.PartNo = "";
                    db.KeepCosts.Add(keepCost);
                    db.SaveChanges();
                    //
                    KeepDtl dtl = db.KeepDtls.Where(d => d.DocId == keepCost.DocId).ToList()
                        .FirstOrDefault();
                    if (dtl != null)
                    {
                        dtl.Cost = db.KeepCosts.Where(k => k.DocId == keepCost.DocId)
                        .Select(k => k.TotalCost)
                        .DefaultIfEmpty(0).Sum();
                        db.Entry(dtl).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    List<KeepCost> kc = db.KeepCosts.Where(c => c.DocId == keepCost.DocId).ToList();
                    kc.ForEach(k =>
                    {
                        if (k.StockType == "0")
                            k.StockType = "庫存";
                        else
                            k.StockType = "發票";
                    });
                    return PartialView("List", kc);
                    //return new JsonResult
                    //{
                    //    Data = new { success = true, error = "" },
                    //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    //};
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

        // GET: MedEngMgt/KeepCosts/Delete/5
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Delete(string docid, string seqno)
        {
            try
            {
                KeepCost keepCost = db.KeepCosts.Find(docid, Convert.ToInt32(seqno));
                db.KeepCosts.Remove(keepCost);
                db.SaveChanges();
                //
                KeepDtl dtl = db.KeepDtls.Where(d => d.DocId == keepCost.DocId).ToList()
                    .FirstOrDefault();
                if (dtl != null)
                {
                    dtl.Cost = db.KeepCosts.Where(k => k.DocId == docid)
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
            List<KeepCost> t = db.KeepCosts.Where(c => c.DocId == id).ToList();
            foreach (KeepCost k in t)
            {
                if (k.StockType == "0")
                    k.StockType = "庫存";
                else
                    k.StockType = "發票";
            }
            return PartialView(t);
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
