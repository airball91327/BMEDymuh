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
using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;

namespace BMEDmgt.Controllers.api
{
    //[Route("api/[controller]")]
    public class RepairsController : ApiController
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: api/Repairs
        public IQueryable<Repair> GetRepairs()
        {
            return db.Repairs;
        }

        public IQueryable<Repair> GetRepList(string assetNo)
        {
            string[] s = new string[] { "?", "2" };
            var rf = db.RepairFlows.Where(f => s.Contains(f.Status));
            List<Repair> rs = new List<Repair>();
            var isQ = db.QuestAnswers.Select(q => q.Docid).Distinct();
            db.Repairs.Where(r => r.AssetNo == assetNo)
                .Join(db.RepairDtls, r => r.DocId, d => d.DocId,
                (r, d) => new
                {
                    r,
                    enddate = d.EndDate
                })
                .Join(rf, r => r.r.DocId, f => f.DocId,
                (r, f) => r).ToList()
                .ForEach(k =>
                {
                    rs.Add(new Repair
                    {
                        DocId = k.r.DocId,
                        ApplyDate = k.r.ApplyDate,
                        TroubleDes = k.r.TroubleDes,
                        RepType = k.r.RepType,
                        EndDate = k.enddate,
                        AssetNo = k.r.AssetNo,
                        AssetName = k.r.AssetName,
                        CheckerId = k.r.CheckerId,
                        PlantClass = k.r.PlantClass,
                        Contact = k.r.Contact
                        //IsQuest = isQ.Contains(k.r.DocId) ? "Y" : "N"
                    });

                });
            return rs.OrderByDescending(r => r.DocId).AsQueryable();
        }

        //public IQueryable<Repair> GetByRegId(string id)
        //{
        //    string[] s = new string[] { "?", "2" };
        //    var rf = db.RepairFlows.Where(f => s.Contains(f.Status));
        //    IQueryable<Repair> rlist;
        //    List<Repair> rs = new List<Repair>();
        //    var isQ = db.QuestAnswers.Select(q => q.Docid).Distinct();
        //    db.Repairs.Where(r => r.RegId == id)
        //        .Join(db.RepairDtls, r => r.DocId, d => d.DocId,
        //        (r, d) => new
        //        {
        //            r,
        //            enddate = d.EndDate
        //        })
        //        .Join(rf, r => r.r.DocId, f => f.DocId,
        //        (r, f) => r).ToList()
        //        .ForEach(k =>
        //        {
        //            rs.Add(new Repair
        //            {
        //                DocId = k.r.DocId,
        //                ApplyDate = k.r.ApplyDate,
        //                TroubleDes = k.r.TroubleDes,
        //                RepType = k.r.RepType,
        //                EndDate = k.enddate,
        //                AssetNo = k.r.AssetNo,
        //                AssetName = k.r.AssetName,
        //                CheckerId = k.r.CheckerId,
        //                PlantClass = k.r.PlantClass,
        //                Contact = k.r.Contact
        //                //IsQuest = isQ.Contains(k.r.DocId) ? "Y" : "N"
        //            });

        //        });

        //    return rs.AsQueryable();
        //}

        public IQueryable<Repair> GetByEngId(string id)
        {
            string[] s = new string[] { "?", "2" };
            int engId = Convert.ToInt32(id);
            //var rf = db.RepairFlows.Where(f => s.Contains(f.Status));
            var rf = db.RepairFlows.Where(f => f.Status == "?").
                Where(f => f.UserId == engId);
            IQueryable<Repair> rlist;
            List<Repair> rs = new List<Repair>();
            var isQ = db.QuestAnswers.Select(q => q.Docid).Distinct();
            db.Repairs
                .Join(db.RepairDtls, r => r.DocId, d => d.DocId,
                (r, d) => new
                {
                    r,
                    enddate = d.EndDate
                })
                .Join(rf, r => r.r.DocId, f => f.DocId,
                (r, f) => r).ToList()
                .ForEach(k =>
                {
                    rs.Add(new Repair
                    {
                        DocId = k.r.DocId,
                        ApplyDate = k.r.ApplyDate,
                        TroubleDes = k.r.TroubleDes,
                        RepType = k.r.RepType,
                        EndDate = k.enddate,
                        AssetNo = k.r.AssetNo,
                        AssetName = k.r.AssetName,
                        CheckerId = k.r.CheckerId,
                        PlantClass = k.r.PlantClass,
                        Contact = k.r.Contact
                        //IsQuest = isQ.Contains(k.r.DocId) ? "Y" : "N"
                    });

                });

            return rs.AsQueryable();
        }

        public IQueryable<Repair> GetByDeviceNo(string id)
        {
            string[] s = new string[] { "?", "2" };
            //var rf = db.RepairFlows.Where(f => s.Contains(f.Status));
            var rf = db.RepairFlows.Where(f => f.Status == "?");
            IQueryable<Repair> rlist;
            List<Repair> rs = new List<Repair>();
            var isQ = db.QuestAnswers.Select(q => q.Docid).Distinct();
            db.Repairs.Where(r => r.AssetNo == id)
                .Join(db.RepairDtls, r => r.DocId, d => d.DocId,
                (r, d) => new
                {
                    r,
                    enddate = d.EndDate
                })
                .Join(rf, r => r.r.DocId, f => f.DocId,
                (r, f) => r).ToList()
                .ForEach(k =>
                {
                    rs.Add(new Repair
                    {
                        DocId = k.r.DocId,
                        ApplyDate = k.r.ApplyDate,
                        TroubleDes = k.r.TroubleDes,
                        RepType = k.r.RepType,
                        EndDate = k.enddate,
                        AssetNo = k.r.AssetNo,
                        AssetName = k.r.AssetName,
                        CheckerId = k.r.CheckerId,
                        PlantClass = k.r.PlantClass,
                        Contact = k.r.Contact
                        //IsQuest = isQ.Contains(k.r.DocId) ? "Y" : "N"
                    });

                });
            //
            return rs.AsQueryable();
        }

        // GET: api/Repairs/5
        [ResponseType(typeof(Repair))]
        public async Task<IHttpActionResult> GetRepair(string id)
        {
            Repair repair = await db.Repairs.FindAsync(id);
            if (repair == null)
            {
                return NotFound();
            }

            return Ok(repair);
        }

        // PUT: api/Repairs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRepair(string id, Repair repair)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != repair.DocId)
            {
                return BadRequest();
            }

            db.Entry(repair).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RepairExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Repairs
        [ResponseType(typeof(Repair))]
        public async Task<IHttpActionResult> PostRepair(Repair repair)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //repair.DocId = MakeDocid();
            Asset at = db.Assets.Find(repair.AssetNo);
            if (at != null)
                repair.AssetName = at.Cname;
            //repair.ApplyDate = DateTime.Now;
            db.Repairs.Add(repair);
            //
            RepairDtl dtl = new RepairDtl();
            dtl.DocId = repair.DocId;
            db.RepairDtls.Add(dtl);
            //
            RepairFlow rf = new RepairFlow();
            rf.DocId = repair.DocId;
            rf.StepId = 1;
            rf.UserId = repair.UserId;
            rf.Status = "1";
            //rf.Role = Roles.GetRolesForUser().FirstOrDefault();
            rf.Rtp = repair.UserId;
            rf.Rdt = null;
            rf.Rtt = DateTime.Now;
            rf.Cls = "申請者";
            db.RepairFlows.Add(rf);
            //
            rf = new RepairFlow();
            rf.DocId = repair.DocId;
            rf.StepId = 2;
            rf.UserId = at == null ? repair.UserId : at.EngId;
            rf.Status = "?";
            //rf.Role = Roles.GetRolesForUser(u.UserName).FirstOrDefault();
            rf.Rtp = null;
            rf.Rdt = null;
            rf.Rtt = DateTime.Now;
            rf.Cls = "設備工程師";
            db.RepairFlows.Add(rf);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RepairExists(repair.DocId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = repair.DocId }, repair);
        }

        // DELETE: api/Repairs/5
        [ResponseType(typeof(Repair))]
        public async Task<IHttpActionResult> DeleteRepair(string id)
        {
            Repair repair = await db.Repairs.FindAsync(id);
            if (repair == null)
            {
                return NotFound();
            }

            db.Repairs.Remove(repair);
            await db.SaveChangesAsync();

            return Ok(repair);
        }
        [HttpPost]
        public IHttpActionResult uploadRepairList([FromBody]List<Repair> value)
        {
            string msg = "";
            try
            {
                foreach (Repair r in value)
                {
                    //r.DocId = new DocIdFactoriesController().GetDocId("1");
                    r.DocId = MakeDocid();
                    //
                    Asset at = db.Assets.Find(r.AssetNo);
                    if (at != null)
                        r.AssetName = at.Cname;
                    db.Repairs.Add(r);
                    //
                    RepairDtl dtl = new RepairDtl();
                    dtl.DocId = r.DocId;
                    db.RepairDtls.Add(dtl);
                    //
                    RepairFlow rf = new RepairFlow();
                    rf.DocId = r.DocId;
                    rf.StepId = 1;
                    rf.UserId = r.UserId;
                    rf.Status = "1";
                    //rf.Role = Roles.GetRolesForUser().FirstOrDefault();
                    rf.Rtp = r.UserId;
                    rf.Rdt = null;
                    rf.Rtt = DateTime.Now;
                    rf.Cls = "申請者";
                    db.RepairFlows.Add(rf);
                    //
                    rf = new RepairFlow();
                    rf.DocId = r.DocId;
                    rf.StepId = 2;
                    rf.UserId = at == null ? r.UserId : at.EngId;
                    rf.Status = "?";
                    //rf.Role = Roles.GetRolesForUser(u.UserName).FirstOrDefault();
                    rf.Rtp = null;
                    rf.Rdt = null;
                    rf.Rtt = DateTime.Now;
                    rf.Cls = "設備工程師";
                    db.RepairFlows.Add(rf);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateException e)
                    {
                        msg += e.Message;
                    }
                }
            }
            catch (Exception e)
            {
                msg += e.Message;
            }

            return Ok(msg);
        }
        private string MakeDocid()
        {
            string s = db.Repairs.Select(r => r.DocId).Max();
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RepairExists(string id)
        {
            return db.Repairs.Count(e => e.DocId == id) > 0;
        }
    }
}