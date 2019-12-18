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
            string cno = fm["qtyContractNo"];
            string aname = fm["qtyASSETNAME"];
            string ano = fm["qtyASSETNO"];
            string vname = fm["qtyVendorName"];
            string vuniteno = fm["qtyVendorUniteNo"];

            //List<AssetMaintainContract> ac = new List<AssetMaintainContract>();

            var contracts = db.AssetMaintainContracts.ToList();

            if(!string.IsNullOrEmpty(cno))  //契約號
            {
                contracts = contracts.Where(c => c.ContractNo == cno).ToList();
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

            return View();
        }

        // POST: MedEngMgt/AssetMaintainContracts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PurchaseNo,ContractNo,ContractName,VendorId,VendorName,VendorUniteNo,AssetNo,AssetName,Brand,Type,SeqNo,Qty,Unite,Sdate,Edate,Cycle,UseLife,TotalCost,YearCost,StagePayment,StageCost,EndNotice,Note,IsTraining,IsYearKeepReport,ContractMgr,ContractType,KeepCostRate,UniteCost")] AssetMaintainContract assetMaintainContract)
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
                ViewData["DefaultAsset"] = listItem;
            }
            else
            {
                listItem.Add(new SelectListItem { Text = "", Value = "" });
                ViewData["DefaultAsset"] = listItem;
            }

            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "全責", Value = "全責" });
            listItem2.Add(new SelectListItem { Text = "半責", Value = "半責" });

            ViewData["ContractType"] = listItem2;
            return View(assetMaintainContract);
        }

        // POST: MedEngMgt/AssetMaintainContracts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseNo,ContractNo,ContractName,VendorId,VendorName,VendorUniteNo,AssetNo,AssetName,Brand,Type,SeqNo,Qty,Unite,Sdate,Edate,Cycle,UseLife,TotalCost,YearCost,StagePayment,StageCost,EndNotice,Note,IsTraining,IsYearKeepReport,ContractMgr,ContractType,KeepCostRate,UniteCost")] AssetMaintainContract assetMaintainContract)
        {
            if (ModelState.IsValid)
            {
                var asset = db.Assets.Find(assetMaintainContract.AssetNo);
                assetMaintainContract.AssetName = asset.Cname;
                assetMaintainContract.Rtp = WebSecurity.CurrentUserId;
                assetMaintainContract.Rtt = DateTime.Now;

                db.Entry(assetMaintainContract).State = EntityState.Modified;
                db.SaveChanges();
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
