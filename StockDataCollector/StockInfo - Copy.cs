using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace StockDataCollector
{
    public class StockInfo
    {
        public string StockCode { get; set; }
        public string StockName { get; set; }
        public decimal OpenPrice { get; set; } = default(decimal);
        public decimal ClosePrice { get; set; } = default(decimal);
        public decimal LastPrice { get; set; } = default(decimal);
        public decimal LowestPrice { get; set; } = default(decimal);
        public decimal HighestPrice { get; set; } = default(decimal);
        public decimal BuyPrice { get; set; } = default(decimal);
        public decimal SalePrice { get; set; } = default(decimal);
        public int Volume { get; set; } = default(int);
        public decimal Turnover { get; set; } = default(decimal);
        public DateTime HostTime { get; set; } = default(DateTime);
        public decimal BuyPrice1 { get; set; } = default(decimal);
        public decimal BuyPrice2 { get; set; } = default(decimal);
        public decimal BuyPrice3 { get; set; } = default(decimal);
        public decimal BuyPrice4 { get; set; } = default(decimal);
        public decimal BuyPrice5 { get; set; } = default(decimal);
        public int BuyVolume1 { get; set; } = default(int);
        public int BuyVolume2 { get; set; } = default(int);
        public int BuyVolume3 { get; set; } = default(int);
        public int BuyVolume4 { get; set; } = default(int);
        public int BuyVolume5 { get; set; } = default(int);
        public decimal SalePrice1 { get; set; } = default(decimal);
        public decimal SalePrice2 { get; set; } = default(decimal);
        public decimal SalePrice3 { get; set; } = default(decimal);
        public decimal SalePrice4 { get; set; } = default(decimal);
        public decimal SalePrice5 { get; set; } = default(decimal);
        public int SaleVolume1 { get; set; } = default(int);
        public int SaleVolume2 { get; set; } = default(int);
        public int SaleVolume3 { get; set; } = default(int);
        public int SaleVolume4 { get; set; } = default(int);
        public int SaleVolume5 { get; set; } = default(int);

        public string ToXml()
        {
            var contentBuilder = new StringBuilder();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var serializer = new XmlSerializer(typeof(StockInfo));
            using (var writer = new StringWriter(contentBuilder))
            {
                serializer.Serialize(writer, this, ns);
            }
            return contentBuilder == null || contentBuilder.ToString().IsNullOrWhiteSpace() ? null : contentBuilder.ToString();
        }

    }
}
