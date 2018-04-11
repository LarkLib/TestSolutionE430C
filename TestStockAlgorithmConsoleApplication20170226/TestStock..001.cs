using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStockAlgorithmConsoleApplication
{
    class TestStock
    {
        private readonly string Code = "sh000802";
        private readonly string StartDate = "20150101";
        private readonly decimal BranchCount = 10;
        private decimal OPenPrice = default(decimal);
        private decimal PriceTick = default(decimal);

        public void ProcessTest()
        {
            TestMethod();
        }
        private void TestMethod()
        {
            Logger.Instance.LogTextMessage("{priceItem.Price},{hitOne.Price},{hitOne.IsBuy},{OPenPrice},{newItem.Price},{newItem.IsBuy},{priceItem.DateTime},{priceItem.Id}", false);
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.CommandText = $@"
                  SELECT DISTINCT  CONVERT(VARCHAR, [DateTime],112) as [date]
                  FROM [dbo].[Detail] 
                  WHERE CONVERT(VARCHAR, [DateTime],112)>'{StartDate}' AND [Code]='{Code}'
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
                conn.Open();
                cmd.CommandText = $@"
                      SELECT [OpenPrice]
                      FROM [Stock].[dbo].[Summary]
                      WHERE CONVERT(VARCHAR, [DateTime],112) = '{date}'  AND [Code]='{Code}'";
                OPenPrice = (decimal)cmd.ExecuteScalar();
                PriceTick = OPenPrice * 0.1m / BranchCount;
                IList<PriceItem> priceList = GetPriceList(date, OPenPrice);

                cmd.CommandText = $@"
                      SELECT [DateTime],[Price],[Id],[Volume],[Amount]
                      FROM [Stock].[dbo].[Detail]
                      WHERE CONVERT(VARCHAR, [DateTime],112) = '{date}'  AND [Code]='{Code}'
                      ORDER BY [DateTime]";
                //var perPrice = default(decimal);
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
                        CheckPrice(priceItem, ref priceList);
                        //Console.WriteLine($"{dt},{price}");
                        //if (perPrice <= default(decimal))
                        //{
                        //    perPrice = price;
                        //    continue;
                        //}
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

        private void CheckPrice(PriceItem priceItem, ref IList<PriceItem> priceList)
        {
            var price = priceItem.Price;
            var hitSellItems = from p in priceList where (price > p.Price && !p.IsBuy) orderby p.Price select p;
            var hitBuyItems = from p in priceList where (price < p.Price && p.IsBuy) orderby p.Price select p;
            if (hitBuyItems.Any())
            {
                var hitOne = hitBuyItems.First();
                priceList.Remove(hitOne);
                var newItem = new PriceItem() { Price = hitOne.Price + PriceTick, IsBuy = !hitOne.IsBuy, Id = Guid.NewGuid() };
                priceList.Add(newItem);
                Console.WriteLine($"{priceItem.PriceDateTime.ToString("yyyy-MM-dd HH:mm:ss")},Price:{price.ToString("#.000")},hitPrice{ hitOne.Price.ToString("#.000") },IsBuy:{hitOne.IsBuy},NewONe: Price:{newItem.Price.ToString("#.000")},IsBuy:{newItem.IsBuy}");
                LogMessage(priceItem, hitOne, newItem);
            }
            else if (hitSellItems.Any())
            {
                var hitOne = hitSellItems.Last();
                priceList.Remove(hitOne);
                var newItem = new PriceItem() { Price = hitOne.Price - PriceTick, IsBuy = !hitOne.IsBuy, Id = Guid.NewGuid() };
                Console.WriteLine($"{priceItem.PriceDateTime.ToString("yyyy-MM-dd HH:mm:ss")},Price:{price.ToString("#.000")},hitPrice{ hitOne.Price.ToString("#.000") },IsBuy:{hitOne.IsBuy},NewONe: Price:{newItem.Price.ToString("#.000")},IsBuy:{newItem.IsBuy}");
                LogMessage(priceItem, hitOne, newItem);
            }
        }

        private void LogMessage(PriceItem priceItem, PriceItem hitOne, PriceItem newItem)
        {
            Logger.Instance.LogTextMessage($"{priceItem.Price.ToString("#.000")},{hitOne.Price.ToString("#.000")},{hitOne.IsBuy},{OPenPrice.ToString("#.000")},{newItem.Price.ToString("#.000")},{newItem.IsBuy},{priceItem.PriceDateTime.ToString("yyyy-MM-dd HH:mm:ss")},{priceItem.Id}", false);
        }

        private IList<PriceItem> GetPriceList(string date, decimal openPrice)
        {
            IList<PriceItem> priceList = new List<PriceItem>();
            for (int i = 1; i <= BranchCount; i++)
            {
                priceList.Add(new PriceItem() { Price = openPrice * (1m + 0.1m / BranchCount * i), IsBuy = false });
                priceList.Add(new PriceItem() { Price = openPrice * (1m - 0.1m / BranchCount * i), IsBuy = true });
            }
            return priceList;
        }
        public IList<string> GetDateList()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.CommandText = $@"
                  SELECT DISTINCT  CONVERT(VARCHAR, [DateTime],112) as [date]
                  FROM [dbo].[Detail] 
                  WHERE CONVERT(VARCHAR, [DateTime],112)>'{StartDate}' AND [Code]='{Code}'
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
        public decimal GetOpenPrice(string date)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.CommandText = $@"
                      SELECT [OpenPrice]
                      FROM [Stock].[dbo].[Summary]
                      WHERE CONVERT(VARCHAR, [DateTime],112) = '{date}'  AND [Code]='{Code}'";
            OPenPrice = (decimal)cmd.ExecuteScalar();
            conn.Close();
            return OPenPrice;
        }
        public IList<PriceItem> GetPriceItemList(string date)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.CommandText = $@"
                      SELECT [DateTime],[Price],[Id],[Volume],[Amount]
                      FROM [Stock].[dbo].[Detail]
                      WHERE CONVERT(VARCHAR, [DateTime],112) = '{date}'  AND [Code]='{Code}'
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

    class PriceItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Price { get; set; }
        public bool IsBuy { get; set; }
        public DateTimeOffset PriceDateTime { get; set; }
        public decimal Amount { get; set; }
        public int Volume { get; set; }
    }

}
