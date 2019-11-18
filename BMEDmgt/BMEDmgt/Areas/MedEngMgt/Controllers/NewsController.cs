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
using WebMatrix.WebData;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
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

            return View(newslist);
        }

        public ActionResult ShowNews()
        {
            List<News> newslist = new List<News>();
            DateTime dt = DateTime.Now;
            db.News.Where(n => n.Status == "Y")
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
            return View(news);
        }

        // POST: MedEngMgt/News/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewsId,NewsTitle,NewsContent,Sdate,Edate,UserId,Status,RTT")] News news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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
