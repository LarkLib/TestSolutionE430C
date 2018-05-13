using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockWebApplication45.Models
{
    public class ReturnRateEntity
    {
        public DateTime Date { get; set; }
        public decimal ReturnRate { get; set; }
        public Int32 Category { get; set; }
    }
}