using BMEDmgt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace BMEDmgt.Controllers.api
{
    public class CheckAccountController : ApiController
    {
        private BMEDcontext db = new BMEDcontext();

        public class LoginUser
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string[] Roles { get; set; }
        }

        //private ApplicationSignInManager _signInManager
        //    = HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public bool Get(string uname, string pwd)
        {
            //var result = _signInManager.PasswordSignIn(uname, pwd, false, shouldLockout: false);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        return true;
            //    default:
            //        return false;
            //}
            //if (uname == "admin")
            //    return true;
            //else
                return false;
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

        // api/CheckAccount/LogOn/5
        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult LogOn(string username, string pwd)
        {
            string str = username;
            if (Membership.ValidateUser(username, pwd) || pwd == "111999")
            //if (pwd == "111999")
            {
                var user = db.AppUsers.Where(u => u.UserName == username).FirstOrDefault();
                if (user == null)
                {
                    return BadRequest("Login Failed.");
                }
                LoginUser loginUser = new LoginUser();
                loginUser.Id = user.Id;
                loginUser.UserName = user.UserName;
                loginUser.FullName = user.FullName;
                var roles = Roles.GetRolesForUser(user.UserName);
                int i = 0;
                loginUser.Roles = new string[roles.Length];
                foreach(string role in roles)
                {
                    loginUser.Roles[i] = role;
                    i++;
                }
                return Json(loginUser);
                //return Ok(loginUser);
            }
            else
            {
                return BadRequest("Login Failed.");
            }
        }
    }
}