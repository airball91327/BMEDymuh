using System;
//using System.Data.Objects;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Security;
using WebMatrix.WebData;
using BMEDmgt.Models;
using BMEDmgt.Areas.MedEngMgt.Models;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    public class DeliveryListVModel
    {
        [Display(Name = "類別")]
        public string DocType { get; set; }
        [Display(Name = "表單編號")]
        public string Docid { get; set; }
        [Display(Name = "申請人代號")]
        public int UserId { get; set; }
        [Display(Name = "申請人姓名")]
        public string UserName { get; set; }
        [Display(Name = "所屬單位")]
        public string Company { get; set; }
        [Display(Name = "所屬單位代號名稱")]
        public string CompanyNam { get; set; }
        [Display(Name = "成本中心")]
        public string AccDpt { get; set; }
        [Display(Name = "成本中心名稱")]
        public string AccDptNam { get; set; }
        [Display(Name = "合約號碼")]
        public string ContractNo { get; set; }
        [Display(Name = "採購案號")]
        public string PurchaseNo { get; set; }
        [Display(Name = "列管編號")]
        public string CrlItemNo { get; set; }
        [Display(Name = "預算編號")]
        public string BudgetId { get; set; }
        [Display(Name = "天數")]
        public int? Days { get; set; }
        public string Flg { get; set; }
        public int FlowUid { get; set; }
        public string VendorNo { get; set; }

        public List<DeliveryListVModel> GetList(string cls = null)
        {
            List<DeliveryListVModel> dv = new List<DeliveryListVModel>();
            BMEDcontext db = new BMEDcontext();
            List<DelivFlow> rf = new List<DelivFlow>();
            List<DelivFlow> rf2;            
            switch (cls)
            {
                case "已處理":
                    rf2 = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DELIVFLOW WHERE STATUS ='?'")
                        .Where(m => m.Userid != WebSecurity.CurrentUserId).ToList();
                    if (Roles.IsUserInRole("MedToDo"))
                    {
                        rf.AddRange(rf2);
                    }
                    else
                    {
                        foreach (DelivFlow f in rf2)
                        {
                            if (db.DelivFlows.Where(m => m.Docid == f.Docid).Where(m => m.Userid == WebSecurity.CurrentUserId).Count() > 0)
                            {
                                rf.Add(f);
                            }
                        }
                    }

                    break;
                case "已結案":
                    rf2 = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DELIVFLOW WHERE STATUS ='2'").ToList();
                    if (Roles.IsUserInRole("MedToDo"))
                    {
                        rf.AddRange(rf2);
                    }
                    else
                    {
                        foreach (DelivFlow f in rf2)
                        {
                            if (db.DelivFlows.Where(m => m.Docid == f.Docid).Where(m => m.Userid == WebSecurity.CurrentUserId).Count() > 0)
                            {
                                rf.Add(f);
                            }
                        }
                    }
                    break;
                case "查詢":
                    rf2 = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DELIVFLOW WHERE STATUS ='?'").ToList();
                    AppUser p = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    //CustOrgan c = db.CustOrgans.Find(p.CustId);
                    Delivery r;
                    foreach (DelivFlow f in rf2)
                    {
                        r = db.Deliveries.Find(f.Docid);
                        //if (db.CustOrgans.Find(r.AccDpt).GroupId == c.GroupId)
                        //{
                            rf.Add(f);
                        //}
                    }
                    break;
                default:
                    rf = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DELIVFLOW WHERE STATUS ='?'")
                        .Where(m => m.Userid == WebSecurity.CurrentUserId).ToList();
                    break;
            }
            rf.OrderByDescending(m => m.Rtt);
            DeliveryListVModel i;
            foreach (DelivFlow f in rf)
            {
                Delivery r = db.Deliveries.Find(f.Docid);
                AppUser p = db.AppUsers.Find(r.UserId);
                Department c = db.Departments.Find(p.DptId);
                //BuyEvaluate b = db.BuyEvaluates.Find(r.PurchaseNo);
                i = new DeliveryListVModel();
                i.DocType = "驗收";
                i.Docid = r.Docid;
                i.UserId = r.UserId;
                i.UserName = r.UserName;
                if (p != null && p.DptId != null)
                {
                    i.Company = p.DptId;
                    i.CompanyNam = c == null ? "" : c.Name_C;
                }
                i.ContractNo = r.ContractNo;
                i.PurchaseNo = r.PurchaseNo;
                i.CrlItemNo = r.CrlItemNo;
                i.AccDptNam = db.Departments.Find(r.AccDpt) == null ? "": db.Departments.Find(r.AccDpt).Name_C;
                //if (b != null)
                //    i.BudgetId = b.BudgetId;
                //else
                    i.BudgetId = "";
                if (f.Status == "?")
                    i.Days = DateTime.Now.Subtract(r.ApplyDate.GetValueOrDefault()).Days;
                else
                    i.Days = null;
                i.Flg = f.Status;
                i.FlowUid = f.Userid;
                dv.Add(i);
            }
            //
            return dv;
        }

        public List<DeliveryListVModel> GetList2(string cls = null)
        {
            List<DeliveryListVModel> dv = new List<DeliveryListVModel>();
            BMEDcontext db = new BMEDcontext();
            List<DelivFlow> rf = new List<DelivFlow>();
            List<DelivFlow> rf2;
            switch (cls)
            {
                case "已處理":
                    rf2 = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DELIVFLOW WHERE STATUS ='?'")
                        .Where(m => m.Userid != WebSecurity.CurrentUserId).ToList();
                    if (!Roles.IsUserInRole("Usual"))
                    {
                        rf.AddRange(rf2);
                    }
                    else
                    {
                        foreach (DelivFlow f in rf2)
                        {
                            if (db.DelivFlows.Where(m => m.Docid == f.Docid).Where(m => m.Userid == WebSecurity.CurrentUserId).Count() > 0)
                            {
                                rf.Add(f);
                            }
                        }
                    }

                    break;
                case "已結案":
                    rf2 = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DELIVFLOW WHERE STATUS ='2'").ToList();
                    if (!Roles.IsUserInRole("Usual"))
                    {
                        rf.AddRange(rf2);
                    }
                    else
                    {
                        foreach (DelivFlow f in rf2)
                        {
                            if (db.DelivFlows.Where(m => m.Docid == f.Docid).Where(m => m.Userid == WebSecurity.CurrentUserId).Count() > 0)
                            {
                                rf.Add(f);
                            }
                        }
                    }
                    break;
                default:
                    rf = db.Database.SqlQuery<DelivFlow>("SELECT * FROM DELIVFLOW WHERE STATUS ='?'")
                        .Where(m => m.Userid == WebSecurity.CurrentUserId).ToList();
                    break;
            }
            rf.OrderByDescending(m => m.Rtt);
            DeliveryListVModel i;
            foreach (DelivFlow f in rf)
            {
                Delivery r = db.Deliveries.Find(f.Docid);
                AppUser p = db.AppUsers.Find(r.UserId);
                //CustOrgan c = db.CustOrgans.Find(p.CustId);
                //BuyEvaluate b = db.BuyEvaluates.Find(r.PurchaseNo);
                i = new DeliveryListVModel();
                i.DocType = "驗收";
                i.Docid = r.Docid;
                i.UserId = r.UserId;
                i.UserName = r.UserName;
                //if (p != null && p.CustId != null)
                //{
                //    i.CustId = p.CustId;
                //    i.CustNam = c.CustNam;
                //}
                i.ContractNo = r.ContractNo;
                i.PurchaseNo = r.PurchaseNo;
                i.CrlItemNo = r.CrlItemNo;
                i.AccDpt = r.AccDpt;
                i.AccDptNam = db.CustOrgans.Find(r.AccDpt) == null ? "" : db.CustOrgans.Find(r.AccDpt).CustNam;
                //if (b != null)
                //    i.BudgetId = b.BudgetId;
                //else
                    i.BudgetId = "";
                if (f.Status == "?")
                    i.Days = DateTime.Now.Subtract(r.ApplyDate.GetValueOrDefault()).Days;
                else
                    i.Days = null;
                i.Flg = f.Status;
                i.FlowUid = f.Userid;
                i.VendorNo = r.VendorId;
                dv.Add(i);
            }
            //
            return dv;
        }
    }
}