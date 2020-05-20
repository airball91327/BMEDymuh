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
using System.Web.UI.WebControls;
using System.Web.Security;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class KeepFlowsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/RepairFlows
        public ActionResult GetList(string id)
        {
            List<KeepFlow> kf = new List<KeepFlow>();
            db.KeepFlows.Where(f => f.DocId == id)
                .Join(db.AppUsers, f => f.UserId, a => a.Id,
                (f, a) => new
                {
                    DocId = f.DocId,
                    StepId = f.StepId,
                    UserName = a.FullName,
                    Opinions = f.Opinions,
                    Role = f.Role,
                    Status = f.Status,
                    Rtt = f.Rtt,
                    Cls = f.Cls
                }).ToList()
                .ForEach(f =>
                {
                    kf.Add(new KeepFlow
                    {
                        DocId = f.DocId,
                        StepId = f.StepId,
                        UserName = f.UserName,
                        Opinions = f.Opinions,
                        Role = f.Role,
                        Status = f.Status,
                        Rtt = f.Rtt,
                        Cls = f.Cls
                    });
                });
            kf = kf.OrderBy(f => f.StepId).ToList();
            return PartialView(kf);
        }

        // GET: MedEngMgt/RepairFlows/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeepFlow keepFlow = db.KeepFlows.Find(id);
            if (keepFlow == null)
            {
                return HttpNotFound();
            }
            return View(keepFlow);
        }

        // GET: MedEngMgt/RepairFlows/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/RepairFlows/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DocId,StepId,UserId,Opinions,Role,Status,Rtp,Rtt,Rdt,Cls")] KeepFlow keepFlow)
        {
            if (ModelState.IsValid)
            {
                db.KeepFlows.Add(keepFlow);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(keepFlow);
        }

        // GET: MedEngMgt/RepairFlows/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeepFlow keepFlow = db.KeepFlows.Find(id);
            if (keepFlow == null)
            {
                return HttpNotFound();
            }
            return View(keepFlow);
        }

        // POST: MedEngMgt/RepairFlows/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocId,StepId,UserId,Opinions,Role,Status,Rtp,Rtt,Rdt,Cls")] KeepFlow keepFlow)
        {
            if (ModelState.IsValid)
            {
                db.Entry(keepFlow).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(keepFlow);
        }

        // GET: MedEngMgt/RepairFlows/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KeepFlow keepFlow = db.KeepFlows.Find(id);
            if (keepFlow == null)
            {
                return HttpNotFound();
            }
            return View(keepFlow);
        }

        // POST: MedEngMgt/RepairFlows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            KeepFlow keepFlow = db.KeepFlows.Find(id);
            db.KeepFlows.Remove(keepFlow);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UpdCases(string docid)
        {
            Assign assign = new Assign();
            assign.DocId = docid;
            List<SelectListItem> listItem = new List<SelectListItem>();
            //listItem.Add(new SelectListItem { Text = "維修工程師", Value = "維修工程師" });
            listItem.Add(new SelectListItem { Text = "申請人", Value = "申請人" });
            listItem.Add(new SelectListItem { Text = "驗收人", Value = "驗收人" });
            //listItem.Add(new SelectListItem { Text = "醫工經辦", Value = "醫工經辦" });
            listItem.Add(new SelectListItem { Text = "單位主管", Value = "單位主管" });
            listItem.Add(new SelectListItem { Text = "設備工程師", Value = "設備工程師" });
            listItem.Add(new SelectListItem { Text = "設備主管", Value = "設備主管" });
            listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });

            ViewData["FlowCls"] = new SelectList(listItem, "Value", "Text", "");

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Text = "", Value = "" });
            ViewData["FlowUid"] = new SelectList(listItem3, "Value", "Text", "");
            assign.Hint = "";

            return PartialView(assign);
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult UpdCases(Assign assign)
        {
            if (assign.FlowCls == "結案" || assign.FlowCls == "廢除")
                assign.FlowUid = WebSecurity.CurrentUserId;
            if (ModelState.IsValid)
            {
                string[] s = assign.DocId.Split(new char[] { ';' });
                foreach (string ss in s)
                {
                    if (!string.IsNullOrEmpty(ss))
                    {
                        KeepFlow kf = db.KeepFlows.Where(f => f.DocId == ss && f.Status == "?").FirstOrDefault();
                        if (assign.FlowCls == "驗收人" || assign.FlowCls == "設備主管")
                        {
                            if (db.KeepEmps.Where(emp => emp.DocId == ss).Count() <= 0)
                            {
                                throw new Exception("【工程師列表】> 工時紀錄尚未填寫!!");
                            }
                            else if (db.KeepDtls.Find(ss).EndDate == null)
                            {
                                throw new Exception("【保養紀錄】> 沒有【完工日】!!");
                            }
                            if (string.IsNullOrEmpty(db.KeepDtls.Find(ss).Result))
                            {
                                throw new Exception("【保養紀錄】> 【保養結果】不可空白!!");
                            }
                            if (string.IsNullOrEmpty(db.KeepDtls.Find(ss).InOut))
                            {
                                throw new Exception("【保養紀錄】> 【保養方式】不可空白!!");
                            }
                        }
                        if (assign.FlowCls == "結案")
                        {
                            KeepDtl kd = db.KeepDtls.Find(ss);
                            kd.CloseDate = DateTime.Now;
                            kf.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                            kf.Status = "2";
                            kf.Rtt = DateTime.Now;
                            kf.Rtp = WebSecurity.CurrentUserId;
                            db.Entry(kf).State = EntityState.Modified;
                            db.Entry(kd).State = EntityState.Modified;
                            db.SaveChanges();
                            //Send Mail
                            Tmail mail = new Tmail();
                            //string body = "";
                            //AppUser u;
                            //Keep kp = db.Keeps.Find(ss);
                            //KeepDtl dtl = db.KeepDtls.Find(ss);
                            //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                            //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                            //u = db.AppUsers.Find(kf.UserId);
                            //mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                            //db.KeepFlows.Where(f => f.DocId == ss).Select(f => f.UserId).Distinct()
                            //    .Join(db.AppUsers, f => f, a => a.Id,
                            //    (f, a) => a).ToList()
                            //    .ForEach(a =>
                            //    {
                            //        mail.cc.Add(new System.Net.Mail.MailAddress(a.Email));
                            //    });

                            //mail.message.Subject = "醫療儀器管理資訊系統[保養案-結案通知]：儀器名稱： " + kp.AssetName;
                            //body += "<p>表單編號：" + ss + "</p>";
                            //body += "<p>申請人：" + kp.UserName + "</p>";
                            //body += "<p>儀器名稱：" + kp.AssetName + "</p>";
                            //body += "<p>處理結果：" + dtl.Result + "</p>";
                            //body += "<p>備註：" + dtl.Memo + "</p>";
                            //body += "<br/>";
                            ////body += "<p>放置地點：" + repair.PlaceLoc + "</p>";
                            //body += "<p><a href='https://bmed.tmuh.org.tw/bmed'>處理案件</a></p>";
                            //body += "<br/>";
                            //body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                            //mail.message.Body = body;
                            //mail.message.IsBodyHtml = true;
                            //mail.SendMail();
                        }
                        else if (assign.FlowCls == "廢除")
                        {
                            kf.Opinions = "[廢除]" + Environment.NewLine + assign.AssignOpn;
                            kf.Status = "3";
                            kf.Rtt = DateTime.Now;
                            kf.Rtp = WebSecurity.CurrentUserId;
                            db.Entry(kf).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            //轉送下一關卡
                            kf.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                            kf.Status = "1";
                            kf.Rtt = DateTime.Now;
                            kf.Rtp = WebSecurity.CurrentUserId;
                            KeepFlow flow = new KeepFlow();
                            flow.DocId = ss;
                            flow.StepId = kf.StepId + 1;
                            flow.UserId = assign.FlowUid.Value;
                            flow.UserName = db.AppUsers.Find(assign.FlowUid).FullName;
                            flow.Status = "?";
                            flow.Cls = assign.FlowCls;
                            flow.Rtt = DateTime.Now;

                            db.Entry(kf).State = EntityState.Modified;
                            db.KeepFlows.Add(flow);
                            db.SaveChanges();
                            if (assign.FlowCls != "設備工程師")
                            {
                                //Send Mail
                                Tmail mail = new Tmail();
                                //string body = "";
                                //AppUser u;
                                //Keep kp = db.Keeps.Find(ss);
                                //KeepDtl dtl = db.KeepDtls.Find(ss);
                                //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                                //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                                //u = db.AppUsers.Find(flow.UserId);
                                //mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                                //mail.message.Subject = "醫療儀器管理資訊系統[保養案]：儀器名稱： " + kp.AssetName;
                                //body += "<p>表單編號：" + ss + "</p>";
                                //body += "<p>申請人：" + kp.UserName + "</p>";
                                //body += "<p>儀器名稱：" + kp.AssetName + "</p>";
                                //body += "<br/>";
                                ////body += "<p>放置地點：" + repair.PlaceLoc + "</p>";
                                //body += "<p><a href='https://bmed.tmuh.org.tw/bmed'>處理案件</a></p>";
                                //body += "<br/>";
                                //body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                                //mail.message.Body = body;
                                //mail.message.IsBodyHtml = true;
                                //mail.SendMail();
                            }
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

        public ActionResult NextFlow(string id)
        {
            Assign assign = new Assign();
            assign.DocId = id;
            List<SelectListItem> listItem = new List<SelectListItem>();
            //listItem.Add(new SelectListItem { Text = "維修工程師", Value = "維修工程師" });
            listItem.Add(new SelectListItem { Text = "申請人", Value = "申請人" });
            listItem.Add(new SelectListItem { Text = "驗收人", Value = "驗收人" });
            //listItem.Add(new SelectListItem { Text = "醫工經辦", Value = "醫工經辦" });
            listItem.Add(new SelectListItem { Text = "單位主管", Value = "單位主管" });
            listItem.Add(new SelectListItem { Text = "設備工程師", Value = "設備工程師" });
            listItem.Add(new SelectListItem { Text = "設備主管", Value = "設備主管" });
            KeepDtl dtl = db.KeepDtls.Find(id);
            KeepFlow kf = db.KeepFlows.Where(f => f.DocId == id && f.Status == "?").FirstOrDefault();
            if (kf != null)
            {
                if (kf.Cls == "驗收人")
                {
                    if (db.KeepDtls.Find(id).IsCharged == "Y")
                    {
                        if (db.KeepCosts.Where(c => c.DocId == id).Count() > 0)
                        {
                            if (db.KeepCosts.Where(c => c.DocId == id).Sum(c => c.TotalCost) > 0)
                                listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
                        }
                    }
                    else
                    {
                        listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
                    }

                }
                if (kf.Cls == "單位主管")
                {
                    listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
                }
                else if (kf.Cls == "設備工程師" && dtl.InOut != "0") //委外
                {
                    listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
                }
                else if (kf.Cls == "維修工程師")
                {
                    listItem.Clear();
                    listItem.Add(new SelectListItem { Text = "維修工程師", Value = "維修工程師" });
                    listItem.Add(new SelectListItem { Text = "設備工程師", Value = "設備工程師" });
                    listItem.Add(new SelectListItem { Text = "設備主管", Value = "設備主管" });
                    if (dtl.InOut == "0") //自行
                    {
                        listItem.Add(new SelectListItem { Text = "驗收人", Value = "驗收人" });
                    }
                }
            }
            ViewData["FlowCls"] = new SelectList(listItem, "Value", "Text", "");

            //List<SelectListItem> flowvendor = new List<SelectListItem>();
            //db.Vendors.ToList().ForEach(v => {
            //    flowvendor.Add(new SelectListItem
            //    {
            //        Value = v.VendorId.ToString(),
            //        Text = v.VendorName,
            //        Selected = false
            //    });
            //});
            //ViewData["FlowVendor"] = new SelectList(flowvendor, "Value", "Text", "");

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Text = "", Value = "" });
            ViewData["FlowUid"] = new SelectList(listItem3, "Value", "Text", "");
            assign.Hint = "";

            return PartialView(assign);
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult NextFlow(Assign assign)
        {
            if (assign.FlowCls == "結案" || assign.FlowCls == "廢除")
                assign.FlowUid = WebSecurity.CurrentUserId;
            if (ModelState.IsValid)
            {
                KeepFlow kf = db.KeepFlows.Where(f => f.DocId == assign.DocId && f.Status == "?").FirstOrDefault();
                if (assign.FlowCls == "驗收人" || assign.FlowCls == "結案" || assign.FlowCls == "設備主管")
                {
                    if (db.KeepEmps.Where(emp => emp.DocId == assign.DocId).Count() <= 0)
                    {
                        throw new Exception("【工程師列表】> 工時紀錄尚未填寫!!");
                    }
                    else if (db.KeepDtls.Find(assign.DocId).EndDate == null)
                    {
                        throw new Exception("【保養紀錄】> 沒有【完工日】!!");
                    }
                    if (string.IsNullOrEmpty(db.KeepDtls.Find(assign.DocId).Result))
                    {
                        throw new Exception("【保養紀錄】> 【保養結果】不可空白!!");
                    }
                    if (string.IsNullOrEmpty(db.KeepDtls.Find(assign.DocId).InOut))
                    {
                        throw new Exception("【保養紀錄】> 【保養方式】不可空白!!");
                    }
                }
                if (assign.FlowCls == "結案")
                {
                    KeepDtl kd = db.KeepDtls.Find(assign.DocId);
                    kd.CloseDate = DateTime.Now;
                    kf.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                    kf.Status = "2";
                    kf.Rtt = DateTime.Now;
                    kf.Rtp = WebSecurity.CurrentUserId;
                    db.Entry(kf).State = EntityState.Modified;
                    db.Entry(kd).State = EntityState.Modified;
                    db.SaveChanges();
                    //Send Mail
                    Tmail mail = new Tmail();
                    //string body = "";
                    //AppUser u;
                    //Keep kp = db.Keeps.Find(assign.DocId);
                    //KeepDtl dtl = db.KeepDtls.Find(assign.DocId);
                    //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                    //u = db.AppUsers.Find(kf.UserId);
                    //mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                    //db.KeepFlows.Where(f => f.DocId == assign.DocId).Select(f => f.UserId).Distinct()
                    //    .Join(db.AppUsers, f => f, a => a.Id,
                    //    (f, a) => a).ToList()
                    //    .ForEach(a =>
                    //    {
                    //        mail.cc.Add(new System.Net.Mail.MailAddress(a.Email));
                    //    });

                    //mail.message.Subject = "醫療儀器管理資訊系統[保養案-結案通知]：儀器名稱： " + kp.AssetName;
                    //body += "<p>表單編號：" + assign.DocId + "</p>";
                    //body += "<p>申請人：" + kp.UserName + "</p>";
                    //body += "<p>儀器名稱：" + kp.AssetName + "</p>";
                    //body += "<p>處理結果：" + dtl.Result + "</p>";
                    //body += "<p>備註：" + dtl.Memo + "</p>";
                    //body += "<br/>";
                    ////body += "<p>放置地點：" + repair.PlaceLoc + "</p>";
                    //body += "<p><a href='https://mdms.ymuh.ym.edu.tw/'>處理案件</a></p>";
                    //body += "<br/>";
                    //body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                    //mail.message.Body = body;
                    //mail.message.IsBodyHtml = true;
                    //mail.SendMail();
                }
                else if (assign.FlowCls == "廢除")
                {
                    kf.Opinions = "[廢除]" + Environment.NewLine + assign.AssignOpn;
                    kf.Status = "3";
                    kf.Rtt = DateTime.Now;
                    kf.Rtp = WebSecurity.CurrentUserId;
                    db.Entry(kf).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //轉送下一關卡
                    kf.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                    kf.Status = "1";
                    kf.Rtt = DateTime.Now;
                    kf.Rtp = WebSecurity.CurrentUserId;
                    KeepFlow flow = new KeepFlow();
                    flow.DocId = assign.DocId;
                    flow.StepId = kf.StepId + 1;
                    flow.UserId = assign.FlowUid.Value;
                    flow.UserName = db.AppUsers.Find(assign.FlowUid).FullName;
                    flow.Status = "?";
                    flow.Cls = assign.FlowCls;
                    flow.Rtt = DateTime.Now;

                    db.Entry(kf).State = EntityState.Modified;
                    db.KeepFlows.Add(flow);
                    db.SaveChanges();
                    if (assign.FlowCls != "設備工程師")
                    {
                        //Send Mail
                        Tmail mail = new Tmail();
                        //string body = "";
                        //AppUser u;
                        //Keep kp = db.Keeps.Find(assign.DocId);
                        //KeepDtl dtl = db.KeepDtls.Find(assign.DocId);
                        //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                        //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                        //u = db.AppUsers.Find(flow.UserId);
                        //mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                        //mail.message.Subject = "醫療儀器管理資訊系統[保養案]：儀器名稱： " + kp.AssetName;
                        //body += "<p>表單編號：" + assign.DocId + "</p>";
                        //body += "<p>申請人：" + kp.UserName + "</p>";
                        //body += "<p>儀器名稱：" + kp.AssetName + "</p>";
                        //body += "<br/>";
                        ////body += "<p>放置地點：" + repair.PlaceLoc + "</p>";
                        //body += "<p><a href='https://mdms.ymuh.ym.edu.tw/'>處理案件</a></p>";
                        //body += "<br/>";
                        //body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                        //mail.message.Body = body;
                        //mail.message.IsBodyHtml = true;
                        //mail.SendMail();
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

        [MyErrorHandler]
        public JsonResult GetNextEmp(string cls, string docid, string vendor)
        {
            List<ListItem> list = null;
            List<string> s;
            ListItem li;
            AppUser u;
            if (docid.Contains(";"))
            {
                docid = docid.Split(new char[] { ';' }).FirstOrDefault();
            }
            Keep r = db.Keeps.Find(docid);
            Asset asset = db.Assets.Find(r.AssetNo);
            //string c = db.AppUsers.Find(r.UserId).DptId;

            switch (cls)
            {
                case "維修工程師":
                    s = Roles.GetUsersInRole("Engineer").ToList();
                    list = new List<ListItem>();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        if (vendor != null && u != null)
                        {
                            if (u.VendorId != null)
                            {
                                if (u.VendorId.ToString() == vendor)
                                {
                                    li = new ListItem();
                                    li.Text = u.FullName;
                                    li.Value = u.Id.ToString();
                                    list.Add(li);
                                }
                            }
                        }
                        else
                        {
                            if (u != null)
                            {
                                li = new ListItem();
                                li.Text = u.FullName;
                                li.Value = u.Id.ToString();
                                list.Add(li);
                            }
                        }

                    }
                    break;
                case "設備主管":
                    s = Roles.GetUsersInRole("MedMgr").ToList();
                    list = new List<ListItem>();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        li = new ListItem();
                        li.Text = u.FullName;
                        li.Value = WebSecurity.GetUserId(l).ToString();
                        list.Add(li);
                    }
                    break;
                case "醫工經辦":
                    s = Roles.GetUsersInRole("MedToDo").ToList();
                    list = new List<ListItem>();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        li = new ListItem();
                        li.Text = u.FullName;
                        li.Value = WebSecurity.GetUserId(l).ToString();
                        list.Add(li);
                    }
                    break;
                case "單位主管":
                    s = Roles.GetUsersInRole("Manager").ToList();
                    string d = db.AppUsers.Find(r.UserId).DptId;
                    list = new List<ListItem>();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        if (u != null)
                        {
                            if (u.DptId == d)
                            {
                                li = new ListItem();
                                li.Text = u.FullName;
                                li.Value = WebSecurity.GetUserId(l).ToString();
                                list.Add(li);
                            }
                        }
                    }
                    break;
                case "申請人":
                    if (r != null)
                    {
                        list = new List<ListItem>();
                        li = new ListItem();
                        li.Text = r.UserName;
                        li.Value = r.UserId.ToString();
                        list.Add(li);
                    }
                    break;
                case "驗收人":
                    if (db.KeepEmps.Where(emp => emp.DocId == docid).Count() <= 0)
                    {
                        throw new Exception("【工程師列表】> 工時紀錄尚未填寫!!");
                    }
                    else if (db.KeepDtls.Find(docid).EndDate == null)
                    {
                        throw new Exception("【保養紀錄】> 沒有【完工日】!!");
                    }
                    else if (db.KeepDtls.Find(docid).Result == null ||
                        db.KeepDtls.Find(docid).Result == "")
                    {
                        throw new Exception("【保養紀錄】 > 沒有【保養結果】!!");
                    }
                    if (r != null)
                    {
                        u = db.AppUsers.Find(r.UserId);
                        List<AppUser> ul = db.AppUsers.Where(f => f.DptId == u.DptId)
                            .Where(f => f.Status == "Y").ToList();
                        if (asset != null)
                        {
                            ul.AddRange(db.AppUsers.Where(f => f.DptId == asset.DelivDpt)
                                .Where(f => f.Status == "Y").ToList());
                        }
                        list = new List<ListItem>();

                        foreach (AppUser l in ul)
                        {
                            li = new ListItem();
                            li.Text = l.FullName;
                            li.Value = l.Id.ToString();
                            list.Add(li);
                        }
                    }
                    break;
                case "設備工程師":
                    list = new List<ListItem>();
                    s = Roles.GetUsersInRole("MedEngineer").ToList();
                    var lastEngFlow = db.KeepFlows.Where(kf => kf.DocId == docid).Where(kf => kf.Cls.Contains("設備工程師"))
                                                  .Where(kf => kf.Status == "1").OrderByDescending(kf => kf.StepId).FirstOrDefault();
                    int? lastEng = null;
                    if (lastEngFlow != null)
                    {
                        lastEng = lastEngFlow.UserId;
                    }
                    if (lastEng != null)
                    {
                        var eng = db.AppUsers.Find(lastEng);
                        li = new ListItem();
                        li.Text = eng.FullName;
                        li.Value = eng.Id.ToString();
                        list.Add(li);
                        foreach (string l in s)
                        {
                            u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                            if (u != null)
                            {
                                if (u.Id != lastEng)
                                {
                                    li = new ListItem();
                                    li.Text = u.FullName;
                                    li.Value = WebSecurity.GetUserId(l).ToString();
                                    list.Add(li);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string l in s)
                        {
                            u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                            if (u != null)
                            {
                                li = new ListItem();
                                li.Text = u.FullName;
                                li.Value = WebSecurity.GetUserId(l).ToString();
                                list.Add(li);
                            }
                        }
                    }
                    break;
                default:
                    list = new List<ListItem>();
                    break;
            }
            return Json(list);
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
