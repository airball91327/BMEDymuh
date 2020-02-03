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
using System.Web.Http.Results;
using Newtonsoft.Json;
using System.Web.Mvc;
using BMEDmgt.Models;
using BMEDmgt.Areas.MedEngMgt.Models;

namespace BMEDmgt.Controllers.api
{
    public class AssetsController : ApiController
    {
        private BMEDcontext db = new BMEDcontext();

        // GET: api/Assets
        public IQueryable<Asset> GetAssets()
        {
            return db.Assets;
        }
        public IQueryable<Asset> GetAssetsByKeyname(string keyname)
        {
            IQueryable<Asset> assets = null;
            if (!string.IsNullOrEmpty(keyname))
            {
                assets = db.Assets.Where(a => a.Cname.Contains(keyname))
                    .Union(db.Assets.Where(a => a.AssetNo.Contains(keyname)));
            }
            return assets;
        }
        // GET: api/Assets/5
        [ResponseType(typeof(Asset))]
        public async Task<IHttpActionResult> GetAsset(string id)
        {
            Asset asset = await db.Assets.FindAsync(id);
            if (asset == null)
            {
                asset = new Asset {
                    AssetNo = id,
                    Cname = "test"
                };
                
                //return NotFound();
                return Ok(asset);
            }

            return Ok(asset);
        }

        // PUT: api/Assets/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAsset(string id, Asset asset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != asset.AssetNo)
            {
                return BadRequest();
            }

            db.Entry(asset).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetExists(id))
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

        // POST: api/Assets
        [ResponseType(typeof(Asset))]
        public async Task<IHttpActionResult> PostAsset(Asset asset)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Assets.Add(asset);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AssetExists(asset.AssetNo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = asset.AssetNo }, asset);
        }

        // DELETE: api/Assets/5
        [ResponseType(typeof(Asset))]
        public async Task<IHttpActionResult> DeleteAsset(string id)
        {
            Asset asset = await db.Assets.FindAsync(id);
            if (asset == null)
            {
                return NotFound();
            }

            db.Assets.Remove(asset);
            await db.SaveChangesAsync();

            return Ok(asset);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AssetExists(string id)
        {
            return db.Assets.Count(e => e.AssetNo == id) > 0;
        }
    }
}