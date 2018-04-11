using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockInfoCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            var info = new StockInfoOps();
            //new StockInfoOps().WebClientTestGetStockInfo(false);
            //new StockInfoOps().WebClientTestGetStockCapitalStructure();
            //info.WebClientTestGetStockHolder();
            //info.WebClientTestGetCirculateStockHolder();
            //info.WebClientTestGetRightsOffering();
            //info.WebClientTestRetrieveRightsOffering();
            info.WebClientTestGetDaily163();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }

    #region StockInfo
    class StockInfoOps
    {
        private static long reds = 0;
        private static int Timeout = 600;

        internal void WebClientTestGetStockInfo(bool isLocal)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(@"D:\stockdata\StockCode.html", Encoding.UTF8);
            var list = htmlDocument.GetElementbyId("stockcode");
            var dataRows = list.Descendants("a").ToArray();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var item in dataRows)
            {
                var arr = item.InnerText.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                var code = arr[1];
                var name = arr[0];
                if (!code.StartsWith("60") && !code.StartsWith("00") && !code.StartsWith("30")) continue;
                var task = Task.Factory.StartNew(() => PrecessStockInfo(code, name, isLocal));
                while (Interlocked.Read(ref reds) > 5)
                {
                    Thread.Sleep(3000);
                }
                //PrecessStockInfo(code, name, isLocal);
                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")},{(int)stopwatch.ElapsedMilliseconds},{item.Name},{item.InnerText},{Path.GetFileNameWithoutExtension(item.GetAttributeValue("href", null))}");
                stopwatch.Restart();

                if (isLocal) break;
                //if (stopwatch.ElapsedMilliseconds > 1000 * 30) break;
            }
            Console.Write("Press any key to exit...");
            Console.ReadKey();
        }
        private void PrecessStockInfo(string code, string name, bool isLocal)
        {
            Interlocked.Increment(ref reds);
            string categoryContent = null;
            string companyInfoContent = null;
            HtmlNode categoryDiv = null;
            HtmlNode companyInfoTable = null;
            var categoryLink = "http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_CorpOtherInfo/stockid/{0}/menu_num/2.phtml";
            var companyInfoLink = "http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_CorpInfo/stockid/{0}.phtml";
            if (isLocal)
            {
                categoryLink = "http://localhost:8090/601766_2.html";
                companyInfoLink = "http://localhost:8090/601766.html";
            }

            StockEntities context = new StockEntities();
            WebClient client = new WebClient() { Encoding = Encoding.GetEncoding(936) };

            try
            {
                client.Encoding = Encoding.GetEncoding(936);
                categoryContent = client.DownloadString(string.Format(categoryLink, code));
                companyInfoContent = client.DownloadString(string.Format(companyInfoLink, code));
            }
            catch (Exception)
            {
                //throw;
            }

            string listingDateString = null;

            if (!string.IsNullOrEmpty(categoryContent))
            {
                var categoryHemlDocument = new HtmlDocument() { OptionOutputAsXml = true };
                categoryHemlDocument.LoadHtml(categoryContent);
                categoryDiv = categoryHemlDocument.GetElementbyId("con02-0");
            }
            if (!string.IsNullOrEmpty(companyInfoContent))
            {
                var companyInfoHtmlDocument = new HtmlDocument() { OptionOutputAsXml = true };
                companyInfoHtmlDocument.LoadHtml(companyInfoContent);
                companyInfoTable = companyInfoHtmlDocument.GetElementbyId("comInfo1");
                listingDateString = companyInfoTable?.Descendants("a")?.First()?.InnerText;
            }
            DateTimeOffset listingDate;
            context.StockInfoes.Add(new StockInfo() { Code = code, Name = name, Category = categoryDiv?.OuterHtml, CompanyInfo = companyInfoTable?.OuterHtml, ListingDate = DateTimeOffset.TryParse(listingDateString, out listingDate) ? (DateTimeOffset?)listingDate : null });
            context.SaveChanges();
            Interlocked.Decrement(ref reds);
        }

        internal void WebClientTestGetStockCapitalStructure()
        {
            var link = "http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_StockStructure/stockid/{0}.phtml";

            var context = new StockEntities();
            var codeIdList = context.StockInfoes.Where(info => info.CapitalStructure == null).Select(info => new { info.Id, info.Code }).ToArray();
            context.Dispose();
            Parallel.ForEach(codeIdList, new ParallelOptions() { MaxDegreeOfParallelism = 50 }, item =>
            {
                var code = item.Code;
                var client = new WebDownload(5 * 60 * 1000) { Encoding = Encoding.GetEncoding(936) };
                var content = client.DownloadString(string.Format(link, code.Substring(2)));
                if (!string.IsNullOrEmpty(content))
                {
                    var htmlDocument = new HtmlDocument() { OptionOutputAsXml = true };
                    htmlDocument.LoadHtml(content);
                    var div = htmlDocument.GetElementbyId("con02-1");
                    using (var stock = new StockEntities())
                    {
                        var connection = stock.Database.Connection;
                        var cmd = connection.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = $"update stockinfo set CapitalStructure=@content where Id='{item.Id}'";
                        cmd.Parameters.Add(new SqlParameter("@content", SqlDbType.Xml) { Value = div?.OuterHtml });
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                    htmlDocument = null;
                    div = null;
                    Console.WriteLine($"{code} done");
                }
                else
                {
                    Console.WriteLine($"{code} null");
                }
                content = null;
                client.Dispose();

            });
        }
        internal void WebClientTestGetStockHolder()
        {
            //http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_StockHolder/stockid/002236.phtml
            var link = "http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_StockHolder/stockid/{0}.phtml";

            var context = new StockEntities();
            var codeIdList = context.StockInfoes.Where(info => info.StockHolder == null).Select(info => new { info.Id, info.Code }).ToArray();
            context.Dispose();
            Parallel.ForEach(codeIdList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, item =>
            {
                var code = item.Code;
                var client = new WebDownload(5 * 60 * 1000) { Encoding = Encoding.GetEncoding(936) };
                var content = client.DownloadString(string.Format(link, code.Substring(2)));
                if (!string.IsNullOrEmpty(content))
                {
                    var htmlDocument = new HtmlDocument() { OptionOutputAsXml = true };
                    htmlDocument.LoadHtml(content);
                    var div = htmlDocument.GetElementbyId("con02-2");
                    using (var stock = new StockEntities())
                    {
                        var connection = stock.Database.Connection;
                        var cmd = connection.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = $"update stockinfo set StockHolder=@content where Id='{item.Id}'";
                        cmd.Parameters.Add(new SqlParameter("@content", SqlDbType.Xml) { Value = div?.OuterHtml });
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                    htmlDocument = null;
                    div = null;
                    Console.WriteLine($"GetStockHolder: {code} done");
                }
                else
                {
                    Console.WriteLine($"GetStockHolder: {code} null");
                }
                content = null;
                client.Dispose();

            });
        }
        internal void WebClientTestGetCirculateStockHolder()
        {
            //http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_CirculateStockHolder/stockid/002236.phtml
            var link = "http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_CirculateStockHolder/stockid/{0}.phtml";

            var context = new StockEntities();
            var codeIdList = context.StockInfoes.Where(info => info.CirculateStockHolder == null).Select(info => new { info.Id, info.Code }).ToArray();
            context.Dispose();
            Parallel.ForEach(codeIdList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, item =>
            {
                var code = item.Code;
                var client = new WebDownload(5 * 60 * 1000) { Encoding = Encoding.GetEncoding(936) };
                var content = client.DownloadString(string.Format(link, code.Substring(2)));
                if (!string.IsNullOrEmpty(content))
                {
                    var htmlDocument = new HtmlDocument() { OptionOutputAsXml = true };
                    htmlDocument.LoadHtml(content);
                    var div = htmlDocument.GetElementbyId("con02-3");
                    using (var stock = new StockEntities())
                    {
                        var connection = stock.Database.Connection;
                        var cmd = connection.CreateCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = $"update stockinfo set CirculateStockHolder=@content where Id='{item.Id}'";
                        cmd.Parameters.Add(new SqlParameter("@content", SqlDbType.Xml) { Value = div?.OuterHtml });
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                    htmlDocument = null;
                    div = null;
                    Console.WriteLine($"GetCirculateStockHolder: {code} done");
                }
                else
                {
                    Console.WriteLine($"GetCirculateStockHolder: {code} null");
                }
                content = null;
                client.Dispose();

            });
        }

        #region GetRightsOffering
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["StockEntities"].ConnectionString;
        private static int MaxDbThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxDbThreadCount"]);
        private static int MaxCodeThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxCodeThreadCount"]);
        private static string ConnectionFormatString = ConfigurationManager.AppSettings["ConnectionFormatString"] as string;
        private class CodeItem
        {
            public DateTimeOffset StartDate { get; set; }
            public string Code { get; set; }
        }

        internal void WebClientTestGetRightsOffering()
        {
            var endDate = DateTime.Parse("2017-04-01");
            var dbList = GetDbList();
            //foreach (var db in dbList)
            Parallel.ForEach(dbList, new ParallelOptions() { MaxDegreeOfParallelism = MaxDbThreadCount }, db =>
            {
                var codeItemList = GetRightsOfferingDateListByDatabase(db);
                //foreach (var code in codeList)
                Parallel.ForEach(codeItemList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, codeItem =>
                {
                    var date = codeItem.StartDate;
                    for (var t = date; t < endDate; t = t.AddMonths(3))
                    {
                        var i = Convert.ToInt16((t.Month - 1) / 3) + 1;
                        var stopwathc = new Stopwatch();
                        stopwathc.Start();
                        var client = new WebDownload(5 * 60 * 1000) { Encoding = Encoding.GetEncoding(936) };
                        var stockAddress = string.Format("http://vip.stock.finance.sina.com.cn/corp/go.php/vMS_FuQuanMarketHistory/stockid/{0}.phtml?year={1}&jidu={2}", codeItem.Code.Substring(2, 6), t.Year, i);
                        var stockContent = client.DownloadString(stockAddress);
                        var rows = 0;
                        if (!string.IsNullOrEmpty(stockContent))
                        {
                            var htmlDocument = new HtmlDocument();
                            htmlDocument.LoadHtml(stockContent);
                            rows = SaveStockRightsOfferingContentToDB(db, codeItem.Code, htmlDocument);
                            htmlDocument = null;
                        }
                        Console.WriteLine($"GetRightsOffering,{codeItem.Code},{t.Year}_Q{i},{rows},{(int)stopwathc.ElapsedMilliseconds}ms");
                    }
                }
                );
            }
            );
        }
        internal void WebClientTestGetDaily163()
        {
            var endDate = DateTime.Parse("2017-04-01");
            var dbList = GetDbList();
            //foreach (var db in dbList)
            Parallel.ForEach(dbList, new ParallelOptions() { MaxDegreeOfParallelism = MaxDbThreadCount }, db =>
            {
                var codeList = GetCodeListByDatabase(db);
                Parallel.ForEach(codeList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, code =>
                {
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        string stockContent = null;
                        for (int i = 0; i < 5; i++)
                        {
                            try
                            {
                                var client = new WebDownload(5 * 60 * 1000) { Encoding = Encoding.GetEncoding(936) };
                                var stockAddress = string.Format("http://quotes.money.163.com/service/chddata.html?code={0}{1}&start=19021231&end=20180526&fields=TCLOSE;HIGH;LOW;TOPEN;LCLOSE;CHG;PCHG;TURNOVER;VOTURNOVER;VATURNOVER;TCAP;MCAP", code.StartsWith("sh") ? 0 : 1, code.Substring(2, 6));
                                stockContent = client.DownloadString(stockAddress);
                                break;
                            }
                            catch (WebException)
                            {
                                Thread.Sleep(15000);
                            }
                        }
                        var rows = 0;
                        if (!string.IsNullOrEmpty(stockContent))
                        {
                            rows = SaveStockDaily163ContentToDB(db, code, stockContent);
                        }
                        Console.WriteLine($"{db},GetDaily163,{code},{rows},{(int)stopwatch.ElapsedMilliseconds}ms");
                    }
                }
                );
            }
            );
        }

        private IList<string> GetCodeListByDatabase(string database)
        {
            var list = new List<string>();
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, database)))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"select code from stockinfo..codeinfo with(nolock)  where db = @db and status = 0";
                cmd.Parameters.Add(new SqlParameter("@db", SqlDbType.VarChar) { Value = database });
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

        private IList<CodeItem> GetRightsOfferingDateListByDatabase(string database)
        {
            var list = new List<CodeItem>();
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, database)))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
                        select ci.code,coalesce(ri.datetime,'2012-03-01') datetime from  stockinfo..codeinfo ci with(nolock)
                        left join (select code, max(dateadd(m,3,datetime)) datetime from RightsOfferingInfo with(nolock) group by code) as ri on ci.code = ri.code
                        where ci.db = @db";
                cmd.Parameters.Add(new SqlParameter("@db", SqlDbType.VarChar) { Value = database });
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new CodeItem() { Code = reader.GetString(0), StartDate = reader.GetDateTimeOffset(1) });
                }
                connection.Close();

            }
            return list;

        }
        private int SaveStockRightsOfferingContentToDB(string database, string code, HtmlDocument htmlDocument)
        {
            var rows = 0;
            var table = htmlDocument.GetElementbyId("FundHoldSharesTable");
            var dataRows = table.Elements("tr").ToArray();
            if (dataRows == null || dataRows.Length < 2) return 0;
            var dataTable = new DataTable("RightsOfferingInfo");
            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn("DateTime", typeof(DateTimeOffset)),
                    new DataColumn("Code", typeof(string)),
                    new DataColumn("OpenPrice", typeof(decimal)),
                    new DataColumn("HighestPrice", typeof(decimal)),
                    new DataColumn("ClosePrice", typeof(decimal)),
                    new DataColumn("LowestPrice", typeof(decimal)),
                    new DataColumn("Volume", typeof(int)),
                    new DataColumn("Amount", typeof(decimal)),
                    new DataColumn("Factor", typeof(decimal))
                });
            DateTimeOffset dateTime;
            decimal openPrice;
            decimal highestPrice;
            decimal closePrice;
            decimal lowestPrice;
            decimal factor;
            int volume = default(int);
            decimal amount = default(decimal);

            for (int i = 1; i < dataRows.Length; i++)
            {
                var fields = dataRows[i].Elements("td").ToArray();
                DateTimeOffset.TryParse(fields[0].InnerText.Trim(), out dateTime);
                decimal.TryParse(fields[1].InnerText, out openPrice);
                decimal.TryParse(fields[2].InnerText, out highestPrice);
                decimal.TryParse(fields[3].InnerText, out closePrice);
                decimal.TryParse(fields[4].InnerText, out lowestPrice);
                int.TryParse(fields[5].InnerText, out volume);
                decimal.TryParse(fields[6].InnerText, out amount);
                decimal.TryParse(fields[7].InnerText, out factor);
                dataTable.Rows.Add(new object[] { dateTime, code, openPrice, highestPrice, closePrice, lowestPrice, volume, amount, factor });
                rows++;
            }
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, database)))
            {
                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.ColumnMappings.Add("DateTime", "DateTime");
                bulkCopy.ColumnMappings.Add("Code", "Code");
                bulkCopy.ColumnMappings.Add("OpenPrice", "OpenPrice");
                bulkCopy.ColumnMappings.Add("HighestPrice", "HighestPrice");
                bulkCopy.ColumnMappings.Add("ClosePrice", "ClosePrice");
                bulkCopy.ColumnMappings.Add("LowestPrice", "LowestPrice");
                bulkCopy.ColumnMappings.Add("Volume", "Volume");
                bulkCopy.ColumnMappings.Add("Amount", "Amount");
                bulkCopy.ColumnMappings.Add("Factor", "Factor");

                connection.Open();
                bulkCopy.DestinationTableName = dataTable.TableName;
                bulkCopy.WriteToServer(dataTable);
                connection.Close();
            }
            return rows;
        }
        private int SaveStockDaily163ContentToDB(string database, string code, string stockContent)
        {
            var rows = 0;
            var dataRows = stockContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (dataRows == null || dataRows.Length < 2) return 0;

            var dataTable = new DataTable("Daily163");
            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn("DateTime", typeof(DateTimeOffset)),
                    new DataColumn("Code", typeof(string)),
                    new DataColumn("ClosePrice", typeof(decimal)),
                    new DataColumn("HighestPrice", typeof(decimal)),
                    new DataColumn("LowestPrice", typeof(decimal)),
                    new DataColumn("OpenPrice", typeof(decimal)),
                    new DataColumn("PrevClose", typeof(decimal)),
                    new DataColumn("Change", typeof(decimal)),
                    new DataColumn("ChangePercent", typeof(decimal)),
                    new DataColumn("TurnoverRate", typeof(decimal)),
                    new DataColumn("Volume", typeof(long)),
                    new DataColumn("Amount", typeof(decimal)),
                    new DataColumn("TotalCapitalization", typeof(decimal)),
                    new DataColumn("MarketCapitalization", typeof(decimal))
                });

            for (int i = 1; i < dataRows.Length; i++)
            {
                var row = dataRows[i].Replace("None", "0");
                var fields = row.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries); ;
                dataTable.Rows.Add(new object[] { DateTimeOffset.Parse(fields[0]), code, decimal.Parse(fields[3], NumberStyles.Float), decimal.Parse(fields[4], NumberStyles.Float), decimal.Parse(fields[5], NumberStyles.Float), decimal.Parse(fields[6], NumberStyles.Float), decimal.Parse(fields[7], NumberStyles.Float), decimal.Parse(fields[8], NumberStyles.Float), decimal.Parse(fields[9], NumberStyles.Float), decimal.Parse(fields[10], NumberStyles.Float), decimal.Parse(fields[11], NumberStyles.Float), (long)decimal.Parse(fields[12], NumberStyles.Float), decimal.Parse(fields[13], NumberStyles.Float), decimal.Parse(fields[14], NumberStyles.Float) });
                rows++;
            }
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, database)))
            {
                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.BulkCopyTimeout = Timeout;
                bulkCopy.ColumnMappings.Add("DateTime", "DateTime");
                bulkCopy.ColumnMappings.Add("Code", "Code");
                bulkCopy.ColumnMappings.Add("ClosePrice", "ClosePrice");
                bulkCopy.ColumnMappings.Add("HighestPrice", "HighestPrice");
                bulkCopy.ColumnMappings.Add("LowestPrice", "LowestPrice");
                bulkCopy.ColumnMappings.Add("OpenPrice", "OpenPrice");
                bulkCopy.ColumnMappings.Add("PrevClose", "PrevClose");
                bulkCopy.ColumnMappings.Add("Change", "Change");
                bulkCopy.ColumnMappings.Add("ChangePercent", "ChangePercent");
                bulkCopy.ColumnMappings.Add("TurnoverRate", "TurnoverRate");
                bulkCopy.ColumnMappings.Add("Volume", "Volume");
                bulkCopy.ColumnMappings.Add("Amount", "Amount");
                bulkCopy.ColumnMappings.Add("TotalCapitalization", "TotalCapitalization");
                bulkCopy.ColumnMappings.Add("MarketCapitalization", "MarketCapitalization");

                connection.Open();
                bulkCopy.DestinationTableName = dataTable.TableName;
                bulkCopy.WriteToServer(dataTable);

                var cmd = connection.CreateCommand();
                cmd.CommandText = "update stockinfo..codeinfo set status=1 where code=@code";
                cmd.Parameters.Add(new SqlParameter("@code", SqlDbType.VarChar) { Value = code });
                cmd.CommandTimeout = Timeout;
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            return rows;
        }
        private IList<string> GetDbList()
        {
            var list = new List<string>();
            using (var stockEntities = new StockEntities())
            {

                using (var connection = stockEntities.Database.Connection)
                {
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "select distinct db from codeinfo with(nolock) order by db";
                    connection.Open();
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(reader.GetString(0));
                    }
                    connection.Close();
                }
            }
            return list;
        }
        internal void WebClientTestRetrieveRightsOffering()
        {
            var endDate = DateTime.Parse("2017-04-01");
            var dbList = GetDbList();
            //foreach (var db in dbList)
            Parallel.ForEach(dbList, new ParallelOptions() { MaxDegreeOfParallelism = MaxDbThreadCount }, db =>
            {
                var codeItemList = GetRetrieveRightsOfferingDateListByDatabase(db);
                //foreach (var code in codeList)
                Parallel.ForEach(codeItemList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, codeItem =>
                {
                    var date = codeItem.StartDate;
                    var t = date;
                    //for (var t = date; t < endDate; t = t.AddMonths(3))
                    //{
                    var i = Convert.ToInt16((t.Month - 1) / 3) + 1;
                    var stopwathc = new Stopwatch();
                    stopwathc.Start();
                    var client = new WebDownload(5 * 60 * 1000) { Encoding = Encoding.GetEncoding(936) };
                    var stockAddress = string.Format("http://vip.stock.finance.sina.com.cn/corp/go.php/vMS_FuQuanMarketHistory/stockid/600030.phtml?year={0}&jidu={1}", t.Year, i);
                    var stockContent = client.DownloadString(stockAddress);
                    var rows = 0;
                    if (!string.IsNullOrEmpty(stockContent))
                    {
                        var htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(stockContent);
                        rows = SaveStockRetrieveRightsOfferingContentToDB(db, codeItem, htmlDocument);
                        htmlDocument = null;
                    }
                    Console.WriteLine($"GetRightsOffering,{codeItem.Code},{t.Year}_Q{i},{rows},{(int)stopwathc.ElapsedMilliseconds}ms");
                    //}
                }
                );
            }
            );
        }
        private IList<CodeItem> GetRetrieveRightsOfferingDateListByDatabase(string database)
        {
            var list = new List<CodeItem>();
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, database)))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select code,dt datetime from  missingDate ci with(nolock)";
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new CodeItem() { Code = reader.GetString(0), StartDate = reader.GetDateTimeOffset(1) });
                }
                connection.Close();

            }
            return list;

        }
        private int SaveStockRetrieveRightsOfferingContentToDB(string database, CodeItem codeItem, HtmlDocument htmlDocument)
        {
            var rows = 0;
            var code = codeItem.Code;
            var table = htmlDocument.GetElementbyId("FundHoldSharesTable");
            var dataRowsAll = table.Elements("tr").ToArray();
            if (dataRowsAll == null || dataRowsAll.Length < 2) return 0;
            var dataRows = dataRowsAll;
            var dataTable = new DataTable("RightsOfferingInfo");
            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn("DateTime", typeof(DateTimeOffset)),
                    new DataColumn("Code", typeof(string)),
                    new DataColumn("OpenPrice", typeof(decimal)),
                    new DataColumn("HighestPrice", typeof(decimal)),
                    new DataColumn("ClosePrice", typeof(decimal)),
                    new DataColumn("LowestPrice", typeof(decimal)),
                    new DataColumn("Volume", typeof(int)),
                    new DataColumn("Amount", typeof(decimal)),
                    new DataColumn("Factor", typeof(decimal))
                });
            DateTimeOffset dateTime;
            decimal openPrice;
            decimal highestPrice;
            decimal closePrice;
            decimal lowestPrice;
            decimal factor;
            int volume = default(int);
            decimal amount = default(decimal);

            for (int i = 1; i < dataRows.Length; i++)
            {
                var fields = dataRows[i].Elements("td").ToArray();
                DateTimeOffset.TryParse(fields[0].InnerText.Trim(), out dateTime);
                decimal.TryParse(fields[1].InnerText, out openPrice);
                decimal.TryParse(fields[2].InnerText, out highestPrice);
                decimal.TryParse(fields[3].InnerText, out closePrice);
                decimal.TryParse(fields[4].InnerText, out lowestPrice);
                int.TryParse(fields[5].InnerText, out volume);
                decimal.TryParse(fields[6].InnerText, out amount);
                decimal.TryParse(fields[7].InnerText, out factor);
                dataTable.Rows.Add(new object[] { dateTime, code, openPrice, highestPrice, closePrice, lowestPrice, volume, amount, factor });
                rows++;
            }
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, database)))
            {
                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.ColumnMappings.Add("DateTime", "DateTime");
                bulkCopy.ColumnMappings.Add("Code", "Code");
                bulkCopy.ColumnMappings.Add("OpenPrice", "OpenPrice");
                bulkCopy.ColumnMappings.Add("HighestPrice", "HighestPrice");
                bulkCopy.ColumnMappings.Add("ClosePrice", "ClosePrice");
                bulkCopy.ColumnMappings.Add("LowestPrice", "LowestPrice");
                bulkCopy.ColumnMappings.Add("Volume", "Volume");
                bulkCopy.ColumnMappings.Add("Amount", "Amount");
                bulkCopy.ColumnMappings.Add("Factor", "Factor");

                connection.Open();
                bulkCopy.DestinationTableName = dataTable.TableName;
                bulkCopy.WriteToServer(dataTable);
                connection.Close();
            }
            return rows;
        }
        #endregion GetRightsOffering

        public class WebDownload : WebClient
        {
            /// <summary>
            /// Time in milliseconds
            /// </summary>
            public int Timeout { get; set; }

            public WebDownload() : this(60000) { }

            public WebDownload(int timeout)
            {
                this.Timeout = timeout;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                if (request != null)
                {
                    request.Timeout = this.Timeout;
                }
                return request;
            }
        }

    }
    #endregion StockInfo
}
