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
using System.Web.Security;
using System.Web.UI.WebControls;
using WebMatrix.WebData;
using BMEDmgt.Filters;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class RepairEmpsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/RepairEmps
        public ActionResult Index()
        {
            return View(db.RepairEmps.ToList());
        }

        public ActionResult List(string id)
        {
            AppUser u;
            List<RepairEmp> emps = db.RepairEmps.Where(p => p.DocId == id).ToList();
            emps.ForEach(rp =>
                {
                    rp.UserName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.UserName;
                    rp.FullName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.FullName;
                });

            return PartialView(emps);
        }

        public ActionResult UpdCasesList(string docid)
        {
            string[] s = docid.Split(new char[] { ';' });
            AppUser u;
            List<RepairEmp> emps;
            List<RepairEmp> emplist = new List<RepairEmp>();
            foreach (string ss in s)
            {
                if (!string.IsNullOrEmpty(ss))
                {
                    emps = db.RepairEmps.Where(p => p.DocId == ss).ToList();
                    emps.ForEach(rp =>
                    {
                        rp.UserName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.UserName;
                        rp.FullName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.FullName;
                    });
                    emplist.AddRange(emps);
                }
            }

            return PartialView(emplist);
        }

        // GET: MedEngMgt/RepairEmps/Details/5
        public ActionResult Details(string id)
        {
            AppUser u;
            List<RepairEmp> emps = db.RepairEmps.Where(p => p.DocId == id).ToList();
            emps.ForEach(rp =>
            {
                rp.UserName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.UserName;
                rp.FullName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.FullName;
            });

            return PartialView(emps);
        }

        // GET: MedEngMgt/RepairEmps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/RepairEmps/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Docid,UserId,Hour,Minute")] RepairEmp repairEmp)
        {
            if (ModelState.IsValid)
            {
                db.RepairEmps.Add(repairEmp);
                db.SaveChanges();
                //
                RepairDtl dtl = db.RepairDtls.Where(d => d.DocId == repairEmp.DocId).ToList()
                                             .FirstOrDefault();
                if (dtl != null)
                {
                    int hr = db.RepairEmps.Where(p => p.DocId == repairEmp.DocId)
                        .Select(p => p.Hour)
                        .DefaultIfEmpty(0).Sum();
                    decimal min = db.RepairEmps.Where(p => p.DocId == repairEmp.DocId)
                        .Select(p => p.Minute)
                        .DefaultIfEmpty(0).Sum();
                    dtl.Hour = hr + Decimal.Round(min / 60m, 2);
                    db.Entry(dtl).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(repairEmp);
        }

        // GET: MedEngMgt/RepairEmps/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RepairEmp repairEmp = db.RepairEmps.Where(p => p.DocId == id).ToList()
                                               .FirstOrDefault();
            if (repairEmp == null)
            {
                repairEmp = new RepairEmp();
                repairEmp.DocId = id;
            }
            var s = Roles.GetUsersInRole("MedEngineer").ToList();
            List<SelectListItem> uids = new List<SelectListItem>();
            AppUser u;
            foreach (string l in s)
            {
                u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (u != null)
                {
                    uids.Add(new SelectListItem
                    {
                        Text = u.FullName,
                        Value = u.Id.ToString()
                    });
                }
            }
            ViewData["UserId"] = uids;
            RepairFlow rf = db.RepairFlows.Where(f => f.DocId == id)
              .Where(f => f.Status == "?").ToList().FirstOrDefault();
            if (!rf.Cls.Contains("工程師"))
            {
                List<RepairEmp> emps = db.RepairEmps.Where(p => p.DocId == id).ToList();
                emps.ForEach(rp =>
                {
                    rp.UserName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.UserName;
                    rp.FullName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.FullName;
                });

                return PartialView("Details", emps);
            }
            // Set default value for dropdownlist.
            if (User.IsInRole("MedEngineer") == true)
            {
                repairEmp.UserId = WebSecurity.CurrentUserId;
            }
            return PartialView(repairEmp);
        }

        // POST: MedEngMgt/RepairEmps/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocId,UserId,Hour,Minute")] RepairEmp repairEmp)
        {
            if (ModelState.IsValid)
            {
                db.RepairEmps.Add(repairEmp);
                db.SaveChanges();
                RepairDtl dtl = db.RepairDtls.Where(d => d.DocId == repairEmp.DocId)
                   .FirstOrDefault();
                if (dtl != null)
                {
                    int hr = db.RepairEmps.Where(p => p.DocId == repairEmp.DocId)
                        .Select(p => p.Hour)
                        .DefaultIfEmpty(0).Sum();
                    decimal min = db.RepairEmps.Where(p => p.DocId == repairEmp.DocId)
                        .Select(p => p.Minute)
                        .DefaultIfEmpty(0).Sum();
                    dtl.Hour = hr + Decimal.Round(min / 60m, 2);
                    db.Entry(dtl).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Edit", new { id = repairEmp.DocId});
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

        // GET: MedEngMgt/RepairEmps/Delete/5
        [MyErrorHandler]
        public ActionResult Delete(string id, int uid)
        {
            try
            {
                RepairEmp repairEmp = db.RepairEmps.Find(id, uid);
                db.RepairEmps.Remove(repairEmp);
                db.SaveChanges();
                //
                RepairDtl dtl = db.RepairDtls.Where(d => d.DocId == repairEmp.DocId).ToList()
                                             .FirstOrDefault();
                if (dtl != null)
                {
                    int hr = db.RepairEmps.Where(p => p.DocId == repairEmp.DocId)
                        .Select(p => p.Hour)
                        .DefaultIfEmpty(0).Sum();
                    decimal min = db.RepairEmps.Where(p => p.DocId == repairEmp.DocId)
                        .Select(p => p.Minute)
                        .DefaultIfEmpty(0).Sum();
                    dtl.Hour = hr + Decimal.Round(min / 60m, 2);
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

        public ActionResult UpdCases(string docid)
        {
            if (docid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RepairEmp repairEmp = new RepairEmp();
            repairEmp.DocId = docid;
            var s = Roles.GetUsersInRole("MedEngineer").ToList();
            List<SelectListItem> uids = new List<SelectListItem>();
            AppUser u;
            foreach (string l in s)
            {
                u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (u != null)
                {
                    uids.Add(new SelectListItem
                    {
                        Text = u.FullName,
                        Value = u.Id.ToString()
                    });
                }
            }
            ViewData["UserId"] = uids;

            return PartialView(repairEmp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdCases(RepairEmp repairEmp)
        {
            if (ModelState.IsValid)
            {
                string[] s = repairEmp.DocId.Split(new char[] { ';' });
                RepairEmp emp;
                RepairDtl dtl;
                foreach (string ss in s)
                {
                    if (!string.IsNullOrEmpty(ss))
                    {
                        emp = new RepairEmp();
                        emp.DocId = ss;
                        emp.UserId = repairEmp.UserId;
                        emp.Hour = repairEmp.Hour;
                        emp.Minute = repairEmp.Minute;
                        db.RepairEmps.Add(emp);
                        db.SaveChanges();
                        //
                        dtl = db.RepairDtls.Where(d => d.DocId == ss).ToList()
                                           .FirstOrDefault();
                        if (dtl != null)
                        {
                            int hr = db.RepairEmps.Where(p => p.DocId == ss)
                                .Select(p => p.Hour)
                                .DefaultIfEmpty(0).Sum();
                            decimal min = db.RepairEmps.Where(p => p.DocId == ss)
                                .Select(p => p.Minute)
                                .DefaultIfEmpty(0).Sum();
                            dtl.Hour = hr + Decimal.Round(min / 60m, 2);
                            db.Entry(dtl).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }

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
