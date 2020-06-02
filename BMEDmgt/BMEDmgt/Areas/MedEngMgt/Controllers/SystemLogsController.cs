using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;
using OfficeOpenXml;
using PagedList;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class SystemLogsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 100;

        // GET: MedEngMgt/SystemLogs
        public ActionResult Index(int page = 1)
        {
            if (User.IsInRole("Admin") == true)
            {
                // Save log. 
                SystemLog log = new SystemLog();
                log.LogClass = "系統管理者紀錄";
                log.LogTime = DateTime.UtcNow.AddHours(8);
                log.UserId = WebSecurity.CurrentUserId;
                log.Action = "系統訊息紀錄";
                db.SystemLogs.Add(log);
                db.SaveChanges();
            }

            var systemLogs = db.SystemLogs.ToList();
            foreach(var item in systemLogs)
            {
                if (item.UserId != null)
                {
                    var user = db.AppUsers.Where(u => u.Id == item.UserId.Value).ToList().FirstOrDefault();
                    item.UserName = user.UserName;
                    item.FullName = user.FullName;
                }
            }

            systemLogs = systemLogs.OrderByDescending(s => s.LogTime).ToList();
            return View(systemLogs.ToPagedList(page, pageSize));
        }

        // GET: MedEngMgt/SystemLogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = db.SystemLogs.Find(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // GET: MedEngMgt/SystemLogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MedEngMgt/SystemLogs/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,LogClass,LogTime,UserId,Action")] SystemLog systemLog)
        {
            if (ModelState.IsValid)
            {
                db.SystemLogs.Add(systemLog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(systemLog);
        }

        // GET: MedEngMgt/SystemLogs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = db.SystemLogs.Find(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // POST: MedEngMgt/SystemLogs/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LogClass,LogTime,UserId,Action")] SystemLog systemLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(systemLog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(systemLog);
        }

        // GET: MedEngMgt/SystemLogs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemLog systemLog = db.SystemLogs.Find(id);
            if (systemLog == null)
            {
                return HttpNotFound();
            }
            return View(systemLog);
        }

        // POST: MedEngMgt/SystemLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SystemLog systemLog = db.SystemLogs.Find(id);
            db.SystemLogs.Remove(systemLog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: MedEngMgt/SystemLogs/ExportToExcel
        public ActionResult ExportToExcel()
        {
            string fileName = "";

            MemoryStream stream = new MemoryStream();
            ExcelPackage package = new ExcelPackage(stream);

            package.Workbook.Worksheets.Add("系統訊息紀錄");
            ExcelWorksheet sheet = package.Workbook.Worksheets[1];

            #region write header
            sheet.Cells[1, 1].Value = "訊息類別";
            sheet.Cells[1, 2].Value = "紀錄時間";
            sheet.Cells[1, 3].Value = "人員代號";
            sheet.Cells[1, 4].Value = "執行動作";


            using (ExcelRange range = sheet.Cells[1, 1, 1, 30])
            {

            }
            #endregion

            #region write content
            var SystemLogList = db.SystemLogs.OrderByDescending(s => s.LogTime).ToList();

            int pos = 2;
            foreach (var item in SystemLogList)
            {
                sheet.Cells[pos, 1].Value = item.LogClass;
                sheet.Cells[pos, 2].Value = item.LogTime.ToString("yyyy/MM/dd");
                var ur = db.AppUsers.Where(u => u.Id == item.UserId).ToList().FirstOrDefault();
                sheet.Cells[pos, 3].Value = ur == null ? "" : ur.UserName;
                sheet.Cells[pos, 4].Value = item.Action;

                using (ExcelRange range = sheet.Cells[pos, 1, pos, 30])
                {
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                }
                pos++;
            }
            #endregion

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "系統訊息紀錄列表_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
            }
            //因為是用Query的方式,這個地方要用串流的方式來存檔
            using (MemoryStream memoryStream = new MemoryStream())
            {
                package.SaveAs(memoryStream);
                //請注意 一定要加入這行,不然Excel會是空檔
                memoryStream.Seek(0, SeekOrigin.Begin);
                //注意Excel的ContentType,是要用這個"application/vnd.ms-excel"
                return this.File(memoryStream.ToArray(), "application/vnd.ms-excel", fileName);
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
