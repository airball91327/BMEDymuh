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
using System.Web.UI.WebControls;

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

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            var keepFormats = db.KeepFormats.ToList();
            foreach(var item in keepFormats)
            {
                listItem3.Add(new SelectListItem { Text = item.FormatId, Value = item.FormatId });
            }
            ViewData["FormatId"] = new SelectList(listItem3, "Value", "Text", "");
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
                var kp = db.AssetKeeps.Find(assetKeep.AssetNo);
                if (kp == null)
                {
                    assetKeep.KeepEngName = db.AppUsers.Find(assetKeep.KeepEngId).FullName;
                    db.AssetKeeps.Add(assetKeep);
                }
                else
                {
                    assetKeep.KeepEngName = db.AppUsers.Find(assetKeep.KeepEngId).FullName;
                    db.Entry(kp).State = EntityState.Detached;
                    db.Entry(assetKeep).State = EntityState.Modified;
                }
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

        public ActionResult EditData(string ano = null, string id = null)
        {
            Asset at = db.Assets.Find(ano);
            Delivery d = db.Deliveries.Find(id);
            int vid = d.VendorId != null ? Convert.ToInt32(d.VendorId) : 0;
            Vendor v = db.Vendors.Where(vv => vv.VendorId == vid).FirstOrDefault();
            List<string> s;
            ListItem li;
            s = Roles.GetUsersInRole("Engineer").ToList();
            List<ListItem> list = new List<ListItem>();
            AppUser u;
            foreach (string l in s)
            {
                u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                if (u != null)
                {
                    if (u.VendorId != null)
                    {
                        if (u.VendorId == v.VendorId)
                        {
                            li = new ListItem();
                            li.Text = u.FullName;
                            li.Value = u.Id.ToString();
                            list.Add(li);
                        }
                    }
                }
            }
            Department dpt = db.Departments.Find(at.DelivDpt);
            Department g;
            if (dpt != null)
            {
                s = Roles.GetUsersInRole("MedEngineer").ToList();
                foreach (string l in s)
                {
                    u = db.AppUsers.Find(WebSecurity.GetUserId(l));
                    if (u != null)
                    {
                        if (u.DptId != null)
                        {
                            g = db.Departments.Find(u.DptId);
                            if (g.DptId == dpt.DptId)
                            {
                                li = new ListItem();
                                li.Text = u.FullName;
                                li.Value = u.Id.ToString();
                                list.Add(li);
                            }
                        }
                    }
                }
            }
            ViewData["Items"] = new SelectList(list, "Value", "Text", "");
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "自行", Value = "自行" });
            listItem.Add(new SelectListItem { Text = "委外", Value = "委外" });
            listItem.Add(new SelectListItem { Text = "保固", Value = "保固" });
            listItem.Add(new SelectListItem { Text = "租賃", Value = "租賃" });
            ViewData["INOUTITEMS"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<ListItem> list2 = new List<ListItem>();
            List<KeepFormat> kf = db.KeepFormats.ToList();
            foreach (KeepFormat k in kf)
            {
                li = new ListItem { Text = k.FormatId, Value = k.FormatId };
                list2.Add(li);
            }
            ViewData["FORMATITEMS"] = new SelectList(list2, "Value", "Text", "");
            //
            AssetKeep assetkeep = db.AssetKeeps.Find(ano);
            if (assetkeep == null)
            {
                return HttpNotFound();
            }
            assetkeep.Cname = db.Assets.Find(assetkeep.AssetNo).Cname;
            if (assetkeep.KeepYm == null)
            {
                assetkeep.KeepYm = (d.DelivDateR.Year - 1911) * 100 + d.DelivDateR.Month;
            }
            return PartialView(assetkeep);
        }

        [HttpPost]
        public JsonResult EditData(AssetKeep assetkeep)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assetkeep).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    return Json(new { success = true, msg = "儲存成功!" });
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                    return Json(new { success = false, msg = s });
                }
            }
            return Json(new { success = false, msg = "儲存失敗!" });
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
