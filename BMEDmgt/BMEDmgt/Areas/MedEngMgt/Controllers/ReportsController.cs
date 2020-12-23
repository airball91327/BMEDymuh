using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.IO;
using System.Data;
using System.Web.Security;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        // GET: MedEngMgt/Reports
        public ActionResult Index(string rpname)
        {
            if (rpname == "儀器設備保養清單")
                return RedirectToAction("AssetKeepList");
            ReportQryVModel pv = new ReportQryVModel();
            pv.ReportClass = rpname;

            List<SelectListItem> listItem = new List<SelectListItem>();
            SelectListItem li;
            listItem.Add(new SelectListItem { Text = "請選擇", Value = "" });
            db.Departments.ToList()
                .ForEach(d =>
                {
                    li = new SelectListItem();
                    li.Text = d.Name_C;
                    li.Value = d.DptId;
                    listItem.Add(li);

                });
            ViewData["ACCDPT"] = new SelectList(listItem, "Value", "Text");

            if (rpname == "保養達成率")
                return View("KeepRateIndex", pv);
            return View(pv);
        }

        private List<AssetKpScheVModel> AssetKpSche(ReportQryVModel v)
        {

            List<AssetKpScheVModel> sv = new List<AssetKpScheVModel>();
            var data = db.Assets
                .Join(db.AssetKeeps.Where(x => x.Cycle > 0), a => a.AssetNo, k => k.AssetNo,
                (a, k) => new
                {
                    asset = a,
                    assetkeep = k
                })
                .Join(db.Departments, a => a.asset.DelivDpt, d => d.DptId,
                (a, d) => new {
                    asset = a.asset,
                    assetkeep = a.assetkeep,
                    dept = d
                });
            data = data.Where(r => r.asset.AssetClass == (v.AssetClass1 ?? v.AssetClass2));
            if (!string.IsNullOrEmpty(v.AssetName))
            {
                data = data.Where(r => r.asset.Cname.Contains(v.AssetName));
            }
            string acc = v.AccDpt;
            string deliv = v.DelivDpt;
            string ano = v.AssetNo;
            if (!string.IsNullOrEmpty(acc))
                data = data.Where(x => x.asset.AccDpt == acc);
            if (!string.IsNullOrEmpty(deliv))
                data = data.Where(x => x.asset.DelivDpt == deliv);
            if (!string.IsNullOrEmpty(ano))
                data = data.Where(x => x.asset.AssetNo == ano);
            AssetKpScheVModel aks;
            int year = DateTime.Now.Year - 1911;
            int yyymm = 0;
            int cycle = 0;
            int y1 = 0;
            int m1 = 0;
            int i = 0;
            foreach (var s in data.ToList())
            {
                aks = new AssetKpScheVModel();
                aks.AssetNo = s.asset.AssetNo;
                aks.AssetName = s.asset.Cname;
                aks.DelivDptName = s.dept.Name_C;
                aks.Brand = s.asset.Brand;
                aks.Type = s.asset.Type;
                yyymm = s.assetkeep.KeepYm == null ? 0 : s.assetkeep.KeepYm.Value;
                cycle = s.assetkeep.Cycle == null ? 0 : s.assetkeep.Cycle.Value;
                y1 = yyymm / 100;
                m1 = yyymm % 100;
                if (yyymm > 0 && cycle > 0)
                {
                    for (i = 1; i <= 12; i++)
                    {
                        if (((year - y1) * 12 + (i - m1)) % cycle == 0)
                        {
                            switch (i)
                            {
                                case 1:
                                    aks.Jan = "*";
                                    break;
                                case 2:
                                    aks.Feb = "*";
                                    break;
                                case 3:
                                    aks.Mar = "*";
                                    break;
                                case 4:
                                    aks.Apr = "*";
                                    break;
                                case 5:
                                    aks.May = "*";
                                    break;
                                case 6:
                                    aks.Jun = "*";
                                    break;
                                case 7:
                                    aks.Jul = "*";
                                    break;
                                case 8:
                                    aks.Aug = "*";
                                    break;
                                case 9:
                                    aks.Sep = "*";
                                    break;
                                case 10:
                                    aks.Oct = "*";
                                    break;
                                case 11:
                                    aks.Nov = "*";
                                    break;
                                case 12:
                                    aks.Dec = "*";
                                    break;
                            }
                        }
                    }
                }
                sv.Add(aks);
            }

            return sv;
        }

        public void AssetKpScheExcel(ReportQryVModel v)
        {
            DataTable dt = new DataTable();
            DataRow dw;
            dt.Columns.Add("財產編號");
            dt.Columns.Add("設備名稱");
            dt.Columns.Add("保管部門");
            dt.Columns.Add("廠牌");
            dt.Columns.Add("型號");
            dt.Columns.Add("一月");
            dt.Columns.Add("二月");
            dt.Columns.Add("三月");
            dt.Columns.Add("四月");
            dt.Columns.Add("五月");
            dt.Columns.Add("六月");
            dt.Columns.Add("七月");
            dt.Columns.Add("八月");
            dt.Columns.Add("九月");
            dt.Columns.Add("十月");
            dt.Columns.Add("十一月");
            dt.Columns.Add("十二月");
            List<AssetKpScheVModel> mv = AssetKpSche(v);
            mv.ForEach(m =>
            {
                dw = dt.NewRow();
                dw[0] = m.AssetNo;
                dw[1] = m.AssetName;
                dw[2] = m.DelivDptName;
                dw[3] = m.Brand;
                dw[4] = m.Type;
                dw[5] = m.Jan;
                dw[6] = m.Feb;
                dw[7] = m.Mar;
                dw[8] = m.Apr;
                dw[9] = m.May;
                dw[10] = m.Jun;
                dw[11] = m.Jul;
                dw[12] = m.Aug;
                dw[13] = m.Sep;
                dw[14] = m.Oct;
                dw[15] = m.Nov;
                dw[16] = m.Dec;
                dt.Rows.Add(dw);
            });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("設備保養排程年報");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=AssetKpSche.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
        public void ExcelQA(ReportQryVModel v)
        {
            List<QuestReport> qrlist = QuestAnaly(v);
            DataTable dt = new DataTable();
            DataRow dw;
            //dt.Columns.Add("合約");
            dt.Columns.Add("表單編號(請修案號)");
            dt.Columns.Add("時間戳記");
            //dt.Columns.Add("部門代號");
            dt.Columns.Add("申請部門");
            //
            int cols = 0;
            db.QuestionnaireMs.Where(m => m.Flg == "Y")
            .Join(db.Questionnaires, m => m.VerId, q => q.VerId,
            (m, q) => q).OrderBy(q => q.Qid)
            .ToList().
            ForEach(q =>
            {
                dt.Columns.Add(q.Qtitle);
            });
            //
            qrlist.ToList()
                    .ForEach(j =>
                    {
                        dw = dt.NewRow();
                        dw[0] = j.Docid;
                        //dw[0] = j.ContractNo + j.Contract;
                        dw[1] = j.TimeStamp;
                        //dw[2] = j.DptId;
                        dw[2] = j.DptName;
                        cols = 3;
                        foreach (QuestAnswer s in j.Answers)
                        {
                            dw[cols] = s.Answer;
                            cols++;
                        }
                        dt.Rows.Add(dw);
                    });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("滿意度調查統計表");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=QuestionAnalysis.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        [HttpPost]
        public ActionResult Index(ReportQryVModel v)
        {

            if (v.Edate != null)
            {
                v.Edate = v.Edate.Value.AddHours(23)
                    .AddMinutes(59)
                    .AddSeconds(59);
            }
            TempData["qry"] = v;
            switch (v.ReportClass)
            {
                case "月故障率統計表":
                    if (v.Edate == null && v.Sdate == null)
                    {
                        return new JsonResult
                        {
                            Data = new { success = false, error = "需選擇一個時間!" },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                    return PartialView("MonthFailRate", MonthFailRate(v));
                case "月維修清單":
                    return PartialView("MonthRepair", MonthRepair(v));
                case "月保養清單":
                    return PartialView("MonthKeep", MonthKeep(v));
                case "維修保養統計表":
                    return PartialView("RepairKeep", RepairKeep(v));
                case "維修金額統計表":
                    return PartialView("RepairCost", RepairCost(v));
                case "保養金額統計表":
                    return PartialView("KeepCost", KeepCost(v));
                case "工作時數統計表":
                    return PartialView("DoHrSumMon", DoHrSumMon(v));
                case "未結案清單":
                    return PartialView("UnSignList", UnSignList(v));
                case "維修保養履歷":
                    ViewData["Ano"] = v.AssetNo;
                    if (v.Edate == null)
                    {
                        if (v.Sdate == null)
                        {
                            v.Sdate = DateTime.Now.AddYears(-1);
                        }
                        v.Edate = DateTime.Now.AddHours(23)
                        .AddMinutes(59)
                        .AddSeconds(59);
                    }

                    List<RpKpHistoryVModel> rk = RpKpHistory(v);
                    AssetAnalysis ay = new AssetAnalysis();
                    ay.RepairHour = rk.Where(r => r.DocType == "請修").Select(r => r.Hours).Sum().Value;
                    ay.RepCost = rk.Where(r => r.DocType == "請修").Select(r => r.Cost).Sum().Value;
                    ay.KeepHour = rk.Where(r => r.DocType == "保養").Select(r => r.Hours).Sum().Value;
                    ay.KeepCost = rk.Where(r => r.DocType == "保養").Select(r => r.Cost).Sum().Value;
                    Asset at = db.Assets.Find(v.AssetNo);
                    if (at != null)
                    {
                        if (at.Cost != null)
                        {
                            if (at.Cost > 0)
                                ay.RepRatio = decimal.Round(ay.RepCost / at.Cost.Value * 100m, 2);
                            else
                                ay.RepRatio = 0;
                        }
                    }
                    double faildays = 0;
                    double d = 0;
                    rk.Where(r => r.DocType == "請修").ToList()
                        .ForEach(r =>
                        {
                            if (r.EndDate == null)
                            {
                                faildays += v.Edate.Value.Subtract(r.ApplyDate).TotalDays;
                            }
                            else if (r.EndDate.Value.CompareTo(v.Edate.Value) > 0)
                            {
                                faildays += v.Edate.Value.Subtract(r.ApplyDate).TotalDays;
                            }
                            else
                            {
                                d = r.EndDate.Value.Subtract(r.ApplyDate).TotalDays;
                                if (d > 0)
                                {
                                    if (d <= 1d)
                                        faildays += 1d;
                                    else
                                        faildays += d;
                                }
                            }
                        });
                    ay.ProperRate = decimal.Round(100m -
                        Convert.ToDecimal(faildays / v.Edate.Value.Subtract(v.Sdate.Value).TotalDays) * 100m, 2);
                    //decimal.Round(Convert.ToDecimal(faildays / v.Edate.Value.Subtract(v.Sdate.Value).TotalDays)*100m, 2);
                    ViewData["Analysis"] = ay;
                    return PartialView("RpKpHistory", rk);
                case "設備妥善率統計表":
                    if (v.Edate == null)
                    {
                        if (v.Sdate == null)
                        {
                            v.Sdate = DateTime.Now.AddYears(-1);
                        }
                        v.Edate = DateTime.Now.AddHours(23)
                        .AddMinutes(59)
                        .AddSeconds(59);
                    }
                    return PartialView("AssetProperRate", AssetProperRate(v));
                case "重複故障清單":
                    return PartialView("RepeatFail", RepeatFail(v));
                case "維修保養零件統計表":
                    return PartialView("RepKeepStok", RepKeepStok(v));
                case "維修保養零件(廠牌)統計表":
                    return PartialView("RpKpStokBd", RpKpStokBd(v));
                case "零件帳務清單":
                    return PartialView("StokCost", StokCost(v));
                case "滿意度調查統計表":
                    return PartialView("QuestionAnalysis", QuestAnaly(v));
                    //case "儀器設備保養清單":
                    //    return PartialView("AssetKeepList", AssetKeepList(v));

            }

            return View();
        }

        [HttpPost]
        public ActionResult KeepEndRateIndex(ReportQryVModel v)
        {
            if (v.Sdate == null || v.Edate == null)
            {
                return new JsonResult
                {
                    Data = new { success = false, error = "需選擇一個時間區間!" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            List<KeepEndRate> result = GetKeepEndRateList(v);
            TempData["qry"] = v;

            return PartialView("KeepEndRate", result);
        }

        public void ExcelKeepEndRate(ReportQryVModel v)
        {
            //
            ExcelPackage excel = new ExcelPackage();
            var sheet1 = excel.Workbook.Worksheets.Add("保養達成率");
            //Sheet1
            List<KeepEndRate> result = GetKeepEndRateList(v);
            //Title
            #region write header
            sheet1.Cells[1, 1].Value = "表單產生日期";
            sheet1.Cells[1, 2].Value = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            sheet1.Cells[2, 1].Value = "工程師代號";
            sheet1.Cells[2, 2].Value = "工程師姓名";
            sheet1.Cells[2, 3].Value = "完成件數";
            sheet1.Cells[2, 4].Value = "總件數";
            sheet1.Cells[2, 5].Value = "達成率";
            sheet1.Cells[2, 6].Value = "完成件數(自行)";
            sheet1.Cells[2, 7].Value = "總件數(自行)";
            sheet1.Cells[2, 8].Value = "達成率(自行)";
            sheet1.Cells[2, 9].Value = "完成件數(委外)";
            sheet1.Cells[2, 10].Value = "總件數(委外)";
            sheet1.Cells[2, 11].Value = "達成率(委外)";
            sheet1.Cells[2, 12].Value = "總件數(其他)";
            #endregion

            //Data
            #region write content
            int pos = 3;
            foreach (var item in result)
            {
                sheet1.Cells[pos, 1].Value = item.UserName;
                sheet1.Cells[pos, 2].Value = item.FullName;
                sheet1.Cells[pos, 3].Value = item.EndCases;
                sheet1.Cells[pos, 4].Value = item.KeepCases;
                sheet1.Cells[pos, 5].Value = item.EndCasesRate;
                sheet1.Cells[pos, 6].Value = item.EndCases1;
                sheet1.Cells[pos, 7].Value = item.KeepCases1;
                sheet1.Cells[pos, 8].Value = item.EndCasesRate1;
                sheet1.Cells[pos, 9].Value = item.EndCases2;
                sheet1.Cells[pos, 10].Value = item.KeepCases2;
                sheet1.Cells[pos, 11].Value = item.EndCasesRate2;
                sheet1.Cells[pos, 12].Value = item.KeepCasesOther;
                pos++;
            }
            #endregion

            var fileName = "保養達成率" + DateTime.Now.ToString("yyyyMMdd")  + ".xlsx";
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=" + fileName);
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }

        private List<KeepEndRate> GetKeepEndRateList(ReportQryVModel v)
        {
            var qrySdate = v.Sdate;
            var qryEdate = v.Edate;
            qryEdate = qryEdate.Value.AddDays(1).AddSeconds(-1);

            var keep = db.Keeps.Where(k => k.SentDate >= qrySdate && k.SentDate <= qryEdate);
            var query = keep.Join(db.KeepDtls, k => k.DocId, kd => kd.DocId,
                            (k, kd) => new
                            {
                                keep = k,
                                kdtl = kd
                            })
                            .Join(db.Assets, k => k.keep.AssetNo, a => a.AssetNo,
                            (k, a) => new
                            {
                                keep = k.keep,
                                kdtl = k.kdtl,
                                asset = a
                            })
                            .Join(db.AssetKeeps, k => k.asset.AssetNo, ak => ak.AssetNo,
                            (k, ak) => new
                            {
                                keep = k.keep,
                                kdtl = k.kdtl,
                                asset = k.asset,
                                assetkeep = ak
                            });

            var groupEngs = query.GroupBy(q => q.assetkeep.KeepEngId).Select(group => group.FirstOrDefault().assetkeep.KeepEngId).Distinct().ToList();
            var engineers = groupEngs.Join(db.AppUsers, g => g, a => a.Id, (g, a) => a).ToList();
            List<KeepEndRate> result = new List<KeepEndRate>();
            /* 0: 自行, 1: 委外, 2: 租賃, 3: 保固, 4: 借用, 5: 委外/自行*/
            foreach (var eng in engineers)
            {
                //總數
                int KeepCases = query.Where(q => q.assetkeep.KeepEngId == eng.Id).Count();
                int EndCases = query.Where(q => q.assetkeep.KeepEngId == eng.Id).Where(q => q.kdtl.EndDate != null).Count();
                decimal EndCasesRate = EndCases == 0 ? 0 : ((decimal)EndCases / KeepCases);
                //自行
                int KeepCases1 = query.Where(q => q.assetkeep.KeepEngId == eng.Id).Where(q => q.kdtl.InOut == "1").Count();
                int EndCases1 = query.Where(q => q.assetkeep.KeepEngId == eng.Id).Where(q => q.kdtl.EndDate != null)
                                     .Where(q => q.kdtl.InOut == "1").Count();
                decimal EndCasesRate1 = EndCases1 == 0 ? 0 : ((decimal)EndCases1 / KeepCases1);
                //委外
                int KeepCases2 = query.Where(q => q.assetkeep.KeepEngId == eng.Id).Where(q => q.kdtl.InOut == "2").Count();
                int EndCases2 = query.Where(q => q.assetkeep.KeepEngId == eng.Id).Where(q => q.kdtl.EndDate != null)
                                     .Where(q => q.kdtl.InOut == "2").Count();
                decimal EndCasesRate2 = EndCases2 == 0 ? 0 : ((decimal)EndCases2 / KeepCases2);
                //其他
                int KeepCasesOther = query.Where(q => q.assetkeep.KeepEngId == eng.Id).Where(q => q.kdtl.InOut != "1" && q.kdtl.InOut != "2").Count();

                KeepEndRate kr = new KeepEndRate();
                kr.UserId = eng.Id;
                kr.UserName = eng.UserName;
                kr.FullName = eng.FullName;
                kr.KeepCases = KeepCases;
                kr.EndCases = EndCases;
                kr.EndCasesRate = EndCasesRate.ToString("P");
                kr.KeepCases1 = KeepCases1;
                kr.EndCases1 = EndCases1;
                kr.EndCasesRate1 = EndCasesRate1.ToString("P");
                kr.KeepCases2 = KeepCases2;
                kr.EndCases2 = EndCases2;
                kr.EndCasesRate2 = EndCasesRate2.ToString("P");
                kr.KeepCasesOther = KeepCasesOther;
                result.Add(kr);
            }
            return result;
        }

        public void ExcelAssetProperRate(ReportQryVModel v)
        {
            DataTable dt = new DataTable();
            DataRow dw;
            dt.Columns.Add("財產編號");
            dt.Columns.Add("設備名稱");
            dt.Columns.Add("廠牌");
            dt.Columns.Add("型號");
            dt.Columns.Add("成本中心");
            dt.Columns.Add("維修日數");
            dt.Columns.Add("維修次數");
            dt.Columns.Add("妥善率");
            List<ProperRate> mv = AssetProperRate(v);
            mv.ForEach(m =>
            {
                dw = dt.NewRow();
                dw[0] = m.AssetNo;
                dw[1] = m.AssetName;
                dw[2] = m.Brand;
                dw[3] = m.Type;
                dw[4] = m.AccDpt + m.AccDptNam;
                dw[5] = m.RepairDays;
                dw[6] = m.RepairCnts;
                dw[7] = m.AssetProperRate;

                dt.Rows.Add(dw);
            });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("設備妥善率統計");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=MonthKeep.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

        }
        private List<ProperRate> AssetProperRate(ReportQryVModel v)
        {
            var days = v.Edate.Value.Subtract(v.Sdate.Value).TotalDays;
            double faildays = 0;
            double dd = 0;
            int cnt = 0;
            List<ProperRate> sv = new List<ProperRate>();
            ProperRate pr;
            List<Asset> assets = db.Assets.Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
                    .Where(a => a.DisposeKind == "正常").ToList();
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                assets = assets.Where(a => a.AccDpt == v.AccDpt).ToList();
            }
            if (!string.IsNullOrEmpty(v.AssetNo))
            {
                assets = assets.Where(a => a.AssetNo == v.AssetNo)
                    .ToList();
            }

            foreach (Asset asset in assets)
            {
                pr = new ProperRate();
                pr.AssetNo = asset.AssetNo;
                pr.AssetName = asset.Cname;
                pr.Brand = asset.Brand;
                pr.Type = asset.Type;
                pr.AccDpt = asset.AccDpt;
                pr.AccDptNam = db.Departments.Find(asset.AccDpt).Name_C;
                faildays = 0;
                dd = 0;
                cnt = 0;
                db.Repairs.Where(r => r.AssetNo == asset.AssetNo)
                    .Where(r => r.ApplyDate >= v.Sdate && r.ApplyDate <= v.Edate)
                    .Join(db.RepairDtls.Where(d => d.EndDate != null), r => r.DocId, d => d.DocId,
                    (r, d) => new { repair = r, d.EndDate })
                    .ToList()
                    .ForEach(r =>
                    {
                        if (r.EndDate.Value.CompareTo(v.Edate.Value) > 0)
                        {
                            faildays += v.Edate.Value.Subtract(r.repair.ApplyDate.Value).TotalDays;
                        }
                        else
                        {
                            dd = r.EndDate.Value.Subtract(r.repair.ApplyDate.Value).TotalDays;
                            if (dd > 0)
                            {
                                if (dd <= 1d)
                                    faildays += 1d;
                                else
                                    faildays += dd;
                            }
                        }
                        cnt++;
                    });
                pr.RepairCnts = cnt;
                pr.RepairDays = faildays;
                pr.AssetProperRate = decimal.Round(100m -
                        Convert.ToDecimal(faildays / days) * 100m, 2);
                sv.Add(pr);
            }
            return sv;
        }

        public void ExcelAssetKeepList()
        {
            DataTable dt = new DataTable();
            DataRow dw;
            dt.Columns.Add("財產編號");
            dt.Columns.Add("儀器名稱");
            dt.Columns.Add("廠牌");
            dt.Columns.Add("規格");
            dt.Columns.Add("型號");
            dt.Columns.Add("保養方式");
            dt.Columns.Add("成本中心名稱");
            dt.Columns.Add("存放地點");
            dt.Columns.Add("保固起始日");
            dt.Columns.Add("保固終止日");
            dt.Columns.Add("合約起始日");
            dt.Columns.Add("合約終止日");
            dt.Columns.Add("備註");

            List<AssetKeepListVModel> mv = GetAssetKeepList();
            mv.ForEach(m =>
            {
                dw = dt.NewRow();
                dw[0] = m.AssetNo;
                dw[1] = m.AssetName;
                dw[2] = m.Brand;
                dw[3] = m.Standard;
                dw[4] = m.Type;
                dw[5] = m.InOut;
                dw[6] = m.AccDptName;
                dw[7] = m.LeaveSite;
                dw[8] = m.WartySt;
                dw[9] = m.WartyEd;
                dw[10] = m.Sdate;
                dw[11] = m.Edate;
                dw[12] = m.Note;
                dt.Rows.Add(dw);
            });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("儀器設備保養清單");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=AssetKeepList.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
        public ActionResult AssetKeepList()
        {
            List<AssetKeepListVModel> lst = GetAssetKeepList();

            return View(lst.Take(100).ToList());
        }

        public List<AssetKeepListVModel> GetAssetKeepList()
        {
            AssetKeepListVModel a;
            List<AssetKeepListVModel> lst = new List<AssetKeepListVModel>();
            db.Assets.Join(db.AssetKeeps, at => at.AssetNo, k => k.AssetNo,
                (at, k) => new
                {
                    asset = at,
                    assetkeep = k
                }).Join(db.Departments, at => at.asset.AccDpt, d => d.DptId,
                (at, d) => new {
                    asset = at.asset,
                    assetkeep = at.assetkeep,
                    dept = d
                }).GroupJoin(db.Contracts, at => at.assetkeep.ContractNo, c => c.ContractNo,
                (at, c) => new
                {
                    at.asset,
                    at.assetkeep,
                    at.dept,
                    contract = c
                }).SelectMany(c => c.contract.DefaultIfEmpty(),
                (x, y) => new
                {
                    x.asset,
                    x.assetkeep,
                    x.dept,
                    contract = y
                }).ToList().ForEach(x =>
                {
                    a = new AssetKeepListVModel();
                    a.AssetNo = x.asset.AssetNo;
                    a.AssetName = x.asset.Cname;
                    a.Brand = x.asset.Brand;
                    a.Standard = x.asset.Standard;
                    a.Type = x.asset.Type;
                    a.InOut = x.assetkeep.InOut;
                    a.AccDptName = x.dept.Name_C;
                    a.LeaveSite = x.asset.LeaveSite;
                    a.WartySt = x.asset.WartySt;
                    a.WartyEd = x.asset.WartyEd;
                    a.Sdate = x.contract == null ? "" : x.contract.Sdate.ToString("yyyy/MM/dd");
                    a.Edate = x.contract == null ? "" : x.contract.Edate.ToString("yyyy/MM/dd");
                    a.Note = x.asset.Note;
                    lst.Add(a);
                });

            return lst;
        }

        private List<QuestReport> QuestAnaly(ReportQryVModel v)
        {
            List<QuestReport> qlist = new List<QuestReport>();
            QuestReport qr;
            List<QuestMain> qm = db.QuestMains.ToList();
            if (v.Sdate != null)
                qm = qm.Where(s => s.Rtt >= v.Sdate).ToList();
            if (v.Edate != null)
                qm = qm.Where(s => s.Rtt <= v.Edate).ToList();
            qm.ForEach(m =>
            {
                qr = new QuestReport();
                qr.Docid = m.Docid;
                qr.TimeStamp = m.Rtt.ToString("yyyy/MM/dd");
                qr.ContractNo = m.ContractNo;
                qr.Contract = "";
                qr.DptId = m.CustId;
                qr.DptName = m.CustNam;
                qr.Answers = db.QuestAnswers.Where(a => a.Docid == m.Docid)
                             .OrderBy(a => a.Qid).ToList();
                if (qr.Answers.Count > 0)
                    qlist.Add(qr);
            });
            return qlist;
        }

        public ActionResult Index2(string rpname)
        {
            ReportQryVModel pv = new ReportQryVModel();
            pv.ReportClass = rpname;

            return View(pv);
        }
        public ActionResult Index3(string rpname)
        {
            ReportQryVModel pv = new ReportQryVModel();
            pv.ReportClass = rpname;
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
            ViewData["DELIVDPT"] = new SelectList(listItem2, "Value", "Text");
            return View(pv);
        }
        [HttpPost]
        public ActionResult Index3(ReportQryVModel v)
        {
            TempData["qry2"] = v;
            return PartialView("AssetKpSche", AssetKpSche(v));
        }
        public void ExcelRpKpHistory(ReportQryVModel v)
        {
            //
            ExcelPackage excel = new ExcelPackage();
            var sheet1 = excel.Workbook.Worksheets.Add("儀器基本資料");
            //Sheet1
            Asset asset = db.Assets.Find(v.AssetNo);
            if (asset != null)
            {
                asset.DelivDptName = db.Departments.Find(asset.DelivDpt).Name_C;
                asset.AccDptName = db.Departments.Find(asset.AccDpt).Name_C;
            }
            sheet1.Cells[1, 1].Value = "儀器基本資料";
            sheet1.Cells[2, 1].Value = "財產編號";
            sheet1.Cells[2, 2].Value = asset.AssetNo;
            sheet1.Cells[3, 1].Value = "成本中心";
            sheet1.Cells[3, 2].Value = asset.AccDptName;
            sheet1.Cells[4, 1].Value = "廠牌";
            sheet1.Cells[4, 2].Value = asset.Brand;
            sheet1.Cells[5, 1].Value = "型號";
            sheet1.Cells[5, 2].Value = asset.Type;
            sheet1.Cells[6, 1].Value = "中文名稱";
            sheet1.Cells[6, 2].Value = asset.Cname;
            sheet1.Cells[7, 1].Value = "取得金額";
            sheet1.Cells[7, 2].Value = asset.Cost;
            sheet1.Cells[8, 1].Value = "取得日期";
            sheet1.Cells[8, 2].Value = asset.BuyDate == null ? "" : asset.BuyDate.Value.ToString("yyyy/MM/dd"); ;
            sheet1.Cells[9, 1].Value = "耐用年限";
            sheet1.Cells[9, 2].Value = asset.UseLife == null ? "" : asset.UseLife.Value.ToString();
            //sheet2
            var sheet2 = excel.Workbook.Worksheets.Add("維修保養履歷");
            List<RpKpHistoryVModel> sv = new List<RpKpHistoryVModel>();
            List<RpKpHistoryVModel> sv2 = new List<RpKpHistoryVModel>();
            var ss = new[] { "?", "2" };
            sv = db.Assets.Where(a => a.AssetNo == v.AssetNo)
                .Join(db.Repairs, a => a.AssetNo, r => r.AssetNo,
                (a, r) => new
                {
                    DocType = "請修",
                    DocId = r.DocId,
                    AssetNo = a.AssetNo,
                    AssetName = a.Cname,
                    ApplyDate = r.ApplyDate,
                    TroubleDes = r.TroubleDes
                }).Join(db.RepairFlows.Where(f => ss.Contains(f.Status)), r => r.DocId, f => f.DocId,
                    (r, f) => r).Join(db.RepairDtls, r => r.DocId, d => d.DocId,
                (r, d) => new
                {
                    DocType = "請修",
                    DocId = r.DocId,
                    AssetNo = r.AssetNo,
                    AssetName = r.AssetName,
                    ApplyDate = r.ApplyDate,
                    TroubleDes = r.TroubleDes,
                    DealDes = d.DealDes,
                    EndDate = d.EndDate,
                    Hours = d.Hour,
                    Cost = d.Cost
                }).Join(db.RepairEmps.Join(db.AppUsers, r => r.UserId, a => a.Id,
                (r, a) => new
                {
                    DocId = r.DocId,
                    UserName = a.FullName
                }), d => d.DocId, e => e.DocId,
                (d, e) => new RpKpHistoryVModel
                {
                    DocType = "請修",
                    DocId = d.DocId,
                    AssetNo = d.AssetNo,
                    AssetName = d.AssetName,
                    ApplyDate = d.ApplyDate.Value,
                    TroubleDes = d.TroubleDes,
                    DealDes = d.DealDes,
                    EndDate = d.EndDate,
                    Hours = d.Hours,
                    Cost = d.Cost,
                    EngName = e.UserName
                }).ToList();
            //
            sv2 = db.Assets.Where(a => a.AssetNo == v.AssetNo)
                .Join(db.Keeps, a => a.AssetNo, r => r.AssetNo,
                (a, r) => new
                {
                    DocType = "保養",
                    DocId = r.DocId,
                    AssetNo = a.AssetNo,
                    AssetName = a.Cname,
                    ApplyDate = r.SentDate,
                    TroubleDes = ""
                }).Join(db.KeepFlows.Where(f => ss.Contains(f.Status)), r => r.DocId, f => f.DocId,
                    (r, f) => r).Join(db.KeepDtls, r => r.DocId, d => d.DocId,
                (r, d) => new
                {
                    DocType = r.DocType,
                    DocId = r.DocId,
                    AssetNo = r.AssetNo,
                    AssetName = r.AssetName,
                    ApplyDate = r.ApplyDate,
                    TroubleDes = r.TroubleDes,
                    DealDes = d.Result,
                    EndDate = d.EndDate,
                    Hours = d.Hours,
                    Cost = d.Cost
                }).Join(db.KeepEmps.Join(db.AppUsers, r => r.UserId, a => a.Id,
                (r, a) => new
                {
                    DocId = r.DocId,
                    UserName = a.FullName
                }), d => d.DocId, e => e.DocId,
                (d, e) => new RpKpHistoryVModel
                {
                    DocType = d.DocType,
                    DocId = d.DocId,
                    AssetNo = d.AssetNo,
                    AssetName = d.AssetName,
                    ApplyDate = d.ApplyDate.Value,
                    TroubleDes = d.TroubleDes,
                    DealDes = d.DealDes,
                    EndDate = d.EndDate,
                    Hours = d.Hours,
                    Cost = d.Cost,
                    EngName = e.UserName
                }).ToList();
            sv.AddRange(sv2);
            if (v.Sdate != null)
                sv = sv.Where(s => s.ApplyDate >= v.Sdate).ToList();
            if (v.Edate != null)
                sv = sv.Where(s => s.ApplyDate <= v.Edate).ToList();
            //Title
            sheet2.Cells[1, 1].Value = "表單類別";
            sheet2.Cells[1, 2].Value = "表單編號";
            sheet2.Cells[1, 3].Value = "送單日期";
            sheet2.Cells[1, 4].Value = "完工日期";
            sheet2.Cells[1, 5].Value = "工程師";
            sheet2.Cells[1, 6].Value = "工時";
            sheet2.Cells[1, 7].Value = "費用";
            sheet2.Cells[1, 8].Value = "故障原因";
            sheet2.Cells[1, 9].Value = "處理情形";
            //Data
            int startPos = 2;
            foreach(var item in sv)
            {
                sheet2.Cells[startPos, 1].Value = item.DocType;
                sheet2.Cells[startPos, 2].Value = item.DocId;
                sheet2.Cells[startPos, 3].Value = item.ApplyDate;
                sheet2.Cells[startPos, 4].Value = item.EndDate;
                sheet2.Cells[startPos, 5].Value = item.EngName;
                sheet2.Cells[startPos, 6].Value = item.Hours;
                sheet2.Cells[startPos, 7].Value = item.Cost;
                sheet2.Cells[startPos, 8].Value = item.TroubleDes;
                sheet2.Cells[startPos, 9].Value = item.DealDes;
                startPos++;
            }
            //sheet3
            if (v.Edate == null)
            {
                if (v.Sdate == null)
                {
                    v.Sdate = DateTime.Now.AddYears(-1);
                }
                v.Edate = DateTime.Now.AddHours(23)
                .AddMinutes(59)
                .AddSeconds(59);
            }

            List<RpKpHistoryVModel> rk = RpKpHistory(v);
            AssetAnalysis ay = new AssetAnalysis();
            ay.RepairHour = rk.Where(r => r.DocType == "請修").Select(r => r.Hours).Sum().Value;
            ay.RepCost = rk.Where(r => r.DocType == "請修").Select(r => r.Cost).Sum().Value;
            ay.KeepHour = rk.Where(r => r.DocType == "保養").Select(r => r.Hours).Sum().Value;
            ay.KeepCost = rk.Where(r => r.DocType == "保養").Select(r => r.Cost).Sum().Value;
            Asset at = db.Assets.Find(v.AssetNo);
            if (at != null)
            {
                if (at.Cost != null)
                {
                    if (at.Cost > 0)
                        ay.RepRatio = decimal.Round(ay.RepCost / at.Cost.Value * 100m, 2);
                    else
                        ay.RepRatio = 0;
                }
            }
            double faildays = 0;
            double day = 0;
            rk.Where(r => r.DocType == "請修").ToList()
                .ForEach(r =>
                {
                    if (r.EndDate == null)
                    {
                        faildays += v.Edate.Value.Subtract(r.ApplyDate).TotalDays;
                    }
                    else if (r.EndDate.Value.CompareTo(v.Edate.Value) > 0)
                    {
                        faildays += v.Edate.Value.Subtract(r.ApplyDate).TotalDays;
                    }
                    else
                    {
                        day = r.EndDate.Value.Subtract(r.ApplyDate).TotalDays;
                        if (day > 0)
                        {
                            if (day <= 1d)
                                faildays += 1d;
                            else
                                faildays += day;
                        }
                    }
                });
            ay.ProperRate = decimal.Round(100m -
                Convert.ToDecimal(faildays / v.Edate.Value.Subtract(v.Sdate.Value).TotalDays) * 100m, 2);
            //
            var sheet3 = excel.Workbook.Worksheets.Add("總計");
            sheet3.Cells[1, 1].Value = "總計";
            sheet3.Cells[2, 1].Value = "維修時數";
            sheet3.Cells[2, 2].Value = ay.RepairHour;
            sheet3.Cells[3, 1].Value = "保養時數";
            sheet3.Cells[3, 2].Value = ay.KeepHour;
            sheet3.Cells[4, 1].Value = "維修金額";
            sheet3.Cells[4, 2].Value = ay.RepCost;
            sheet3.Cells[5, 1].Value = "保養金額";
            sheet3.Cells[5, 2].Value = ay.KeepCost;
            sheet3.Cells[6, 1].Value = "妥善率";
            sheet3.Cells[6, 2].Value = ay.ProperRate;
            sheet3.Cells[7, 1].Value = "維修比";
            sheet3.Cells[7, 2].Value = ay.RepRatio;
            sheet3.Cells[8, 1].Value = "備註：維修比 = 維修總金額/取得金額。";
        
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=RpKpHistory.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
        private List<RpKpHistoryVModel> RpKpHistory(ReportQryVModel v)
        {
            List<RpKpHistoryVModel> sv = new List<RpKpHistoryVModel>();
            List<RpKpHistoryVModel> sv2 = new List<RpKpHistoryVModel>();
            if (string.IsNullOrEmpty(v.AssetNo))
                return sv;
            var ss = new[] { "?", "2" };
            sv = db.Assets.Where(a => a.AssetNo == v.AssetNo)
                .Join(db.Repairs, a => a.AssetNo, r => r.AssetNo,
                (a, r) => new
                {
                    DocType = "請修",
                    DocId = r.DocId,
                    AssetNo = a.AssetNo,
                    AssetName = a.Cname,
                    ApplyDate = r.ApplyDate,
                    TroubleDes = r.TroubleDes
                }).Join(db.RepairFlows.Where(f => ss.Contains(f.Status)), r => r.DocId, f => f.DocId,
                    (r, f) => r).Join(db.RepairDtls, r => r.DocId, d => d.DocId,
                (r, d) => new
                {
                    DocType = "請修",
                    DocId = r.DocId,
                    AssetNo = r.AssetNo,
                    AssetName = r.AssetName,
                    ApplyDate = r.ApplyDate,
                    TroubleDes = r.TroubleDes,
                    DealDes = d.DealDes,
                    EndDate = d.EndDate,
                    Hours = d.Hour,
                    Cost = d.Cost
                }).Join(db.RepairEmps.Join(db.AppUsers, r => r.UserId, a => a.Id,
                (r, a) => new
                {
                    DocId = r.DocId,
                    UserName = a.FullName
                }), d => d.DocId, e => e.DocId,
                (d, e) => new RpKpHistoryVModel
                {
                    DocType = "請修",
                    DocId = d.DocId,
                    AssetNo = d.AssetNo,
                    AssetName = d.AssetName,
                    ApplyDate = d.ApplyDate.Value,
                    TroubleDes = d.TroubleDes,
                    DealDes = d.DealDes,
                    EndDate = d.EndDate,
                    Hours = d.Hours,
                    Cost = d.Cost,
                    EngName = e.UserName
                }).ToList();
            //
            sv2 = db.Assets.Where(a => a.AssetNo == v.AssetNo)
                .Join(db.Keeps, a => a.AssetNo, r => r.AssetNo,
                (a, r) => new
                {
                    DocType = "保養",
                    DocId = r.DocId,
                    AssetNo = a.AssetNo,
                    AssetName = a.Cname,
                    ApplyDate = r.SentDate,
                    TroubleDes = ""
                }).Join(db.KeepFlows.Where(f => ss.Contains(f.Status)), r => r.DocId, f => f.DocId,
                    (r, f) => r).Join(db.KeepDtls, r => r.DocId, d => d.DocId,
                (r, d) => new
                {
                    DocType = r.DocType,
                    DocId = r.DocId,
                    AssetNo = r.AssetNo,
                    AssetName = r.AssetName,
                    ApplyDate = r.ApplyDate,
                    TroubleDes = r.TroubleDes,
                    DealDes = d.Result,
                    EndDate = d.EndDate,
                    Hours = d.Hours,
                    Cost = d.Cost
                }).Join(db.KeepEmps.Join(db.AppUsers, r => r.UserId, a => a.Id,
                (r, a) => new
                {
                    DocId = r.DocId,
                    UserName = a.FullName
                }), d => d.DocId, e => e.DocId,
                (d, e) => new RpKpHistoryVModel
                {
                    DocType = d.DocType,
                    DocId = d.DocId,
                    AssetNo = d.AssetNo,
                    AssetName = d.AssetName,
                    ApplyDate = d.ApplyDate.Value,
                    TroubleDes = d.TroubleDes,
                    DealDes = d.DealDes,
                    EndDate = d.EndDate,
                    Hours = d.Hours,
                    Cost = d.Cost,
                    EngName = e.UserName
                }).ToList();
            sv.AddRange(sv2);
            if (v.Sdate != null)
                sv = sv.Where(s => s.ApplyDate >= v.Sdate).ToList();
            if (v.Edate != null)
                sv = sv.Where(s => s.ApplyDate <= v.Edate).ToList();

            return sv;
        }

        private List<UnSignListVModel> UnSignList(ReportQryVModel v)
        {
            List<UnSignListVModel> sv = new List<UnSignListVModel>();
            List<UnSignListVModel> sv2 = new List<UnSignListVModel>();

            sv = db.RepairFlows.Where(f => f.Status == "?")
            .Join(db.RepairDtls, f => f.DocId, rd => rd.DocId,
            (f, rd) => new
            {
                f.DocId,
                f.UserId,
                f.Cls,
                rd.Cost,
                rd.EndDate,
                rd.CloseDate,
                rd.FailFactor,
                rd.DealDes,
                rd.DealState
            })
            .Join(db.Repairs, rd => rd.DocId, k => k.DocId,
            (rd, k) => new
            {
                rd.DocId,
                rd.UserId,
                rd.Cls,
                k.AccDpt,
                k.ApplyDate,
                k.AssetNo,
                rd.Cost,
                rd.EndDate,
                rd.CloseDate,
                rd.FailFactor,
                rd.DealDes,
                rd.DealState,
                k.TroubleDes
            }).Where(k => k.ApplyDate >= v.Sdate && k.ApplyDate <= v.Edate)
            .Join(db.Assets, k => k.AssetNo, at => at.AssetNo,
            (k, at) => new
            {
                k.DocId,
                k.UserId,
                k.Cls,
                k.AccDpt,
                k.ApplyDate,
                k.AssetNo,
                at.Cname,
                k.Cost,
                k.EndDate,
                k.CloseDate,
                k.TroubleDes,
                k.FailFactor,
                k.DealDes,
                k.DealState,
                at.Type,
                at.AssetClass
            })
             .Join(db.Departments, k => k.AccDpt, c => c.DptId,
            (k, c) => new
            {
                k.DocId,
                k.UserId,
                k.Cls,
                k.AccDpt,
                k.ApplyDate,
                k.AssetNo,
                k.Cname,
                k.Cost,
                k.EndDate,
                k.CloseDate,
                k.TroubleDes,
                k.FailFactor,
                k.DealDes,
                k.DealState,
                k.Type,
                k.AssetClass,
                c.Name_C
            })
            .Join(db.AppUsers, k => k.UserId, u => u.Id,
            (k, u) => new UnSignListVModel
            {
                DocTyp = "請修",
                DocId = k.DocId,
                AccDpt = k.AccDpt,
                AccDptNam = k.Name_C,
                AssetNo = k.AssetNo,
                AssetName = k.Cname,
                Type = k.Type,
                ApplyDate = k.ApplyDate.Value,
                EndDate = k.EndDate,
                TroubleDes = k.TroubleDes,
                FailFactor = k.FailFactor,
                DealDes = k.DealDes,
                DealState = k.DealState,
                EngNam = null,
                ClsEmp = u.FullName,
                AssetClass = k.AssetClass
            }).ToList();
            //
            foreach (UnSignListVModel s in sv)
            {
                RepairEmp kp = db.RepairEmps.Where(p => p.DocId == s.DocId).ToList()
                   .FirstOrDefault();
                if (kp != null)
                {
                    s.EngNam = db.AppUsers.Find(kp.UserId).FullName;
                }
                List<RepairCost> lk = db.RepairCosts.Where(r => r.DocId == s.DocId).ToList();
                if (lk != null)
                    s.Cost = lk.Sum(r => r.TotalCost);
            }
            //保養
            string str = "";
            str += "SELECT '保養' AS DOCTYP,B.DOCID,B.ASSETNO, B.ASSETNAME,F.TYPE,B.SENTDATE AS APPLYDATE,D.FULLNAME AS CLSEMP,";
            str += "B.ACCDPT,E.NAME_C AS ACCDPTNAM, C.ENDDATE,C.RESULT AS DEALSTATE,C.INOUT, ";
            str += "C.MEMO AS DEALDES, F.ASSETCLASS ";
            str += "FROM KEEPFLOW AS A JOIN KEEP AS B ON A.DOCID = B.DOCID ";
            str += "JOIN KEEPDTL AS C ON B.DOCID = C.DOCID ";
            str += "JOIN APPUSER AS D ON A.USERID = D.ID ";
            str += "JOIN DEPARTMENT AS E ON B.ACCDPT = E.DPTID "; ;
            str += "LEFT JOIN ASSET AS F ON B.AssetNo = F.AssetNo ";
            str += "LEFT JOIN ASSETKEEP AS G ON B.AssetNo = G.AssetNo ";
            str += "WHERE A.STATUS = '?' AND (B.SENTDATE BETWEEN @D1 AND @D2) ";

            sv2 = db.Database.SqlQuery<UnSignListVModel>(str,
                new SqlParameter("D1", v.Sdate),
                new SqlParameter("D2", v.Edate)).ToList();
            foreach (UnSignListVModel s in sv2)
            {
                switch (s.DealState)
                {
                    case "1":
                        s.DealState = "功能正常";
                        break;
                    case "2":
                        s.DealState = "預防處理";
                        break;
                    case "3":
                        s.DealState = "異常處理";
                        break;
                    case "4":
                        s.DealState = "維修時保養";
                        break;
                    case "5":
                        s.DealState = "退件";
                        break;
                    default:
                        s.DealState = "";
                        break;
                }
                switch (s.InOut)
                {
                    case "0":
                        s.InOut = "自行";
                        break;
                    case "1":
                        s.InOut = "委外";
                        break;
                    case "2":
                        s.InOut = "租賃";
                        break;
                    case "3":
                        s.InOut = "保固";
                        break;
                    case "4":
                        s.InOut = "借用";
                        break;
                    default:
                        s.InOut = "";
                        break;
                }
                s.TroubleDes = db.Keeps.Find(s.DocId).Cycle.ToString();
                s.FailFactor = "";
                KeepEmp kp = db.KeepEmps.Where(p => p.DocId == s.DocId).ToList()
                    .FirstOrDefault();
                if (kp != null)
                {
                    s.EngNam = db.AppUsers.Find(kp.UserId).FullName;
                }
                List<KeepCost> lk = db.KeepCosts.Where(r => r.DocId == s.DocId).ToList();
                if (lk != null)
                    s.Cost = lk.Sum(r => r.TotalCost);
            }
            sv.AddRange(sv2);
            sv = sv.Where(m => m.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1)).ToList();
            //
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                sv = sv.Where(vv => vv.AccDpt == v.AccDpt).ToList();
            }
            //
            return sv;
        }

        public void ExcelUS(ReportQryVModel v)
        {
            string str = "";
            str += "類別,表單編號,送單日期,完工日期,財產編號,設備名稱,型號,成本中心,成本中心名稱,";
            str += "故障描述/保養週期,故障原因,處理狀況/保養結果,保養方式,處理描述,費用,工程師,現在關卡";
            DataTable dt = new DataTable();
            DataRow dw;
            str.Split(new char[] { ',' }).ToList()
                .ForEach(s =>
                {
                    dt.Columns.Add(s);
                });
            List<UnSignListVModel> uv = UnSignList(v);
            uv.ForEach(m =>
            {
                dw = dt.NewRow();
                dw[0] = m.DocTyp;
                dw[1] = m.DocId;
                dw[2] = m.ApplyDate;
                dw[3] = m.EndDate;
                dw[4] = m.AssetNo;
                dw[5] = m.AssetName;
                dw[6] = m.Type;
                dw[7] = m.AccDpt;
                dw[8] = m.AccDptNam;
                dw[9] = m.TroubleDes;
                dw[10] = m.FailFactor;
                dw[11] = m.DealState;
                dw[12] = m.InOut;
                dw[13] = m.DealDes;
                dw[14] = m.Cost;
                dw[15] = m.EngNam;
                dw[16] = m.ClsEmp;
                dt.Rows.Add(dw);
            });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("未結案清單");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=UnSignList.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }


        public List<DoHrSumMonVModel> DoHrSumMon(ReportQryVModel v)
        {

            List<DoHrSumMonVModel> mv = new List<DoHrSumMonVModel>();
            DoHrSumMonVModel dv;

            List<UserHour> query = db.RepairDtls.Where(d => d.EndDate >= v.Sdate)
                .Where(d => d.EndDate <= v.Edate)
                .Join(db.Repairs, rd => rd.DocId, r => r.DocId,
                (rd, r) => new
                {
                    rd.DocId,
                    rd.Hour,
                    rd.EndDate,
                    rd.InOut,
                    r.ApplyDate,
                    r.AccDpt,
                    r.AssetNo
                }).Join(db.Assets, k => k.AssetNo, c => c.AssetNo,
                (k, c) => new
                {
                    k.DocId,
                    k.Hour,
                    k.EndDate,
                    k.InOut,
                    k.ApplyDate,
                    k.AccDpt,
                    c.AssetClass
                }).Join(db.Departments, k => k.AccDpt, c => c.DptId,
                (k, c) => new
                {
                    k.DocId,
                    k.Hour,
                    k.EndDate,
                    k.InOut,
                    k.ApplyDate,
                    k.AssetClass,
                    k.AccDpt
                }).Join(db.RepairEmps, rd => rd.DocId, re => re.DocId,
                (rd, re) => new UserHour
                {
                    Uid = re.UserId,
                    Hour = rd.Hour,
                    InOut = rd.InOut,
                    AssetClass = rd.AssetClass,
                    ApplyDate = rd.ApplyDate.Value,
                    EndDate = rd.EndDate.Value,
                    AccDpt = rd.AccDpt
                }).Where(m => m.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1)).ToList();
            //
            List<UserHour> query2 = db.KeepDtls.Where(d => d.EndDate >= v.Sdate)
                .Where(d => d.EndDate <= v.Edate)
                .Join(db.Keeps, kd => kd.DocId, k => k.DocId,
                (kd, k) => new
                {
                    kd.DocId,
                    kd.Hours,
                    kd.EndDate,
                    kd.InOut,
                    k.SentDate,
                    k.AccDpt,
                    k.AssetNo
                }).Join(db.Assets, k => k.AssetNo, c => c.AssetNo,
                (k, c) => new
                {
                    k.DocId,
                    k.Hours,
                    k.EndDate,
                    k.InOut,
                    k.SentDate,
                    k.AccDpt,
                    c.AssetClass
                }).Join(db.Departments, k => k.AccDpt, c => c.DptId,
                (k, c) => new 
                {
                    k.DocId,
                    k.Hours,
                    k.EndDate,
                    k.InOut,
                    k.SentDate,
                    k.AssetClass,
                    k.AccDpt
                }).Join(db.KeepEmps, kd => kd.DocId, ke => ke.DocId,
                (kd, ke) => new UserHour
                {
                    Uid = ke.UserId,
                    Hour = kd.Hours == null ? 0 : kd.Hours.Value,
                    InOut = kd.InOut,
                    AssetClass = kd.AssetClass,
                    ApplyDate = kd.SentDate.Value,
                    EndDate = kd.EndDate.Value,
                    AccDpt = kd.AccDpt
                }).Where(m => m.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1)).ToList();
            //
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                query = query.Where(vv => vv.AccDpt == v.AccDpt).ToList();
                query2 = query2.Where(vv => vv.AccDpt == v.AccDpt).ToList();
            }
            //
            IEnumerable<IGrouping<int, UserHour>> rt = query.GroupBy(j => j.Uid);
            IEnumerable<IGrouping<int, UserHour>> kt = query2.GroupBy(j => j.Uid);
            int case1 = 0;
            int case5 = 0;
            int abc = 0;
            DateTime sd = v.Edate.Value.AddMonths(-3);
            AppUser ur;
            //Get all engineers.
            var engs = Roles.GetUsersInRole("MedEngineer").ToList();
            foreach (var ss in engs)
            {
                ur = db.AppUsers.Find(WebSecurity.GetUserId(ss));
                if (ur != null)
                {
                    var g = rt.Where(r => r.Key == ur.Id).FirstOrDefault();
                    var keeps = kt.Where(k => k.Key == ur.Id).FirstOrDefault();
                    case1 = 0;
                    case5 = 0;
                    dv = new DoHrSumMonVModel();
                    dv.UserId = ur.Id;
                    dv.UserNam = ur.FullName;
                    dv.Cases = g == null ? 0 : g.Count();
                    dv.Hours = g == null ? 0 : g.Sum(s => s.Hour);
                    dv.KeepCases = keeps == null ? 0 : keeps.Count();
                    dv.KeepHours = keeps == null ? 0 : keeps.Sum(k => k.Hour);
                    case1 = g == null ? 0 : g.Where(g1 => g1.EndDate.Subtract(g1.ApplyDate).Days < 5).Count();
                    case5 = g == null ? 0 : g.Where(g1 => g1.EndDate.Subtract(g1.ApplyDate).Days >= 5).Count();
                    dv.OverFive = case5;
                    if (case1 + case5 > 0)
                    {
                        dv.OverFiveRate = Decimal.Round(Convert.ToDecimal(case1) /
                                Convert.ToDecimal(case1 + case5), 2);
                    }
                    else
                        dv.OverFiveRate = 0m;
                    //
                    if (g != null)
                    {
                        dv.Case3M = db.RepairDtls.Where(d => d.EndDate >= sd)
                        .Where(d => d.EndDate <= v.Edate)
                        .Join(db.RepairEmps, rd => rd.DocId, re => re.DocId,
                        (rd, re) => new
                        {
                            re.UserId
                        }).Where(re => re.UserId == g.Key).Count();
                        //dv.Fail3MRate
                        IEnumerable<IGrouping<string, UserAsset>> ob = db.RepairDtls.Where(d => d.EndDate >= sd)
                        .Where(d => d.EndDate <= v.Edate)
                        .Join(db.Repairs, rd => rd.DocId, r => r.DocId,
                        (rd, r) => new
                        {
                            rd.DocId,
                            r.AssetNo
                        })
                        .Join(db.RepairEmps, rd => rd.DocId, re => re.DocId,
                        (rd, re) => new UserAsset
                        {
                            Uid = re.UserId,
                            AssetNo = rd.AssetNo
                        }).Where(re => re.Uid == g.Key).GroupBy(j => j.AssetNo);
                        abc = 0;
                        foreach (var q in ob)
                        {
                            if (q.Count() >= 2)
                                abc += q.Count();
                        }
                        if (dv.Case3M > 0)
                            dv.Fail3MRate = Decimal.Round(Convert.ToDecimal(abc) / Convert.ToDecimal(dv.Case3M), 2);
                        else
                            dv.Fail3MRate = 0m;
                        //
                        ob = db.RepairDtls.Where(d => d.EndDate >= v.Sdate)
                        .Where(d => d.EndDate <= v.Edate)
                        .Join(db.Repairs, rd => rd.DocId, r => r.DocId,
                        (rd, r) => new
                        {
                            rd.DocId,
                            r.AssetNo
                        })
                        .Join(db.RepairEmps, rd => rd.DocId, re => re.DocId,
                        (rd, re) => new UserAsset
                        {
                            Uid = re.UserId,
                            AssetNo = rd.AssetNo
                        }).Where(re => re.Uid == g.Key).GroupBy(re => re.AssetNo);
                        abc = 0;
                        foreach (var q in ob)
                        {
                            if (q.Count() >= 2)
                                abc += q.Count();
                        }
                        if (dv.Case3M > 0)
                            dv.Fail1MRate = Decimal.Round(Convert.ToDecimal(abc) / Convert.ToDecimal(dv.Case3M), 2);
                        else
                            dv.Fail1MRate = 0m;
                        //
                        if (dv.Case3M > 0)
                            dv.SelfRate = Decimal.Round(g.Where(x => x.InOut == "內修").Count() / Convert.ToDecimal(dv.Case3M), 2);
                        else
                            dv.SelfRate = 0m;
                    }
                    mv.Add(dv);
                }
            }
            //
            return mv;
        }
        public List<MonthFailRateVModel> MonthFailRate(ReportQryVModel v)
        {
            List<MonthFailRateVModel> mv = new List<MonthFailRateVModel>();

            // Admin & 設備主管才可查詢全單位，其餘Role只可查詢自己單位
            if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("MedMgr"))
            {
                // Do nothing.
            }
            else
            {
                var ur = db.AppUsers.Where(u => u.UserName == User.Identity.Name).ToList().FirstOrDefault();
                v.AccDpt = ur.DptId;
            }
            //計算時間區間
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            if (v.Sdate == null && v.Edate != null)
            {
                endDate = v.Edate.Value;
                startDate = endDate.AddYears(-1).AddDays(6);
            }
            else if (v.Sdate != null && v.Edate == null)
            {
                endDate = v.Sdate.Value;
                startDate = endDate.AddYears(-1).AddDays(6);
            }
            else
            {
                endDate = v.Edate.Value;
                startDate = v.Sdate.Value;
            }
            int totalMins = 0;
            TimeSpan ts;
            ts = endDate - startDate;
            totalMins = Convert.ToInt32(ts.TotalMinutes);
            // Get AccDpt assets.
            List<Asset> assets;
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                assets = db.Assets.Where(a => a.AccDpt == v.AccDpt).ToList();
            }
            else
            {
                assets = db.Assets.ToList();
            }
            if (!string.IsNullOrEmpty(v.AssetClass1))
            {
                assets = assets.Where(a => a.AssetClass == v.AssetClass1).ToList();
            }
            if (!string.IsNullOrEmpty(v.AssetClass2))
            {
                assets = assets.Where(a => a.AssetClass == v.AssetClass2).ToList();
            }
            foreach (var item in assets)
            {
                int repairMins = 0;
                MonthFailRateVModel m = new MonthFailRateVModel();
                var repairDocs = db.Repairs.Where(r => r.AssetNo == item.AssetNo)
                                           .Join(db.RepairDtls, r => r.DocId, rd => rd.DocId,
                                           (r, rd) => new {
                                               repair = r,
                                               repairDtl = rd
                                           }).Where(r => r.repairDtl.EndDate != null);
                if (repairDocs.Count() > 0)
                {
                    foreach(var r in repairDocs)
                    {
                        TimeSpan ts2 = r.repairDtl.EndDate.Value.AddDays(1) - r.repair.ApplyDate.Value;
                        repairMins += Convert.ToInt32(ts2.TotalMinutes);
                    }
                }
                //
                m.AssetNo = item.AssetNo;
                m.Cname = item.Cname;
                m.CustId = item.AccDpt;
                m.CustNam = db.Departments.Find(m.CustId).Name_C;
                m.RepairMins = repairMins;
                m.TotalMins = totalMins;
                m.FailRate = decimal.Round(m.RepairMins / m.TotalMins, 4).ToString("P");
                mv.Add(m);
            }

            return mv;
        }

        //public List<MonthFailRateVModel> MonthFailRate(ReportQryVModel v)
        //{

        //    List<MonthFailRateVModel> mv = new List<MonthFailRateVModel>();
        //    MonthFailRateVModel m;

        //    foreach (Department p in db.Departments.ToList())
        //    {
        //        m = new MonthFailRateVModel();
        //        m.CustId = p.DptId;
        //        m.CustNam = p.Name_C;
        //        m.RepairAmt =
        //            db.RepairDtls.Where(d => d.EndDate >= v.Sdate)
        //            .Where(d => d.EndDate <= v.Edate)
        //            .Join(db.Repairs, rd => rd.DocId, r => r.DocId,
        //            (rd, r) => new
        //            {
        //                rd.DocId,
        //                r.AccDpt,
        //                r.AssetNo
        //            }).Join(db.Assets.Where(r => r.AccDpt == p.DptId)
        //            .Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
        //            .Where(a => a.DisposeKind == "正常"), rd => rd.AssetNo, r => r.AssetNo,
        //            (rd, r) => new
        //            {
        //                rd.DocId,
        //                r.AccDpt,
        //                r.AssetClass
        //            }).Count();
        //        //
        //        m.PlantAmt = db.Assets.Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
        //            .Where(r => r.AccDpt == p.DptId)
        //            .Where(a => a.DisposeKind == "正常")
        //            .Count();
        //        if (m.PlantAmt > 0)
        //            m.FailRate = decimal.Round(Convert.ToDecimal(m.RepairAmt) / Convert.ToDecimal(m.PlantAmt) * 100m, 2);
        //        else
        //            m.FailRate = 0m;
        //        mv.Add(m);
        //    }
        //    if (!string.IsNullOrEmpty(v.AccDpt))
        //    {
        //        mv = mv.Where(vv => vv.CustId == v.AccDpt).ToList();
        //    }
        //    return mv;
        //}

        public void ExcelMR(ReportQryVModel v)
        {
            DataTable dt = new DataTable();
            DataRow dw;
            dt.Columns.Add("表單編號");
            dt.Columns.Add("請修日期");
            dt.Columns.Add("完工日期");
            dt.Columns.Add("財產編號");
            dt.Columns.Add("成本中心");
            dt.Columns.Add("故障描述");
            dt.Columns.Add("故障原因");
            dt.Columns.Add("處理狀況");
            dt.Columns.Add("處理描述");
            dt.Columns.Add("維修方式");
            dt.Columns.Add("維修費用");
            dt.Columns.Add("工程師");
            dt.Columns.Add("總工時");
            List<MonthRepairVModel> mv = MonthRepair(v);
            mv.ForEach(m =>
            {
                dw = dt.NewRow();
                dw[0] = m.DocId;
                dw[1] = m.ApplyDate;
                dw[2] = m.EndDate;
                dw[3] = m.AssetNo + m.AssetNam;
                dw[4] = m.AccDpt + m.AccDptNam;
                dw[5] = m.TroubleDes;
                dw[6] = m.FailFactor;
                dw[7] = m.DealState;
                dw[8] = m.DealDes;
                dw[9] = m.InOut;
                dw[10] = m.Cost;
                dw[11] = m.EngNam;
                dw[12] = m.Hour;
                dt.Rows.Add(dw);
            });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("月維修清單");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=MonthRepair.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

        }

        public List<MonthRepairVModel> MonthRepair(ReportQryVModel v)
        {

            List<MonthRepairVModel> mv = new List<MonthRepairVModel>();

            mv = db.RepairDtls
           .Join(db.Repairs.Where(d => d.ApplyDate >= v.Sdate)
           .Where(d => d.ApplyDate <= v.Edate), rd => rd.DocId, k => k.DocId,
           (rd, k) => new
           {
               rd.DocId,
               k.AccDpt,
               k.ApplyDate,
               k.AssetNo,
               rd.Cost,
               rd.EndDate,
               rd.FailFactor,
               rd.DealDes,
               rd.DealState,
               k.TroubleDes,
               rd.InOut,
               k.AssetName,
               rd.Hour,
               k.PlantClass
           })
           .Join(db.Assets, k => k.AssetNo, at => at.AssetNo,
           (k, at) => new
           {
               k.DocId,
               k.AccDpt,
               k.ApplyDate,
               k.AssetNo,
               at.Cname,
               k.Cost,
               k.EndDate,
               k.TroubleDes,
               k.FailFactor,
               k.DealDes,
               k.DealState,
               k.InOut,
               at.Type,
               at.AssetClass,
               k.AssetName,
               k.Hour,
               k.PlantClass
           })
           .Join(db.Departments, k => k.AccDpt, c => c.DptId,
           (k, c) => new
           {
               k.DocId,
               k.AccDpt,
               c.Name_C,
               ApplyDate = k.ApplyDate.Value,
               k.AssetNo,
               k.Cname,
               Cost = k.Cost,
               EndDate = k.EndDate,
               k.FailFactor,
               k.TroubleDes,
               k.DealDes,
               k.DealState,
               k.InOut,
               k.Type,
               k.AssetClass,
               k.AssetName,
               k.Hour,
               k.PlantClass
           })
           .GroupJoin(db.RepairEmps, k => k.DocId, ke => ke.DocId,
            (k, ke) => new { k, ke })
            .SelectMany(oi => oi.ke.DefaultIfEmpty(),
            (k, ke) => new
           {
               k.k.DocId,
               k.k.AccDpt,
               k.k.Name_C,
               k.k.ApplyDate,
               k.k.AssetNo,
               k.k.Cname,
               k.k.Cost,
               k.k.EndDate,
               k.k.TroubleDes,
               k.k.FailFactor,
               k.k.DealDes,
               k.k.DealState,
               k.k.InOut,
               k.k.Type,
               k.k.AssetClass,
               ke.UserId,
               k.k.AssetName,
               k.k.Hour,
               k.k.PlantClass
           })
            .GroupJoin(db.AppUsers, k => k.UserId, u => u.Id,
            (k, u) => new { k, u })
            .SelectMany(ui => ui.u.DefaultIfEmpty(),
            (k, u) => new MonthRepairVModel
           {
               DocId = k.k.DocId,
               AccDpt = k.k.AccDpt,
               AccDptNam = k.k.Name_C,
               ApplyDate = k.k.ApplyDate,
               AssetNo = k.k.AssetNo == "000" ? "無財編" : k.k.AssetNo,
               AssetNam = k.k.AssetNo == "000" ? k.k.AssetName : k.k.Cname,
               Cost = k.k.Cost,
               EndDate = k.k.EndDate,
               FailFactor = k.k.FailFactor,
               DealDes = k.k.DealDes,
               DealState = k.k.DealState,
               InOut = k.k.InOut,
               TroubleDes = k.k.TroubleDes,
               Type = k.k.Type,
               EngNam = u.FullName,
               AssetClass = k.k.AssetClass,
               Hour = k.k.Hour,
               PlantClass = k.k.PlantClass
           }).Where(m => m.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1)).ToList();

            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                mv = mv.Where(vv => vv.AccDpt == v.AccDpt).ToList();
            }
            return mv;
        }

        public void ExcelMK(ReportQryVModel v)
        {
            DataTable dt = new DataTable();
            DataRow dw;
            dt.Columns.Add("表單編號");
            dt.Columns.Add("送單日期");
            dt.Columns.Add("完工日期");
            dt.Columns.Add("財產編號");
            dt.Columns.Add("成本中心");
            dt.Columns.Add("意見描述");
            dt.Columns.Add("保養方式");
            dt.Columns.Add("保養週期");
            dt.Columns.Add("保養費用");
            dt.Columns.Add("工程師");
            List<MonthKeepVModel> mv = MonthKeep(v);
            mv.ForEach(m =>
            {
                dw = dt.NewRow();
                dw[0] = m.DocId;
                dw[1] = m.SentDate;
                dw[2] = m.EndDate;
                dw[3] = m.AssetNo + m.AssetNam;
                dw[4] = m.AccDpt + m.AccDptNam;
                dw[5] = m.DealDes;
                dw[6] = m.InOut;
                dw[7] = m.Cycle;
                dw[8] = m.Cost;
                dw[9] = m.EngNam;
                dt.Rows.Add(dw);
            });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("月保養清單");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=MonthKeep.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }

        }
        public List<MonthKeepVModel> MonthKeep(ReportQryVModel v)
        {

            List<MonthKeepVModel> mv = new List<MonthKeepVModel>();
            string s = "";
            db.KeepDtls.Where(d => d.EndDate >= v.Sdate)
           .Where(d => d.EndDate <= v.Edate)
           .Join(db.Keeps, rd => rd.DocId, k => k.DocId,
           (rd, k) => new
           {
               rd.DocId,
               k.AccDpt,
               k.SentDate,
               k.AssetNo,
               k.Cycle,
               rd.Cost,
               rd.EndDate,
               rd.Memo,
               rd.InOut
           })
           .Join(db.Assets, k => k.AssetNo, at => at.AssetNo,
           (k, at) => new
           {
               k.DocId,
               k.AccDpt,
               k.SentDate,
               k.AssetNo,
               at.Cname,
               k.Cycle,
               k.Cost,
               k.EndDate,
               k.Memo,
               k.InOut,
               at.AssetClass
           })
           .Join(db.Departments, k => k.AccDpt, c => c.DptId,
           (k, c) => new
           {
               k.DocId,
               k.AccDpt,
               c.Name_C,
               SentDate = k.SentDate.Value,
               k.AssetNo,
               k.Cname,
               k.Cycle,
               Cost = k.Cost == null ? 0 : k.Cost.Value,
               EndDate = k.EndDate.Value,
               k.Memo,
               k.InOut,
               k.AssetClass
           })
           .GroupJoin(db.KeepEmps, k => k.DocId, ke => ke.DocId,
            (k, ke) => new { k, ke })
            .SelectMany(oi => oi.ke.DefaultIfEmpty(),
            (k, ke) => new
            {
                k.k.DocId,
                k.k.AccDpt,
                k.k.Name_C,
                k.k.SentDate,
                k.k.AssetNo,
                k.k.Cname,
                k.k.Cycle,
                k.k.Cost,
                k.k.EndDate,
                k.k.Memo,
                k.k.InOut,
                k.k.AssetClass,
                ke.UserId
            })
            .GroupJoin(db.AppUsers, k => k.UserId, u => u.Id,
            (k, u) => new { k, u })
            .SelectMany(ui => ui.u.DefaultIfEmpty(),
            (k, u) => new MonthKeepVModel
            {
                DocId = k.k.DocId,
                AccDpt = k.k.AccDpt,
                AccDptNam = k.k.Name_C,
                SentDate = k.k.SentDate,
                AssetNo = k.k.AssetNo,
                AssetNam = k.k.Cname,
                Cycle = k.k.Cycle,
                Cost = k.k.Cost,
                EndDate = k.k.EndDate,
                DealDes = k.k.Memo,
                InOut = k.k.InOut,
                AssetClass = k.k.AssetClass,
                EngNam = u.FullName
            }).Where(m => m.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1)).ToList()
            .ForEach(m =>
            {
                switch (m.InOut)
                {
                    case "0":
                        s = "自行";
                        break;
                    case "1":
                        s = "委外";
                        break;
                    case "2":
                        s = "租賃";
                        break;
                    case "3":
                        s = "保固";
                        break;
                    case "4":
                        s = "借用";
                        break;
                }
                m.InOut = s;
                mv.Add(m);
            });
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                mv = mv.Where(vv => vv.AccDpt == v.AccDpt).ToList();
            }
            return mv;
        }
        public ActionResult MonthFailRateExcel()
        {
            return PartialView();
        }

        public List<RepairKeepVModel> RepairKeep(ReportQryVModel v)
        {

            List<RepairKeepVModel> mv = new List<RepairKeepVModel>();
            RepairKeepVModel m;
            int rcnt = 0;
            int kcnt = 0;
            decimal tolcost = 0m;

            foreach (Department p in db.Departments.ToList())
            {
                m = new RepairKeepVModel();
                m.CustId = p.DptId;
                m.CustNam = p.Name_C;
                rcnt = 0;
                kcnt = 0;
                tolcost = 0m;
                var ss = new[] { "?", "2" };
                List<Repair> rs = db.Repairs.Where(r => r.ApplyDate >= v.Sdate)
                    .Where(r => r.ApplyDate <= v.Edate)
                    .Join(db.RepairFlows.Where(f => ss.Contains(f.Status)), r => r.DocId, f => f.DocId,
                    (r, f) => r).Join(db.Assets
                          .Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
                          .Where(r => r.AccDpt == p.DptId), rd => rd.AssetNo, r => r.AssetNo,
                          (rd, r) => rd).ToList();
                //
                rcnt = rs.Join(db.RepairDtls.Where(d => d.EndDate != null),
                          rd => rd.DocId, r => r.DocId,
                          (rd, r) => rd).ToList().Count();
                m.RpEndAmt = rcnt;
                m.RepairAmt = rs.Count();
                if (rcnt > 0)
                {
                    m.RepFinishedRate =
                        decimal.Round(Convert.ToDecimal(rcnt) / Convert.ToDecimal(m.RepairAmt) * 100m, 2);
                }
                else
                    m.RepFinishedRate = 0m;
                //目前沒有維護費用所以先省略
                tolcost = 0m;
                tolcost = rs.Join(db.RepairCosts.Where(c => c.TotalCost > 0), rd => rd.DocId, c => c.DocId,
                         (rd, c) => c).Select(c => c.TotalCost).DefaultIfEmpty(0).Sum();

                m.RepCost = tolcost;
                //
                List<Keep> ks = db.Keeps.Where(r => r.SentDate >= v.Sdate)
                   .Where(r => r.SentDate <= v.Edate)
                   .Join(db.KeepFlows.Where(f => ss.Contains(f.Status)), r => r.DocId, f => f.DocId,
                   (r, f) => r).Join(db.Assets
                          .Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
                          .Where(r => r.AccDpt == p.DptId), rd => rd.AssetNo, r => r.AssetNo,
                          (rd, r) => rd).ToList();

                kcnt = ks.Join(db.KeepDtls.Where(d => d.EndDate != null),
                          rd => rd.DocId, r => r.DocId,
                          (rd, r) => rd).ToList()
                          .Count();
                m.KpEndAmt = kcnt;
                m.KeepAmt = ks.Count();
                if (kcnt > 0)
                {
                    m.KeepFinishedRate =
                        decimal.Round(Convert.ToDecimal(kcnt) / Convert.ToDecimal(m.KeepAmt) * 100m, 2);
                }
                else
                    m.KeepFinishedRate = 0m;
                tolcost = 0m;
                tolcost = ks.Join(db.KeepCosts.Where(c => c.TotalCost > 0), rd => rd.DocId, c => c.DocId,
                          (rd, c) => c).Select(c => c.TotalCost).DefaultIfEmpty(0).Sum();

                m.KeepCost = tolcost;
                mv.Add(m);
            }
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                mv = mv.Where(vv => vv.CustId == v.AccDpt).ToList();
            }
            return mv;
        }
        public List<RepairKeepVModel> RepairCost(ReportQryVModel v)
        {

            List<RepairKeepVModel> mv = new List<RepairKeepVModel>();
            RepairKeepVModel m;
            int rcnt = 0;
            int kcnt = 0;
            decimal tolcost = 0m;

            foreach (Department p in db.Departments.ToList())
            {
                m = new RepairKeepVModel();
                m.CustId = p.DptId;
                m.CustNam = p.Name_C;
                rcnt = 0;
                kcnt = 0;
                tolcost = 0m;
                var ss = new[] { "?", "2" };
                List<Repair> rs = db.Repairs.Where(r => r.ApplyDate >= v.Sdate)
                    .Where(r => r.ApplyDate <= v.Edate)
                    .Join(db.RepairFlows.Where(f => ss.Contains(f.Status)), r => r.DocId, f => f.DocId,
                    (r, f) => r).Join(db.Assets
                          .Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
                          .Where(r => r.AccDpt == p.DptId), rd => rd.AssetNo, r => r.AssetNo,
                          (rd, r) => rd).ToList();
                //
                rcnt = rs.Join(db.RepairDtls.Where(d => d.EndDate != null),
                          rd => rd.DocId, r => r.DocId,
                          (rd, r) => rd).ToList().Count();
                m.RpEndAmt = rcnt;
                m.RepairAmt = rs.Count();
                if (rcnt > 0)
                {
                    m.RepFinishedRate =
                        decimal.Round(Convert.ToDecimal(rcnt) / Convert.ToDecimal(m.RepairAmt) * 100m, 2);
                }
                else
                    m.RepFinishedRate = 0m;
                //目前沒有維護費用所以先省略
                tolcost = 0m;
                tolcost = rs.Join(db.RepairCosts.Where(c => c.TotalCost > 0), rd => rd.DocId, c => c.DocId,
                         (rd, c) => c).Select(c => c.TotalCost).DefaultIfEmpty(0).Sum();

                m.RepCost = tolcost;
                //
                List<Keep> ks = db.Keeps.Where(r => r.SentDate >= v.Sdate)
                   .Where(r => r.SentDate <= v.Edate)
                   .Join(db.KeepFlows.Where(f => ss.Contains(f.Status)), r => r.DocId, f => f.DocId,
                   (r, f) => r).Join(db.Assets
                          .Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
                          .Where(r => r.AccDpt == p.DptId), rd => rd.AssetNo, r => r.AssetNo,
                          (rd, r) => rd).ToList();

                kcnt = ks.Join(db.KeepDtls.Where(d => d.EndDate != null),
                          rd => rd.DocId, r => r.DocId,
                          (rd, r) => rd).ToList()
                          .Count();
                m.KpEndAmt = kcnt;
                m.KeepAmt = ks.Count();
                if (kcnt > 0)
                {
                    m.KeepFinishedRate =
                        decimal.Round(Convert.ToDecimal(kcnt) / Convert.ToDecimal(m.KeepAmt) * 100m, 2);
                }
                else
                    m.KeepFinishedRate = 0m;
                tolcost = 0m;
                tolcost = ks.Join(db.KeepCosts.Where(c => c.TotalCost > 0), rd => rd.DocId, c => c.DocId,
                          (rd, c) => c).Select(c => c.TotalCost).DefaultIfEmpty(0).Sum();

                m.KeepCost = tolcost;
                mv.Add(m);
            }
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                mv = mv.Where(vv => vv.CustId == v.AccDpt).ToList();
            }
            return mv;
        }
        public List<RepairKeepVModel> KeepCost(ReportQryVModel v)
        {

            List<RepairKeepVModel> mv = new List<RepairKeepVModel>();
            RepairKeepVModel m;
            int rcnt = 0;
            int kcnt = 0;
            decimal tolcost = 0m;

            foreach (Department p in db.Departments.ToList())
            {
                m = new RepairKeepVModel();
                m.CustId = p.DptId;
                m.CustNam = p.Name_C;
                rcnt = 0;
                kcnt = 0;
                tolcost = 0m;
                var ss = new[] { "?", "2" };
                List<Repair> rs = db.Repairs.Where(r => r.ApplyDate >= v.Sdate)
                    .Where(r => r.ApplyDate <= v.Edate)
                    .Join(db.RepairFlows.Where(f => ss.Contains(f.Status)), r => r.DocId, f => f.DocId,
                    (r, f) => r).Join(db.Assets
                          .Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
                          .Where(r => r.AccDpt == p.DptId), rd => rd.AssetNo, r => r.AssetNo,
                          (rd, r) => rd).ToList();
                //
                rcnt = rs.Join(db.RepairDtls.Where(d => d.EndDate != null),
                          rd => rd.DocId, r => r.DocId,
                          (rd, r) => rd).ToList().Count();
                m.RpEndAmt = rcnt;
                m.RepairAmt = rs.Count();
                if (rcnt > 0)
                {
                    m.RepFinishedRate =
                        decimal.Round(Convert.ToDecimal(rcnt) / Convert.ToDecimal(m.RepairAmt) * 100m, 2);
                }
                else
                    m.RepFinishedRate = 0m;
                //目前沒有維護費用所以先省略
                tolcost = 0m;
                tolcost = rs.Join(db.RepairCosts.Where(c => c.TotalCost > 0), rd => rd.DocId, c => c.DocId,
                         (rd, c) => c).Select(c => c.TotalCost).DefaultIfEmpty(0).Sum();

                m.RepCost = tolcost;
                //
                List<Keep> ks = db.Keeps.Where(r => r.SentDate >= v.Sdate)
                   .Where(r => r.SentDate <= v.Edate)
                   .Join(db.KeepFlows.Where(f => ss.Contains(f.Status)), r => r.DocId, f => f.DocId,
                   (r, f) => r).Join(db.Assets
                          .Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
                          .Where(r => r.AccDpt == p.DptId), rd => rd.AssetNo, r => r.AssetNo,
                          (rd, r) => rd).ToList();

                kcnt = ks.Join(db.KeepDtls.Where(d => d.EndDate != null),
                          rd => rd.DocId, r => r.DocId,
                          (rd, r) => rd).ToList()
                          .Count();
                m.KpEndAmt = kcnt;
                m.KeepAmt = ks.Count();
                if (kcnt > 0)
                {
                    m.KeepFinishedRate =
                        decimal.Round(Convert.ToDecimal(kcnt) / Convert.ToDecimal(m.KeepAmt) * 100m, 2);
                }
                else
                    m.KeepFinishedRate = 0m;
                tolcost = 0m;
                tolcost = ks.Join(db.KeepCosts.Where(c => c.TotalCost > 0), rd => rd.DocId, c => c.DocId,
                          (rd, c) => c).Select(c => c.TotalCost).DefaultIfEmpty(0).Sum();

                m.KeepCost = tolcost;
                mv.Add(m);
            }
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                mv = mv.Where(vv => vv.CustId == v.AccDpt).ToList();
            }
            return mv;
        }

        public void ExcelRF(ReportQryVModel v)
        {
            DataTable dt = new DataTable();
            DataRow dw;
            dt.Columns.Add("表單編號");
            dt.Columns.Add("請修日期");
            dt.Columns.Add("單位名稱");
            dt.Columns.Add("設備財編");
            dt.Columns.Add("設備型號");
            dt.Columns.Add("故障描述");
            dt.Columns.Add("故障原因");
            dt.Columns.Add("處理描述");
            dt.Columns.Add("完工日期");
            List<RepeatFailVModel> mv = RepeatFail(v);
            mv.ForEach(m =>
            {
                dw = dt.NewRow();
                dw[0] = m.DocId;
                dw[1] = m.ApplyDate;
                dw[2] = m.CustNam;
                dw[3] = m.AssetNo + m.AssetNam;
                dw[4] = m.Type;
                dw[5] = m.TroubleDes;
                dw[6] = m.FailFactor;
                dw[7] = m.DealDes;
                dw[8] = m.EndDate;
                dt.Rows.Add(dw);
            });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("重複故障清單");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=RepeatFail.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
        public List<RepeatFailVModel> RepeatFail(ReportQryVModel v)
        {
            List<RepeatFailVModel> mv = new List<RepeatFailVModel>();
            List<RepeatFailVModel> mv2 = new List<RepeatFailVModel>();
            RepeatFailVModel m;
            Repair r;
            RepairEmp p;
            Asset a;
            Department o;

            List<RepairDtl> rdtl = db.RepairDtls.Where(d => d.EndDate >= v.Sdate)
                .Where(d => d.EndDate <= v.Edate).ToList();

            foreach (RepairDtl rd in rdtl)
            {
                m = new RepeatFailVModel();
                m.DocId = rd.DocId;
                m.DealDes = rd.DealDes;
                m.EndDate = rd.EndDate.Value;
                m.Cost = rd.Cost;
                m.FailFactor = rd.FailFactor;
                r = db.Repairs.Where(i => i.DocId == rd.DocId).ToList().FirstOrDefault();
                if (r != null)
                {
                    m.TroubleDes = r.TroubleDes;
                    m.CustId = r.AccDpt;
                    o = db.Departments.Where(c => c.DptId == r.AccDpt).ToList().FirstOrDefault();
                    if (o != null)
                        m.CustNam = o.Name_C;
                    m.ApplyDate = r.ApplyDate.Value;
                    m.AssetNo = r.AssetNo;
                    a = db.Assets.Where(s => s.AssetNo == r.AssetNo)
                            .Where(s => s.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1)).FirstOrDefault();
                    if (a != null)
                    {
                        m.AssetNam = a.Cname;
                        m.Type = a.Type;
                    }
                }
                p = db.RepairEmps.Where(e => e.DocId == rd.DocId).ToList().FirstOrDefault();
                if (p != null)
                {
                    AppUser u = db.AppUsers.Find(p.UserId);
                    m.EngNam = u.FullName;
                }
                mv2.Add(m);
            }
            IEnumerable<IGrouping<string, RepeatFailVModel>> query = mv2.GroupBy(s => s.AssetNo);
            foreach (var group in query)
            {
                if (group.ToList().Count >= 2)
                {
                    foreach (RepeatFailVModel l in group)
                    {
                        mv.Add(l);
                    }
                }
            }
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                mv = mv.Where(vv => vv.CustId == v.AccDpt).ToList();
            }
            return mv;
        }

        public List<RepKeepStokVModel> RepKeepStok(ReportQryVModel v)
        {
            List<RepKeepStokVModel> mv = new List<RepKeepStokVModel>();
            List<Cust> cv;
            cv =
           db.RepairDtls.Where(d => d.CloseDate >= v.Sdate &&
               d.CloseDate <= v.Edate)
               .Join(db.Repairs, rd => rd.DocId, r => r.DocId,
               (rd, r) => new
               {
                   rd.DocId,
                   r.AccDpt,
                   r.AssetNo
               }).Join(db.Assets, rd => rd.AssetNo, r => r.AssetNo,
               (rd, r) => new
               {
                   rd.DocId,
                   r.AccDpt,
                   r.AssetClass
               }).Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
               .Join(db.Departments, rd => rd.AccDpt, c => c.DptId,
               (rd, c) => new Cust
               {
                   CustId = c.DptId,
                   CustNam = c.Name_C
               }).Union(
          db.KeepDtls.Where(d => d.CloseDate >= v.Sdate &&
              d.CloseDate <= v.Edate)
              .Join(db.Keeps, rd => rd.DocId, r => r.DocId,
              (rd, r) => new
              {
                  rd.DocId,
                  r.AccDpt,
                  r.AssetNo
              }).Join(db.Assets, rd => rd.AssetNo, r => r.AssetNo,
               (rd, r) => new
               {
                   rd.DocId,
                   r.AccDpt,
                   r.AssetClass
               }).Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
              .Join(db.Departments, rd => rd.AccDpt, c => c.DptId,
              (rd, c) => new Cust
              {
                  CustId = c.DptId,
                  CustNam = c.Name_C
              })).Distinct().ToList();


            RepKeepStokVModel m;
            int rcnt = 0;
            int kcnt = 0;
            decimal tolcost = 0m;
            List<RepTotalPrice> pp = new List<RepTotalPrice>();
            foreach (Cust co in cv)
            {
                rcnt = 0;
                tolcost = 0m;
                pp.Clear();
                m = new RepKeepStokVModel();
                m.CustId = co.CustId;
                m.CustNam = co.CustNam;
                rcnt = db.RepairDtls.Where(d => d.CloseDate >= v.Sdate &&
                d.CloseDate <= v.Edate)
                .Join(db.Repairs, rd => rd.DocId, r => r.DocId,
                (rd, r) => new
                {
                    rd.DocId,
                    r.AccDpt
                }).Where(r => r.AccDpt == co.CustId).Count();
                pp = db.RepairDtls.Where(d => d.CloseDate >= v.Sdate &&
                d.CloseDate <= v.Edate)
                .Join(db.Repairs, rd => rd.DocId, r => r.DocId,
                (rd, r) => new
                {
                    rd.DocId,
                    r.AccDpt
                }).Where(r => r.AccDpt == co.CustId).Join(db.RepairCosts, rd => rd.DocId, c => c.DocId,
                (rd, c) => new RepTotalPrice
                {
                    DocId = rd.DocId,
                    totalprice = c.TotalCost
                }).ToList();
                if (pp.Count > 0)
                    tolcost = pp.Sum(p => p.totalprice);
                m.RepairAmt = rcnt;
                m.RepairCost = tolcost;
                //
                kcnt = 0;
                tolcost = 0;
                kcnt = db.KeepDtls.Where(d => d.CloseDate >= v.Sdate &&
                d.CloseDate <= v.Edate)
               .Join(db.Keeps, rd => rd.DocId, r => r.DocId,
               (rd, r) => new
               {
                   rd.DocId,
                   r.AccDpt
               }).Where(r => r.AccDpt == co.CustId).Count();
                pp.Clear();
                pp = db.KeepDtls.Where(d => d.CloseDate >= v.Sdate &&
                d.CloseDate <= v.Edate)
                .Join(db.Keeps, rd => rd.DocId, r => r.DocId,
                (rd, r) => new
                {
                    rd.DocId,
                    r.AccDpt
                }).Where(r => r.AccDpt == co.CustId).Join(db.KeepCosts, rd => rd.DocId, c => c.DocId,
                (rd, c) => new RepTotalPrice
                {
                    DocId = rd.DocId,
                    totalprice = c.TotalCost
                }).ToList();
                if (pp.Count > 0)
                    tolcost = pp.Sum(p => p.totalprice);
                m.KeepAmt = kcnt;
                m.KeepCost = tolcost;
                //
                m.TotalAmt = m.RepairAmt + m.KeepAmt;
                m.TotalCost = m.RepairCost + m.KeepCost;
                mv.Add(m);
            }
            //
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                mv = mv.Where(vv => vv.CustId == v.AccDpt).ToList();
            }
            //
            return mv;
        }

        public List<RpKpStokBdVModel> RpKpStokBd(ReportQryVModel v)
        {
            List<RpKpStokBdVModel> sv = new List<RpKpStokBdVModel>();
            List<RpKpStokBdVModel> sv2 = new List<RpKpStokBdVModel>();
            RpKpStokBdVModel rb;
            var scv = db.RepairDtls.Where(d => d.CloseDate >= v.Sdate &&
                d.CloseDate <= v.Edate)
                .Join(db.Repairs, rd => rd.DocId, r => r.DocId,
                (rd, r) => new
                {
                    rd.DocId,
                    r.AccDpt,
                    r.AssetNo
                }).Join(db.Assets, rd => rd.AssetNo, r => r.AssetNo,
                (rd, r) => new
                {
                    rd.DocId,
                    rd.AccDpt,
                    r.AssetClass
                }).Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
                .Join(db.Departments, rd => rd.AccDpt, c => c.DptId,
                (rd, c) => new
                {
                    rd.DocId,
                    CustId = c.DptId,
                    CustNam = c.Name_C
                })
                .Join(db.RepairCosts, rd => rd.DocId, rc => rc.DocId,
                (rd, rc) => new
                {
                    DocTyp = "請修",
                    rd.CustId,
                    rd.CustNam,
                    rc.PartNo,
                    rc.Qty,
                    rc.TotalCost
                })
                .Union(
                db.KeepDtls.Where(d => d.CloseDate >= v.Sdate &&
                d.CloseDate <= v.Edate)
                .Join(db.Keeps, rd => rd.DocId, r => r.DocId,
                (rd, r) => new
                {
                    rd.DocId,
                    r.AccDpt,
                    r.AssetNo
                }).Join(db.Assets, rd => rd.AssetNo, r => r.AssetNo,
                (rd, r) => new
                {
                    rd.DocId,
                    rd.AccDpt,
                    r.AssetClass
                }).Where(r => r.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1))
                .Join(db.Departments, rd => rd.AccDpt, c => c.DptId,
                (rd, c) => new
                {
                    rd.DocId,
                    //c.GroupId,
                    CustId = c.DptId,
                    CustNam = c.Name_C
                })
                .Join(db.KeepCosts, rd => rd.DocId, rc => rc.DocId,
                (rd, rc) => new
                {
                    DocTyp = "保養",
                    rd.CustId,
                    rd.CustNam,
                    rc.PartNo,
                    rc.Qty,
                    rc.TotalCost
                })
                ).GroupBy(rd => rd.CustId).ToList();

            foreach (var a in scv)
            {
                //var scv2 = a.Where(z => z.DocTyp == "請修")
                //    .Join(db.DeptStoks, x => x.PartNo, d => d.StokNo,
                //    (x, d) => new
                //    {
                //        d.Brand,
                //        x.TotalCost
                //    }).GroupBy(x => x.Brand).ToList();
                var scv2 = a.Where(z => z.DocTyp == "請修")
                    .Join(db.DeptStoks, x => x.PartNo, d => d.StokNo,
                    (x, d) => new
                    {
                        d.Brand,
                        d.StokNo,
                        d.StokNam,
                        x.Qty,
                        x.TotalCost
                    }).GroupBy(x => new { x.Brand, x.StokNo, x.StokNam }).ToList();
                foreach (var b in scv2)
                {
                    rb = new RpKpStokBdVModel();
                    rb.CustId = a.Key;
                    rb.CustNam = db.Departments.Find(a.Key).Name_C;
                    rb.DocTyp = "請修";
                    rb.Brand = b.Key.Brand;
                    rb.StokNo = b.Key.StokNo;
                    rb.StokNam = b.Key.StokNam;
                    rb.Amt = b.Sum(x => x.Qty);
                    rb.Up1000 = b.Where(x => x.TotalCost >= 1000).Sum(x => x.TotalCost);
                    rb.Dn1000 = b.Where(x => x.TotalCost < 1000).Sum(x => x.TotalCost);
                    rb.Cost = b.Sum(x => x.TotalCost);
                    sv.Add(rb);
                }
                //
                scv2.Clear();
                scv2 = a.Where(z => z.DocTyp == "保養")
                    .Join(db.DeptStoks, x => x.PartNo, d => d.StokNo,
                    (x, d) => new
                    {
                        d.Brand,
                        d.StokNo,
                        d.StokNam,
                        x.Qty,
                        x.TotalCost
                    }).GroupBy(x => new { x.Brand, x.StokNo, x.StokNam }).ToList();
                foreach (var b in scv2)
                {
                    rb = new RpKpStokBdVModel();
                    rb.CustId = a.Key;
                    rb.CustNam = db.Departments.Find(a.Key).Name_C;
                    rb.DocTyp = "保養";
                    rb.Brand = b.Key.Brand;
                    rb.StokNo = b.Key.StokNo;
                    rb.StokNam = b.Key.StokNam;
                    rb.Amt = b.Sum(x => x.Qty);
                    rb.Up1000 = b.Where(x => x.TotalCost >= 1000).Sum(x => x.TotalCost);
                    rb.Dn1000 = b.Where(x => x.TotalCost < 1000).Sum(x => x.TotalCost);
                    rb.Cost = b.Sum(x => x.TotalCost);
                    sv.Add(rb);
                }
                //sv.Add(rb);
            }
            //
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                sv = sv.Where(vv => vv.CustId == v.AccDpt).ToList();
            }
            //
            return sv;
        }

        public void ExcelSC(ReportQryVModel v)
        {
            string str = "";
            str += "類別,表單編號,送單日期,完工日期,財產編號,設備名稱,型號,成本中心,成本中心名稱,";
            str += "故障描述/保養週期,故障原因,處理描述,零件廠牌,料號,零件名稱,數量,單價,合計,結案日,工程師";
            DataTable dt = new DataTable();
            DataRow dw;
            str.Split(new char[] { ',' }).ToList()
                .ForEach(s =>
                {
                    dt.Columns.Add(s);
                });
            List<StokCostVModel> uv = StokCost(v);
            uv.ForEach(m =>
            {
                dw = dt.NewRow();
                dw[0] = m.DocTyp;
                dw[1] = m.DocId;
                dw[2] = m.ApplyDate;
                dw[3] = m.EndDate;
                dw[4] = m.AssetNo;
                dw[5] = m.AssetNam;
                dw[6] = m.Type;
                dw[7] = m.AccDpt;
                dw[8] = m.AccDptNam;
                dw[9] = m.TroubleDes;
                dw[10] = m.FailFactor;
                dw[11] = m.DealDes;
                dw[12] = m.Brand;
                dw[13] = m.StokNo;
                dw[14] = m.StokNam;
                dw[15] = m.Qty;
                dw[16] = m.Price;
                dw[17] = m.TotalPrice;
                dw[18] = m.CloseDate;
                dw[19] = m.EngNam;
                dt.Rows.Add(dw);
            });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("零件帳務清單");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=StokCost.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
        public List<StokCostVModel> StokCost(ReportQryVModel v)
        {
            List<StokCostVModel> sv = new List<StokCostVModel>();
            List<StokCostVModel> sv2 = new List<StokCostVModel>();

            sv = db.RepairDtls.Where(d => d.EndDate >= v.Sdate
                && d.EndDate <= v.Edate)
            .Join(db.Repairs, rd => rd.DocId, k => k.DocId,
            (rd, k) => new
            {
                rd.DocId,
                k.AccDpt,
                k.ApplyDate,
                k.AssetNo,
                rd.Cost,
                rd.EndDate,
                rd.CloseDate,
                rd.FailFactor,
                rd.DealDes,
                k.TroubleDes
            })
            .Join(db.Assets, k => k.AssetNo, at => at.AssetNo,
            (k, at) => new
            {
                k.DocId,
                k.AccDpt,
                k.ApplyDate,
                k.AssetNo,
                at.Cname,
                k.Cost,
                k.EndDate,
                k.CloseDate,
                k.FailFactor,
                k.TroubleDes,
                k.DealDes,
                at.Type,
                at.AssetClass
            })
            .GroupJoin(db.RepairCosts, k => k.DocId, rc => rc.DocId,
            (k, rc) => new { k, rc })
            .SelectMany(oi => oi.rc.DefaultIfEmpty(),
           (k, rc) => new
           {
               k.k.DocId,
               k.k.AccDpt,
               k.k.ApplyDate,
               k.k.AssetNo,
               k.k.Cname,
               k.k.Cost,
               k.k.EndDate,
               k.k.CloseDate,
               k.k.FailFactor,
               k.k.TroubleDes,
               k.k.DealDes,
               k.k.Type,
               k.k.AssetClass,
               PartNo = rc.PartNo != null ? rc.PartNo : "",
               PartNam = rc.PartName != null ? rc.PartName : "",
               Qty = rc.Qty != null ? rc.Qty : 0,
               Price = rc.Price != null ? rc.Price : 0,
               TotalCost = rc.TotalCost != null ? rc.TotalCost : 0
           })
             .Join(db.Departments, k => k.AccDpt, c => c.DptId,
            (k, c) => new
            {
                k.DocId,
                k.AccDpt,
                k.ApplyDate,
                k.AssetNo,
                k.Cname,
                k.Cost,
                k.EndDate,
                k.CloseDate,
                k.FailFactor,
                k.TroubleDes,
                k.DealDes,
                k.Type,
                k.PartNo,
                k.PartNam,
                k.AssetClass,
                k.Qty,
                k.Price,
                k.TotalCost,
                c.Name_C
            })
             .Join(db.RepairEmps, k => k.DocId, ke => ke.DocId,
            (k, ke) => new
            {
                k.DocId,
                k.AccDpt,
                k.ApplyDate,
                k.AssetNo,
                k.Cname,
                k.Cost,
                k.EndDate,
                k.CloseDate,
                k.FailFactor,
                k.TroubleDes,
                k.DealDes,
                k.Type,
                k.PartNo,
                k.PartNam,
                k.AssetClass,
                k.Qty,
                k.Price,
                k.TotalCost,
                k.Name_C,
                ke.UserId
            })
            .Join(db.AppUsers, k => k.UserId, u => u.Id,
            (k, u) => new StokCostVModel
            {
                DocTyp = "請修",
                DocId = k.DocId,
                AccDpt = k.AccDpt,
                AccDptNam = k.Name_C,
                AssetNo = k.AssetNo,
                AssetNam = k.Cname,
                Type = k.Type,
                ApplyDate = k.ApplyDate.Value,
                EndDate = k.EndDate,
                CloseDate = k.CloseDate,
                FailFactor = k.FailFactor,
                TroubleDes = k.TroubleDes,
                DealDes = k.DealDes,
                StokNo = k.PartNo,
                StokNam = k.PartNam,
                Qty = k.Qty,
                Price = k.Price,
                TotalPrice = k.TotalCost,
                EngNam = u.FullName,
                AssetClass = k.AssetClass
            }).ToList();
            //
            foreach (StokCostVModel s in sv)
            {
                if (s.StokNo != "")
                    s.Cost = db.RepairCosts.Where(r => r.DocId == s.DocId).Sum(r => r.TotalCost);
            }
            //保養
            sv2 = db.KeepDtls.Where(d => d.EndDate >= v.Sdate
               && d.EndDate <= v.Edate)
           .Join(db.Keeps, rd => rd.DocId, k => k.DocId,
           (rd, k) => new
           {
               rd.DocId,
               k.AccDpt,
               k.SentDate,
               k.AssetNo,
               rd.Cost,
               rd.EndDate,
               rd.CloseDate,
               rd.Result,
               k.Cycle
           })
           .Join(db.Assets, k => k.AssetNo, at => at.AssetNo,
           (k, at) => new
           {
               k.DocId,
               k.AccDpt,
               k.SentDate,
               k.AssetNo,
               at.Cname,
               k.Cost,
               k.EndDate,
               k.CloseDate,
               k.Result,
               k.Cycle,
               at.Type,
               at.AssetClass
           })
            .GroupJoin(db.KeepCosts, k => k.DocId, rc => rc.DocId,
            (k, rc) => new { k, rc })
            .SelectMany(oi => oi.rc.DefaultIfEmpty(),
           (k, rc) => new
           {
               k.k.DocId,
               k.k.AccDpt,
               k.k.SentDate,
               k.k.AssetNo,
               k.k.Cname,
               k.k.Cost,
               k.k.EndDate,
               k.k.CloseDate,
               k.k.Result,
               k.k.Cycle,
               k.k.Type,
               k.k.AssetClass,
               PartNo = rc.PartNo != null ? rc.PartNo : "",
               PartNam = rc.PartName != null ? rc.PartName : "",
               Qty = rc.Qty != null ? rc.Qty : 0,
               Price = rc.Price != null ? rc.Price : 0,
               TotalCost = rc.TotalCost != null ? rc.TotalCost : 0
           })
            .Join(db.Departments, k => k.AccDpt, c => c.DptId,
           (k, c) => new
           {
               k.DocId,
               k.AccDpt,
               k.SentDate,
               k.AssetNo,
               k.Cname,
               k.Cost,
               k.EndDate,
               k.CloseDate,
               k.Result,
               k.Cycle,
               k.Type,
               k.PartNo,
               k.PartNam,
               k.Qty,
               k.Price,
               k.TotalCost,
               k.AssetClass,
               //c.GroupId,
               c.Name_C
           })
            .GroupJoin(db.KeepEmps, k => k.DocId, ke => ke.DocId,
           (k, ke) => new { k, ke })
            .SelectMany(ee => ee.ke.DefaultIfEmpty(),
           (k, ke) => new
           {
               k.k.DocId,
               k.k.AccDpt,
               k.k.SentDate,
               k.k.AssetNo,
               k.k.Cname,
               k.k.Cost,
               k.k.EndDate,
               k.k.CloseDate,
               k.k.Result,
               k.k.Cycle,
               k.k.Type,
               k.k.PartNo,
               k.k.PartNam,
               k.k.Qty,
               k.k.Price,
               k.k.TotalCost,
               k.k.Name_C,
               k.k.AssetClass,
               UserId = ke.UserId != null ? ke.UserId : 0
           })
           .GroupJoin(db.AppUsers, k => k.UserId, u => u.Id,
           (k, u) => new { k, u })
            .SelectMany(ee => ee.u.DefaultIfEmpty(),
           (k, u) => new StokCostVModel
           {
               DocTyp = "保養",
               DocId = k.k.DocId,
               AccDpt = k.k.AccDpt,
               AccDptNam = k.k.Name_C,
               AssetNo = k.k.AssetNo,
               AssetNam = k.k.Cname,
               Type = k.k.Type,
               ApplyDate = k.k.SentDate.Value,
               EndDate = k.k.EndDate,
               CloseDate = k.k.CloseDate,
               DealDes = k.k.Result,
               StokNo = k.k.PartNo,
               StokNam = k.k.PartNam,
               Qty = k.k.Qty,
               Price = k.k.Price,
               TotalPrice = k.k.TotalCost,
               EngNam = u.Id != 0 ? u.FullName : "",
               AssetClass = k.k.AssetClass
           }).ToList();
            //
            foreach (StokCostVModel s in sv2)
            {
                switch (s.DealDes)
                {
                    case "1":
                        s.DealDes = "功能正常";
                        break;
                    case "2":
                        s.DealDes = "預防處理";
                        break;
                    case "3":
                        s.DealDes = "異常處理";
                        break;
                    case "4":
                        s.DealDes = "維修時保養";
                        break;
                    case "5":
                        s.DealDes = "退件";
                        break;
                }
                s.TroubleDes = db.Keeps.Find(s.DocId).Cycle.ToString();
                s.FailFactor = "";
                if (s.StokNo != "")
                    s.Cost = db.KeepCosts.Where(r => r.DocId == s.DocId).Sum(r => r.TotalCost);
            }
            sv.AddRange(sv2);
            sv = sv.Where(s => s.AssetClass == (v.AssetClass1 == null ? v.AssetClass2 : v.AssetClass1)).ToList();
            sv = sv.GroupJoin(db.DeptStoks, s => s.StokNo, d => d.StokNo,
                (s, d) => new { s, d })
                .SelectMany(k => k.d.DefaultIfEmpty(),
                (k, u) => new StokCostVModel
                {
                    DocTyp = k.s.DocTyp,
                    DocId = k.s.DocId,
                    AccDpt = k.s.AccDpt,
                    AccDptNam = k.s.AccDptNam,
                    AssetNo = k.s.AssetNo,
                    AssetNam = k.s.AssetNam,
                    Type = k.s.Type,
                    ApplyDate = k.s.ApplyDate,
                    EndDate = k.s.EndDate,
                    CloseDate = k.s.CloseDate,
                    DealDes = k.s.DealDes,
                    FailFactor = k.s.FailFactor,
                    TroubleDes = k.s.TroubleDes,
                    StokNo = k.s.StokNo,
                    StokNam = k.s.StokNam,
                    Qty = k.s.Qty,
                    Price = k.s.Price,
                    TotalPrice = k.s.TotalPrice,
                    EngNam = k.s.EngNam,
                    AssetClass = k.s.AssetClass,
                    Brand = u == null ? "" : u.Brand
                }).ToList();
            //
            if (!string.IsNullOrEmpty(v.AccDpt))
            {
                sv = sv.Where(vv => vv.AccDpt == v.AccDpt).ToList();
            }
            if (!string.IsNullOrEmpty(v.StockName))
            {
                sv = sv.Where(vv => !string.IsNullOrEmpty(vv.StokNam)).ToList();
                sv = sv.Where(vv => vv.StokNam.Contains(v.StockName)).ToList();
            }
            //
            return sv;
        }

        public ActionResult EffectRatio()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EffectRatio(EffectRatio effectRatio)
        {
            if (ModelState.IsValid)
            {
                effectRatio.TotalInRepCases =
                db.RepairDtls.Where(d => d.EndDate >= effectRatio.Sdate && d.EndDate <= effectRatio.Edate)
                .Where(d => d.InOut == "內修").Count();
                effectRatio.TotalKeepCases =
                    db.Keeps.Where(d => d.SentDate >= effectRatio.Sdate && d.SentDate <= effectRatio.Edate)
                    .Count();
                effectRatio.FiveDaysInRepCases =
                    db.RepairDtls.Where(d => d.EndDate >= effectRatio.Sdate && d.EndDate <= effectRatio.Edate)
                    .Where(d => d.InOut == "內修")
                    .Join(db.Repairs, r => r.DocId, d => d.DocId,
                    (r, d) => new
                    {
                        Edate = r.EndDate.Value,
                        Adate = d.ApplyDate.Value
                    }).Where(f => f.Adate >= System.Data.Entity.DbFunctions.AddDays(f.Edate, -7)).Count();
                effectRatio.KpFinishedCases =
                    db.Keeps.Where(d => d.SentDate >= effectRatio.Sdate && d.SentDate <= effectRatio.Edate)
                    .Join(db.KeepDtls.Where(k => k.EndDate != null), d => d.DocId, k => k.DocId,
                    (d, k) => k).Count();
                effectRatio.FiveInRepRatio = 0m;
                if (effectRatio.TotalInRepCases > 0)
                {
                    effectRatio.FiveInRepRatio =
                        Convert.ToDecimal(effectRatio.FiveDaysInRepCases) / Convert.ToDecimal(effectRatio.TotalInRepCases);
                }
                effectRatio.KeepFinishedRatio = 0m;
                if (effectRatio.TotalKeepCases > 0)
                {
                    effectRatio.KeepFinishedRatio =
                        Convert.ToDecimal(effectRatio.KpFinishedCases) / Convert.ToDecimal(effectRatio.TotalKeepCases);
                }
            }

            return View(effectRatio);
        }
    }
}