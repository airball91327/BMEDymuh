﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Extensions;
using BMEDmgt.Models;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class EngSubStaffsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/EngSubStaffs
        public ActionResult Index()
        {
            if (User.IsInRole("Admin") == true)
            {
                // Save log. 
                string logClass = "系統管理者紀錄";
                string logAction = "設定代理人";
                var result = new SystemLogsController().SaveLog(logClass, logAction);
            }
            var user = db.AppUsers.Find(WebSecurity.CurrentUserId);

            var engSubStaffs = db.EngSubStaffs.Include(e => e.EngAppUsers).Include(e => e.SubAppUsers)
                                              .Where(e => e.EngId == user.Id);
            return View(engSubStaffs.ToList());
        }

        // GET: MedEngMgt/EngSubStaffs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var engSubStaff = db.EngSubStaffs.Include(e => e.EngAppUsers)
                                             .Include(e => e.SubAppUsers)
                                             .SingleOrDefaultAsync(m => m.EngId == id);
            if (engSubStaff == null)
            {
                return HttpNotFound();
            }
            return View(engSubStaff);
        }

        // GET: MedEngMgt/EngSubStaffs/Create
        public ActionResult Create()
        {
            var user = db.AppUsers.Find(WebSecurity.CurrentUserId);
            /* Get all engineers by role. */
            var allEngs = Roles.GetUsersInRole("MedEngineer").ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem li = new SelectListItem();
            foreach (string l in allEngs)
            {
                var u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (u != null)
                {
                    li = new SelectListItem();
                    li.Text = u.FullName;
                    li.Value = WebSecurity.GetUserId(l).ToString();
                    list.Add(li);
                }
            }

            ViewData["SubstituteId"] = new SelectList(list, "Value", "Text");

            EngSubStaff engSubStaff = new EngSubStaff();
            engSubStaff.EngId = user.Id;
            engSubStaff.StartDate = DateTime.Now;
            engSubStaff.EndDate = DateTime.Now;
            return View(engSubStaff);
        }

        // POST: MedEngMgt/EngSubStaffs/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EngId,SubstituteId,StartDate,EndDate")] EngSubStaff engSubStaff)
        {
            if (ModelState.IsValid)
            {
                db.EngSubStaffs.Add(engSubStaff);
                db.SaveChanges();
                // Save log. 
                var subUser = db.AppUsers.Find(engSubStaff.SubstituteId);
                string logClass = "管理紀錄";
                string logAction = "設定代理人 > 新增 > ";
                if (subUser != null)
                {
                    logAction += subUser.UserName + "(" + subUser.FullName + ")";
                }
                var result = new SystemLogsController().SaveLog(logClass, logAction);

                return RedirectToAction("Index");
            }

            var user = db.AppUsers.Find(WebSecurity.CurrentUserId);
            /* Get all engineers by role. */
            var allEngs = Roles.GetUsersInRole("MedEngineer").ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem li = new SelectListItem();
            foreach (string l in allEngs)
            {
                var u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (u != null)
                {
                    li = new SelectListItem();
                    li.Text = u.FullName;
                    li.Value = WebSecurity.GetUserId(l).ToString();
                    list.Add(li);
                }
            }

            ViewData["EngId"] = user.Id;
            ViewData["SubstituteId"] = new SelectList(list, "Value", "Text");
            return View(engSubStaff);
        }

        // GET: MedEngMgt/EngSubStaffs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EngSubStaff engSubStaff = db.EngSubStaffs.Find(id);
            if (engSubStaff == null)
            {
                return HttpNotFound();
            }
            /* Get all engineers by role. */
            var allEngs = Roles.GetUsersInRole("MedEngineer").ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem li = new SelectListItem();
            foreach (string l in allEngs)
            {
                var u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (u != null)
                {
                    li = new SelectListItem();
                    li.Text = u.FullName;
                    li.Value = WebSecurity.GetUserId(l).ToString();
                    list.Add(li);
                }
            }
            ViewData["SubstituteId"] = new SelectList(list, "Value", "Text", engSubStaff.SubstituteId);
            return View(engSubStaff);
        }

        // POST: MedEngMgt/EngSubStaffs/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "EngId,SubstituteId,StartDate,EndDate")] EngSubStaff engSubStaff)
        {
            if (id != engSubStaff.EngId)
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                var oriObj = db.EngSubStaffs.Find(engSubStaff.EngId);
                db.Entry(oriObj).State = EntityState.Detached;
                //
                db.Entry(engSubStaff).State = EntityState.Modified;
                db.SaveChanges();
                // Save log. 
                var currentObj = db.EngSubStaffs.Find(engSubStaff.EngId);
                var logAction2 = oriObj.EnumeratePropertyDifferences<EngSubStaff>(currentObj);
                var subUser = db.AppUsers.Find(engSubStaff.SubstituteId);
                string logClass = "管理紀錄";
                string logAction = "設定代理人 > 編輯 > ";
                if (subUser != null)
                {
                    logAction += subUser.UserName + "(" + subUser.FullName + ")";
                }
                var result = new SystemLogsController().SaveLog(logClass, logAction, logAction2);

                return RedirectToAction("Index");
            }
            /* Get all engineers by role. */
            var allEngs = Roles.GetUsersInRole("MedEngineer").ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem li = new SelectListItem();
            foreach (string l in allEngs)
            {
                var u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (u != null)
                {
                    li = new SelectListItem();
                    li.Text = u.FullName;
                    li.Value = WebSecurity.GetUserId(l).ToString();
                    list.Add(li);
                }
            }
            ViewData["SubstituteId"] = new SelectList(list, "Value", "Text", engSubStaff.SubstituteId);
            return View(engSubStaff);
        }

        // GET: MedEngMgt/EngSubStaffs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EngSubStaff engSubStaff = db.EngSubStaffs.Find(id);
            if (engSubStaff == null)
            {
                return HttpNotFound();
            }
            return View(engSubStaff);
        }

        // POST: MedEngMgt/EngSubStaffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EngSubStaff engSubStaff = db.EngSubStaffs.Find(id);
            db.EngSubStaffs.Remove(engSubStaff);
            db.SaveChanges();
            // Save log. 
            var subUser = db.AppUsers.Find(engSubStaff.SubstituteId);
            string logClass = "管理紀錄";
            string logAction = "設定代理人 > 刪除 > ";
            if (subUser != null)
            {
                logAction += subUser.UserName + "(" + subUser.FullName + ")";
            }
            var result = new SystemLogsController().SaveLog(logClass, logAction);

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
