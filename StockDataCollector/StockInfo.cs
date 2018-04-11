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
            return $@"
    <{nameof(StockInfo)}>
        <{nameof(StockCode)}>{StockCode}</{nameof(StockCode)}>
        <{nameof(StockName)}>{StockName}</{nameof(StockName)}>
        <{nameof(OpenPrice)}>{OpenPrice}</{nameof(OpenPrice)}>
        <{nameof(ClosePrice)}>{ClosePrice}</{nameof(ClosePrice)}>
        <{nameof(LastPrice)}>{LastPrice}</{nameof(LastPrice)}>
        <{nameof(LowestPrice)}>{LowestPrice}</{nameof(LowestPrice)}>
        <{nameof(HighestPrice)}>{HighestPrice}</{nameof(HighestPrice)}>
        <{nameof(BuyPrice)}>{BuyPrice}</{nameof(BuyPrice)}>
        <{nameof(SalePrice)}>{SalePrice}</{nameof(SalePrice)}>
        <{nameof(Volume)}>{Volume}</{nameof(Volume)}>
        <{nameof(Turnover)}>{Turnover}</{nameof(Turnover)}>
        <{nameof(HostTime)}>{HostTime}</{nameof(HostTime)}>
        <{nameof(BuyPrice1)}>{BuyPrice1}</{nameof(BuyPrice1)}>
        <{nameof(BuyPrice2)}>{BuyPrice2}</{nameof(BuyPrice2)}>
        <{nameof(BuyPrice3)}>{BuyPrice3}</{nameof(BuyPrice3)}>
        <{nameof(BuyPrice4)}>{BuyPrice4}</{nameof(BuyPrice4)}>
        <{nameof(BuyPrice5)}>{BuyPrice5}</{nameof(BuyPrice5)}>
        <{nameof(BuyVolume1)}>{BuyVolume1}</{nameof(BuyVolume1)}>
        <{nameof(BuyVolume2)}>{BuyVolume2}</{nameof(BuyVolume2)}>
        <{nameof(BuyVolume3)}>{BuyVolume3}</{nameof(BuyVolume3)}>
        <{nameof(BuyVolume4)}>{BuyVolume4}</{nameof(BuyVolume4)}>
        <{nameof(BuyVolume5)}>{BuyVolume5}</{nameof(BuyVolume5)}>
        <{nameof(SalePrice1)}>{SalePrice1}</{nameof(SalePrice1)}>
        <{nameof(SalePrice2)}>{SalePrice2}</{nameof(SalePrice2)}>
        <{nameof(SalePrice3)}>{SalePrice3}</{nameof(SalePrice3)}>
        <{nameof(SalePrice4)}>{SalePrice4}</{nameof(SalePrice4)}>
        <{nameof(SalePrice5)}>{SalePrice5}</{nameof(SalePrice5)}>
        <{nameof(SaleVolume1)}>{SaleVolume1}</{nameof(SaleVolume1)}>
        <{nameof(SaleVolume2)}>{SaleVolume2}</{nameof(SaleVolume2)}>
        <{nameof(SaleVolume3)}>{SaleVolume3}</{nameof(SaleVolume3)}>
        <{nameof(SaleVolume4)}>{SaleVolume4}</{nameof(SaleVolume4)}>
        <{nameof(SaleVolume5)}>{SaleVolume5}</{nameof(SaleVolume5)}>
    </{nameof(StockInfo)}>";
            //var contentBuilder = new StringBuilder();
            //XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //ns.Add("", "");
            //var serializer = new XmlSerializer(typeof(StockInfo));
            //using (var writer = new StringWriter(contentBuilder))
            //{
            //    serializer.Serialize(writer, this, ns);
            //}
            //return contentBuilder == null || contentBuilder.ToString().IsNullOrWhiteSpace() ? null : contentBuilder.ToString();

            //var objStockInfo = new StockInfo();
            //MemoryStream ms = new MemoryStream();
            //DataContractSerializer ser = new DataContractSerializer(typeof(StockInfo), nameof(StockInfo), "");
            //ser.WriteObject(ms, objStockInfo);
            //string xmlString = Encoding.ASCII.GetString(ms.ToArray());
            //return xmlString.Replace(@"xmlns:i=""http://www.w3.org/2001/XMLSchema-instance""", string.Empty);
        }
    }
}
