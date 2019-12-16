﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Web.Security;
using Newtonsoft.Json;
using BMEDmgt.Models;
using BMEDmgt.Areas.MedEngMgt.Models;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class objuser
    {
        public int uid;
        public string uname;
        public string gid;
    }
    public class DeliveryController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        //
        // GET: /Delivery/

        public ActionResult Index()
        {
            return View(db.Deliveries.ToList());
        }

        public ActionResult Index2()
        {
            //
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "已處理", Value = "已處理" });
            listItem.Add(new SelectListItem { Text = "已結案", Value = "已結案" });
            ViewData["FLOWTYP"] = new SelectList(listItem, "Value", "Text", "已處理");
            //
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            SelectListItem li;
            db.CustOrgans.ToList()
                .ForEach(d =>
                {
                    li = new SelectListItem();
                    li.Text = d.CustNam;
                    li.Value = d.CustId;
                    listItem2.Add(li);

                });
            ViewData["ACCDPT"] = new SelectList(listItem2, "Value", "Text");
            if (Roles.IsUserInRole("Usual"))
            {
                //listItem2.Clear();
                //AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                //if (u != null)
                //{
                //    li = new SelectListItem();
                //    li.Text = db.CustOrgans.Find(u.CustId).CustNam;
                //    li.Value = u.CustId;
                //    listItem2.Add(li);
                //}
            }
            else if (Roles.IsUserInRole("GroupMgr"))
            {
                //listItem2.Clear();
                //AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                //string gid = db.CustOrgans.Find(u.CustId).GroupId;
                //db.CustOrgans.Where(d => d.GroupId == gid).ToList()
                //    .ForEach(d =>
                //    {
                //        li = new SelectListItem();
                //        li.Text = d.CustNam;
                //        li.Value = d.CustId;
                //        listItem2.Add(li);
                //    });
            }
            ViewData["APPLYDPT"] = new SelectList(listItem2, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult Index2(FormCollection form)
        {
            List<DeliveryListVModel> vm;
            vm = new DeliveryListVModel().GetList2(form["qtyFLOWTYP"]);
            if (form["qtyDOCID"] != "")
            {
                vm = vm.Where(m => m.Docid == form["qtyDOCID"]).ToList();
            }
            if (form["qtyPURCHASENO"] != "")
            {
                vm = vm.Where(m => m.PurchaseNo == form["qtyPURCHASENO"]).ToList();
            }
            if (form["qtyDPTID"] != "")
            {
                vm = vm.Where(m => m.CustId == form["qtyDPTID"]).ToList();
            }
            if (form["qtyBUDGETID"] != "")
            {
                vm = vm.Where(m => m.BudgetId == form["qtyBUDGETID"]).ToList();
            }
            if (form["qtyCONTRACTNO"] != "")
            {
                vm = vm.Where(m => m.ContractNo == form["qtyCONTRACTNO"]).ToList();
            }
            if (form["qtyACCDPT"] != "")
            {
                vm = vm.Where(m => m.AccDpt == form["qtyACCDPT"]).ToList();
            }
            if (form["qtyASSETNO"] != "")
            {
                Asset at = db.Assets.Find(form["qtyASSETNO"]);
                if (at != null)
                {
                    vm = vm.Where(m => m.Docid == at.Docid).ToList();
                }
                else
                    vm.Clear();
            }
            if (form["qtyASSETNAME"] != "")
            {
                string s = form["qtyASSETNAME"];
                List<Asset> at = db.Assets.Where(a => a.Cname.Contains(s))
                    .ToList();
                if (at != null)
                {
                    vm = vm.Join(at, v => v.Docid, a => a.Docid,
                        (v, a) => v).ToList();
                }
                else
                    vm.Clear();
            }
            if (form["qtyVENDOR"] != "")
            {
                string s = form["qtyVENDOR"];
                List<Vendor> vs = db.Vendors.Where(a => a.VendorName.Contains(s))
                    .ToList();
                if (vs != null)
                {
                    vm = vm.Join(vs, v => v.VendorNo, a => a.UniteNo,
                        (v, a) => v).ToList();
                }
                else
                    vm.Clear();
            }
            return PartialView("_DeliveryList", vm);
        }

        public ActionResult DeliveryListIndex()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "待處理", Value = "待處理" });
            listItem.Add(new SelectListItem { Text = "已處理", Value = "已處理" });
            listItem.Add(new SelectListItem { Text = "已結案", Value = "已結案" });
            ViewData["Item"] = new SelectList(listItem, "Value", "Text", "待處理");
            return PartialView("_DeliveryListIndex");
        }
        [HttpPost]
        public ActionResult DeliveryListIndex(FormCollection form)
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "待處理", Value = "待處理" });
            listItem.Add(new SelectListItem { Text = "已處理", Value = "已處理" });
            listItem.Add(new SelectListItem { Text = "已結案", Value = "已結案" });
            ViewData["Item"] = new SelectList(listItem, "Value", "Text", form["qtyFLOWTYP"]);
            //
            List<DeliveryListVModel> vm;
            vm = new DeliveryListVModel().GetList(form["qtyFLOWTYP"]);
            if (form["qtyDOCID"] != "")
            {
                vm = vm.Where(m => m.Docid == form["qtyDOCID"]).ToList();
            }
            if (form["qtyPURCHASENO"] != "")
            {
                vm = vm.Where(m => m.PurchaseNo == form["qtyPURCHASENO"]).ToList();
            }
            if (form["qtyCUSTID"] != "")
            {
                vm = vm.Where(m => m.CustId == form["qtyCUSTID"]).ToList();
            }
            if (form["qtyBUDGETID"] != "")
            {
                vm = vm.Where(m => m.BudgetId == form["qtyBUDGETID"]).ToList();
            }
            if (form["qtyCONTRACTNO"] != "")
            {
                vm = vm.Where(m => m.ContractNo == form["qtyCONTRACTNO"]).ToList();
            }
            if (form["qtyASSETNO"] != "")
            {
                Asset at = db.Assets.Find(form["qtyASSETNO"]);
                if (at != null)
                {
                    vm = vm.Where(m => m.Docid == at.Docid).ToList();
                }
                else
                    vm.Clear();
            }
            return PartialView("_DeliveryList", vm);
        }
        [HttpPost]
        public JsonResult EditData(DelivDataVModel dv)
        {
            if (ModelState.IsValid)
            {
                Delivery d = db.Deliveries.Find(dv.Docid);
                if (d != null)
                {
                    db.Entry(d).State = EntityState.Modified;
                    d.WartyNt = dv.WartyNt;
                    d.AcceptDate = dv.AcceptDate;
                    d.WartySt = dv.WartySt;
                    d.WartyEd = dv.WartyEd;
                    d.FileTestDate = dv.FileTestDate;
                    d.TestUid = dv.TestUid;
                    d.Code = dv.Code;
                    d.Stype2 = dv.Stype2;
                    d.OpenDate = dv.OpenDate;
                    d.OrderDate = dv.OrderDate;
                    db.SaveChanges();
                }
                return Json(new { success = true, msg = "儲存成功!" });
            }
            return Json(new { success = false, msg = "儲存失敗!" });
        }
        public ActionResult WartyData(string id = null)
        {
            DelivDataVModel dv = new DelivDataVModel();
            if (id != null)
            {
                Delivery d = db.Deliveries.Find(id);
                if (d != null)
                {
                    dv.Docid = d.Docid;
                    dv.WartyNt = d.WartyNt;
                    dv.AcceptDate = d.AcceptDate;
                    if (d.WartySt == null)
                        dv.WartySt = DateTime.Now;
                    else
                        dv.WartySt = d.WartySt;
                    if (d.WartyEd == null)
                    {
                        if (d.WartyMon != null)
                            dv.WartyEd = dv.WartySt.Value.AddMonths(d.WartyMon);
                        else
                            dv.WartyEd = DateTime.Now;
                    }
                    else
                        dv.WartyEd = d.WartyEd;
                    if (d.FileTestDate == null)
                        dv.FileTestDate = DateTime.Now;
                    else
                        dv.FileTestDate = d.FileTestDate;
                    dv.Stype2 = d.Stype2;
                    dv.Code = d.Code;
                    dv.TestUid = d.TestUid;
                    dv.OpenDate = d.OpenDate;
                    dv.OrderDate = d.OrderDate;
                    List<Asset> av = db.Assets.Where(a => a.Docid == d.Docid).ToList();
                    List<AssetKeep> kv = new List<AssetKeep>();
                    AssetKeep ak;
                    foreach (Asset a in av)
                    {
                        ak = db.AssetKeeps.Find(a.AssetNo);
                        if (ak != null)
                        {
                            ak.Cname = a.Cname;
                            kv.Add(ak);
                        }
                    }
                    dv.ak = kv;
                }
                List<SelectListItem> listItem = new List<SelectListItem>();
                listItem.Add(new SelectListItem { Text = "自行", Value = "自行" });
                listItem.Add(new SelectListItem { Text = "委外", Value = "委外" });
                listItem.Add(new SelectListItem { Text = "保固", Value = "保固" });
                listItem.Add(new SelectListItem { Text = "租賃", Value = "租賃" });
                ViewData["FMINOUT"] = new SelectList(listItem, "Value", "Text");
                //
                List<SelectListItem> code = new List<SelectListItem>();
                //WsResource.WsVendorSoapClient wsv = new WsResource.WsVendorSoapClient();
                //DataTable dt = JsonConvert.DeserializeObject<DataTable>(wsv.GetNhiPlantKind());
                //foreach (DataRow r in dt.Rows)
                //{
                //    code.Add(new SelectListItem { Text = r["dsc"].ToString(), Value = r["code"].ToString() });
                //}
                ViewData["Code"] = new SelectList(code, "Value", "Text", dv.Code);
                //
                List<SelectListItem> stype2 = new List<SelectListItem>();
                stype2.Add(new SelectListItem { Text = "一般", Value = "N" });
                stype2.Add(new SelectListItem { Text = "須報備", Value = "S" });
                ViewData["Stype2"] = new SelectList(stype2, "Value", "Text", dv.Stype2);
            }
            return PartialView(dv);
        }
        public ActionResult DeliveryList()
        {
            return PartialView("_DeliveryList", new DeliveryListVModel().GetList("待處理"));
        }

        //
        // GET: /Delivery/Details/5

        public ActionResult Details(string id = null)
        {
            Delivery delivery = db.Deliveries.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            CustOrgan c = db.CustOrgans.Find(delivery.AccDpt);
            if (c != null)
                delivery.AccDptNam = c.CustNam;
            Vendor v = db.Vendors.Where(vv => vv.UniteNo == delivery.VendorNo).FirstOrDefault();
            if (v != null)
                delivery.VendorNam = v.VendorName;
            AppUser u = db.AppUsers.Find(Convert.ToInt32(delivery.DelivPson));
            if (u != null)
                delivery.DelivPsonNam = u.FullName;
            return View(delivery);
        }

        //
        // GET: /Delivery/Create

        public ActionResult Create(string id = null)
        {
            Delivery r = new Delivery();
            AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
            //CustOrgan c = db.CustOrgans.Find(u.CustId);
            Vendor v = db.Vendors.Find(u.VendorId);
            if (id != null)
            {
                //BuyEvaluate b = db.BuyEvaluates.Find(id);
                //if (b != null)
                //{
                //    r.EngId = b.EngId;
                //    r.BudgetId = b.BudgetId;
                //    int a = 0;
                //    if (int.TryParse(b.AccDpt, out a))
                //    {
                //        c = db.CustOrgans.Find(b.AccDpt);
                //        if (c != null)
                //        {
                //            r.AccDpt = c.CustId;
                //            r.AccDptNam = c.CustNam;
                //        }
                //    }
                //    else
                //    {
                //        c = db.CustOrgans.Where(d => d.CustNam == b.AccDpt).FirstOrDefault();
                //        if (c != null)
                //        {
                //            r.AccDpt = c.CustId;
                //            r.AccDptNam = c.CustNam;
                //        }
                //    }
                //}
            }
            r.Docid = r.GetID(ref db);
            r.UserId = u.Id;
            r.UserName = u.FullName;
            //c = db.CustOrgans.Find(u.CustId);
            //if (c != null)
            //{
            //    r.Company = c.CustId == null ? "" : c.CustNam;
            //    if (r.AccDpt == null)
            //    {
            //        r.AccDpt = c.CustId == null ? "" : c.CustId;
            //        r.AccDptNam = c.CustId == null ? "" : c.CustNam;
            //    }
            //}
            r.Contact = u.Mobile;
            r.ApplyDate = DateTime.Now;
            r.PurchaseNo = id;
            r.WartyMon = 0;
            r.DelivDateR = DateTime.Now;
            db.Deliveries.Add(r);
            db.SaveChanges();
            List<SelectListItem> listItem = new List<SelectListItem>();
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            List<SelectListItem> listItem3 = new List<SelectListItem>();
            List<SelectListItem> listItem4 = new List<SelectListItem>();
            List<SelectListItem> listItem5 = new List<SelectListItem>();
            string[] eng = Roles.GetUsersInRole("MedEngineer");
            string[] buyer = Roles.GetUsersInRole("Buyer");
            AppUser p;
            foreach (string s in eng)
            {
                p = db.AppUsers.Find(WebSecurity.GetUserId(s));
                if (p.Status == "Y")
                    listItem.Add(new SelectListItem { Text = p.FullName, Value = p.Id.ToString() });
                //if (p.CustId != null)
                //{
                //    if (db.CustOrgans.Find(p.CustId).GroupId == c.GroupId)
                //    {
                //        listItem.Add(new SelectListItem { Text = p.FullName, Value = p.UserId.ToString() });
                //    }
                //}
            }
            ViewData["ENG"] = new SelectList(listItem, "Value", "Text");
            //
            foreach (string s2 in buyer)
            {
                p = db.AppUsers.Find(WebSecurity.GetUserId(s2));
                listItem2.Add(new SelectListItem { Text = p.FullName, Value = p.Id.ToString() });
            }
            ViewData["PUR"] = new SelectList(listItem2, "Value", "Text");
            //
            //List<BuyVendor> bv = db.BuyVendors.Where(t => t.Docid == id).ToList();
            //foreach (BuyVendor b in bv)
            //{
            //    listItem3.Add(new SelectListItem { Text = b.VendorNam, Value = b.UniteNo });
            //}
            List<Vendor> bv = db.Vendors.ToList();
            foreach (Vendor b in bv)
            {
                listItem3.Add(new SelectListItem { Text = b.VendorName, Value = b.UniteNo });
            }
            ViewData["Vendors"] = new SelectList(listItem3, "Value", "Text");
            //
            //List<objuser> uv = db.AppUsers.Join(db.CustOrgans, up => up.CustId, co => co.CustId,
            //    (up, co) => new objuser
            //    {
            //        uid = up.Id,
            //        uname = up.FullName,
            //        gid = co.GroupId
            //    }).Where(co => co.gid == c.GroupId).ToList();
            List<AppUser> uv = db.AppUsers.ToList();

            foreach (AppUser z in uv)
            {
                listItem4.Add(new SelectListItem { Text = z.FullName, Value = z.Id.ToString() });
            }
            ViewData["Users"] = new SelectList(listItem4, "Value", "Text");
            ViewData["Item2"] = new SelectList(listItem5, "Value", "Text");
            return View(r);
        }

        //
        // POST: /Delivery/Create

        [HttpPost]
        public ActionResult Create(Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(delivery).State = EntityState.Modified;
                DelivFlow rf = new DelivFlow();
                rf.Docid = delivery.Docid;
                rf.Stepid = 1;
                rf.Userid = WebSecurity.CurrentUserId;
                rf.Status = "1";
                rf.Role = Roles.GetRolesForUser().GetValue(0).ToString();
                rf.Rtp = WebSecurity.CurrentUserId;
                rf.Rdt = null;
                rf.Rtt = DateTime.Now;
                rf.Cls = "申請者";
                db.DelivFlows.Add(rf);
                //
                rf = new DelivFlow();
                rf.Docid = delivery.Docid;
                rf.Stepid = 2;
                rf.Status = "?";
                AppUser u;
                //CustOrgan c = db.CustOrgans.Find(db.UserProfiles.Find(WebSecurity.CurrentUserId).CustId);
                //foreach (string l in Roles.GetUsersInRole("MedToDo").ToList())
                //{
                //    u = db.UserProfiles.Find(WebSecurity.GetUserId(l));
                //    if (c.GroupId == db.CustOrgans.Find(u.CustId).GroupId)
                //    {
                //        rf.Userid = u.UserId;
                //        rf.Role = Roles.GetRolesForUser(u.UserName).FirstOrDefault();
                //        rf.Rtp = null;
                //        rf.Rdt = null;
                //        rf.Rtt = DateTime.Now;
                //        rf.Cls = "設備經辦";
                //        db.DelivFlows.Add(rf);
                //    }
                //}
                u = db.AppUsers.Find(Convert.ToInt32(delivery.DelivPson));
                rf.Userid = u.Id;
                rf.Role = Roles.GetRolesForUser(u.UserName).FirstOrDefault();
                rf.Rtp = null;
                rf.Rdt = null;
                rf.Rtt = DateTime.Now;
                rf.Cls = "得標廠商";
                db.DelivFlows.Add(rf);
                //
                List<Asset> ar = db.Assets.Where(a => a.Docid == delivery.PurchaseNo).ToList();
                Vendor v;
                u = db.AppUsers.Find(Convert.ToInt32(delivery.UserDpt));
                foreach (Asset a in ar)
                {
                    v = db.Vendors.Where(vv => vv.UniteNo == delivery.VendorNo).FirstOrDefault();
                    if (v != null)
                        a.VendorId = v.VendorId;
                    a.DelivUid = u.Id;
                    a.DelivDpt = u.DptId;
                    db.Entry(a).State = EntityState.Modified;
                }
                //
                db.SaveChanges();
                //----------------------------------------------------------------------------------
                // Mail
                //----------------------------------------------------------------------------------
                //Tmail mail = new Tmail();
                //string body = "";
                //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                //mail.from = new System.Net.Mail.MailAddress(u.Email);
                //u = db.AppUsers.Find(Convert.ToInt32(delivery.DelivPson));
                //mail.to = new System.Net.Mail.MailAddress(u.Email);
                //AppUser up = db.AppUsers.Find(delivery.EngId);
                //if (up != null)
                //{
                //    if (!string.IsNullOrEmpty(up.Email))
                //        mail.cc = new System.Net.Mail.MailAddress(up.Email);
                //}
                ////
                //mail.message.Subject = "醫療儀器管理資訊系統[驗收案]：採購案號： " + delivery.PurchaseNo;
                //body += "<p>申請人：" + delivery.UserName + "</p>";
                //body += "<p>合約號碼：" + delivery.ContractNo + "</p>";
                //body += "<p>採購案號：" + delivery.PurchaseNo + "</p>";
                //body += "<p><a href='http://dms.cch.org.tw/MvcMedEngMgr'>處理案件</a></p>";
                //body += "<br/>";
                //body += "<p>若有任何問題，請與驗收工程師(" + db.UserProfiles.Find(delivery.EngId).FullName + ")聯絡</p>";
                //body += "<h3>this is a inform letter from system manager.Do not reply for it.</h3>";
                //mail.message.Body = body;
                //mail.message.IsBodyHtml = true;
                //mail.SendMail();
                //----------------------------------------------------------------------------------

                return RedirectToAction("Index", "Members");
            }

            return View(delivery);
        }

        //
        // GET: /Delivery/Edit/5

        public ActionResult Edit(string id = null)
        {
            Delivery delivery = db.Deliveries.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            CustOrgan c = db.CustOrgans.Find(delivery.AccDpt);
            if (c != null)
                delivery.AccDptNam = c.CustNam;
            Vendor v = db.Vendors.Where(vv => vv.UniteNo == delivery.VendorNo).FirstOrDefault();
            if (v != null)
                delivery.VendorNam = v.VendorName;
            AppUser u = db.AppUsers.Find(Convert.ToInt32(delivery.DelivPson));
            if (u != null)
                delivery.DelivPsonNam = u.FullName;
            //BuyEvaluate b = db.BuyEvaluates.Find(delivery.PurchaseNo);
            //if (b != null)
                //delivery.BudgetId = b.BudgetId;
            return View(delivery);
        }

        //
        // POST: /Delivery/Edit/5

        [HttpPost]
        public ActionResult Edit(Delivery delivery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(delivery).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(delivery);
        }

        //
        // GET: /Delivery/Delete/5

        public ActionResult Delete(string id = null)
        {
            Delivery delivery = db.Deliveries.Find(id);
            if (delivery == null)
            {
                return HttpNotFound();
            }
            return View(delivery);
        }

        //
        // POST: /Delivery/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            Delivery delivery = db.Deliveries.Find(id);
            db.Deliveries.Remove(delivery);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}