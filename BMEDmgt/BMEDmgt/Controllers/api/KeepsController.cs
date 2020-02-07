using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Security;
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;

namespace BMEDmgt.Controllers.api
{
    public class KeepsController : ApiController
    {
        private BMEDcontext db = new BMEDcontext();

        // POST: api/Keeps
        [ResponseType(typeof(Keep))]
        public async Task<IHttpActionResult> PostKeep(Keep keep)
        {
            AssetKeep kp = db.AssetKeeps.Find(keep.AssetNo);
            Asset at = db.Assets.Find(keep.AssetNo);
            //
            keep.Email = "99242@gmail.com";
            keep.AssetName = db.Assets.Find(keep.AssetNo).Cname;
            keep.AccDpt = at.AccDpt;
            keep.SentDate = DateTime.Now;
            keep.Cycle = kp == null ? 0 : kp.Cycle.Value;
            keep.Src = "M";
            db.Entry(keep).State = EntityState.Modified;

            //
            KeepDtl dl = new KeepDtl();
            dl.DocId = keep.DocId;
            switch (kp == null ? "自行" : kp.InOut)
            {
                case "自行":
                    dl.InOut = "0";
                    break;
                case "委外":
                    dl.InOut = "1";
                    break;
                case "租賃":
                    dl.InOut = "2";
                    break;
                case "保固":
                    dl.InOut = "3";
                    break;
                case "借用":
                    dl.InOut = "4";
                    break;
                case "委外/自行":
                    dl.InOut = "5";
                    break;
                default:
                    dl.InOut = "0";
                    break;
            }
            db.KeepDtls.Add(dl);
            db.SaveChanges();
            //
            KeepFlow rf = new KeepFlow();
            rf.DocId = keep.DocId;
            rf.StepId = 1;
            rf.UserId = keep.UserId;
            rf.Status = "1";
            rf.Role = "";
            rf.Rtp = keep.UserId;
            rf.Rdt = null;
            rf.Rtt = DateTime.Now;
            rf.Cls = "申請者";
            db.KeepFlows.Add(rf);
            //
            //AssetKeep kp = db.AssetKeeps.Find(keep.AssetNo);
            //if (kp == null)
            //{
            //    throw new Exception("無工程師資料!!");
            //}
            rf = new KeepFlow();
            rf.DocId = keep.DocId;
            rf.StepId = 2;
            rf.UserId = kp == null ? keep.UserId : kp.KeepEngId;
            rf.Status = "?";
            AppUser u = db.AppUsers.Find(rf.UserId);
            if (u == null)
            {
                throw new Exception("無工程師資料!!");
            }
            rf.Role = Roles.GetRolesForUser(u.UserName).FirstOrDefault();
            rf.Rtp = null;
            rf.Rdt = null;
            rf.Rtt = DateTime.Now;
            rf.Cls = "設備工程師";
            if (dl.InOut != "0")         //2018-5-16修改
                rf.Cls = "維修工程師";
            db.KeepFlows.Add(rf);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (KeepExists(keep.DocId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = keep.DocId }, keep);
        }
        [HttpPost]
        public string PostKeepRecords(IEnumerable<KeepRecord> keepRecords)
        {
            string result = "";
            List<KeepRecord> newkprd = new List<KeepRecord>();
            //
            string docid = keepRecords.Select(k => k.DocId).FirstOrDefault();
            if (!string.IsNullOrEmpty(docid))
            {
                try
                {
                    string ano = db.Keeps.Find(docid).AssetNo;
                    db.AssetKeeps.Where( k => k.AssetNo == ano)
                        .Join(db.KeepFormatDtls, k => k.FormatId, f => f.FormatId,
                        (k, f) => f).ToList()
                        .ForEach(f => {
                            if (keepRecords.Count(r => r.FormatId == f.FormatId && r.Sno == f.Sno) <= 0)
                            {
                                newkprd.Add(new KeepRecord
                                {
                                    DocId = docid,
                                    FormatId = f.FormatId,
                                    Sno = f.Sno,
                                    Descript = f.Descript,
                                    KeepDes = ""
                                });
                            }
                        });
                    newkprd.AddRange(keepRecords.ToList());
                    db.KeepRecords.RemoveRange(db.KeepRecords.Where(k => k.DocId == docid));
                    db.KeepRecords.AddRange(newkprd);
                    db.SaveChanges();
                    result = "儲存成功!";
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            return result;
        }
        public IQueryable<KeepRecord> GetKeepRecordList(string ano)
        {
            List<KeepRecord> keepRecords = new List<KeepRecord>();
            var data = db.Keeps.Where(k => k.AssetNo == ano)
                .Join(db.KeepFlows.Where(f => f.Status == "?"),
                k => k.DocId, f => f.DocId,
                (k, f) => new { k, f }).FirstOrDefault();
            string docid = "";
            if (data != null)
            {
                docid = data.k.DocId;
                keepRecords = db.KeepRecords.Where(r => r.DocId == docid).ToList();
                if (keepRecords.Count() <= 0)
                {
                    db.AssetKeeps.Where(a => a.AssetNo == ano).
                    Join(db.KeepFormatDtls, a => a.FormatId, d => d.FormatId,
                    (a, d) => d).ToList()
                    .ForEach(d =>
                    {
                        keepRecords.Add(new KeepRecord
                        {
                            DocId = docid,
                            FormatId = d.FormatId,
                            Sno = d.Sno,
                            Descript = d.Descript,
                            KeepDes = ""
                        });
                    });
                }
            }
            
            return keepRecords.AsQueryable();
        }
            private bool KeepExists(string id)
        {
            return db.Keeps.Count(e => e.DocId == id) > 0;
        }
    }
}
