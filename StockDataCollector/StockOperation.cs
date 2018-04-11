using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Utilities;
using System.Xml.Linq;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace StockDataCollector
{
    class StockOperation
    {
        public void ProcessStockInfo()
        {
            var step = 9m / Constants.BranchNumber;
            var branchcount = ((int)(Constants.StockList.Length / step)) + 1;
            var stockListArray = new string[branchcount];
            for (int i = 0; i < branchcount; i++)
            {
                var index = Constants.StockList.IndexOf(',', i * Constants.BranchNumber);
                step = index + step > Constants.StockList.Length ? Constants.StockList.Length - index : step;
                stockListArray[i] = Constants.StockList.Substring(index, (int)step).TrimEnd(new[] { ',' });
            }
            Parallel.ForEach(stockListArray, stockList =>
            {
                WriteLog($"{nameof(ProcessStockInfo)} begin");
                DateTime tempDateTime = DateTime.Now;
                DateTime beginDateTime = DateTime.Now;
                var stockContent = GetStockContent(Constants.StockList);
                WriteLog($"{nameof(GetStockContent)} {(DateTime.Now - tempDateTime).GetIntTotalMilliseconds()}ms");
                tempDateTime = DateTime.Now;
                IEnumerable<StockInfo> stockInfos = ParseStockContent(stockContent);
                var stockXMLContent = GetStockXMLContent(stockInfos);
                WriteLog($"{nameof(ParseStockContent)} {(DateTime.Now - tempDateTime).GetIntTotalMilliseconds()}ms");
                tempDateTime = DateTime.Now;
                SaveStockContent(stockXMLContent);
                WriteLog($"{nameof(SaveStockContent)} {(DateTime.Now - tempDateTime).GetIntTotalMilliseconds()}ms");
                WriteLog($"{nameof(ProcessStockInfo)} {(DateTime.Now - beginDateTime).GetIntTotalMilliseconds()}ms end");
            });
        }

        private string GetStockXMLContent(IEnumerable<StockInfo> stockInfos)
        {
            if (stockInfos == null || !stockInfos.Any()) return null;
            var xmlBiulder = new StringBuilder("<StockInfos>");
            foreach (var stockInfo in stockInfos)
            {
                xmlBiulder.Append(stockInfo.ToXml());

            }
            xmlBiulder.Append($"{Environment.NewLine}</StockInfos>");
            return xmlBiulder.ToString();

            //var xElement = new XElement("StockInfos");
            //foreach (var stockInfo in stockInfos)
            //{
            //    xElement.Add(XElement.Load(new StringReader(stockInfo.ToXml())));

            //}
            //return xElement.ToString();
        }

        private string GetStockContent(string stockList)
        {
            if (UseTestData)
            {
                return @"
            var hq_str_sz399001=""深证成指,10207.428,10215.477,10224.017,10236.093,10194.095,0.000,0.000,6664179360,92460631442.535,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,2017-01-12,11:09:15,00"";
            var hq_str_sz399101=""中小板综,11471.972,11484.106,11478.021,11505.275,11452.197,0.000,0.000,2299543800,37087784525.930,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,2017-01-12,11:09:15,00"";
            var hq_str_sz399102=""创业板综,2571.377,2573.661,2580.089,2583.408,2571.377,0.000,0.000,1067230131,23844892314.890,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,0,0.000,2017-01-12,11:09:15,00"";
            var hq_str_sh000001=""上证指数,3133.6015,3136.7535,3143.8976,3144.7485,3132.7004,0,0,66975977,73216631849,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2017-01-12,11:09:08,00"";
                            ";
            }

            if (string.IsNullOrEmpty(stockList)) return null;

            string stockAddress = Constants.StockAddress.FormatInvariantCulture(stockList);
            WebClient client = new WebClient();
            //var beginDateTime = DateTime.Now;
            var stockContent = client.DownloadString(stockAddress);
            //var endDateTime = DateTime.Now;
            //var milliseconds = (endDateTime - beginDateTime).GetIntTotalMilliseconds();
            //WriteLog($"{nameof(GetStockContent)} {milliseconds}ms");
            return stockContent;
        }

        private IEnumerable<StockInfo> ParseStockContent(string stockContent)
        {
            if (string.IsNullOrEmpty(stockContent))
            {
                yield return null;
                yield break;
            }

            var stockItmes = stockContent.Trim().Replace("\r", "").Replace("\n", "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in stockItmes)
            {
                var stockParts = item.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (stockParts.Length == 2)
                {
                    var stockCode = stockParts[0].Right(8);
                    var stockFields = stockParts[1].Replace("\"", "").Trim().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime hostTime = DateTime.TryParse($"{stockFields[30]} {stockFields[31]}", out hostTime) ? hostTime : default(DateTime);
                    var stockInfo = new StockInfo()
                    {
                        //0：”大秦铁路”，股票名字；
                        //1：”27.55″，今日开盘价；
                        //2：”27.25″，昨日收盘价；
                        //3：”26.91″，当前价格；
                        //4：”27.55″，今日最高价；
                        //5：”26.20″，今日最低价；
                        //6：”26.91″，竞买价，即“买一”报价；
                        //7：”26.92″，竞卖价，即“卖一”报价；
                        //8：”22114263″，成交的股票数，由于股票交易以一百股为基本单位，所以在使用时，通常把该值除以一百；
                        //9：”589824680″，成交金额，单位为“元”，为了一目了然，通常以“万元”为成交金额的单位，所以通常把该值除以一万；
                        //10：”4695″，“买一”申请4695股，即47手；
                        //11：”26.91″，“买一”报价；
                        //12：”57590″，“买二”
                        //13：”26.90″，“买二”
                        //14：”14700″，“买三”
                        //15：”26.89″，“买三”
                        //16：”14300″，“买四”
                        //17：”26.88″，“买四”
                        //18：”15100″，“买五”
                        //19：”26.87″，“买五”
                        //20：”3100″，“卖一”申报3100股，即31手；
                        //21：”26.92″，“卖一”报价
                        //(22, 23), (24, 25), (26,27), (28, 29)分别为“卖二”至“卖四的情况”
                        //30：”2008 - 01 - 11″，日期；
                        //31：”15:05:32″，时间；

                        StockCode = stockCode,
                        StockName = stockFields[0],
                        OpenPrice = stockFields[1].ToDecimal(),
                        ClosePrice = stockFields[2].ToDecimal(),
                        LastPrice = stockFields[3].ToDecimal(),
                        HighestPrice = stockFields[4].ToDecimal(),
                        LowestPrice = stockFields[5].ToDecimal(),
                        BuyPrice = stockFields[6].ToDecimal(),
                        SalePrice = stockFields[7].ToDecimal(),
                        Volume = stockFields[8].ToInt(),
                        Turnover = stockFields[9].ToDecimal(),
                        BuyVolume1 = stockFields[10].ToInt(),
                        BuyPrice1 = stockFields[11].ToDecimal(),
                        BuyVolume2 = stockFields[12].ToInt(),
                        BuyPrice2 = stockFields[13].ToDecimal(),
                        BuyVolume3 = stockFields[14].ToInt(),
                        BuyPrice3 = stockFields[15].ToDecimal(),
                        BuyVolume4 = stockFields[16].ToInt(),
                        BuyPrice4 = stockFields[17].ToDecimal(),
                        BuyVolume5 = stockFields[18].ToInt(),
                        BuyPrice5 = stockFields[19].ToDecimal(),
                        SaleVolume1 = stockFields[20].ToInt(),
                        SalePrice1 = stockFields[21].ToDecimal(),
                        SaleVolume2 = stockFields[22].ToInt(),
                        SalePrice2 = stockFields[23].ToDecimal(),
                        SaleVolume3 = stockFields[24].ToInt(),
                        SalePrice3 = stockFields[25].ToDecimal(),
                        SaleVolume4 = stockFields[26].ToInt(),
                        SalePrice4 = stockFields[27].ToDecimal(),
                        SaleVolume5 = stockFields[28].ToInt(),
                        SalePrice5 = stockFields[29].ToDecimal(),
                        HostTime = hostTime
                    };
                    //WriteConsoleLog($"{stockCode} Parsed");
                    yield return stockInfo;
                }
            }
        }

        private static readonly SqlConnection conn = new SqlConnection(Constants.StockConnectionString);

        private void SaveStockContent(string stockXMLContent)
        {
            if (string.IsNullOrEmpty(stockXMLContent)) return;
            try
            {
                SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "[usp_SaveStockContent]";   //sql语句
                cmd.Parameters.Add(new SqlParameter("@stockContentXml", SqlDbType.Xml) { Value = stockXMLContent });
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        private void WriteLog(string message)
        {
            //Task.Run(() =>
            //{
            Logger.Instance.LogTextMessage(message);
            //});
        }

        private void WriteConsoleLog(string message)
        {
            //Task.Run(() =>
            //{
            Logger.Instance.LogConsoleMessage(message);
            //});
        }

        public bool UseTestData { get; set; } = false;
    }
}
