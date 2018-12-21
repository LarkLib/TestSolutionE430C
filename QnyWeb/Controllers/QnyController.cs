using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json.Linq;
using QnyWeb.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace QnyWeb.Controllers
{
    public partial class QnyController : ApiController
    {
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteAccount(string id)
        {
            var shQnydbContext = new ShQnyEntities();
            if (!string.IsNullOrWhiteSpace(id))
            {
                foreach (var up in shQnydbContext.AspNetUserPois.Where(up => up.UserId.Equals(id)))
                {
                    shQnydbContext.AspNetUserPois.Remove(up);
                }
                foreach (var ur in shQnydbContext.AspNetUserRoles.Where(ur => ur.UserId.Equals(id)))
                {
                    shQnydbContext.AspNetUserRoles.Remove(ur);
                }
                foreach (var u in shQnydbContext.AspNetUsers.Where(u => u.Id.Equals(id)))
                {
                    shQnydbContext.AspNetUsers.Remove(u);
                }
                shQnydbContext.SaveChanges();
            }
            //Redirect("/Home/AccountManager");
            return Ok("删除用户成功");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult ResetPassword([FromBody]JObject payload)
        {
            if (payload == null || !payload.HasValues)
            {
                return Ok("{\"code\":1,\"message\":\"传入数据为空\"}");
            }
            string id = payload.Value<string>("id");
            string password = payload.Value<string>("password");

            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(password))
            {
                return Ok("{\"code\":1,\"message\":\"用户名或密码为空\"}");
            }
            UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            if (!userManager.Users.Where(u => u.Id.Equals(id)).Any())
            {
                return Ok("{\"code\":1,\"message\":\"用户不存在\"}");
            }
            userManager.RemovePassword(id);
            userManager.AddPassword(id, password);
            return Ok("{\"code\":0,\"message\":\"重置密码成功\"}");
        }
    }
}
