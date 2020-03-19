﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebMatrix.WebData;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using System.IO;
using System.Data.SqlClient;
using BMEDmgt.Models;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class AssetFileController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        //
        // GET: /AssetFile/

        public ActionResult Index()
        {
            return View(db.AssetFiles.ToList());
        }

        //
        // GET: /AssetFile/Create

        public ActionResult Create(string id = null, int sno = 1, string title = null)
        {
            AssetFile a = new AssetFile();
            a.AssetNo = id;
            a.Title = title;
            a.SeqNo = sno;
            a.Rtp = WebSecurity.CurrentUserId.ToString();
            a.Rtt = DateTime.Now;
            return View(a);
        }

        //
        // POST: /AssetFile/Create

        [HttpPost]
        public ActionResult Create(AssetFile assetfile, IEnumerable<HttpPostedFileBase> file)
        {
            if (ModelState.IsValid)
            {
                string s = "~/Files";
                s += "/Asset";

#if DEBUG
                s = "~/App_Data";
#endif
                int? i = db.Database.SqlQuery<int?>("SELECT MAX(Fid) FROM AssetFile WHERE ASSETNO = @ano AND SEQNO = @sno ",
                        new SqlParameter("ano", assetfile.AssetNo),
                        new SqlParameter("sno", assetfile.SeqNo)).FirstOrDefault();
                if (i == null)
                    assetfile.Fid = 1;
                else
                    assetfile.Fid = Convert.ToInt32(i + 1);
                string path = Path.Combine(Server.MapPath(s), assetfile.AssetNo + "_"
                    + assetfile.SeqNo.ToString() + "_" + assetfile.Fid.ToString() + Path.GetExtension(Request.Files[0].FileName));
                string filelink = assetfile.AssetNo + "_" + assetfile.SeqNo.ToString() + "_"
                    + assetfile.Fid.ToString() + Path.GetExtension(Request.Files[0].FileName);
                try
                {
                    Request.Files[0].SaveAs(path);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e);
                }
                assetfile.FileLink = "Asset/" + filelink;
                assetfile.Rtt = DateTime.Now;
                db.AssetFiles.Add(assetfile);
                try
                {
                    if (TryUpdateModel(assetfile))
                    {
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e);
                }
                return View();
            }

            return Content("錯誤!!");
        }

        // GET: /AssetFile/CopyTo

        public ActionResult CopyTo(string id, int sno = 1)
        {
            Asset t = db.Assets.Find(id);
            List<Asset> at = new List<Asset>();
            if (t != null)
            {
                at = db.Assets.Where(a => a.Docid == t.Docid).ToList();
                AssetFile af = db.AssetFiles.Where(f => f.AssetNo == id && f.SeqNo == sno)
                    .FirstOrDefault();
                ViewData["ano"] = id;
                ViewData["sno"] = sno;
                ViewData["cname"] = t.Cname;
                ViewData["title"] = af.Title;
            }
            return View(at);
        }

        [HttpPost]
        public ActionResult CopyTo(FormCollection fm)
        {
            AssetFile af;
            string ano = fm["ano"];
            int sno = Convert.ToInt32(fm["sno"]);
            List<AssetFile> fs = db.AssetFiles.Where(f => f.AssetNo == ano && f.SeqNo == sno).ToList();
            string[] slist = fm.GetValues("IsCopy");
            int fid = 0;
            if (slist != null)
            {
                foreach (string s in slist)
                {
                    fid = db.AssetFiles.Where(f => f.AssetNo == s && f.SeqNo == sno)
                        .DefaultIfEmpty()
                        .Max(p => p == null ? 0 : p.Fid);

                    foreach (AssetFile a in fs)
                    {
                        fid++;
                        FileInfo inf = new FileInfo(Server.MapPath("~/Files/") + a.FileLink);
                        af = new AssetFile();
                        af.AssetNo = s;
                        af.SeqNo = sno;
                        af.Fid = fid;
                        af.Title = a.Title;
                        af.FileLink = "Asset/" + af.AssetNo + "_" + af.SeqNo.ToString() + "_"
                                   + af.Fid.ToString() + inf.Extension;
                        af.Rtp = WebSecurity.CurrentUserId.ToString();
                        af.Rtt = DateTime.Now;
                        db.AssetFiles.Add(af);
                        try
                        {
                            inf.CopyTo(Server.MapPath("~/Files/") + af.FileLink);
                        }
                        catch (Exception ex)
                        {
                            return Content(ex.Message);
                        }
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
            }

            return Content("");
        }

        //
        // GET: /AssetFile/Delete/5

        public ActionResult Delete(string ano = null, int seqno = 1, int fid = 1)
        {
            AssetFile assetfile = db.AssetFiles.Find(ano, seqno, fid);
            if (assetfile == null)
            {
                return HttpNotFound();
            }
            else
            {
                try
                {
                    FileInfo ff = new FileInfo(Path.Combine(Server.MapPath("~/Files"), assetfile.FileLink));
                    ff.Delete();
                    db.AssetFiles.Remove(assetfile);
                    if (TryUpdateModel(assetfile))
                    {
                        db.SaveChanges();
                    }
                    //return Json(new { msg = "儲存成功!" }, JsonRequestBehavior.AllowGet);
                    return Content("");
                }
                catch (Exception ex)
                {
                    return Content(ex.Message);
                }
                //List<AssetFile> af = db.AssetFiles.Where(f => f.AssetNo == ano).ToList();
                //if (title != null)
                //    af = af.Where(f => f.Title == title).ToList();
                //return PartialView("List", af);

            }
        }

        //
        // POST: /AssetFile/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id)
        {
            AssetFile assetfile = db.AssetFiles.Find(id);
            db.AssetFiles.Remove(assetfile);
            if (TryUpdateModel(assetfile))
            {
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult List(string id = null, string title = null)
        {
            List<AssetFile> af = new List<AssetFile>();
            if (id != null)
            {
                AppUser u;
                af = db.AssetFiles.Where(f => f.AssetNo == id).ToList();
                if (title != null)
                    af = af.Where(f => f.Title == title).ToList();
                foreach (AssetFile a in af)
                {
                    u = db.AppUsers.Find(Convert.ToInt32(a.Rtp));
                    a.UserName = u.FullName;
                }
            }
            else
            {
                AppUser u;
                af = db.AssetFiles.ToList();
                foreach (AssetFile a in af)
                {
                    u = db.AppUsers.Find(Convert.ToInt32(a.Rtp));
                    a.UserName = u.FullName;
                }
            }
            return View(af);
        }
        //public ActionResult AssetList(string id = null, int sno = 1, string title = null)
        //{
        //    List<AssetFile> af = new List<AssetFile>();
        //    if (id != null)
        //    {
        //        Asset at = db.Assets.Find(id);
        //        ViewData["PlantNo"] = at.AssetNo;
        //        ViewData["PlantName"] = at.Cname;
        //        UserProfile u;
        //        af = db.AssetFiles.Where(f => f.AssetNo == id && f.SeqNo == sno).ToList();
        //        if (title != null)
        //            af = af.Where(f => f.Title == title).ToList();
        //        foreach (AssetFile a in af)
        //        {
        //            u = db.UserProfiles.Find(Convert.ToInt32(a.Rtp));
        //            a.UserName = u.FullName;
        //        }
        //    }
        //    else
        //    {
        //        UserProfile u;
        //        af = db.AssetFiles.ToList();
        //        foreach (AssetFile a in af)
        //        {
        //            u = db.UserProfiles.Find(Convert.ToInt32(a.Rtp));
        //            a.UserName = u.FullName;
        //        }
        //    }
        //    return View(af);
        //}

        public ActionResult AssetList(string id)
        {
            List<AssetFile> af = new List<AssetFile>();
            if (id != null)
            {
                Asset at = db.Assets.Find(id);
                ViewData["PlantNo"] = at.AssetNo;
                ViewData["PlantName"] = at.Cname;
                AppUser u;
                af = db.AssetFiles.Where(f => f.AssetNo == id).OrderBy(f => f.SeqNo).ToList();

                foreach (AssetFile a in af)
                {
                    u = db.AppUsers.Find(Convert.ToInt32(a.Rtp));
                    a.UserName = u.FullName;
                }
            }
            else
            {
                AppUser u;
                af = db.AssetFiles.ToList();
                foreach (AssetFile a in af)
                {
                    u = db.AppUsers.Find(Convert.ToInt32(a.Rtp));
                    a.UserName = u.FullName;
                }
            }
            return View(af);
        }

        public ActionResult CheckFiles(string id = null, string cls = null)
        {
            if (id != null)
            {
                List<AssetFile> af = db.AssetFiles.Where(f => f.AssetNo == id).ToList();
                List<NeedFile> nf = new List<NeedFile>();
                if (cls == "得標廠商")
                    nf = db.NeedFiles.Where(e => e.Type == "1").ToList();
                else if (cls == "設備工程師")
                    nf = db.NeedFiles.Where(e => e.Type == "2").ToList();
                foreach (NeedFile n in nf)
                {
                    if (af.Where(f => f.Title == n.Title).Count() <= 0)
                        return Content("檔案尚未上載完成!!");
                }
            }
            return Content("");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}