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
using BMEDmgt.Extensions;

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

            List<SelectListItem> listItem3 = new List<SelectListItem>();
            listItem3.Add(new SelectListItem { Value = "", Text = "請選擇" });
            listItem3.Add(new SelectListItem { Value = "0", Text = "無" });
            listItem3.Add(new SelectListItem { Value = "1", Text = "I" });
            listItem3.Add(new SelectListItem { Value = "2", Text = "II" });
            listItem3.Add(new SelectListItem { Value = "3", Text = "III" });
            ViewData["RiskLvl"] = new SelectList(listItem3, "Value", "Text");

            List<SelectListItem> listItem4 = new List<SelectListItem>();
            ViewData["VENDORID"] = new SelectList(listItem4, "Value", "Text");

            List<SelectListItem> listItem5 = new List<SelectListItem>();
            listItem5.Add(new SelectListItem { Value = "", Text = "請選擇" });
            var assetClasses = db.Assets.Where(a => !string.IsNullOrEmpty(a.AssetClass)).Select(a => a.AssetClass).Distinct().ToList();
            foreach(string s in assetClasses)
            {
                listItem5.Add(new SelectListItem { Value = s, Text = s });
            }
            ViewData["AssetClass"] = new SelectList(listItem5, "Value", "Text");

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
            qryAsset.RiskLvl = fm["RiskLvl"];
            qryAsset.VendorId = fm["VendorId"];
            qryAsset.AssetCName2 = fm["AssetCName2"];
            qryAsset.AssetClass = fm["AssetClass"];

            string buyDate1 = fm["BuyDate1"];
            string buyDate2 = fm["BuyDate2"];
            DateTime buyDateFrom = DateTime.Now;
            DateTime buyDateTo = DateTime.Now;
            /* Dealing search by date. */
            if (buyDate1 != "" && buyDate2 != "")// If 2 date inputs have been insert, compare 2 dates.
            {
                DateTime date1 = DateTime.Parse(buyDate1);
                DateTime date2 = DateTime.Parse(buyDate2);
                int result = DateTime.Compare(date1, date2);
                if (result < 0)
                {
                    buyDateFrom = date1.Date;
                    buyDateTo = date2.Date;
                }
                else if (result == 0)
                {
                    buyDateFrom = date1.Date;
                    buyDateTo = date1.Date;
                }
                else
                {
                    buyDateFrom = date2.Date;
                    buyDateTo = date1.Date;
                }
            }
            else if (buyDate1 == "" && buyDate2 != "")
            {
                buyDateFrom = DateTime.Parse(buyDate2);
                buyDateTo = DateTime.Parse(buyDate2);
            }
            else if (buyDate1 != "" && buyDate2 == "")
            {
                buyDateFrom = DateTime.Parse(buyDate1);
                buyDateTo = DateTime.Parse(buyDate1);
            }
            if (string.IsNullOrEmpty(buyDate1) == false || string.IsNullOrEmpty(buyDate2) == false)
            {
                qryAsset.BuyDate1 = buyDateFrom;
                qryAsset.BuyDate2 = buyDateTo;
            }

            TempData["qry"] = qryAsset;
            List<Asset> at = new List<Asset>();
            List<Asset> at2 = new List<Asset>();
            //
            var qAsset = db.Assets.AsQueryable();
            if (!string.IsNullOrEmpty(qryAsset.AssetNo))
            {
                qAsset = qAsset.Where(a => a.AssetNo == qryAsset.AssetNo);
            }
            if (!string.IsNullOrEmpty(qryAsset.AssetName))
            {
                qAsset = qAsset.Where(a => a.Cname.Contains(qryAsset.AssetName));
            }
            if (!string.IsNullOrEmpty(qryAsset.AssetCName2))
            {
                qAsset = qAsset.Where(a => a.Cname2 != null).Where(a => a.Cname2.Contains(qryAsset.AssetCName2));
            }
            if (!string.IsNullOrEmpty(qryAsset.AccDpt))
            {
                qAsset = qAsset.Where(a => a.AccDpt == qryAsset.AccDpt);
            }
            if (!string.IsNullOrEmpty(qryAsset.DelivDpt))
            {
                qAsset = qAsset.Where(a => a.DelivDpt == qryAsset.DelivDpt);
            }
            if (!string.IsNullOrEmpty(qryAsset.Type))
            {
                qAsset = qAsset.Where(a => a.Type == qryAsset.Type);
            }
            if (!string.IsNullOrEmpty(qryAsset.RiskLvl))
            {
                qAsset = qAsset.Where(a => a.RiskLvl == qryAsset.RiskLvl);
            }
            if (!string.IsNullOrEmpty(qryAsset.VendorId))
            {
                var vid = Convert.ToInt32(qryAsset.VendorId);
                qAsset = qAsset.Where(a => a.VendorId == vid);
            }
            if (string.IsNullOrEmpty(buyDate1) == false || string.IsNullOrEmpty(buyDate2) == false)
            {
                qAsset = qAsset.Where(v => v.BuyDate >= buyDateFrom && v.BuyDate <= buyDateTo);
            }
            if (!string.IsNullOrEmpty(qryAsset.AssetClass))
            {
                qAsset = qAsset.Where(a => a.AssetClass == qryAsset.AssetClass);
            }
            //
            try {
                qAsset.GroupJoin(db.Departments, a => a.DelivDpt, d => d.DptId,
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
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            
            //
            List<SelectListItem> listItem = new List<SelectListItem>();
            Roles.GetUsersInRole("MedEngineer").ToList()
                        .ForEach(x =>
                        {
                            AppUser u = db.AppUsers.Find(WebSecurity.GetUserId(x));
                            if (u != null)
                            {
                                listItem.Add(new SelectListItem { Text = "(" + u.UserName + ")" + u.FullName, Value = u.Id.ToString() });
                            }
                        });
            ViewData["KeepEngId"] = new SelectList(listItem, "Value", "Text", "");
            ViewData["AssetEngId"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            if (at2.Count() >= 1)
            {
                string dptid = at2.First().DelivDpt;
                db.AppUsers.Where(u => u.DptId == dptid).ToList()
                    .ForEach(u => {
                        listItem2.Add(new SelectListItem { Text = u.FullName, Value = u.Id.ToString() });
                    });
            }
            ViewData["DelivUid"] = new SelectList(listItem2, "Value", "Text", "");
            //
            if (at2.ToPagedList(page, pageSize).Count <= 0)
                return PartialView("List", at2.ToPagedList(1, pageSize));
            return PartialView("List", at2.ToPagedList(page, pageSize));
        }

        public List<Asset> QryAsset(QryAsset qryAsset)
        {
            DateTime? buyDateFrom = qryAsset.BuyDate1;
            DateTime? buyDateTo = qryAsset.BuyDate2;

            List<Asset> at = new List<Asset>();
            var qAsset = db.Assets.AsQueryable();
            if (!string.IsNullOrEmpty(qryAsset.AssetNo))
            {
                qAsset = qAsset.Where(a => a.AssetNo == qryAsset.AssetNo);
            }
            if (!string.IsNullOrEmpty(qryAsset.AssetName))
            {
                qAsset = qAsset.Where(a => a.Cname.Contains(qryAsset.AssetName));
            }
            if (!string.IsNullOrEmpty(qryAsset.AssetCName2))
            {
                qAsset = qAsset.Where(a => a.Cname2 != null).Where(a => a.Cname2.Contains(qryAsset.AssetCName2));
            }
            if (!string.IsNullOrEmpty(qryAsset.AccDpt))
            {
                qAsset = qAsset.Where(a => a.AccDpt == qryAsset.AccDpt);
            }
            if (!string.IsNullOrEmpty(qryAsset.DelivDpt))
            {
                qAsset = qAsset.Where(a => a.DelivDpt == qryAsset.DelivDpt);
            }
            if (!string.IsNullOrEmpty(qryAsset.Type))
            {
                qAsset = qAsset.Where(a => a.Type == qryAsset.Type);
            }
            if (!string.IsNullOrEmpty(qryAsset.RiskLvl))
            {
                qAsset = qAsset.Where(a => a.RiskLvl == qryAsset.RiskLvl);
            }
            if (!string.IsNullOrEmpty(qryAsset.VendorId))
            {
                var vid = Convert.ToInt32(qryAsset.VendorId);
                qAsset = qAsset.Where(a => a.VendorId == vid);
            }
            if (buyDateFrom.HasValue == true || buyDateTo.HasValue == true)
            {
                qAsset = qAsset.Where(v => v.BuyDate >= buyDateFrom && v.BuyDate <= buyDateTo);
            }
            if (!string.IsNullOrEmpty(qryAsset.AssetClass))
            {
                qAsset = qAsset.Where(a => a.AssetClass == qryAsset.AssetClass);
            }

            qAsset.GroupJoin(db.Departments, a => a.DelivDpt, d => d.DptId,
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
            dt.Columns.Add("廠商名稱");
            dt.Columns.Add("廠商統編");
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
            dt.Columns.Add("保養格式代號");

            List<Asset> mv = QryAsset(v);
            mv.Join(db.AppUsers, a => a.EngId, u => u.Id,
                (a, u) => new {
                    asset = a,
                    user = u
                })
                .GroupJoin(db.AssetKeeps, a => a.asset.AssetNo, k => k.AssetNo,
                (a, k) => new {
                    asset = a.asset,
                    user = a.user,
                    assetkeep = k
                }).SelectMany(a => a.assetkeep.DefaultIfEmpty(), 
                (a, s) => new {
                    asset = a.asset,
                    user = a.user,
                    assetkeep = s
                })
                .GroupJoin(db.Vendors, a => a.asset.VendorId, vd => vd.VendorId,
                (a, vd) => new {
                    asset = a.asset,
                    user = a.user,
                    assetkeep = a.assetkeep,
                    vendor = vd
                }).SelectMany(a => a.vendor.DefaultIfEmpty(),
                (a, s) => new {
                    asset = a.asset,
                    user = a.user,
                    assetkeep = a.assetkeep,
                    vendor = s
                })
                .ToList()
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
                dw[10] = m.vendor == null ? "" : m.vendor.VendorName;
                dw[11] = m.vendor == null ? "" : m.vendor.UniteNo;
                dw[12] = m.asset.MakeNo;
                dw[13] = m.asset.DisposeKind;
                dw[14] = m.asset.Cost;
                dw[15] = m.assetkeep == null ? null : m.assetkeep.Cycle;
                dw[16] = m.assetkeep == null ? null : m.assetkeep.KeepYm;
                dw[17] = m.assetkeep == null ? "" : m.assetkeep.InOut;
                dw[18] = m.asset.WartySt == null ? "" : m.asset.WartySt.Value.ToString("yyyy/MM/dd");
                dw[19] = m.asset.WartyEd == null ? "" : m.asset.WartyEd.Value.ToString("yyyy/MM/dd");
                dw[20] = m.assetkeep == null ? "" : m.assetkeep.KeepEngName;
                dw[21] = m.asset.BuyDate == null?"":m.asset.BuyDate.Value.ToString("yyyy/MM/dd");
                dw[22] = m.assetkeep == null ? "" : m.assetkeep.FormatId;
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
        public ActionResult Details(string ano)
        {
            if (ano == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ano = ano.Trim();
            Asset asset = db.Assets.Find(ano);
            if (asset == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.RequestTimeout);
            }
            if(asset.DelivUid != null)
            {
                asset.DelivEmp = "(" + db.AppUsers.Find(asset.DelivUid.Value).UserName + ") "
                    + asset.DelivEmp;
            }
            if (!string.IsNullOrEmpty(asset.DelivDpt))
            {
                asset.DelivDptName = db.Departments.Find(asset.DelivDpt).Name_C;
            }
            if (!string.IsNullOrEmpty(asset.AccDpt))
            {
                asset.AccDptName = db.Departments.Find(asset.AccDpt).Name_C;
            }
            if (asset.EngId != 0)
            {
                var ur = db.AppUsers.Find(asset.EngId);
                asset.EngName = "(" + ur.UserName + ") " + ur.FullName;
                asset.EngEmail = ur.Email;
                asset.EngTel = ur.Mobile;
            }
            if (asset.VendorId != null)
            {
                var vd = db.Vendors.Find(asset.VendorId);
                asset.VendorName = vd == null ? "" : vd.VendorName;
            }
            else
            {
                asset.VendorName = "";
            }

            return View(asset);
        }

        // GET: MedEngMgt/Assets/DetailsForModal/5
        public ActionResult DetailsForModal(string ano)
        {
            if (ano == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ano = ano.Trim();
            Asset asset = db.Assets.Find(ano);
            if (asset == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.RequestTimeout);
            }
            if (asset.DelivUid != null)
            {
                asset.DelivEmp = "(" + db.AppUsers.Find(asset.DelivUid.Value).UserName + ") "
                    + asset.DelivEmp;
            }
            if (!string.IsNullOrEmpty(asset.DelivDpt))
            {
                asset.DelivDptName = db.Departments.Find(asset.DelivDpt).Name_C;
            }
            if (!string.IsNullOrEmpty(asset.AccDpt))
            {
                asset.AccDptName = db.Departments.Find(asset.AccDpt).Name_C;
            }
            if (asset.EngId != 0)
            {
                var ur = db.AppUsers.Find(asset.EngId);
                asset.EngName = "(" + ur.UserName + ") " + ur.FullName;
                asset.EngEmail = ur.Email;
                asset.EngTel = ur.Mobile;
            }
            if (asset.VendorId != null)
            {
                var vd = db.Vendors.Find(asset.VendorId);
                asset.VendorName = vd == null ? "" : vd.VendorName;
            }
            else
            {
                asset.VendorName = "";
            }

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
                                listItem5.Add(new SelectListItem { Text = "(" + u.UserName + ")" + u.FullName, Value = u.Id.ToString() });
                            }
                        });
            ViewData["EngId"] = new SelectList(listItem5, "Value", "Text", "");

            List<SelectListItem> listItem6 = new List<SelectListItem>();
            listItem6.Add(new SelectListItem { Text = "醫工", Value = "醫工" });
            listItem6.Add(new SelectListItem { Text = "工務", Value = "工務" });
            ViewData["ResponsDpt"] = new SelectList(listItem6, "Value", "Text", "");

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
                    // Save log. 
                    string logClass = "醫療儀器紀錄";
                    string logAction = "資產維護 > 新增設備 > " + asset.AssetNo + "(" + asset.Cname + ")";
                    var result = new SystemLogsController().SaveLog(logClass, logAction);

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
                listItem2.Add(new SelectListItem { Text = "(" + a.UserName + ")" + a.FullName, Value = a.Id.ToString() });
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
                Vendor vr = db.Vendors.Where(v => v.VendorId == asset.VendorId).ToList().FirstOrDefault();
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
                                listItem5.Add(new SelectListItem { Text = "(" + u.UserName + ")" + u.FullName, Value = u.Id.ToString() });
                            }
                        });
            ViewData["EngId"] = new SelectList(listItem5, "Value", "Text", "");

            List<SelectListItem> listItem6 = new List<SelectListItem>();
            listItem6.Add(new SelectListItem { Text = "醫工", Value = "醫工" });
            listItem6.Add(new SelectListItem { Text = "工務", Value = "工務" });
            ViewData["ResponsDpt"] = new SelectList(listItem6, "Value", "Text", "");

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
                Asset oriObj = db.Assets.Find(asset.AssetNo);
                db.Entry(oriObj).State = EntityState.Detached;
                asset.DelivEmp = asset.DelivUid == null ? "" : db.AppUsers.Find(asset.DelivUid).FullName;
                db.Entry(asset).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    // Save log.
                    var currentObj = db.Assets.Find(asset.AssetNo);
                    var logAction2 = oriObj.EnumeratePropertyDifferences<Asset>(currentObj);
                    string logClass = "醫療儀器紀錄";
                    string logAction = "資產維護 > 設備編輯 > " + asset.AssetNo + "(" + asset.Cname + ")" + " ";
                    var result = new SystemLogsController().SaveLog(logClass, logAction, logAction2);

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

        public ActionResult NewEdit(string id)
        {
            Asset asset = db.Assets.Find(id);
            AssetKeep ak = db.AssetKeeps.Find(id);
            if (ak != null)
            {
                asset.KeepYm = ak.KeepYm;
                asset.Cycle = ak.Cycle;
            }
            List<SelectListItem> listItem = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = "正常", Value = "正常" });
            listItem.Add(new SelectListItem { Text = "報廢", Value = "報廢" });
            ViewData["DKIND"] = new SelectList(listItem, "Value", "Text");
            //
            //UserProfile u = db.UserProfiles.Find(asset.DelivUid);
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            List<SelectListItem> delivdpt = new List<SelectListItem>();
            List<SelectListItem> accdpt = new List<SelectListItem>();
            if (asset.DelivUid == null)
            {
                asset.DelivUid = null;
                asset.DelivEmp = "";
                asset.DelivDpt = "";
            }
            listItem2.Add(new SelectListItem { Text = asset.DelivEmp, Value = asset.DelivUid.Value.ToString() });
            List<AppUser> ul;
            string gid = "CCH";
            db.Departments.ToList()
                .ForEach(d => {
                    delivdpt.Add(new SelectListItem
                    {
                        Text = d.Name_C,
                        Value = d.DptId
                    });
                    accdpt.Add(new SelectListItem
                    {
                        Text = d.Name_C,
                        Value = d.DptId
                    });
                });
            ViewData["DelivUids"] = new SelectList(listItem2, "Value", "Text");
            ViewData["DelivDpts"] = new SelectList(delivdpt, "Value", "Text", asset.DelivDpt);
            ViewData["AccDpts"] = new SelectList(accdpt, "Value", "Text", asset.AccDpt);

            return View(asset);
        }

        [HttpPost]
        public ActionResult NewEdit(Asset asset)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(asset.LeaveSite))
                {
                    return Content("[放置地點]不可空白!!");
                }
                if (string.IsNullOrEmpty(asset.RiskLvl))
                {
                    return Content("[風險等級]不可空白!!");
                }
                if (asset.KeepYm == null || asset.Cycle == null)
                {
                    return Content("[保養起始年月]或[保養週期]不可空白!!");
                }
                if (string.IsNullOrEmpty(asset.MakeNo))
                {
                    return Content("[製造號碼]不可空白!!");
                }
                //if (asset.RelDate == null)
                //{
                //    return Content("[製造日期]不可空白!!");
                //}
                asset.Rtp = WebSecurity.CurrentUserId;
                asset.Rtt = DateTime.Now;
                db.Entry(asset).State = EntityState.Modified;
                //
                AssetKeep ak = db.AssetKeeps.Find(asset.AssetNo);
                if (ak == null)
                {
                    ak = new AssetKeep();
                    ak.AssetNo = asset.AssetNo;
                    ak.KeepYm = asset.KeepYm;
                    ak.Cycle = asset.Cycle;
                    db.AssetKeeps.Add(ak);
                }
                else
                {
                    ak.KeepYm = asset.KeepYm;
                    ak.Cycle = asset.Cycle;
                    db.Entry(ak).State = EntityState.Modified;
                }
                try
                {
                    db.SaveChanges();
                    return Content("success");
                }
                catch (Exception e)
                {
                    return Content(e.Message);
                }
            }
            else
            {
                string msg = "";
                foreach (var error in ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors))
                {
                    msg += error.ErrorMessage + Environment.NewLine;
                }
                return Content(msg);
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
            // Save log. 
            string logClass = "醫療儀器紀錄";
            string logAction = "資產維護 > 刪除設備 > " + asset.AssetNo + "(" + asset.Cname + ")";
            var result = new SystemLogsController().SaveLog(logClass, logAction);

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

        // GET: /Asset/Delete2/5

        public ActionResult Delete2(string id = null)
        {
            Asset asset = db.Assets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        //
        // POST: /Asset/Delete/5

        [HttpPost]
        public JavaScriptResult DeleteById(string id)
        {
            Asset asset = db.Assets.Find(id);
            db.Assets.Remove(asset);
            AssetKeep keep = db.AssetKeeps.Find(id);
            db.AssetKeeps.Remove(keep);
            db.SaveChanges();

            return JavaScript("alert('刪除成功!');window.opener.location.reload();close();");
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

        public JsonResult GetAssetsByKeynameAndAcc(string keyname, string accDpt = null)
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
                        Cname = a.Cname + "(" + a.AssetNo + ")",
                        AccDpt = a.AccDpt
                    }).ToList();
                if (!string.IsNullOrEmpty(accDpt))
                {
                    result = result.Where(r => r.AccDpt == accDpt || r.AssetNo == "000" || r.AssetNo == "001").ToList();
                }

                string s = JsonConvert.SerializeObject(result);
                return Json(s, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAssetById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var result = db.Assets
                    .Where(a => a.DisposeKind != "報廢")
                    .Where(a => a.AssetNo == id).ToList().FirstOrDefault();

                return Json(result, JsonRequestBehavior.AllowGet);
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
                    }).ToList();

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
                    }).ToList();

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

        [HttpPost]
        [MyErrorHandler]
        public ActionResult UpdEngineer(string id, string assets)
        {
            string[] s = assets.Split(new char[] { ';' });
            Asset asset;
            foreach (string ss in s)
            {
                asset = db.Assets.Find(ss);
                if (asset != null)
                {
                    AppUser u = db.AppUsers.Find(Convert.ToInt32(id));
                    if (u != null)
                    {
                        asset.EngId = u.Id;
                        asset.EngName = u.FullName;
                        db.Entry(asset).State = EntityState.Modified;
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

        public ActionResult BuyEvaluateListItem(string id = null, string upload = null, int page = 1)
        {
            List<Asset> at = new List<Asset>();
            if (upload == "採購人員")
            {
                Delivery dd = db.Deliveries.Find(id);
                if (dd != null)
                    at = db.Assets.Where(a => a.Docid == dd.Docid).ToList();
            }
            else
                at = db.Assets.Where(a => a.Docid == id).ToList();
            foreach (Asset e in at)
            {
                e.upload = upload;
            }
            if (upload == "查看")
                return PartialView("_BuyEvaluateListItem2", at);
            if (at.ToPagedList(page, pageSize).Count <= 0)
                return PartialView("_BuyEvaluateListItem", at.ToPagedList(1, pageSize));
            return PartialView("_BuyEvaluateListItem", at.ToPagedList(page, pageSize));
        }

        public ActionResult New(string docid, DateTime rdate)
        {
            Asset asset = new Asset();
            asset.AccDate = DateTime.Now;
            asset.BuyDate = DateTime.Now;
            asset.DisposeKind = "正常";
            if (docid != null)
            {
                Delivery dy = db.Deliveries.Find(docid);
                //BuyEvaluate d = db.BuyEvaluates.Find(dy.PurchaseNo);
                //if (d != null)
                //{
                //    asset.Cname = d.PlantCnam;
                //    asset.Ename = d.PlantEnam;
                //}
                if (dy != null)
                {
                    asset.AccDpt = dy.AccDpt;
                    int vendorId = Convert.ToInt32(dy.VendorId);
                    Vendor vr = db.Vendors.Where(v => v.VendorId == vendorId).ToList().FirstOrDefault();
                    asset.VendorId = vr == null ? 0 : vr.VendorId;
                }
                asset.Docid = docid;
            }
            //asset.DelivUid = 34;
            //asset.DelivEmp = "張三";
            //asset.DelivDpt = "8420";
            asset.KeepYm = (rdate.Year - 1911) * 100 + rdate.Month;
            AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
            if (u != null)
            {
                asset.DelivUid = u.Id;
                asset.DelivEmp = u.FullName;
                asset.DelivDpt = u.DptId;
                
                if (u.DptId != null)
                {
                    asset.AccDpt = u.DptId;
                    asset.DelivDpt = u.DptId;
                    Department co = db.Departments.Find(u.DptId);
                    if (co != null)
                        asset.LeaveSite = co.Name_C;
                }
            }
            //
            List<SelectListItem> listItem = new List<SelectListItem>();
            List<SelectListItem> delivdpt = new List<SelectListItem>();
            List<SelectListItem> accdpt = new List<SelectListItem>();
            listItem.Add(new SelectListItem { Text = asset.DelivEmp, Value = asset.DelivUid.Value.ToString() });
            List<AppUser> ul;
            string gid = "CCH";
            db.Departments.ToList()
                .ForEach(o => {
                    delivdpt.Add(new SelectListItem
                    {
                        Text = o.Name_C,
                        Value = o.DptId
                    });
                    accdpt.Add(new SelectListItem
                    {
                        Text = o.Name_C,
                        Value = o.DptId
                    });
                });
            ViewData["DelivUids"] = new SelectList(listItem, "Value", "Text");
            ViewData["DelivDpts"] = new SelectList(delivdpt, "Value", "Text", asset.DelivDpt);
            ViewData["AccDpts"] = new SelectList(accdpt, "Value", "Text", asset.AccDpt);

            return PartialView(asset);
        }

        [HttpPost]
        public ActionResult New(Asset asset)
        {
            if (ModelState.IsValid)
            {
                if (asset.AssetNo.Contains(";") || asset.AssetNo.Contains("、") || asset.AssetNo.Contains(","))
                {
                    return Content("財產編號只能有一個，請重新輸入!!");
                }
                if (db.Assets.Where(a => a.AssetNo == asset.AssetNo).Count() > 0)
                {
                    return Content("新增失敗，此財產編號已經存在!!");
                    //return View(asset);
                }
                if (string.IsNullOrEmpty(asset.LeaveSite))
                {
                    return Content("[放置地點]不可空白!!");
                }
                if (string.IsNullOrEmpty(asset.RiskLvl))
                {
                    return Content("[風險等級]不可空白!!");
                }
                if (asset.KeepYm == null || asset.Cycle == null)
                {
                    return Content("[保養起始年月]或[保養週期]不可空白!!");
                }
                if (string.IsNullOrEmpty(asset.MakeNo))
                {
                    return Content("[製造號碼]不可空白!!");
                }
                //if (asset.RelDate == null)
                //{
                //    return Content("[製造日期]不可空白!!");
                //}
                if (string.IsNullOrEmpty(asset.DelivDpt))
                {
                    return Content("[保管部門]不可空白!!");
                }
                if (string.IsNullOrEmpty(asset.AccDpt))
                {
                    return Content("[成本中心]不可空白!!");
                }
                asset.Rtp = WebSecurity.CurrentUserId;
                asset.Rtt = DateTime.Now;
                db.Assets.Add(asset);
                AssetKeep ak = db.AssetKeeps.Find(asset.AssetNo);
                if (ak == null)
                {
                    ak = new AssetKeep();
                    ak.AssetNo = asset.AssetNo;
                    ak.KeepYm = asset.KeepYm;
                    ak.Cycle = asset.Cycle;
                    db.AssetKeeps.Add(ak);
                }
                else
                {
                    ak.KeepYm = asset.KeepYm;
                    ak.Cycle = asset.Cycle;
                    db.Entry(ak).State = EntityState.Modified;
                }
                try
                {
                    db.SaveChanges();
                    return Content("success");
                }
                catch (Exception e)
                {
                    return Content(e.Message);
                }
            }
            else
            {
                string msg = "";
                foreach (var error in ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors))
                {
                    msg += error.ErrorMessage + Environment.NewLine;
                }
                return Content(msg);
            }
        }

        [MyErrorHandler]
        [HttpPost]
        public ActionResult ImportAssetsByExcel()
        {
            HttpPostedFileBase uploadFile = Request.Files[0] as HttpPostedFileBase;
            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                var extension = Path.GetExtension(uploadFile.FileName);
                var excelFile = Path.Combine(Server.MapPath("~/Files/"), "匯入設備Excel" + extension);
                if (System.IO.File.Exists(excelFile))
                {
                    System.IO.File.Delete(excelFile);
                }
                // User上傳未知檔案格式時，瀏覽器傳入的ContentType一律會為application/octet-stream
                if (uploadFile.ContentType == "application/vnd.ms-excel" || uploadFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    uploadFile.SaveAs(excelFile);//WORKS FINE
                                           //BEGINING OF IMPORT
                    FileInfo eFile = new FileInfo(excelFile);
                    using (var excelPackage = new ExcelPackage(eFile))
                    {
                        if (!eFile.Name.EndsWith("xlsx"))//Return ModelState.AddModelError()
                        {
                            throw new Exception("不相容的Excel格式，請使用2007或更新的版本。");
                        }
                        else
                        {
                            var worksheet = excelPackage.Workbook.Worksheets[1];
                            if (worksheet == null)
                            {
                                throw new Exception("錯誤的Excel格式!");
                            }
                            else
                            {
                                var lastRow = worksheet.Dimension.End.Row;
                                for (var row = 2; row <= lastRow; row++)
                                {                        
                                    if (string.IsNullOrEmpty(worksheet.Cells[row, 1].Value.ToString()))
                                    {
                                        throw new Exception("匯入失敗於第" + row + "行資料，【財產編號】未填寫!");
                                    }
                                    if (string.IsNullOrEmpty(worksheet.Cells[row, 3].Value.ToString()))
                                    {
                                        throw new Exception("匯入失敗於第" + row + "行資料，【設備類別】未填寫!");
                                    }
                                    if (string.IsNullOrEmpty(worksheet.Cells[row, 4].Value.ToString()))
                                    {
                                        throw new Exception("匯入失敗於第" + row + "行資料，【中文名稱】未填寫!");
                                    }
                                    if (string.IsNullOrEmpty(worksheet.Cells[row, 17].Value.ToString()))
                                    {
                                        throw new Exception("匯入失敗於第" + row + "行資料，【處分性質】未填寫!");
                                    }
                                    if (string.IsNullOrEmpty(worksheet.Cells[row, 18].Value.ToString()))
                                    {
                                        throw new Exception("匯入失敗於第" + row + "行資料，【保管部門】未填寫!");
                                    }
                                    if (string.IsNullOrEmpty(worksheet.Cells[row, 19].Value.ToString()))
                                    {
                                        throw new Exception("匯入失敗於第" + row + "行資料，【保管人代號】未填寫!");
                                    }
                                    if (string.IsNullOrEmpty(worksheet.Cells[row, 21].Value.ToString()))
                                    {
                                        throw new Exception("匯入失敗於第" + row + "行資料，【工程師代號】未填寫!");
                                    }

                                    var delivUname = worksheet.Cells[row, 19].Value.ToString();
                                    var engUname = worksheet.Cells[row, 21].Value.ToString();
                                    try
                                    {
                                        var lastColumn = worksheet.Dimension.End.Column;
                                        var asset = new Asset
                                        {
                                            AssetNo = worksheet.Cells[row, 1].Value.ToString(),
                                            BmedNo = worksheet.Cells[row, 2].Value.ToString(),
                                            AssetClass = worksheet.Cells[row, 3].Value.ToString(),
                                            Cname = worksheet.Cells[row, 4].Value.ToString(),
                                            Cname2 = worksheet.Cells[row, 5].Value.ToString(),
                                            Ename = worksheet.Cells[row, 6].Value.ToString(),
                                            AccDate = DateTime.Parse(worksheet.Cells[row, 7].Value.ToString()),
                                            BuyDate = DateTime.Parse(worksheet.Cells[row, 8].Value.ToString()),
                                            //RelDate = DateTime.Parse(worksheet.Cells[row, 9].Value.ToString()),
                                            Brand = worksheet.Cells[row, 10].Value.ToString(),
                                            Standard = worksheet.Cells[row, 11].Value.ToString(),
                                            Type = worksheet.Cells[row, 12].Value.ToString(),
                                            Origin = worksheet.Cells[row, 13].Value.ToString(),
                                            Voltage = worksheet.Cells[row, 14].Value.ToString(),
                                            Current = worksheet.Cells[row, 15].Value.ToString(),
                                            DisposeKind = worksheet.Cells[row, 17].Value.ToString(),
                                            DelivDpt = worksheet.Cells[row, 18].Value.ToString(),
                                            DelivUid = db.AppUsers.Where(u => u.UserName == delivUname).First().Id,
                                            DelivEmp = worksheet.Cells[row, 20].Value.ToString(),
                                            EngId = db.AppUsers.Where(u => u.UserName == engUname).First().Id,
                                            LeaveSite = worksheet.Cells[row, 22].Value.ToString(),
                                            AccDpt = worksheet.Cells[row, 23].Value.ToString(),
                                            Cost = Convert.ToDecimal(worksheet.Cells[row, 24].Value),
                                            RiskLvl = worksheet.Cells[row, 25].Value.ToString(),
                                            MakeNo = worksheet.Cells[row, 27].Value.ToString(),
                                            Note = worksheet.Cells[row, 28].Value.ToString(),
                                            WartySt = DateTime.Parse(worksheet.Cells[row, 29].Value.ToString()),
                                            WartyEd = DateTime.Parse(worksheet.Cells[row, 30].Value.ToString()),
                                            Docid = worksheet.Cells[row, 31].Value.ToString(),
                                            ResponsDpt = worksheet.Cells[row, 32].Value.ToString(),
                                            Rtt = DateTime.Now,
                                            Rtp = WebSecurity.CurrentUserId
                                        };
                                        for (var column = 1; column <= lastColumn; column++)
                                        {
                                            if (worksheet.Cells[row, column].Value == null ||
                                                worksheet.Cells[row, column].Value == DBNull.Value ||
                                                String.IsNullOrWhiteSpace(worksheet.Cells[row, column].Value.ToString()))
                                            {
                                                worksheet.Cells[row, column].Value = null;
                                            }
                                            else if (worksheet.Cells[row, column].Value.ToString() == "NULL")
                                            {
                                                worksheet.Cells[row, column].Value = null;
                                            }
                                        }
                                        if (worksheet.Cells[row, 16].Value == null ||
                                            worksheet.Cells[row, 16].Value == DBNull.Value ||
                                            String.IsNullOrWhiteSpace(worksheet.Cells[row, 16].Value.ToString()))
                                        {
                                            asset.VendorId = null;
                                        }
                                        else if (worksheet.Cells[row, 16].Value.ToString() == "NULL")
                                        {
                                            asset.VendorId = null;
                                        }
                                        if (worksheet.Cells[row, 26].Value == null ||
                                            worksheet.Cells[row, 26].Value == DBNull.Value ||
                                            String.IsNullOrWhiteSpace(worksheet.Cells[row, 26].Value.ToString()))
                                        {
                                            asset.UseLife = null;
                                        }
                                        else if (worksheet.Cells[row, 26].Value.ToString() == "NULL")
                                        {
                                            asset.UseLife = null;
                                        }

                                        db.Assets.Add(asset);
                                    }
                                    catch(Exception e)
                                    {
                                        throw new Exception(e.Message);
                                    }
                                    
                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception) { throw new Exception("第" + row + "行資料匯入失敗!"); }
                                }
                            }

                        }

                    }//END OF IMPORT
                    return new JsonResult
                    {
                        Data = new { success = true, error = "" },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
                throw new Exception("錯誤: 資料未上傳成功，請確認上傳的資料格式是否正確。");
            }
            else
            {
                throw new Exception("錯誤: 資料未上傳成功，請確認上傳的資料是否正確。");
            }
        }

        public ActionResult GetAssetWarranty(string assetNo)
        {
            string returnString = "";
            AssetPurchaseContract purchaseContract = null;
            AssetMaintainContract maintainContract = null;
            if (!string.IsNullOrEmpty(assetNo))
            {
                var asset = db.Assets.Find(assetNo);
                var assetKeep = db.AssetKeeps.Find(assetNo);
                if (asset != null)
                {
                    // 依照Asset Table內合約編號查詢新購合約
                    if (!string.IsNullOrEmpty(asset.ContractNo))
                        purchaseContract = db.AssetPurchaseContracts.Find(asset.ContractNo);
                    if (purchaseContract != null)
                    {
                        //設備是否於保固日內
                        bool todayIsInWarranty = false;
                        if (purchaseContract.WarrantySdate <= DateTime.Now &&
                            purchaseContract.WarrantyEdate.Value.AddDays(1).AddSeconds(-1) >= DateTime.Now)
                        {
                            todayIsInWarranty = true;
                        }
                        if (todayIsInWarranty == true)
                            returnString = "是";
                        else
                            returnString = "否";
                    }
                }
                if (assetKeep != null)
                {
                    // 依照AssetKeep Table內合約編號查詢維護合約
                    if (!string.IsNullOrEmpty(asset.ContractNo))
                        maintainContract = db.AssetMaintainContracts.Find(assetKeep.ContractNo);
                    if (maintainContract != null)
                        returnString += "(" + maintainContract.ContractType + ")";
                }
            }

            return new JsonResult
            {
                Data = new { success = true, error = "", data = returnString },
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
