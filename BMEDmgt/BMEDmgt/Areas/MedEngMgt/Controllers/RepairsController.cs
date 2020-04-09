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
using WebMatrix.WebData;
using System.Web.Security;
using BMEDmgt.Filters;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.IO;
using PagedList;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class RepairsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 100;
        public ActionResult Index()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "待處理", Value = "待處理"});
            listItem.Add(new SelectListItem { Text = "已處理", Value = "已處理" });
            listItem.Add(new SelectListItem { Text = "已結案", Value = "已結案" });
            ViewData["FLOWTYPE"] = new SelectList(listItem, "Value", "Text", "待處理");
            //
            List<SelectListItem> listItem3 = new List<SelectListItem>();
            db.DealStatus.Where(d => d.Flg == "Y").ToList()
                .ForEach(d =>
                {
                    listItem3.Add(new SelectListItem {Text = d.Title, Value = d.Title });
                });
            ViewData["DEALSTATUS"] = new SelectList(listItem3, "Value", "Text");
            //
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
            return PartialView();
        }

        [HttpPost]
        public ActionResult Index(FormCollection fm)
        {
            string ftype = fm["qtyFLOWTYPE"];
            string aname = fm["qtyASSETNAME"];
            string ano = fm["qtyASSETNO"];
            string acc = fm["qtyACCDPT"];
            string docid = fm["qtyDOCID"];
            string dptid = fm["qtyDPTID"];
            string dealstatus = fm["qtyDEALSTATUS"];
            
            List<RepairListVModel> rv = new List<RepairListVModel>();
            AppUser usr = db.AppUsers.Find(WebSecurity.CurrentUserId);
            var vendors = db.Assets.Join(db.Vendors, a => a.VendorId, v => v.VendorId,
               (a, v) => new {
                   ano = a.AssetNo,
                   vname = v.VendorName
               });
            Department c;
            switch (ftype)
            {
                case "已處理":
                    db.RepairFlows.Where(f => f.Status == "?")
 .Join(db.RepairFlows.Where(f2 => f2.UserId == WebSecurity.CurrentUserId && f2.Status == "1"),
 f => f.DocId, f2 => f2.DocId, (f, f2) => f)
                .Select(f => new
                {
                    f.DocId,
                    f.UserId,
                    f.Status,
                    f.Cls
                }).Distinct()
                .Join(db.Repairs, f => f.DocId, r => r.DocId,
                (f, r) => new
                {
                    repair = r,
                    flow = f
                })
                .Join(db.RepairDtls, m => m.repair.DocId, d => d.DocId,
                (m, d) => new
                {
                    repair = m.repair,
                    flow = m.flow,
                    repdtl = d
                }).Join(db.Departments, j => j.repair.AccDpt, d => d.DptId,
                    (j, d) => new {
                        repair = j.repair,
                        flow = j.flow,
                        repdtl = j.repdtl,
                        dpt = d
                    }).GroupJoin(vendors, j => j.repair.AssetNo, v => v.ano,
                    (j, v) => new {
                        j = j,
                        v = v
                    }).SelectMany(v => v.v.DefaultIfEmpty(),
                    (x, y) => new { x, y }).ToList()
                    .ForEach(j => rv.Add(new RepairListVModel
                    {
                        DocType = "請修",
                        DocId = j.x.j.repair.DocId,
                        AssetNo = j.x.j.repair.AssetNo,
                        AssetName = j.x.j.repair.AssetName,
                        ApplyDpt = j.x.j.repair.DptId,
                        AccDpt = j.x.j.repair.AccDpt,
                        AccDptName = j.x.j.dpt.Name_C,
                        TroubleDes = j.x.j.repair.TroubleDes,
                        DealState = j.x.j.repdtl.DealState,
                        DealDes = j.x.j.repdtl.DealDes,
                        Cost = j.x.j.repdtl.Cost,
                        Days = DateTime.Now.Subtract(j.x.j.repair.ApplyDate.GetValueOrDefault()).Days,
                        Flg = j.x.j.flow.Status,
                        FlowUid = j.x.j.flow.UserId,
                        FlowCls = j.x.j.flow.Cls,
                        Vendor = j.y == null ? "" : j.y.vname
                    }));
                   
                    break;
                case "已結案":
                    List<RepairFlow> rf = db.RepairFlows.Where(f => f.Status == "2").ToList();
                    if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Manager"))
                    {
                        if (Roles.IsUserInRole("Manager"))
                        {
                            rf = rf.Join(db.Repairs.Where(r => r.AccDpt == usr.DptId),
                                f => f.DocId, r => r.DocId, (f, r) => f).ToList();
                        }
                    }
                    else
                    {
                        rf = rf.Join(db.RepairFlows.Where(f2 => f2.UserId == WebSecurity.CurrentUserId),
                             f => f.DocId, f2 => f2.DocId, (f, f2) => f).ToList();
                    }

                    rf.Select(f => new
                    {
                        f.DocId,
                        f.UserId,
                        f.Status,
                        f.Cls
                    }).Distinct().Join(db.Repairs, f => f.DocId, r => r.DocId,
               (f, r) => new
               {
                   repair = r,
                   flow = f
               })
               .Join(db.RepairDtls, m => m.repair.DocId, d => d.DocId,
               (m, d) => new
               {
                   repair = m.repair,
                   flow = m.flow,
                   repdtl = d
               }).Join(db.Departments, j => j.repair.AccDpt, d => d.DptId,
                    (j, d) => new {
                        repair = j.repair,
                        flow = j.flow,
                        repdtl = j.repdtl,
                        dpt = d
                    }).GroupJoin(vendors, j => j.repair.AssetNo, v => v.ano,
                    (j, v) => new {
                        j = j,
                        v = v
                    }).SelectMany(v => v.v.DefaultIfEmpty(),
                    (x, y) => new { x, y }).ToList()
                    .ForEach(j => rv.Add(new RepairListVModel
                    {
                        DocType = "請修",
                        DocId = j.x.j.repair.DocId,
                        AssetNo = j.x.j.repair.AssetNo,
                        AssetName = j.x.j.repair.AssetName,
                        ApplyDpt = j.x.j.repair.DptId,
                        AccDpt = j.x.j.repair.AccDpt,
                        AccDptName = j.x.j.dpt.Name_C,
                        TroubleDes = j.x.j.repair.TroubleDes,
                        DealState = j.x.j.repdtl.DealState,
                        DealDes = j.x.j.repdtl.DealDes,
                        Cost = j.x.j.repdtl.Cost,
                        Days = DateTime.Now.Subtract(j.x.j.repair.ApplyDate.GetValueOrDefault()).Days,
                        Flg = j.x.j.flow.Status,
                        FlowUid = j.x.j.flow.UserId,
                        FlowCls = j.x.j.flow.Cls,
                        Vendor = j.y == null ? "" : j.y.vname
                    }));
                    break;
                case "待處理":
                    db.RepairFlows.Where(f => f.Status == "?" && f.UserId == WebSecurity.CurrentUserId)
                .Select(f => new
                {
                    f.DocId,
                    f.UserId,
                    f.Status,
                    f.Cls
                }).Join(db.Repairs, f => f.DocId, r => r.DocId,
                (f, r) => new
                {
                    repair = r,
                    flow = f
                })
                .Join(db.RepairDtls, m => m.repair.DocId, d => d.DocId,
                (m, d) => new
                {
                    repair = m.repair,
                    flow = m.flow,
                    repdtl = d
                }).Join(db.Departments, j => j.repair.AccDpt, d => d.DptId,
                    (j, d) => new {
                        repair = j.repair,
                        flow = j.flow,
                        repdtl = j.repdtl,
                        dpt = d
                    }).GroupJoin(vendors, j => j.repair.AssetNo, v => v.ano,
                    (j, v) => new {
                        j = j,
                        v = v
                    }).SelectMany(v => v.v.DefaultIfEmpty(),
                    (x, y) => new { x, y}).ToList()
                    .ForEach(j => rv.Add(new RepairListVModel
                    {
                        DocType = "請修",
                        DocId = j.x.j.repair.DocId,
                        AssetNo = j.x.j.repair.AssetNo,
                        AssetName = j.x.j.repair.AssetName,
                        ApplyDpt = j.x.j.repair.DptId,
                        AccDpt = j.x.j.repair.AccDpt,
                        AccDptName = j.x.j.dpt.Name_C,
                        TroubleDes = j.x.j.repair.TroubleDes,
                        DealState = j.x.j.repdtl.DealState,
                        DealDes = j.x.j.repdtl.DealDes,
                        Cost = j.x.j.repdtl.Cost,
                        Days = DateTime.Now.Subtract(j.x.j.repair.ApplyDate.GetValueOrDefault()).Days,
                        Flg = j.x.j.flow.Status,
                        FlowUid = j.x.j.flow.UserId,
                        FlowCls = j.x.j.flow.Cls,
                        Vendor = j.y == null ? "" : j.y.vname
                    }));
                    break;
            };


            if (!string.IsNullOrEmpty(docid))
            {
                rv = rv.Where(v => v.DocId == docid).ToList();
            }
            if (!string.IsNullOrEmpty(ano))
            {
                rv = rv.Where(v => v.AssetNo == ano).ToList();
            }
            if (!string.IsNullOrEmpty(dptid))
            {
                rv = rv.Where(v => v.ApplyDpt == dptid).ToList();
            }
            if (!string.IsNullOrEmpty(acc))
            {
                rv = rv.Where(v => v.AccDpt == acc).ToList();
            }
            if (!string.IsNullOrEmpty(aname))
            {
                rv = rv.Where(v => v.AssetName.Contains(aname)).ToList();
            }
            if (!string.IsNullOrEmpty(dealstatus))
            {
                rv = rv.Where(v => v.DealState == dealstatus).ToList();
            }
            rv = rv.OrderByDescending(k => k.DocId).ToList();

            return PartialView("List", rv);
        }

        public ActionResult Index2()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            //listItem.Add(new SelectListItem { Text = "待處理", Value = "待處理" });
            listItem.Add(new SelectListItem { Text = "所有", Value = "所有" });
            listItem.Add(new SelectListItem { Text = "已處理", Value = "已處理" });
            listItem.Add(new SelectListItem { Text = "已結案", Value = "已結案" });
            ViewData["FLOWTYP"] = new SelectList(listItem, "Value", "Text", "所有");
            //
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
            if (Roles.IsUserInRole("Usual"))
            {
                AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                listItem2.Clear();
                if (u != null)
                {
                    li = new SelectListItem();
                    li.Text = db.Departments.Find(u.DptId).Name_C;
                    li.Value = u.DptId;
                    listItem2.Add(li);
                    ViewData["APPLYDPT"] = new SelectList(listItem2, "Value", "Text", u.DptId);
                }
            }
            else
            {
                ViewData["APPLYDPT"] = new SelectList(listItem2, "Value", "Text");
            }

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Text = "醫療儀器", Value = "醫療儀器" });
            listItem3.Add(new SelectListItem { Text = "雜項", Value = "雜項" });
            listItem3.Add(new SelectListItem { Text = "水電", Value = "水電" });
            listItem3.Add(new SelectListItem { Text = "空調", Value = "空調" });
            listItem3.Add(new SelectListItem { Text = "消防", Value = "消防" });
            listItem3.Add(new SelectListItem { Text = "護士呼叫", Value = "護士呼叫" });
            listItem3.Add(new SelectListItem { Text = "病床", Value = "病床" });
            listItem3.Add(new SelectListItem { Text = "氣體", Value = "氣體" });
            listItem3.Add(new SelectListItem { Text = "氣送", Value = "氣送" });
            ViewData["PLANTCLASS"] = new SelectList(listItem3, "Value", "Text");

            List<SelectListItem> listItem4 = new List<SelectListItem>();
            SelectListItem li2;
            Roles.GetUsersInRole("Engineer").ToList()
                        .ForEach(x =>
                        {
                            AppUser u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                li2 = new SelectListItem();
                                li2.Text = "(" + u.UserName + ")" + u.FullName;
                                li2.Value = u.Id.ToString();
                                listItem4.Add(li2);
                            }
                        });
            ViewData["ENGID"] = new SelectList(listItem4, "Value", "Text");

            List<SelectListItem> listItem5 = new List<SelectListItem>();
            listItem5.Add(new SelectListItem { Text = "蘭陽", Value = "蘭陽" });
            listItem5.Add(new SelectListItem { Text = "新民", Value = "新民" });
            ViewData["REPAIRAREA"] = new SelectList(listItem5, "Value", "Text");

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
            string typ = fm["qtyTYPE"];
            string sd = fm["Sdate"];
            string ed = fm["Edate"];
            DateTime? sdate = null, edate = null;
            string plantClass = fm["qtyPlantClass"];
            string troubleDes = fm["qtyTroubleDes"];
            string qtyEngId = fm["qtyEngId"];
            string repairArea = fm["qtyRepairArea"];
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
            List<RepairListVModel> rv = new List<RepairListVModel>();
            if (string.IsNullOrEmpty(aname) && string.IsNullOrEmpty(ano) &&
                string.IsNullOrEmpty(acc) && string.IsNullOrEmpty(docid) &&
                string.IsNullOrEmpty(dptid) && string.IsNullOrEmpty(typ) &&
                string.IsNullOrEmpty(plantClass) && string.IsNullOrEmpty(troubleDes) &&
                string.IsNullOrEmpty(qtyEngId) && string.IsNullOrEmpty(repairArea) &&
                sdate == null && edate == null)
            {
                throw new Exception("請輸入查詢條件!!");
            }
            else {
                List<Repair> rps = db.Repairs.ToList();
                foreach(var item in rps)
                {
                    var lastEngFlow = db.RepairFlows.Where(rf => rf.DocId == item.DocId)
                                                    .Where(rf => rf.Cls.Contains("工程師")).OrderByDescending(rf => rf.StepId).FirstOrDefault();
                    if (lastEngFlow != null)
                    {
                        item.EngId = lastEngFlow.UserId;
                    }
                }
                if (Roles.IsUserInRole("Manager"))  //單位主管可查詢單位請修案
                {
                    AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    if (u != null)
                        rps = rps.Where(r => r.DptId == u.DptId)
                            .Union(db.Repairs.Where(r => r.AccDpt == u.DptId))
                            .Distinct().ToList();
                    else
                        throw new Exception("無部門資料!!");
                }
                if (Roles.IsUserInRole("Usual"))  //一般使用者可查詢自己申請之案件
                {
                    AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                    if (u != null)
                        rps = rps.Where(r => r.UserId == u.Id).ToList();
                    else
                        throw new Exception("查無人員!!");
                }
                //if (Roles.IsUserInRole("Usual"))
                //{
                //    AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                //    if (u != null)
                //        rps = rps.Where(r => r.DptId == u.DptId)
                //            .Union(db.Repairs.Where(r => r.AccDpt == u.DptId))
                //            .Distinct().ToList();
                //    else
                //        throw new Exception("無部門資料!!");
                //}
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
                if (!string.IsNullOrEmpty(plantClass))
                    rps = rps.Where(r => r.PlantClass == plantClass).ToList();
                if (!string.IsNullOrEmpty(troubleDes))
                    rps = rps.Where(r => r.TroubleDes.Contains(troubleDes)).ToList();
                if (!string.IsNullOrEmpty(qtyEngId))
                {
                    int tempEngId = Convert.ToInt32(qtyEngId);
                    rps = rps.Where(r => r.EngId == tempEngId).ToList();
                }
                if (!string.IsNullOrEmpty(repairArea))
                    rps = rps.Where(r => r.RepairArea == repairArea).ToList();
                if (!string.IsNullOrEmpty(typ))
                {
                    if (!string.IsNullOrEmpty(aname))
                    {
                        rps = rps.Union(db.Repairs.Join(db.Assets.Where(a => a.Type == typ), r => r.AssetNo, a => a.AssetNo,
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
                    rps = rps.Where(r => r.ApplyDate >= sdate.Value).ToList();
                }
                if (edate.HasValue)
                {
                    rps = rps.Where(r => r.ApplyDate <= edate.Value).ToList();
                }
                var ss = new[] { "?", "2" };
                rps.Join(db.RepairDtls, r => r.DocId, d => d.DocId,
                    (r, d) => new {
                        repair = r,
                        dtl = d
                    }).Join(db.RepairFlows.Where(f => ss.Contains(f.Status)),  m => m.repair.DocId, f => f.DocId,
                    (m, f) => new
                    {
                        repair = m.repair,
                        flow = f,
                        repdtl = m.dtl
                    }).Join(db.Departments, j => j.repair.AccDpt, d => d.DptId,
                    (j, d) => new {
                        repair = j.repair,
                        flow = j.flow,
                        repdtl = j.repdtl,
                        dpt = d
                    }).ToList()
                    .ForEach(j => rv.Add(new RepairListVModel
                    {
                        DocType = "請修",
                        DocId = j.repair.DocId,
                        AssetNo = j.repair.AssetNo,
                        AssetName = j.repair.AssetName,
                        ApplyDpt = j.repair.DptId,
                        AccDpt = j.repair.AccDpt,
                        AccDptName = j.dpt.Name_C,
                        TroubleDes = j.repair.TroubleDes,
                        DealState = j.repdtl.DealState,
                        DealDes = j.repdtl.DealDes,
                        Cost = j.repdtl.Cost,
                        Days = DateTime.Now.Subtract(j.repair.ApplyDate.GetValueOrDefault()).Days,
                        Flg = j.flow.Status,
                        FlowUid = j.flow.UserId,
                        FlowCls = j.flow.Cls
                    }));
            }
            if (flw == "已處理")
                rv = rv.Where(r => r.Flg == "?").ToList();
            else if (flw == "已結案")
                rv = rv.Where(r => r.Flg == "2").ToList();
            else if (flw == "所有")
                rv = rv.ToList();
            return PartialView("List2", rv.OrderByDescending(r => r.DocId));
        }

        public void ExcelRepData(List<string> rv)
        {
            var ss = new[] { "?", "2" };
            List<RepairListVModel> rv2 = new List<RepairListVModel>();
            DataTable dt = new DataTable();
            DataRow dw;
            dt.Columns.Add("類別");
            dt.Columns.Add("表單編號");
            dt.Columns.Add("成本中心名稱");
            dt.Columns.Add("財產編號");
            dt.Columns.Add("儀器名稱");
            dt.Columns.Add("故障描述");
            dt.Columns.Add("處理狀態");
            dt.Columns.Add("處理描述");
            dt.Columns.Add("費用");
            dt.Columns.Add("天數");
            //
            rv.Join(db.Repairs, r => r, d => d.DocId,
                    (r, p) => p)
                    .Join(db.RepairDtls, r => r.DocId, d => d.DocId,
                    (r, d) => new {
                        repair = r,
                        dtl = d
                    }).Join(db.RepairFlows.Where(f => ss.Contains(f.Status)), m => m.repair.DocId, f => f.DocId,
                    (m, f) => new
                    {
                        repair = m.repair,
                        flow = f,
                        repdtl = m.dtl
                    }).Join(db.Departments, j => j.repair.AccDpt, d => d.DptId,
                    (j, d) => new {
                        repair = j.repair,
                        flow = j.flow,
                        repdtl = j.repdtl,
                        dpt = d
                    }).ToList()
                    .ForEach(j => 
                    {
                        dw = dt.NewRow();
                        dw[0] = "請修";
                        dw[1] = j.repair.DocId;
                        //AssetNo = j.repair.AssetNo,
                        dw[4] = j.repair.AssetName;
                        dw[3] = j.repair.AssetNo;
                        //ApplyDpt = j.repair.DptId,
                        //AccDpt = j.repair.AccDpt,
                        dw[2] = j.dpt.Name_C;
                        dw[5] = j.repair.TroubleDes;
                        dw[6] = j.repdtl.DealState;
                        dw[7] = j.repdtl.DealDes;
                        dw[8] = j.repdtl.Cost;
                        dw[9] = DateTime.Now.Subtract(j.repair.ApplyDate.GetValueOrDefault()).Days;
                        //Flg = j.flow.Status,
                        //FlowUid = j.flow.UserId,
                        //FlowCls = j.flow.Cls
                        dt.Rows.Add(dw);
                    });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("請修案件查詢");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=RepairQryData.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
        //public ActionResult List()
        //{
        //    List<RepairListVModel> rv = new List<RepairListVModel>();
        //    Department c;
        //    db.RepairFlows.Where(f => f.Status == "?")
        //        .Where(f => f.UserId == WebSecurity.CurrentUserId)
        //        .Select(f => new
        //        {
        //            f.DocId,
        //            f.UserId,
        //            f.Status,
        //            f.Cls
        //        })
        //        .Join(db.Repairs, f => f.DocId, r => r.DocId,
        //        (f, r) => new
        //        {
        //            repair = r,
        //            flow = f
        //        })
        //        .Join(db.RepairDtls, m => m.repair.DocId, d => d.DocId,
        //        (m, d) => new
        //        {
        //            repair = m.repair,
        //            flow = m.flow,
        //            repdtl = d
        //        }).ToList()
        //            .ForEach(j => rv.Add(new RepairListVModel
        //            {
        //                DocType = "請修",
        //                DocId = j.repair.DocId,
        //                AssetNo = j.repair.AssetNo,
        //                AssetName = j.repair.AssetName,
        //                AccDpt = j.repair.AccDpt,
        //                AccDptName = (c = db.Departments.Find(j.repair.AccDpt)) == null ? "" : c.Name_C,
        //                TroubleDes = j.repair.TroubleDes,
        //                DealState = j.repdtl.DealState,
        //                DealDes = j.repdtl.DealDes,
        //                Cost = j.repdtl.Cost,
        //                Days = DateTime.Now.Subtract(j.repair.ApplyDate.GetValueOrDefault()).Days,
        //                Flg = j.flow.Status,
        //                FlowUid = j.flow.UserId,
        //                FlowCls = j.flow.Cls
        //            }));

        //    return PartialView(rv);
        //}

        // GET: MedEngMgt/Repairs/Details/5

        [HttpPost]
        public ActionResult List(FormCollection fm, int page = 1)
        {
            string ftype = fm["qtyFLOWTYPE"];
            string aname = fm["qtyASSETNAME"];
            string ano = fm["qtyASSETNO"];
            string acc = fm["qtyACCDPT"];
            string docid = fm["qtyDOCID"];
            string dptid = fm["qtyDPTID"];
            string dealstatus = fm["qtyDEALSTATUS"];
            string otherDoc = fm["qtyOTHERDOC"];

            List<RepairListVModel> rv = new List<RepairListVModel>();
            AppUser usr = db.AppUsers.Find(WebSecurity.CurrentUserId);
            var vendors = db.Assets.Join(db.Vendors, a => a.VendorId, v => v.VendorId,
               (a, v) => new {
                   ano = a.AssetNo,
                   vname = v.VendorName
               });
            Department c;
            switch (ftype)
            {
                case "已處理":
                    db.RepairFlows.Where(f => f.Status == "?")
 .Join(db.RepairFlows.Where(f2 => f2.UserId == WebSecurity.CurrentUserId && f2.Status == "1"),
 f => f.DocId, f2 => f2.DocId, (f, f2) => f)
                .Select(f => new
                {
                    f.DocId,
                    f.UserId,
                    f.Status,
                    f.Cls
                }).Distinct()
                .Join(db.Repairs, f => f.DocId, r => r.DocId,
                (f, r) => new
                {
                    repair = r,
                    flow = f
                })
                .Join(db.RepairDtls, m => m.repair.DocId, d => d.DocId,
                (m, d) => new
                {
                    repair = m.repair,
                    flow = m.flow,
                    repdtl = d
                }).Join(db.Departments, j => j.repair.AccDpt, d => d.DptId,
                    (j, d) => new {
                        repair = j.repair,
                        flow = j.flow,
                        repdtl = j.repdtl,
                        dpt = d
                    }).GroupJoin(vendors, j => j.repair.AssetNo, v => v.ano,
                    (j, v) => new {
                        j = j,
                        v = v
                    }).SelectMany(v => v.v.DefaultIfEmpty(),
                    (x, y) => new { x, y }).ToList()
                    .ForEach(j => rv.Add(new RepairListVModel
                    {
                        DocType = "請修",
                        DocId = j.x.j.repair.DocId,
                        AssetNo = j.x.j.repair.AssetNo,
                        AssetName = j.x.j.repair.AssetName,
                        ApplyDpt = j.x.j.repair.DptId,
                        AccDpt = j.x.j.repair.AccDpt,
                        AccDptName = j.x.j.dpt.Name_C,
                        TroubleDes = j.x.j.repair.TroubleDes,
                        DealState = j.x.j.repdtl.DealState,
                        DealDes = j.x.j.repdtl.DealDes,
                        Cost = j.x.j.repdtl.Cost,
                        Days = DateTime.Now.Subtract(j.x.j.repair.ApplyDate.GetValueOrDefault()).Days,
                        Flg = j.x.j.flow.Status,
                        FlowUid = j.x.j.flow.UserId,
                        FlowCls = j.x.j.flow.Cls,
                        Vendor = j.y == null ? "" : j.y.vname
                    }));

                    break;
                case "已結案":
                    List<RepairFlow> rf = db.RepairFlows.Where(f => f.Status == "2").ToList();
                    if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Manager"))
                    {
                        if (Roles.IsUserInRole("Manager"))
                        {
                            rf = rf.Join(db.Repairs.Where(r => r.AccDpt == usr.DptId),
                                f => f.DocId, r => r.DocId, (f, r) => f).ToList();
                        }
                    }
                    else
                    {
                        rf = rf.Join(db.RepairFlows.Where(f2 => f2.UserId == WebSecurity.CurrentUserId),
                             f => f.DocId, f2 => f2.DocId, (f, f2) => f).ToList();
                    }

                    rf.Select(f => new
                    {
                        f.DocId,
                        f.UserId,
                        f.Status,
                        f.Cls
                    }).Distinct().Join(db.Repairs, f => f.DocId, r => r.DocId,
               (f, r) => new
               {
                   repair = r,
                   flow = f
               })
               .Join(db.RepairDtls, m => m.repair.DocId, d => d.DocId,
               (m, d) => new
               {
                   repair = m.repair,
                   flow = m.flow,
                   repdtl = d
               }).Join(db.Departments, j => j.repair.AccDpt, d => d.DptId,
                    (j, d) => new {
                        repair = j.repair,
                        flow = j.flow,
                        repdtl = j.repdtl,
                        dpt = d
                    }).GroupJoin(vendors, j => j.repair.AssetNo, v => v.ano,
                    (j, v) => new {
                        j = j,
                        v = v
                    }).SelectMany(v => v.v.DefaultIfEmpty(),
                    (x, y) => new { x, y }).ToList()
                    .ForEach(j => rv.Add(new RepairListVModel
                    {
                        DocType = "請修",
                        DocId = j.x.j.repair.DocId,
                        AssetNo = j.x.j.repair.AssetNo,
                        AssetName = j.x.j.repair.AssetName,
                        ApplyDpt = j.x.j.repair.DptId,
                        AccDpt = j.x.j.repair.AccDpt,
                        AccDptName = j.x.j.dpt.Name_C,
                        TroubleDes = j.x.j.repair.TroubleDes,
                        DealState = j.x.j.repdtl.DealState,
                        DealDes = j.x.j.repdtl.DealDes,
                        Cost = j.x.j.repdtl.Cost,
                        Days = DateTime.Now.Subtract(j.x.j.repair.ApplyDate.GetValueOrDefault()).Days,
                        Flg = j.x.j.flow.Status,
                        FlowUid = j.x.j.flow.UserId,
                        FlowCls = j.x.j.flow.Cls,
                        Vendor = j.y == null ? "" : j.y.vname
                    }));
                    break;
                case "待處理":
                    var repairFlows = db.RepairFlows.ToList();
                    if (otherDoc == null ? false : otherDoc.Contains("true"))
                    {
                        var ur = db.AppUsers.Where(a => a.UserName == "eao").FirstOrDefault();
                        repairFlows = repairFlows.Where(f => f.Status == "?" && f.UserId == ur.Id).ToList();
                    }
                    else
                    {
                        repairFlows = repairFlows.Where(f => f.Status == "?" && f.UserId == WebSecurity.CurrentUserId).ToList();
                    }
                repairFlows.Select(f => new
                {
                    f.DocId,
                    f.UserId,
                    f.Status,
                    f.Cls
                }).Join(db.Repairs, f => f.DocId, r => r.DocId,
                (f, r) => new
                {
                    repair = r,
                    flow = f
                })
                .Join(db.RepairDtls, m => m.repair.DocId, d => d.DocId,
                (m, d) => new
                {
                    repair = m.repair,
                    flow = m.flow,
                    repdtl = d
                }).Join(db.Departments, j => j.repair.AccDpt, d => d.DptId,
                    (j, d) => new {
                        repair = j.repair,
                        flow = j.flow,
                        repdtl = j.repdtl,
                        dpt = d
                    }).GroupJoin(vendors, j => j.repair.AssetNo, v => v.ano,
                    (j, v) => new {
                        j = j,
                        v = v
                    }).SelectMany(v => v.v.DefaultIfEmpty(),
                    (x, y) => new { x, y }).ToList()
                    .ForEach(j => rv.Add(new RepairListVModel
                    {
                        DocType = "請修",
                        DocId = j.x.j.repair.DocId,
                        AssetNo = j.x.j.repair.AssetNo,
                        AssetName = j.x.j.repair.AssetName,
                        ApplyDpt = j.x.j.repair.DptId,
                        AccDpt = j.x.j.repair.AccDpt,
                        AccDptName = j.x.j.dpt.Name_C,
                        TroubleDes = j.x.j.repair.TroubleDes,
                        DealState = j.x.j.repdtl.DealState,
                        DealDes = j.x.j.repdtl.DealDes,
                        Cost = j.x.j.repdtl.Cost,
                        Days = DateTime.Now.Subtract(j.x.j.repair.ApplyDate.GetValueOrDefault()).Days,
                        Flg = j.x.j.flow.Status,
                        FlowUid = j.x.j.flow.UserId,
                        FlowCls = j.x.j.flow.Cls,
                        Vendor = j.y == null ? "" : j.y.vname,
                        PlantClass = j.x.j.repair.PlantClass
                    }));
                    break;
            };


            if (!string.IsNullOrEmpty(docid))
            {
                rv = rv.Where(v => v.DocId == docid).ToList();
            }
            if (!string.IsNullOrEmpty(ano))
            {
                rv = rv.Where(v => v.AssetNo == ano).ToList();
            }
            if (!string.IsNullOrEmpty(dptid))
            {
                rv = rv.Where(v => v.ApplyDpt == dptid).ToList();
            }
            if (!string.IsNullOrEmpty(acc))
            {
                rv = rv.Where(v => v.AccDpt == acc).ToList();
            }
            if (!string.IsNullOrEmpty(aname))
            {
                rv = rv.Where(v => v.AssetName.Contains(aname)).ToList();
            }
            if (!string.IsNullOrEmpty(dealstatus))
            {
                rv = rv.Where(v => v.DealState == dealstatus).ToList();
            }
            rv = rv.OrderByDescending(k => k.DocId).ToList();

            if (rv.ToPagedList(page, pageSize).Count <= 0)
                return PartialView(rv.ToPagedList(1, pageSize));

            return PartialView(rv.ToPagedList(page, pageSize));
        }
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                Repair r = new Repair();
                r.DocId = "000";
                r.UserName = "宋大衛";
                return PartialView(r);
            }
            Repair repair = db.Repairs.Find(id);
            if (repair == null)
            {
                return HttpNotFound();
            }
            repair.DptName = db.Departments.Find(repair.DptId).Name_C;
            repair.AccDptName = db.Departments.Find(repair.AccDpt).Name_C;
            Asset asset = db.Assets.Find(repair.AssetNo);
            if (asset != null)
            {
                repair.BuyDate = asset.BuyDate;
                repair.Cost = asset.Cost;
            }
            return PartialView(repair);
        }

        // GET: MedEngMgt/Repairs/Views/5
        public ActionResult Views(string id)
        {
            Repair repair = db.Repairs.Find(id);
            if (repair == null)
            {
                return HttpNotFound();
            }
            return View(repair);
        }

        // GET: MedEngMgt/Repairs/Create
        public ActionResult Create()
        {
            Repair r = new Repair();
            AppUser usr = null;
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
            r.ApplyDate = DateTime.Now;
            r.DptId = d == null ? "" : d.DptId;
            r.DptName = d == null ? "" : d.Name_C;
            r.AccDpt = d == null ? "" : d.DptId;
            r.AccDptName = d == null ? "" : d.Name_C;
            r.Contact = u.Ext == null ? "" : u.Ext;
            r.ApplyDate = DateTime.Now;
            r.CheckerId = u.Id;
            r.CheckerName = u.FullName;
            r.Amt = 1;
            //
            Roles.GetUsersInRole("Manager").ToList()
                 .ForEach(x =>
                 {
                     usr = db.AppUsers.Find(WebSecurity.GetUserId(x));
                     if (u.DptId == usr.DptId)
                     {
                         r.CheckerId = usr.Id;
                         r.CheckerName = usr.FullName;
                     }
                 });
            //
            string str = "insert into repair(DocId,UserId,UserName,ApplyDate,DptId,Contact,AccDpt,Amt,CheckerId";
            str += ") values(@1,@2,@3,@4,@5,@6,@7,@8,@9)";
            db.Database.ExecuteSqlCommand(str, 
                new SqlParameter("@1",r.DocId),
                new SqlParameter("@2", r.UserId),
                new SqlParameter("@3", r.UserName),
                new SqlParameter("@4", r.ApplyDate),
                new SqlParameter("@5", r.DptId),
                new SqlParameter("@6", r.Contact),
                new SqlParameter("@7", r.AccDpt),
                new SqlParameter("@8", r.Amt),
                new SqlParameter("@9", r.CheckerId));
            db.SaveChanges();
            //
            //r.TroubleDes = "";
            //r.PlantClass = "";
            //r.RepType = "";
            //r.AssetNo = "";
            //r.AssetName = "";
            //r.PlaceLoc = "";
            //db.Repairs.Add(r);
            //
            RepairDtl dl = new RepairDtl();
            dl.DocId = r.DocId;
            db.RepairDtls.Add(dl);
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
            List<AppUser> ulist = null;
            if (u.DptId != null)
                ulist = db.AppUsers.Where(ur => ur.DptId == u.DptId && ur.Status == "Y").ToList();
            else if (u.VendorId > 0)
                ulist = db.AppUsers.Where(ur => ur.VendorId == u.VendorId && ur.Status == "Y").ToList();
            ulist = ulist.OrderBy(ul => ul.UserName).ToList();
            for (int i = 0; i < ulist.Count; i++)
            {
                if (ulist[i] != null)
                {
                    if (ulist[i].Id == WebSecurity.CurrentUserId)
                    {
                        listItem.Add(new SelectListItem
                        {
                            Text = "(" + ulist[i].UserName + ")" + ulist[i].FullName,
                            Value = ulist[i].Id.ToString(),
                            Selected = true
                        });
                    }
                    else
                    {
                        listItem.Add(new SelectListItem
                        {
                            Text = "(" + ulist[i].UserName + ")" + ulist[i].FullName,
                            Value = ulist[i].Id.ToString()
                        });
                    }
                }
            }
            if (usr != null)
            {
                ViewData["CheckerName"] = new SelectList(listItem, "Value", "Text", usr.Id);
            }
            else
            {
                ViewData["CheckerName"] = new SelectList(listItem, "Value", "Text", u.Id);
            }       
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

            List<SelectListItem> assetNameList = new List<SelectListItem>();
            db.NoAssetNoLists.ToList()
                .ForEach(an =>
                {
                    assetNameList.Add(new SelectListItem
                    {
                        Value = an.Title,
                        Text = an.Title,
                        Selected = false
                    });
                });
            assetNameList.Add(new SelectListItem { Value = "000", Text = "其他類", Selected = false});
            ViewData["assetNameList"] = assetNameList;

            var repairAreas = db.RepairAreas.ToList();
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "蘭陽", Value = "蘭陽" });
            listItem2.Add(new SelectListItem { Text = "新民", Value = "新民" });
            if (repairAreas.Count() > 0)
            {
                listItem2.Clear();
                foreach(var item in repairAreas)
                {
                    listItem2.Add(new SelectListItem { Text = item.AreaName, Value = item.AreaName });
                }
            }
            ViewData["RepairArea"] = new SelectList(listItem2, "Value", "Text");

            return View(r);
        }

        // POST: MedEngMgt/Repairs/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Create(Repair repair)
        {
            if (repair.RepType == "送修")
            {
                ModelState.Remove("PlaceLoc");
                if (string.IsNullOrEmpty(repair.PlaceLoc))
                {
                    repair.PlaceLoc = "送修無須填寫";
                }
            }

            if (ModelState.IsValid)
            {

                //更新申請人的Email
                if (string.IsNullOrEmpty(repair.Email))
                {
                    throw new Exception("電子信箱不可空白!!");
                }
                AppUser a = db.AppUsers.Find(repair.UserId);
                a.Email = repair.Email;
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
                //
                AppUser u;
                u = db.AppUsers.Find(Convert.ToInt32(repair.CheckerName));
                if (u != null)
                {
                    repair.CheckerId = u.Id;
                    repair.CheckerName = u.FullName;
                }
                else
                {
                    throw new Exception("無[驗收人]資料!!");
                }
                Asset at;
                at = db.Assets.Find(repair.AssetNo);

                //repair.AccDpt = at.AccDpt;
                if (at != null)
                {
                    if (repair.AssetNo != "000" && repair.AssetNo != "001")    //AssetNo 000、001 為無財編對應的Asset
                    {
                        repair.AssetName = at.Cname;
                    }
                }
                repair.ApplyDate = DateTime.Now;
                db.Entry(repair).State = EntityState.Modified;
                //
                RepairFlow rf = new RepairFlow();
                rf.DocId = repair.DocId;
                rf.StepId = 1;
                rf.UserId = WebSecurity.CurrentUserId;
                rf.Status = "1";
                rf.Role = Roles.GetRolesForUser().FirstOrDefault();
                rf.Rtp = WebSecurity.CurrentUserId;
                rf.Rdt = null;
                rf.Rtt = DateTime.Now;
                rf.Cls = "申請者";
                db.RepairFlows.Add(rf);
                //
                AssetKeep kp = db.AssetKeeps.Find(repair.AssetNo);
                //if (kp == null)
                //{
                //    throw new Exception("無工程師資料!!");
                //}

                /* 如有代理人，將工程師改為代理人*/
                if (at != null)
                {
                    var subStaff = db.EngSubStaffs.SingleOrDefault(e => e.EngId == at.EngId);
                    if (subStaff != null)
                    {
                        int startDate = Convert.ToInt32(subStaff.StartDate.Value.ToString("yyyyMMdd"));
                        int endDate = Convert.ToInt32(subStaff.EndDate.Value.ToString("yyyyMMdd"));
                        int today = Convert.ToInt32(DateTime.UtcNow.AddHours(08).ToString("yyyyMMdd"));
                        /* 如在代理期間內，將代理人指定為負責工程師 */
                        if (today >= startDate && today <= endDate)
                        {
                            at.EngId = subStaff.SubstituteId;
                        }
                    }
                }

                /* 非醫療儀器的設備全送給雅雲 */
                if(repair.PlantClass != "醫療儀器")
                {
                    var tempEng = db.AppUsers.Where(ur => ur.UserName == "eao").FirstOrDefault();
                    at.EngId = tempEng.Id;
                }

                rf = new RepairFlow();
                rf.DocId = repair.DocId;
                rf.StepId = 2;
                rf.UserId = at == null ? WebSecurity.CurrentUserId : at.EngId;
                rf.Status = "?";
                u = db.AppUsers.Find(rf.UserId);
                if (u == null)
                {
                    u = db.AppUsers.Where(ur => ur.UserName == "16552").FirstOrDefault();
                    rf.UserId = db.AppUsers.Where(ur => ur.UserName == "16552").FirstOrDefault().Id;
                    //throw new Exception("無工程師資料!!");
                }
                rf.Role = Roles.GetRolesForUser(u.UserName).FirstOrDefault();
                rf.Rtp = null;
                rf.Rdt = null;
                rf.Rtt = DateTime.Now;
                rf.Cls = "設備工程師";
                db.RepairFlows.Add(rf);
                db.SaveChanges();
                //send mail
                Tmail mail = new Tmail();
                string body = "";
                u = db.AppUsers.Find(WebSecurity.CurrentUserId);
                mail.from = new System.Net.Mail.MailAddress(u.Email); //u.Email
                u = db.AppUsers.Find(rf.UserId);
                mail.to = new System.Net.Mail.MailAddress(u.Email); //u.Email
                //mail.cc.Add(new System.Net.Mail.MailAddress("16147@ymuh.ym.edu.tw"));
                mail.message.Subject = "醫療儀器管理資訊系統[請修案]：儀器名稱： " + repair.AssetName;
                body += "<p>申請人：" + repair.UserName + "</p>";
                body += "<p>財產編號：" + repair.AssetNo + "</p>";
                body += "<p>儀器名稱：" + repair.AssetName + "</p>";
                body += "<p>放置地點：" + repair.PlaceLoc + "</p>";
                body += "<p>故障描述：" + repair.TroubleDes + "</p>";
                //body += "<p>放置地點：" + repair.PlaceLoc + "</p>";
                body += "<p><a href='http://172.17.3.114/EMS'>處理案件</a></p>";
                body += "<br/>";
                body += "<h3>此封信件為系統通知郵件，請勿回覆。</h3>";
                mail.message.Body = body;
                mail.message.IsBodyHtml = true;
                mail.SendMail();

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

        // GET: MedEngMgt/Repairs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return View();
            }
            Repair repair = db.Repairs.Find(id);
            if (!(Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Manager")))
            {

                RepairFlow rf = db.RepairFlows.Where(f => f.DocId == id && f.Status == "?").FirstOrDefault();
                if (rf != null)
                {
                    if (rf.UserId != WebSecurity.CurrentUserId)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            if (repair == null)
            {
                return HttpNotFound();
            }

            ViewData["showCost"] = "Edit";

            RepairDtl dtl = db.RepairDtls.Find(id);
            if (dtl != null)
            {
                if (dtl.IsCharged == "Y" && dtl.EndDate != null)
                {
                    ViewData["showCost"] = "Print";
                }
            }
            return View(repair);
        }

        // POST: MedEngMgt/Repairs/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DocId,UserId,UserName,DptId,Contact,AccDpt,RepType,AssetNo,AssetName,Amt,PlantDoc,PlaceLoc,ApplyDate,TroubleDes,CheckerId,CheckerName,PlantClass")] Repair repair)
        {
            if (ModelState.IsValid)
            {
                db.Entry(repair).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(repair);
        }

        // GET: MedEngMgt/Repairs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Repair repair = db.Repairs.Find(id);
            if (repair == null)
            {
                return HttpNotFound();
            }
            return View(repair);
        }

        // POST: MedEngMgt/Repairs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            RepairFlow repflow = db.RepairFlows.Where(f => f.DocId == id && f.Status == "?")
                .FirstOrDefault();
            repflow.Status = "3";
            repflow.Rtp = WebSecurity.CurrentUserId;
            repflow.Rtt = DateTime.Now;

            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public string GetID()
        {
            //string str = "";
            //str += "SELECT MAX(DOCID) RECNO FROM REPAIR ";
            //var r = db.Database.SqlQuery(typeof(string), str);
            string s = db.Repairs.Select(r => r.DocId).Max();
            string did = "";
            int yymm = (System.DateTime.Now.Year - 1911) * 100 + System.DateTime.Now.Month;
            if (!string.IsNullOrEmpty(s))
            {
                did = s;
            }
            if (did != "")
            {
                if (Convert.ToInt64(did) / 100000 == yymm)
                    did = Convert.ToString(Convert.ToInt64(did) + 1);
                else
                    did = Convert.ToString(yymm * 100000 + 1);
            }
            else
            {
                did = Convert.ToString(yymm * 100000 + 1);
            }
            return did;
        }

        public ActionResult Print(string id = null)
        {
            Repair repair = db.Repairs.Find(id);
            RepairDtl dtl = db.RepairDtls.Find(id);
            RepairEmp emp = db.RepairEmps
                .Where(ep => ep.DocId == id).FirstOrDefault();
            string[] s = new string[] { "?", "2" };
            RepairFlow flow = db.RepairFlows.Where(f => f.DocId == id)
                .Where(f => s.Contains(f.Status)).FirstOrDefault();
            RepairPrintVModel vm = new RepairPrintVModel();
            if (repair == null)
            {
                return HttpNotFound();
            }
            else
            {
                vm.Docid = id;
                vm.UserId = repair.UserId;
                vm.UserName = repair.UserName;
                vm.AccDpt = repair.AccDpt;
                vm.ApplyDate = repair.ApplyDate;
                vm.AssetNo = repair.AssetNo;
                vm.AssetNam = repair.AssetName;
                vm.Company = db.Departments.Find(repair.DptId).Name_C;
                vm.Amt = repair.Amt;
                vm.Contact = repair.Contact;
                vm.PlaceLoc = repair.PlaceLoc;
                vm.PlantDoc = repair.PlantDoc;
                vm.RepType = repair.RepType;
                vm.TroubleDes = repair.TroubleDes;
                if (dtl != null)
                {
                    vm.DealDes = dtl.DealDes;
                    vm.EndDate = dtl.EndDate;
                }
                //
                vm.AccDptNam = db.Departments.Find(repair.AccDpt).Name_C;
                vm.Hour = dtl.Hour;
                vm.InOut = dtl.InOut;
                vm.EngName = emp == null ? "" : db.AppUsers.Find(emp.UserId).FullName;
                AppUser u;
                if (flow != null)
                {
                    if (flow.Status == "2")
                    {
                        vm.CloseDate = flow.Rtt;
                         u = db.AppUsers.Find(flow.UserId);
                        if (u != null)
                        {
                            vm.DelivEmp = u.UserName;
                            vm.DelivEmpName = u.FullName;
                        }
                    }
                    else {
                        flow = db.RepairFlows.Where(f => f.DocId == id && f.Cls == "驗收人")
                            .OrderByDescending(f => f.StepId).FirstOrDefault();
                        if (flow != null)
                        {
                            vm.CloseDate = flow.Rtt;
                            u = db.AppUsers.Find(flow.UserId);
                            if (u != null)
                            {
                                vm.DelivEmp = u.UserName;
                                vm.DelivEmpName = u.FullName;
                            }
                        }
                        else
                        {
                            u = db.AppUsers.Find(repair.CheckerId);
                            if (u != null)
                            {
                                vm.DelivEmp = u.UserName;
                                vm.DelivEmpName = u.FullName;
                            }
                        }
                            
                    }
                }
            }
            Asset asset = db.Assets.Find(repair.AssetNo);
            if (asset != null)
            {
                vm.BuyDate = asset.BuyDate;
                vm.Cost = asset.Cost;
            }
            return PartialView(vm);
        }

        public ActionResult UpdCases(string kps)
        {
            Repair rp = new Repair();
            rp.DocId = kps;

            return View(rp);
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
