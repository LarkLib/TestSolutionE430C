using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StockWebApplication45.Utility;

namespace StockWebApplication45.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var utility = new Operation();
            var stockEntities = utility.GetStockEntityList();
            return View(stockEntities);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Select Stock by policy.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "";

            return View();
        }
    }
}