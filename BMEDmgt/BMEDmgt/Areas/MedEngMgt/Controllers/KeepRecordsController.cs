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

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class KeepRecordsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        // GET: MedEngMgt/KeepRecords
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(string id = null)
        {
            Keep kp = db.Keeps.Find(id);
            List<KeepFormatListVModel> kf = new List<KeepFormatListVModel>();
            KeepFormat f;
            KeepRecord r;
            if (kp != null)
            {
                AssetKeep ak = db.AssetKeeps.Find(kp.AssetNo);
                if (ak != null)
                {
                    if (!string.IsNullOrEmpty(ak.FormatId))
                    {
                        db.KeepFormatDtls.Where(d => d.FormatId == ak.FormatId)
                            .ToList()
                            .ForEach(d =>
                            {
                                kf.Add(new KeepFormatListVModel
                                {
                                    Docid = id,
                                    FormatId = d.FormatId,
                                    Plants = (f = db.KeepFormats.Find(d.FormatId)) == null ? "" :
                                    f.Plants,
                                    Sno = d.Sno,
                                    Descript = d.Descript,
                                    KeepDes = (r = db.KeepRecords.Find(id, d.FormatId, d.Sno)) == null ? "" :
                                    r.KeepDes
                                });
                            });
                    }
                    
                }
                KeepFlow kf2 = db.KeepFlows.Where(f2 => f2.DocId == id)
                          .Where(f2 => f2.Status == "?").ToList().FirstOrDefault();
                if (kf2.Cls.Contains("工程師"))
                    return PartialView(kf);
                else
                    return PartialView("Details", kf);
            }
            return HttpNotFound();
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit(FormCollection vmodel)
        {
            if (ModelState.IsValid)
            {
                KeepRecord r;
                KeepRecord r2;
                if (vmodel.GetValues("item.Sno") == null)
                {
                    throw new Exception("需先登錄保養項目!");
                }
                int i = vmodel.GetValues("item.Sno").Count();
                for (int j = 0; j < i; j++)
                {
                    r = new KeepRecord();
                    r.DocId = vmodel.GetValues("item.DocId")[j];
                    r.FormatId = vmodel.GetValues("item.FormatId")[j];
                    r.Sno = Convert.ToInt32(vmodel.GetValues("item.Sno")[j]);
                    r.Descript = vmodel.GetValues("item.Descript")[j];
                    r.KeepDes = vmodel.GetValues("item.KeepDes")[j];
                    r2 = db.KeepRecords.Find(r.DocId, r.FormatId, r.Sno);
                    if (r2 != null)
                    {
                        r2.KeepDes = r.KeepDes;
                        db.Entry(r2).State = EntityState.Modified;
                    }
                    else
                    {
                        db.KeepRecords.Add(r);
                    }
                }
                try
                {
                    db.SaveChanges();
                    return new JsonResult
                    {
                        Data = new { success = true, error = "" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
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
        }

        public ActionResult Details(string id = null)
        {
            Keep kp = db.Keeps.Find(id);
            List<KeepFormatListVModel> kf = new List<KeepFormatListVModel>();
            KeepFormat f;
            KeepRecord r;
            if (kp != null)
            {
                AssetKeep ak = db.AssetKeeps.Find(kp.AssetNo);
                if (ak != null)
                {
                    if (!string.IsNullOrEmpty(ak.FormatId))
                    {
                        db.KeepFormatDtls.Where(d => d.FormatId == ak.FormatId)
                            .ToList()
                            .ForEach(d =>
                            {
                                kf.Add(new KeepFormatListVModel
                                {
                                    Docid = id,
                                    FormatId = d.FormatId,
                                    Plants = (f = db.KeepFormats.Find(d.FormatId)) == null ? "" :
                                    f.Plants,
                                    Sno = d.Sno,
                                    Descript = d.Descript,
                                    KeepDes = (r = db.KeepRecords.Find(id, d.FormatId, d.Sno)) == null ? "" :
                                    r.KeepDes
                                });
                            });
                    }

                }
                return PartialView("Details", kf);
            }
            return HttpNotFound();
        }

    }
}