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
using BMEDmgt.Filters;
using System.Web.Security;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class AssetKeepsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/AssetKeeps
        public ActionResult Index()
        {
            return View(db.AssetKeeps.ToList());
        }

        // GET: MedEngMgt/AssetKeeps/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetKeep assetKeep = db.AssetKeeps.Find(id);
            if (assetKeep == null)
            {
                return HttpNotFound();
            }
            var keepEng = db.AppUsers.Find(assetKeep.KeepEngId);
            assetKeep.KeepEngName = keepEng == null ? "" : keepEng.FullName;
            return PartialView(assetKeep);
        }

        // GET: MedEngMgt/AssetKeeps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/AssetKeeps/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AssetKeep assetKeep)
        {
            if (ModelState.IsValid)
            {
                db.AssetKeeps.Add(assetKeep);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(assetKeep);
        }

        // GET: MedEngMgt/AssetKeeps/Edit/5
        public ActionResult Edit(string id)
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            AppUser u;
            //db.AppUsers.ToList().ForEach(d =>
            //{
            //    listItem.Add(new SelectListItem { Text = d.FullName, Value = d.Id.ToString() });
            //});
            Roles.GetUsersInRole("MedEngineer").ToList()
                        .ForEach(x =>
                        {
                            u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                listItem.Add(new SelectListItem { Text = u.FullName, Value = u.Id.ToString() });
                            }
                        });
            Roles.GetUsersInRole("Engineer").ToList()
                        .ForEach(x =>
                        {
                            u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                listItem.Add(new SelectListItem { Text = u.FullName, Value = u.Id.ToString() });
                            }
                        });
            listItem = listItem.GroupBy(g => g.Value).Select(g => g.First()).ToList();
            ViewData["KeepEngId"] = new SelectList(listItem, "Value", "Text", "");

            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "自行", Value = "自行" });
            listItem2.Add(new SelectListItem { Text = "委外", Value = "委外" });
            listItem2.Add(new SelectListItem { Text = "保固", Value = "保固" });
            listItem2.Add(new SelectListItem { Text = "租賃", Value = "租賃" });
            listItem2.Add(new SelectListItem { Text = "借用", Value = "借用" });
            ViewData["InOut"] = new SelectList(listItem2, "Value", "Text", "");
            if (id == null)
            {
                return PartialView();
            }
            AssetKeep assetKeep = db.AssetKeeps.Find(id);
            if (assetKeep == null)
            {
                assetKeep = new AssetKeep();
                assetKeep.AssetNo = id;
            }
            return PartialView(assetKeep);
        }

        // POST: MedEngMgt/AssetKeeps/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit(AssetKeep assetKeep)
        {
            if (ModelState.IsValid)
            {
                assetKeep.KeepEngName = db.AppUsers.Find(assetKeep.KeepEngId).FullName;
                db.Entry(assetKeep).State = EntityState.Modified;
                db.SaveChanges();
                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
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

        // GET: MedEngMgt/AssetKeeps/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetKeep assetKeep = db.AssetKeeps.Find(id);
            if (assetKeep == null)
            {
                return HttpNotFound();
            }
            return View(assetKeep);
        }

        // POST: MedEngMgt/AssetKeeps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AssetKeep assetKeep = db.AssetKeeps.Find(id);
            db.AssetKeeps.Remove(assetKeep);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult UpdEngineer(string id, string assets)
        {
            string[] s = assets.Split(new char[] { ';' });
            AssetKeep ak;
            foreach (string ss in s)
            {
                ak = db.AssetKeeps.Find(ss);
                if (ak != null)
                {
                    AppUser u = db.AppUsers.Find(Convert.ToInt32(id));
                    if (u != null)
                    {
                        ak.KeepEngId = u.Id;
                        ak.KeepEngName = u.FullName;
                        db.Entry(ak).State = EntityState.Modified;
                        db.SaveChanges();
                    }               
                }
            }
            return new JsonResult
            {
                Data = new { success = true, error = "" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
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
