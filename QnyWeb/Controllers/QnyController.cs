using QnyWeb.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QnyWeb.Controllers
{
    public class QnyController : ApiController
    {
        #region Test Api
        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public decimal Price { get; set; }
        }

        Product[] products = new Product[]
        {
            new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };

        public IEnumerable<Product> GetAllProducts()
        {
            return products;
        }
        #endregion
        public IEnumerable<Status> GetStatusList()
        {
            //IEnumerable<Status> statusList = null;
            //using (var dbContext = new ShQnyEntities())
            //{
            //    statusList = dbContext.Status.AsEnumerable<Status>();
            //}
            //return statusList;
            return new ShQnyEntities().Status;
        }
    }
}
