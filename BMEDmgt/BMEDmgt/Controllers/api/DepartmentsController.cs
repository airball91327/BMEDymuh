using BMEDmgt.Areas.MedEngMgt.Models;
using BMEDmgt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BMEDmgt.Controllers.api
{
    public class DepartmentsController : ApiController
    {
        private BMEDcontext db = new BMEDcontext();

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }
        public IQueryable<Department> GetDptsByKeyname(string keyname)
        {
            IQueryable<Department> dpts = null;
            if (!string.IsNullOrEmpty(keyname))
            {
                dpts = db.Departments.Where(d => d.Name_C.Contains(keyname))
                    .Union(db.Departments.Where(d => d.DptId.Contains(keyname)));
            }
            return dpts;
        }
        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}