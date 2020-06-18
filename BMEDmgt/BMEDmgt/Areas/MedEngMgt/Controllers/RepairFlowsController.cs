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
    public class RepairFlowsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        // GET: MedEngMgt/RepairFlows
        public ActionResult GetList(string id)
        {
            List<RepairFlow> rf = new List<Models.RepairFlow>();
            db.RepairFlows.Where(f => f.DocId == id)
                .Join(db.AppUsers, f => f.UserId, a => a.Id,
                (f, a) => new
                {
                    DocId = f.DocId,
                    StepId = f.StepId,
                    UserName = "(" + a.UserName + ")" + a.FullName,
                    Opinions = f.Opinions,
                    Role = f.Role,
                    Status = f.Status,
                    Rtt = f.Rtt,
                    Rtp = f.Rtp,
                    Cls = f.Cls
                }).ToList()
                .ForEach(f =>
                {
                    var rtpUser = db.AppUsers.Where(u => u.Id == f.Rtp).ToList().FirstOrDefault();
                    rf.Add(new RepairFlow
                    {
                        DocId = f.DocId,
                        StepId = f.StepId,
                        UserName = f.UserName,
                        Opinions = f.Opinions,
                        Role = f.Role,
                        Status = f.Status,
                        Rtt = f.Rtt,
                        RtpName = rtpUser == null ? "" : "(" + rtpUser.UserName + ")" + rtpUser.FullName,
                        Cls = f.Cls
                    });
                });
            rf = rf.OrderBy(f => f.StepId).ToList();
            return PartialView(rf);
        }

        // GET: MedEngMgt/RepairFlows/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairFlow repairFlow = db.RepairFlows.Find(id);
            if (repairFlow == null)
            {
                return HttpNotFound();
            }
            return View(repairFlow);
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
        public ActionResult Create([Bind(Include = "DocId,StepId,UserId,Opinions,Role,Status,Rtp,Rtt,Rdt,Cls")] RepairFlow repairFlow)
        {
            if (ModelState.IsValid)
            {
                db.RepairFlows.Add(repairFlow);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(repairFlow);
        }

        // GET: MedEngMgt/RepairFlows/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairFlow repairFlow = db.RepairFlows.Find(id);
            if (repairFlow == null)
            {
                return HttpNotFound();
            }
            return View(repairFlow);
        }

        // POST: MedEngMgt/RepairFlows/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocId,StepId,UserId,Opinions,Role,Status,Rtp,Rtt,Rdt,Cls")] RepairFlow repairFlow)
        {
            if (ModelState.IsValid)
            {
                db.Entry(repairFlow).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(repairFlow);
        }

        // GET: MedEngMgt/RepairFlows/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RepairFlow repairFlow = db.RepairFlows.Find(id);
            if (repairFlow == null)
            {
                return HttpNotFound();
            }
            return View(repairFlow);
        }

        // POST: MedEngMgt/RepairFlows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            RepairFlow repairFlow = db.RepairFlows.Find(id);
            db.RepairFlows.Remove(repairFlow);
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
                        RepairFlow rf = db.RepairFlows.Where(f => f.DocId == ss && f.Status == "?").ToList().FirstOrDefault();
                        if (assign.FlowCls == "驗收人")
                        {
                            if (db.RepairEmps.Where(emp => emp.DocId == ss).Count() <= 0)
                            {
                                throw new Exception("【案件" + ss + "】【工程師列表】> 工時紀錄尚未填寫!!");
                            }
                            else if (db.RepairDtls.Find(ss).EndDate == null)
                            {
                                throw new Exception("【案件" + ss + "】【請修紀錄】> 沒有【完工日】!!");
                            }
                            if (string.IsNullOrEmpty(db.RepairDtls.Find(ss).InOut))
                            {
                                throw new Exception("【案件" + ss + "】【請修紀錄】> 【維修方式】不可空白!!");
                            }
                        }
                        if (assign.FlowCls == "結案")
                        {
                            RepairDtl rd = db.RepairDtls.Find(ss);
                            rd.CloseDate = DateTime.Now;
                            rf.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                            rf.Status = "2";
                            rf.Rtt = DateTime.Now;
                            rf.Rtp = WebSecurity.CurrentUserId;
                            db.Entry(rf).State = EntityState.Modified;
                            db.Entry(rd).State = EntityState.Modified;
                            db.SaveChanges();
                            //Send Mail
                            Tmail mail = new Tmail();
                            //string body = "";
                            //AppUser u;
                            //Repair rp = db.Repairs.Find(ss);
                            //RepairDtl dtl = db.RepairDtls.Find(ss);
                            //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                            //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                            //u = db.AppUsers.Find(rf.UserId);
                            //mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                            //db.RepairFlows.Where(f => f.DocId == ss).Select(f => f.UserId).Distinct()
                            //    .Join(db.AppUsers, f => f, a => a.Id,
                            //    (f, a) => a).ToList()
                            //    .ForEach(a =>
                            //    {
                            //        mail.cc.Add(new System.Net.Mail.MailAddress(a.Email));
                            //    });

                            //mail.message.Subject = "醫療儀器管理資訊系統[請修案-結案通知]：儀器名稱： " + rp.AssetName;
                            //body += "<p>表單編號：" + ss + "</p>";
                            //body += "<p>申請人：" + rp.UserName + "</p>";
                            //body += "<p>儀器名稱：" + rp.AssetName + "</p>";
                            //body += "<p>處理結果：" + dtl.DealState + "</p>";
                            //body += "<p>處理描述：" + dtl.DealDes + "</p>";
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
                            rf.Opinions = "[廢除]" + Environment.NewLine + assign.AssignOpn;
                            rf.Status = "3";
                            rf.Rtt = DateTime.Now;
                            rf.Rtp = WebSecurity.CurrentUserId;
                            db.Entry(rf).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            //轉送下一關卡
                            rf.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                            rf.Status = "1";
                            rf.Rtt = DateTime.Now;
                            rf.Rtp = WebSecurity.CurrentUserId;
                            RepairFlow flow = new RepairFlow();
                            flow.DocId = ss;
                            flow.StepId = rf.StepId + 1;
                            flow.UserId = assign.FlowUid.Value;
                            flow.UserName = db.AppUsers.Find(assign.FlowUid).FullName;
                            flow.Status = "?";
                            flow.Cls = assign.FlowCls;
                            flow.Rtt = DateTime.Now;

                            db.Entry(rf).State = EntityState.Modified;
                            db.RepairFlows.Add(flow);
                            db.SaveChanges();
                            if (assign.FlowCls != "設備工程師")
                            {
                                //Send Mail
                                Tmail mail = new Tmail();
                                //string body = "";
                                //AppUser u;
                                //Repair rp = db.Repairs.Find(ss);
                                //RepairDtl dtl = db.RepairDtls.Find(ss);
                                //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                                //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                                //u = db.AppUsers.Find(flow.UserId);
                                //mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                                //mail.message.Subject = "醫療儀器管理資訊系統[請修案]：儀器名稱： " + rp.AssetName;
                                //body += "<p>表單編號：" + ss + "</p>";
                                //body += "<p>申請人：" + rp.UserName + "</p>";
                                //body += "<p>儀器名稱：" + rp.AssetName + "</p>";
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
            RepairDtl dtl = db.RepairDtls.Find(assign.DocId);
            //if (dtl != null)
            //{
            //    assign.CanClose = dtl.CanClose == "Y" ? true : false;
            //}
            assign.CanClose = true;
            List<SelectListItem> listItem = new List<SelectListItem>();
            //listItem.Add(new SelectListItem { Text = "維修工程師", Value = "維修工程師" });
            listItem.Add(new SelectListItem { Text = "申請人", Value = "申請人" });
            listItem.Add(new SelectListItem { Text = "驗收人", Value = "驗收人" });
            //listItem.Add(new SelectListItem { Text = "醫工經辦", Value = "醫工經辦" });
            listItem.Add(new SelectListItem { Text = "單位主管", Value = "單位主管" });
            listItem.Add(new SelectListItem { Text = "設備工程師", Value = "設備工程師" });
            listItem.Add(new SelectListItem { Text = "設備主管", Value = "設備主管" });
            RepairFlow rf = db.RepairFlows.Where(f => f.DocId == id && f.Status == "?").ToList().FirstOrDefault();
            if (rf != null)
            {
                assign.ClsNow = rf.Cls;

                if (rf.Cls == "驗收人")
                {
                    if (dtl.IsCharged == "Y")
                    {
                        if (db.RepairCosts.Where(c => c.DocId == id).Count() > 0)
                        {
                            if (db.RepairCosts.Where(c => c.DocId == id).Sum(c => c.TotalCost) > 0
                                && dtl.CanClose == "Y")
                                listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
                        }
                    }
                    else
                    {
                        if(dtl.CanClose == "Y")
                            listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
                    }

                }
                if (rf.Cls == "醫工經辦")
                {
                    listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
                }
                //if (rf.Cls == "單位主管" && dtl.CanClose == "Y")
                //{
                //    listItem.Add(new SelectListItem { Text = "結案", Value = "結案" });
                //}
                if (rf.Cls == "申請者")
                    listItem.Add(new SelectListItem { Text = "廢除", Value = "廢除" });
                if (rf.Cls == "維修工程師")
                {
                    listItem.Clear();
                    listItem.Add(new SelectListItem { Text = "申請人", Value = "申請人" });
                    listItem.Add(new SelectListItem { Text = "單位主管", Value = "單位主管" });
                    listItem.Add(new SelectListItem { Text = "設備工程師", Value = "設備工程師" });
                    listItem.Add(new SelectListItem { Text = "設備主管", Value = "設備主管" });
                }
            }
            if (rf.Cls == "驗收人")
            {
                ViewData["FlowCls"] = new SelectList(listItem, "Value", "Text", "結案");
            }
            else
            {
                ViewData["FlowCls"] = new SelectList(listItem, "Value", "Text", "");
            }
            

            //List<SelectListItem> flowvendor = new List<SelectListItem>();
            //db.Vendors.ToList().ForEach(v =>
            //{
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
            if (rf.Cls == "驗收人")
            {
                listItem3.Clear();
                listItem3.Add(new SelectListItem { Text = "", Value = "0" });
                ViewData["FlowUid"] = new SelectList(listItem3, "Value", "Text", "0");
            }
            //
            assign.Hint = "使用者key單→設備工程師維護(若無費用user驗收結案,有費用夾帶報價資料給設備主管)→單位主管(相關單位主管核決)→設備工程師維護→使用者驗收結案";
            return PartialView(assign);
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult NextFlow(Assign assign)
        {
            
            if (assign.FlowCls == "結案" || assign.FlowCls == "廢除")
                assign.FlowUid = WebSecurity.CurrentUserId;
            Repair repair = db.Repairs.Find(assign.DocId);
            if (!string.IsNullOrEmpty(assign.AssetNo))
            {
                Asset asset = db.Assets.Find(assign.AssetNo);
                if (asset != null)
                {
                    repair.AssetNo = asset.AssetNo;
                    repair.AssetName = asset.Cname;
                    repair.AccDpt = asset.AccDpt;
                    repair.Email = db.AppUsers.Find(repair.UserId).Email;
                    db.Entry(repair).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            RepairDtl dtl = db.RepairDtls.Find(assign.DocId);
            if (dtl != null)
            {
                if (assign.CanClose)
                {
                    dtl.CanClose = "Y";
                    db.Entry(dtl).State = EntityState.Modified;
                }
                else
                {
                    dtl.CanClose = "N";
                    db.Entry(dtl).State = EntityState.Modified;
                }
                db.SaveChanges();
            }

            if (ModelState.IsValid)
            {
                RepairFlow rf = db.RepairFlows.Where(f => f.DocId == assign.DocId && f.Status == "?").ToList().FirstOrDefault();
                if (assign.FlowCls == "驗收人" || assign.FlowCls == "設備主管")
                {
                    if (db.RepairEmps.Where(emp => emp.DocId == assign.DocId).Count() <= 0)
                    {
                        throw new Exception("【工程師列表】> 工時紀錄尚未填寫!!");
                    }
                    else if (db.RepairDtls.Find(assign.DocId).EndDate == null)
                    {
                        throw new Exception("【請修紀錄】> 沒有【完工日】!!");
                    }
                    else if (string.IsNullOrEmpty(db.RepairDtls.Find(assign.DocId).DealState))
                    {
                        throw new Exception("【請修紀錄】> 【處理狀態】不可空值!!");
                    }
                    if (string.IsNullOrEmpty(db.RepairDtls.Find(assign.DocId).FailFactor))
                    {
                        throw new Exception("【請修紀錄】> 【故障原因】不可空白!!");
                    }
                    if (string.IsNullOrEmpty(db.RepairDtls.Find(assign.DocId).InOut))
                    {
                        throw new Exception("【請修紀錄】> 【維修方式】不可空白!!");
                    }
                }
                if (assign.FlowCls == "結案")
                {
                    var isQuestExist = db.QuestAnswers.Where(q => q.Docid == assign.DocId).ToList().FirstOrDefault();
                    if (isQuestExist == null)
                    {
                        throw new Exception("請填寫滿意度問卷。");
                    }
                    //
                    RepairDtl rd = db.RepairDtls.Find(assign.DocId);
                    rd.CloseDate = DateTime.Now;
                    rf.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                    rf.Status = "2";
                    rf.Rtt = DateTime.Now;
                    rf.Rtp = WebSecurity.CurrentUserId;
                    db.Entry(rf).State = EntityState.Modified;
                    db.Entry(rd).State = EntityState.Modified;
                    db.SaveChanges();
                    //Send Mail
                    Tmail mail = new Tmail();
                    //string body = "";
                    //string sto = "";
                    //AppUser u;
                    //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                    //db.RepairFlows.Where(f => f.DocId == assign.DocId)
                    //    .ToList()
                    //    .ForEach(f =>
                    //    {
                    //        if (!f.Cls.Contains("工程師"))
                    //        {
                    //            u = db.AppUsers.Find(f.UserId);
                    //            sto += u.Email + ",";
                    //        }
                    //    });
                    //mail.sto = sto.TrimEnd(new char[] { ',' });

                    //mail.message.Subject = "醫療儀器管理資訊系統[請修案-結案通知]：儀器名稱： " + repair.AssetName;
                    //body += "<p>申請人：" + repair.UserName + "</p>";
                    //body += "<p>財產編號：" + repair.AssetNo + "</p>";
                    //body += "<p>儀器名稱：" + repair.AssetName + "</p>";
                    //body += "<p>放置地點：" + repair.PlaceLoc + "</p>";
                    //body += "<p>故障描述：" + repair.TroubleDes + "</p>";
                    //body += "<p>處理描述：" + rd.DealDes + "</p>";
                    //body += "<p><a href='https://mdms.ymuh.ym.edu.tw/'>檢視案件</a></p>";
                    //body += "<br/>";
                    //body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                    //mail.message.Body = body;
                    //mail.message.IsBodyHtml = true;
                    //mail.SendMail();
                }
                else if (assign.FlowCls == "廢除")
                {
                    rf.Opinions = "[廢除]" + Environment.NewLine + assign.AssignOpn;
                    rf.Status = "3";
                    rf.Rtt = DateTime.Now;
                    rf.Rtp = WebSecurity.CurrentUserId;
                    db.Entry(rf).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    //轉送下一關卡
                    rf.Opinions = "[" + assign.AssignCls + "]" + Environment.NewLine + assign.AssignOpn;
                    rf.Status = "1";
                    rf.Rtt = DateTime.Now;
                    rf.Rtp = WebSecurity.CurrentUserId;
                    db.Entry(rf).State = EntityState.Modified;
                    db.SaveChanges();
                    //
                    RepairFlow flow = new RepairFlow();
                    flow.DocId = assign.DocId;
                    flow.StepId = rf.StepId + 1;
                    flow.UserId = assign.FlowUid.Value;
                    flow.UserName = db.AppUsers.Find(assign.FlowUid.Value).FullName;
                    flow.Status = "?";
                    flow.Cls = assign.FlowCls;
                    flow.Rtt = DateTime.Now;
                    db.RepairFlows.Add(flow);
                    db.SaveChanges();
                    if (!assign.FlowCls.Contains("工程師"))
                    {
                        //Send Mail
                        Tmail mail = new Tmail();
                        //string body = "";
                        //AppUser u;
                        ////Repair repair = db.Repairs.Find(assign.DocId);
                        //u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                        //mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                        //u = db.AppUsers.Find(flow.UserId);
                        //mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                        //                                                    //mail.cc = new System.Net.Mail.MailAddress("99242@cch.org.tw");
                        //mail.message.Subject = "醫療儀器管理資訊系統[請修案]：儀器名稱： " + repair.AssetName;
                        //body += "<p>申請人：" + repair.UserName + "</p>";
                        //body += "<p>財產編號：" + repair.AssetNo + "</p>";
                        //body += "<p>儀器名稱：" + repair.AssetName + "</p>";
                        //body += "<p>放置地點：" + repair.PlaceLoc + "</p>";
                        //body += "<p>故障描述：" + repair.TroubleDes + "</p>";
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
            List<ListItem> list = new List<ListItem>();
            List<string> s;
            ListItem li;
            AppUser u;
            Repair r = db.Repairs.Find(docid);
            Asset asset = new Asset();
            if (r != null)
            {
                asset = db.Assets.Find(r.AssetNo);
            }
            string g = "";
            //if(r != null)
            //    g = db.CustOrgans.Find(db.AppUsers.Find(r.UserId).DptId).GroupId;
            switch (cls)
            {
                case "維修工程師":
                    Roles.GetUsersInRole("Engineer").ToList()
                        .ForEach(x =>
                        {
                            u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (vendor != null && u != null)
                            {
                                if (u.VendorId != null)
                                {
                                    if (u.VendorId.ToString() == vendor)
                                    {
                                        li = new ListItem();
                                        li.Text = "(" + u.UserName + ")" + u.FullName;
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
                                    li.Text = "(" + u.UserName + ")" + u.FullName;
                                    li.Value = u.Id.ToString();
                                    list.Add(li);
                                }
                            }
                        });
                    break;
                case "設備主管":
                    s = Roles.GetUsersInRole("MedMgr").ToList();
                    list = new List<ListItem>();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        if (!string.IsNullOrEmpty(u.DptId))
                        {
                            li = new ListItem();
                            li.Text = "(" + u.UserName + ")" + u.FullName;
                            li.Value = WebSecurity.GetUserId(l).ToString();
                            list.Add(li);
                        }
                    }
                    break;
                case "醫工經辦":
                    s = Roles.GetUsersInRole("MedToDo").ToList();
                    list = new List<ListItem>();
                    foreach (string l in s)
                    {
                        u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                        li = new ListItem();
                        li.Text = "(" + u.UserName + ")" + u.FullName;
                        li.Value = WebSecurity.GetUserId(l).ToString();
                        list.Add(li);
                    }
                    break;
                case "單位主管":
                    s = Roles.GetUsersInRole("Manager").ToList();
                    if (r != null)
                    {
                        string c = db.AppUsers.Find(r.UserId).DptId;
                        list = new List<ListItem>();
                        foreach (string l in s)
                        {
                            u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                            if (u != null)
                            {
                                if (u.DptId == c)
                                {
                                    li = new ListItem();
                                    li.Text = "(" + u.UserName + ")" + u.FullName;
                                    li.Value = WebSecurity.GetUserId(l).ToString();
                                    list.Add(li);
                                }
                            }
                        }
                    }
                    break;
                case "申請人":
                    if (r != null)
                    {
                        u = db.AppUsers.Find(r.UserId);
                        if (u != null)
                        {
                            list = new List<ListItem>();
                            li = new ListItem();
                            li.Text = "(" + u.UserName + ")" + r.UserName;
                            li.Value = r.UserId.ToString();
                            list.Add(li);
                        }
                    }
                    else
                    {
                        list = new List<ListItem>();
                        li = new ListItem();
                        li.Text = "宋大衛";
                        li.Value = "000";
                        list.Add(li);
                    }
                    break;
                case "驗收人":
                    if (db.RepairEmps.Where(emp => emp.DocId == docid).Count() <= 0)
                    {
                        throw new Exception("【工程師列表】> 工時紀錄尚未填寫!!");
                    }
                    if (r != null)
                    {
                        List<AppUser> ul = db.AppUsers.Where(f => f.DptId == r.DptId)
                            .Where(f => f.Status == "Y").ToList();
                        if (asset != null)
                        {
                            ul.AddRange(db.AppUsers.Where(f => f.DptId == asset.DelivDpt)
                                .Where(f => f.Status == "Y").ToList());
                        }

                        list = new List<ListItem>();
                        li = new ListItem();
                        u = db.AppUsers.Find(r.CheckerId);
                        if (u != null)
                        {
                            li.Text = "(" + u.UserName + ")" + r.CheckerName;
                            li.Value = r.CheckerId.ToString();
                            list.Add(li);
                        }
                        //
                        foreach (AppUser l in ul)
                        {
                            u = db.AppUsers.Find(l.Id);
                            li = new ListItem();
                            if (u != null)
                            {
                                li.Text = "(" + l.UserName + ")" + l.FullName;
                                li.Value = l.Id.ToString();
                                list.Add(li);
                            }
                        }
                    }
                    break;
                case "設備工程師":
                    s = Roles.GetUsersInRole("MedEngineer").ToList();
                    list = new List<ListItem>();
                    var lastEngFlow = db.RepairFlows.Where(rf => rf.DocId == docid).Where(rf => rf.Cls.Contains("設備工程師"))
                                                    .Where(rf => rf.Status == "1").OrderByDescending(rf => rf.StepId).ToList().FirstOrDefault();
                    int? lastEng = null;
                    if (lastEngFlow != null)
                    {
                        lastEng = lastEngFlow.UserId;
                    }
                    if (lastEng != null)
                    {
                        var eng = db.AppUsers.Find(lastEng);
                        li = new ListItem();
                        li.Text = "(" + eng.UserName + ")" + eng.FullName;
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
                                    li.Text = "(" + u.UserName + ")" + u.FullName;
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
                                li.Text = "(" + u.UserName + ")" + u.FullName;
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
