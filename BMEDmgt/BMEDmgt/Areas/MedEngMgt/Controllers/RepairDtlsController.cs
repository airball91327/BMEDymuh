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
    public class RepairDtlsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/RepairDtls
        public ActionResult Index()
        {
            return View(db.RepairDtls.ToList());
        }

        // GET: MedEngMgt/RepairDtls/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairDtl repairDtl = db.RepairDtls.Find(id);
            if (repairDtl == null)
            {
                return HttpNotFound();
            }
            return PartialView(repairDtl);
        }

        // GET: MedEngMgt/RepairDtls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/RepairDtls/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocId,DealState,DealDes,FailFactor,InOut,Hour,IsCharged,Cost,EndDate,CloseDate,ShutDate")] RepairDtl repairDtl)
        {
            if (ModelState.IsValid)
            {
                db.RepairDtls.Add(repairDtl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(repairDtl);
        }

        // GET: MedEngMgt/RepairDtls/Edit/5
        public ActionResult Edit(string id)
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            db.DealStatus.Where(d => d.Flg == "Y")
                .ToList()
                .ForEach(d => {
                    listItem.Add(new SelectListItem { Text = d.Title, Value = d.Title });
                });
           
            ViewData["States"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            db.FailFactors.Where(d => d.Flg == "Y")
               .ToList()
               .ForEach(d => {
                   listItem2.Add(new SelectListItem { Text = d.Title, Value = d.Title });
               });
           
            ViewData["Factors"] = new SelectList(listItem2, "Value", "Text", "");
            if (id == null)
            {
                RepairDtl dtl = new RepairDtl();
                dtl.IsCharged = "N";
                return PartialView(dtl);
            }
            RepairDtl repairDtl = db.RepairDtls.Find(id);
            if (repairDtl == null)
            {
                return HttpNotFound();
            }
            RepairFlow rf = db.RepairFlows.Where(f => f.DocId == id)
                .Where(f => f.Status == "?").FirstOrDefault();
            if (rf.Cls.Contains("工程師") || rf.Cls == "醫工經辦")
                return PartialView(repairDtl);
            else
                return PartialView("Details", repairDtl);
        }

        // POST: MedEngMgt/RepairDtls/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RepairDtl repairDtl)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(repairDtl.DealDes))
                {
                    throw new Exception("請輸入[處理描述]!!");
                }
                try
                {
                    if (repairDtl.IsCharged == "N")
                    {
                        db.RepairCosts.RemoveRange(db.RepairCosts.Where(c => c.DocId == repairDtl.DocId));
                        repairDtl.Cost = 0;
                    }
                    else
                    {
                        repairDtl.Cost = db.RepairCosts.Where(k => k.DocId == repairDtl.DocId)
                        .Select(k => k.TotalCost)
                        .DefaultIfEmpty(0).Sum();
                        if (repairDtl.EndDate != null && repairDtl.Cost <= 0)
                        {
                            throw new Exception("請先維護費用明細資料，才可以儲存完工日期!!");
                        }
                        else
                        {
                            DeptStok dk;
                            StokRecord sr;
                            if (db.StokRecords.Where(r => r.DocId == repairDtl.DocId).Count() <= 0)
                            {
                                db.RepairCosts.Where(k => k.DocId == repairDtl.DocId)
                                    .Where(k => k.StockType == "0").ToList()
                                    .ForEach(c =>
                                    {
                                        dk = db.DeptStoks.Where(d => d.StokNo == c.PartNo).FirstOrDefault();
                                        if (dk != null)
                                        {
                                            sr = new StokRecord();
                                            sr.StokNo = dk.StokNo;
                                            sr.StokNam = dk.StokNam;
                                            sr.RecordType = "請修";
                                            sr.DocId = repairDtl.DocId;
                                            sr.InOut = "出庫";
                                            sr.Amt = c.Qty;
                                            sr.TotalAmt = dk.Qty;
                                            sr.Rtp = WebSecurity.CurrentUserId;
                                            sr.Rtt = DateTime.Now;
                                            db.StokRecords.Add(sr);
                                        //
                                        dk.Qty -= c.Qty;
                                            dk.Rtp = WebSecurity.CurrentUserId;
                                            dk.Rtt = DateTime.Now;
                                            db.Entry(dk).State = EntityState.Modified;
                                        }
                                    });
                                try
                                {
                                    db.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                    }
                    db.Entry(repairDtl).State = EntityState.Modified;
                    db.SaveChanges();

                    // 多筆儲存處理描述
                    RepairDtlRecord rdtlRecord = new RepairDtlRecord();
                    rdtlRecord.DocId = repairDtl.DocId;
                    rdtlRecord.UserId = WebSecurity.CurrentUserId;
                    rdtlRecord.DealDes = repairDtl.DealDes;
                    rdtlRecord.Rtt = DateTime.Now;
                    db.RepairDtlRecords.Add(rdtlRecord);
                    db.SaveChanges();

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

        // GET: MedEngMgt/RepairDtls/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairDtl repairDtl = db.RepairDtls.Find(id);
            if (repairDtl == null)
            {
                return HttpNotFound();
            }
            return View(repairDtl);
        }

        // POST: MedEngMgt/RepairDtls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            RepairDtl repairDtl = db.RepairDtls.Find(id);
            db.RepairDtls.Remove(repairDtl);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UpdCases(string docid)
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            db.DealStatus.Where(d => d.Flg == "Y")
                .ToList()
                .ForEach(d => {
                    listItem.Add(new SelectListItem { Text = d.Title, Value = d.Title });
                });

            ViewData["States"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            db.FailFactors.Where(d => d.Flg == "Y")
               .ToList()
               .ForEach(d => {
                   listItem2.Add(new SelectListItem { Text = d.Title, Value = d.Title });
               });

            ViewData["Factors"] = new SelectList(listItem2, "Value", "Text", "");

            RepairDtl dtl = new RepairDtl();
            dtl.DocId = docid;
            dtl.IsCharged = "N";
            return PartialView(dtl);
        }

        [HttpPost]
        [MyErrorHandler]
        [ValidateAntiForgeryToken]
        public ActionResult UpdCases(RepairDtl repairDtl)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(repairDtl.DealDes))
                {
                    throw new Exception("請輸入[處理描述]!!");
                }
                try
                {
                    string[] s = repairDtl.DocId.Split(new char[] { ';' });
                    RepairDtl dtl;
                    foreach (string ss in s)
                    {
                        if (!string.IsNullOrEmpty(ss))
                        {
                            dtl = db.RepairDtls.Find(ss);
                            if (dtl == null)
                            {
                                dtl = new RepairDtl();
                                dtl.DocId = ss;
                                db.RepairDtls.Add(dtl);
                                db.SaveChanges();
                            }
                            if (repairDtl.IsCharged == "N")
                            {
                                db.RepairCosts.RemoveRange(db.RepairCosts.Where(c => c.DocId == ss));
                                repairDtl.Cost = 0;
                            }
                            else
                            {
                                repairDtl.Cost = db.RepairCosts.Where(k => k.DocId == ss)
                                .Select(k => k.TotalCost)
                                .DefaultIfEmpty(0).Sum();
                                if (repairDtl.EndDate != null && repairDtl.Cost <= 0)
                                {
                                    throw new Exception("請先維護費用明細資料，才可以儲存完工日期!!");
                                }
                                else
                                {
                                    DeptStok dk;
                                    StokRecord sr;
                                    if (db.StokRecords.Where(r => r.DocId == ss).Count() <= 0)
                                    {
                                        db.RepairCosts.Where(k => k.DocId == ss)
                                            .Where(k => k.StockType == "0").ToList()
                                            .ForEach(c =>
                                            {
                                                dk = db.DeptStoks.Where(d => d.StokNo == c.PartNo).FirstOrDefault();
                                                if (dk != null)
                                                {
                                                    sr = new StokRecord();
                                                    sr.StokNo = dk.StokNo;
                                                    sr.StokNam = dk.StokNam;
                                                    sr.RecordType = "請修";
                                                    sr.DocId = ss;
                                                    sr.InOut = "出庫";
                                                    sr.Amt = c.Qty;
                                                    sr.TotalAmt = dk.Qty;
                                                    sr.Rtp = WebSecurity.CurrentUserId;
                                                    sr.Rtt = DateTime.Now;
                                                    db.StokRecords.Add(sr);
                                                    //
                                                    dk.Qty -= c.Qty;
                                                    dk.Rtp = WebSecurity.CurrentUserId;
                                                    dk.Rtt = DateTime.Now;
                                                    db.Entry(dk).State = EntityState.Modified;
                                                }
                                            });
                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(ex.Message);
                                        }
                                    }
                                }
                            }
                            dtl.ShutDate = repairDtl.ShutDate;
                            dtl.InOut = repairDtl.InOut;
                            dtl.DealState = repairDtl.DealState;
                            dtl.DealDes = repairDtl.DealDes;
                            dtl.FailFactor = repairDtl.FailFactor;
                            dtl.Hour = repairDtl.Hour;
                            dtl.IsCharged = repairDtl.IsCharged;
                            dtl.EndDate = repairDtl.EndDate;
                            dtl.Cost = repairDtl.Cost;
                            dtl.CloseDate = repairDtl.CloseDate;
                            dtl.PurchaseNo = repairDtl.PurchaseNo;
                            db.Entry(dtl).State = EntityState.Modified;
                            db.SaveChanges();
                        }
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

        public ActionResult DealDesList(string id)
        {
            AppUser u;
            List<RepairDtlRecord> rdtl = db.RepairDtlRecords.Where(r => r.DocId == id).ToList();
            rdtl.ForEach(rp =>
            {
                rp.UserName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.UserName;
                rp.FullName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.FullName;
            });

            return PartialView(rdtl);
        }

        public ActionResult DealDesDetails(string id)
        {
            AppUser u;
            List<RepairDtlRecord> rdtl = db.RepairDtlRecords.Where(r => r.DocId == id).ToList();
            rdtl.ForEach(rp =>
            {
                rp.UserName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.UserName;
                rp.FullName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.FullName;
            });

            return PartialView(rdtl);
        }

        [MyErrorHandler]
        public ActionResult DeleteDesRecord(string id, int seqNo)
        {
            try
            {
                RepairDtlRecord repairDtlRecord = db.RepairDtlRecords.Find(id, seqNo);
                db.RepairDtlRecords.Remove(repairDtlRecord);
                db.SaveChanges();
                //
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
