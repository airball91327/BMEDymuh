using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class NeedFileController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        //
        // GET: /NeedFile/

        public ActionResult Index(string id = null, string up = null)
        {
            ViewData["ano"] = id;
            List<NeedFile> nf = new List<NeedFile>();
            if (up == null)
                nf = db.NeedFiles.ToList();
            else if (up == "得標廠商")
            {
                nf = db.NeedFiles.Where(f => f.Type == "1").ToList();
            }
            else if (up == "設備工程師")
            {
                nf = db.NeedFiles.Where(f => f.Type != "3" && f.Type != "9").ToList();
            }
            else if (up == "維修工程師")
            {
                nf = db.NeedFiles.Where(f => f.Type == "3").ToList();
            }
            else if (up == "採購人員")
            {
                NeedFile n = new NeedFile();
                n.SeqNo = 12;
                n.Title = "其他";
                n.FileDes = "";
                nf.Add(n);
            }
            return PartialView(nf);
        }

        public ActionResult GetList(string ano = null)
        {
            List<AssetFile> af = db.AssetFiles.Where(f => f.AssetNo == ano).ToList();
            return View(db.NeedFiles.ToList());
        }

        //
        // GET: /NeedFile/Details/5

        public ActionResult Details(int id = 0)
        {
            NeedFile needfile = db.NeedFiles.Find(id);
            if (needfile == null)
            {
                return HttpNotFound();
            }
            return View(needfile);
        }

        //
        // GET: /NeedFile/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /NeedFile/Create

        [HttpPost]
        public ActionResult Create(NeedFile needfile)
        {
            if (ModelState.IsValid)
            {
                db.NeedFiles.Add(needfile);
                if (TryUpdateModel(needfile))
                {
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(needfile);
        }

        //
        // GET: /NeedFile/Edit/5

        public ActionResult Edit(int id = 0)
        {
            NeedFile needfile = db.NeedFiles.Find(id);
            if (needfile == null)
            {
                return HttpNotFound();
            }
            return View(needfile);
        }

        //
        // POST: /NeedFile/Edit/5

        [HttpPost]
        public ActionResult Edit(NeedFile needfile)
        {
            if (ModelState.IsValid)
            {
                db.Entry(needfile).State = EntityState.Modified;
                if (TryUpdateModel(needfile))
                {
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(needfile);
        }

        //
        // GET: /NeedFile/Delete/5

        public ActionResult Delete(int id = 0)
        {
            NeedFile needfile = db.NeedFiles.Find(id);
            if (needfile == null)
            {
                return HttpNotFound();
            }
            return View(needfile);
        }

        //
        // POST: /NeedFile/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NeedFile needfile = db.NeedFiles.Find(id);
            db.NeedFiles.Remove(needfile);
            if (TryUpdateModel(needfile))
            {
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}