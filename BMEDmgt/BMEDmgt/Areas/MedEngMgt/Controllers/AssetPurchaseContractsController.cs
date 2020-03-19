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
    public class AssetPurchaseContractsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/AssetPurchaseContracts
        public ActionResult Index()
        {
            return View();
        }

        // POST: MedEngMgt/AssetPurchaseContracts
        [HttpPost]
        public ActionResult Index(FormCollection fm)
        {
            string pno = fm["qtyPurchaseNo"];
            string vname = fm["qtyVendorName"];
            string vuniteno = fm["qtyVendorUniteNo"];

            var contracts = db.AssetPurchaseContracts.ToList();
            if (!string.IsNullOrEmpty(pno))  //契約號
            {
                contracts = contracts.Where(c => c.PurchaseNo == pno).ToList();
            }
            if (!string.IsNullOrEmpty(vname))   //廠商名稱關鍵字
            {
                contracts = contracts.Where(c => c.VendorName.Contains(vname)).ToList();
            }
            if (!string.IsNullOrEmpty(vuniteno))    //廠商統編
            {
                contracts = contracts.Where(c => c.VendorUniteNo == vuniteno).ToList();
            }

            foreach(var item in contracts)
            {
                var dptU = db.Departments.Where(d => d.DptId == item.UseDpt).FirstOrDefault();
                var dptP = db.Departments.Where(d => d.DptId == item.PurchaseDpt).FirstOrDefault();
                var dptS = db.Departments.Where(d => d.DptId == item.Sponsor).FirstOrDefault();
                var dptC = db.Departments.Where(d => d.DptId == item.CoOrganizer).FirstOrDefault();
                var userS = db.AppUsers.Where(u => u.Id == item.SponsorUid).FirstOrDefault();
                var userC = db.AppUsers.Where(u => u.Id == item.CoOrganizerUid).FirstOrDefault();
                item.UseDptName = dptU == null ? "" : dptU.Name_C;
                item.PurchaseDptName = dptP == null ? "" : dptP.Name_C;
                item.SponsorDptName = dptS == null ? "" : dptS.Name_C;
                item.CoOrganizerDptName = dptC == null ? "" : dptC.Name_C;
                item.SponsorName = userS == null ? "" : userS.FullName;
                item.CoOrganizerName = userC == null ? "" : userC.FullName;
            }

            return View("List", contracts);
        }

        // GET: MedEngMgt/AssetPurchaseContracts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetPurchaseContract assetPContract = db.AssetPurchaseContracts.Find(id);
            if (assetPContract == null)
            {
                return HttpNotFound();
            }
            var dptU = db.Departments.Where(d => d.DptId == assetPContract.UseDpt).FirstOrDefault();
            var dptP = db.Departments.Where(d => d.DptId == assetPContract.PurchaseDpt).FirstOrDefault();
            var dptS = db.Departments.Where(d => d.DptId == assetPContract.Sponsor).FirstOrDefault();
            var dptC = db.Departments.Where(d => d.DptId == assetPContract.CoOrganizer).FirstOrDefault();
            var userS = db.AppUsers.Where(u => u.Id == assetPContract.SponsorUid).FirstOrDefault();
            var userC = db.AppUsers.Where(u => u.Id == assetPContract.CoOrganizerUid).FirstOrDefault();
            var user3 = db.AppUsers.Where(u => u.Id == assetPContract.ContractMgr).FirstOrDefault();
            var user4 = db.AppUsers.Where(u => u.Id == assetPContract.SecondMgr).FirstOrDefault();
            assetPContract.UseDptName = dptU == null ? "" : dptU.Name_C;
            assetPContract.PurchaseDptName = dptP == null ? "" : dptP.Name_C;
            assetPContract.SponsorDptName = dptS == null ? "" : dptS.Name_C;
            assetPContract.CoOrganizerDptName = dptC == null ? "" : dptC.Name_C;
            assetPContract.SponsorName = userS == null ? "" : userS.FullName;
            assetPContract.CoOrganizerName = userC == null ? "" : userC.FullName;
            assetPContract.ContractMgrName = user3 == null ? "" : user3.FullName;
            assetPContract.SecondMgrName = user4 == null ? "" : user4.FullName;

            return View(assetPContract);
        }

        // GET: MedEngMgt/AssetPurchaseContracts/Create
        public ActionResult Create()
        {
            List<SelectListItem> Dpts = new List<SelectListItem>();
            db.Departments.ToList()
                .ForEach(dp =>
                {
                    Dpts.Add(new SelectListItem
                    {
                        Value = dp.DptId,
                        Text = dp.Name_C,
                        Selected = false
                    });
                });
            ViewData["UseDpt"] = new SelectList(Dpts, "Value", "Text");
            ViewData["PurchaseDpt"] = new SelectList(Dpts, "Value", "Text");
            ViewData["Sponsor"] = new SelectList(Dpts, "Value", "Text");
            ViewData["CoOrganizer"] = new SelectList(Dpts, "Value", "Text");

            List<SelectListItem> ListItem1 = new List<SelectListItem>();
            ListItem1.Add(new SelectListItem { Text = "請選擇", Value = "" });
            ViewData["PurchaseUid"] = new SelectList(ListItem1, "Value", "Text", "");
            ViewData["SponsorUid"] = new SelectList(ListItem1, "Value", "Text", "");
            ViewData["CoOrganizerUid"] = new SelectList(ListItem1, "Value", "Text", "");
            ViewData["ContractMgr"] = new SelectList(ListItem1, "Value", "Text");
            ViewData["SecondMgr"] = new SelectList(ListItem1, "Value", "Text");

            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "工程類", Value = "工程類" });
            listItem2.Add(new SelectListItem { Text = "財務類", Value = "財務類" });
            listItem2.Add(new SelectListItem { Text = "勞務類", Value = "勞務類" });
            ViewData["ContractClass"] = new SelectList(listItem2, "Value", "Text", "");

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Text = "蘭陽院區", Value = "蘭陽院區" });
            listItem3.Add(new SelectListItem { Text = "新民院區", Value = "新民院區" });
            ViewData["LeaveLoc"] = new SelectList(listItem3, "Value", "Text", "");

            return View();
        }

        // POST: MedEngMgt/AssetPurchaseContracts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseNo,ContractNo,PurchaseName,LeaveLoc,VendorId,VendorName,VendorUniteNo,VendorPhone,Budget,BasicPrice,ContractClass,ContractTotalPrice,AwardDate,AcceptDate,Warranty,AssetClass,WarrantySdate,WarrantyEdate,WarrantyMargin,PerformanceMargin,UseDpt,PurchaseDpt,PurchaseUid,HasPermitNo,PermitNo,PermitValid,Sponsor,SponsorUid,CoOrganizer,CoOrganizerUid,PAssetClass,EndNotice,EndNoticeMonth,ContractMgr,SecondMgr,Note,Status,Rtp,Rtt")] AssetPurchaseContract assetPurchaseContract)
        {
            if (ModelState.IsValid)
            {
                assetPurchaseContract.Status = "Y";
                assetPurchaseContract.Rtp = WebSecurity.CurrentUserId;
                assetPurchaseContract.Rtt = DateTime.Now;

                if (string.IsNullOrEmpty(assetPurchaseContract.ContractNo))
                {
                    assetPurchaseContract.ContractNo = "無";
                }

                db.AssetPurchaseContracts.Add(assetPurchaseContract);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(assetPurchaseContract);
        }

        // GET: MedEngMgt/AssetPurchaseContracts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetPurchaseContract assetPContract = db.AssetPurchaseContracts.Find(id);
            if (assetPContract == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> Dpts = new List<SelectListItem>();
            db.Departments.ToList()
                .ForEach(dp =>
                {
                    Dpts.Add(new SelectListItem
                    {
                        Value = dp.DptId,
                        Text = dp.Name_C,
                        Selected = false
                    });
                });
            ViewData["UseDpt"] = new SelectList(Dpts, "Value", "Text", assetPContract.UseDpt);
            ViewData["PurchaseDpt"] = new SelectList(Dpts, "Value", "Text", assetPContract.PurchaseDpt);
            ViewData["Sponsor"] = new SelectList(Dpts, "Value", "Text", assetPContract.Sponsor);
            ViewData["CoOrganizer"] = new SelectList(Dpts, "Value", "Text", assetPContract.CoOrganizer);

            List<SelectListItem> ListItem1 = new List<SelectListItem>();
            AppUser ur;
            ListItem1.Add(new SelectListItem { Text = "請選擇", Value = "" });
            if (assetPContract.PurchaseUid != null)
            {
                ur = db.AppUsers.Where(u => u.Id == assetPContract.PurchaseUid).FirstOrDefault();
                if (ur != null)
                {
                    ListItem1.Add(new SelectListItem { Text = ur.FullName, Value = ur.Id.ToString() });
                }
            }
            if (assetPContract.SponsorUid != null)
            {
                ur = db.AppUsers.Where(u => u.Id == assetPContract.SponsorUid).FirstOrDefault();
                if (ur != null)
                {
                    ListItem1.Add(new SelectListItem { Text = ur.FullName, Value = ur.Id.ToString() });
                }
            }
            if (assetPContract.CoOrganizerUid != null)
            {
                ur = db.AppUsers.Where(u => u.Id == assetPContract.CoOrganizerUid).FirstOrDefault();
                if (ur != null)
                {
                    ListItem1.Add(new SelectListItem { Text = ur.FullName, Value = ur.Id.ToString() });
                }
            }
            if (assetPContract.ContractMgr != null)
            {
                ur = db.AppUsers.Where(u => u.Id == assetPContract.ContractMgr).FirstOrDefault();
                if (ur != null)
                {
                    ListItem1.Add(new SelectListItem { Text = ur.FullName, Value = ur.Id.ToString() });
                }
            }
            if (assetPContract.SecondMgr != null)
            {
                ur = db.AppUsers.Where(u => u.Id == assetPContract.SecondMgr).FirstOrDefault();
                if (ur != null)
                {
                    ListItem1.Add(new SelectListItem { Text = ur.FullName, Value = ur.Id.ToString() });
                }
            }
            ViewData["PurchaseUid"] = new SelectList(ListItem1, "Value", "Text", assetPContract.PurchaseUid);
            ViewData["SponsorUid"] = new SelectList(ListItem1, "Value", "Text", assetPContract.SponsorUid);
            ViewData["CoOrganizerUid"] = new SelectList(ListItem1, "Value", "Text", assetPContract.CoOrganizerUid);
            ViewData["ContractMgr"] = new SelectList(ListItem1, "Value", "Text", assetPContract.ContractMgr);
            ViewData["SecondMgr"] = new SelectList(ListItem1, "Value", "Text", assetPContract.SecondMgr);

            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "工程類", Value = "工程類" });
            listItem2.Add(new SelectListItem { Text = "財務類", Value = "財務類" });
            listItem2.Add(new SelectListItem { Text = "勞務類", Value = "勞務類" });
            ViewData["ContractClass"] = new SelectList(listItem2, "Value", "Text", assetPContract.ContractClass);

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Text = "蘭陽院區", Value = "蘭陽院區" });
            listItem3.Add(new SelectListItem { Text = "新民院區", Value = "新民院區" });
            ViewData["LeaveLoc"] = new SelectList(listItem3, "Value", "Text", assetPContract.LeaveLoc);

            return View(assetPContract);
        }

        // POST: MedEngMgt/AssetPurchaseContracts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContractNo,PurchaseNo,PurchaseName,LeaveLoc,VendorId,VendorName,VendorUniteNo,VendorPhone,Budget,BasicPrice,ContractClass,ContractTotalPrice,AwardDate,AcceptDate,Warranty,AssetClass,WarrantySdate,WarrantyEdate,WarrantyMargin,PerformanceMargin,UseDpt,PurchaseDpt,PurchaseUid,HasPermitNo,PermitNo,PermitValid,Sponsor,SponsorUid,CoOrganizer,CoOrganizerUid,PAssetClass,EndNotice,EndNoticeMonth,ContractMgr,SecondMgr,Note,Status,Rtp,Rtt")] AssetPurchaseContract assetPurchaseContract)
        {
            if (ModelState.IsValid)
            {
                assetPurchaseContract.Status = "Y";
                assetPurchaseContract.Rtp = WebSecurity.CurrentUserId;
                assetPurchaseContract.Rtt = DateTime.Now;

                db.Entry(assetPurchaseContract).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(assetPurchaseContract);
        }

        // GET: MedEngMgt/AssetPurchaseContracts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetPurchaseContract assetPContract = db.AssetPurchaseContracts.Find(id);
            if (assetPContract == null)
            {
                return HttpNotFound();
            }
            var dptU = db.Departments.Where(d => d.DptId == assetPContract.UseDpt).FirstOrDefault();
            var dptP = db.Departments.Where(d => d.DptId == assetPContract.PurchaseDpt).FirstOrDefault();
            var dptS = db.Departments.Where(d => d.DptId == assetPContract.Sponsor).FirstOrDefault();
            var dptC = db.Departments.Where(d => d.DptId == assetPContract.CoOrganizer).FirstOrDefault();
            var userS = db.AppUsers.Where(u => u.Id == assetPContract.SponsorUid).FirstOrDefault();
            var userC = db.AppUsers.Where(u => u.Id == assetPContract.CoOrganizerUid).FirstOrDefault();
            assetPContract.UseDptName = dptU == null ? "" : dptU.Name_C;
            assetPContract.PurchaseDptName = dptP == null ? "" : dptP.Name_C;
            assetPContract.SponsorDptName = dptS == null ? "" : dptS.Name_C;
            assetPContract.CoOrganizerDptName = dptC == null ? "" : dptC.Name_C;
            assetPContract.SponsorName = userS == null ? "" : userS.FullName;
            assetPContract.CoOrganizerName = userC == null ? "" : userC.FullName;

            return View(assetPContract);
        }

        // POST: MedEngMgt/AssetPurchaseContracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AssetPurchaseContract assetPurchaseContract = db.AssetPurchaseContracts.Find(id);
            db.AssetPurchaseContracts.Remove(assetPurchaseContract);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: MedEngMgt/AssetPurchaseContracts/CheckCNo/5
        public ActionResult CheckCNo(string id)
        {
            string result;
            var checkCNo = db.AssetPurchaseContracts.Find(id);
            if (checkCNo != null)
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

        // GET: MedEngMgt/AssetPurchaseContracts/CheckPNo/5
        public ActionResult CheckPNo(string id)
        {
            string result;
            var checkPNo = db.AssetPurchaseContracts.Find(id);
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

        // GET: MedEngMgt/AssetPurchaseContracts/CreatePermit/5
        public ActionResult CreatePermit()
        {
            return PartialView();
        }

        // POST: MedEngMgt/AssetPurchaseContracts/CreatePermit/5
        [MyErrorHandler]
        [HttpPost]
        public ActionResult CreatePermit(string purchaseNo, string permitNo, DateTime? permitValid)
        {
            if (string.IsNullOrEmpty(purchaseNo) || string.IsNullOrEmpty(permitNo) )
            {
                throw new Exception("尚未輸入採購編號及證號!");
            }
            if (permitValid == null)
            {
                throw new Exception("尚未輸入日期!");
            }
            var checkNo = db.AssetPContractPermits.Find(purchaseNo, permitNo);
            if (checkNo != null)
            {
                throw new Exception("已有相同證號!");
            }
            else
            {
                AssetPContractPermit permit = new AssetPContractPermit();
                permit.PurchaseNo = purchaseNo;
                permit.PermitNo = permitNo;
                permit.PermitValid = permitValid;
                permit.Status = "Y";
                permit.Rtp = WebSecurity.CurrentUserId;
                permit.Rtt = DateTime.Now;
                db.AssetPContractPermits.Add(permit);
                db.SaveChanges();

                return new JsonResult
                {
                    Data = new { success = true, error = "", data = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        // GET: MedEngMgt/AssetPurchaseContracts/GetPermitList/5
        public ActionResult GetPermitList(string purchaseNo)
        {
            if (!string.IsNullOrEmpty(purchaseNo))
            {
                var permitList = db.AssetPContractPermits.Where(a => a.PurchaseNo == purchaseNo).ToList();

                return View("PermitList", permitList);
            }
            return View("PermitList");
        }

        // GET: MedEngMgt/AssetPurchaseContracts/GetPermitList2/5
        public ActionResult GetPermitList2(string purchaseNo)
        {
            if (!string.IsNullOrEmpty(purchaseNo))
            {
                var permitList = db.AssetPContractPermits.Where(a => a.PurchaseNo == purchaseNo).ToList();

                return View("PermitList2", permitList);
            }
            return View("PermitList2");
        }

        // GET: MedEngMgt/AssetPurchaseContracts/GetContractById/5
        public ActionResult GetContractById(string purchaseNo)
        {
            if (!string.IsNullOrEmpty(purchaseNo))
            {
                var purContract = db.AssetPurchaseContracts.Where(p => p.PurchaseNo == purchaseNo).FirstOrDefault();
                if (purContract != null)
                {
                    return new JsonResult
                    {
                        Data = new { success = true, error = "", data = purContract },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            return new JsonResult
            {
                Data = new { success = false, error = "", data = "查無資料!" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ExportToExcel(string contracts)
        {
            string[] PNOList = contracts.Split(new char[] { ';' });
            string fileName = "";

            MemoryStream stream = new MemoryStream();
            ExcelPackage package = new ExcelPackage(stream);

            package.Workbook.Worksheets.Add("新購合約列表");
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];

            #region write header
            sheet.Cells[1, 1].Value = "採購編號";
            sheet.Cells[1, 2].Value = "採購名稱";
            sheet.Cells[1, 3].Value = "契約案號";
            sheet.Cells[1, 4].Value = "存置院區";
            sheet.Cells[1, 5].Value = "合約廠商";
            sheet.Cells[1, 6].Value = "廠商統一編號";
            sheet.Cells[1, 7].Value = "廠商電話";
            sheet.Cells[1, 8].Value = "預算金額";
            sheet.Cells[1, 9].Value = "底價金額";
            sheet.Cells[1, 10].Value = "合約類別";
            sheet.Cells[1, 11].Value = "合約總價";
            sheet.Cells[1, 12].Value = "決標日期";
            sheet.Cells[1, 13].Value = "驗收日期";
            sheet.Cells[1, 14].Value = "保固期間(年)";
            sheet.Cells[1, 15].Value = "設備類別";
            sheet.Cells[1, 16].Value = "保固起始日";
            sheet.Cells[1, 17].Value = "保固終止日";
            sheet.Cells[1, 18].Value = "保固保證金金額";
            sheet.Cells[1, 19].Value = "履約保證金金額";
            sheet.Cells[1, 20].Value = "使用單位";
            sheet.Cells[1, 21].Value = "請購單位";
            sheet.Cells[1, 22].Value = "採購人員";
            sheet.Cells[1, 23].Value = "是否有衛署登記證";
            sheet.Cells[1, 24].Value = "主辦單位";
            sheet.Cells[1, 25].Value = "主辦人員";
            sheet.Cells[1, 26].Value = "協辦單位";
            sheet.Cells[1, 27].Value = "協辦人員";
            sheet.Cells[1, 28].Value = "採購設備類別";
            sheet.Cells[1, 29].Value = "備註";
            sheet.Cells[1, 30].Value = "異動人員";
            sheet.Cells[1, 31].Value = "異動日期";

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
            List<AssetPurchaseContract> ContractList = new List<AssetPurchaseContract>();
            foreach (string purchaseNo in PNOList)
            {
                var targetContract = db.AssetPurchaseContracts.Where(s => s.PurchaseNo == purchaseNo).FirstOrDefault();
                if (targetContract != null)
                {
                    ContractList.Add(targetContract);
                }
            }

            int pos = 2;
            foreach (AssetPurchaseContract s in ContractList)
            {
                sheet.Cells[pos, 1].Value = s.PurchaseNo;
                sheet.Cells[pos, 2].Value = s.PurchaseName;
                sheet.Cells[pos, 3].Value = s.ContractNo;
                sheet.Cells[pos, 4].Value = s.LeaveLoc;
                sheet.Cells[pos, 5].Value = s.VendorName;
                sheet.Cells[pos, 6].Value = s.VendorUniteNo;
                sheet.Cells[pos, 7].Value = s.VendorPhone;
                sheet.Cells[pos, 8].Value = s.Budget;
                sheet.Cells[pos, 9].Value = s.BasicPrice;
                sheet.Cells[pos, 10].Value = s.ContractClass;
                sheet.Cells[pos, 11].Value = s.ContractTotalPrice;
                sheet.Cells[pos, 12].Value = s.AwardDate == null ? "" : s.AwardDate.Value.ToString("yyyy/MM/dd");
                sheet.Cells[pos, 13].Value = s.AcceptDate == null ? "" : s.AcceptDate.Value.ToString("yyyy/MM/dd");
                sheet.Cells[pos, 14].Value = s.Warranty;
                sheet.Cells[pos, 15].Value = s.AssetClass;
                sheet.Cells[pos, 16].Value = s.WarrantySdate == null ? "" : s.WarrantySdate.Value.ToString("yyyy/MM/dd");
                sheet.Cells[pos, 17].Value = s.WarrantyEdate == null ? "" : s.WarrantyEdate.Value.ToString("yyyy/MM/dd");
                sheet.Cells[pos, 18].Value = s.WarrantyMargin;
                sheet.Cells[pos, 19].Value = s.PerformanceMargin;
                sheet.Cells[pos, 20].Value = s.UseDpt == null ? "" : db.Departments.Find(s.UseDpt).Name_C;
                sheet.Cells[pos, 21].Value = s.PurchaseDpt == null ? "" : db.Departments.Find(s.PurchaseDpt).Name_C;
                sheet.Cells[pos, 22].Value = s.PurchaseUid == null ? "" : db.AppUsers.Find(s.PurchaseUid).FullName;
                sheet.Cells[pos, 23].Value = s.HasPermitNo == "Y" ? "是" : "否";
                sheet.Cells[pos, 24].Value = s.Sponsor == null ? "" : db.Departments.Find(s.Sponsor).Name_C;
                sheet.Cells[pos, 25].Value = s.SponsorUid == null ? "" : db.AppUsers.Find(s.SponsorUid).FullName;
                sheet.Cells[pos, 26].Value = s.CoOrganizer == null ? "" : db.Departments.Find(s.CoOrganizer).Name_C;
                sheet.Cells[pos, 27].Value = s.CoOrganizerUid == null ? "" : db.AppUsers.Find(s.CoOrganizerUid).FullName;
                sheet.Cells[pos, 28].Value = s.PAssetClass;
                sheet.Cells[pos, 29].Value = s.Note;
                sheet.Cells[pos, 30].Value = db.AppUsers.Where(a => a.Id == s.Rtp).FirstOrDefault() == null ? "" : db.AppUsers.Where(a => a.Id == s.Rtp).FirstOrDefault().FullName;
                sheet.Cells[pos, 31].Value = s.Rtt == null ? "" : s.Rtt.ToString("yyyy/MM/dd");

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
                fileName = "新購合約列表_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
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

        // POST: MedEngMgt/AssetPurchaseContracts/DeletePermit/5
        [MyErrorHandler]
        [HttpPost]
        public ActionResult DeletePermit(string purchaseNo, string permitNo)
        {
            try
            {
                AssetPContractPermit permit = db.AssetPContractPermits.Find(purchaseNo, permitNo);
                db.AssetPContractPermits.Remove(permit);
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
