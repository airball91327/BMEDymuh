using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Filters;
using BMEDmgt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class FuncsController : Controller
    {
        private BMEDcontext db = new BMEDcontext();
        // GET: MedEngMgt/Funcs
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetFuncs(string module)
        {
            List<Func> fc = db.Funcs.Where(f => f.Flg == "Y")
                .Where(f => f.Module == module).ToList();
            string rname = Roles.GetRolesForUser().FirstOrDefault();
            int rid = 4;
            if (!string.IsNullOrEmpty(rname))
            {
                rid = db.AppRoles.Where(r => r.RoleName == rname).First()
                                .RoleId;
                
            }
            fc = fc.Join(db.FuncsInRoles.Where(r => r.RoleId == rid), f => f.FuncId, r => r.FuncId,
                    (f, r) => f).ToList();
            return PartialView(fc);
        }
        //
        public ActionResult RoleWithFuncs(string roleid = "", string module = "")
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            db.AppRoles.ToList().ForEach(d =>
            {
                listItem.Add(new SelectListItem { Text = d.Description, Value = d.RoleId.ToString() });
            });
            ViewData["RoleId"] = new SelectList(listItem, "Value", "Text", "");
            //
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            db.Funcs.GroupBy(g => g.Module).Select(g => g.FirstOrDefault())
                .ToList().ForEach(d =>
            {
                listItem2.Add(new SelectListItem { Text = d.Module, Value = d.Module });
            });
            ViewData["Module"] = new SelectList(listItem2, "Value", "Text", "");
            //
            if (roleid != "" && module != "")
            {
                FuncsInRoles fs = new FuncsInRoles();
                fs.RoleId = Int32.Parse(roleid);
                fs.Module = module;
                RoleInFuncsVModel rv;
                List<RoleInFuncsVModel> rvlst = new List<RoleInFuncsVModel>();
                db.Funcs.Where(f => f.Module == fs.Module).ToList()
                    .ForEach(f =>
                    {
                        rv = new RoleInFuncsVModel();
                        rv.FuncId = f.FuncId;
                        rv.FuncName = f.FuncName;
                        rv.IsSelected = db.FuncsInRoles.Where(fr => fr.RoleId == fs.RoleId)
                        .Where(fr => fr.FuncId == f.FuncId).Count() > 0 ? true : false;
                        rvlst.Add(rv);
                    });
                fs.InFuncs = rvlst;
                return View(fs);
            }
            return View();
        }


        [HttpPost]
        [MyErrorHandler]
        public ActionResult RoleWithFuncs(FuncsInRoles fs)
        {
            //
            FuncsInRoles r;
            List<RoleInFuncsVModel> rvlst = new List<RoleInFuncsVModel>();
            db.FuncsInRoles.Where(f => f.RoleId == fs.RoleId).ToList()
                .ForEach(f =>
                {
                    if (db.Funcs.Find(f.FuncId).Module == fs.Module)
                        db.FuncsInRoles.Remove(f);
                });
            fs.InFuncs.Where(f => f.IsSelected == true).ToList()
                .ForEach(f =>
                {
                    r = new FuncsInRoles();
                    r.FuncId = f.FuncId;
                    r.RoleId = fs.RoleId;
                    db.FuncsInRoles.Add(r);
                });
            try
            {
                db.SaveChanges();
                return new JsonResult
                {
                    Data = new { success = true, error = "" },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}