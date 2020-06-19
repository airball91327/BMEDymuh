using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        // GET: MedEngMgt/Home
        public ActionResult Index()
        {
            UnsignCounts v = new UnsignCounts();
            int repairCount = db.RepairFlows.Where(f => f.Status == "?")
                .Where(f => f.UserId == WebSecurity.CurrentUserId).Count();
            v.RepairCount = repairCount;
            //公用帳號的案件數量
            var publicAcc = db.AppUsers.Where(u => u.UserName == "eao").ToList().FirstOrDefault();
            int repairCount2 = 0;
            if (publicAcc != null)
            {
                repairCount2 = db.RepairFlows.Where(f => f.Status == "?")
                                 .Where(f => f.UserId == publicAcc.Id).Count();
            }
            v.RepairCount2 = repairCount2;
            //
            int keepCount = db.Keeps
                .Join(db.KeepFlows.Where(f => f.Status == "?")
                .Where(f => f.UserId == WebSecurity.CurrentUserId), k => k.DocId, f => f.DocId,
                (k, f) => f).Count();
            v.KeepCount = keepCount;
            //
            int deliveryCount = db.Deliveries
                .Join(db.DelivFlows.Where(f => f.Status == "?")
                .Where(f => f.Userid == WebSecurity.CurrentUserId), k => k.Docid, f => f.Docid,
                (k, f) => f).Count();
            v.DeliveryCount = deliveryCount;
            v.BuyEvalateCount = 0;

            return View(v);
        }

        // GET: MedEngMgt/Home/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MedEngMgt/Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/Home/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MedEngMgt/Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MedEngMgt/Home/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: MedEngMgt/Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MedEngMgt/Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
