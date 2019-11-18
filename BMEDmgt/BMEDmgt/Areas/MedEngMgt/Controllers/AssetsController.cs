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
using Newtonsoft.Json;
using PagedList;
using System.Web.Security;
using WebMatrix.WebData;
using OfficeOpenXml;
using System.IO;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class AssetsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 100;

        // GET: MedEngMgt/Assets
        public ActionResult Index()
        {
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            SelectListItem li;
            db.Departments.ToList()
                .ForEach(d =>
                {
                    li = new SelectListItem();
                    li.Text = d.Name_C;
                    li.Value = d.DptId;
                    listItem2.Add(li);

                });
            ViewData["ACCDPT"] = new SelectList(listItem2, "Value", "Text");
            ViewData["DELIVDPT"] = new SelectList(listItem2, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection fm, int page = 1)
        {
            QryAsset qryAsset = new QryAsset();
            qryAsset.AssetName = fm["AssetName"];
            qryAsset.AssetNo = fm["AssetNo"];
            qryAsset.AccDpt = fm["AccDpt"];
            qryAsset.DelivDpt = fm["DelivDpt"];
            qryAsset.Type = fm["Type"];
            TempData["qry"] = qryAsset;
            List<Asset> at = new List<Asset>();
            List<Asset> at2 = new List<Asset>();
            db.Assets.GroupJoin(db.Departments, a => a.DelivDpt, d => d.DptId,
                (a, d) => new { Asset = a, Department = d })
                .SelectMany(p => p.Department.DefaultIfEmpty(),
                (x, y) => new { Asset = x.Asset, Department = y })
                .ToList()
                .GroupJoin(db.AppUsers, e => e.Asset.DelivUid, u => u.Id,
                (e, u) => new { Asset = e, AppUser = u })
                .SelectMany(p => p.AppUser.DefaultIfEmpty(),
                (e, y) => new { Asset = e.Asset.Asset, Department = e.Asset.Department, AppUser = y })
                .ToList()
                .ForEach(p =>
                {
                    p.Asset.DelivDptName = p.Department == null ? "" : p.Department.Name_C;
                    p.Asset.DelivEmp = p.AppUser == null ? "" : p.AppUser.FullName;
                    at.Add(p.Asset);
                });
            at.GroupJoin(db.Departments, a => a.AccDpt, d => d.DptId,
                (a, d) => new { Asset = a, Department = d })
                .SelectMany(p => p.Department.DefaultIfEmpty(),
                (x, y) => new { Asset = x.Asset, Department = y })
                .ToList()
                .ForEach(p =>
                {
                    p.Asset.AccDptName = p.Department == null ? "" : p.Department.Name_C;
                    at2.Add(p.Asset);
                });
            if (!string.IsNullOrEmpty(qryAsset.AssetNo))
            {
                at2 = at2.Where(a => a.AssetNo == qryAsset.AssetNo).ToList();
            }
            if (!string.IsNullOrEmpty(qryAsset.AssetName))
            {
                at2 = at2.Where(a => a.Cname.Contains(qryAsset.AssetName)).ToList();
            }
            if (!string.IsNullOrEmpty(qryAsset.AccDpt))
            {
                at2 = at2.Where(a => a.AccDpt == qryAsset.AccDpt).ToList();
            }
            if (!string.IsNullOrEmpty(qryAsset.DelivDpt))
            {
                at2 = at2.Where(a => a.DelivDpt == qryAsset.DelivDpt).ToList();
            }
            if (!string.IsNullOrEmpty(qryAsset.Type))
            {
                at2 = at2.Where(a => a.Type == qryAsset.Type).ToList();
            }
            //
            List<SelectListItem> listItem = new List<SelectListItem>();
            Roles.GetUsersInRole("MedEngineer").ToList()
                        .ForEach(x =>
                        {
                            AppUser u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                listItem.Add(new SelectListItem { Text = u.FullName, Value = u.Id.ToString() });
                            }
                        });
            ViewData["KeepEngId"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            string dptid = at2.First().DelivDpt;
            db.AppUsers.Where(u => u.DptId == dptid).ToList()
                .ForEach(u => {
                    listItem2.Add(new SelectListItem { Text = u.FullName, Value = u.Id.ToString() });
                });
            ViewData["DelivUid"] = new SelectList(listItem2, "Value", "Text", "");
            //
            if (at2.ToPagedList(page, pageSize).Count <= 0)
                return PartialView("List", at2.ToPagedList(1, pageSize));
            return PartialView("List", at2.ToPagedList(page, pageSize));
        }

        public List<Asset> QryAsset(QryAsset qryAsset)
        {
            List<Asset> at = new List<Asset>();

            db.Assets.GroupJoin(db.Departments, a => a.DelivDpt, d => d.DptId,
                (a, d) => new { Asset = a, Department = d })
                .SelectMany(p => p.Department.DefaultIfEmpty(),
                (x, y) => new { Asset = x.Asset, Department = y })
                .ToList()
                .GroupJoin(db.AppUsers, e => e.Asset.DelivUid, u => u.Id,
                (e, u) => new { Asset = e, AppUser = u })
                .SelectMany(p => p.AppUser.DefaultIfEmpty(),
                (e, y) => new { Asset = e.Asset.Asset, Department = e.Asset.Department, AppUser = y })
                .ToList()
                .ForEach(p =>
                {
                    p.Asset.DelivDptName = p.Department == null ? "" : p.Department.Name_C;
                    p.Asset.DelivEmp = p.AppUser == null ? "" : p.AppUser.FullName;
                    at.Add(p.Asset);
                });
            at.GroupJoin(db.Departments, a => a.AccDpt, d => d.DptId,
                (a, d) => new { Asset = a, Department = d })
                .SelectMany(p => p.Department.DefaultIfEmpty(),
                (x, y) => new { Asset = x.Asset, Department = y })
                .ToList()
                .ForEach(p =>
                {
                    p.Asset.AccDptName = p.Department == null ? "" : p.Department.Name_C;
                    at.Add(p.Asset);
                });
            if (!string.IsNullOrEmpty(qryAsset.AssetNo))
            {
                at = at.Where(a => a.AssetNo == qryAsset.AssetNo).ToList();
            }
            if (!string.IsNullOrEmpty(qryAsset.AssetName))
            {
                at = at.Where(a => a.Cname.Contains(qryAsset.AssetName)).ToList();
            }
            if (!string.IsNullOrEmpty(qryAsset.AccDpt))
            {
                at = at.Where(a => a.AccDpt == qryAsset.AccDpt).ToList();
            }
            if (!string.IsNullOrEmpty(qryAsset.Type))
            {
                at = at.Where(a => a.Type == qryAsset.Type).ToList();
            }
            if (!string.IsNullOrEmpty(qryAsset.AssetClass1))
            {
                if (string.IsNullOrEmpty(qryAsset.AssetClass2))
                {
                    at = at.Where(a => a.AssetClass == qryAsset.AssetClass1).ToList();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(qryAsset.AssetClass2))
                {
                    at = at.Where(a => a.AssetClass == qryAsset.AssetClass2).ToList();
                }
            }
            if (qryAsset.AccDate1 != null)
                at = at.Where(a => a.AccDate >= qryAsset.AccDate1).ToList();
            if (qryAsset.AccDate2 != null)
                at = at.Where(a => a.AccDate <= qryAsset.AccDate2).ToList();

            at = at.GroupBy(a => a.AssetNo).Select(g => g.First()).ToList();

            return at;
        }

        public void AssetExcel(QryAsset v)
        {
            DataTable dt = new DataTable();
            DataRow dw;
            dt.Columns.Add("類別");
            dt.Columns.Add("財產編號");
            dt.Columns.Add("設備名稱");
            dt.Columns.Add("成本中心");
            dt.Columns.Add("保管部門");
            dt.Columns.Add("保管人員");
            dt.Columns.Add("工程師名稱");
            dt.Columns.Add("廠牌");
            dt.Columns.Add("規格");
            dt.Columns.Add("型號");
            dt.Columns.Add("製造號碼");
            dt.Columns.Add("財產狀況");
            dt.Columns.Add("成本(取得金額)");
            dt.Columns.Add("保養週期");
            dt.Columns.Add("保養起始月");
            dt.Columns.Add("保養方式");
            dt.Columns.Add("保固起始日");
            dt.Columns.Add("保固終止日");
            dt.Columns.Add("維修工程師(保養用)");
            dt.Columns.Add("購入日(取得日期)");

            List<Asset> mv = QryAsset(v);
            mv.Join(db.AppUsers, a => a.EngId, u => u.Id,
                (a, u) => new {
                    asset = a,
                    user = u
                }).Join(db.AssetKeeps, a => a.asset.AssetNo, k => k.AssetNo,
                (a, k) => new {
                    asset = a.asset,
                    user = a.user,
                    assetkeep = k
                }).ToList()
            .ForEach(m =>
            {
                dw = dt.NewRow();
                dw[0] = m.asset.AssetClass;
                dw[1] = m.asset.AssetNo;
                dw[2] = m.asset.Cname;
                dw[3] = m.asset.AccDptName;
                dw[4] = m.asset.DelivDptName;
                dw[5] = m.asset.DelivEmp;
                dw[6] = m.user.FullName;
                dw[7] = m.asset.Brand;
                dw[8] = m.asset.Standard;
                dw[9] = m.asset.Type;
                dw[10] = m.asset.MakeNo;
                dw[11] = m.asset.DisposeKind;
                dw[12] = m.asset.Cost;
                dw[13] = m.assetkeep.Cycle;
                dw[14] = m.assetkeep.KeepYm;
                dw[15] = m.assetkeep.InOut;
                dw[16] = m.asset.WartySt == null ? "" : m.asset.WartySt.Value.ToString("yyyy/MM/dd");
                dw[17] = m.asset.WartyEd == null ? "" : m.asset.WartyEd.Value.ToString("yyyy/MM/dd");
                dw[18] = m.assetkeep.KeepEngName;
                dw[19] = m.asset.BuyDate == null?"":m.asset.BuyDate.Value.ToString("yyyy/MM/dd");
                dt.Rows.Add(dw);
            });
            //
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add("設備列表清單");
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=AssetKpSche.xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(Response.OutputStream);
                Response.Flush();
                Response.End();
            }
        }
        public ActionResult List()
        {
            List<Asset> at = db.Assets.ToList();
            Department d;
            at.ForEach(a =>
            {
                a.DelivDptName = (d = db.Departments.Find(a.DelivDpt)) == null ? "" : d.Name_C;
            });

            return PartialView(at);
        }

        // GET: MedEngMgt/Assets/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            if(asset.DelivUid != null)
            {
                asset.DelivEmp = "(" + db.AppUsers.Find(asset.DelivUid.Value).UserName + ") "
                    + asset.DelivEmp;
            }           
            asset.DelivDptName = db.Departments.Find(asset.DelivDpt).Name_C;
            asset.AccDptName = db.Departments.Find(asset.AccDpt).Name_C;
            asset.VendorName = asset.VendorId == null ? "" : db.Vendors.Find(asset.VendorId).VendorName;

            return View(asset);
        }

        public ActionResult AssetView(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            asset.DelivDptName = db.Departments.Find(asset.DelivDpt).Name_C;
            asset.AccDptName = db.Departments.Find(asset.AccDpt).Name_C;

            return PartialView(asset);
        }

        // GET: MedEngMgt/Assets/Create
        public ActionResult Create()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            db.Departments.ToList().ForEach(d =>
            {
                listItem.Add(new SelectListItem { Text = d.Name_C, Value = d.DptId });
            });
            ViewData["DelivDpt"] = new SelectList(listItem, "Value", "Text", "");

            List<SelectListItem> listItem2 = new List<SelectListItem>();
            listItem2.Add(new SelectListItem { Text = "", Value = "" });
            ViewData["DelivUid"] = new SelectList(listItem2, "Value", "Text", "");

            ViewData["AccDpt"] = new SelectList(listItem, "Value", "Text", "");

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Text = "正常", Value = "正常" });
            listItem3.Add(new SelectListItem { Text = "報廢", Value = "報廢" });
            ViewData["DisposeKind"] = new SelectList(listItem3, "Value", "Text", "");
            //
            List<SelectListItem> listItem4 = new List<SelectListItem>();
            db.DeviceClassCodes.ToList()
                .ForEach(d =>
                {
                    listItem4.Add(new SelectListItem { Text = d.M_name, Value = d.M_code });
                });
            ViewData["BmedNo"] = new SelectList(listItem4, "Value", "Text", "");
            //
            List<SelectListItem> listItem5 = new List<SelectListItem>();
            AppUser u;
            Roles.GetUsersInRole("MedEngineer").ToList()
                        .ForEach(x =>
                        {
                            u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                listItem5.Add(new SelectListItem { Text = u.FullName, Value = u.Id.ToString() });
                            }
                        });
            ViewData["EngId"] = new SelectList(listItem5, "Value", "Text", "");

            return View();
        }

        // POST: MedEngMgt/Assets/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Create(Asset asset)
        {
            if (ModelState.IsValid)
            {
                asset.AssetNo = asset.AssetNo.Trim();
                if (db.Assets.Find(asset.AssetNo) != null)
                {
                    throw new Exception("財產編號已經存在!!");
                }
                try
                {
                    asset.DelivEmp = asset.DelivUid == null ? "" : db.AppUsers.Find(asset.DelivUid).FullName;
                    db.Assets.Add(asset);
                    if (db.AssetKeeps.Find(asset.AssetNo) == null)
                    {
                        AssetKeep ak = new AssetKeep();
                        ak.AssetNo = asset.AssetNo;
                        db.AssetKeeps.Add(ak);
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                return new JsonResult
                {
                    Data = new { success = true, error = "", id = asset.AssetNo },
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
            //List<SelectListItem> listItem = new List<SelectListItem>();
            //db.Departments.ToList().ForEach(d => {
            //    listItem.Add(new SelectListItem { Text = d.Name_C, Value = d.DptId });
            //});
            //ViewData["DelivDpt"] = new SelectList(listItem, "Value", "Text", "");

            //List<SelectListItem> listItem2 = new List<SelectListItem>();
            //listItem2.Add(new SelectListItem { Text = "", Value = "" });
            //ViewData["DelivUid"] = new SelectList(listItem2, "Value", "Text", "");

            //ViewData["AccDpt"] = new SelectList(listItem, "Value", "Text", "");

            //List<SelectListItem> listItem3 = new List<SelectListItem>();
            //listItem3.Add(new SelectListItem { Text = "正常", Value = "正常" });
            //listItem3.Add(new SelectListItem { Text = "報廢", Value = "報廢" });
            //ViewData["DisposeKind"] = new SelectList(listItem3, "Value", "Text", "");

            //return View(asset);
        }

        // GET: MedEngMgt/Assets/Edit/5
        public ActionResult Edit(string ano)
        {
            if (ano == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asset asset = db.Assets.Find(ano);
            if (asset == null)
            {
                return HttpNotFound();
            }
            //
            List<SelectListItem> listItem = new List<SelectListItem>();
            db.Departments.ToList().ForEach(d =>
            {
                listItem.Add(new SelectListItem { Text = d.Name_C, Value = d.DptId });
            });
            ViewData["DelivDpt"] = new SelectList(listItem, "Value", "Text", "");
            AppUser u;
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            db.AppUsers.Where(a => a.DptId == asset.DelivDpt).ToList().ForEach(a =>
            {
                listItem2.Add(new SelectListItem { Text = a.FullName, Value = a.Id.ToString() });
            });
            if (listItem2.Where(item => item.Value == asset.DelivUid.ToString()).Count() == 0)
                listItem2.Add(new SelectListItem { Text = asset.DelivEmp, Value = asset.DelivUid.ToString() });
            ViewData["DelivUid"] = new SelectList(listItem2, "Value", "Text", "");

            ViewData["AccDpt"] = new SelectList(listItem, "Value", "Text", "");

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Text = "正常", Value = "正常" });
            listItem3.Add(new SelectListItem { Text = "報廢", Value = "報廢" });
            ViewData["DisposeKind"] = new SelectList(listItem3, "Value", "Text", "");
            //
            List<SelectListItem> listItem4 = new List<SelectListItem>();
            db.DeviceClassCodes.ToList()
                .ForEach(d =>
                {
                    listItem4.Add(new SelectListItem { Text = d.M_name, Value = d.M_code });
                });
            ViewData["BmedNo"] = new SelectList(listItem4, "Value", "Text", "");
            //
            if (asset.VendorId != null)
            {
                Vendor vr = db.Vendors.Where(v => v.VendorId == asset.VendorId).FirstOrDefault();
                if(vr != null)
                    asset.VendorName = vr.VendorName;
            }
            //
            List<SelectListItem> listItem5 = new List<SelectListItem>();
            Roles.GetUsersInRole("MedEngineer").ToList()
                        .ForEach(x =>
                        {
                            u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                listItem5.Add(new SelectListItem { Text = u.FullName, Value = u.Id.ToString() });
                            }
                        });
            ViewData["EngId"] = new SelectList(listItem5, "Value", "Text", "");
            return View(asset);
        }

        // POST: MedEngMgt/Assets/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit(Asset asset)
        {
            if (ModelState.IsValid)
            {
                asset.DelivEmp = asset.DelivUid == null ? "" : db.AppUsers.Find(asset.DelivUid).FullName;
                db.Entry(asset).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
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

        [MyErrorHandler]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Asset asset = db.Assets.Find(id);
            AssetKeep ak = db.AssetKeeps.Find(id);
            if(asset != null)
                db.Assets.Remove(asset);
            if(ak != null)
                db.AssetKeeps.Remove(ak);
            db.SaveChanges();

            //return RedirectToAction("Index");
            return new JsonResult
            {
                Data = new { success = true, error = "" },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // POST: MedEngMgt/Assets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Asset asset = db.Assets.Find(id);
            db.Assets.Remove(asset);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetAssetsByKeyname(string keyname)
        {

            if (!string.IsNullOrEmpty(keyname))
            {
                var result = db.Assets
                    .Where(a => a.DisposeKind != "報廢")
                    .Where(a => a.Cname.Contains(keyname))
                    .Union(db.Assets.Where(a => a.AssetNo == keyname)
                    .Where(a => a.DisposeKind != "報廢"))
                    .Select(a => new
                    {
                        AssetNo = a.AssetNo,
                        Cname = a.Cname + "(" + a.AssetNo + ")"
                    });

                string s = JsonConvert.SerializeObject(result);
                return Json(s, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAssetsByAccDpt(string dpt)
        {

            if (!string.IsNullOrEmpty(dpt))
            {
                var result = db.Assets
                    .Where(a => a.DisposeKind != "報廢")
                    .Where(a => a.AccDpt == dpt)
                    .Select(a => new
                    {
                        AssetNo = a.AssetNo,
                        Cname = a.Cname + "(" + a.AssetNo + ")"
                    });

                string s = JsonConvert.SerializeObject(result);
                return Json(s, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAccDptByAsset(string ano)
        {

            if (!string.IsNullOrEmpty(ano))
            {
                var result = db.Assets
                    .Where(a => a.DisposeKind != "報廢")
                    .Where(a => a.AssetNo == ano)
                    .Join(db.Departments, a => a.AccDpt, d => d.DptId,
                    (a,d) => d)
                    .Select(d => new
                    {
                        dptid = d.DptId,
                        dptname = d.Name_C
                    });

                string s = JsonConvert.SerializeObject(result);
                return Json(s, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult UpdDelivUid(string id, string assets)
        {
            string[] s = assets.Split(new char[] { ';' });
            AppUser u = db.AppUsers.Find(Convert.ToInt32(id));
            Asset at;
            foreach (string ss in s)
            {
                at = db.Assets.Find(ss);
                if (at != null)
                {                    
                    if (u != null)
                    {
                        at.DelivUid = u.Id;
                        at.DelivEmp = u.FullName;
                        if (ModelState.IsValid)
                        {
                            db.Entry(at).State = EntityState.Modified;
                            db.SaveChanges();
                        }
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
