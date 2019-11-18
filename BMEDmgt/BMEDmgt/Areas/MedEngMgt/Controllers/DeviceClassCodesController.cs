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
using Newtonsoft.Json;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class DeviceClassCodesController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/DeviceClassCodes
        public ActionResult Index()
        {
            return View(db.DeviceClassCodes.ToList());
        }

        // GET: MedEngMgt/DeviceClassCodes/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceClassCode deviceClassCode = db.DeviceClassCodes.Find(id);
            if (deviceClassCode == null)
            {
                return HttpNotFound();
            }
            return View(deviceClassCode);
        }

        // GET: MedEngMgt/DeviceClassCodes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/DeviceClassCodes/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "M_code,M_name")] DeviceClassCode deviceClassCode)
        {
            if (ModelState.IsValid)
            {
                db.DeviceClassCodes.Add(deviceClassCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(deviceClassCode);
        }

        // GET: MedEngMgt/DeviceClassCodes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceClassCode deviceClassCode = db.DeviceClassCodes.Find(id);
            if (deviceClassCode == null)
            {
                return HttpNotFound();
            }
            return View(deviceClassCode);
        }

        // POST: MedEngMgt/DeviceClassCodes/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "M_code,M_name")] DeviceClassCode deviceClassCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deviceClassCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(deviceClassCode);
        }

        // GET: MedEngMgt/DeviceClassCodes/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeviceClassCode deviceClassCode = db.DeviceClassCodes.Find(id);
            if (deviceClassCode == null)
            {
                return HttpNotFound();
            }
            return View(deviceClassCode);
        }

        // POST: MedEngMgt/DeviceClassCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DeviceClassCode deviceClassCode = db.DeviceClassCodes.Find(id);
            db.DeviceClassCodes.Remove(deviceClassCode);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult GetDataByKeyname(string keyname)
        {
            List<DeviceClassCode> ul;
            string s = "";
            if (string.IsNullOrEmpty(keyname))
                ul = db.DeviceClassCodes.ToList();
            else
                ul = db.DeviceClassCodes.Where(p => p.M_name.Contains(keyname)).ToList();
            s = JsonConvert.SerializeObject(ul);
            return Json(s, JsonRequestBehavior.AllowGet);
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
