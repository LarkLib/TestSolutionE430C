using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Utilities;
using System.Diagnostics;
using Common.DB;

namespace StockDataDownloaderConsoleApplication
{
    class Program
    {
        private static readonly DbHelper dbHelper = new DbHelper(Constants.StockConnectionString);
        static void Main(string[] args)
        {
            //GetStockDetail("sh600519", DateTime.Parse("2015-10-19"));
        }
        private static string GetStartDate(string stockCode)
        {
            return null;
        }
        private static void GetStockSummary(string stockCode, DateTime startDate)
        {
            WebClient client = new WebClient();
            SoundPlayer player = new SoundPlayer(@"提示.wav");          //声音
            player.Play();
            Console.WriteLine($"{DateTime.Now.ToNormalString()}: GetStockSummary");
            var failtCount = 0;
            for (var t = startDate; t < DateTime.Now; t = t.AddYears(1))
            {
                for (int i = 1; i <= 4; i++)
                {
                    var stockAddress = string.Format(Constants.StockSummaryAddress, t.Year, i);
                    var tString = $"{ t.Year }_0{ i}";
                    try
                    {
                        var stockContent = client.DownloadString(stockAddress);
                        var fileName = string.Format(Path.Combine(Constants.StockSummaryAddress, @"{0}_{1}_0{1}.html"), stockCode, t.Year, i);
                        File.WriteAllText(fileName, stockContent, Encoding.UTF8);
                        Console.WriteLine(string.Format("{0} --- {1}-{2}", DateTime.Now.ToNormalString(), t.Year, i));
                        Logger.Instance.LogTextMessage($"GetStockSummary,{stockCode},{tString},Downloaded");
                    }
                    catch (Exception e)
                    {

                        if (failtCount++ < Constants.MaxFailCount)
                        {
                            Logger.Instance.LogTextMessage($"GetStockSummary,{stockCode},{tString},retry,{failtCount},{e.Message}");
                            t = t.AddDays(-1);
                            Thread.Sleep(Constants.FailSleepMilliseconds);
                        }
                        else
                        {
                            failtCount = 0;
                            Logger.Instance.LogTextMessage($"GetStockSummary,{stockCode},{tString},failed,,{e.Message}");
                        }
                    }
                }
            }
            Console.WriteLine(DateTime.Now.ToNormalString());
            player.Play();
        }
        private static void ParseSummaryFiles()
        {
            var files = Directory.GetFiles(Constants.StockSummaryAddress, "*.html", SearchOption.TopDirectoryOnly).OrderBy(f => f);
            if (!files.Any())
            {
                return;
            }
            var stopwatch = new Stopwatch();
            foreach (var file in files)
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.Load(file);
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileArray = fileName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                var code = fileArray[0];
                try
                {
                    stopwatch.Start();
                    ParseSummaryDataTDB(code, htmlDocument);
                    Logger.Instance.LogTextMessage($"{nameof(ParseSummaryDataTDB)},{code},{fileName},Succesful,{(int)stopwatch.Elapsed.TotalMilliseconds}");
                    stopwatch.Restart();
                    File.Move(file, file.Replace("summary\\", "summary\\Backup\\"));
                    Logger.Instance.LogTextMessage($"File.Move,{code},{fileName},Succesful,{(int)stopwatch.Elapsed.TotalMilliseconds}");
                }
                catch (Exception e)
                {
                    Logger.Instance.LogTextMessage($"ParseSummaryFilesTDB,{code},{fileName},failed,,{e.Message}");
                }
            }
        }
        private static void ParseSummaryDataTDB(string code, HtmlDocument htmlDocument)
        {
            var rows = 0;
            var table = htmlDocument.GetElementbyId("FundHoldSharesTable");
            var dataRows = table.Elements("tr").ToArray();

            var sql = new StringBuilder(@"
                        INSERT INTO [dbo].[Summary]
                               ([DateTime]
                               ,[Code]
                               ,[OpenPrice]
                               ,[HighestPrice]
                               ,[ClosePrice]
                               ,[LowestPrice]
                               ,[Volume]
                               ,[Amount])
                         VALUES");
            for (int i = 1; i < dataRows.Length; i++)
            {

                var fields = dataRows[i].Elements("td").ToArray();
                sql.Append($"('{fields[0].InnerText.Trim()}','{code}','{fields[1].InnerText}','{fields[2].InnerText}','{fields[3].InnerText}','{fields[4].InnerText}','{fields[5].InnerText}','{fields[6].InnerText}'),");
                rows++;
            }
            sql.Remove(sql.Length - 1, 1);
            dbHelper.ExecuteNonQueryHelper(sql.ToString());
        }
        private static void GetStockDetail(string stockCode, DateTime startDate)
        {
            SoundPlayer player = new SoundPlayer(@"提示.wav");          //声音
            player.Play();
            Console.WriteLine($"{DateTime.Now.ToNormalString()}: GetStockDetail");
            WebClient client = new WebClient();
            var failtCount = 0;
            for (var t = startDate; t <= DateTime.Now; t = t.AddDays(1))
            {
                var tString = t.ToString("yyyy-MM-dd");
                if (t.DayOfWeek == DayOfWeek.Sunday || t.DayOfWeek == DayOfWeek.Saturday)
                {
                    Console.WriteLine(string.Format("{0} --- {1} 当天没有数据", DateTime.Now.ToNormalString(), tString));
                    Logger.Instance.LogTextMessage($"GetStockDetail,{stockCode},{tString},Nodata");
                    continue;
                }
                string stockAddress = string.Format(Constants.StockDetailAddress, tString, stockCode);
                try
                {
                    var stockContent = client.DownloadString(stockAddress);
                    if (!stockContent.Contains("当天没有数据"))
                    {
                        var fileName = string.Format(Path.Combine(Constants.StockDetailAddress, @"{1}_{0}.xls", tString, stockCode));
                        File.WriteAllText(fileName, stockContent);
                        Console.WriteLine(string.Format("{0} --- {1}", DateTime.Now.ToNormalString(), tString));
                        Logger.Instance.LogTextMessage($"GetStockDetail,{stockCode},{tString},succesful");
                        Thread.Sleep(100);
                    }
                    else
                    {
                        Console.WriteLine(string.Format("{0} --- {1} 当天没有数据", DateTime.Now.ToNormalString(), tString));
                        Logger.Instance.LogTextMessage($"GetStockDetail,{stockCode},{tString},Nodata");

                    }
                }
                catch (Exception e)
                {

                    if (failtCount++ < Constants.MaxFailCount)
                    {
                        Logger.Instance.LogTextMessage($"GetStockDetail,{stockCode},{tString},retry,{failtCount},{e.Message}");
                        t = t.AddDays(-1);
                        Thread.Sleep(Constants.FailSleepMilliseconds);
                    }
                    else
                    {
                        failtCount = 0;
                        Logger.Instance.LogTextMessage($"GetStockDetail,{stockCode},{tString},failed,,{e.Message}");
                    }
                }
            }
            Console.WriteLine(DateTime.Now.ToNormalString());
            player.Play();
        }
        private static void ParseDetailFiles()
        {
            var stopwatch = new Stopwatch();
            var files = Directory.GetFiles(Constants.StockDetailAddress, "*.xls", SearchOption.TopDirectoryOnly).OrderBy(f => f);
            if (!files.Any())
            {
                return;
            }
            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var fileArray = fileName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                var code = fileArray[0];
                var dealDate = fileArray[1];
                var lines = File.ReadLines(file).ToArray();
                try
                {
                    stopwatch.Start();
                    ParseDetailDataTDB(lines, code, dealDate);
                    Logger.Instance.LogTextMessage($"{nameof(ParseDetailDataTDB)},{code},{fileName},Succesful,{(int)stopwatch.Elapsed.TotalMilliseconds}");
                    stopwatch.Restart();
                    File.Move(file, file.Replace("detail\\", "detail\\Backup\\"));
                    Logger.Instance.LogTextMessage($"File.Move,{code},{fileName},Succesful,{(int)stopwatch.Elapsed.TotalMilliseconds}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"error:{e.Message}");
                    Logger.Instance.LogTextMessage($"{nameof(ParseDetailFiles)},{code},{fileName},failed,,{e.Message}");
                }
            }

        }
        private static void ParseDetailDataTDB(string[] lines, string code, string dealDate)
        {
            var rows = 0;
            var consoleLines = 0;
            consoleLines++;
            var sqlHeader = new StringBuilder(@"
                         INSERT INTO [dbo].[Detail]
                                ([DateTime]
                               ,[Code]
                               ,[Price]
                               ,[PriceOffset]
                               ,[Volume]
                               ,[Amount]
                               ,[Direction])
                         VALUES");
            var step = 900;
            if (lines.Length > 1)
            {

                for (int j = 1; j < lines.Length; j += step)
                {
                    var k = Math.Min(j + step, lines.Length);
                    var sqlValues = new StringBuilder();
                    for (int i = j; i < k; i++)
                    {

                        var fields = lines[i].Split(new[] { '	' }, StringSplitOptions.RemoveEmptyEntries);
                        var priceOffset = fields[2].Equals("--") ? "0" : fields[2];
                        sqlValues.Append($"('{dealDate} {fields[0]}','{code}','{fields[1]}','{priceOffset}','{int.Parse(fields[3]) * 100}','{fields[4]}','{fields[5]}'),");
                        rows++;
                    }
                    dbHelper.ExecuteNonQueryHelper($"{sqlHeader.ToString()}{sqlValues.ToString()}".TrimEnd(','));
                }
            }
        }

        private static void SaveLogInfoToDb(StockOperation operation)
        {
            var sql = $@"INSERT INTO [dbo].[StockOperation] ([Code] ,[Action] ,[Content] ,[Status] ,[ElapsedMilliseconds] ,[Message])
                         VALUES('{operation.Code}' ,{(int)operation.Action} ,'{operation.Content}' ,{(int)operation.Status} ,{(int)operation.ElapsedMilliseconds} ,'{operation.Message}')";
            dbHelper.ExecuteNonQueryHelper(sql);
        }
    }
    class StockOperation
    {
        public StockOperation(string code, StockAction action, string content, ActionStatus status, int elapsedMilliseconds, string message = null)
        {
            this.Code = code;
            this.Action = action;
            this.Content = content;
            this.Status = status;
            this.ElapsedMilliseconds = elapsedMilliseconds;
            this.Message = message;
        }
        internal string Code { get; private set; }
        internal StockAction Action { get; private set; }
        internal string Content { get; private set; }
        internal ActionStatus Status { get; private set; }
        internal int ElapsedMilliseconds { get; private set; }
        internal string Message { get; private set; }
    }
    enum StockAction
    {
        Download = 1,
        SaveToDB = 2
    }
    enum ActionStatus
    {
        Succesful = 1,
        Failed = -1
    }
}
