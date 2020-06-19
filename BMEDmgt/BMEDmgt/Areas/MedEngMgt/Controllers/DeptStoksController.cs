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
using PagedList;
using WebMatrix.WebData;
using BMEDmgt.Filters;
using System.IO;
using OfficeOpenXml;
using System.Drawing;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class DeptStoksController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 100;
        // GET: MedEngMgt/DeptStoks
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection fm, int page = 1)
        {
            string stokno = fm["qtySTOKNO"];
            string dname = fm["qtyDEPTNAME"];
            string brand = fm["qtyBRAND"];
            string stockClass = fm["qtySTOCKCLASS"];
            List<DeptStok> dv = db.DeptStoks.ToList();
            if (!string.IsNullOrEmpty(stokno))
            {
                dv = dv.Where(d => d.StokNo.Contains(stokno)).ToList();
            }
            if (!string.IsNullOrEmpty(dname))
            {
                dv = dv.Where(d => d.StokNam.Contains(dname)).ToList();
            }
            if (!string.IsNullOrEmpty(brand))
            {
                dv = dv.Where(d => d.Brand == brand.ToUpper()).ToList();
            }
            if (!string.IsNullOrEmpty(stockClass))
            {
                dv = dv.Where(d => !string.IsNullOrEmpty(d.StokCls))
                       .Where(d => d.StokCls.Contains(stockClass)).ToList();
            }
            if (dv.ToPagedList(page, pageSize).Count <= 0)
                return PartialView("List", dv.ToPagedList(1, pageSize));

            return PartialView("List", dv.ToPagedList(page, pageSize));
        }

        // GET: MedEngMgt/DeptStoks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeptStok deptStok = db.DeptStoks.Find(id);
            if (deptStok == null)
            {
                return HttpNotFound();
            }
            return View(deptStok);
        }

        // GET: MedEngMgt/DeptStoks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/DeptStoks/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DeptStok deptStok)
        {
            if (ModelState.IsValid)
            {
                deptStok.Rtp = WebSecurity.CurrentUserId;
                deptStok.Rtt = DateTime.Now;
                db.DeptStoks.Add(deptStok);
                db.SaveChanges();
                // Save log. 
                SystemLog log = new SystemLog();
                log.LogClass = "醫療儀器紀錄";
                log.LogTime = DateTime.UtcNow.AddHours(8);
                log.UserId = WebSecurity.CurrentUserId;
                log.Action = "庫存管理 > 新增 > " + deptStok.StokNam + "(" + deptStok.StokNo + ")";
                db.SystemLogs.Add(log);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(deptStok);
        }

        public ActionResult InStock()
        {

            return View();
        }
        [HttpPost]
        public ActionResult InStock(StokRecord stokRecord)
        {
            if (ModelState.IsValid)
            {
                DeptStok sk = db.DeptStoks.Where(d => d.StokNo == stokRecord.StokNo).ToList().FirstOrDefault();
                if (sk != null)
                {
                    if (stokRecord.InOut == "進庫")
                        sk.Qty += 1;
                    else
                        sk.Qty -= 1;
                    db.Entry(sk).State = EntityState.Modified;
                }
                stokRecord.Rtp = WebSecurity.CurrentUserId;
                stokRecord.Rtt = DateTime.Now;
                db.StokRecords.Add(stokRecord);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex);
                    return View(stokRecord);
                }

                return RedirectToAction("Index");
            }

            return View(stokRecord);
        }
        // GET: MedEngMgt/DeptStoks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeptStok deptStok = db.DeptStoks.Find(id);
            if (deptStok == null)
            {
                return HttpNotFound();
            }
            return View(deptStok);
        }

        // POST: MedEngMgt/DeptStoks/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [MyErrorHandler]
        public ActionResult Edit(DeptStok deptStok)
        {
            if (ModelState.IsValid)
            {
                deptStok.Rtp = WebSecurity.CurrentUserId;
                deptStok.Rtt = DateTime.Now;
                db.Entry(deptStok).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(deptStok);
        }

        // GET: MedEngMgt/DeptStoks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeptStok deptStok = db.DeptStoks.Find(id);
            if (deptStok == null)
            {
                return HttpNotFound();
            }
            return View(deptStok);
        }

        // POST: MedEngMgt/DeptStoks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DeptStok deptStok = db.DeptStoks.Find(id);
            db.DeptStoks.Remove(deptStok);
            db.SaveChanges();
            // Save log. 
            SystemLog log = new SystemLog();
            log.LogClass = "醫療儀器紀錄";
            log.LogTime = DateTime.UtcNow.AddHours(8);
            log.UserId = WebSecurity.CurrentUserId;
            log.Action = "庫存管理 > 刪除 > " + deptStok.StokNam + "(" + deptStok.StokNo + ")";
            db.SystemLogs.Add(log);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Choose()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Choose(FormCollection fc)
        {
            string cname = fc["qtyCNAME"];
            string stokid = fc["qtySTOKID"];
            List<DeptStok> dv = db.DeptStoks.Where(d => d.Status == "Y").ToList();
            if (!string.IsNullOrEmpty(cname))
            {
                dv = dv.Where(d => d.StokNam.Contains(cname)).ToList();
            }
            if (!string.IsNullOrEmpty(stokid))
            {
                dv = dv.Where(d => d.StokNo == stokid).ToList();
            }
            return PartialView("SelectList", dv);
        }

        public ActionResult List()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ExportToExcel(FormCollection fm)
        {
            string stokno = fm["qtySTOKNO"];
            string dname = fm["qtyDEPTNAME"];
            string brand = fm["qtyBRAND"];
            string stockClass = fm["qtySTOCKCLASS"];
            List<DeptStok> dv = db.DeptStoks.ToList();
            if (!string.IsNullOrEmpty(stokno))
            {
                dv = dv.Where(d => d.StokNo.Contains(stokno)).ToList();
            }
            if (!string.IsNullOrEmpty(dname))
            {
                dv = dv.Where(d => d.StokNam.Contains(dname)).ToList();
            }
            if (!string.IsNullOrEmpty(brand))
            {
                dv = dv.Where(d => d.Brand == brand.ToUpper()).ToList();
            }
            if (!string.IsNullOrEmpty(stockClass))
            {
                dv = dv.Where(d => !string.IsNullOrEmpty(d.StokCls))
                       .Where(d => d.StokCls.Contains(stockClass)).ToList();
            }

            string fileName = "";
            MemoryStream stream = new MemoryStream();
            ExcelPackage package = new ExcelPackage(stream);

            package.Workbook.Worksheets.Add("庫存列表");
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];

            #region write header
            sheet.Cells[1, 1].Value = "庫存類別";
            sheet.Cells[1, 2].Value = "材料編號";
            sheet.Cells[1, 3].Value = "材料名稱";
            sheet.Cells[1, 4].Value = "單位";
            sheet.Cells[1, 5].Value = "單價";
            sheet.Cells[1, 6].Value = "數量";
            sheet.Cells[1, 7].Value = "安全存量";
            sheet.Cells[1, 8].Value = "規格";
            sheet.Cells[1, 9].Value = "零件廠牌";

            #endregion

            #region write content
            int pos = 2;
            foreach (var item in dv)
            {
                sheet.Cells[pos, 1].Value = item.StokCls;
                sheet.Cells[pos, 2].Value = item.StokNo;
                sheet.Cells[pos, 3].Value = item.StokNam;
                sheet.Cells[pos, 4].Value = item.Unite;
                sheet.Cells[pos, 5].Value = item.Price;
                sheet.Cells[pos, 6].Value = item.Qty;
                sheet.Cells[pos, 7].Value = item.SafeQty;
                sheet.Cells[pos, 8].Value = item.Standard;
                sheet.Cells[pos, 9].Value = item.Brand;
                pos++;
            }
            #endregion

            // Generate a new unique identifier against which the file can be stored
            string handle = Guid.NewGuid().ToString();

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "庫存列表_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            }
            //因為是用Query的方式,這個地方要用串流的方式來存檔
            using (MemoryStream memoryStream = new MemoryStream())
            {
                package.SaveAs(memoryStream);
                //請注意 一定要加入這行,不然Excel會是空檔
                memoryStream.Seek(0, SeekOrigin.Begin);

                TempData[handle] = memoryStream.ToArray();
                // Note we are returning a filename as well as the handle
                return new JsonResult()
                {
                    Data = new { FileGuid = handle, FileName = fileName }
                };
            }
        }
        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                byte[] data = TempData[fileGuid] as byte[];
                return File(data, "application/vnd.ms-excel", fileName);
            }
            else
            {
                // Problem - Log the error, generate a blank file,
                //           redirect to another controller action - whatever fits with your application
                return new EmptyResult();
            }
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
