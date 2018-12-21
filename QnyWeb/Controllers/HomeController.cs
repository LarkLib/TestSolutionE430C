using QnyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QnyWeb.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;
using X.PagedList;

namespace QnyWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Temp()
        {
            return View();
        }
        [Authorize(Roles = "Sales,Admin")]
        public ActionResult PmsPoList()
        {
            var userId = User.Identity.GetUserId();
            var IsAdmin = User.IsInRole("Admin");
            var dbContext = new ShQnyEntities();
            var pois = (from poi in dbContext.AspNetUserPois where poi.UserId.Equals(userId) select poi.poiId).ToList();
            var pmsPoList = from pmsPoView in dbContext.PmsPoViews where pois.Contains(pmsPoView.poiId) || IsAdmin select pmsPoView;
            return View(pmsPoList.OrderByDescending(po => po.poNo));
        }
        [Authorize(Roles = "Sales,Admin")]
        public ActionResult PmsPoDetail(string poNo)
        {
            var userId = User.Identity.GetUserId();
            var IsAdmin = User.IsInRole("Admin");
            var dbContext = new ShQnyEntities();
            var pois = (from poi in dbContext.AspNetUserPois where poi.UserId.Equals(userId) select poi.poiId).ToList();
            var pmsPoDetail = dbContext.PmsPoDetails.Where(item => (pois.Contains(item.poiId.Value) && item.poNo.Equals(poNo)) || IsAdmin).First();
            return View(pmsPoDetail);
        }
        [HttpGet]
        [Authorize(Roles = "Inputer,Admin")]
        public ActionResult ReceivingNote(string cDate = null)
        {
            ViewBag.SaveStatus = string.Empty;
            var date = string.IsNullOrWhiteSpace(cDate) ? DateTime.Now.Date : DateTime.Parse(cDate);
            var dbContext = new ShQnyEntities();
            var rnItems = dbContext.ReceivingNoteItemViews.Where(item => item.cDate.Value == date);
            return View(rnItems);
        }
        [HttpPost]
        [Authorize(Roles = "Inputer,Admin")]
        //public ActionResult ReceivingNote(IEnumerable<ReceivingNoteItemView> model)
        public ActionResult ReceivingNote(FormCollection form)
        {
            //var type = typeof(List<ReceivingNoteItemView>);
            //object ViewModel = Activator.CreateInstance(type);
            //if (TryUpdateModel(ViewModel))
            //{
            //    // save the ViewModel
            //}
            //var ViewModel = new List<ReceivingNoteItemView>();
            //if (TryUpdateModel<List<ReceivingNoteItemView>>(ViewModel))
            //{
            //    // save the ViewModel
            //}
            ViewBag.SaveStatus = string.Empty;
            var cDate = form["cDate"];
            var operation = new Operation();
            operation.ParserFormCollection(form);
            var date = string.IsNullOrWhiteSpace(cDate) ? DateTime.Now.Date : DateTime.Parse(cDate);
            var dbContext = new ShQnyEntities();
            var rnItems = dbContext.ReceivingNoteItemViews.Where(item => item.cDate.Value == date);
            ViewBag.SaveStatus = "数据保存成功!";
            return View(rnItems);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult SmsPhoneNumber()
        {
            var shQnydbContext = new ShQnyEntities();
            var phones = shQnydbContext.PoiConfigs.OrderBy(u => u.poiid).ToList();
            ViewBag.Pois = shQnydbContext.PoiLists.ToDictionary(p => p.poiid, p => p.poiname);
            return View(phones);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult SmsPhoneNumber(FormCollection configs)
        {
            var shQnydbContext = new ShQnyEntities();
            var poiConfigs = shQnydbContext.PoiConfigs;
            foreach (var poiConfig in poiConfigs)
            {
                var key = $"item.phonenumber.{poiConfig.poiid}";
                if (configs.AllKeys.Contains(key) && !configs[key].Equals(poiConfig.phonenumber))
                {
                    poiConfig.phonenumber = configs[key];
                }
            }
            int rows = shQnydbContext.SaveChanges();
            var phones = shQnydbContext.PoiConfigs.OrderBy(u => u.poiid).ToList();
            ViewBag.Pois = shQnydbContext.PoiLists.ToDictionary(p => p.poiid, p => p.poiname);
            return View(phones);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult AccountManager()
        {
            var shQnydbContext = new ShQnyEntities();
            var userInfos = shQnydbContext.AspNetUsers.OrderByDescending(u => u.Id).ToList();
            ViewBag.Roles = shQnydbContext.AspNetRoles.ToList();
            ViewBag.Pois = shQnydbContext.PoiLists.ToList();
            return View(userInfos);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult AccountManager(FormCollection model)
        {
            var results = string.IsNullOrWhiteSpace(model["hiddenResult"]) ? Enumerable.Empty<string>() : model["hiddenResult"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            IList<AspNetUser> userInfos = null;
            var shQnydbContext = new ShQnyEntities();
            var userRoles = shQnydbContext.AspNetUserRoles;
            foreach (var ur in userRoles)
            {
                userRoles.Remove(ur);
            }
            foreach (var ur in results.Where(item => item.StartsWith("ur")))
            {
                var values = ur.Split(new[] { '_' });
                userRoles.Add(new AspNetUserRole() { UserId = values[1], RoleId = values[2] });
            }
            var userPois = shQnydbContext.AspNetUserPois;
            foreach (var up in userPois)
            {
                userPois.Remove(up);
            }
            foreach (var up in results.Where(item => item.StartsWith("up")))
            {
                var values = up.Split(new[] { '_' });
                userPois.Add(new AspNetUserPois() { UserId = values[1], poiId = int.Parse(values[2]) });
            }
            shQnydbContext.SaveChanges();
            userInfos = shQnydbContext.AspNetUsers.OrderByDescending(u => u.Id).ToList();
            ViewBag.Roles = shQnydbContext.AspNetRoles.ToList();
            ViewBag.Pois = shQnydbContext.PoiLists.ToList();
            return View(userInfos);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            //ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}