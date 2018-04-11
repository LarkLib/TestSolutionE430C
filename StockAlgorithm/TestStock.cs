using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace StockAlgorithm
{
    public class TestStock
    {
        public readonly bool RemoveCompletedTransaction = true;
        public readonly int BranchCount = int.Parse(ConfigurationManager.AppSettings["BranchCount"]);
        public readonly int DaysForXDaysIncreaseRole = int.Parse(ConfigurationManager.AppSettings["DaysForXDaysIncreaseRole"]);
        public readonly int LengthForDeltaIncreaseRole = int.Parse(ConfigurationManager.AppSettings["LengthForDeltaIncreaseRole"]);
        public readonly decimal MinOffsetForDeltaIncreaseRole = decimal.Parse(ConfigurationManager.AppSettings["MinOffsetForDeltaIncreaseRole"]);
        public readonly decimal CoefficientForDeltaIncreaseRole = decimal.Parse(ConfigurationManager.AppSettings["CoefficientForDeltaIncreaseRole"]);
        public readonly decimal PriceOffset = decimal.Parse(ConfigurationManager.AppSettings["PriceOffset"]);
        public readonly decimal HeapOffset = decimal.Parse(ConfigurationManager.AppSettings["HeapOffset"]);
        public readonly decimal LimitUp = decimal.Parse(ConfigurationManager.AppSettings["LimitUp"] ?? "0.093");
        public readonly decimal LimitDown = decimal.Parse(ConfigurationManager.AppSettings["LimitDown"] ?? "0.093");
        private readonly string[] RoleList = ConfigurationManager.AppSettings["RoleList"]?.Split(new[] { ',' });

        private static readonly int Timeout = 600;
        private static readonly string DataBaseList = ConfigurationManager.AppSettings["DataBaseList"];
        private static readonly string StockList = ConfigurationManager.AppSettings["StockList"];
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["StockEntities"].ConnectionString;
        private static readonly int MaxDbThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxDbThreadCount"]);
        private static readonly int MaxCodeThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxCodeThreadCount"]);
        private static readonly int TimeStep = int.Parse(ConfigurationManager.AppSettings["TimeStep"]);
        private static readonly string ConnectionFormatString = ConfigurationManager.AppSettings["ConnectionFormatString"] as string;
        private static readonly DateTimeOffset StartDate = DateTimeOffset.Parse(ConfigurationManager.AppSettings["StartDate"]);
        public static IList<string> StockListArray
        {
            get
            {
                return string.IsNullOrEmpty(StockList) ? null : StockList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public static IList<string> DataBaseListArray
        {
            get
            {
                return string.IsNullOrEmpty(DataBaseList) ? null : DataBaseList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public void ProcessTest()
        {
            IList<BaseRole> roleList = GetRoleList(RoleList);
            foreach (var role in roleList)
            {
                var dbList = string.IsNullOrEmpty(DataBaseList) || DataBaseListArray == null || !DataBaseListArray.Any() ? GetDbList() : DataBaseListArray;
                Parallel.ForEach(dbList, new ParallelOptions() { MaxDegreeOfParallelism = MaxDbThreadCount }, db =>
                {
                    var codeList = GetCodeList(db);
                    if (StockListArray != null && StockListArray.Any())
                    {
                        codeList = codeList.Where(item => StockListArray.Contains(item)).ToList();
                    }

                    //foreach (var code in codeList)
                    Parallel.ForEach(codeList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, code =>
                    {
                        if (Directory.GetFiles(Logger.Instance.filePath, $"{code}_{role.GetType().Name}_*.csv").Any())
                        {
                            return;
                        }
                        List<PriceTransactionItem> priceTransactions = new List<PriceTransactionItem>();
                        DateTimeOffset startDate = StartDate;

                        var config = new PriceConfig()
                        {
                            StockCode = code,
                            BranchCount = BranchCount,
                            HeapOffset = HeapOffset,
                            StartDate = startDate,
                            LimitDown = LimitDown,
                            LimitUp = LimitUp,
                            PriceOffset = PriceOffset,
                            Role = role,
                            DaysForXDaysIncreaseRole = DaysForXDaysIncreaseRole,
                            LengthForDeltaIncreaseRole = LengthForDeltaIncreaseRole,
                            MinOffsetForDeltaIncreaseRole = MinOffsetForDeltaIncreaseRole,
                            CoefficientForDeltaIncreaseRole = CoefficientForDeltaIncreaseRole
                        };
                        TestMethod(db, config, priceTransactions);
                        priceTransactions = null;
                    });
                });
            }
        }
        private void TestMethod(string database, PriceConfig config, List<PriceTransactionItem> priceTransactions)
        {
            var code = config.StockCode;
            var startDate = config.StartDate;
            var startRunningTime = config.StartRunningTime;
            Logger.Instance.LogTextMessage(code, config, "{CurrentPrice},{BranchNumber},{HitPrice},{Action},{BasePrice},{TargetPrice},{DailyBuyCount},{DailySaleCount},{TotalBuyCount},{TotalSaleCount},{PriceDateTime},{PriceId},{TransactionId},RelatedId", false);
            var dateList = GetDateList(startDate, code, database);
            foreach (var date in dateList)
            {
                //if (date.CompareTo("2015-01-15") > 0) break;
                var dateTime = DateTimeOffset.Parse(date);
                var priceItemList = GetPriceItemList(date, code, database, config.Role);

                if (priceItemList.Any(p => p.PriceDateTime > dateTime))
                {
                    var basePrice = GetBasePrice(dateTime, code, database) ?? priceItemList[0].Price;
                    var priceSummary = new PriceSummaryItem() { Config = config, DetailDate = DateTimeOffset.Parse(date), BasePrice = (decimal)basePrice };
                    if (config.XDaysCountHistory.Any())
                    {
                        priceSummary.XDaysOffsetPrice = priceSummary.XDaysCountOffset * priceSummary.HeapTick;
                    }

                    foreach (var priceItem in priceItemList.Where(p => p.PriceDateTime >= dateTime))
                    {
                        var index = priceItemList.IndexOf(priceItem);
                        var deltaArray = ((List<PriceItem>)priceItemList).GetRange(index > config.LengthForDeltaIncreaseRole ? index - config.LengthForDeltaIncreaseRole : 0, index > config.LengthForDeltaIncreaseRole ? config.LengthForDeltaIncreaseRole : index).ToArray();
                        if (deltaArray.Any())
                        {
                            decimal avg = deltaArray.Average(p => p.Price);
                            decimal stdev = (decimal)Math.Sqrt(deltaArray.Sum(p => ((double)(p.Price - avg) * (double)(p.Price - avg))) / (double)config.LengthForDeltaIncreaseRole);
                            deltaArray = null;
                            priceSummary.DeltaPrice = stdev * config.CoefficientForDeltaIncreaseRole;
                        }
                        CheckPrice(priceItem, priceSummary, priceTransactions);
                    }
                }
                priceItemList = null;
                if (RemoveCompletedTransaction)
                {
                    priceTransactions.RemoveAll(p => p.DetailDateTime.Date.AddDays(1) <= dateTime.Date && p.Status == TransactionStatus.Completed);
                }
                var days = config.XDaysCountHistory.Select(p => p.DetailDateTime.Date).Distinct().ToArray();
                if (days.Length > config.DaysForXDaysIncreaseRole)
                {
                    config.XDaysCountHistory.RemoveAll(p => days[days.Length - config.DaysForXDaysIncreaseRole - 1] <= dateTime.Date);
                }
                //XmlDocument xd = new XmlDocument();
                //using (StringWriter sw = new StringWriter())
                //{
                //    XmlSerializer xz = new XmlSerializer(typeof(List<PriceTransactionItem>));
                //    xz.Serialize(sw, PriceTransactions);
                //    Console.WriteLine(sw.ToString());
                //    xd.LoadXml(sw.ToString());
                //    xd.Save("c:\\1.xml");
                //}
            }
        }
        private void CheckPrice(PriceItem priceItem, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions)
        {
            priceSummary.Config.Role.ProcessRole(priceItem, priceSummary, priceTransactions);
        }
        private IList<string> GetDbList()
        {
            var list = new List<string>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandTimeout = Timeout;
                cmd.CommandText = $"select distinct db from CodeInfo with(nolock) order by db";
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
                connection.Close();
            }
            return list;
        }
        private IList<string> GetCodeList(string db)
        {
            var list = new List<string>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandTimeout = Timeout;
                cmd.CommandText = "select distinct Code from CodeInfo  with(nolock) where db=@db";
                cmd.Parameters.Add(new SqlParameter("@db", db) { SqlDbType = SqlDbType.NVarChar, Size = 50 });
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
                connection.Close();
            }
            return list;
        }
        public IList<string> GetDateList(DateTimeOffset startDate, string code, string database)
        {
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, database)))
            {
                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = Timeout;
                connection.Open();
                cmd.CommandText = $@"
                  SELECT DISTINCT  CONVERT(VARCHAR(10), [DateTime],120) as [date]
                  FROM [Minute] with(nolock)
                  WHERE [DateTime] >'{startDate.ToString("yyyyMMdd")}' AND [Code]='{code}'
                  --GROUP BY CONVERT(VARCHAR, [DateTime],112)
                  ORDER BY [date]";
                var dateListReader = cmd.ExecuteReader();
                var dateList = new List<string>();
                while (dateListReader.Read())
                {
                    dateList.Add(dateListReader.GetString(0));
                }
                dateListReader.Close();
                connection.Close();
                return dateList;
            }
        }
        public decimal? GetBasePrice(DateTimeOffset dateTime, string code, string database)
        {
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, database)))
            {
                SqlCommand cmd = connection.CreateCommand();              //创建SqlCommand对象
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = Timeout;
                connection.Open();
                cmd.CommandText = $@"
                      SELECT TOP 1 [ClosePrice]
                      FROM [RightsOfferingInfo] with(nolock)
                      WHERE [DateTime] <= '{dateTime.AddDays(-1).Date}' AND [Code]='{code}'
                      ORDER BY [DateTime] DESC";
                var basePrice = (decimal?)cmd.ExecuteScalar();
                connection.Close();
                return basePrice;
            }
        }
        public IList<PriceItem> GetPriceItemList(string date, string code, string database, BaseRole role)
        {
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, database)))
            {
                SqlCommand cmd = connection.CreateCommand();              //创建SqlCommand对象
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = Timeout;
                connection.Open();
                var sqlxDay = $@"
                      SELECT [DateTime],[ClosePrice] [Price],[Id],[Volume],[Amount]
                      FROM 
                      (
                         SELECT TOP {LengthForDeltaIncreaseRole} [DateTime],[ClosePrice],[Id],[Volume],[Amount]
                         FROM [Minute] with(nolock)
                         WHERE [DateTime] < '{date}' and [DateTime] > '{DateTimeOffset.Parse(date).AddMonths(-1).ToString("yyyy-MM-dd")}' AND [Code]='{code}'
                         ORDER BY [DateTime] DESC
                         UNION
                         SELECT [DateTime],[ClosePrice],[Id],[Volume],[Amount]
                         FROM [Minute] with(nolock)
                         WHERE [DateTime] >= '{date}' and [DateTime] < '{DateTimeOffset.Parse(date).AddDays(1).ToString("yyyy-MM-dd")}' AND [Code]='{code}'
                      ) as PriceItems
                      ORDER BY [DateTime]";
                var sqlNormal = $@"
                        SELECT [DateTime],[ClosePrice]  [Price],[Id],[Volume],[Amount]
                        FROM [Minute] with(nolock)
                        WHERE [DateTime] >= '{date}' and [DateTime] < '{DateTimeOffset.Parse(date).AddDays(1).ToString("yyyy-MM-dd")}' AND [Code]='{code}'
                        ORDER BY [DateTime]";
                cmd.CommandText = role.GetType().Name.Equals("DailyExponentialIncreaseRole") ? sqlxDay : sqlNormal;
                var priceList = new List<PriceItem>();

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
                return priceList;
            }
        }

        public IList<BaseRole> GetRoleList(string[] roles)
        {
            var roleListObject = new List<BaseRole>();
            foreach (var role in RoleList)
            {
                roleListObject.Add(Activator.CreateInstance(Type.GetType($"{typeof(BaseRole).Namespace}.{role}")) as BaseRole);
            }
            return roleListObject;
        }
    }

    public abstract class BaseRole
    {
        public void ProcessRole(PriceItem priceItem, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions)
        {
            Execute(priceItem, priceSummary, priceTransactions);
        }

        protected bool IsBasePriceRange(decimal price, PriceSummaryItem priceSummary)
        {
            return (Math.Abs(priceSummary.BasePrice - price) < priceSummary.PriceTick && priceSummary.DailySaleCount == priceSummary.DailyBuyCount);
        }
        protected abstract KeyValuePair<decimal, decimal> GetRangePrices(PriceItem priceItem, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions);
        protected abstract void Execute(PriceItem priceItem, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions);
        protected void ProcessTransactions(PriceItem priceItem, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions)
        {
            PriceTransactionItem figOne = null;
            var processTransactions = priceTransactions.Any() ?
                priceTransactions.Where(
                    p => (
                    (
                        p.TargetPrice <= priceItem.Price && p.Action == StockAction.Buy)
                        || (p.TargetPrice >= priceItem.Price && p.Action == StockAction.Sale)
                    )
                    && p.Status == TransactionStatus.Active).ToList()
                : Enumerable.Empty<PriceTransactionItem>();
            if (processTransactions.Any())
            {
                foreach (var item in processTransactions)
                {
                    #region new PriceTransactionItem
                    figOne = new PriceTransactionItem()
                    {
                        PriceSummary = priceSummary,
                        Price = priceItem.Price,
                        Action = item.Action == StockAction.Buy ? StockAction.Sale : StockAction.Buy,
                        HitPrice = item.TargetPrice,
                        TotalBuyCount = priceSummary.Config.TotalBuyCount,
                        TotalSaleCount = priceSummary.Config.TotalSaleCount,
                        DailyBuyCount = priceSummary.DailyBuyCount,
                        DailySaleCount = priceSummary.DailySaleCount,
                        DetailDateTime = priceItem.PriceDateTime,
                        DetailId = priceItem.Id,
                        Status = TransactionStatus.Completed,
                        TargetPrice = 0m,
                        RelatedId = item.Id
                    };
                    #endregion  new PriceTransactionItem
                    AddTransaction(figOne, priceSummary, priceTransactions);
                    item.Status = TransactionStatus.Completed;
                    item.LastUpdateTime = DateTimeOffset.Now;
                }
                processTransactions = null;
                return;
            }
        }
        protected virtual void AddTransaction(PriceTransactionItem figOne, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions)
        {
            priceTransactions.Add(figOne);
            priceSummary.UpdateBuySaleCount(figOne.Action);
            WriteLog(figOne);
        }
        protected void WriteLog(PriceTransactionItem figOne)
        {
            var priceSummary = figOne.PriceSummary;
            Logger.Instance.LogTextMessage(priceSummary.Config.StockCode, priceSummary.Config, $"{figOne.Price.ToString("#.000")},{figOne.BranchNumber},{figOne.HitPrice.ToString("#.000")},{figOne.Action},{priceSummary.BasePrice.ToString("#.000")},{ figOne.TargetPrice.ToString("#.000") },{figOne.DailyBuyCount},{figOne.DailySaleCount},{figOne.TotalBuyCount},{figOne.TotalSaleCount},{figOne.DetailDateTime.ToString("yyyy-MM-dd HH:mm:ss")},{figOne.DetailId},{figOne.Id},{figOne.RelatedId}", false);
            Console.WriteLine($"{priceSummary.Config.StockCode},{priceSummary.Config.Role.GetType().Name},{figOne.DetailDateTime.ToString("yyyy-MM-dd HH:mm:ss")},{figOne.Price.ToString("#.000")},{ figOne.HitPrice.ToString("#.000") },{figOne.Action},{figOne.DailyBuyCount},{figOne.DailySaleCount},{figOne.TotalBuyCount},{figOne.TotalSaleCount}");
        }
    }

    public class DailyIncreaseRole : BaseRole
    {
        protected override void Execute(PriceItem priceItem, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions)
        {
            if (!priceSummary.IsValidPrice(priceItem.Price)) return;
            if (priceSummary.IsFirstItem && RoleSet.MinMaxRole(priceItem.Price, priceSummary.LimitDownPrice, priceSummary.LimitUpPrice, Isbetween: false))
                return;

            priceSummary.IsFirstItem = false;
            ProcessTransactions(priceItem, priceSummary, priceTransactions);

            var todayTransactions = priceTransactions?.Where(p => p.DetailDateTime.Date == priceSummary.DetailDate.Date);
            var currentBasePrice = todayTransactions.Any() ? todayTransactions.Last().Price : priceSummary.BasePrice;
            priceSummary.CurrentBasePrice = currentBasePrice;
            priceSummary.XDaysOffsetPrice = todayTransactions.Any(p => p.Action == (priceSummary.XDaysOffsetPrice > 0 ? StockAction.Sale : StockAction.Buy)) ? 0m : priceSummary.XDaysOffsetPrice;
            var rangePrice = GetRangePrices(null, priceSummary, null);
            var lowerRangePrice = rangePrice.Key;
            var upperRangePrice = rangePrice.Value;
            if (RoleSet.MinMaxRole(priceItem.Price, lowerRangePrice, upperRangePrice, Isbetween: true)
                || IsBasePriceRange(priceItem.Price, priceSummary))
            {
                return;
            }

            var branchNumber = 0;
            var action = priceItem.Price >= upperRangePrice ? StockAction.Sale : StockAction.Buy;
            if (
                //todayTransactions.Any(p => p.Status == TransactionStatus.Active && (int)p.Action == (-1) * (int)action)
                //|| 
                todayTransactions.Any(p => p.Status == TransactionStatus.Active && Math.Abs(p.TargetPrice - priceItem.Price) < priceSummary.PriceTick)//exist transaction that the less than 1 price tick
                )
            {
                return;
            }
            var tempPrice = currentBasePrice;
            var offsetPrice = Math.Abs(priceItem.Price - currentBasePrice) - priceSummary.PriceTick;
            while (offsetPrice > 0)
            {
                #region new PriceTransactionItem
                var figOne = new PriceTransactionItem()
                {
                    PriceSummary = priceSummary,
                    Price = priceItem.Price,
                    Action = action,
                    HitPrice = priceItem.Price >= upperRangePrice ? upperRangePrice : lowerRangePrice,
                    TotalBuyCount = priceSummary.Config.TotalBuyCount,
                    TotalSaleCount = priceSummary.Config.TotalSaleCount,
                    DailyBuyCount = priceSummary.DailyBuyCount,
                    DailySaleCount = priceSummary.DailySaleCount,
                    DetailDateTime = priceItem.PriceDateTime,
                    DetailId = priceItem.Id,
                    Status = TransactionStatus.Active,
                    BranchNumber = branchNumber
                };
                #endregion new PriceTransactionItem
                AddTransaction(figOne, priceSummary, priceTransactions);
                tempPrice = tempPrice - (decimal)action * (priceSummary.PriceTick + priceSummary.HeapTick);
                if (IsBasePriceRange(tempPrice, priceSummary))
                {
                    tempPrice = tempPrice - (decimal)action * (priceSummary.PriceTick + priceSummary.HeapTick);
                }
                branchNumber++;
                offsetPrice -= priceSummary.PriceTick + branchNumber * priceSummary.HeapTick;
            }
        }

        protected override KeyValuePair<decimal, decimal> GetRangePrices(PriceItem priceItem, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions)
        {
            var currentBasePrice = priceSummary.CurrentBasePrice;
            var upperRangePrice = currentBasePrice + priceSummary.PriceTick + ((priceSummary.DailySaleCount - priceSummary.DailyBuyCount) > 0 ? 1m : 0m) * priceSummary.HeapTick;
            var lowerRangePrice = currentBasePrice - priceSummary.PriceTick - ((priceSummary.DailySaleCount - priceSummary.DailyBuyCount) < 0 ? 1m : 0m) * priceSummary.HeapTick;
            return new KeyValuePair<decimal, decimal>(lowerRangePrice, upperRangePrice);
        }
    }

    public class XDaysIncreaseRole : DailyIncreaseRole
    {
        protected override KeyValuePair<decimal, decimal> GetRangePrices(PriceItem priceItem, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions)
        {
            var currentBasePrice = priceSummary.CurrentBasePrice;
            var upperRangePrice = currentBasePrice + priceSummary.PriceTick + ((priceSummary.XDaysCountOffset) > 0 ? 1m * priceSummary.HeapTick + priceSummary.XDaysOffsetPrice : 0m);
            var lowerRangePrice = currentBasePrice - priceSummary.PriceTick - ((priceSummary.XDaysCountOffset) < 0 ? 1m * priceSummary.HeapTick + priceSummary.XDaysOffsetPrice : 0m);
            return new KeyValuePair<decimal, decimal>(lowerRangePrice, upperRangePrice);
        }
        protected override void AddTransaction(PriceTransactionItem figOne, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions)
        {
            base.AddTransaction(figOne, priceSummary, priceTransactions);
            priceSummary.Config.XDaysCountHistory.Add(new CountHistoryItem(figOne.DetailDateTime, figOne.TotalBuyCount, figOne.TotalSaleCount));
        }
    }
    public class DailyExponentialIncreaseRole : DailyIncreaseRole
    {
        protected override KeyValuePair<decimal, decimal> GetRangePrices(PriceItem priceItem, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions)
        {
            var currentBasePrice = priceSummary.CurrentBasePrice;
            var upperRangePrice = currentBasePrice * (1m + priceSummary.PriceOffset + priceSummary.Config.HeapOffset);
            var lowerRangePrice = currentBasePrice / (1m + priceSummary.PriceOffset + priceSummary.Config.HeapOffset);
            return new KeyValuePair<decimal, decimal>(lowerRangePrice, upperRangePrice);
        }
    }
    public class DeltaIncreaseRole : DailyIncreaseRole
    {
        protected override KeyValuePair<decimal, decimal> GetRangePrices(PriceItem priceItem, PriceSummaryItem priceSummary, List<PriceTransactionItem> priceTransactions)
        {
            var currentBasePrice = priceSummary.CurrentBasePrice;
            var minDeltaPrice = priceSummary.BasePrice * priceSummary.Config.MinOffsetForDeltaIncreaseRole;
            var offsetPrice = priceSummary.DeltaPrice > minDeltaPrice ? priceSummary.DeltaPrice : minDeltaPrice;
            var upperRangePrice = currentBasePrice + offsetPrice;//priceSummary.PriceTick + ((priceSummary.XDaysCountOffset) > 0 ? 1m * priceSummary.HeapTick + priceSummary.XDaysOffsetPrice : 0m);
            var lowerRangePrice = currentBasePrice - offsetPrice;//priceSummary.PriceTick - ((priceSummary.XDaysCountOffset) < 0 ? 1m * priceSummary.HeapTick + priceSummary.XDaysOffsetPrice : 0m);
            return new KeyValuePair<decimal, decimal>(lowerRangePrice, upperRangePrice);
        }
    }
    public sealed class RoleSet
    {
        public static bool MinMaxRole<T>(IDictionary<string, T> parameters) where T : IComparable<T>
        {
            var max = parameters["max"];
            var min = parameters["min"];
            var value = parameters["value"];
            return max.CompareTo(value) > 0 && min.CompareTo(value) < 0;
        }
        public static bool MinMaxRole(decimal value, decimal min, decimal max, bool Isbetween = true, bool includeEqual = false)
        {
            return includeEqual ? Isbetween ? value <= max && value >= min : value >= max || value <= min : Isbetween ? value < max && value > min : value > max || value < min;
        }
    }
    [Serializable]
    public class PriceItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Price { get; set; }
        public DateTimeOffset PriceDateTime { get; set; }
        public decimal Amount { get; set; }
        public int Volume { get; set; }
    }
    [Serializable]
    public class PriceConfig
    {
        public string StockCode { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset StartRunningTime { get; set; } = DateTimeOffset.Now;
        public decimal PriceOffset { get; set; }
        public decimal HeapOffset { get; set; }
        public int BranchCount { get; set; }
        public decimal LimitUp { get; set; }
        public decimal LimitDown { get; set; }
        public int TotalBuyCount { get; private set; } = 0;
        public int TotalSaleCount { get; private set; } = 0;
        public BaseRole Role { get; set; }

        public List<CountHistoryItem> XDaysCountHistory { get; set; } = new List<CountHistoryItem>();
        public int DaysForXDaysIncreaseRole { get; set; }
        public int LengthForDeltaIncreaseRole { get; internal set; }
        public decimal MinOffsetForDeltaIncreaseRole { get; internal set; }
        public decimal CoefficientForDeltaIncreaseRole { get; internal set; }

        public void UpdateBuySaleCount(StockAction action)
        {
            var c = action == StockAction.Buy ? TotalBuyCount++ : TotalSaleCount++;
        }

    }
    [Serializable]
    public class PriceSummaryItem
    {
        public PriceConfig Config { get; set; }
        public DateTimeOffset DetailDate { get; set; }
        public decimal BasePrice { get; set; }
        public decimal CurrentBasePrice { get; set; }
        public decimal PriceTick { get { return BasePrice * Config.PriceOffset / Config.BranchCount; } }
        public decimal PriceOffset { get { return Config.PriceOffset / Config.BranchCount; } }
        public decimal HeapTick { get { return BasePrice * Config.HeapOffset; } }
        public decimal LimitUpPrice { get { return BasePrice * (1m + Config.LimitUp); } }
        public decimal LimitDownPrice { get { return BasePrice * (1m - Config.LimitDown); } }
        public int DailyBuyCount { get; private set; }
        public int DailySaleCount { get; private set; }
        public bool IsFirstItem { get; set; } = true;
        public DateTimeOffset LastUpdateTime { get; set; } = DateTimeOffset.Now;
        public bool IsValidPrice(decimal price) => price < BasePrice * 1.13m || price > BasePrice * 0.87m;
        public void UpdateBuySaleCount(StockAction action)
        {
            var c = action == StockAction.Buy ? DailyBuyCount++ : DailySaleCount++;
            Config.UpdateBuySaleCount(action);
        }
        public int XDaysCountOffset
        {
            get
            {
                if (Config.XDaysCountHistory.Any())
                {
                    var firstHistory = Config.XDaysCountHistory.First();
                    return (Config.TotalSaleCount - firstHistory.TotalSaleCount) - (Config.TotalBuyCount - firstHistory.TotalBuyCount);
                }
                return 0;
            }
        }
        public decimal XDaysOffsetPrice { get; set; }
        public decimal DeltaPrice { get; internal set; }
    }

    [Serializable]
    public class PriceTransactionItem //: IComparable<PriceTransactionItem>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RelatedId { get; set; }
        public PriceSummaryItem PriceSummary { get; set; }
        public StockAction Action { get; set; }
        public int Quantity { get; set; } = 1;
        public int BranchNumber { get; set; } = 0;
        public decimal Price { get; set; }

        private decimal targetPrice = -1m;
        public decimal TargetPrice
        {
            get
            {
                return targetPrice < 0 ? Price + (decimal)Action * (PriceSummary.PriceTick + BranchNumber * (PriceSummary.PriceTick + PriceSummary.HeapTick)) : targetPrice;
            }
            set
            {
                targetPrice = value;
            }
        }
        public StockAction TargetAction
        {
            get
            {
                if (TargetPrice <= 0m)
                    return StockAction.None;
                else if (Action == StockAction.Sale)
                    return StockAction.Buy;
                else if (Action == StockAction.Buy)
                    return StockAction.Sale;
                else return StockAction.None;
            }
        }
        public decimal HitPrice { get; set; }
        public DateTimeOffset DetailDateTime { get; set; }
        public DateTimeOffset LastUpdateTime { get; set; } = DateTimeOffset.Now;
        public Guid DetailId { get; set; }
        public int DailyBuyCount { get; set; }
        public int DailySaleCount { get; set; }
        public int TotalBuyCount { get; set; }
        public int TotalSaleCount { get; set; }
        public TransactionStatus Status { get; set; }

    }

    public class CountHistoryItem
    {
        public CountHistoryItem(DateTimeOffset detailDateTime, int totalBuyCount, int totalSaleCount)
        {
            this.DetailDateTime = detailDateTime;
            this.TotalSaleCount = totalSaleCount;
            this.TotalBuyCount = totalBuyCount;
        }
        public DateTimeOffset DetailDateTime { get; private set; }
        public int TotalBuyCount { get; private set; }
        public int TotalSaleCount { get; private set; }

    }
    public enum StockAction
    {
        Buy = 1,
        Sale = -1,
        None = 0
    }
    public enum TransactionStatus
    {
        None = 0,
        Active = 1,
        Completed = 0,
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
