using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using System.Web.Security;
using BMEDmgt.Models;
using WebMatrix.WebData;
using BMEDmgt.Filters;
using BMEDmgt.Areas.MedEngMgt.Models;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class DelivFlowController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        //
        // GET: /DelivFlow/

        public ActionResult Index()
        {
            return View(db.DelivFlows.ToList());
        }

        public ActionResult GetList(string id = null)
        {
            List<DelivFlow> rf = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DelivFlow WHERE DOCID = @id",
                new SqlParameter("id", id)).ToList();
            AppUser p;
            foreach (DelivFlow f in rf)
            {
                p = db.AppUsers.Find(f.Userid);
                f.UserNam = p.UserName + "(" + p.FullName + ")";
                if (f.Status == "?")
                    ViewData["cls_now"] = f.Cls;
            }
            return PartialView(rf);
        }

        //
        // GET: /DelivFlow/Details/5

        public ActionResult NextFlow(string id = null)
        {
            DelivFlow rf = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DelivFlow WHERE DOCID = @id "
                + "AND STATUS = '?' ",
                new SqlParameter("id", id)).FirstOrDefault();
            rf.Docid = id;
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "申請者", Value = "申請者" });
            //listItem.Add(new SelectListItem { Text = "採購人員", Value = "採購人員" });
            //listItem.Add(new SelectListItem { Text = "單位主管", Value = "單位主管" });
            listItem.Add(new SelectListItem { Text = "設備工程師", Value = "設備工程師" });
            listItem.Add(new SelectListItem { Text = "設備主管", Value = "設備主管" });
            //listItem.Add(new SelectListItem { Text = "得標廠商", Value = "得標廠商" });
            //listItem.Add(new SelectListItem { Text = "使用單位", Value = "使用單位" });
            listItem.Add(new SelectListItem { Text = "設備經辦", Value = "設備經辦" });
            //listItem.Add(new SelectListItem { Text = "採購主管", Value = "採購主管" });
            if (rf.Cls == "設備主管")
                listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
            ViewData["Item"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "", Value = "" });
            ViewData["Item2"] = new SelectList(listItem2, "Value", "Text", "");
            rf.Cls = "";
            //
            Delivery ra = db.Deliveries.Find(id);
            //if (ra != null)
            //{
            //    string cid = db.AppUsers.Find(ra.UserId).CustId;
            //    string gid = db.CustOrgans.Find(cid).GroupId;
            //    if (gid != null)
            //    {
            //        rf.FlowHint = db.Groups.Find(gid).FlowHint4;
            //    }
            //}
            rf.SelOpin = "同意";

            return PartialView(rf);
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult NextFlow(DelivFlow DelivFlow)
        {
            if (ModelState.IsValid)
            {
                if (DelivFlow.SelOpin == "其他" && string.IsNullOrEmpty(DelivFlow.Opinions))
                {
                    throw new Exception("請填寫意見欄!!");
                }
                DelivFlow rf = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DelivFlow WHERE DOCID = @id "
                + "AND STATUS = '?' ",
                new SqlParameter("id", DelivFlow.Docid)).FirstOrDefault();
                if (rf.Cls == "得標廠商")
                {
                    if (db.Assets.Where(a => a.Docid == DelivFlow.Docid).Count() <= 0)
                    {
                        throw new Exception("沒有設備紀錄! 請新增設備!");
                    }
                }
                List<DelivFlow> rflist = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DelivFlow WHERE DOCID = @id ",
                new SqlParameter("id", DelivFlow.Docid)).ToList();
                Delivery r = db.Deliveries.Find(DelivFlow.Docid);
                AppUser u;
                Tmail mail;
                string body = "";
                if (DelivFlow.Cls == "結案")
                {
                    string sto = "";
                    //KeepDtl rd = db.KeepDtls.Find(KeepFlow.Docid);
                    rf.Opinions = DelivFlow.SelOpin + Environment.NewLine + DelivFlow.Opinions;
                    rf.Status = "2";
                    rf.Rtt = DateTime.Now;
                    rf.Rtp = WebSecurity.CurrentUserId;
                    db.Entry(rf).State = EntityState.Modified;
                    db.SaveChanges();
                    //
                    // Send Mail
                    //mail = new Tmail();
                    //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                    //foreach (DelivFlow rr in rflist)
                    //{
                    //    u = db.AppUsers.Find(rr.Userid);
                    //    sto += u.Email + ",";
                    //}
                    //mail.sto = sto.TrimEnd(new char[] { ',' });
                    ////mail.cc = new System.Net.Mail.MailAddress("99242@cch.org.tw");
                    //mail.message.Subject = "醫療儀器管理資訊系統[驗收案-結案通知]：採購案號： " + r.PurchaseNo;
                    //body += "<p>申請人：" + r.UserName + "</p>";
                    //body += "<p>合約號碼：" + r.ContractNo + "</p>";
                    //body += "<p>採購案號：" + r.PurchaseNo + "</p>";
                    //body += "<p><a href='http://dms.cch.org.tw/MvcMedEngMgr'>前往網站檢視</a></p>";
                    //body += "<br/>";
                    //body += "<h3>this is a inform letter from system manager.Do not reply for it.</h3>";
                    //mail.message.Body = body;
                    //mail.message.IsBodyHtml = true;
                    //mail.SendMail();
                    return new JsonResult
                    {
                        Data = new { success = true, error = "" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                DelivFlow.Stepid = rf.Stepid + 1;
                DelivFlow.Rtt = DateTime.Now;
                switch (DelivFlow.Cls)
                {
                    case "申請者":
                        DelivFlow.Userid = r.UserId;
                        break;
                }
                DelivFlow.Status = "?";
                u = db.AppUsers.Find(DelivFlow.Userid);
                DelivFlow.Role = Roles.GetRolesForUser(u.UserName).FirstOrDefault();
                rf.Opinions = DelivFlow.SelOpin + Environment.NewLine + DelivFlow.Opinions;
                rf.Status = "1";
                rf.Rtp = WebSecurity.CurrentUserId;
                DelivFlow.Opinions = null;
                db.Entry(rf).State = EntityState.Modified;
                db.DelivFlows.Add(DelivFlow);
                db.SaveChanges();
                //
                //Send Mail
                //mail = new Tmail();
                //body = "";
                //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                //u = db.AppUsers.Find(DelivFlow.Userid);
                //mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                ////mail.cc = new System.Net.Mail.MailAddress("99242@cch.org.tw");
                //mail.message.Subject = "醫療儀器管理資訊系統[驗收案]：採購案號： " + r.PurchaseNo;
                //body += "<p>申請人：" + r.UserName + "</p>";
                //body += "<p>合約號碼：" + r.ContractNo + "</p>";
                //body += "<p>採購案號：" + r.PurchaseNo + "</p>";
                //body += "<p><a href='http://dms.cch.org.tw/MvcMedEngMgr'>處理案件</a></p>";
                //body += "<br/>";
                //body += "<p>若有任何問題，請與驗收工程師(" + db.UserProfiles.Find(r.EngId).FullName + ")聯絡</p>";
                //body += "<h3>this is a inform letter from system manager.Do not reply for it.</h3>";
                //mail.message.Body = body;
                //mail.message.IsBodyHtml = true;
                //mail.SendMail();
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

        //
        // GET: /DelivFlow/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DelivFlow/Create

        [HttpPost]
        public ActionResult Create(DelivFlow delivflow)
        {
            if (ModelState.IsValid)
            {
                db.DelivFlows.Add(delivflow);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(delivflow);
        }

        //
        // GET: /DelivFlow/Edit/5

        public ActionResult Edit(string id = null)
        {
            DelivFlow delivflow = db.DelivFlows.Find(id);
            if (delivflow == null)
            {
                return HttpNotFound();
            }
            return View(delivflow);
        }

        //
        // POST: /DelivFlow/Edit/5

        [HttpPost]
        public ActionResult Edit(DelivFlow delivflow)
        {
            if (ModelState.IsValid)
            {
                db.Entry(delivflow).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(delivflow);
        }

        //
        // GET: /DelivFlow/Delete/5

        public ActionResult Delete(string id = null)
        {
            DelivFlow delivflow = db.DelivFlows.Find(id);
            if (delivflow == null)
            {
                return HttpNotFound();
            }
            return View(delivflow);
        }

        //
        // POST: /DelivFlow/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            DelivFlow delivflow = db.DelivFlows.Find(id);
            db.DelivFlows.Remove(delivflow);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetNextEmp(string cls = null, string docid = null, string vendor = null)
        {
            List<ListItem> list = new List<ListItem>();
            List<string> s;
            ListItem li;
            Delivery r = db.Deliveries.Find(docid);
            //string c = db.UserProfiles.Find(r.UserId).CustId;
            string c = db.AppUsers.Find(WebSecurity.CurrentUserId).DptId;
            //string g = db.CustOrgans.Find(c).GroupId;
            AppUser u;
            List<AppUser> uv;
            switch (cls)
            {
                case "設備工程師":
                    s = Roles.GetUsersInRole("MedEngineer").ToList();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        //if (db.CustOrgans.Find(u.CustId).GroupId == g)
                        //{
                        //    li = new ListItem();
                        //    li.Text = u.FullName;
                        //    li.Value = WebSecurity.GetUserId(l).ToString();
                        //    list.Add(li);
                        //}
                        if (u != null)
                        {
                            li = new ListItem();
                            li.Text = u.FullName;
                            li.Value = WebSecurity.GetUserId(l).ToString();
                            if (li.Value == Convert.ToString(r.EngId))
                                list.Insert(0, li);
                            else
                                list.Add(li);
                        }
                    }
                    u = db.AppUsers.Find(r.EngId);
                    if (u != null)
                    {
                        li = new ListItem();
                        li.Text = u.FullName;
                        li.Value = u.Id.ToString();
                        list.Add(li);
                    }
                    break;
                case "設備主管":
                    s = Roles.GetUsersInRole("MedMgr").ToList();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        if (u != null)
                        {
                            if (u.Status == "Y")
                            {
                                li = new ListItem();
                                li.Text = u.FullName;
                                li.Value = WebSecurity.GetUserId(l).ToString();
                                list.Add(li);
                            }
                        }
                    }
                    break;
                case "單位主管":
                    s = Roles.GetUsersInRole("Manager").ToList();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        //CustOrgan o = db.CustOrgans.Find(u.CustId);
                        //if (g != null)
                        //{
                        //    if (o.GroupId == g)
                        //    {
                        //        li = new ListItem();
                        //        li.Text = u.FullName;
                        //        li.Value = WebSecurity.GetUserId(l).ToString();
                        //        list.Add(li);
                        //    }
                        //}
                        //else
                        //{
                        //    if (u.CustId == c)
                        //    {
                        //        li = new ListItem();
                        //        li.Text = u.FullName;
                        //        li.Value = WebSecurity.GetUserId(l).ToString();
                        //        list.Add(li);
                        //    }
                        //}
                        li = new ListItem();
                        li.Text = u.FullName;
                        li.Value = WebSecurity.GetUserId(l).ToString();
                        list.Add(li);
                    }
                    break;
                case "申請者":
                    if (r != null)
                    {
                        li = new ListItem();
                        li.Text = r.UserName;
                        li.Value = r.UserId.ToString();
                        list.Add(li);
                    }
                    break;
                case "採購人員":
                    if (r != null)
                    {

                        li = new ListItem();
                        li.Text = db.AppUsers.Find(r.PurchaserId).FullName;
                        li.Value = r.PurchaserId.ToString();
                        list.Add(li);
                    }
                    break;
                case "得標廠商":
                    int vendorId = Convert.ToInt32(r.VendorId);
                    List<Vendor> vv = db.Vendors.Where(v => v.VendorId == vendorId).ToList();
                    foreach (Vendor v in vv)
                    {
                        uv = db.AppUsers.Where(u2 => u2.VendorId == v.VendorId).ToList();
                        foreach (AppUser u3 in uv)
                        {
                            li = new ListItem();
                            li.Text = u3.FullName;
                            li.Value = u3.Id.ToString();
                            list.Add(li);
                        }
                    }
                    /* for test */
                    if (list.Count() <= 0)
                    {
                        li = new ListItem();
                        li.Text = r.UserName;
                        li.Value = r.UserId.ToString();
                        list.Add(li);
                    }
                    break;
                case "使用單位":
                    DelivFlow df = db.DelivFlows.Where(d => d.Docid == r.Docid).Where(d => d.Cls == "使用單位").FirstOrDefault();
                    if (df != null)
                    {
                        u = db.AppUsers.Find(df.Userid);
                        if (u != null)
                        {
                            li = new ListItem();
                            li.Text = u.FullName;
                            li.Value = u.Id.ToString();
                            list.Add(li);
                        }
                    }
                    break;
                case "設備經辦":
                    s = Roles.GetUsersInRole("MedToDo").ToList();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        //CustOrgan o = db.CustOrgans.Find(u.CustId);
                        //if (g != null)
                        //{
                        //    if (o.GroupId == g)
                        //    {
                        //        li = new ListItem();
                        //        li.Text = u.FullName;
                        //        li.Value = WebSecurity.GetUserId(l).ToString();
                        //        list.Add(li);
                        //    }
                        //}
                        //else
                        //{
                        //    if (u.CustId == c)
                        //    {
                        //        li = new ListItem();
                        //        li.Text = u.FullName;
                        //        li.Value = WebSecurity.GetUserId(l).ToString();
                        //        list.Add(li);
                        //    }
                        //}
                        li = new ListItem();
                        li.Text = u.FullName;
                        li.Value = WebSecurity.GetUserId(l).ToString();
                        list.Add(li);
                    }
                    break;
                case "採購主管":
                    s = Roles.GetUsersInRole("BuyerMgr").ToList();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        //CustOrgan o = db.CustOrgans.Find(u.CustId);
                        //if (g != null)
                        //{
                        //    if (o.GroupId == g)
                        //    {
                        //        li = new ListItem();
                        //        li.Text = u.FullName;
                        //        li.Value = WebSecurity.GetUserId(l).ToString();
                        //        list.Add(li);
                        //    }
                        //}
                        //else
                        //{
                        //    if (u.CustId == c)
                        //    {
                        //        li = new ListItem();
                        //        li.Text = u.FullName;
                        //        li.Value = WebSecurity.GetUserId(l).ToString();
                        //        list.Add(li);
                        //    }
                        //}
                        li = new ListItem();
                        li.Text = u.FullName;
                        li.Value = WebSecurity.GetUserId(l).ToString();
                        list.Add(li);
                    }
                    break;
                default:
                    break;
            }
            return Json(list);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}