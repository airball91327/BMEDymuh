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
using System.Web.Security;
using WebMatrix.WebData;
using BMEDmgt.Filters;
using Newtonsoft.Json;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class ContractsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/Contracts
        public ActionResult Index()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "正常", Value = "正常" });
            listItem.Add(new SelectListItem { Text = "終止", Value = "終止" });
            listItem.Add(new SelectListItem { Text = "全部", Value = "全部" });
            ViewData["STATUS"] = new SelectList(listItem, "Value", "Text", "全部");

            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection fm)
        {
            string cno = fm["qtyCONTRACTNO"];
            string cname = fm["qtyCONTRACTNAME"];
            string cstatus = fm["qtySTATUS"];

            var rv = db.Contracts.ToList();

            if (!string.IsNullOrEmpty(cno))
            {
                rv = rv.Where(v => v.ContractNo.Contains(cno)).ToList();
            }
            if (!string.IsNullOrEmpty(cname))
            {
                rv = rv.Where(v => v.ContractName.Contains(cname)).ToList();
            }
            if (!string.IsNullOrEmpty(cstatus))
            {
                switch (cstatus)
                {
                    case "正常":
                        rv = rv.Where(v => v.Status == "Y").ToList();
                        break;
                    case "終止":
                        rv = rv.Where(v => v.Status == "N").ToList();
                        break;
                }
            }
            return PartialView("List", rv);
        }
        public ActionResult List()
        {
            return PartialView(db.Contracts.ToList());
        }

        // GET: MedEngMgt/Contracts/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            return View(contract);
        }

        // GET: MedEngMgt/Contracts/Create
        public ActionResult Create()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            db.Vendors.ToList()
                        .ForEach(x =>
                        {
                            listItem.Add(new SelectListItem { Text = x.VendorName, Value = x.VendorId.ToString() });
                        });
            ViewData["VendorId"] = new SelectList(listItem, "Value", "Text", "");

            Roles.GetUsersInRole("MedEngineer").ToList()
                        .ForEach(x =>
                        {
                            AppUser u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                listItem2.Add(new SelectListItem { Text = u.FullName, Value = u.Id.ToString() });
                            }
                        });
            ViewData["EngId"] = new SelectList(listItem2, "Value", "Text", "");

            return View();
        }

        // POST: MedEngMgt/Contracts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Create([Bind(Include = "ContractNo,ContractName,Sdate,Edate,Cost,VendorId,VendorName,EngId,EngName,Status,Rtp,Rtt")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                if (db.Contracts.Where(c => c.ContractNo == contract.ContractNo).Count() > 0)
                {
                    throw new Exception("合約案號不可重複!!");
                }
                try
                {
                    contract.VendorName = db.Vendors.Find(contract.VendorId).VendorName;
                    contract.EngName = db.AppUsers.Find(contract.EngId).FullName;
                    contract.Rtp = WebSecurity.CurrentUserId;
                    contract.Rtt = DateTime.Now;
                    db.Contracts.Add(contract);
                    db.SaveChanges();

                    return new JsonResult
                    {
                        Data = new { success = true, error = "" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
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

        // GET: MedEngMgt/Contracts/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            List<SelectListItem> listItem = new List<SelectListItem>();
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            db.Vendors.ToList()
                        .ForEach(x =>
                        {
                            listItem.Add(new SelectListItem { Text = x.VendorName, Value = x.VendorId.ToString() });
                        });
            ViewData["Vendor"] = new SelectList(listItem, "Value", "Text", contract.VendorId.ToString());

            Roles.GetUsersInRole("MedEngineer").ToList()
                        .ForEach(x =>
                        {
                            AppUser u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                listItem2.Add(new SelectListItem { Text = u.FullName, Value = u.Id.ToString() });
                            }
                        });
            ViewData["Eng"] = new SelectList(listItem2, "Value", "Text", contract.EngId.ToString());


            return View(contract);
        }

        // POST: MedEngMgt/Contracts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit([Bind(Include = "ContractNo,ContractName,Sdate,Edate,Cost,VendorId,VendorName,EngId,EngName,Status,Rtp,Rtt")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    contract.VendorName = db.Vendors.Find(contract.VendorId).VendorName;
                    contract.EngName = db.AppUsers.Find(contract.EngId).FullName;
                    contract.Rtp = WebSecurity.CurrentUserId;
                    contract.Rtt = DateTime.Now;
                    db.Entry(contract).State = EntityState.Modified;
                    db.SaveChanges();

                    return new JsonResult
                    {
                        Data = new { success = true, error = "" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
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

        // GET: MedEngMgt/Contracts/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            return View(contract);
        }

        // POST: MedEngMgt/Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Contract contract = db.Contracts.Find(id);
            db.Contracts.Remove(contract);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public JsonResult GetByDpt(string dpt)
        {
            if (!string.IsNullOrEmpty(dpt))
            {
                var result = db.Assets
                    .Where(a => a.DelivDpt == dpt)
                    .Join(db.AssetKeeps, a => a.AssetNo, k => k.AssetNo,
                    (a, k) => k)
                    .Join(db.Contracts, k => k.ContractNo, c => c.ContractNo,
                    (k, c) => c).Distinct()
                    .Select(c => new
                    {
                        cno = c.ContractNo,
                        cname = c.ContractName
                    });

                string s = JsonConvert.SerializeObject(result);
                return Json(s, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
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
