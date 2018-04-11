using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockWebApplication45.Models
{
    public class StockEntity
    {
        public string Date { get; set; }
        public String Code { get; set; }
        public Decimal BuyPrice { get; set; }
        public Decimal LastPrice { get; set; }
        public Decimal? ReturnRate { get; set; }
        public string SaleDate { get; set; }
        public Int32 Category { get; set; }
        public String Status { get; set; }
        public int Class { get; set; }

    }
}