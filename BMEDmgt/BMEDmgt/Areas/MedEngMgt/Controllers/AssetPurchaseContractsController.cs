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
            string cno = fm["qtyContractNo"];
            string vname = fm["qtyVendorName"];
            string vuniteno = fm["qtyVendorUniteNo"];

            var contracts = db.AssetPurchaseContracts.ToList();
            if (!string.IsNullOrEmpty(cno))  //契約號
            {
                contracts = contracts.Where(c => c.ContractNo == cno).ToList();
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
            assetPContract.UseDptName = dptU == null ? "" : dptU.Name_C;
            assetPContract.PurchaseDptName = dptP == null ? "" : dptP.Name_C;
            assetPContract.SponsorDptName = dptS == null ? "" : dptS.Name_C;
            assetPContract.CoOrganizerDptName = dptC == null ? "" : dptC.Name_C;
            assetPContract.SponsorName = userS == null ? "" : userS.FullName;
            assetPContract.CoOrganizerName = userC == null ? "" : userC.FullName;

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

            return View();
        }

        // POST: MedEngMgt/AssetPurchaseContracts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ContractNo,PurchaseNo,PurchaseName,LeaveLoc,VendorId,VendorName,VendorPhone,Budget,BasicPrice,ContractClass,ContractTotalPrice,AwardDate,AcceptDate,Warranty,AssetClass,WarrantySdate,WarrantyEdate,WarrantyMargin,PerformanceMargin,UseDpt,PurchaseDpt,PurchaseUid,PurchaseUName,PermitNo,PermitValid,Sponsor,SponsorUid,CoOrganizer,CoOrganizerUid,PAssetClass,Note,Status,Rtp,Rtt")] AssetPurchaseContract assetPurchaseContract)
        {
            if (ModelState.IsValid)
            {
                assetPurchaseContract.Status = "Y";
                assetPurchaseContract.Rtp = WebSecurity.CurrentUserId;
                assetPurchaseContract.Rtt = DateTime.Now;

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
            ViewData["PurchaseUid"] = new SelectList(ListItem1, "Value", "Text", assetPContract.PurchaseUid);
            ViewData["SponsorUid"] = new SelectList(ListItem1, "Value", "Text", assetPContract.SponsorUid);
            ViewData["CoOrganizerUid"] = new SelectList(ListItem1, "Value", "Text", assetPContract.CoOrganizerUid);

            return View(assetPContract);
        }

        // POST: MedEngMgt/AssetPurchaseContracts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ContractNo,PurchaseNo,PurchaseName,LeaveLoc,VendorId,VendorName,VendorPhone,Budget,BasicPrice,ContractClass,ContractTotalPrice,AwardDate,AcceptDate,Warranty,AssetClass,WarrantySdate,WarrantyEdate,WarrantyMargin,PerformanceMargin,UseDpt,PurchaseDpt,PurchaseUid,PurchaseUName,PermitNo,PermitValid,Sponsor,SponsorUid,CoOrganizer,CoOrganizerUid,PAssetClass,Note,Status,Rtp,Rtt")] AssetPurchaseContract assetPurchaseContract)
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
