using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;
using WebMatrix.WebData;
using BMEDmgt.Filters;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    [Authorize]
    public class QuestionnaireController : Controller
    {
        private BMEDcontext db = new BMEDcontext();


        public ActionResult Index()
        {
            QuestSend sd = new QuestSend();
            sd.YYYYmm = (DateTime.Now.Year * 100 + DateTime.Now.Month).ToString();
            //List<SelectListItem> listItem = new List<SelectListItem>();
            //List<Group> gv = db.Groups.ToList();
            //SelectListItem it;
            //foreach (Group g in gv)
            //{
            //    it = new SelectListItem();
            //    it.Text = g.GrpName;
            //    it.Value = g.GroupId;
            //    listItem.Add(it);
            //}
            //ViewData["GROUP"] = new SelectList(listItem, "Value", "Text", "");
            return View(sd);
        }
        //
        // GET: /Questionnaire/
        //[AllowAnonymous]
        //public ActionResult New(int id)
        //{
        //    DateTime dd = DateTime.Now;
        //    string yyyymm = Convert.ToString(dd.Year * 100 + dd.Month);

        //    var evalVM = new Evaluation();
        //    AppUser u = db.AppUsers.Find(WebSecurity.CurrentUserId);
        //    QuestMain main = new QuestMain();
        //    main.Docid = main.GetID(ref db);
        //    main.YYYYmm = yyyymm;
        //    if (u != null)
        //    {
        //        if (u.DptId != null)
        //        {
        //            Department c = db.Departments.Find(u.DptId);
        //            main.CustId = u.DptId;
        //            main.CustNam = c.Name_C;
        //        }
        //    }
        //    main.Rtt = DateTime.Now;
        //    db.QuestMains.Add(main);
        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        //ModelState.AddModelError("", e.Message);
        //        return Content(e.Message);
        //    }
        //    //
        //    evalVM.Qname = db.QuestionnaireMs.Find(id).Qname;
        //    evalVM.Docid = main.Docid;
        //    evalVM.YYYYmm = yyyymm;
        //    evalVM.CustId = main.CustId;
        //    evalVM.CustNam = main.CustNam;

        //    List<Questionnaire> ql =
        //        db.Questionnaires.Where(qt => qt.VerId == id).ToList();
        //    Question q;
        //    int i = 1;
        //    foreach (Questionnaire a in ql)
        //    {
        //        q = new Question();
        //        q.ID = id;
        //        q.QID = a.Qid;
        //        q.QuestionText = "(" + Convert.ToString(i) + ") " + a.Qtitle;
        //        q.Typ = a.Typ;
        //        evalVM.Questions.Add(q);
        //        i++;
        //    }
        //    //
        //    List<SelectListItem> listItem = new List<SelectListItem>();
        //    List<SelectListItem> listItem2 = new List<SelectListItem>();
        //    SelectListItem li;
        //    db.Departments.ToList()
        //        .ForEach(d =>
        //        {
        //            li = new SelectListItem();
        //            li.Text = d.Name_C;
        //            li.Value = d.DptId;
        //            listItem.Add(li);

        //        });
        //    ViewData["Dept"] = new SelectList(listItem, "Value", "Text");
        //    db.Contracts.ToList()
        //        .ForEach(c =>
        //        {
        //            li = new SelectListItem();
        //            li.Text = c.ContractName;
        //            li.Value = c.ContractNo;
        //            listItem2.Add(li);
        //        });
        //    ViewData["Contract"] = new SelectList(listItem2, "Value", "Text");

        //    return View(evalVM);
        //}

        [AllowAnonymous]
        public ActionResult New(int id, string dpt)
        {
            DateTime dd = DateTime.Now;
            string yyyymm = Convert.ToString(dd.Year * 100 + dd.Month);

            var evalVM = new Evaluation();
            QuestMain main = new QuestMain();
            main.Docid = main.GetID(ref db);
            main.YYYYmm = yyyymm;
            Department c = db.Departments.Find(dpt);
            main.CustId = c.DptId;
            main.CustNam = c.Name_C;
            main.Rtt = DateTime.Now;
            db.QuestMains.Add(main);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                //ModelState.AddModelError("", e.Message);
                return Content(e.Message);
            }
            //
            evalVM.Qname = db.QuestionnaireMs.Find(id).Qname;
            evalVM.Docid = main.Docid;
            evalVM.YYYYmm = yyyymm;
            evalVM.CustId = main.CustId;
            evalVM.CustNam = main.CustNam;

            List<Questionnaire> ql =
                db.Questionnaires.Where(qt => qt.VerId == id).ToList();
            Question q;
            int i = 1;
            foreach (Questionnaire a in ql)
            {
                q = new Question();
                q.ID = id;
                q.QID = a.Qid;
                q.QuestionText = "(" + Convert.ToString(i) + ") " + a.Qtitle;
                q.Typ = a.Typ;
                evalVM.Questions.Add(q);
                i++;
            }
            //
            List<SelectListItem> listItem = new List<SelectListItem>();
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            SelectListItem li;
            db.Departments.ToList()
                .ForEach(d =>
                {
                    li = new SelectListItem();
                    li.Text = d.Name_C;
                    li.Value = d.DptId;
                    listItem.Add(li);

                });
            ViewData["Dept"] = new SelectList(listItem, "Value", "Text");
            //db.Contracts.Where(ct => ct.Status == "Y")
            //    .Join(db.AssetKeeps, ct => ct.ContractNo, k => k.ContractNo,
            //    (ct, k) => new { ct, k})
            //    .Join(db.Assets.Where(a => a.DelivDpt == dpt), ct => ct.k.AssetNo, a => a.AssetNo,
            //    (ct, a) => ct).ToList()
            //    .ForEach(ct =>
            //    {
            //        li = new SelectListItem();
            //        li.Text = ct.ct.ContractName;
            //        li.Value = ct.ct.ContractNo;
            //        listItem2.Add(li);
            //    });
            //ViewData["Contract"] = new SelectList(listItem2, "Value", "Text");

            return View(evalVM);
        }

        [HttpPost]
        [MyErrorHandler]
        [AllowAnonymous]
        public ActionResult New(Evaluation model)
        {
            if (model.Questions.Where(q => q.SelectedAnswer == null && q.Typ == "select").Count() > 0)
            {
                throw new Exception("尚有項目未圈選!!");
            }
            if (ModelState.IsValid)
            {
                QuestMain main = db.QuestMains.Find(model.Docid);
                main.CustId = model.CustId;
                main.CustNam = db.Departments.Find(model.CustId).Name_C;
                main.ContractNo = model.ContractNo;
                db.Entry(main).State = EntityState.Modified;
                //
                List<QuestAnswer> at = db.QuestAnswers.Where(a => a.Docid == model.Docid).ToList();
                QuestAnswer ar;
                foreach (QuestAnswer w in at)
                {
                    if (w != null)
                        db.QuestAnswers.Remove(w);
                }
                foreach (var q in model.Questions)
                {
                    // Save the data 
                    ar = new QuestAnswer();
                    ar.Docid = model.Docid;
                    ar.VerId = q.ID;
                    ar.Qid = q.QID;
                    ar.Answer = q.SelectedAnswer;
                    db.QuestAnswers.Add(ar);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
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

        public ActionResult New2(int id, string docId)
        {
            DateTime dd = DateTime.Now;
            string yyyymm = Convert.ToString(dd.Year * 100 + dd.Month);

            var evalVM = new Evaluation();
            QuestMain main = new QuestMain();
            //main.Docid = main.GetID(ref db);
            main.Docid = docId;
            main.YYYYmm = yyyymm;
            //Department c = db.Departments.Find(dpt);
            //main.CustId = c.DptId;
            //main.CustNam = c.Name_C;
            main.Rtt = DateTime.Now;
            var questMain = db.QuestMains.Where(qm => qm.Docid == docId).FirstOrDefault();
            if (questMain == null)
            {
                db.QuestMains.Add(main);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    //ModelState.AddModelError("", e.Message);
                    return Content(e.Message);
                }
            }
            //
            evalVM.Qname = db.QuestionnaireMs.Find(id).Qname;
            evalVM.Docid = main.Docid;
            evalVM.YYYYmm = yyyymm;
            evalVM.CustId = main.CustId;
            evalVM.CustNam = main.CustNam;

            List<Questionnaire> ql =
                db.Questionnaires.Where(qt => qt.VerId == id).ToList();
            Question q;
            int i = 1;
            foreach (Questionnaire a in ql)
            {
                q = new Question();
                q.ID = id;
                q.QID = a.Qid;
                q.QuestionText = "(" + Convert.ToString(i) + ") " + a.Qtitle;
                q.Typ = a.Typ;
                evalVM.Questions.Add(q);
                i++;
            }
            //
            List<SelectListItem> listItem = new List<SelectListItem>();
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            SelectListItem li;
            db.Departments.ToList()
                .ForEach(d =>
                {
                    li = new SelectListItem();
                    li.Text = d.Name_C;
                    li.Value = d.DptId;
                    listItem.Add(li);

                });
            ViewData["Dept"] = new SelectList(listItem, "Value", "Text");
            //db.Contracts.Where(ct => ct.Status == "Y")
            //    .Join(db.AssetKeeps, ct => ct.ContractNo, k => k.ContractNo,
            //    (ct, k) => new { ct, k})
            //    .Join(db.Assets.Where(a => a.DelivDpt == dpt), ct => ct.k.AssetNo, a => a.AssetNo,
            //    (ct, a) => ct).ToList()
            //    .ForEach(ct =>
            //    {
            //        li = new SelectListItem();
            //        li.Text = ct.ct.ContractName;
            //        li.Value = ct.ct.ContractNo;
            //        listItem2.Add(li);
            //    });
            //ViewData["Contract"] = new SelectList(listItem2, "Value", "Text");

            return View(evalVM);
        }

        [HttpPost]
        [MyErrorHandler]
        public ActionResult New2(Evaluation model)
        {
            if (model.Questions.Where(q => q.SelectedAnswer == null && q.Typ == "select").Count() > 0)
            {
                throw new Exception("尚有項目未圈選!!");
            }
            if (ModelState.IsValid)
            {
                QuestMain main = db.QuestMains.Find(model.Docid);
                //main.CustId = model.CustId;
                ///main.CustNam = db.Departments.Find(model.CustId).Name_C;
                //main.ContractNo = model.ContractNo;
                db.Entry(main).State = EntityState.Modified;
                //
                List<QuestAnswer> at = db.QuestAnswers.Where(a => a.Docid == model.Docid).ToList();
                QuestAnswer ar;
                foreach (QuestAnswer w in at)
                {
                    if (w != null)
                        db.QuestAnswers.Remove(w);
                }
                foreach (var q in model.Questions)
                {
                    // Save the data 
                    ar = new QuestAnswer();
                    ar.Docid = model.Docid;
                    ar.VerId = q.ID;
                    ar.Qid = q.QID;
                    ar.Answer = q.SelectedAnswer;
                    db.QuestAnswers.Add(ar);
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
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

        public ActionResult Titles()
        {
            var titles = db.QuestionnaireMs.Where(m => m.Flg == "Y")
            .Join(db.Questionnaires, m => m.VerId, q => q.VerId,
            (m, q) => q).OrderBy(q => q.VerId).ThenBy(q => q.Qid).Select(q => q.Qtitle);

            return PartialView(titles);
        }
        // GET: /Questionnaire/Details/5

        public ActionResult Details(int id = 0)
        {
            Questionnaire questionnaire = db.Questionnaires.Find(id);
            if (questionnaire == null)
            {
                return HttpNotFound();
            }
            return View(questionnaire);
        }

        public ActionResult Preview(int id)
        {
            DateTime dd = DateTime.Now;
            string yyyymm = Convert.ToString(dd.Year * 100 + dd.Month);

            var evalVM = new Evaluation();
            QuestMain main = new QuestMain();
            main.Docid = main.GetID(ref db);
            main.YYYYmm = yyyymm;
            main.Rtt = DateTime.Now;
            db.QuestMains.Add(main);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                //ModelState.AddModelError("", e.Message);
                return Content(e.Message);
            }
            //
            evalVM.Qname = db.QuestionnaireMs.Find(id).Qname;
            evalVM.Docid = main.Docid;
            evalVM.YYYYmm = yyyymm;
            //evalVM.CustId = main.CustId;
            //evalVM.CustNam = main.CustNam;

            List<Questionnaire> ql =
                db.Questionnaires.Where(qt => qt.VerId == id).ToList();
            Question q;
            int i = 1;
            foreach (Questionnaire a in ql)
            {
                q = new Question();
                q.ID = id;
                q.QID = a.Qid;
                q.QuestionText = "(" + Convert.ToString(i) + ") " + a.Qtitle;
                q.Typ = a.Typ;
                evalVM.Questions.Add(q);
                i++;
            }
            //
            List<SelectListItem> listItem = new List<SelectListItem>();
            List<SelectListItem> listItem2 = new List<SelectListItem>();
            SelectListItem li;
            db.Departments.ToList()
                .ForEach(d =>
                {
                    li = new SelectListItem();
                    li.Text = d.Name_C;
                    li.Value = d.DptId;
                    listItem.Add(li);

                });
            ViewData["Dept"] = new SelectList(listItem, "Value", "Text");
            //db.Contracts.ToList()
            //    .ForEach(c =>
            //    {
            //        li = new SelectListItem();
            //        li.Text = c.ContractName;
            //        li.Value = c.ContractNo;
            //        listItem2.Add(li);
            //    });
            //ViewData["Contract"] = new SelectList(listItem2, "Value", "Text");

            return View(evalVM);
        }

        //
        // GET: /Questionnaire/Create

        public ActionResult Create(int id)
        {
            Questionnaire q = new Questionnaire();
            int cnt = db.Questionnaires.Where(u => u.VerId == id).Select(u => u.Qid).DefaultIfEmpty(0).Max();
            q.VerId = id;
            q.Qid = cnt + 1;

            return View(q);
        }

        //
        // POST: /Questionnaire/Create

        [HttpPost]
        public ActionResult Create(Questionnaire questionnaire)
        {
            if (ModelState.IsValid)
            {
                db.Questionnaires.Add(questionnaire);
                db.SaveChanges();

                return RedirectToAction("List", new { id = questionnaire.VerId });
            }

            return View(questionnaire);
        }

        //
        // GET: /Questionnaire/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ViewData["verid"] = id;
            return View();
        }

        //
        // POST: /Questionnaire/Edit/5

        [HttpPost]
        public ActionResult Edit(Questionnaire questionnaire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionnaire).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(questionnaire);
        }

        public ActionResult List(int id = 0)
        {
            ViewData["verid"] = id;
            return View(db.Questionnaires.Where(q => q.VerId == id).ToList());
        }
        //
        // GET: /Questionnaire/Delete/5

        public ActionResult Delete(int id = 0, int qid = 0)
        {
            Questionnaire questionnaire = db.Questionnaires.Find(id, qid);
            if (questionnaire == null)
            {
                return HttpNotFound();
            }
            db.Questionnaires.Remove(questionnaire);
            db.SaveChanges();

            return RedirectToAction("List", new { id = id });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}