using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Filters;
using BMEDmgt.Models;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class KeepsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 100;

        public ActionResult Create()
        {
            Keep r = new Keep();
            AppUser u = db.AppUsers.Where(p => p.Id == WebSecurity.CurrentUserId).FirstOrDefault();
            //CustOrgan c = db.CustOrgans.Find(u.DptId);
            //Vendor v = db.Vendors.Find(u.VendorId);
            r.Email = u.Email == null ? "" : u.Email;
            Department d = db.Departments.Find(u.DptId);
            r.DocId = GetID();
            r.UserId = u.Id;
            r.UserName = u.FullName;
            //r.DptName = c.CustId == null ? "" : c.CustNam;
            //r.AccDpt = c.CustId == null ? "" : c.CustId;
            //r.AccDptName = c.CustId == null ? "" : c.CustNam;
            r.SentDate = DateTime.Now;
            r.DptId = d == null ? "" : d.DptId;
            r.Company = d == null ? "" : d.Name_C;
            r.AccDpt = d == null ? "" : d.DptId;
            r.AccDptName = d == null ? "" : d.Name_C;
            r.Contact = u.Mobile == null ? "" : u.Mobile;
            //
            db.Keeps.Add(r);
            db.SaveChanges();
            List<SelectListItem> listItem = new List<SelectListItem>();
            List<SelectListItem> AccDpt = new List<SelectListItem>();
            db.Departments.ToList()
                .ForEach(dp =>
                {
                    AccDpt.Add(new SelectListItem
                    {
                        Value = dp.DptId,
                        Text = dp.Name_C,
                        Selected = false
                    });
                });
            ViewData["AccDpt"] = AccDpt;
            //
            List<Asset> alist = null;
            if (u.DptId != null)
                alist = db.Assets.Where(at => at.AccDpt == u.DptId)
                    .Where(at => at.DisposeKind != "報廢").ToList();
            else if (u.VendorId > 0)
            {
                string s = Convert.ToString(u.VendorId);
                alist = db.Assets.Where(at => at.AccDpt == s)
                    .Where(at => at.DisposeKind != "報廢").ToList();
            }

            return View(r);
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult Create(Keep keep)
        {
            if (string.IsNullOrEmpty(keep.AssetNo))
            {
                throw new Exception("財產編號不可空白!!");
            }
            if (ModelState.IsValid)
            {

                //更新申請人的Email
                if (string.IsNullOrEmpty(keep.Email))
                {
                    throw new Exception("電子信箱不可空白!!");
                }
                AppUser a = db.AppUsers.Find(keep.UserId);
                a.Email = keep.Email;
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                //
                AssetKeep kp = db.AssetKeeps.Find(keep.AssetNo);
                Asset at = db.Assets.Find(keep.AssetNo);
                //
                keep.AssetName = db.Assets.Find(keep.AssetNo).Cname;
                keep.AccDpt = at.AccDpt;
                keep.SentDate = DateTime.Now;
                keep.Cycle = kp == null ? 0 : kp.Cycle.Value;
                keep.Src = "M";
                db.Entry(keep).State = EntityState.Modified;

                //
                KeepDtl dl = new KeepDtl();
                dl.DocId = keep.DocId;
                switch (kp == null ? "自行" : kp.InOut)
                {
                    case "自行":
                        dl.InOut = "0";
                        break;
                    case "委外":
                        dl.InOut = "1";
                        break;
                    case "租賃":
                        dl.InOut = "2";
                        break;
                    case "保固":
                        dl.InOut = "3";
                        break;
                    case "借用":
                        dl.InOut = "4";
                        break;
                    case "委外/自行":
                        dl.InOut = "5";
                        break;
                    default:
                        dl.InOut = "0";
                        break;
                }
                db.KeepDtls.Add(dl);
                db.SaveChanges();
                //
                KeepFlow rf = new KeepFlow();
                rf.DocId = keep.DocId;
                rf.StepId = 1;
                rf.UserId = WebSecurity.CurrentUserId;
                rf.Status = "1";
                rf.Role = Roles.GetRolesForUser().FirstOrDefault();
                rf.Rtp = WebSecurity.CurrentUserId;
                rf.Rdt = null;
                rf.Rtt = DateTime.Now;
                rf.Cls = "申請者";
                db.KeepFlows.Add(rf);
                //
                //AssetKeep kp = db.AssetKeeps.Find(keep.AssetNo);
                //if (kp == null)
                //{
                //    throw new Exception("無工程師資料!!");
                //}
                rf = new KeepFlow();
                rf.DocId = keep.DocId;
                rf.StepId = 2;
                rf.UserId = kp == null ? WebSecurity.CurrentUserId : kp.KeepEngId;
                rf.Status = "?";
                AppUser u = db.AppUsers.Find(rf.UserId);
                if (u == null)
                {
                    throw new Exception("無工程師資料!!");
                }
                rf.Role = Roles.GetRolesForUser(u.UserName).FirstOrDefault();
                rf.Rtp = null;
                rf.Rdt = null;
                rf.Rtt = DateTime.Now;
                rf.Cls = "設備工程師";
                if(dl.InOut != "0")         //2018-5-16修改
                    rf.Cls = "維修工程師"; 
                db.KeepFlows.Add(rf);
                db.SaveChanges();
                if (dl.InOut != "0")
                {
                    //send mail
                    Tmail mail = new Tmail();
                    string body = "";
                    u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                    u = db.AppUsers.Find(kp.KeepEngId);
                    mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                                                                        //mail.cc = new System.Net.Mail.MailAddress("99242@cch.org.tw");
                    mail.message.Subject = "醫療儀器管理資訊系統[保養案]：儀器名稱： " + keep.AssetName;
                    body += "<p>申請人：" + keep.UserName + "</p>";
                    body += "<p>財產編號：" + keep.AssetNo + "</p>";
                    body += "<p>儀器名稱：" + keep.AssetName + "</p>";
                    body += "<p>放置地點：" + keep.PlaceLoc + "</p>";
                    //body += "<p>故障描述：" + repair.TroubleDes + "</p>";
                    //body += "<p>放置地點：" + repair.PlaceLoc + "</p>";
                    body += "<p><a href='https://bmed.tmuh.org.tw/bmed'>處理案件</a></p>";
                    body += "<br/>";
                    body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                    mail.message.Body = body;
                    mail.message.IsBodyHtml = true;
                    //mail.SendMail();
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

        public string GetID()
        {
            //string str = "";
            //str += "SELECT MAX(DOCID) RECNO FROM REPAIR ";
            //var r = db.Database.SqlQuery(typeof(string), str);
            string s = db.Keeps.Select(r => r.DocId).Max();
            string did = "";
            int yymm = (System.DateTime.Now.Year - 1911) * 100 + System.DateTime.Now.Month;
            if (!string.IsNullOrEmpty(s))
            {
                did = s;
            }
            if (did != "")
            {
                if (Convert.ToInt64(did) / 10000 == yymm)
                    did = Convert.ToString(Convert.ToInt64(did) + 1);
                else
                    did = Convert.ToString(yymm * 10000 + 1);
            }
            else
            {
                did = Convert.ToString(yymm * 10000 + 1);
            }
            return did;
        }
        public ActionResult Index()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "待處理", Value = "待處理" });
            listItem.Add(new SelectListItem { Text = "已處理", Value = "已處理" });
            listItem.Add(new SelectListItem { Text = "已結案", Value = "已結案" });
            ViewData["FLOWTYPE"] = new SelectList(listItem, "Value", "Text", "待處理");

            List<SelectListItem> listItem2 = new List<SelectListItem>();
            SelectListItem li;
            db.Departments.ToList()
                .ForEach(d =>
                {
                    li = new SelectListItem();
                    li.Text = d.Name_C;
                    li.Value = d.DptId;
                    listItem2.Add(li);

                });
            ViewData["ACCDPT"] = new SelectList(listItem2, "Value", "Text");
            ViewData["APPLYDPT"] = new SelectList(listItem2, "Value", "Text");
            //
            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Text = "自行", Value = "自行" });
            listItem3.Add(new SelectListItem { Text = "委外", Value = "委外" });
            listItem3.Add(new SelectListItem { Text = "租賃", Value = "租賃" });
            listItem3.Add(new SelectListItem { Text = "保固", Value = "保固" });
            listItem3.Add(new SelectListItem { Text = "借用", Value = "借用" });
            listItem3.Add(new SelectListItem { Text = "委外/自行", Value = "委外/自行" });
            ViewData["INOUT"] = new SelectList(listItem3, "Value", "Text");

            return PartialView();
        }
        //[HttpPost]
        //public ActionResult Index(FormCollection fm)
        //{
        //    string ftype = fm["qtyFLOWTYPE"];
        //    string aname = fm["qtyASSETNAME"];
        //    string ano = fm["qtyASSETNO"];
        //    string acc = fm["qtyACCDPT"];
        //    string docid = fm["qtyDOCID"];
        //    string dptid = fm["qtyDPTID"];
        //    List<KeepListVModel> kv = new List<KeepListVModel>();
        //    AppUser usr = db.AppUsers.Find(WebSecurity.CurrentUserId);
        //    var vendors = db.Assets.Join(db.Vendors, a => a.VendorId, v => v.VendorId,
        //        (a, v) => new {
        //            ano = a.AssetNo,
        //            vname = v.VendorName
        //        });
        //    switch (ftype)
        //    {
        //        case "已處理":
        //            db.KeepFlows.Where(f => f.Status == "?")
        //            .Join(db.KeepFlows.Where(f2 => f2.UserId == WebSecurity.CurrentUserId && f2.Status == "1"),
        //             f => f.DocId, f2 => f2.DocId, (f, f2) => f)
        //        .Select(f => new
        //        {
        //            f.DocId,
        //            f.UserId,
        //            f.Status,
        //            f.Cls
        //        })
        //        .Join(db.Keeps, f => f.DocId, k => k.DocId,
        //        (f, k) => new
        //        {
        //            keep = k,
        //            flow = f
        //        })
        //        .Join(db.KeepDtls, m => m.keep.DocId, d => d.DocId,
        //        (m, d) => new
        //        {
        //            keep = m.keep,
        //            flow = m.flow,
        //            keepdtl = d
        //        }).Join(db.Departments, j => j.keep.AccDpt, d => d.DptId,
        //            (j, d) => new
        //            {
        //                keep = j.keep,
        //                flow = j.flow,
        //                keepdtl = j.keepdtl,
        //                dpt = d
        //            }).ToList()
        //            //}).Join(vendors, j => j.keep.AssetNo, v => v.ano,
        //            //(j, v) => new {
        //            //    keep = j.keep,
        //            //    flow = j.flow,
        //            //    keepdtl = j.keepdtl,
        //            //    dpt = j.dpt,
        //            //    vname = v.vname
        //            //}).ToList()
        //            .ForEach(j => kv.Add(new KeepListVModel
        //            {
        //                DocType = "保養",
        //                DocId = j.keep.DocId,
        //                AssetNo = j.keep.AssetNo,
        //                AssetName = j.keep.AssetName,
        //                ApplyDpt = j.keep.DptId,
        //                AccDpt = j.keep.AccDpt,
        //                AccDptName = j.dpt.Name_C,
        //                //Result = j.keepdtl.Result,
        //                //InOut = j.keepdtl.InOut,
        //                Result = j.keepdtl.Result == "1" ? "功能正常" :
        //                j.keepdtl.Result == "2" ? "預防處理" :
        //                j.keepdtl.Result == "3" ? "異常處理" :
        //                j.keepdtl.Result == "4" ? "維修時保養" :
        //                j.keepdtl.Result == "5" ? "退件" : "",
        //                InOut = j.keepdtl.InOut == "0" ? "自行" :
        //                j.keepdtl.InOut == "1" ? "委外" :
        //                j.keepdtl.InOut == "2" ? "租賃" :
        //                j.keepdtl.InOut == "3" ? "保固" : "",
        //                Memo = j.keepdtl.Memo,
        //                Cost = j.keepdtl.Cost,
        //                Days = DateTime.Now.Subtract(j.keep.SentDate.GetValueOrDefault()).Days,
        //                Flg = j.flow.Status,
        //                FlowUid = j.flow.UserId,
        //                FlowCls = j.flow.Cls,
        //                //Vendor = j.vname,
        //                Src = j.keep.Src
        //            }));
        //            break;
        //        case "已結案":
        //            List<KeepFlow> kf = db.KeepFlows.Where(f => f.Status == "2").ToList();
        //            if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Manager"))
        //            {
        //                if (Roles.IsUserInRole("Manager"))
        //                {
        //                    kf = kf.Join(db.Keeps.Where(r => r.AccDpt == usr.DptId),
        //                        f => f.DocId, r => r.DocId, (f, r) => f).ToList();
        //                }
        //            }
        //            else
        //            {
        //                kf = kf.Join(db.KeepFlows.Where(f2 => f2.UserId == WebSecurity.CurrentUserId),
        //                     f => f.DocId, f2 => f2.DocId, (f, f2) => f).ToList();
        //            }
        //            //
        //            kf.Select(f => new
        //            {
        //                f.DocId,
        //                f.UserId,
        //                f.Status
        //            })
        //        .Join(db.Keeps, f => f.DocId, k => k.DocId,
        //        (f, k) => new
        //        {
        //            keep = k,
        //            flow = f
        //        })
        //        .Join(db.KeepDtls, m => m.keep.DocId, d => d.DocId,
        //        (m, d) => new
        //        {
        //            keep = m.keep,
        //            flow = m.flow,
        //            keepdtl = d
        //        }).Join(db.Departments, j => j.keep.AccDpt, d => d.DptId,
        //            (j, d) => new {
        //                keep = j.keep,
        //                flow = j.flow,
        //                keepdtl = j.keepdtl,
        //                dpt = d
        //            }).GroupJoin(vendors, j => j.keep.AssetNo, v => v.ano,
        //            (j, v) => new {
        //                j = j,
        //                v = v
        //            }).SelectMany(v => v.v.DefaultIfEmpty(),
        //            (x, y) => new { x, y }).ToList()
        //            .ForEach(j => kv.Add(new KeepListVModel
        //            {
        //                DocType = "保養",
        //                DocId = j.x.j.keep.DocId,
        //                AssetNo = j.x.j.keep.AssetNo,
        //                AssetName = j.x.j.keep.AssetName,
        //                ApplyDpt = j.x.j.keep.DptId,
        //                AccDpt = j.x.j.keep.AccDpt,
        //                AccDptName = j.x.j.dpt.Name_C,
        //                //Result = j.keepdtl.Result,
        //                //InOut = j.keepdtl.InOut,
        //                Result = j.x.j.keepdtl.Result == "1" ? "功能正常" :
        //                j.x.j.keepdtl.Result == "2" ? "預防處理" :
        //                j.x.j.keepdtl.Result == "3" ? "異常處理" :
        //                j.x.j.keepdtl.Result == "4" ? "維修時保養" :
        //                j.x.j.keepdtl.Result == "5" ? "退件" : "",
        //                InOut = j.x.j.keepdtl.InOut == "0" ? "自行" :
        //                j.x.j.keepdtl.InOut == "1" ? "委外" :
        //                j.x.j.keepdtl.InOut == "2" ? "租賃" :
        //                j.x.j.keepdtl.InOut == "3" ? "保固" : "",
        //                Memo = j.x.j.keepdtl.Memo,
        //                Cost = j.x.j.keepdtl.Cost,
        //                Days = DateTime.Now.Subtract(j.x.j.keep.SentDate.GetValueOrDefault()).Days,
        //                Flg = j.x.j.flow.Status,
        //                FlowUid = j.x.j.flow.UserId,
        //                Src = j.x.j.keep.Src,
        //                Vendor = j.y == null ? "" : j.y.vname
        //            }));
        //            break;
        //        case "待處理":
        //            db.KeepFlows.Where(f => f.Status == "?" && f.UserId == WebSecurity.CurrentUserId)
        //        .Select(f => new
        //        {
        //            f.DocId,
        //            f.UserId,
        //            f.Status,
        //            f.Cls
        //        })
        //        .Join(db.Keeps, f => f.DocId, k => k.DocId,
        //        (f, k) => new
        //        {
        //            keep = k,
        //            flow = f
        //        })
        //        .Join(db.KeepDtls, m => m.keep.DocId, d => d.DocId,
        //        (m, d) => new
        //        {
        //            keep = m.keep,
        //            flow = m.flow,
        //            keepdtl = d
        //        }).Join(db.Departments, j => j.keep.AccDpt, d => d.DptId,
        //            (j, d) => new {
        //                keep = j.keep,
        //                flow = j.flow,
        //                keepdtl = j.keepdtl,
        //                dpt = d
        //            }).GroupJoin(vendors, j => j.keep.AssetNo, v => v.ano,
        //            (j, v) => new {
        //                j = j,
        //                v = v
        //            }).SelectMany(v => v.v.DefaultIfEmpty(),
        //            (x, y) => new { x, y }).ToList()
        //            .ForEach(j => kv.Add(new KeepListVModel
        //            {
        //                DocType = "保養",
        //                DocId = j.x.j.keep.DocId,
        //                AssetNo = j.x.j.keep.AssetNo,
        //                AssetName = j.x.j.keep.AssetName,
        //                ApplyDpt = j.x.j.keep.DptId,
        //                AccDpt = j.x.j.keep.AccDpt,
        //                AccDptName = j.x.j.dpt.Name_C,
        //                //Result = j.keepdtl.Result,
        //                //InOut = j.keepdtl.InOut,
        //                Result = j.x.j.keepdtl.Result == "1" ? "功能正常" :
        //                j.x.j.keepdtl.Result == "2" ? "預防處理" :
        //                j.x.j.keepdtl.Result == "3" ? "異常處理" :
        //                j.x.j.keepdtl.Result == "4" ? "維修時保養" :
        //                j.x.j.keepdtl.Result == "5" ? "退件" : "",
        //                InOut = j.x.j.keepdtl.InOut == "0" ? "自行" :
        //                j.x.j.keepdtl.InOut == "1" ? "委外" :
        //                j.x.j.keepdtl.InOut == "2" ? "租賃" :
        //                j.x.j.keepdtl.InOut == "3" ? "保固" : "",
        //                Memo = j.x.j.keepdtl.Memo,
        //                Cost = j.x.j.keepdtl.Cost,
        //                Days = DateTime.Now.Subtract(j.x.j.keep.SentDate.GetValueOrDefault()).Days,
        //                Flg = j.x.j.flow.Status,
        //                FlowUid = j.x.j.flow.UserId,
        //                FlowCls = j.x.j.flow.Cls,
        //                Src = j.x.j.keep.Src,
        //                Vendor = j.y == null ? "" : j.y.vname
        //            }));
        //            break;
        //    };


        //    if (!string.IsNullOrEmpty(docid))
        //    {
        //        kv = kv.Where(v => v.DocId == docid).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(ano))
        //    {
        //        kv = kv.Where(v => v.AssetNo == ano).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(dptid))
        //    {
        //        kv = kv.Where(v => v.ApplyDpt == dptid).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(acc))
        //    {
        //        kv = kv.Where(v => v.AccDpt == acc).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(aname))
        //    {
        //        kv = kv.Where(v => v.AssetName.Contains(aname)).ToList();
        //    }
        //    return PartialView("List", kv);
        //}

        public ActionResult Index2()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            //listItem.Add(new SelectListItem { Text = "待處理", Value = "待處理" });
            listItem.Add(new SelectListItem { Text = "已處理", Value = "已處理" });
            listItem.Add(new SelectListItem { Text = "已結案", Value = "已結案" });
            ViewData["FLOWTYP"] = new SelectList(listItem, "Value", "Text", "已處理");
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            SelectListItem li;
            db.Departments.ToList()
                .ForEach(d =>
                {
                    li = new SelectListItem();
                    li.Text = d.Name_C;
                    li.Value = d.DptId;
                    listItem2.Add(li);

                });
            ViewData["ACCDPT"] = new SelectList(listItem2, "Value", "Text");
            AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
            if (Roles.IsUserInRole("Usual"))
            {
                listItem2.Clear();
                if (u != null)
                {
                    li = new SelectListItem();
                    li.Text = db.Departments.Find(u.DptId).Name_C;
                    li.Value = u.DptId;
                    listItem2.Add(li);
                }
            }
            ViewData["APPLYDPT"] = new SelectList(listItem2, "Value", "Text");
            //
            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Text = "自行", Value = "自行" });
            listItem3.Add(new SelectListItem { Text = "委外", Value = "委外" });
            listItem3.Add(new SelectListItem { Text = "租賃", Value = "租賃" });
            listItem3.Add(new SelectListItem { Text = "保固", Value = "保固" });
            listItem3.Add(new SelectListItem { Text = "借用", Value = "借用" });
            listItem3.Add(new SelectListItem { Text = "委外/自行", Value = "委外/自行" });
            ViewData["INOUT"] = new SelectList(listItem3, "Value", "Text");
            ViewData["FLOWID"] = new SelectList(new List<SelectListItem>() {
            new SelectListItem(){ Text=u.FullName, Value=u.Id.ToString()} }, "Value", "Text");

            return View();
        }
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Index2(FormCollection fm)
        {
            string aname = fm["qtyASSETNAME"];
            string ano = fm["qtyASSETNO"];
            string acc = fm["qtyACCDPT"];
            string docid = fm["qtyDOCID"];
            string dptid = fm["qtyDPTID"];
            string flw = fm["qtyFLOWTYP"];
            string inout = fm["qtyINOUT"];
            string typ = fm["qtyTYPE"];
            string flowuid = fm["qtyFLOW"];
            string sd = fm["Sdate"];
            string ed = fm["Edate"];
            DateTime? sdate = null, edate = null;
            if (!string.IsNullOrEmpty(sd))
            {
                sdate = DateTime.ParseExact(sd.Replace("-", "/"), "yyyy/MM/dd", null);
            }
            if (!string.IsNullOrEmpty(ed))
            {
                edate = DateTime.ParseExact(ed.Replace("-", "/"), "yyyy/MM/dd", null)
                    .AddHours(23)
                    .AddMinutes(59)
                    .AddSeconds(59);
            }

            List<KeepListVModel> kv = new List<KeepListVModel>();
            if (string.IsNullOrEmpty(aname) && string.IsNullOrEmpty(ano) &&
                string.IsNullOrEmpty(acc) && string.IsNullOrEmpty(docid) &&
                string.IsNullOrEmpty(dptid) && string.IsNullOrEmpty(inout) &&
                string.IsNullOrEmpty(typ) && sdate == null && edate == null &&
                string.IsNullOrEmpty(flowuid))
            {
                throw new Exception("請輸入查詢條件!!");
            }
            else
            {
                List<Keep> rps = db.Keeps.ToList();
                if (Roles.IsUserInRole("Usual"))
                {
                    AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    if (u != null)
                        rps = rps.Where(r => dptid == u.DptId).ToList();
                    else
                        throw new Exception("無部門資料!!");
                }
                if (!string.IsNullOrEmpty(aname))
                    rps = rps.Where(r => r.AssetName != null)
                        .Where(r => r.AssetName.Contains(aname))
                        .ToList();
                if (!string.IsNullOrEmpty(ano))
                    rps = rps.Where(r => r.AssetNo == ano).ToList();
                if (!string.IsNullOrEmpty(acc))
                    rps = rps.Where(r => r.AccDpt == acc).ToList();
                if (!string.IsNullOrEmpty(docid))
                    rps = rps.Where(r => r.DocId == docid).ToList();
                if (!string.IsNullOrEmpty(dptid))
                    rps = rps.Where(r => r.DptId == dptid).ToList();
                if (!string.IsNullOrEmpty(typ))
                {
                    if (!string.IsNullOrEmpty(aname))
                    {
                        rps = rps.Union(db.Keeps.Join(db.Assets.Where(a => a.Type == typ), r => r.AssetNo, a => a.AssetNo,
                        (r, a) => r).ToList()).ToList();
                    }
                    else
                    {
                        rps = rps.Join(db.Assets.Where(a => a.Type == typ), r => r.AssetNo, a => a.AssetNo,
                            (r, a) => r).ToList();
                    }
                }
                if (sdate.HasValue)
                {
                    rps = rps.Where(r => r.SentDate >= sdate.Value).ToList();
                }
                if (edate.HasValue)
                {
                    rps = rps.Where(r => r.SentDate <= edate.Value).ToList();
                }
                var ss = new[] { "?", "2" };
                rps.Join(db.KeepDtls, r => r.DocId, d => d.DocId,
                    (r, d) => new {
                        keep = r,
                        dtl = d
                    }).Join(db.KeepFlows.Where(f => ss.Contains(f.Status)).Join(db.AppUsers, f => f.UserId, u => u.Id,
                    (f, u) => new { f.DocId,f.UserId,u.FullName,f.Status}), m => m.keep.DocId, f => f.DocId,
                    (m, f) => new
                    {
                        keep = m.keep,
                        flow = f,
                        keepdtl = m.dtl
                    }).Join(db.Departments, j => j.keep.AccDpt, d => d.DptId,
                    (j, d) => new {
                        keep = j.keep,
                        flow = j.flow,
                        keepdtl = j.keepdtl,
                        dpt = d
                    }).Join(db.Assets, j => j.keep.AssetNo, a => a.AssetNo,
                    (j, a) => new {
                        keep = j.keep,
                        flow = j.flow,
                        keepdtl = j.keepdtl,
                        dpt = j.dpt,
                        asset = a
                    }).ToList()
                    .ForEach(j => kv.Add(new KeepListVModel
                    {
                        DocType = "保養",
                        DocId = j.keep.DocId,
                        AssetNo = j.keep.AssetNo,
                        AssetName = j.keep.AssetName,
                        ApplyDpt = j.keep.DptId,
                        AccDpt = j.keep.AccDpt,
                        AccDptName = j.dpt.Name_C,
                        Result = j.keepdtl.Result,
                        InOut = j.keepdtl.InOut == "0" ? "自行" :
                        j.keepdtl.InOut == "1" ? "委外" :
                        j.keepdtl.InOut == "2" ? "租賃" :
                        j.keepdtl.InOut == "3" ? "保固" :
                        j.keepdtl.InOut == "4" ? "借用" :
                        j.keepdtl.InOut == "5" ? "委外/自行" : "",
                        Memo = j.keepdtl.Memo,
                        Cost = j.keepdtl.Cost,
                        Days = DateTime.Now.Subtract(j.keep.SentDate.GetValueOrDefault()).Days,
                        Flg = j.flow.Status,
                        FlowUid = j.flow.UserId,
                        FlowUname = j.flow.FullName,
                        Cycle = j.keep.Cycle,
                        WartySt = j.asset.WartySt,
                        WartyEd = j.asset.WartyEd,
                        EndDate = j.keepdtl.EndDate
                    }));
            }
            if (flw == "已處理")
                kv = kv.Where(r => r.Flg == "?").ToList();
            else if (flw == "已結案")
                kv = kv.Where(r => r.Flg == "2").ToList();

            if (!string.IsNullOrEmpty(inout))
                kv = kv.Where(r => r.InOut == inout).ToList();
            if (!string.IsNullOrEmpty(flowuid))
                kv = kv.Where(v => v.FlowUid == Convert.ToInt32(flowuid)).ToList();

            return PartialView("List2", kv.OrderByDescending(r => r.DocId));
        }

        [HttpPost]
        public void ExcelRepData(FormCollection fm) //public void ExcelRepData(List<string> rv)
        {
            string aname = fm["qtyASSETNAME"];
            string ano = fm["qtyASSETNO"];
            string acc = fm["qtyACCDPT"];
            string docid = fm["qtyDOCID"];
            string dptid = fm["qtyDPTID"];
            string flw = fm["qtyFLOWTYP"];
            string inout = fm["qtyINOUT"];
            List<Keep> rps = db.Keeps.ToList();
            if (Roles.IsUserInRole("Usual"))
            {
                AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                if (u != null)
                    rps = rps.Where(r => dptid == u.DptId).ToList();
                else
                    throw new Exception("無部門資料!!");
            }
            if (!string.IsNullOrEmpty(aname))
                rps = rps.Where(r => r.AssetName != null)
                    .Where(r => r.AssetName.Contains(aname))
                    .ToList();
            if (!string.IsNullOrEmpty(ano))
                rps = rps.Where(r => r.AssetNo == ano).ToList();
            if (!string.IsNullOrEmpty(acc))
                rps = rps.Where(r => r.AccDpt == acc).ToList();
            if (!string.IsNullOrEmpty(docid))
                rps = rps.Where(r => r.DocId == docid).ToList();
            if (!string.IsNullOrEmpty(dptid))
                rps = rps.Where(r => r.DptId == dptid).ToList();
            var ss = new[] { "?", "2" };
            List<KeepListVModel> rv = new List<KeepListVModel>();
            DataTable dt = new DataTable();
            DataRow dw;
            dt.Columns.Add("類別");
            dt.Columns.Add("表單編號");
            dt.Columns.Add("成本中心名稱");
            dt.Columns.Add("儀器名稱");
            dt.Columns.Add("保養方式");
            dt.Columns.Add("保養結果");
            dt.Columns.Add("保養描述");
            dt.Columns.Add("費用");
            dt.Columns.Add("天數");
            dt.Columns.Add("關卡人員");
            //
            var data = rps.Join(db.KeepDtls, r => r.DocId, d => d.DocId,
                    (r, d) => new {
                        keep = r,
                        dtl = d
                    }).Join(db.KeepFlows.Where(f => ss.Contains(f.Status)).Join(db.AppUsers, f => f.UserId, u => u.Id,
                    (f, u) => new { f.DocId, f.UserId, u.FullName, f.Status }), m => m.keep.DocId, f => f.DocId,
                    (m, f) => new
                    {
                        keep = m.keep,
                        flow = f,
                        keepdtl = m.dtl
                    }).Join(db.Departments, j => j.keep.AccDpt, d => d.DptId,
                    (j, d) => new {
                        keep = j.keep,
                        flow = j.flow,
                        keepdtl = j.keepdtl,
                        dpt = d
                    }).ToList();
            if (flw == "已處理")
                data = data.Where(r => r.flow.Status == "?").ToList();
            else if (flw == "已結案")
                data = data.Where(r => r.flow.Status == "2").ToList();
            switch (inout)
            {
                case "自行":
                    data = data.Where(r => r.keepdtl.InOut == "0").ToList();
                    break;
                case "委外":
                    data = data.Where(r => r.keepdtl.InOut == "1").ToList();
                    break;
                case "租賃":
                    data = data.Where(r => r.keepdtl.InOut == "2").ToList();
                    break;
                case "保固":
                    data = data.Where(r => r.keepdtl.InOut == "3").ToList();
                    break;
                case "借用":
                    data = data.Where(r => r.keepdtl.InOut == "4").ToList();
                    break;
                case "委外/自行":
                    data = data.Where(r => r.keepdtl.InOut == "5").ToList();
                    break;
            }
            data.ForEach(j =>
                    {
                        dw = dt.NewRow();
                        dw[0] = "保養";
                        dw[1] = j.keep.DocId;
                        //AssetNo = j.keep.AssetNo,
                        dw[3] = j.keep.AssetName;
                        //ApplyDpt = j.keep.DptId,
                        //AccDpt = j.keep.AccDpt,
                        dw[2] = j.dpt.Name_C;
                        dw[5] = j.keepdtl.Result;
                        dw[4] = j.keepdtl.InOut == "0" ? "自行" :
                        j.keepdtl.InOut == "1" ? "委外" :
                        j.keepdtl.InOut == "2" ? "租賃" :
                        j.keepdtl.InOut == "3" ? "保固" :
                        j.keepdtl.InOut == "4" ? "借用" :
                        j.keepdtl.InOut == "5" ? "委外/自行" : "";
                        dw[6] = j.keepdtl.Memo;
                        dw[7] = j.keepdtl.Cost;
                        dw[8] = DateTime.Now.Subtract(j.keep.SentDate.GetValueOrDefault()).Days;
                        //Flg = j.flow.Status,
                        //FlowUid = j.flow.UserId
                        dw[9] = j.flow.FullName;
                        dt.Rows.Add(dw);
                    });
            
            
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("保養案件查詢");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=KeepQryData.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        [HttpPost]
        public ActionResult List(FormCollection fm, int page = 1)
        {
            string ftype = fm["qtyFLOWTYPE"];
            string aname = fm["qtyASSETNAME"];
            string ano = fm["qtyASSETNO"];
            string acc = fm["qtyACCDPT"];
            string docid = fm["qtyDOCID"];
            string dptid = fm["qtyDPTID"];
            string inout = fm["qtyINOUT"];

            List<KeepListVModel> kv = new List<KeepListVModel>();
            AppUser usr = db.AppUsers.Find(WebSecurity.CurrentUserId);
            var vendors = db.Assets.Join(db.Vendors, a => a.VendorId, v => v.VendorId,
                (a, v) => new {
                    ano = a.AssetNo,
                    vname = v.VendorName
                });
            switch (ftype)
            {
                case "已處理":
                    db.KeepFlows.Where(f => f.Status == "?")
                    .Join(db.KeepFlows.Where(f2 => f2.UserId == WebSecurity.CurrentUserId && f2.Status == "1"),
                     f => f.DocId, f2 => f2.DocId, (f, f2) => f)
                .Select(f => new
                {
                    f.DocId,
                    f.UserId,
                    f.Status,
                    f.Cls
                })
                .Join(db.Keeps, f => f.DocId, k => k.DocId,
                (f, k) => new
                {
                    keep = k,
                    flow = f
                })
                .Join(db.KeepDtls, m => m.keep.DocId, d => d.DocId,
                (m, d) => new
                {
                    keep = m.keep,
                    flow = m.flow,
                    keepdtl = d
                }).Join(db.Departments, j => j.keep.AccDpt, d => d.DptId,
                    (j, d) => new
                    {
                        keep = j.keep,
                        flow = j.flow,
                        keepdtl = j.keepdtl,
                        dpt = d
                    }).ToList()
                    //}).Join(vendors, j => j.keep.AssetNo, v => v.ano,
                    //(j, v) => new {
                    //    keep = j.keep,
                    //    flow = j.flow,
                    //    keepdtl = j.keepdtl,
                    //    dpt = j.dpt,
                    //    vname = v.vname
                    //}).ToList()
                    .ForEach(j => kv.Add(new KeepListVModel
                    {
                        DocType = "保養",
                        DocId = j.keep.DocId,
                        AssetNo = j.keep.AssetNo,
                        AssetName = j.keep.AssetName,
                        ApplyDpt = j.keep.DptId,
                        AccDpt = j.keep.AccDpt,
                        AccDptName = j.dpt.Name_C,
                        //Result = j.keepdtl.Result,
                        //InOut = j.keepdtl.InOut,
                        Result = j.keepdtl.Result,
                        InOut = j.keepdtl.InOut == "0" ? "自行" :
                        j.keepdtl.InOut == "1" ? "委外" :
                        j.keepdtl.InOut == "2" ? "租賃" :
                        j.keepdtl.InOut == "3" ? "保固" :
                        j.keepdtl.InOut == "4" ? "借用" :
                        j.keepdtl.InOut == "5" ? "委外/自行" : "",
                        Memo = j.keepdtl.Memo,
                        Cost = j.keepdtl.Cost,
                        Days = DateTime.Now.Subtract(j.keep.SentDate.GetValueOrDefault()).Days,
                        Flg = j.flow.Status,
                        FlowUid = j.flow.UserId,
                        FlowCls = j.flow.Cls,
                        //Vendor = j.vname,
                        Src = j.keep.Src
                    }));
                    break;
                case "已結案":
                    List<KeepFlow> kf = db.KeepFlows.Where(f => f.Status == "2").ToList();
                    if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Manager"))
                    {
                        if (Roles.IsUserInRole("Manager"))
                        {
                            kf = kf.Join(db.Keeps.Where(r => r.AccDpt == usr.DptId),
                                f => f.DocId, r => r.DocId, (f, r) => f).ToList();
                        }
                    }
                    else
                    {
                        kf = kf.Join(db.KeepFlows.Where(f2 => f2.UserId == WebSecurity.CurrentUserId),
                             f => f.DocId, f2 => f2.DocId, (f, f2) => f).ToList();
                    }
                    //
                    kf.Select(f => new
                    {
                        f.DocId,
                        f.UserId,
                        f.Status
                    })
                .Join(db.Keeps, f => f.DocId, k => k.DocId,
                (f, k) => new
                {
                    keep = k,
                    flow = f
                })
                .Join(db.KeepDtls, m => m.keep.DocId, d => d.DocId,
                (m, d) => new
                {
                    keep = m.keep,
                    flow = m.flow,
                    keepdtl = d
                }).Join(db.Departments, j => j.keep.AccDpt, d => d.DptId,
                    (j, d) => new {
                        keep = j.keep,
                        flow = j.flow,
                        keepdtl = j.keepdtl,
                        dpt = d
                    }).GroupJoin(vendors, j => j.keep.AssetNo, v => v.ano,
                    (j, v) => new {
                        j = j,
                        v = v
                    }).SelectMany(v => v.v.DefaultIfEmpty(),
                    (x, y) => new { x, y }).ToList()
                    .ForEach(j => kv.Add(new KeepListVModel
                    {
                        DocType = "保養",
                        DocId = j.x.j.keep.DocId,
                        AssetNo = j.x.j.keep.AssetNo,
                        AssetName = j.x.j.keep.AssetName,
                        ApplyDpt = j.x.j.keep.DptId,
                        AccDpt = j.x.j.keep.AccDpt,
                        AccDptName = j.x.j.dpt.Name_C,
                        //Result = j.keepdtl.Result,
                        //InOut = j.keepdtl.InOut,
                        Result = j.x.j.keepdtl.Result,
                        InOut = j.x.j.keepdtl.InOut == "0" ? "自行" :
                        j.x.j.keepdtl.InOut == "1" ? "委外" :
                        j.x.j.keepdtl.InOut == "2" ? "租賃" :
                        j.x.j.keepdtl.InOut == "3" ? "保固" :
                        j.x.j.keepdtl.InOut == "4" ? "借用" :
                        j.x.j.keepdtl.InOut == "5" ? "委外/自行" : "",
                        Memo = j.x.j.keepdtl.Memo,
                        Cost = j.x.j.keepdtl.Cost,
                        Days = DateTime.Now.Subtract(j.x.j.keep.SentDate.GetValueOrDefault()).Days,
                        Flg = j.x.j.flow.Status,
                        FlowUid = j.x.j.flow.UserId,
                        Src = j.x.j.keep.Src,
                        Vendor = j.y == null ? "" : j.y.vname
                    }));
                    break;
                case "待處理":
                    db.KeepFlows.Where(f => f.Status == "?" && f.UserId == WebSecurity.CurrentUserId)
                .Select(f => new
                {
                    f.DocId,
                    f.UserId,
                    f.Status,
                    f.Cls
                })
                .Join(db.Keeps, f => f.DocId, k => k.DocId,
                (f, k) => new
                {
                    keep = k,
                    flow = f
                })
                .Join(db.KeepDtls, m => m.keep.DocId, d => d.DocId,
                (m, d) => new
                {
                    keep = m.keep,
                    flow = m.flow,
                    keepdtl = d
                }).Join(db.Departments, j => j.keep.AccDpt, d => d.DptId,
                    (j, d) => new {
                        keep = j.keep,
                        flow = j.flow,
                        keepdtl = j.keepdtl,
                        dpt = d
                    }).GroupJoin(vendors, j => j.keep.AssetNo, v => v.ano,
                    (j, v) => new {
                        j = j,
                        v = v
                    }).SelectMany(v => v.v.DefaultIfEmpty(),
                    (x, y) => new { x, y }).ToList()
                    .ForEach(j => kv.Add(new KeepListVModel
                    {
                        DocType = "保養",
                        DocId = j.x.j.keep.DocId,
                        AssetNo = j.x.j.keep.AssetNo,
                        AssetName = j.x.j.keep.AssetName,
                        ApplyDpt = j.x.j.keep.DptId,
                        AccDpt = j.x.j.keep.AccDpt,
                        AccDptName = j.x.j.dpt.Name_C,
                        //Result = j.keepdtl.Result,
                        //InOut = j.keepdtl.InOut,
                        Result = j.x.j.keepdtl.Result,
                        InOut = j.x.j.keepdtl.InOut == "0" ? "自行" :
                        j.x.j.keepdtl.InOut == "1" ? "委外" :
                        j.x.j.keepdtl.InOut == "2" ? "租賃" :
                        j.x.j.keepdtl.InOut == "3" ? "保固" :
                        j.x.j.keepdtl.InOut == "4" ? "借用" :
                        j.x.j.keepdtl.InOut == "5" ? "委外/自行" : "",
                        Memo = j.x.j.keepdtl.Memo,
                        Cost = j.x.j.keepdtl.Cost,
                        Days = DateTime.Now.Subtract(j.x.j.keep.SentDate.GetValueOrDefault()).Days,
                        Flg = j.x.j.flow.Status,
                        FlowUid = j.x.j.flow.UserId,
                        FlowCls = j.x.j.flow.Cls,
                        Src = j.x.j.keep.Src,
                        Vendor = j.y == null ? "" : j.y.vname
                    }));
                    break;
            };


            if (!string.IsNullOrEmpty(docid))
            {
                kv = kv.Where(v => v.DocId == docid).ToList();
            }
            if (!string.IsNullOrEmpty(ano))
            {
                kv = kv.Where(v => v.AssetNo == ano).ToList();
            }
            if (!string.IsNullOrEmpty(dptid))
            {
                kv = kv.Where(v => v.ApplyDpt == dptid).ToList();
            }
            if (!string.IsNullOrEmpty(acc))
            {
                kv = kv.Where(v => v.AccDpt == acc).ToList();
            }
            if (!string.IsNullOrEmpty(aname))
            {
                kv = kv.Where(v => v.AssetName.Contains(aname)).ToList();
            }
            if (!string.IsNullOrEmpty(inout))
                kv = kv.Where(r => r.InOut == inout).ToList();
            kv = kv.OrderByDescending(k => k.DocId).ToList();

            if (kv.ToPagedList(page, pageSize).Count <= 0)
                return PartialView(kv.ToPagedList(1, pageSize));

            return PartialView(kv.ToPagedList(page, pageSize));
        }

        public ActionResult Edit(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Keep keep = db.Keeps.Find(id);
                if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Manager"))
                {
                    return View(keep);
                }
                KeepFlow rf = db.KeepFlows.Where(f => f.DocId == id && f.Status == "?").FirstOrDefault();
                if (rf != null)
                {
                    if (rf.UserId != WebSecurity.CurrentUserId)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                return View(keep);
            }
            return View();
        }

        public ActionResult UpdCases(string kps)
        {
            Keep kp = new Keep();
            kp.DocId = kps;

            return View(kp);
        }

        [MyErrorHandler]
        public ActionResult Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                KeepFlow rf = db.KeepFlows.Where(f => f.DocId == id && f.Status == "?").FirstOrDefault();
                if (rf != null)
                {
                    if (rf.UserId != WebSecurity.CurrentUserId)
                    {
                        throw new Exception("沒有權限!!");
                    }
                    rf.Status = "3";
                    rf.Rtp = WebSecurity.CurrentUserId;
                    rf.Rtt = DateTime.Now;
                    db.Entry(rf).State = EntityState.Modified;
                    db.SaveChanges();
                    return new JsonResult
                    {
                        Data = new { success = true, error = "" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                else
                {
                    throw new Exception("無資料");
                }
            }
            else
            {
                throw new Exception("無資料");
            }
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                Keep r = new Keep();
                r.DocId = "000";
                r.UserName = "宋大衛";
                return PartialView(r);
            }
            Keep kp = db.Keeps.Find(id);
            if (kp == null)
            {
                return HttpNotFound();
            }
            kp.AccDptName = kp.AccDpt == null ? "" : db.Departments.Find(kp.AccDpt).Name_C;
            Asset at = db.Assets.Find(kp.AssetNo);
            if (at != null)
                kp.BuyDate = at.BuyDate;

            return PartialView(kp);
        }

        public ActionResult Views(string id)
        {
            Keep kp = db.Keeps.Find(id);
            if (kp == null)
            {
                return HttpNotFound();
            }
            return View(kp);
        }
    }
}