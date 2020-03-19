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
using System.IO;
using WebMatrix.WebData;
using PagedList;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    //[Authorize]
    public class ManualsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 100;
        // GET: MedEngMgt/Manuals
        public ActionResult Index()
        {
            //
            List<SelectListItem> Item = new List<SelectListItem>();
            db.Manuals.Select(m => m.FileType).Distinct()
                .ToList()
                .ForEach(m =>
                {
                    Item.Add(new SelectListItem
                    {
                        Text = m,
                        Value = m
                    });
                });
            ViewData["FTYPE"] = new SelectList(Item, "Value", "Text", "");

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection fm, int page = 1)
        {
            string brand = fm["qtyBRAND"];
            string type = fm["qtyTYPE"];
            string ftype = fm["qtyFTYPE"];
            List<Manual> mlist;
            mlist = db.Manuals.ToList();
            if (!string.IsNullOrEmpty(brand))
            {
                mlist = mlist.Where(m => m.AssetBrand == brand).ToList();
            }
            if (!string.IsNullOrEmpty(type))
            {
                mlist = mlist.Where(m => m.AssetType == type).ToList();
            }
            if (!string.IsNullOrEmpty(ftype))
            {
                mlist = mlist.Where(m => m.FileType == ftype).ToList();
            }

            if (mlist.ToPagedList(page, pageSize).Count <= 0)
                return PartialView("List", mlist.ToPagedList(1, pageSize));

            return PartialView("List", mlist.ToPagedList(page, pageSize));
        }

        // GET: MedEngMgt/Manuals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manual manual = db.Manuals.Find(id);
            if (manual == null)
            {
                return HttpNotFound();
            }
            return View(manual);
        }

        // GET: MedEngMgt/Manuals/Create
        public ActionResult Create()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "中文", Value = "中文" });
            listItem.Add(new SelectListItem { Text = "英文", Value = "英文" });
            listItem.Add(new SelectListItem { Text = "日文", Value = "日文" });
            ViewData["FileLanguage"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<SelectListItem> Item = new List<SelectListItem>();
            db.Manuals.Select(m => m.FileType).Distinct()
                .ToList()
                .ForEach(m =>
                {
                    Item.Add(new SelectListItem
                    {
                        Text = m,
                        Value = m
                    });
                });
            Item.Add(new SelectListItem
            {
                Text = "新增",
                Value = "新增"
            });
            ViewData["FileType"] = new SelectList(Item, "Value", "Text", "");
            //
            return View();
        }

        // POST: MedEngMgt/Manuals/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Create(Manual manual)
        {
            if (ModelState.IsValid)
            {
                manual.Rtp = WebSecurity.CurrentUserId;
                manual.Rtt = DateTime.Now;
                db.Manuals.Add(manual);
                if (TryUpdateModel(manual))
                {
                    db.SaveChanges();
                }
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

        [HttpPost]
        [MyErrorHandler]
        public ActionResult Upload(Manual manaul)
        {
            if (ModelState.IsValid)
            {
                if (manaul.FileType == "新增")
                {
                    if (!string.IsNullOrEmpty(manaul.NewFileType))
                        manaul.FileType = manaul.NewFileType;
                    else
                        throw new Exception("檔案類型不可空白!");
                }
                try
                {
                    string s = "~/Files";
#if DEBUG
                    s = "~/App_Data";
#endif
                    string path = s + "/" + manaul.FileType + "/"
                        + manaul.AssetBrand + "_" + manaul.AssetType +
                        "/" + manaul.FileName + Path.GetExtension(Request.Files[0].FileName);
                    string dir = s + "/" + manaul.FileType + "/" + manaul.AssetBrand + "_" + manaul.AssetType +
                        "/";

                    Directory.CreateDirectory(Server.MapPath(dir));
                    FileInfo ff = new FileInfo(Server.MapPath(path));
                    FileStream fs = ff.OpenWrite();
                    Request.Files[0].InputStream.CopyTo(fs);
                    fs.Close();
                    //                    
                    manaul.FilePath = "/Files/" + manaul.FileType + "/" + manaul.AssetBrand + "_" + manaul.AssetType +
                        "/";
                    manaul.FileName = manaul.FileName + Path.GetExtension(Request.Files[0].FileName);
                    manaul.Rtp = WebSecurity.CurrentUserId;
                    manaul.Rtt = DateTime.Now;
                    db.Manuals.Add(manaul);
                    if (TryUpdateModel(manaul))
                    {
                        db.SaveChanges();
                    }

                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
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

            return new JsonResult
            {
                Data = new { success = true, error = "" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // GET: MedEngMgt/Manuals/Edit/5
        public ActionResult Edit(int? id)
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "中文", Value = "中文" });
            listItem.Add(new SelectListItem { Text = "英文", Value = "英文" });
            listItem.Add(new SelectListItem { Text = "日文", Value = "日文" });
            ViewData["FileLanguage"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<SelectListItem> Item = new List<SelectListItem>();
            db.Manuals.Select(m => m.FileType).Distinct()
                .ToList()
                .ForEach(m =>
                {
                    Item.Add(new SelectListItem
                    {
                        Text = m,
                        Value = m
                    });
                });
            ViewData["FileType"] = new SelectList(Item, "Value", "Text", "");
            //
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manual manual = db.Manuals.Find(id);
            if (manual == null)
            {
                return HttpNotFound();
            }
            return View(manual);
        }

        // POST: MedEngMgt/Manuals/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit(Manual manual)
        {
            if (ModelState.IsValid)
            {
                manual.Rtp = WebSecurity.CurrentUserId;
                manual.Rtt = DateTime.Now;
                db.Entry(manual).State = EntityState.Modified;
                if (TryUpdateModel(manual))
                {
                    db.SaveChanges();
                }
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

        // GET: MedEngMgt/Manuals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Manual manual = db.Manuals.Find(id);
            if (manual == null)
            {
                return HttpNotFound();
            }
            return View(manual);
        }

        // POST: MedEngMgt/Manuals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Manual manual = db.Manuals.Find(id);
            string path = "~/Files/" + manual.FileType + "/" + manual.AssetBrand + "_" + manual.AssetType +
                        "/" + manual.FileName;
#if DEBUG
            path = "~/App_Data/desk/dd_123/dddd.dmg";
#endif
            FileInfo ff = new FileInfo(Server.MapPath(path));
            if (ff.Exists)
            {
                ff.Delete();
            }
            db.Manuals.Remove(manual);
            if (TryUpdateModel(manual))
            {
                db.SaveChanges();
            }
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
