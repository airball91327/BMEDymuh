using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Filters;
using BMEDmgt.Models;
using OfficeOpenXml;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class AssetMaintainContractsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/AssetMaintainContracts
        public ActionResult Index()
        {
            return View();
        }

        // POST: MedEngMgt/AssetMaintainContracts
        [HttpPost]
        public ActionResult Index(FormCollection fm)
        {
            string pno = fm["qtyPurchaseNo"];
            string aname = fm["qtyASSETNAME"];
            string ano = fm["qtyASSETNO"];
            string vname = fm["qtyVendorName"];
            string vuniteno = fm["qtyVendorUniteNo"];

            //List<AssetMaintainContract> ac = new List<AssetMaintainContract>();

            var contracts = db.AssetMaintainContracts.ToList();

            if(!string.IsNullOrEmpty(pno))  //契約號
            {
                contracts = contracts.Where(c => c.PurchaseNo == pno).ToList();
            }
            if (!string.IsNullOrEmpty(aname))   //財產關鍵字
            {
                contracts = contracts.Where(c => c.AssetName.Contains(aname)).ToList();
            }
            if (!string.IsNullOrEmpty(ano))     //財產編號
            {
                contracts = contracts.Where(c => c.AssetNo == ano).ToList();
            }
            if (!string.IsNullOrEmpty(vname))   //廠商名稱關鍵字
            {
                contracts = contracts.Where(c => c.VendorName.Contains(vname)).ToList();
            }
            if (!string.IsNullOrEmpty(vuniteno))    //廠商統編
            {
                contracts = contracts.Where(c => c.VendorUniteNo == vuniteno).ToList();
            }

            return View("List", contracts);
        }

        // GET: MedEngMgt/AssetMaintainContracts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetMaintainContract assetMaintainContract = db.AssetMaintainContracts.Find(id);
            if (assetMaintainContract == null)
            {
                return HttpNotFound();
            }
            if (assetMaintainContract.ContractMgr != null)
            {
                assetMaintainContract.ContractMgrName = db.AppUsers.Find(assetMaintainContract.ContractMgr).FullName;
            }
            if (assetMaintainContract.SecondMgr != null)
            {
                assetMaintainContract.SecondMgrName = db.AppUsers.Find(assetMaintainContract.SecondMgr).FullName;
            }
            return View(assetMaintainContract);
        }

        // GET: MedEngMgt/AssetMaintainContracts/Create
        public ActionResult Create()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "全責", Value = "全責" });
            listItem.Add(new SelectListItem { Text = "半責", Value = "半責" });
            ViewData["ContractType"] = new SelectList(listItem, "Value", "Text", "全責");

            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "請選擇", Value = "" });
            ViewData["ContractMgr"] = new SelectList(listItem2, "Value", "Text", "");
            ViewData["SecondMgr"] = new SelectList(listItem2, "Value", "Text", "");

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Text = "工程類", Value = "工程類" });
            listItem3.Add(new SelectListItem { Text = "財務類", Value = "財務類" });
            listItem3.Add(new SelectListItem { Text = "勞務類", Value = "勞務類" });
            ViewData["ContractClass"] = new SelectList(listItem3, "Value", "Text", "");

            return View();
        }

        // GET: MedEngMgt/AssetMaintainContracts/Create2
        public ActionResult Create2(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AssetMaintainContract oldContract = db.AssetMaintainContracts.Find(id);
            List<SelectListItem> listItem = new List<SelectListItem>();
            if (oldContract == null)
            {
                return HttpNotFound();
            }
            if (oldContract.AssetNo != null)
            {
                listItem.Add(new SelectListItem
                {
                    Text = oldContract.AssetName + "(" + oldContract.AssetNo + ")",
                    Value = oldContract.AssetNo
                });
                ViewData["DefaultAsset"] = new SelectList(listItem, "Value", "Text", oldContract.AssetNo);
            }
            else
            {
                listItem.Add(new SelectListItem { Text = "", Value = "" });
                ViewData["DefaultAsset"] = new SelectList(listItem, "Value", "Text");
            }

            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "全責", Value = "全責" });
            listItem2.Add(new SelectListItem { Text = "半責", Value = "半責" });
            ViewData["ContractType"] = new SelectList(listItem2, "Value", "Text", oldContract.ContractType);

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            AppUser ur;
            listItem3.Add(new SelectListItem { Text = "請選擇", Value = "" });
            if (oldContract.ContractMgr != null)
            {
                ur = db.AppUsers.Where(u => u.Id == oldContract.ContractMgr).FirstOrDefault();
                if (ur != null)
                {
                    listItem3.Add(new SelectListItem { Text = ur.FullName, Value = ur.Id.ToString() });
                }
            }
            ViewData["ContractMgr"] = new SelectList(listItem3, "Value", "Text", oldContract.ContractMgr);
            ViewData["SecondMgr"] = new SelectList(listItem3, "Value", "Text", oldContract.SecondMgr);

            List<SelectListItem> listItem4 = new List<SelectListItem>();
            listItem4.Add(new SelectListItem { Text = "工程類", Value = "工程類" });
            listItem4.Add(new SelectListItem { Text = "財務類", Value = "財務類" });
            listItem4.Add(new SelectListItem { Text = "勞務類", Value = "勞務類" });
            ViewData["ContractClass"] = new SelectList(listItem4, "Value", "Text", oldContract.ContractClass);

            AssetMaintainContract newContract = oldContract;
            newContract.PurchaseNo = oldContract.PurchaseNo + 1;

            return View(newContract);
        }

        // POST: MedEngMgt/AssetMaintainContracts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseNo,ContractNo,ContractName,ContractClass,VendorId,VendorName,VendorUniteNo,AssetNo,AssetName,Brand,Type,SeqNo,Qty,Unite,Sdate,Edate,Cycle,UseLife,TotalCost,YearCost,StagePayment,StageCost,EndNotice,EndNoticeMonth,Note,IsTraining,IsYearKeepReport,ContractMgr,SecondMgr,ContractType,KeepCostRate,UniteCost")] AssetMaintainContract assetMaintainContract)
        {
            if (ModelState.IsValid)
            {
                var asset = db.Assets.Find(assetMaintainContract.AssetNo);
                assetMaintainContract.AssetName = asset.Cname;
                assetMaintainContract.Status = "Y";
                assetMaintainContract.Rtp = WebSecurity.CurrentUserId;
                assetMaintainContract.Rtt = DateTime.Now;

                db.AssetMaintainContracts.Add(assetMaintainContract);
                db.SaveChanges();
                //回寫總金額至各設備保養金額
                var assets = db.AssetsInMContracts.Where(a => a.PurchaseNo == assetMaintainContract.PurchaseNo).ToList();
                if (assets.Count() > 0)
                {
                    foreach(var item in assets)
                    {
                        var assetKeep = db.AssetKeeps.Where(ak => ak.AssetNo == item.AssetNo).FirstOrDefault();
                        if (assetKeep != null)
                        {
                            assetKeep.Cost = Convert.ToInt32(assetMaintainContract.TotalCost);
                            assetKeep.ContractNo = assetMaintainContract.PurchaseNo;
                            db.Entry(assetKeep).State = EntityState.Modified;
                        }
                        else
                        {
                            assetKeep = new AssetKeep();
                            assetKeep.AssetNo = item.AssetNo;
                            assetKeep.Cost = Convert.ToInt32(assetMaintainContract.TotalCost);
                            assetKeep.ContractNo = assetMaintainContract.PurchaseNo;
                            db.AssetKeeps.Add(assetKeep);
                        }
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(assetMaintainContract);
        }

        // GET: MedEngMgt/AssetMaintainContracts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetMaintainContract assetMaintainContract = db.AssetMaintainContracts.Find(id);
            List<SelectListItem> listItem = new List<SelectListItem>();
            if (assetMaintainContract == null)
            {
                return HttpNotFound();
            }
            if (assetMaintainContract.AssetNo != null)
            {
                listItem.Add(new SelectListItem { Text = assetMaintainContract.AssetName + "(" + assetMaintainContract.AssetNo + ")",
                                                  Value = assetMaintainContract.AssetNo });
                ViewData["AssetNo"] = new SelectList(listItem, "Value", "Text", assetMaintainContract.AssetNo);
            }
            else
            {
                listItem.Add(new SelectListItem { Text = "", Value = "" });
                ViewData["AssetNo"] = new SelectList(listItem, "Value", "Text");
            }

            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "全責", Value = "全責" });
            listItem2.Add(new SelectListItem { Text = "半責", Value = "半責" });
            ViewData["ContractType"] = new SelectList(listItem2, "Value", "Text", assetMaintainContract.ContractType);

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            AppUser ur;
            listItem3.Add(new SelectListItem { Text = "請選擇", Value = "" });
            if (assetMaintainContract.ContractMgr != null)
            {
                ur = db.AppUsers.Where(u => u.Id == assetMaintainContract.ContractMgr).FirstOrDefault();
                if (ur != null)
                {
                    listItem3.Add(new SelectListItem { Text = ur.FullName, Value = ur.Id.ToString() });
                }
            }
            if (assetMaintainContract.SecondMgr != null)
            {
                ur = db.AppUsers.Where(u => u.Id == assetMaintainContract.SecondMgr).FirstOrDefault();
                if (ur != null)
                {
                    listItem3.Add(new SelectListItem { Text = ur.FullName, Value = ur.Id.ToString() });
                }
            }
            ViewData["ContractMgr"] = new SelectList(listItem3, "Value", "Text", assetMaintainContract.ContractMgr);
            ViewData["SecondMgr"] = new SelectList(listItem3, "Value", "Text", assetMaintainContract.SecondMgr);

            List<SelectListItem> listItem4 = new List<SelectListItem>();
            listItem4.Add(new SelectListItem { Text = "工程類", Value = "工程類" });
            listItem4.Add(new SelectListItem { Text = "財務類", Value = "財務類" });
            listItem4.Add(new SelectListItem { Text = "勞務類", Value = "勞務類" });
            ViewData["ContractClass"] = new SelectList(listItem4, "Value", "Text", assetMaintainContract.ContractClass);

            return View(assetMaintainContract);
        }

        // POST: MedEngMgt/AssetMaintainContracts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseNo,ContractNo,ContractName,ContractClass,VendorId,VendorName,VendorUniteNo,AssetNo,AssetName,Brand,Type,SeqNo,Qty,Unite,Sdate,Edate,Cycle,UseLife,TotalCost,YearCost,StagePayment,StageCost,EndNotice,EndNoticeMonth,Note,IsTraining,IsYearKeepReport,ContractMgr,SecondMgr,ContractType,KeepCostRate,UniteCost")] AssetMaintainContract assetMaintainContract)
        {
            if (ModelState.IsValid)
            {
                var asset = db.Assets.Find(assetMaintainContract.AssetNo);
                assetMaintainContract.AssetName = asset.Cname;
                assetMaintainContract.Rtp = WebSecurity.CurrentUserId;
                assetMaintainContract.Rtt = DateTime.Now;

                db.Entry(assetMaintainContract).State = EntityState.Modified;
                db.SaveChanges();
                //回寫總金額至各設備保養金額
                var assets = db.AssetsInMContracts.Where(a => a.PurchaseNo == assetMaintainContract.PurchaseNo).ToList();
                if (assets.Count() > 0)
                {
                    foreach (var item in assets)
                    {
                        var assetKeep = db.AssetKeeps.Where(ak => ak.AssetNo == item.AssetNo).FirstOrDefault();
                        if (assetKeep != null)
                        {
                            assetKeep.Cost = Convert.ToInt32(assetMaintainContract.TotalCost);
                            assetKeep.ContractNo = assetMaintainContract.PurchaseNo;
                            db.Entry(assetKeep).State = EntityState.Modified;
                        }
                        else
                        {
                            assetKeep = new AssetKeep();
                            assetKeep.AssetNo = item.AssetNo;
                            assetKeep.Cost = Convert.ToInt32(assetMaintainContract.TotalCost);
                            assetKeep.ContractNo = assetMaintainContract.PurchaseNo;
                            db.AssetKeeps.Add(assetKeep);
                        }
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(assetMaintainContract);
        }

        // GET: MedEngMgt/AssetMaintainContracts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetMaintainContract assetMaintainContract = db.AssetMaintainContracts.Find(id);
            if (assetMaintainContract == null)
            {
                return HttpNotFound();
            }
            if (assetMaintainContract.ContractMgr != null)
            {
                assetMaintainContract.ContractMgrName = db.AppUsers.Find(assetMaintainContract.ContractMgr).FullName;
            }
            return View(assetMaintainContract);
        }

        // POST: MedEngMgt/AssetMaintainContracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AssetMaintainContract assetMaintainContract = db.AssetMaintainContracts.Find(id);
            db.AssetMaintainContracts.Remove(assetMaintainContract);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: MedEngMgt/AssetMaintainContracts/CheckPNo/5
        public ActionResult CheckPNo(string id)
        {
            string result;
            var checkPNo = db.AssetMaintainContracts.Find(id);
            if (checkPNo != null)
            {
                result = "<span style='color:red;'>已有相同合約編號</span>";
                return new JsonResult
                {
                    Data = new { success = false, error = "", data = result },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                result = "<span style='color:green;'>可用編號</span>";
                return new JsonResult
                {
                    Data = new { success = true, error = "", data = result },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        // POST: MedEngMgt/AssetMaintainContracts/AddContractAsset/5
        [HttpPost]
        [MyErrorHandler]
        public ActionResult AddContractAsset(string purchaseNo , string assetNo)
        {
            if (string.IsNullOrEmpty(purchaseNo))
            {
                throw new Exception("尚未輸入案號");
            }
                if (!string.IsNullOrEmpty(purchaseNo) && !string.IsNullOrEmpty(assetNo))
            {
                var dataExist = db.AssetsInMContracts.Where(a => a.PurchaseNo == purchaseNo && a.AssetNo == assetNo)
                                                     .FirstOrDefault();
                if (dataExist != null)
                {
                    throw new Exception("設備或案號重複");
                }
                var asset = db.Assets.Where(a => a.AssetNo == assetNo).FirstOrDefault();
                AssetsInMContracts ac = new AssetsInMContracts();
                ac.PurchaseNo = purchaseNo;
                ac.AssetNo = asset.AssetNo;
                ac.AssetName = asset.Cname;
                ac.Brand = asset.Brand;
                ac.Type = asset.Type;
                ac.SerialNo = "";
                ac.Qty = "1";
                ac.UniteCost = asset.Cost;
                ac.Rtp = WebSecurity.CurrentUserId;
                ac.Rtt = DateTime.Now;

                db.AssetsInMContracts.Add(ac);
                db.SaveChanges();
            }
            return new JsonResult
            {
                Data = new { success = true, error = "" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // GET: MedEngMgt/AssetMaintainContracts/GetContractAssetList/5
        public ActionResult GetContractAssetList(string purchaseNo)
        {
            var assets = db.AssetsInMContracts.Where(a => a.PurchaseNo == purchaseNo).ToList();
            foreach(var item in assets)
            {
                var accDpt = db.Assets.Where(a => a.AssetNo == item.AssetNo).FirstOrDefault().AccDpt;
                var dpt = db.Departments.Where(d => d.DptId == accDpt).FirstOrDefault();
                if (dpt != null)
                {
                    item.AccDptName = dpt.Name_C;
                }
            }
            return PartialView(assets);
        }

        // GET: MedEngMgt/AssetMaintainContracts/GetContractAssetList2/5
        public ActionResult GetContractAssetList2(string purchaseNo)
        {
            var assets = db.AssetsInMContracts.Where(a => a.PurchaseNo == purchaseNo).ToList();
            foreach (var item in assets)
            {
                var accDpt = db.Assets.Where(a => a.AssetNo == item.AssetNo).FirstOrDefault().AccDpt;
                var dpt = db.Departments.Where(d => d.DptId == accDpt).FirstOrDefault();
                if (dpt != null)
                {
                    item.AccDptName = dpt.Name_C;
                }
            }
            return PartialView(assets);
        }

        // POST: MedEngMgt/AssetMaintainContracts/DeleteAsset/5
        [HttpPost]
        [MyErrorHandler]
        public ActionResult DeleteAsset(string purchaseNo, string assetNo)
        {
            try
            {
                AssetsInMContracts asset = db.AssetsInMContracts.Find(purchaseNo, assetNo);
                db.AssetsInMContracts.Remove(asset);
                db.SaveChanges();
                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public ActionResult ExportToExcel(string contracts)
        {
            string[] PNOList = contracts.Split(new char[] { ';' });
            string fileName = "";

            MemoryStream stream = new MemoryStream();
            ExcelPackage package = new ExcelPackage(stream);

            package.Workbook.Worksheets.Add("維護合約列表");
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];

            #region write header
            sheet.Cells[1, 1].Value = "採購編號";
            sheet.Cells[1, 2].Value = "契約案號";
            sheet.Cells[1, 3].Value = "合約名稱";
            sheet.Cells[1, 4].Value = "合約廠商";
            sheet.Cells[1, 5].Value = "廠商統一編號";
            sheet.Cells[1, 6].Value = "財產編號";
            sheet.Cells[1, 7].Value = "財產名稱";
            sheet.Cells[1, 8].Value = "廠牌";
            sheet.Cells[1, 9].Value = "型號";
            sheet.Cells[1, 10].Value = "序號";
            sheet.Cells[1, 11].Value = "台數";
            sheet.Cells[1, 12].Value = "單位";
            sheet.Cells[1, 13].Value = "合約起始日";
            sheet.Cells[1, 14].Value = "合約終止日";
            sheet.Cells[1, 15].Value = "保養週期(月)";
            sheet.Cells[1, 16].Value = "年限(年)";
            sheet.Cells[1, 17].Value = "總費用";
            sheet.Cells[1, 18].Value = "費用/年";
            sheet.Cells[1, 19].Value = "付款分期(次)";
            sheet.Cells[1, 20].Value = "每期金額";
            sheet.Cells[1, 21].Value = "合約結束通知";
            sheet.Cells[1, 22].Value = "是否教育訓練";
            sheet.Cells[1, 23].Value = "是否年度保養報告";
            sheet.Cells[1, 24].Value = "合約負責人";
            sheet.Cells[1, 25].Value = "合約執行方案";
            sheet.Cells[1, 26].Value = "保養匯率";
            sheet.Cells[1, 27].Value = "設備單價";
            sheet.Cells[1, 28].Value = "備註";
            sheet.Cells[1, 29].Value = "異動人員";
            sheet.Cells[1, 30].Value = "異動日期";

            using (ExcelRange range = sheet.Cells[1, 1, 1, 30])
            {
                //range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                //range.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
                range.Style.Border.Bottom.Color.SetColor(Color.Black);
                //range.AutoFitColumns(4);
            }
            #endregion

            #region write content
            List<AssetMaintainContract> ContractList = new List<AssetMaintainContract>();
            foreach (string purchaseNo in PNOList)
            {
                var targetContract = db.AssetMaintainContracts.Where(s => s.PurchaseNo == purchaseNo).FirstOrDefault();
                if (targetContract != null)
                {
                    ContractList.Add(targetContract);
                }
            }

            int pos = 2;
            foreach (AssetMaintainContract s in ContractList)
            {
                sheet.Cells[pos, 1].Value = s.PurchaseNo;
                sheet.Cells[pos, 2].Value = s.ContractNo;
                sheet.Cells[pos, 3].Value = s.ContractName;
                sheet.Cells[pos, 4].Value = s.VendorName;
                sheet.Cells[pos, 5].Value = s.VendorUniteNo;
                sheet.Cells[pos, 6].Value = s.AssetNo;
                sheet.Cells[pos, 7].Value = s.AssetName;
                sheet.Cells[pos, 8].Value = s.Brand;
                sheet.Cells[pos, 9].Value = s.Type;
                sheet.Cells[pos, 10].Value = s.SerialNo;
                sheet.Cells[pos, 11].Value = s.Qty;
                sheet.Cells[pos, 12].Value = s.Unite;
                sheet.Cells[pos, 13].Value = s.Sdate == null ? "" : s.Sdate.ToString("yyyy/MM/dd");
                sheet.Cells[pos, 14].Value = s.Edate == null ? "" : s.Edate.ToString("yyyy/MM/dd");
                sheet.Cells[pos, 15].Value = s.Cycle;
                sheet.Cells[pos, 16].Value = s.UseLife;
                sheet.Cells[pos, 17].Value = s.TotalCost;
                sheet.Cells[pos, 18].Value = s.YearCost;
                sheet.Cells[pos, 19].Value = s.StagePayment;
                sheet.Cells[pos, 20].Value = s.StageCost;
                sheet.Cells[pos, 21].Value = s.EndNotice == "Y" ? "是" : "否";
                sheet.Cells[pos, 22].Value = s.IsTraining == "Y" ? "是" : "否";
                sheet.Cells[pos, 23].Value = s.IsYearKeepReport == "Y" ? "是" : "否";
                sheet.Cells[pos, 24].Value = s.ContractMgr == null ? "" : db.AppUsers.Find(s.ContractMgr).FullName;
                sheet.Cells[pos, 25].Value = s.ContractType;
                sheet.Cells[pos, 26].Value = s.KeepCostRate;
                sheet.Cells[pos, 27].Value = s.UniteCost;
                sheet.Cells[pos, 28].Value = s.Note;
                sheet.Cells[pos, 29].Value = db.AppUsers.Where(a => a.Id == s.Rtp).FirstOrDefault() == null ? "" : db.AppUsers.Where(a => a.Id == s.Rtp).FirstOrDefault().FullName;
                sheet.Cells[pos, 30].Value = s.Rtt == null ? "" : s.Rtt.ToString("yyyy/MM/dd");

                using (ExcelRange range = sheet.Cells[pos, 1, pos, 30])
                {
                    //range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    //range.Style.Border.Bottom.Color.SetColor(Color.Black);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                }
                pos++;
            }
            #endregion

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "維護合約列表_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            }
            //因為是用Query的方式,這個地方要用串流的方式來存檔
            using (MemoryStream memoryStream = new MemoryStream())
            {
                package.SaveAs(memoryStream);
                //請注意 一定要加入這行,不然Excel會是空檔
                memoryStream.Seek(0, SeekOrigin.Begin);
                //注意Excel的ContentType,是要用這個"application/vnd.ms-excel"
                return this.File(memoryStream.ToArray(), "application/vnd.ms-excel", fileName);
            }
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
