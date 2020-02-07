using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;

namespace BMEDmgt.Controllers.api
{
    public class DocIdFactoriesController : ApiController
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: api/DocIdFactories/5
        [ResponseType(typeof(string))]
        public string GetDocId(string typ, string username)
        {
            if (!string.IsNullOrEmpty(typ))
            {
                string s = "";
                if (typ == "1") //Repair
                {
                    s = db.Repairs.Select(r => r.DocId).Max();
                }
                else if (typ == "2") //Keep
                    s = db.Keeps.Select(k => k.DocId).Max();
                string did = "";
                int yymm = (System.DateTime.Now.Year - 1911) * 100 + System.DateTime.Now.Month;
                if (!string.IsNullOrEmpty(s))
                {
                    did = s;
                }
                if (did != "")
                {
                    if (Convert.ToInt64(did) / 10000 == yymm)
                        did = Convert.ToString(Convert.ToInt64(did) + 1);
                    else
                        did = Convert.ToString(yymm * 10000 + 1);
                }
                else
                {
                    did = Convert.ToString(yymm * 10000 + 1);
                }
                if (typ == "1")
                {
                    Repair r = new Repair();
                    AppUser u = db.AppUsers.Where(p => p.UserName == username).FirstOrDefault();
                    //CustOrgan c = db.CustOrgans.Find(u.DptId);
                    //Vendor v = db.Vendors.Find(u.VendorId);
                    r.Email = u.Email == null ? "" : u.Email;
                    Department d = db.Departments.Find(u.DptId);
                    r.DocId = did;
                    r.UserId = u.Id;
                    r.UserName = u.FullName;
                    //r.DptName = c.CustId == null ? "" : c.CustNam;
                    //r.AccDpt = c.CustId == null ? "" : c.CustId;
                    //r.AccDptName = c.CustId == null ? "" : c.CustNam;
                    r.ApplyDate = DateTime.Now;
                    r.DptId = d == null ? "" : d.DptId;
                    r.DptName = d == null ? "" : d.Name_C;
                    r.AccDpt = d == null ? "" : d.DptId;
                    r.AccDptName = d == null ? "" : d.Name_C;
                    r.Contact = u.Ext == null ? "" : u.Ext;
                    r.ApplyDate = DateTime.Now;
                    r.CheckerId = u.Id;
                    r.CheckerName = u.FullName;
                    r.Amt = 0;
                    //
                    string str = "insert into repair(DocId,UserId,UserName,ApplyDate,DptId,Contact,AccDpt,Amt,CheckerId";
                    str += ") values(@1,@2,@3,@4,@5,@6,@7,@8,@9)";
                    db.Database.ExecuteSqlCommand(str,
                        new SqlParameter("@1", r.DocId),
                        new SqlParameter("@2", r.UserId),
                        new SqlParameter("@3", r.UserName),
                        new SqlParameter("@4", r.ApplyDate),
                        new SqlParameter("@5", r.DptId),
                        new SqlParameter("@6", r.Contact),
                        new SqlParameter("@7", r.AccDpt),
                        new SqlParameter("@8", r.Amt),
                        new SqlParameter("@9", r.CheckerId));
                    db.SaveChanges();
                    //
                    RepairDtl dl = new RepairDtl();
                    dl.DocId = r.DocId;
                    db.RepairDtls.Add(dl);
                    db.SaveChanges();

                }
                else if (typ == "2")
                {
                    Keep r = new Keep();
                    AppUser u = db.AppUsers.Where(p => p.UserName == username).FirstOrDefault();
                    //CustOrgan c = db.CustOrgans.Find(u.DptId);
                    //Vendor v = db.Vendors.Find(u.VendorId);
                    r.Email = u.Email == null ? "" : u.Email;
                    Department d = db.Departments.Find(u.DptId);
                    r.DocId = did;
                    r.UserId = u.Id;
                    r.UserName = u.FullName;
                    //r.DptName = c.CustId == null ? "" : c.CustNam;
                    //r.AccDpt = c.CustId == null ? "" : c.CustId;
                    //r.AccDptName = c.CustId == null ? "" : c.CustNam;
                    r.SentDate = DateTime.Now;
                    r.DptId = d == null ? "" : d.DptId;
                    r.Company = d == null ? "" : d.Name_C;
                    r.AccDpt = d == null ? "" : d.DptId;
                    r.AccDptName = d == null ? "" : d.Name_C;
                    r.Contact = u.Ext == null ? "" : u.Ext;
                    //
                    db.Keeps.Add(r);
                    db.SaveChanges();
                }
                return did;
            }

            return "";
        }
        
        public string GetID(string doctyp)
        {
            //string str = "";
            //str += "SELECT MAX(DOCID) RECNO FROM REPAIR ";
            //var r = db.Database.SqlQuery(typeof(string), str);
            string s = "";
            if (doctyp == "1") //Repair
                s = db.Repairs.Select(r => r.DocId).Max();
            else if(doctyp == "2") //Keep
                s = db.Keeps.Select(k => k.DocId).Max();
            string did = "";
            int yymm = (System.DateTime.Now.Year - 1911) * 100 + System.DateTime.Now.Month;
            if (!string.IsNullOrEmpty(s))
            {
                did = s;
            }
            if (did != "")
            {
                if (Convert.ToInt64(did) / 100000 == yymm)
                    did = Convert.ToString(Convert.ToInt64(did) + 1);
                else
                    did = Convert.ToString(yymm * 100000 + 1);
            }
            else
            {
                did = Convert.ToString(yymm * 100000 + 1);
            }
            return did;
        }
        //public string GetDocId(string id)
        //{
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        ApplicationDbContext db = new ApplicationDbContext();
        //        string DocId;
        //        string yyyymm = Convert.ToString(DateTime.Now.Year * 100 + DateTime.Now.Month);
        //        string oldDocid = db.DocIdFactorys.Where(d => d.DocType == id)
        //            .Select(d => d.DocId).DefaultIfEmpty().Max();
        //        DocIdFactory factory = new DocIdFactory(id);
        //        //string newDocid = "";
        //        if (!string.IsNullOrEmpty(oldDocid))
        //        {
        //            if (oldDocid.Substring(0, 6) == yyyymm)
        //            {
        //                DocId = Convert.ToString(Convert.ToInt32(oldDocid) + 1);
        //            }
        //            else
        //            {
        //                DocId = yyyymm + "0001";
        //            }
        //        }
        //        else
        //        {
        //            DocId = yyyymm + "0001";
        //        }
        //        factory.DocId = DocId;
        //        factory.Rtt = DateTime.Now;
        //        db.DocIdFactorys.Add(factory);
        //        try
        //        {
        //            db.SaveChanges();
        //            return DocId;
        //        }
        //        catch (Exception e)
        //        {
        //            return "Error: " + e.Message;
        //        }
        //    }
        //    return "Error: 請確認[表單類別]";
        //}
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //private bool DocIdFactoryExists(string id)
        //{
        //    return db.DocIdFactorys.Count(e => e.DocType == id) > 0;
        //}
    }
}