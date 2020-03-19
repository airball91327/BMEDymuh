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
using OfficeOpenXml;
using System.Data.SqlClient;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class AttainFilesController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/AttainFiles
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload(string doctype, string docid)
        {
            AttainFile attainFile = new AttainFile();
            attainFile.DocType = doctype;
            attainFile.DocId = docid;
            attainFile.SeqNo = 1;
            attainFile.IsPublic = "N";

            return PartialView(attainFile);
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult Upload(AttainFile attainFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //接收文件
                    //HttpPostedFileBase file = Request.Files[0];
                    //文件扩展名
                    //string extension = Path.GetExtension(file.FileName);
                    string s = "~/Files";
#if DEBUG
                    s = "~/App_Data";
#endif
                    switch (attainFile.DocType)
                    {
                        case "0":
                            s += "/Budget";
                            break;
                        case "1":
                            s += "/Repair";
                            break;
                        case "2":
                            s += "/Keep";
                            break;
                        case "3":
                            s += "/BuyEvaluate";
                            break;
                        case "4":
                            s += "/Delivery";
                            break;
                        case "5":
                            s += "/Asset";
                            break;
                        case "6":
                            s += "/DeptStok";
                            break;
                        case "7":
                            s += "/MContract";
                            break;
                        case "8":
                            s += "/PContract";
                            break;
                    }
                    var i = db.AttainFiles
                        .Where(a => a.DocType == attainFile.DocType)
                        .Where(a => a.DocId == attainFile.DocId).ToList();
                    attainFile.SeqNo = i.Count == 0 ? 1 : i.Select(a => a.SeqNo).Max() + 1;

                    string path = Path.Combine(Server.MapPath(s), attainFile.DocId + "_"
                    + attainFile.SeqNo.ToString() + Path.GetExtension(Request.Files[0].FileName));
                    Request.Files[0].SaveAs(path);
                    string filelink = attainFile.DocId + "_"
                    + attainFile.SeqNo.ToString() + Path.GetExtension(Request.Files[0].FileName);

                    switch (attainFile.DocType)
                    {
                        case "0":
                            attainFile.FileLink = "Budget/" + filelink;
                            break;
                        case "1":
                            attainFile.FileLink = "Repair/" + filelink;
                            break;
                        case "2":
                            attainFile.FileLink = "Keep/" + filelink;
                            break;
                        case "3":
                            attainFile.FileLink = "BuyEvaluate/" + filelink;
                            break;
                        case "4":
                            attainFile.FileLink = "Delivery/" + filelink;
                            break;
                        case "5":
                            attainFile.FileLink = "Asset/" + filelink;
                            break;
                        case "6":
                            attainFile.FileLink = "DeptStok/" + filelink;
                            break;
                        case "7":
                            attainFile.FileLink = "MContract/" + filelink;
                            break;
                        case "8":
                            attainFile.FileLink = "PContract/" + filelink;
                            break;
                    }
                    attainFile.Rtt = DateTime.Now;
                    attainFile.Rtp = WebSecurity.CurrentUserId;
                    db.AttainFiles.Add(attainFile);
                    db.SaveChanges();

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

        public ActionResult UpdCases(string doctype, string docid)
        {
            AttainFile attainFile = new AttainFile();
            attainFile.DocType = doctype;
            attainFile.DocId = docid;
            attainFile.SeqNo = 1;
            attainFile.IsPublic = "N";

            return PartialView(attainFile);
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult UpdCases(AttainFile attainFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //接收文件
                    //HttpPostedFileBase file = Request.Files[0];
                    //文件扩展名
                    //string extension = Path.GetExtension(file.FileName);
                    string s = "~/Files";
#if DEBUG
                    s = "~/App_Data";
#endif
                    switch (attainFile.DocType)
                    {
                        case "0":
                            s += "/Budget";
                            break;
                        case "1":
                            s += "/Repair";
                            break;
                        case "2":
                            s += "/Keep";
                            break;
                        case "3":
                            s += "/BuyEvaluate";
                            break;
                        case "4":
                            s += "/Delivery";
                            break;
                        case "5":
                            s += "/Asset";
                            break;
                        case "6":
                            s += "/DeptStok";
                            break;
                        case "7":
                            s += "/MContract";
                            break;
                        case "8":
                            s += "/PContract";
                            break;
                    }
                    //
                    string[] f = attainFile.DocId.Split(new char[] { ';' });
                    List<AttainFile> i = new List<AttainFile>();
                    AttainFile af;
                    string path = "";
                    string filelink = "";
                    foreach (string ff in f)
                    {
                        if (!string.IsNullOrEmpty(ff))
                        {
                            af = new AttainFile();
                            af.DocId = ff;
                            af.DocType = attainFile.DocType;
                            i = db.AttainFiles.Where(a => a.DocType == attainFile.DocType)
                                  .Where(a => a.DocId == ff).ToList();
                            af.SeqNo = i.Count == 0 ? 1 : i.Select(a => a.SeqNo).Max() + 1;
                            path = Path.Combine(Server.MapPath(s), ff + "_"
                                   + af.SeqNo.ToString() + Path.GetExtension(Request.Files[0].FileName));
                            Request.Files[0].SaveAs(path);
                            filelink = af.DocId + "_" + af.SeqNo.ToString() 
                                       + Path.GetExtension(Request.Files[0].FileName);
                            //
                            switch (af.DocType)
                            {
                                case "0":
                                    af.FileLink = "Budget/" + filelink;
                                    break;
                                case "1":
                                    af.FileLink = "Repair/" + filelink;
                                    break;
                                case "2":
                                    af.FileLink = "Keep/" + filelink;
                                    break;
                                case "3":
                                    af.FileLink = "BuyEvaluate/" + filelink;
                                    break;
                                case "4":
                                    af.FileLink = "Delivery/" + filelink;
                                    break;
                                case "5":
                                    af.FileLink = "Asset/" + filelink;
                                    break;
                                case "6":
                                    af.FileLink = "DeptStok/" + filelink;
                                    break;
                                case "7":
                                    attainFile.FileLink = "MContract/" + filelink;
                                    break;
                                case "8":
                                    attainFile.FileLink = "PContract/" + filelink;
                                    break;
                            }
                            af.Title = attainFile.Title;
                            af.IsPublic = attainFile.IsPublic;
                            af.Rtt = DateTime.Now;
                            af.Rtp = WebSecurity.CurrentUserId;
                            db.AttainFiles.Add(af);
                            db.SaveChanges();
                        }
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

        public ActionResult Upload2(string doctype, string docid)
        {
            AttainFile attainFile = new AttainFile();
            attainFile.DocType = doctype;
            attainFile.DocId = docid;
            attainFile.SeqNo = 1;
            attainFile.IsPublic = "N";

            return PartialView(attainFile);
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult Upload2(AttainFile attainFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //接收文件
                    //HttpPostedFileBase file = Request.Files[0];
                    //文件扩展名
                    //string extension = Path.GetExtension(file.FileName);
                    string s = "~/Files";
#if DEBUG
                    s = "~/App_Data";
#endif
                    switch (attainFile.DocType)
                    {
                        case "0":
                            s += "/Budget";
                            break;
                        case "1":
                            s += "/Repair";
                            break;
                        case "2":
                            s += "/Keep";
                            break;
                        case "3":
                            s += "/BuyEvaluate";
                            break;
                        case "4":
                            s += "/Delivery";
                            break;
                        case "5":
                            s += "/Asset";
                            break;
                        case "6":
                            s += "/DeptStok";
                            break;
                        case "7":
                            s += "/MContract";
                            break;
                        case "8":
                            s += "/PContract";
                            break;
                    }
                    var i = db.AttainFiles
                        .Where(a => a.DocType == attainFile.DocType)
                        .Where(a => a.DocId == attainFile.DocId).ToList();
                    attainFile.SeqNo = i.Count == 0 ? 1 : i.Select(a => a.SeqNo).Max() + 1;

                    string path = Path.Combine(Server.MapPath(s), attainFile.DocId + "_"
                    + attainFile.SeqNo.ToString() + Path.GetExtension(Request.Files[0].FileName));
                    Request.Files[0].SaveAs(path);
                    string filelink = attainFile.DocId + "_"
                    + attainFile.SeqNo.ToString() + Path.GetExtension(Request.Files[0].FileName);

                    switch (attainFile.DocType)
                    {
                        case "0":
                            attainFile.FileLink = "Budget/" + filelink;
                            break;
                        case "1":
                            attainFile.FileLink = "Repair/" + filelink;
                            break;
                        case "2":
                            attainFile.FileLink = "Keep/" + filelink;
                            break;
                        case "3":
                            attainFile.FileLink = "BuyEvaluate/" + filelink;
                            break;
                        case "4":
                            attainFile.FileLink = "Delivery/" + filelink;
                            break;
                        case "5":
                            attainFile.FileLink = "Asset/" + filelink;
                            break;
                        case "6":
                            attainFile.FileLink = "DeptStok/" + filelink;
                            break;
                        case "7":
                            attainFile.FileLink = "MContract/" + filelink;
                            break;
                        case "8":
                            attainFile.FileLink = "PContract/" + filelink;
                            break;
                    }
                    attainFile.Rtt = DateTime.Now;
                    attainFile.Rtp = WebSecurity.CurrentUserId;
                    db.AttainFiles.Add(attainFile);
                    db.SaveChanges();

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

        public ActionResult UpdAssetExcel()
        {

            return PartialView();
        }

        public ActionResult List(string id = null, string typ = null)
        {
            List<AttainFile> af = new List<AttainFile>();
            AppUser u;
            af = db.AttainFiles.Where(f => f.DocType == typ).Where(f => f.DocId == id).ToList();
            foreach (AttainFile a in af)
            {
                u = db.AppUsers.Find(a.Rtp);
                if (u != null)
                    a.UserName = u.FullName;
            }

            return PartialView(af);
        }
        public ActionResult List2(string id = null, string typ = null)
        {
            List<AttainFile> af = new List<AttainFile>();
            AppUser u;
            af = db.AttainFiles.Where(f => f.DocType == typ).Where(f => f.DocId == id).ToList();
            foreach (AttainFile a in af)
            {
                u = db.AppUsers.Find(a.Rtp);
                if (u != null)
                    a.UserName = u.FullName;
            }

            return PartialView(af);
        }

        public ActionResult UpdCasesList(string docid = null, string typ = null)
        {
            List<AttainFile> af = new List<AttainFile>();
            List<AttainFile> flist = new List<AttainFile>();
            AppUser u;
            string[] s = docid.Split(new char[] { ';' });
            foreach(string ss in s)
            {
                if (!string.IsNullOrEmpty(ss))
                {
                    af = db.AttainFiles.Where(f => f.DocType == typ).Where(f => f.DocId == ss).ToList();
                    foreach (AttainFile a in af)
                    {
                        u = db.AppUsers.Find(a.Rtp);
                        if (u != null)
                            a.UserName = u.FullName;
                    }
                    flist.AddRange(af);
                }
            }

            return PartialView(flist);
        }

        public ActionResult Delete(string id = null, int seq = 0, string typ = null)
        {
            AttainFile attainfiles = db.AttainFiles.Find(typ, id, seq);
            if (attainfiles != null)
            {
                FileInfo ff;
                try
                {
                    if (typ == "2")
                    {
                        ff = new FileInfo(Path.Combine(Server.MapPath("~/Files/"), attainfiles.FileLink.Replace("Files/", "")));
#if DEBUG
                        ff = new FileInfo(Path.Combine(Server.MapPath("~/App_Data/"), attainfiles.FileLink.Replace("App_Data/", "")));
#endif
                        ff.Delete();
                    }
                    else
                    {
                        ff = new FileInfo(Path.Combine(Server.MapPath("~/Files/"), attainfiles.FileLink));
#if DEBUG
                        ff = new FileInfo(Path.Combine(Server.MapPath("~/App_Data/"), attainfiles.FileLink));
#endif
                        ff.Delete();
                    }
                }
                catch (Exception e)
                {
                    return Content(e.Message);
                }
                db.AttainFiles.Remove(attainfiles);
                db.SaveChanges();
            }
            List<AttainFile> af = db.AttainFiles.Where(f => f.DocId == id)
                    .Where(f => f.DocType == typ).ToList();
            AppUser u;
            foreach (AttainFile a in af)
            {
                u = db.AppUsers.Find(a.Rtp);
                if (u != null)
                    a.UserName = u.FullName;
            }

            return PartialView("List", af);
        }

        public ActionResult Delete2(string id = null, int seq = 0, string typ = null)
        {
            AttainFile attainfiles = db.AttainFiles.Find(typ, id, seq);
            if (attainfiles != null)
            {
                FileInfo ff;
                try
                {
                    if (typ == "2")
                    {
                        ff = new FileInfo(Path.Combine(Server.MapPath("~/Files/"), attainfiles.FileLink.Replace("Files/", "")));
#if DEBUG
                        ff = new FileInfo(Path.Combine(Server.MapPath("~/App_Data/"), attainfiles.FileLink.Replace("App_Data/", "")));
#endif
                        ff.Delete();
                    }
                    else
                    {
                        ff = new FileInfo(Path.Combine(Server.MapPath("~/Files/"), attainfiles.FileLink));
#if DEBUG
                        ff = new FileInfo(Path.Combine(Server.MapPath("~/App_Data/"), attainfiles.FileLink));
#endif
                        ff.Delete();
                    }
                }
                catch (Exception e)
                {
                    return Content(e.Message);
                }
                db.AttainFiles.Remove(attainfiles);
                db.SaveChanges();
            }
            List<AttainFile> af = db.AttainFiles.Where(f => f.DocId == id)
                    .Where(f => f.DocType == typ).ToList();
            AppUser u;
            foreach (AttainFile a in af)
            {
                u = db.AppUsers.Find(a.Rtp);
                if (u != null)
                    a.UserName = u.FullName;
            }
            return PartialView("List2", af);
        }

        public ActionResult DelUpdCases(string id = null, int seq = 0, string typ = null, string docs = null)
        {
            AttainFile attainfiles = db.AttainFiles.Find(typ, id, seq);
            if (attainfiles != null)
            {
                string fs = "~/Files";
#if DEBUG
                fs = "~/App_Data";
#endif

                FileInfo ff;
                try
                {
                    if (typ == "2")
                    {
                        ff = new FileInfo(Path.Combine(Server.MapPath(fs), attainfiles.FileLink.Replace("Files", "")));
                        ff.Delete();
                    }
                    else
                    {
                        ff = new FileInfo(Path.Combine(Server.MapPath(fs), attainfiles.FileLink));
                        ff.Delete();
                    }
                }
                catch (Exception e)
                {
                    return Content(e.Message);
                }
                db.AttainFiles.Remove(attainfiles);
                db.SaveChanges();
            }
            //
            List<AttainFile> af = new List<AttainFile>();
            List<AttainFile> flist = new List<AttainFile>();
            AppUser u;
            string[] s = docs.Split(new char[] { ';' });
            foreach (string ss in s)
            {
                if (!string.IsNullOrEmpty(ss))
                {
                    af = db.AttainFiles.Where(f => f.DocType == typ).Where(f => f.DocId == ss).ToList();
                    foreach (AttainFile a in af)
                    {
                        u = db.AppUsers.Find(a.Rtp);
                        if (u != null)
                            a.UserName = u.FullName;
                    }
                    flist.AddRange(af);
                }
            }

            return PartialView("UpdCasesList", flist);
        }

        [HttpPost]
        public JsonResult CountFile(string id = null, string typ = null)
        {
            List<AttainFile> af = new List<AttainFile>();
            af = db.AttainFiles.Where(f => f.DocType == typ).Where(f => f.DocId == id).ToList();

            return Json(af.Count());

        }

        // GET: /AttainFiles/Create
        public ActionResult Create(string id = null, string typ = null, string title = null, int? vendorId = null)
        {
            AttainFile a = new AttainFile();
            a.DocType = typ;
            a.DocId = id;
            a.Title = title;
            if (typ == "3" && vendorId != null)
                a.Rtp = vendorId;
            else
                a.Rtp = WebSecurity.CurrentUserId;
            a.Rtt = DateTime.Now;
            return View(a);
        }

        // POST: /AttainFiles/Create
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Create(AttainFile attainFile, IEnumerable<HttpPostedFileBase> file)
        {
            string s = "~/Files";
#if DEBUG
            s = "~/App_Data";
#endif
            switch (attainFile.DocType)
            {
                case "0":
                    s += "/Budget";
                    break;
                case "1":
                    s += "/Repair";
                    break;
                case "2":
                    s += "/Keep";
                    break;
                case "3":
                    s += "/BuyEvaluate";
                    break;
                case "4":
                    s += "/Delivery";
                    break;
                case "5":
                    s += "/Asset";
                    break;
                case "6":
                    s += "/DeptStok";
                    break;
                case "7":
                    s += "/MContract";
                    break;
                case "8":
                    s += "/PContract";
                    break;
            }

            int? i = db.Database.SqlQuery<int?>("SELECT MAX(SEQNO) FROM AttainFile WHERE DOCTYPE = @typ AND DOCID = @id",
                    new SqlParameter("typ", attainFile.DocType),
                    new SqlParameter("id", attainFile.DocId)).FirstOrDefault();
            if (i == null)
                attainFile.SeqNo = 1;
            else
                attainFile.SeqNo = Convert.ToInt32(i + 1);
            string path = Path.Combine(Server.MapPath(s), attainFile.DocId + "_"
                + attainFile.SeqNo.ToString() + Path.GetExtension(Request.Files[0].FileName));
            string filelink = attainFile.DocId + "_"
                + attainFile.SeqNo.ToString() + Path.GetExtension(Request.Files[0].FileName);
            try
            {
                Request.Files[0].SaveAs(path);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            switch (attainFile.DocType)
            {
                case "0":
                    attainFile.FileLink = "Budget/" + filelink;
                    break;
                case "1":
                    attainFile.FileLink = "Repair/" + filelink;
                    break;
                case "2":
                    attainFile.FileLink = "Keep/" + filelink;
                    break;
                case "3":
                    attainFile.FileLink = "BuyEvaluate/" + filelink;
                    break;
                case "4":
                    attainFile.FileLink = "Delivery/" + filelink;
                    break;
                case "5":
                    attainFile.FileLink = "Asset/" + filelink;
                    break;
                case "6":
                    attainFile.FileLink = "DeptStok/" + filelink;
                    break;
                case "7":
                    attainFile.FileLink = "MContract/" + filelink;
                    break;
                case "8":
                    attainFile.FileLink = "PContract/" + filelink;
                    break;
            }
            if (attainFile.IsPub)
                attainFile.IsPublic = "Y";
            attainFile.Rtt = DateTime.Now;
            db.AttainFiles.Add(attainFile);
            try
            {
                db.SaveChanges();
                //
                //if (attainFile.DocType == "0")
                //{
                //    string s1 = ReadBudgetExcel(attainFile);
                //    if (!string.IsNullOrEmpty(s1))
                //    {
                //        throw new Exception(s1);
                //    }
                //}
                //return new JsonResult
                //{
                //    Data = new { success = true, error = "" },
                //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                //};
                return Content("檔案上載完成");
            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
                ModelState.AddModelError("", e);
                return Content(e.Message);
            }

        }

    }
}