using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: MedEngMgt/News
        public ActionResult Index()
        {
            List<News> newslist = new List<News>();
            db.News.Join(db.AppUsers, n => n.UserId, u => u.Id,
            (n, u) => new
            {
                news = n,
                user = u
            }).ToList()
            .ForEach(n =>
                newslist.Add(new News
                {

                    NewsId = n.news.NewsId,
                    NewsClass = n.news.NewsClass,
                    NewsTitle = n.news.NewsTitle,
                    NewsContent = n.news.NewsContent,
                    UserId = n.news.UserId,
                    UserName = n.user.FullName,
                    Sdate = n.news.Sdate,
                    Edate = n.news.Edate,
                    Status = n.news.Status,
                    RTT = n.news.RTT
                })
            );

            if (User.IsInRole("Admin") == true)
            {
                // Save log. 
                SystemLog log = new SystemLog();
                log.LogClass = "系統管理者紀錄";
                log.LogTime = DateTime.UtcNow.AddHours(8);
                log.UserId = WebSecurity.CurrentUserId;
                log.Action = "最新消息維護";
                db.SystemLogs.Add(log);
                db.SaveChanges();
            }

            return View(newslist);
        }

        public ActionResult ShowNews()
        {
            List<News> newslist = new List<News>();
            DateTime dt = DateTime.Now;
            db.News.Where(n => n.Status == "Y")
                .Where(n => n.NewsClass == "最新消息")
                .Where(n => n.Sdate <= dt && n.Edate >= dt)
                .Join(db.AppUsers, n => n.UserId, u => u.Id,
            (n, u) => new
            {
                news = n,
                user = u
            }).ToList()
            .ForEach(n =>
                newslist.Add(new News
                {

                    NewsId = n.news.NewsId,
                    NewsClass = n.news.NewsClass,
                    NewsTitle = n.news.NewsTitle,
                    NewsContent = n.news.NewsContent,
                    UserId = n.news.UserId,
                    UserName = n.user.FullName,
                    Sdate = n.news.Sdate,
                    Edate = n.news.Edate,
                    Status = n.news.Status,
                    RTT = n.news.RTT
                })
            );

            return PartialView(newslist);
        }

        public ActionResult ShowWarnings()
        {
            List<News> newslist = new List<News>();
            DateTime dt = DateTime.Now;
            db.News.Where(n => n.Status == "Y")
                .Where(n => n.NewsClass == "警報訊息")
                .Where(n => n.Sdate <= dt && n.Edate >= dt)
                .Join(db.AppUsers, n => n.UserId, u => u.Id,
            (n, u) => new
            {
                news = n,
                user = u
            }).ToList()
            .ForEach(n =>
                newslist.Add(new News
                {

                    NewsId = n.news.NewsId,
                    NewsClass = n.news.NewsClass,
                    NewsTitle = n.news.NewsTitle,
                    NewsContent = n.news.NewsContent,
                    UserId = n.news.UserId,
                    UserName = n.user.FullName,
                    Sdate = n.news.Sdate,
                    Edate = n.news.Edate,
                    Status = n.news.Status,
                    RTT = n.news.RTT
                })
            );

            return PartialView(newslist);
        }

        // GET: MedEngMgt/News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: MedEngMgt/News/Create
        public ActionResult Create()
        {
            News news = new News();
            news.Status = "Y";
            news.UserId = WebSecurity.CurrentUserId;
            List<SelectListItem> listItem1 = new List<SelectListItem>();
            listItem1.Add(new SelectListItem { Value = "最新消息", Text = "最新消息" });
            listItem1.Add(new SelectListItem { Value = "警報訊息", Text = "警報訊息" });
            ViewData["NewsClass"] = new SelectList(listItem1, "Value", "Text", "");
            return View(news);
        }

        // POST: MedEngMgt/News/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(News news)
        {
            if (ModelState.IsValid)
            {
                news.RTT = DateTime.Now;
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<SelectListItem> listItem1 = new List<SelectListItem>();
            listItem1.Add(new SelectListItem { Value = "最新消息", Text = "最新消息" });
            listItem1.Add(new SelectListItem { Value = "警報訊息", Text = "警報訊息" });
            ViewData["NewsClass"] = new SelectList(listItem1, "Value", "Text", "");
            return View(news);
        }

        // GET: MedEngMgt/News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            news.UserName = db.AppUsers.Find(news.UserId).FullName;
            List<SelectListItem> listItem1 = new List<SelectListItem>();
            listItem1.Add(new SelectListItem { Value = "最新消息", Text = "最新消息" });
            listItem1.Add(new SelectListItem { Value = "警報訊息", Text = "警報訊息" });
            ViewData["NewsClass"] = new SelectList(listItem1, "Value", "Text", news.NewsClass);
            return View(news);
        }

        // POST: MedEngMgt/News/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewsClass,NewsId,NewsTitle,NewsContent,Sdate,Edate,UserId,Status,RTT")] News news)
        {
            if (ModelState.IsValid)
            {
                news.RTT = DateTime.Now;
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<SelectListItem> listItem1 = new List<SelectListItem>();
            listItem1.Add(new SelectListItem { Value = "最新消息", Text = "最新消息" });
            listItem1.Add(new SelectListItem { Value = "警報訊息", Text = "警報訊息" });
            ViewData["NewsClass"] = new SelectList(listItem1, "Value", "Text", news.NewsClass);
            return View(news);
        }

        // GET: MedEngMgt/News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: MedEngMgt/News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            //Delete files
            var fileList = db.AttainFiles.Where(af => af.DocId == id.ToString() && af.DocType == "9").ToList(); ;
            foreach(var item in fileList)
            {
                AttainFile attainfiles = db.AttainFiles.Find("9", id.ToString(), item.SeqNo);
            if (attainfiles != null)
                {
                    FileInfo ff;
                    try
                    {
                        ff = new FileInfo(Path.Combine(Server.MapPath("~/Files/"), attainfiles.FileLink));
#if DEBUG
                        ff = new FileInfo(Path.Combine(Server.MapPath("~/App_Data/"), attainfiles.FileLink));
#endif
                        ff.Delete();
                    }
                    catch (Exception e)
                    {
                        return Content(e.Message);
                    }
                    db.AttainFiles.Remove(attainfiles);
                    db.SaveChanges();
                }
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
