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

            return View("List", contracts);
        }

        // GET: MedEngMgt/AssetPurchaseContracts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetPurchaseContract assetPurchaseContract = db.AssetPurchaseContracts.Find(id);
            if (assetPurchaseContract == null)
            {
                return HttpNotFound();
            }
            return View(assetPurchaseContract);
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
            ViewData["UseDpt"] = Dpts;
            ViewData["PurchaseDpt"] = Dpts;

            List<SelectListItem> ListItem1 = new List<SelectListItem>();
            ListItem1.Add(new SelectListItem { Text = "請選擇", Value = "" });
            ViewData["PurchaseUid"] = ListItem1;
            ViewData["SponsorUid"] = ListItem1;
            ViewData["CoOrganizerUid"] = ListItem1;

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
            AssetPurchaseContract assetPurchaseContract = db.AssetPurchaseContracts.Find(id);
            if (assetPurchaseContract == null)
            {
                return HttpNotFound();
            }
            return View(assetPurchaseContract);
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
            AssetPurchaseContract assetPurchaseContract = db.AssetPurchaseContracts.Find(id);
            if (assetPurchaseContract == null)
            {
                return HttpNotFound();
            }
            return View(assetPurchaseContract);
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
