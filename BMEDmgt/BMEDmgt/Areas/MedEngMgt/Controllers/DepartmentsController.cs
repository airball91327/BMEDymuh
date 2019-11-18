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
    [Authorize]
    public class DepartmentsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/Departments
        public ActionResult Index()
        {
            return View(db.Departments.ToList());
        }

        // GET: MedEngMgt/Departments/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: MedEngMgt/Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/Departments/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                department.DateCreated = DateTime.Now;
                department.LastActivityDate = DateTime.Now;
                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: MedEngMgt/Departments/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: MedEngMgt/Departments/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                department.LastActivityDate = DateTime.Now;
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: MedEngMgt/Departments/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: MedEngMgt/Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetDptsByKeyname(string keyname)
        {
            List<DptList> dpts = new List<DptList>();

            string s = "";
            if (!string.IsNullOrEmpty(keyname))
            {
                db.Departments.Where(d => d.Name_C.Contains(keyname))
                  .ToList()
                  .ForEach(dp =>
                  {
                      DptList d = new DptList();
                      d.dptid = dp.DptId;
                      d.dptname = dp.Name_C;
                      dpts.Add(d);
                  });
                //
                Department dt = db.Departments.Find(keyname);
                if (dt != null)
                {
                    DptList d = new DptList();
                    d.dptid = dt.DptId;
                    d.dptname = dt.Name_C;
                    dpts.Add(d);
                }
            }
            else
            {
                db.Departments.ToList()
                  .ForEach(dp =>
                  {
                      DptList d = new DptList();
                      d.dptid = dp.DptId;
                      d.dptname = dp.Name_C;
                      dpts.Add(d);
                  });
            }
            s = JsonConvert.SerializeObject(dpts);

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

    public class DptList
    {
        public string dptid;
        public string dptname;
    }
}
