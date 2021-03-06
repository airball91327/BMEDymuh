﻿using System;
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
    public class KeepEmpsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/RepairEmps
        public ActionResult Index()
        {
            return View(db.KeepEmps.ToList());
        }

        public ActionResult UpdCasesList(string docid)
        {
            string[] s = docid.Split(new char[] { ';' });
            AppUser u;
            List<KeepEmp> emps;
            List<KeepEmp> emplist = new List<KeepEmp>();
            foreach (string ss in s)
            {
                if (!string.IsNullOrEmpty(ss))
                {
                    emps = db.KeepEmps.Where(p => p.DocId == ss).ToList();
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

        public ActionResult List(string id)
        {
            AppUser u;
            List<KeepEmp> emps = db.KeepEmps.Where(p => p.DocId == id).ToList();
            emps.ForEach(rp =>
            {
                rp.UserName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.UserName;
                rp.FullName = (u = db.AppUsers.Find(rp.UserId)) == null ? "" : u.FullName;
            });
            return PartialView(emps);
        }

        // GET: MedEngMgt/RepairEmps/Details/5
        public ActionResult Details(string id)
        {
            AppUser u;
            List<KeepEmp> emps = db.KeepEmps.Where(p => p.DocId == id).ToList();
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
        public ActionResult Create(KeepEmp keepEmp)
        {
            if (ModelState.IsValid)
            {
                db.KeepEmps.Add(keepEmp);
                db.SaveChanges();
                //
                //
                KeepDtl dtl = db.KeepDtls.Where(d => d.DocId == keepEmp.DocId).ToList()
                    .FirstOrDefault();
                if (dtl != null)
                {
                    int hr = db.KeepEmps.Where(p => p.DocId == keepEmp.DocId)
                        .Select(p => p.Hour)
                        .DefaultIfEmpty(0).Sum();
                    decimal min = db.KeepEmps.Where(p => p.DocId == keepEmp.DocId)
                        .Select(p => p.Minute)
                        .DefaultIfEmpty(0).Sum();
                    dtl.Hours = hr + Decimal.Round(min / 60m, 2);
                    db.Entry(dtl).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //
                return RedirectToAction("Index");
            }

            return View(keepEmp);
        }

        public ActionResult UpdCases(string docid)
        {
            if (docid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeepEmp emp = new KeepEmp();
            emp.DocId = docid;
            //
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

            return PartialView(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MyErrorHandler]
        public ActionResult UpdCases(KeepEmp keepEmp)
        {
            if (ModelState.IsValid)
            {
                string[] s = keepEmp.DocId.Split(new char[] { ';' });
                KeepEmp emp, empTemp;
                KeepDtl dtl;
                foreach (string ss in s)
                {
                    if (!string.IsNullOrEmpty(ss))
                    {
                        empTemp = db.KeepEmps.Where(e => e.DocId == ss && e.UserId == keepEmp.UserId).ToList().FirstOrDefault();
                        if (empTemp != null)
                        {
                            throw new Exception("已有該工程師工時!");
                        }
                        emp = new KeepEmp();
                        emp.DocId = ss;
                        emp.UserId = keepEmp.UserId;
                        emp.Hour = keepEmp.Hour;
                        emp.Minute = keepEmp.Minute;
                        db.KeepEmps.Add(emp);
                        db.SaveChanges();
                        //
                        dtl = db.KeepDtls.Where(d => d.DocId == ss)
                              .FirstOrDefault();
                        if (dtl != null)
                        {
                            int hr = db.KeepEmps.Where(p => p.DocId == ss)
                                .Select(p => p.Hour)
                                .DefaultIfEmpty(0).Sum();
                            decimal min = db.KeepEmps.Where(p => p.DocId == ss)
                                .Select(p => p.Minute)
                                .DefaultIfEmpty(0).Sum();
                            dtl.Hours = hr + Decimal.Round(min / 60m, 2);
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
        // GET: MedEngMgt/RepairEmps/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            KeepEmp keepEmp = db.KeepEmps.Where(p => p.DocId == id).ToList()
                .FirstOrDefault();
            if (keepEmp == null)
            {
                keepEmp = new KeepEmp();
                keepEmp.DocId = id;
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
            KeepFlow kf = db.KeepFlows.Where(f => f.DocId == id)
                         .Where(f => f.Status == "?").ToList().FirstOrDefault();
            if (!kf.Cls.Contains("工程師"))
            {
                List<KeepEmp> emps = db.KeepEmps.Where(p => p.DocId == id).ToList();
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
                keepEmp.UserId = WebSecurity.CurrentUserId;
            }
            return PartialView(keepEmp);
        }

        // POST: MedEngMgt/RepairEmps/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KeepEmp keepEmp)
        {
            if (ModelState.IsValid)
            {
                db.KeepEmps.Add(keepEmp);
                db.SaveChanges();
                //
                KeepDtl dtl = db.KeepDtls.Where(d => d.DocId == keepEmp.DocId).ToList()
                    .FirstOrDefault();
                if (dtl != null)
                {
                    int hr = db.KeepEmps.Where(p => p.DocId == keepEmp.DocId)
                        .Select(p => p.Hour)
                        .DefaultIfEmpty(0).Sum();
                    decimal min = db.KeepEmps.Where(p => p.DocId == keepEmp.DocId)
                        .Select(p => p.Minute)
                        .DefaultIfEmpty(0).Sum();
                    dtl.Hours = hr + Decimal.Round(min / 60m, 2);
                    db.Entry(dtl).State = EntityState.Modified;
                    db.SaveChanges();
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

        // GET: MedEngMgt/RepairEmps/Delete/5
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Delete(string docid, int uid)
        {
            try
            {
                KeepEmp keepEmp = db.KeepEmps.Find(docid, uid);
                db.KeepEmps.Remove(keepEmp);
                db.SaveChanges();
                //
                KeepDtl dtl = db.KeepDtls.Where(d => d.DocId == docid).ToList()
                    .FirstOrDefault();
                if (dtl != null)
                {
                    int hr = db.KeepEmps.Where(p => p.DocId == keepEmp.DocId)
                        .Select(p => p.Hour)
                        .DefaultIfEmpty(0).Sum();
                    decimal min = db.KeepEmps.Where(p => p.DocId == keepEmp.DocId)
                        .Select(p => p.Minute)
                        .DefaultIfEmpty(0).Sum();
                    dtl.Hours = hr + Decimal.Round(min / 60m, 2);
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

        [HttpPost]
        [MyErrorHandler]
        public ActionResult DelUpdCases(string docid, int uid, string docs)
        {
            try
            {
                KeepEmp keepEmp = db.KeepEmps.Find(docid, uid);
                db.KeepEmps.Remove(keepEmp);
                db.SaveChanges();
                //
                KeepDtl dtl = db.KeepDtls.Where(d => d.DocId == docid)
                    .FirstOrDefault();
                if (dtl != null)
                {
                    int hr = db.KeepEmps.Where(p => p.DocId == keepEmp.DocId)
                        .Select(p => p.Hour)
                        .DefaultIfEmpty(0).Sum();
                    decimal min = db.KeepEmps.Where(p => p.DocId == keepEmp.DocId)
                        .Select(p => p.Minute)
                        .DefaultIfEmpty(0).Sum();
                    dtl.Hours = hr + Decimal.Round(min / 60m, 2);
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
