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
    public class KeepDtlsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/KeepDtls
        public ActionResult Index()
        {
            return View(db.KeepDtls.ToList());
        }

        // GET: MedEngMgt/KeepDtls/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeepDtl keepDtl = db.KeepDtls.Find(id);
            if (keepDtl == null)
            {
                return HttpNotFound();
            }
            else
            {
                switch (keepDtl.InOut)
                {
                    case "0":
                        keepDtl.InOut = "自行";
                        break;
                    case "1":
                        keepDtl.InOut = "委外";
                        break;
                    case "2":
                        keepDtl.InOut = "租賃";
                        break;
                    case "3":
                        keepDtl.InOut = "保固";
                        break;
                    case "4":
                        keepDtl.InOut = "借用";
                        break;
                    case "5":
                        keepDtl.InOut = "委外/自行";
                        break;
                }
                switch (keepDtl.Result)
                {
                    case "1":
                        keepDtl.Result = "功能正常";
                        break;
                    case "2":
                        keepDtl.Result = "預防處理";
                        break;
                    case "3":
                        keepDtl.Result = "異常處理";
                        break;
                    case "4":
                        keepDtl.Result = "維修時保養";
                        break;
                    case "5":
                        keepDtl.Result = "退件";
                        break;
                }
            }
            return PartialView(keepDtl);
        }

        // GET: MedEngMgt/KeepDtls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/KeepDtls/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocId,DealState,DealDes,FailFactor,InOut,Hour,IsCharged,Cost,EndDate,CloseDate,ShutDate")] KeepDtl keepDtl)
        {
            if (ModelState.IsValid)
            {
                db.KeepDtls.Add(keepDtl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(keepDtl);
        }

        public ActionResult UpdCases(string docid)
        {
            List<SelectListItem> listItem1 = new List<SelectListItem>();
            listItem1.Add(new SelectListItem { Text = "自行", Value = "0" });
            listItem1.Add(new SelectListItem { Text = "委外", Value = "1" });
            listItem1.Add(new SelectListItem { Text = "租賃", Value = "2" });
            listItem1.Add(new SelectListItem { Text = "保固", Value = "3" });
            listItem1.Add(new SelectListItem { Text = "借用", Value = "4" });
            listItem1.Add(new SelectListItem { Text = "委外/自行", Value = "5" });
            ViewData["InOut"] = new SelectList(listItem1, "Value", "Text", "");
            //
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            db.KeepResults.Where(d => d.Flg == "Y")
               .ToList()
               .ForEach(d =>
               {
                   listItem2.Add(new SelectListItem { Text = d.Title, Value = d.Title });
               });

            ViewData["Result"] = new SelectList(listItem2, "Value", "Text", "");
            KeepDtl dtl = new KeepDtl();
            dtl.DocId = docid;
            dtl.IsCharged = "N";
            return PartialView(dtl);
        }

        [HttpPost]
        [MyErrorHandler]
        [ValidateAntiForgeryToken]
        public ActionResult UpdCases(KeepDtl keepDtl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string[] s = keepDtl.DocId.Split(new char[] { ';' });
                    KeepDtl dtl;
                    foreach (string ss in s)
                    {
                        if (!string.IsNullOrEmpty(ss))
                        {
                            dtl = db.KeepDtls.Find(ss);
                            if (dtl == null)
                            {
                                dtl = new KeepDtl();
                                dtl.DocId = ss;
                                db.KeepDtls.Add(dtl);
                                db.SaveChanges();
                            }
                            if (keepDtl.IsCharged == "N")
                            {
                                db.KeepCosts.RemoveRange(db.KeepCosts.Where(c => c.DocId == ss));
                                keepDtl.Cost = 0;
                            }
                            else
                            {
                                keepDtl.Cost = db.KeepCosts.Where(k => k.DocId == ss)
                                .Select(k => k.TotalCost)
                                .DefaultIfEmpty(0).Sum();
                            }
                            dtl.ShutDate = keepDtl.ShutDate;
                            dtl.InOut = keepDtl.InOut;
                            dtl.Result = keepDtl.Result;
                            dtl.Memo = keepDtl.Memo;
                            dtl.Hours = keepDtl.Hours;
                            dtl.IsCharged = keepDtl.IsCharged;
                            dtl.EndDate = keepDtl.EndDate;
                            dtl.Cost = keepDtl.Cost;
                            dtl.CloseDate = keepDtl.CloseDate;
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
        // GET: MedEngMgt/KeepDtls/Edit/5
        public ActionResult Edit(string id)
        {
            List<SelectListItem> listItem1 = new List<SelectListItem>();
            listItem1.Add(new SelectListItem { Text = "自行", Value = "0" });
            listItem1.Add(new SelectListItem { Text = "委外", Value = "1" });
            listItem1.Add(new SelectListItem { Text = "租賃", Value = "2" });
            listItem1.Add(new SelectListItem { Text = "保固", Value = "3" });
            listItem1.Add(new SelectListItem { Text = "借用", Value = "4" });
            listItem1.Add(new SelectListItem { Text = "委外/自行", Value = "5" });
            ViewData["InOut"] = new SelectList(listItem1, "Value", "Text", "");
            //
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            db.KeepResults.Where(d => d.Flg == "Y")
               .ToList()
               .ForEach(d =>
               {
                   listItem2.Add(new SelectListItem { Text = d.Title, Value = d.Title });
               });

            ViewData["Result"] = new SelectList(listItem2, "Value", "Text", "");
            if (id == null)
            {
                KeepDtl dtl = new KeepDtl();
                dtl.IsCharged = "N";
                return PartialView(dtl);
            }
            KeepDtl keepDtl = db.KeepDtls.Find(id);
            if (keepDtl == null)
            {
                return HttpNotFound();
            }
            KeepFlow kf = db.KeepFlows.Where(f => f.DocId == id)
                           .Where(f => f.Status == "?").FirstOrDefault();
            if (kf.Cls.Contains("工程師"))
                return PartialView(keepDtl);
            else
            {
                switch (keepDtl.InOut)
                {
                    case "0":
                        keepDtl.InOut = "自行";
                        break;
                    case "1":
                        keepDtl.InOut = "委外";
                        break;
                    case "2":
                        keepDtl.InOut = "租賃";
                        break;
                    case "3":
                        keepDtl.InOut = "保固";
                        break;
                    case "4":
                        keepDtl.InOut = "借用";
                        break;
                    case "5":
                        keepDtl.InOut = "委外/自行";
                        break;
                }
                switch (keepDtl.Result)
                {
                    case "1":
                        keepDtl.Result = "功能正常";
                        break;
                    case "2":
                        keepDtl.Result = "預防處理";
                        break;
                    case "3":
                        keepDtl.Result = "異常處理";
                        break;
                    case "4":
                        keepDtl.Result = "維修時保養";
                        break;
                    case "5":
                        keepDtl.Result = "退件";
                        break;
                }
                return PartialView("Details", keepDtl);
            }
        }

        // POST: MedEngMgt/KeepDtls/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KeepDtl keepDtl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (keepDtl.IsCharged == "N")
                    {
                        db.KeepCosts.RemoveRange(db.KeepCosts.Where(c => c.DocId == keepDtl.DocId));
                        keepDtl.Cost = 0;
                    }
                    else
                    {
                        keepDtl.Cost = db.KeepCosts.Where(k => k.DocId == keepDtl.DocId)
                        .Select(k => k.TotalCost)
                        .DefaultIfEmpty(0).Sum();
                    }
                    db.Entry(keepDtl).State = EntityState.Modified;
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

        // GET: MedEngMgt/KeepDtls/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeepDtl keepDtl = db.KeepDtls.Find(id);
            if (keepDtl == null)
            {
                return HttpNotFound();
            }
            return View(keepDtl);
        }

        // POST: MedEngMgt/KeepDtls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            KeepDtl keepDtl = db.KeepDtls.Find(id);
            db.KeepDtls.Remove(keepDtl);
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
