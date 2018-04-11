using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace TestStockAlgorithmConsoleApplication
{
    class TestStock
    {
        //internal static readonly string Code = "sh000802";
        //internal static string StartDate { get; set; } = "20041008";
        internal static readonly decimal BranchCount = int.Parse(ConfigurationManager.AppSettings["BranchCount"]);
        internal static readonly decimal PriceOffset = decimal.Parse(ConfigurationManager.AppSettings["PriceOffset"]);
        internal static readonly decimal HeapOffset = decimal.Parse(ConfigurationManager.AppSettings["HeapOffset"]);
        private static decimal BasePrice = default(decimal);
        private decimal PriceTick = default(decimal);
        private DateTimeOffset startRunningTime = default(DateTimeOffset);
        internal static int DailyBuyCount { get; set; }
        internal static int DailySaleCount { get; set; }
        internal static int TotalBuyCount { get; set; }
        internal static int TotalSaleCount { get; set; }
        static string[] stockList = ConfigurationManager.AppSettings["StockList"]?.Split(new[] { ',' });


        internal void ProcessTest()
        {
            //TestMethod();
            foreach (var item in stockList)
            {
                TotalBuyCount = 0;
                TotalSaleCount = 0;
                DateTimeOffset startDate;
                var fields = item?.Split(new[] { '|' });
                var code = fields[0];
                DateTimeOffset.TryParse(fields[1], out startDate);
                startRunningTime = DateTimeOffset.Now;
                TestMethod(code, startDate);
            }
        }
        private void TestMethod(string code, DateTimeOffset startDate)
        {
            Logger.Instance.LogTextMessage(code, startRunningTime, "{CurrentPrice},{HitPrice},{Action},{BasePrice},{DailyBuyCount},{DailySaleCount},{TotalBuyCount},{TotalSaleCount},{PriceDateTime},{Id}", false);
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.CommandText = $@"
                  SELECT DISTINCT  CONVERT(VARCHAR(10), [DateTime],120) as [date]
                  FROM [dbo].[Detail] with(nolock)
                  WHERE CONVERT(VARCHAR, [DateTime],112)>'{startDate.ToString("yyyyMMdd")}' AND [Code]='{code}'
                  --GROUP BY CONVERT(VARCHAR, [DateTime],112)
                  ORDER BY [date]";
            var dateListReader = cmd.ExecuteReader();
            var dateList = new List<string>();
            while (dateListReader.Read())
            {
                dateList.Add(dateListReader.GetString(0));
            }
            dateListReader.Close();
            conn.Close();
            foreach (var date in dateList)
            {
                DailyBuyCount = 0;
                DailySaleCount = 0;

                conn.Open();
                cmd.CommandText = $@"
                      SELECT TOP 1 [ClosePrice]
                      FROM [Stock].[dbo].[Summary] with(nolock)
                      WHERE [DateTime] < '{date}'  AND [Code]='{code}'
                      ORDER BY [DateTime] DESC";
                var basePrice = cmd.ExecuteScalar();

                cmd.CommandText = $@"
                      SELECT [DateTime],[Price],[Id],[Volume],[Amount]
                      FROM [Stock].[dbo].[Detail] with(nolock)
                      WHERE [DateTime] >= '{date}' and [DateTime] < '{DateTimeOffset.Parse(date).AddDays(1).ToString("yyyy-MM-dd")}' AND [Code]='{code}'
                      ORDER BY [DateTime]";
                //var perPrice = default(decimal);
                try
                {

                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        BasePrice = basePrice != null ? (decimal)basePrice : reader.GetDecimal(1);
                        PriceTick = BasePrice * PriceOffset / BranchCount;
                        IList<PriceItem> priceList = GetPriceList(BasePrice);
                        do
                        {
                            var dt = reader.GetDateTimeOffset(0);
                            var price = reader.GetDecimal(1);
                            var id = reader.GetGuid(2);
                            var volume = reader.GetInt32(3);
                            var amount = reader.GetDecimal(4);
                            var priceItem = new PriceItem() { Id = id, Price = price, PriceDateTime = dt, Amount = amount, Volume = volume };
                            CheckPrice(code, priceItem, ref priceList, BasePrice);
                            //Console.WriteLine($"{dt},{price}");
                            //if (perPrice <= default(decimal))
                            //{
                            //    perPrice = price;
                            //    continue;
                            //}
                        } while (reader.Read());

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
                finally
                {
                    conn.Close();
                }
                conn.Close();
            }
        }

        private PriceItem CheckPrice(string code, PriceItem priceItem, ref IList<PriceItem> priceList, decimal basePrice)
        {
            //var priceTick = basePrice / BranchCount;
            var price = priceItem.Price;
            var hitSaleItems = from p in priceList where (price > p.Price && p.Action == StockAction.Sale) orderby p.Price select p;
            var hitBuyItems = from p in priceList where (price < p.Price && p.Action == StockAction.Buy) orderby p.Price select p;
            if ((hitBuyItems == null || !hitBuyItems.Any()) && (hitSaleItems == null || !hitSaleItems.Any())) return null;
            PriceItem hitOne = null;
            //PriceItem nextItem = null;
            if (hitBuyItems.Any())
            {
                hitOne = hitBuyItems.First();
                //priceList.Remove(hitOne);
                //var newItem = new PriceItem() { Price = hitOne.Price + priceTick, Action = StockAction.Sale, Id = Guid.NewGuid() };
                //priceList.Add(newItem);
                //nextItem = priceList.Where(p => p.Price > hitOne.Price).OrderBy(p => p.Price).First();
                //nextItem.Action = StockAction.Sale;
                //Console.WriteLine($"{priceItem.PriceDateTime.ToString("yyyy-MM-dd HH:mm:ss")},Price:{price.ToString("#.000")},hitPrice{ hitOne.Price.ToString("#.000") },IsBuy:{hitOne.Action},NewONe: Price:{newItem.Price.ToString("#.000")},IsBuy:{newItem.Action}");
                //LogMessage(priceItem, hitOne, nextItem);
            }
            else if (hitSaleItems.Any())
            {
                hitOne = hitSaleItems.Last();
                //priceList.Remove(hitOne);
                //var newItem = new PriceItem() { Price = hitOne.Price - priceTick, Action = StockAction.Buy, Id = Guid.NewGuid() };
                //priceList.Add(newItem);
                //nextItem = priceList.Where(p => p.Price < hitOne.Price).OrderBy(p => p.Price).First();
                //nextItem.Action = StockAction.Buy;
            }
            if (basePrice == hitOne.Price && DailySaleCount == DailyBuyCount)
            {
                return null;
            }
            var c = hitOne.Action == StockAction.Buy ? DailyBuyCount++ : DailySaleCount++;
            c = hitOne.Action == StockAction.Buy ? TotalBuyCount++ : TotalSaleCount++;
            Console.WriteLine($"{code},{priceItem.PriceDateTime.ToString("yyyy-MM-dd HH:mm:ss")},{price.ToString("#.000")},{ hitOne.Price.ToString("#.000") },{hitOne.Action},{DailyBuyCount},{DailySaleCount}");
            Logger.Instance.LogTextMessage(code, startRunningTime, $"{priceItem.Price.ToString("#.000")},{hitOne.Price.ToString("#.000")},{hitOne.Action},{basePrice.ToString("#.000")},{DailyBuyCount},{DailySaleCount},{TotalBuyCount},{TotalSaleCount},{priceItem.PriceDateTime.ToString("yyyy-MM-dd HH:mm:ss")},{priceItem.Id}", false);
            var resultPriceItem = hitOne?.CopyObject<PriceItem>();
            //hitOne.Action = StockAction.None;
            var hitOneIndex = priceList.IndexOf(hitOne);
            var heapTick = basePrice * HeapOffset;
            for (int i = 0; i < priceList.Count; i++)
            {
                priceList[i].Action = i < hitOneIndex ? StockAction.Sale : i > hitOneIndex ? StockAction.Buy : StockAction.None;
                priceList[i].Price = hitOne.Price + (hitOneIndex - i) * PriceTick;
                if ((DailyBuyCount > DailySaleCount && i > hitOneIndex) || (DailySaleCount > DailyBuyCount && i < hitOneIndex))
                {
                    priceList[i].Price += (hitOneIndex - i) * heapTick;
                }
                if ((DailyBuyCount > DailySaleCount && i < hitOneIndex) || (DailySaleCount > DailyBuyCount && i > hitOneIndex))
                {
                    priceList[i].Price += (hitOneIndex - i) * heapTick * (DailyBuyCount - DailySaleCount);
                }
            }
            return resultPriceItem;
        }

        internal static IList<PriceItem> GetPriceList(decimal basePrice)
        {
            IList<PriceItem> priceList = new List<PriceItem>();
            priceList.Add(new PriceItem() { Price = basePrice, Action = StockAction.None });
            for (int i = 1; i <= BranchCount; i++)
            {
                priceList.Add(new PriceItem() { Price = basePrice * (1m + PriceOffset / BranchCount * i), Action = StockAction.Sale });
                priceList.Add(new PriceItem() { Price = basePrice * (1m - PriceOffset / BranchCount * i), Action = StockAction.Buy });
            }
            return priceList.OrderByDescending(p => p.Price).ToList<PriceItem>();
        }
        internal static IList<string> GetDateList(string startDate, string code)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.CommandText = $@"
                  SELECT DISTINCT  CONVERT(VARCHAR, [DateTime],112) as [date]
                  FROM [dbo].[Detail] with(nolock)
                  WHERE CONVERT(VARCHAR, [DateTime],112)>='{startDate}' AND [Code]='{code}'
                  --GROUP BY CONVERT(VARCHAR, [DateTime],112)
                  ORDER BY [date]";
            var dateListReader = cmd.ExecuteReader();
            var dateList = new List<string>();
            while (dateListReader.Read())
            {
                dateList.Add(dateListReader.GetString(0));
            }
            dateListReader.Close();
            conn.Close();
            return dateList;
        }
        internal static decimal GetOpenPrice(string date, string code)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.CommandText = $@"
                      SELECT [OpenPrice]
                      FROM [Stock].[dbo].[Summary] with(nolock)
                      WHERE CONVERT(VARCHAR, [DateTime],112) = '{date}'  AND [Code]='{code}'";
            var oPenPrice = (decimal)cmd.ExecuteScalar();
            conn.Close();
            return oPenPrice;
        }
        internal static IList<PriceItem> GetPriceItemList(string date, string code)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.CommandText = $@"
                      SELECT [DateTime],[Price],[Id],[Volume],[Amount]
                      FROM [Stock].[dbo].[Detail] with(nolock)
                      WHERE CONVERT(VARCHAR, [DateTime],112) = '{date}'  AND [Code]='{code}'
                      ORDER BY [DateTime]";
            var priceList = new List<PriceItem>();
            try
            {

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var dt = reader.GetDateTimeOffset(0);
                    var price = reader.GetDecimal(1);
                    var id = reader.GetGuid(2);
                    var volume = reader.GetInt32(3);
                    var amount = reader.GetDecimal(4);
                    var priceItem = new PriceItem() { Id = id, Price = price, PriceDateTime = dt, Amount = amount, Volume = volume };
                    priceList.Add(priceItem);
                }
            }
            finally
            {
                conn.Close();
            }
            return priceList;
        }
    }

    [Serializable]
    internal class PriceItem
    {
        internal Guid Id { get; set; } = Guid.NewGuid();
        internal decimal Price { get; set; }
        internal StockAction Action { get; set; }
        internal DateTimeOffset PriceDateTime { get; set; }
        internal decimal Amount { get; set; }
        internal int Volume { get; set; }
    }
    internal enum StockAction
    {
        Buy = 1,
        Sale = -1,
        None = 0
    }
    public static class ObjectExtension

    {
        public static T CopyObject<T>(this object objSource)

        {

            using (MemoryStream stream = new MemoryStream())

            {

                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(stream, objSource);

                stream.Position = 0;

                return (T)formatter.Deserialize(stream);

            }

        }
    }
}
