﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;
using System.Web.Security;
using WebMatrix.WebData;
using Newtonsoft.Json;
using PagedList;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class AppUsersController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        private int pageSize = 100;

        public ActionResult Index()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            SelectListItem li;
            db.Departments.ToList()
                .ForEach(d =>
                {
                    li = new SelectListItem();
                    li.Text = d.Name_C;
                    li.Value = d.DptId;
                    listItem.Add(li);

                });
            ViewData["DEPT"] = new SelectList(listItem, "Value", "Text");


            if (User.IsInRole("Admin") == true)
            {
                // Save log. 
                string logClass = "系統管理者紀錄";
                string logAction = "使用者維護";
                var result = new SystemLogsController().SaveLog(logClass, logAction);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection fm, int page = 1)
        {
            string qname = fm["qtyNAME"];
            string dpt = fm["qtyDEPT"];
            string uname = fm["qtyUSERNAME"];
            List<AppUser> ulist;
            AppUser usr = db.AppUsers.Find(WebSecurity.CurrentUserId);

            if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Manager"))
            {
                ulist = db.AppUsers.ToList();
                if (Roles.IsUserInRole("Manager"))
                {
                    ulist = ulist.Where(u => u.DptId == usr.DptId).ToList();
                }
            }
            else
            {
                ulist = db.AppUsers.Where(u => u.UserName == WebSecurity.CurrentUserName).ToList();
            }
            if (!string.IsNullOrEmpty(qname))
            {
                ulist = ulist.Where(u => u.FullName != null)
                             .Where(u => u.FullName.Contains(qname)).ToList();
            }
            if (!string.IsNullOrEmpty(dpt))
            {
                ulist = ulist.Where(u => u.DptId == dpt).ToList();
            }
            if (!string.IsNullOrEmpty(uname))
            {
                //int abc;
                //if (Int32.TryParse(uname, out abc))
                //{
                //    uname = uname.PadLeft(10, '0');                   
                //}
                ulist = ulist.Where(u => u.UserName == uname).ToList();
            }
            if (ulist.ToPagedList(page, pageSize).Count <= 0)
                return PartialView("List", ulist.ToPagedList(1, pageSize));

            return PartialView("List", ulist.ToPagedList(page, pageSize));
        }

        public ActionResult SelectUser()
        {
            List<SelectListItem> UserId = new List<SelectListItem>();
            AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
            UserId.Add(new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = "(" + u.UserName + ")" + u.FullName,
                Selected = false
            });
            ViewData["Suserid"] = UserId;

            return PartialView();
        }

        public ActionResult SelUsrCases()
        {
            List<SelectListItem> UserId = new List<SelectListItem>();
            AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
            UserId.Add(new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.FullName,
                Selected = false
            });
            ViewData["Suserid"] = UserId;

            return PartialView();
        }
        // GET: MedEngMgt/AppUser
        public ActionResult List()
        {
            AppUser usr = db.AppUsers.Find(WebSecurity.CurrentUserId);
            List<AppUser> ulist;
            if (Roles.IsUserInRole("Admin") || Roles.IsUserInRole("Manager"))
            {
                ulist = db.AppUsers.Where(u => u.Status == "Y").ToList();
                if (Roles.IsUserInRole("Manager"))
                {
                    ulist = ulist.Where(u => u.DptId == usr.DptId).ToList();
                }
                return PartialView(ulist);
            }
            else
                return PartialView(db.AppUsers.Where(u => u.UserName == WebSecurity.CurrentUserName).ToList());
        }

        // GET: MedEngMgt/AppUser/Details/5
        public ActionResult Details(int id)
        {
            AppUser u = db.AppUsers.Find(id);
            List<UserInRolesVModel> rv = UserInRolesVModel.getRoles();
            UserInRolesVModel uv;
            foreach (string r in Roles.GetRolesForUser(u.UserName))
            {
                uv = rv.Where(v => v.RoleName == r).ToList().FirstOrDefault();
                if (uv != null)
                {
                    uv.IsSelected = true;
                }
            }
            u.InRoles = rv;
            return View(u);
        }

        // GET: MedEngMgt/AppUser/Create
        public ActionResult Create()
        {
            AppUser u = new AppUser();
            u.InRoles = UserInRolesVModel.getRoles();
            return View(u);
        }

        // POST: MedEngMgt/AppUser/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                //appUser.Id = Guid.NewGuid();
                //db.AppUsers.Add(appUser);
                //db.SaveChanges();
                MembershipUser user = Membership.GetUser(appUser.UserName);
                if (user != null)
                {
                    ModelState.AddModelError("","使用者名稱重複");
                    return View(appUser);
                }
                //
                MembershipUser ur = Membership.CreateUser(appUser.UserName, appUser.Password, appUser.Email);
                //
                AppUser newUser = db.AppUsers.Where(a => a.UserName == appUser.UserName).ToList()
                                             .FirstOrDefault();
                newUser.FullName = appUser.FullName;
                newUser.DptId = appUser.DptId;
                newUser.Mobile = appUser.Mobile;
                newUser.VendorId = appUser.VendorId;
                db.Entry(newUser).State = EntityState.Modified;
                db.SaveChanges();
                // Save log. 
                string logClass = "管理紀錄";
                string logAction = "使用者維護 > 新增使用者 > " + newUser.FullName + "(" + newUser.UserName + ")";
                var result = new SystemLogsController().SaveLog(logClass, logAction);
                //
                List<UserInRolesVModel> uv = appUser.InRoles.Where(v => v.IsSelected == true).ToList();
                foreach (UserInRolesVModel u in uv)
                {
                    Roles.AddUserToRole(appUser.UserName, u.RoleName);
                }
                return RedirectToAction("Index");
            }

            return View(appUser);
        }

        // GET: MedEngMgt/AppUser/Edit/5
        public ActionResult Edit(int id)
        {
            AppUser u = db.AppUsers.Find(id);
            if (u == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (u.VendorId != null)
                    u.VendorName = db.Vendors.Find(u.VendorId).VendorName;
                List<UserInRolesVModel> rv = UserInRolesVModel.getRoles();
                UserInRolesVModel uv;
                foreach (string r in Roles.GetRolesForUser(u.UserName))
                {
                    uv = rv.Where(v => v.RoleName == r).ToList().FirstOrDefault();
                    if (uv != null)
                    {
                        uv.IsSelected = true;
                    }
                }
                u.InRoles = rv;
            }
            return View(u);
        }

        // POST: MedEngMgt/AppUser/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                MembershipUser user = Membership.GetUser(appUser.UserName);
                string checkResult = "";
                if (user != null)
                {
                    //user.Email = appUser.Email;
                    //Membership.UpdateUser(user);
                    if (Roles.IsUserInRole("Admin"))
                    {
                        var oriRoles = Roles.GetRolesForUser(appUser.UserName).ToList();  // Origin user roles.
                        if (Roles.GetRolesForUser(appUser.UserName).Count() > 0)
                            Roles.RemoveUserFromRoles(appUser.UserName, Roles.GetRolesForUser(appUser.UserName));
                        //Roles.AddUserToRole(userprofile.UserName, userprofile.InRole);
                        List<UserInRolesVModel> uv = appUser.InRoles.Where(v => v.IsSelected == true).ToList();
                        foreach (UserInRolesVModel u in uv)
                        {
                            Roles.AddUserToRole(appUser.UserName, u.RoleName);
                        }
                        var newRoles = Roles.GetRolesForUser(appUser.UserName).ToList();  // Updated user roles.
                        var removeRoles = oriRoles.Except(newRoles).ToList();
                        var addRoles = newRoles.Except(oriRoles).ToList();
                        if (removeRoles.Count() > 0 || addRoles.Count() > 0)
                        {
                            checkResult += "設定角色：";
                        }
                        if (removeRoles.Count() > 0)
                        {
                            checkResult += "刪除";
                            foreach (var name in removeRoles)
                            {
                                var roleDes = db.AppRoles.Where(r => r.RoleName == name).ToList().First().Description;
                                checkResult += "【" + roleDes + "】";
                            }
                            checkResult += ";";
                        }
                        if (addRoles.Count() > 0)
                        {
                            checkResult += "新增";
                            foreach (var name in addRoles)
                            {
                                var roleDes = db.AppRoles.Where(r => r.RoleName == name).ToList().First().Description;
                                checkResult += "【" + roleDes + "】";
                            }
                            checkResult += ";";
                        }
                    }
                    //Checking the modified columns.
                    var oriUser = db.AppUsers.Where(u => u.Id == appUser.Id).ToList().FirstOrDefault();
                    if (oriUser.UserName != appUser.UserName)
                    {
                        checkResult += "使用者名稱由" + oriUser.UserName + "變更為" + appUser.UserName + "；";
                    }
                    if (oriUser.FullName != appUser.FullName)
                    {
                        checkResult += "使用者全名由" + oriUser.FullName + "變更為" + appUser.FullName + "；";
                    }
                    if (oriUser.Email != appUser.Email)
                    {
                        checkResult += "電子信箱由" + oriUser.Email + "變更為" + appUser.Email + "；";
                    }
                    if (oriUser.Ext != appUser.Ext)
                    {
                        checkResult += "分機由" + oriUser.Ext + "變更為" + appUser.Ext + "；";
                    }
                    if (oriUser.Mobile != appUser.Mobile)
                    {
                        checkResult += "行動電話由" + oriUser.Mobile + "變更為" + appUser.Mobile + "；";
                    }
                    if (oriUser.DptId != appUser.DptId)
                    {
                        checkResult += "所屬部門由" + oriUser.DptId + "變更為" + appUser.DptId + "；";
                    }
                    if (oriUser.Status != appUser.Status)
                    {
                        checkResult += "狀態由" + oriUser.Status + "變更為" + appUser.Status + "；";
                    }
                    //
                    appUser.LastActivityDate = DateTime.Now;
                    db.Entry(oriUser).State = EntityState.Detached;
                    db.Entry(appUser).State = EntityState.Modified;
                    db.SaveChanges();
                    //
                    if (!string.IsNullOrEmpty(appUser.newPassword))
                    {
                        user.ChangePassword(appUser.Password, appUser.newPassword);
                        checkResult += "密碼變更；";
                    }
                }
                if (Roles.IsUserInRole("Admin"))
                {
                    var ur = db.AppUsers.Find(appUser.Id);
                    if (ur != null)
                    {
                        //Change status from "N" to "Y"
                        if (ur.Status == "N" && appUser.Status == "Y")  
                        {
                            ur.Status = appUser.Status;
                            db.Entry(ur).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                // Save log. 
                SystemLog log = new SystemLog();
                log.LogClass = "管理紀錄";
                log.LogTime = DateTime.UtcNow.AddHours(8);
                log.UserId = WebSecurity.CurrentUserId;
                log.Action = "使用者維護 > 編輯使用者 > " + appUser.FullName + "(" + appUser.UserName + ") > " + checkResult;
                db.SystemLogs.Add(log);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(appUser);
        }

        // GET: MedEngMgt/AppUser/Delete/5
        public ActionResult Delete(int id)
        {
            AppUser u = db.AppUsers.Find(id);
            List<UserInRolesVModel> rv = UserInRolesVModel.getRoles();
            UserInRolesVModel uv;
            foreach (string r in Roles.GetRolesForUser(u.UserName))
            {
                uv = rv.Where(v => v.RoleName == r).ToList().FirstOrDefault();
                if (uv != null)
                {
                    uv.IsSelected = true;
                }
            }
            u.InRoles = rv;
            return View(u);
        }

        // POST: MedEngMgt/AppUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            AppUser appUser = db.AppUsers.Find(id);
            db.AppUsers.Remove(appUser);
            db.SaveChanges();
            // Save log. 
            string logClass = "管理紀錄";
            string logAction = "使用者維護 > 刪除使用者 > " + appUser.FullName + "(" + appUser.UserName + ")";
            var result = new SystemLogsController().SaveLog(logClass, logAction);

            return RedirectToAction("Index");
        }

        public JsonResult GetUsersInDpt(string id)
        {
            Department d = db.Departments.Find(id);
            List<AppUser> ul;
            List<UserList> us = new List<UserList>();
            string s = "";
            if (d != null)
            {
                ul = db.AppUsers.Where(p => p.DptId == d.DptId).ToList();
                //s += "[";
                foreach (AppUser f in ul)
                {
                    UserList u = new UserList();
                    u.uid = f.Id;
                    u.uname = "(" + f.UserName + ")" + f.FullName;
                    us.Add(u);
                }
                s = JsonConvert.SerializeObject(us);
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsersByKeyname(string keyname)
        {
            List<AppUser> ul;
            List<UserList> us = new List<UserList>();
            string s = "";
            ul = db.AppUsers.Where(p => p.FullName.Contains(keyname) || p.UserName == keyname)
                            .Where(p => p.Status == "Y").ToList();
            foreach (AppUser f in ul)
            {
                UserList u = new UserList();
                u.uid = f.Id;
                u.uname = "(" + f.UserName + ")" + f.FullName;
                us.Add(u);
            }
            s = JsonConvert.SerializeObject(us);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsersByAssetNo(string id)
        {
            Asset at = db.Assets.Find(id);
            string s = "";
            if (at != null)
            {
                Department d = db.Departments.Find(at.DelivDpt);
                List<AppUser> ul;
                List<UserList> us = new List<UserList>();
                
                if (d != null)
                {
                    ul = db.AppUsers.Where(p => p.DptId == d.DptId).ToList();
                    //s += "[";
                    foreach (AppUser f in ul)
                    {
                        UserList u = new UserList();
                        u.uid = f.Id;
                        u.uname = "(" + f.UserName + ")" + f.FullName;
                        us.Add(u);
                    }
                    s = JsonConvert.SerializeObject(us);
                }
            }
           
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsersInDptByKeyname(string keyname)
        {
            List<AppUser> ul;
            List<UserList> us = new List<UserList>();
            string s = "";
            if (!string.IsNullOrEmpty(keyname))
            {
                db.Departments.Where(u => u.Name_C.Contains(keyname))
                    .ToList()
                    .ForEach(d =>
                    {
                        ul = db.AppUsers.Where(p => p.DptId == d.DptId)
                                        .Where(p => p.Status == "Y").ToList();
                        foreach (AppUser f in ul)
                        {
                            UserList u = new UserList();
                            u.uid = f.Id;
                            u.uname = "(" + f.UserName + ")" + f.FullName;
                            us.Add(u);
                        }
                        s = JsonConvert.SerializeObject(us);
                    });
            }
            s = JsonConvert.SerializeObject(us);

            return Json(s, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserById(int id)
        {
            var user = db.AppUsers.Where(p => p.Id == id).ToList().FirstOrDefault();
            if (user != null)
            {
                return Json(user, JsonRequestBehavior.AllowGet);
            }
            return Json("查無資料", JsonRequestBehavior.AllowGet);
        }

        public string GetFullName()
        {
            return db.AppUsers.Find(WebSecurity.CurrentUserId).FullName;
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

    public class UserList
    {
        public int uid;
        public string uname;
    }
}
