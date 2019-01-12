using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json.Linq;
using QnyWeb.Models;
using QnyWeb.Utilities;
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
        [HttpGet]
        [Authorize(Roles = "Inputer,Admin")]
        public IEnumerable<ReceivingNoteItemView> GetRnItems(string cDate = null)
        {
            var date = string.IsNullOrWhiteSpace(cDate) ? DateTime.Now.Date : DateTime.Parse(cDate);
            var dbContext = new ShQnyEntities();
            var rnItems = dbContext.ReceivingNoteItemViews.Where(item => item.cDate.Value == date);
            return rnItems.OrderBy(rn => rn.poNo).ThenBy(rn => rn.skuId);
        }
        //[HttpGet]
        //[Authorize(Roles = "Inputer,Admin")]
        //public IOrderedQueryable<IGrouping<string, ReceivingNoteItemView>> GetRnItems(string cDate = null)
        //{
        //    var date = string.IsNullOrWhiteSpace(cDate) ? DateTime.Now.Date : DateTime.Parse(cDate);
        //    var dbContext = new ShQnyEntities();
        //    var rnItems = dbContext.ReceivingNoteItemViews.Where(item => item.cDate.Value == date);
        //    var rnItemGroups = from rn in rnItems group rn by rn.poNo into poGroup orderby poGroup.Key select poGroup;
        //    return rnItemGroups;
        //}
        //[HttpGet]
        //[Authorize(Roles = "Inputer,Admin")]
        //public IDictionary<string, List<ReceivingNoteItemView>> GetRnItems(string cDate = null)
        //{
        //    var date = string.IsNullOrWhiteSpace(cDate) ? DateTime.Now.Date : DateTime.Parse(cDate);
        //    var dbContext = new ShQnyEntities();
        //    var rnItems = dbContext.ReceivingNoteItemViews.Where(item => item.cDate.Value == date).OrderBy(rn => rn.poNo).ThenBy(rn => rn.skuId).ToList();
        //    //group by后,两条相同pono,skuid的记录有不同的rnId,分组后两条记录具有相同的rnid
        //    //找到原因为ReceivingNoteItemView中pono,skuid为 key, 不唯一
        //    var rnItemGroups = from rn in rnItems group rn by rn.poNo into poGroup orderby poGroup.Key select poGroup;
        //    var rnItemsDictionary = rnItemGroups.ToDictionary(rn => rn.Key, rn => rn.ToList());
        //    return rnItemsDictionary;
        //}
        [HttpPost]
        [Authorize(Roles = "Inputer,Admin")]
        public IHttpActionResult PostRnItems(JObject payload)
        {
            var rns = ((JObject)payload.GetValue("payload")).GetValue("rns");
            var deletedRns = ((JObject)payload.GetValue("payload")).GetValue("deletedRns");
            if (rns != null && rns.Count() > 0)
            {
                var updateRnItemList = new List<ReceivingNote>();
                var insertRnItemList = new List<ReceivingNote>();
                //var rnsDictionary = rns.ToObject<IDictionary<string, List<ReceivingNoteItemView>>>();
                var rnList = rns.ToObject<List<ReceivingNoteItemView>>();
                foreach (var rn in rnList)
                {
                    var r = new ReceivingNote();
                    var propertyInfos = r.GetType().GetProperties();
                    foreach (var propertyInfo in propertyInfos)
                    {
                        var itemPropertyInfo = rn.GetType().GetProperty(propertyInfo.Name);
                        if (itemPropertyInfo != null)
                        {
                            propertyInfo.SetValue(r, itemPropertyInfo.GetValue(rn), null);
                        }
                    }
                    if (r.RnId == Guid.Empty)
                    {
                        r.RnId = UuidHelper.NewUuid();
                        insertRnItemList.Add(r);
                    }
                    else
                    {
                        updateRnItemList.Add(r);
                    }

                }

                using (var dbContext = new ShQnyEntities())
                {
                    //dbContext.ReceivingNotes.AddRange(rnItemList.Where(rn => rn.RnId != null || rn.Quantity > 0 || rn.TotalPrice > 0 || rn.UnitPrice > 0));
                    //var updates = updateRnItemList.Where(rn => rn.RnId != null && rn.RnId != Guid.Empty);
                    foreach (var item in updateRnItemList)
                    {
                        var original = dbContext.ReceivingNotes.Where(rn => rn.RnId == item.RnId).First();
                        var originalPropertyInfos = original.GetType().GetProperties();
                        foreach (var propertyInfo in originalPropertyInfos)
                        {
                            var itemPropertyInfo = item.GetType().GetProperty(propertyInfo.Name);
                            propertyInfo.SetValue(original, itemPropertyInfo.GetValue(item), null);
                        }
                    }
                    if (deletedRns != null && deletedRns.Count() > 0)
                    {
                        var deletedList = deletedRns.ToObject<List<Guid>>();
                        foreach (var item in deletedList)
                        {
                            var rnItem = dbContext.ReceivingNotes.Where(rn => rn.RnId == item).FirstOrDefault();
                            dbContext.ReceivingNotes.Remove(rnItem);
                        }
                    }
                    dbContext.ReceivingNotes.AddRange(insertRnItemList);
                    var rows = dbContext.SaveChanges();
                }
            }
            return Ok("数据保存成功!");
        }
        [HttpPost]
        [Authorize(Roles = "Inputer,Admin")]
        public IDictionary<int, List<KeyValuePair<int, string>>> GetSupplierOptions(JObject payload)
        {
            var skuIds = payload.GetValue("payload");
            var skuOptions = new List<KeyValuePair<int, string>>();
            if (skuIds != null && skuIds.Count() > 0)
            {
                var skuIdArray = skuIds.ToObject<List<int>>();
                var skuIdTable = string.Empty;
                for (var i = 0; i < skuIdArray.Count; i++)
                {
                    if (i == 0)
                    {
                        skuIdTable = $"select {skuIdArray[i]} skuId";
                    }
                    else
                    {
                        skuIdTable += $" union select {skuIdArray[i]}";
                    }
                }
                using (var dbContext = new ShQnyEntities())
                {
                    using (var connection = dbContext.Database.Connection)
                    {
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = $@"
select s.skuId,r.Supplier 
from ({skuIdTable}) as s 
left join ReceivingNote r 
on s.skuId=r.skuId
group by s.skuId,r.Supplier
order by max(r.RnId) desc
";
                        connection.Open();
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var key = reader.GetInt32(0);
                            var value = reader.IsDBNull(1) ? null : reader.GetString(1);
                            skuOptions.Add(new KeyValuePair<int, string>(key, value));
                        }
                        connection.Close();
                    }
                }
            }
            return skuOptions.GroupBy(op => op.Key).ToDictionary(op => op.Key, op => op.ToList());
        }
    }
}
