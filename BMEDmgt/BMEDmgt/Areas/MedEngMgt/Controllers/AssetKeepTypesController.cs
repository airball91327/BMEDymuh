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
    public class AssetKeepTypesController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/AssetKeepTypes
        public ActionResult Index()
        {
            return View(db.AssetKeepTypes.ToList());
        }

        // GET: MedEngMgt/AssetKeepTypes/List
        public ActionResult List()
        {
            var list = db.AssetKeepTypes.ToList();
            foreach(var item in list)
            {
                var keepType = db.AssetKeepTypeNos.Find(item.KeepTypeNo);
                if (keepType != null)
                {
                    item.KeepTypeDes = keepType.Descirption;
                }
            }
            return PartialView(list);
        }

        // GET: MedEngMgt/AssetKeepTypes/Create
        public ActionResult Create(string id)
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            AppUser u;
            Roles.GetUsersInRole("MedEngineer").ToList()
                        .ForEach(x =>
                        {
                            u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                listItem.Add(new SelectListItem { Text = "(" + u.UserName + ")" + u.FullName, Value = u.Id.ToString() });
                            }
                        });
            listItem = listItem.GroupBy(g => g.Value).Select(g => g.First()).ToList();
            ViewData["KeepTypeEngId"] = new SelectList(listItem, "Value", "Text", "");

            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "自行", Value = "自行" });
            listItem2.Add(new SelectListItem { Text = "委外", Value = "委外" });
            listItem2.Add(new SelectListItem { Text = "保固", Value = "保固" });
            listItem2.Add(new SelectListItem { Text = "租賃", Value = "租賃" });
            listItem2.Add(new SelectListItem { Text = "借用", Value = "借用" });
            ViewData["KeepTypeInOut"] = new SelectList(listItem2, "Value", "Text", "");

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            var keepFormats = db.KeepFormats.ToList();
            foreach (var item in keepFormats)
            {
                listItem3.Add(new SelectListItem { Text = item.FormatId, Value = item.FormatId });
            }
            ViewData["KeepTypeFormatId"] = new SelectList(listItem3, "Value", "Text", "");

            List<SelectListItem> listItem4 = new List<SelectListItem>();
            var assetKeepTypeNos = db.AssetKeepTypeNos.ToList();
            foreach(var item in assetKeepTypeNos)
            {
                listItem4.Add(new SelectListItem { Text = item.Descirption, Value = item.Id.ToString() });
            }
            ViewData["KeepTypeNo"] = new SelectList(listItem4, "Value", "Text", "");

            if (id == null)
            {
                return PartialView();
            }
            AssetKeepType assetKeepType = new AssetKeepType();
            assetKeepType.AssetNo = id;
            return PartialView(assetKeepType);
        }

        // POST: MedEngMgt/AssetKeepTypes/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [MyErrorHandler]
        public ActionResult Create(AssetKeepType assetKeepType)
        {
            if (ModelState.IsValid)
            {
                var dataExist = db.AssetKeepTypes.Where(kt => kt.AssetNo == assetKeepType.AssetNo && kt.KeepTypeNo == assetKeepType.KeepTypeNo)
                                                 .ToList().FirstOrDefault();
                if (dataExist != null)
                {
                    throw new Exception("已有相同保養週期!");
                }
                var eng = db.AppUsers.Find(assetKeepType.KeepEngId);
                if (eng != null)
                {
                    assetKeepType.KeepEngName = eng.FullName;
                }
                assetKeepType.Rtt = DateTime.Now;
                assetKeepType.Rtp = WebSecurity.CurrentUserId;
                db.AssetKeepTypes.Add(assetKeepType);
                db.SaveChanges();
                return RedirectToAction("List");
            }
            return RedirectToAction("List");
        }

        // GET: MedEngMgt/AssetKeepTypes/Edit/5
        public ActionResult Edit(string assetNo, string keepTypeNo)
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            AppUser u;
            Roles.GetUsersInRole("MedEngineer").ToList()
                        .ForEach(x =>
                        {
                            u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                listItem.Add(new SelectListItem { Text = "(" + u.UserName + ")" + u.FullName, Value = u.Id.ToString() });
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
            if (assetNo == null || keepTypeNo == null)
            {
                throw new Exception("無資料!");
            }
            AssetKeepType assetKeepType = db.AssetKeepTypes.Find(assetNo, Convert.ToInt32(keepTypeNo));
            if (assetKeepType == null)
            {
                return RedirectToAction("Create", new { id = assetNo });
            }
            var keepType = db.AssetKeepTypeNos.Find(assetKeepType.KeepTypeNo);
            if (keepType != null)
            {
                assetKeepType.KeepTypeDes = keepType.Descirption;
            }
            return PartialView(assetKeepType);
        }

        // POST: MedEngMgt/AssetKeepTypes/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit(AssetKeepType assetKeepType)
        {
            if (ModelState.IsValid)
            {
                var kp = db.AssetKeepTypes.Find(assetKeepType.AssetNo, assetKeepType.KeepTypeNo);
                if (kp == null)
                {
                    assetKeepType.KeepEngName = db.AppUsers.Find(assetKeepType.KeepEngId).FullName;
                    db.AssetKeepTypes.Add(assetKeepType);
                }
                else
                {
                    var eng = db.AppUsers.Find(assetKeepType.KeepEngId);
                    if (eng != null)
                    {
                        assetKeepType.KeepEngName = eng.FullName;
                    }
                    db.Entry(kp).State = EntityState.Detached;
                    db.Entry(assetKeepType).State = EntityState.Modified;
                }
                db.SaveChanges();
                return RedirectToAction("List");
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

        // GET: MedEngMgt/AssetKeepTypes/Delete/5
        public ActionResult Delete(string assetNo, string keepTypeNo)
        {
            if (assetNo == null || keepTypeNo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AssetKeepType assetKeepType = db.AssetKeepTypes.Find(assetNo, Convert.ToInt32(keepTypeNo));
            if (assetKeepType == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.AssetKeepTypes.Remove(assetKeepType);
                db.SaveChanges();
            }
            return RedirectToAction("List");
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
