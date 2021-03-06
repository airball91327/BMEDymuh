﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Extensions;
using BMEDmgt.Models;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class VendorsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/Vendors
        public ActionResult Index()
        {

            if (User.IsInRole("Admin") == true)
            {
                // Save log. 
                string logClass = "系統管理者紀錄";
                string logAction = "廠商管理";
                var result = new SystemLogsController().SaveLog(logClass, logAction);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection fm)
        {
            string qname = fm["qtyNAME"];
            string uno = fm["qtyUNITENO"];
            List<Vendor> vt = db.Vendors.Where(v => v.Status == "Y").ToList();
            if (!string.IsNullOrEmpty(qname))
            {
                vt = vt.Where(v => v.VendorName.Contains(qname)).ToList();
            }
            if (!string.IsNullOrEmpty(uno))
            {
                vt = vt.Where(v => v.UniteNo == uno).ToList();
            }

            return PartialView("List", vt);
        }
        public ActionResult List()
        {
            return PartialView(db.Vendors.ToList());
        }

        // GET: MedEngMgt/Vendors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            if (vendor.MgrId != null)
            {
                int mgrid = Convert.ToInt32(vendor.MgrId);
                var ur = db.AppUsers.Where(u => u.Id == mgrid).ToList().FirstOrDefault();
                if (ur != null)
                {
                    vendor.MgrName = ur.FullName;
                }
            }
            return View(vendor);
        }

        // GET: MedEngMgt/Vendors/Details2/5
        public ActionResult Details2(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        // GET: MedEngMgt/Vendors/Create
        public ActionResult Create()
        {
            List<VendorType> vt = db.VendorTypes.ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (VendorType t in vt)
            {
                listItem.Add(new SelectListItem { Text = t.TypName, Value = t.TypName });
            }
            ViewData["TypeItem"] = new SelectList(listItem, "Value", "Text", "");
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "請選擇", Value = "" });
            ViewData["MgrId"] = new SelectList(listItem2, "Value", "Text", "");
            return View();
        }

        // POST: MedEngMgt/Vendors/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VendorId,VendorName,Address,Tel,Fax,Email,UniteNo,TaxAddress,TaxZipCode,Contact,ContactTel,ContactEmail,StartDate,EndDate,Status,Kind,MgrId")] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                db.Vendors.Add(vendor);
                db.SaveChanges();
                // Save log. 
                string logClass = "管理紀錄";
                string logAction = "廠商維護 > 新增 > " + vendor.VendorName + "(" + vendor.UniteNo + ")";
                var result = new SystemLogsController().SaveLog(logClass, logAction);

                return RedirectToAction("Index");
            }
            List<VendorType> vt = db.VendorTypes.ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (VendorType t in vt)
            {
                listItem.Add(new SelectListItem { Text = t.TypName, Value = t.TypName });
            }
            ViewData["TypeItem"] = new SelectList(listItem, "Value", "Text", "");
            return View(vendor);
        }

        // GET: MedEngMgt/Vendors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            List<VendorType> vt = db.VendorTypes.ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (VendorType t in vt)
            {
                listItem.Add(new SelectListItem { Text = t.TypName, Value = t.TypName });
            }
            ViewData["TypeItem"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<SelectListItem> ListItem2 = new List<SelectListItem>();
            AppUser ur;
            ListItem2.Add(new SelectListItem { Text = "請選擇", Value = "" });
            if (vendor.MgrId != null)
            {
                int mgrid = Convert.ToInt32(vendor.MgrId);
                ur = db.AppUsers.Where(u => u.Id == mgrid).ToList().FirstOrDefault();
                if (ur != null)
                {
                    ListItem2.Add(new SelectListItem { Text = ur.FullName, Value = ur.Id.ToString() });
                }
            }
            ViewData["MgrId"] = new SelectList(ListItem2, "Value", "Text", vendor.MgrId);

            return View(vendor);
        }

        // POST: MedEngMgt/Vendors/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VendorId,VendorName,Address,Tel,Fax,Email,UniteNo,TaxAddress,TaxZipCode,Contact,ContactTel,ContactEmail,StartDate,EndDate,Status,Kind,MgrId")] Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                var oriObj = db.Vendors.Find(vendor.VendorId);
                db.Entry(oriObj).State = EntityState.Detached;
                //
                db.Entry(vendor).State = EntityState.Modified;
                db.SaveChanges();
                // Save log.
                var currentObj = db.Vendors.Find(vendor.VendorId);
                var logAction2 = oriObj.EnumeratePropertyDifferences<Vendor>(currentObj);
                string logClass = "醫療儀器紀錄";
                string logAction = "廠商維護 > 編輯 > " + vendor.VendorName + "(" + vendor.UniteNo + ")";
                var result = new SystemLogsController().SaveLog(logClass, logAction, logAction2);

                return RedirectToAction("Index");
            }
            List<VendorType> vt = db.VendorTypes.ToList();
            List<SelectListItem> listItem = new List<SelectListItem>();
            foreach (VendorType t in vt)
            {
                listItem.Add(new SelectListItem { Text = t.TypName, Value = t.TypName });
            }
            ViewData["TypeItem"] = new SelectList(listItem, "Value", "Text", "");
            return View(vendor);
        }

        // GET: MedEngMgt/Vendors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendor vendor = db.Vendors.Find(id);
            if (vendor == null)
            {
                return HttpNotFound();
            }
            return View(vendor);
        }

        // POST: MedEngMgt/Vendors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vendor vendor = db.Vendors.Find(id);
            db.Vendors.Remove(vendor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult QryVendor()
        {
            QryVendor qv = new QryVendor();
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() {
                Text = "",
                Value = "",
            });
            qv.VendorList = items;
            return PartialView(qv);
        }

        [HttpPost]
        public ActionResult QryVendor(QryVendor qv)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (qv.QryType == "關鍵字")
            {
                db.Vendors.Where(v => v.Status != "N")
                          .Where(v => v.VendorName.Contains(qv.KeyWord))
                    .ToList()
                    .ForEach(v => {
                        items.Add(new SelectListItem()
                        {
                            Text = v.VendorName,
                            Value = v.VendorId.ToString()
                        });
                    });
            }
            else if (qv.QryType == "統一編號")
            {
                db.Vendors.Where(v => v.Status != "N")
                          .Where(v => v.UniteNo == qv.UniteNo)
                    .ToList()
                    .ForEach(v => {
                        items.Add(new SelectListItem()
                        {
                            Text = v.VendorName,
                            Value = v.VendorId.ToString()
                        });
                    });
            }
            
            qv.VendorList = items;

            return PartialView("QryVendor2", qv);
        }

        public JsonResult GetVendorUnite(int id)
        {
            var vendor = db.Vendors.Find(id);
            if (vendor != null)
            {
                return Json(vendor.UniteNo, JsonRequestBehavior.AllowGet);
            }
            return Json("查無資料", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVendorDetails(int id)
        {
            var vendor = db.Vendors.Find(id);
            if (vendor != null)
            {
                return Json(vendor, JsonRequestBehavior.AllowGet);
            }
            return Json("查無資料", JsonRequestBehavior.AllowGet);
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
