using LarkLib.Common.Utilities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace StockDataDownloader
{
    internal class StockDataOperation
    {
        internal static readonly StockDataOperation Instance = new StockDataOperation();
        //private static object objectLock = new object();

        internal void Execute()
        {
            //ParseDetailFiles();
            GetSummary();
            Task.WaitAll();
            GetDetail();
            Task.WaitAll();
        }

        #region GetSummary
        private void GetSummary()
        {
            foreach (var stockCode in Constants.StockListArray)
            {
                var datelist = GetSummaryDateListByCode(stockCode);
                var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Constants.MaxThreadCount };
                if (datelist == null || !datelist.Any()) continue;
                Parallel.ForEach(datelist, parallelOptions, dateItem =>
                      {
                          var stopwatch = new Stopwatch();
                          stopwatch.Start();
                          UpdateStockOperationStatus(dateItem, StockAction.SummaryDownload, ActionStatus.Initialized, $"{stockCode},{StockAction.SummaryDownload.ToString()},{ActionStatus.Initialized.ToString()}", null, 0);
                          bool hasError = false;
                          string errorMessage = null;
                          var actionDate = dateItem.ActionDate;
                          var rows = Task.FromResult(0);
                          for (int i = 0; i < Constants.MaxFailCount; i++)
                          {
                              hasError = false;
                              try
                              {
                                  var stockContent = GetStockSummaryContent(stockCode, dateItem);
                                  rows = SaveStockSummaryContentToDB(stockCode, stockContent);
                                  stockContent = null;
                                  break;
                              }
                              catch (Exception e)
                              {
                                  hasError = true;
                                  var tString = $"{ actionDate.Year }_0{actionDate.GetQuarter()}";
                                  errorMessage = e.Message;
                                  Logger.Instance.LogTextMessage($"GetStockSummary,{stockCode},{tString},retry,{i},{errorMessage}", isError: true);
                                  Thread.Sleep(Constants.FailSleepMilliseconds);
                              }
                          }
                          stopwatch.Stop();
                          var currentDate = DateTimeOffset.Now;
                          var isCurrentQuarter = actionDate.Year == currentDate.Year && actionDate.GetQuarter() == currentDate.GetQuarter();
                          var action = isCurrentQuarter ? StockAction.Created : StockAction.SummarySave;
                          var status = isCurrentQuarter ? ActionStatus.Initialized : hasError ? ActionStatus.Faulted : ActionStatus.Succeeded;
                          var content = $"{stockCode},{action.ToString()},{status.ToString()},{rows}";
                          UpdateStockOperationStatus(dateItem, action, status, content, errorMessage, (int)stopwatch.ElapsedMilliseconds);
                          stopwatch = null;
                          errorMessage = null;
                      });
                datelist = null;
                parallelOptions = null;
            }
        }
        private int UpdateStockOperationStatus(usp_GetSummaryDateListByCode_Result dateItem, StockAction action, ActionStatus status, string content, string message, int elapsedMilliseconds)
        {
            using (var stockEntities = new StockEntities())
            {
                //lock (objectLock)
                //{
                return stockEntities.usp_UpdateStockOperationStatus(dateItem.Id, (int)action, (int)status, content, message, elapsedMilliseconds);
                //}
            }
        }
        private IList<usp_GetSummaryDateListByCode_Result> GetSummaryDateListByCode(string stockCode)
        {
            using (StockEntities stockEntities = new StockEntities())
            {
                return stockEntities.usp_GetSummaryDateListByCode(stockCode).ToList();
            }

        }
        private string GetStockSummaryContent(string stockCode, usp_GetSummaryDateListByCode_Result dateItem)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            //Console.WriteLine($"{DateTime.Now.ToNormalString()}:GetStockSummary Start({stockCode}-{Thread.CurrentThread.ManagedThreadId})");
            var startDate = dateItem.ActionDate;
            var year = startDate.Year;
            var quarter = startDate.GetQuarter();
            var stockAddress = string.Format(Constants.StockSummaryAddress, stockCode.Right(6), year, quarter);
            var tString = $"{ year }_0{quarter}";
            WebClient client = new WebClient();

            var stockContent = client.DownloadString(stockAddress);
            client.Dispose();
            if (Constants.SaveSummaryContentToFile)
            {
                Task.Run(() => SaveStockContentToFile(
                    string.Format(Path.Combine(Constants.StockSummaryDataPath, @"{0}_{1}_0{2}.html"), stockCode, year, quarter),
                    stockContent));
            }
            stopwatch.Stop();
            Logger.Instance.LogTextMessage($"GetStockSummary,{stockCode},{tString},{(int)stopwatch.ElapsedMilliseconds},Downloaded");
            Console.WriteLine($"{DateTime.Now.ToNormalString()}:GetStockSummary End {stockCode},{tString},{(int)stopwatch.ElapsedMilliseconds}");
            stopwatch = null;
            return stockContent;
        }
        private void SaveStockContentToFile(string fileName, string stockContent)
        {
            if (!fileName.IsNullOrEmpty() && !stockContent.IsNullOrEmpty())
            {
                File.WriteAllText(fileName, stockContent, Encoding.UTF8);
            }
        }
        private async void ParseSummaryFiles()
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
                    var result = await SaveStockSummaryContentToDB(code, htmlDocument);
                    Logger.Instance.LogTextMessage($"{nameof(SaveStockSummaryContentToDB)},{code},{fileName},Succesful,{(int)stopwatch.Elapsed.TotalMilliseconds}");
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
        private async Task<int> SaveStockSummaryContentToDB(string code, string stockContent)
        {
            if (!stockContent.IsNullOrWhiteSpace())
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(stockContent);
                var result = await SaveStockSummaryContentToDB(code, htmlDocument);
                htmlDocument = null;
                return result;
            }
            return 0;
        }

        private Task<int> SaveStockSummaryContentToDB(string code, HtmlDocument htmlDocument)
        {
            var rows = 0;
            var table = htmlDocument.GetElementbyId("FundHoldSharesTable");
            var dataRows = table.Elements("tr").ToArray();

            var xml = new StringBuilder("<SummaryItems>");
            for (int i = 1; i < dataRows.Length; i++)
            {
                var fields = dataRows[i].Elements("td").ToArray();
                DateTimeOffset dateTime;
                DateTimeOffset.TryParse(fields[0].InnerText.Trim(), out dateTime);
                xml.Append($@"<Item>
<DateTime>{dateTime}</DateTime>
<Code>{code}</Code>
<OpenPrice>{fields[1].InnerText}</OpenPrice>
<HighestPrice>{fields[2].InnerText}</HighestPrice>
<ClosePrice>{fields[3].InnerText}</ClosePrice>
<LowestPrice>{fields[4].InnerText}</LowestPrice>
<Volume>{fields[5].InnerText}</Volume>
<Amount>{fields[6].InnerText}</Amount>
</Item>");
                rows++;
            }
            xml.Append("</SummaryItems>");
            using (StockEntities stockEntities = new StockEntities())
            {
                //lock (objectLock)
                //{
                return Task.FromResult<int>(stockEntities.usp_SaveStockSummary(xml.ToString()));
                //}
            }
        }

        #region Removed
        /*
         private Task<int> SaveStockSummaryContentToDB(string code, HtmlDocument htmlDocument)
        {
            var rows = 0;
            var table = htmlDocument.GetElementbyId("FundHoldSharesTable");
            var dataRows = table.Elements("tr").ToArray();
            var deleteSql = @"    
    IF(EXISTS(  SELECT 1
                FROM   [Summary] WITH (NOLOCK)
                WHERE  [DateTime] >= '{0}' AND [DateTime] <= '{1}'  AND [Code] = '{2}'))
    BEGIN
        DELETE [Summary]
        WHERE  [id] IN(SELECT [id]
                       FROM   [Summary] WITH (NOLOCK)
                       WHERE  [DateTime] >= '{0}' AND [DateTime] <= '{1}'  AND [Code] = '{2}')
    END
";
            var dataTable = new DataTable("Summary");
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
                    new DataColumn("Amount", typeof(decimal))
                });
            DateTimeOffset dateTime;
            DateTimeOffset startDate = DateTimeOffset.MaxValue;
            DateTimeOffset endDate = DateTimeOffset.MinValue;
            decimal openPrice;
            decimal highestPrice;
            decimal closePrice;
            decimal lowestPrice;
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
                startDate = dateTime < startDate ? dateTime : startDate;
                endDate = dateTime > endDate ? dateTime : endDate;
                dataTable.Rows.Add(new object[] { dateTime, code, openPrice, highestPrice, closePrice, lowestPrice, volume * 100, amount });
                rows++;
            }
            using (StockEntities stockEntities = new StockEntities())
            {
                var connection = (SqlConnection)stockEntities.Database.Connection;
                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.ColumnMappings.Add("DateTime", "DateTime");
                bulkCopy.ColumnMappings.Add("Code", "Code");
                bulkCopy.ColumnMappings.Add("OpenPrice", "OpenPrice");
                bulkCopy.ColumnMappings.Add("HighestPrice", "HighestPrice");
                bulkCopy.ColumnMappings.Add("ClosePrice", "ClosePrice");
                bulkCopy.ColumnMappings.Add("LowestPrice", "LowestPrice");
                bulkCopy.ColumnMappings.Add("Volume", "Volume");
                bulkCopy.ColumnMappings.Add("Amount", "Amount");

                if (connection.State != ConnectionState.Open)
                    connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = deleteSql.FormatInvariantCulture(startDate, endDate, code);
                cmd.ExecuteNonQuery();
                bulkCopy.DestinationTableName = dataTable.TableName;
                bulkCopy.WriteToServer(dataTable);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return Task.FromResult<int>(rows);
        }
*/
        #endregion

        #endregion GetSummary

        #region GetDetail
        private void GetDetail()
        {
            foreach (var stockCode in Constants.StockListArray)
            {
                var datelist = GetDetailDateListByCode(stockCode);
                var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = Constants.MaxThreadCount };
                if (datelist == null || !datelist.Any()) continue;
                Parallel.ForEach(datelist, parallelOptions, dateItem =>
                 {
                     var stopwatch = new Stopwatch();
                     stopwatch.Start();
                     bool hasError = false;
                     string errorMessage = null;
                     var actionDate = dateItem.ActionDate;
                     var tString = $"{actionDate.ToString("yyyy-MM-dd")}";
                     var rows = Task.FromResult(0);
                     for (int i = 0; i < Constants.MaxFailCount; i++)
                     {
                         hasError = false;
                         try
                         {
                             var stockContent = GetStockDetailContent(stockCode, dateItem);
                             rows = SaveStockDetailContentToDB(stockCode, stockContent, tString);
                             break;
                         }
                         catch (Exception e)
                         {
                             hasError = true;
                             errorMessage = e.Message;
                             Logger.Instance.LogTextMessage($"GetDetail,{stockCode},{tString},retry,{i},{errorMessage}", isError: true);
                             Thread.Sleep(Constants.FailSleepMilliseconds);
                         }
                     }
                     stopwatch.Stop();
                     var status = hasError ? ActionStatus.Faulted : ActionStatus.Succeeded;
                     UpdateStockDetailStatus(dateItem, status, $"{stockCode},{status.ToString()},{rows.Result}", (int)stopwatch.ElapsedMilliseconds);
                     stopwatch = null;
                     errorMessage = null;
                 });
                datelist = null;
                parallelOptions = null;
            }
        }
        private int UpdateStockDetailStatus(usp_GetDetailDateListByCode_Result dateItem, ActionStatus status, string content, int elapsedMilliseconds)
        {
            using (var stockEntities = new StockEntities())
            {
                //lock (objectLock)
                //{
                return stockEntities.usp_UpdateStockSummaryStatus(dateItem.Id, (int)status, content, elapsedMilliseconds);
                //}
            }
        }
        private IList<usp_GetDetailDateListByCode_Result> GetDetailDateListByCode(string stockCode)
        {
            using (StockEntities stockEntities = new StockEntities())
            {
                return stockEntities.usp_GetDetailDateListByCode(stockCode).ToList();
            }

        }
        private string GetStockDetailContent(string stockCode, usp_GetDetailDateListByCode_Result dateItem)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            WebClient client = new WebClient();
            //Console.WriteLine($"{DateTime.Now.ToNormalString()}: GetStockDetailContent Start({stockCode}-{Thread.CurrentThread.ManagedThreadId})");
            var startDate = dateItem.ActionDate;
            var tString = startDate.ToString("yyyy-MM-dd");
            var stockAddress = string.Format(Constants.StockDetailAddress, tString, stockCode); ;

            var stockContent = client.DownloadString(stockAddress);
            if (Constants.SaveDetailContentToFile)
            {
                Task.Run(() => SaveStockContentToFile(
                    Path.Combine(Constants.StockDetailDataPath, string.Format(@"{1}_{0}.xls", tString, stockCode)),
                    stockContent));
            }
            stopwatch.Stop();
            Logger.Instance.LogTextMessage($"GetStockDetailContent,{stockCode},{tString},{(int)stopwatch.ElapsedMilliseconds},Downloaded");
            Console.WriteLine($"{DateTime.Now.ToNormalString()}:GetStockDetailContent End({stockCode}),{tString},{(int)stopwatch.ElapsedMilliseconds}");
            stopwatch = null;
            return stockContent;
        }
        private void ParseDetailFiles()
        {
            var stopwatch = new Stopwatch();
            var files = Directory.GetFiles(Constants.StockDetailDataPath, "*.xls", SearchOption.TopDirectoryOnly).OrderBy(f => f);
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
                    SaveStockDetailContentToDB(code, lines, dealDate);
                    Logger.Instance.LogTextMessage($"{nameof(SaveStockDetailContentToDB)},{code},{fileName},Succesful,{(int)stopwatch.Elapsed.TotalMilliseconds}");
                    stopwatch.Restart();
                    File.Move(file, file.Replace("detail\\", "detail\\Backup\\"));
                    Logger.Instance.LogTextMessage($"File.Move,{code},{fileName},Succesful,{(int)stopwatch.Elapsed.TotalMilliseconds}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"error:{e.Message}");
                    Logger.Instance.LogTextMessage($"{nameof(ParseDetailFiles)},{code},{fileName},failed,,{e.Message}", isError: true);
                }
            }

        }
        private async Task<int> SaveStockDetailContentToDB(string code, string stockContent, string dealDate)
        {
            if (code.IsNullOrEmpty() || stockContent.IsNullOrEmpty() || dealDate.IsNullOrEmpty() || !stockContent[0].Equals('成')) return 0;
            var lines = stockContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = await SaveStockDetailContentToDB(code, lines, dealDate);
            lines = null;
            return result;
        }
        private Task<int> SaveStockDetailContentToDB(string code, string[] lines, string dealDate)
        {
            var dataTable = new DataTable("Detail");
            dataTable.Columns.AddRange(
                new[]
                {
                    //new DataColumn("Id", typeof(Guid)),
                    new DataColumn("DateTime", typeof(DateTimeOffset)),
                    new DataColumn("Code", typeof(string)),
                    new DataColumn("Price", typeof(decimal)),
                    new DataColumn("PriceOffset", typeof(decimal)),
                    new DataColumn("Volume", typeof(int)),
                    new DataColumn("Amount", typeof(decimal)),
                    new DataColumn("Direction", typeof(string)),
                    //new DataColumn("LastUpdateTime", typeof(DateTimeOffset))
                    new DataColumn("BuyPrice1", typeof(decimal)),
                    new DataColumn("BuyPrice2", typeof(decimal)),
                    new DataColumn("BuyPrice3", typeof(decimal)),
                    new DataColumn("BuyPrice4", typeof(decimal)),
                    new DataColumn("BuyPrice5", typeof(decimal)),
                    new DataColumn("SalePrice1", typeof(decimal)),
                    new DataColumn("SalePrice2", typeof(decimal)),
                    new DataColumn("SalePrice3", typeof(decimal)),
                    new DataColumn("SalePrice4", typeof(decimal)),
                    new DataColumn("SalePrice5", typeof(decimal)),
                    new DataColumn("BuyVolume1", typeof(int)),
                    new DataColumn("BuyVolume2", typeof(int)),
                    new DataColumn("BuyVolume3", typeof(int)),
                    new DataColumn("BuyVolume4", typeof(int)),
                    new DataColumn("BuyVolume5", typeof(int)),
                    new DataColumn("SaleVolume1", typeof(int)),
                    new DataColumn("SaleVolume2", typeof(int)),
                    new DataColumn("SaleVolume3", typeof(int)),
                    new DataColumn("SaleVolume4", typeof(int)),
                    new DataColumn("SaleVolume5", typeof(int)),
                    new DataColumn("TickCount", typeof(int)),
                });
            var rows = lines.Length - 1;
            if (lines != null && lines.Length > 1 && lines[0][0].Equals('成'))
            {
                DateTimeOffset dealDateTime;
                DateTimeOffset.TryParse(dealDate, out dealDateTime);

                DateTimeOffset dateTime;
                decimal price = default(decimal);
                decimal priceOffset = default(decimal);
                int volume = default(int);
                decimal amount = default(decimal);
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(new[] { '	' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fields.Length != 6) continue;
                    DateTimeOffset.TryParse($"{dealDate} {fields[0]}", out dateTime);
                    decimal.TryParse(fields[1], out price);
                    decimal.TryParse(fields[2].Equals("--") ? "0" : fields[2], out priceOffset);
                    int.TryParse(fields[3], out volume);
                    decimal.TryParse(fields[4], out amount);
                    dataTable.Rows.Add(new object[]
                    {
                        /*Uuid.NewUuid(),*/ dateTime, code, price, priceOffset, volume * 100, amount, fields[5],
                        0m,0m,0m,0m,0m,0m,0m,0m,0m,0m,0,0,0,0,0,0,0,0,0,0,0
                    });
                }
                using (StockEntities stockEntities = new StockEntities())
                {
                    stockEntities.usp_SaveStockDetail(batchNumber: 0, stockCode: code, actionDate: dealDateTime, stockDetailSql: null);

                    var connection = (SqlConnection)stockEntities.Database.Connection;
                    var bulkCopy = new SqlBulkCopy(connection);
                    bulkCopy.ColumnMappings.Add("DateTime", "DateTime");
                    bulkCopy.ColumnMappings.Add("Code", "Code");
                    bulkCopy.ColumnMappings.Add("Price", "Price");
                    bulkCopy.ColumnMappings.Add("PriceOffset", "PriceOffset");
                    bulkCopy.ColumnMappings.Add("Volume", "Volume");
                    bulkCopy.ColumnMappings.Add("Amount", "Amount");
                    bulkCopy.ColumnMappings.Add("Direction", "Direction");
                    bulkCopy.ColumnMappings.Add("BuyPrice1", "BuyPrice1");
                    bulkCopy.ColumnMappings.Add("BuyPrice2", "BuyPrice2");
                    bulkCopy.ColumnMappings.Add("BuyPrice3", "BuyPrice3");
                    bulkCopy.ColumnMappings.Add("BuyPrice4", "BuyPrice4");
                    bulkCopy.ColumnMappings.Add("BuyPrice5", "BuyPrice5");
                    bulkCopy.ColumnMappings.Add("BuyVolume1", "BuyVolume1");
                    bulkCopy.ColumnMappings.Add("BuyVolume2", "BuyVolume2");
                    bulkCopy.ColumnMappings.Add("BuyVolume3", "BuyVolume3");
                    bulkCopy.ColumnMappings.Add("BuyVolume4", "BuyVolume4");
                    bulkCopy.ColumnMappings.Add("BuyVolume5", "BuyVolume5");
                    bulkCopy.ColumnMappings.Add("SalePrice1", "SalePrice1");
                    bulkCopy.ColumnMappings.Add("SalePrice2", "SalePrice2");
                    bulkCopy.ColumnMappings.Add("SalePrice3", "SalePrice3");
                    bulkCopy.ColumnMappings.Add("SalePrice4", "SalePrice4");
                    bulkCopy.ColumnMappings.Add("SalePrice5", "SalePrice5");
                    bulkCopy.ColumnMappings.Add("SaleVolume1", "SaleVolume1");
                    bulkCopy.ColumnMappings.Add("SaleVolume2", "SaleVolume2");
                    bulkCopy.ColumnMappings.Add("SaleVolume3", "SaleVolume3");
                    bulkCopy.ColumnMappings.Add("SaleVolume4", "SaleVolume4");
                    bulkCopy.ColumnMappings.Add("SaleVolume5", "SaleVolume5");
                    bulkCopy.ColumnMappings.Add("TickCount", "TickCount");

                    if (connection.State != ConnectionState.Open)
                        connection.Open();
                    bulkCopy.DestinationTableName = dataTable.TableName;
                    bulkCopy.WriteToServer(dataTable);
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }
            }
            return Task.FromResult<int>(rows);
        }

        #region Removed
        /*
        private Task<int> SaveStockDetailContentToDB(string code, string[] lines, string dealDate)
        {
            var rows = 0;
            if (lines != null && lines.Length > 1 && lines[0][0].Equals('成'))
            {
                DateTimeOffset dateTime;
                DateTimeOffset.TryParse(dealDate, out dateTime);
                var sqlHeader = new StringBuilder(@"
                         INSERT INTO [Detail]
                                ([DateTime]
                               ,[Code]
                               ,[Price]
                               ,[PriceOffset]
                               ,[Volume]
                               ,[Amount]
                               ,[Direction])
                         VALUES");
                var step = 900;

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
                    //dbHelper.ExecuteNonQueryHelper($"{sqlHeader.ToString()}{sqlValues.ToString()}".TrimEnd(','));
                    using (StockEntities stockEntities = new StockEntities())
                    {
                        //lock (objectLock)
                        //{
                        //rows += stockEntities.Database.ExecuteSqlCommand($"{sqlHeader.ToString()}{sqlValues.ToString()}".TrimEnd(','));
                        stockEntities.usp_SaveStockDetail(batchNumber: rows, stockCode: code, actionDate: dateTime, stockDetailSql: $"{sqlHeader.ToString()}{sqlValues.ToString()}".TrimEnd(','));
                        //}
                    }
                    sqlValues = null;
                }
                sqlHeader = null;
            }
            return Task.FromResult<int>(rows);
        }
        */
        /*
        private Task<int> SaveStockDetailContentToDB(string code, string[] lines, string dealDate)
        {
            var dataTable = new DataTable("Detail");
            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn("Id", typeof(Guid)),
                    new DataColumn("DateTime", typeof(DateTimeOffset)),
                    new DataColumn("Code", typeof(string)),
                    new DataColumn("Price", typeof(decimal)),
                    new DataColumn("PriceOffset", typeof(decimal)),
                    new DataColumn("Volume", typeof(int)),
                    new DataColumn("Amount", typeof(decimal)),
                    new DataColumn("Direction", typeof(string)),
                    //new DataColumn("LastUpdateTime", typeof(DateTimeOffset))
                });
            var rows = lines.Length - 1;
            if (lines != null && lines.Length > 1 && lines[0][0].Equals('成'))
            {
                DateTimeOffset dealDateTime;
                DateTimeOffset.TryParse(dealDate, out dealDateTime);

                DateTimeOffset dateTime;
                decimal price = default(decimal);
                decimal priceOffset = default(decimal);
                int volume = default(int);
                decimal amount = default(decimal);
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(new[] { '	' }, StringSplitOptions.RemoveEmptyEntries);
                    DateTimeOffset.TryParse($"{dealDate} {fields[0]}", out dateTime);
                    decimal.TryParse(fields[1], out price);
                    decimal.TryParse(fields[2].Equals("--") ? "0" : fields[2], out priceOffset);
                    int.TryParse(fields[3], out volume);
                    decimal.TryParse(fields[4], out amount);
                    //sqlValues.Append($"('{dealDate} {fields[0]}','{code}','{fields[1]}','{priceOffset}','{int.Parse(fields[3]) * 100}','{fields[4]}','{fields[5]}'),");
                    dataTable.Rows.Add(new object[] { Uuid.NewUuid(), dateTime, code, price, priceOffset, volume * 100, amount, fields[5] });
                }
                using (StockEntities stockEntities = new StockEntities())
                {
                    stockEntities.usp_SaveStockDetail(stockCode: code, actionDate: dealDateTime, stockDetailSql: null);

                    var bulkCopy = new SqlBulkCopy((SqlConnection)stockEntities.Database.Connection);
                    bulkCopy.DestinationTableName = dataTable.TableName;
                    bulkCopy.WriteToServer(dataTable);
                }
            }
            return Task.FromResult<int>(rows);
        }

        */
        /* 
        private Task<int> SaveStockDetailContentToDB(string code, string[] lines, string dealDate)
        {
            var rows = 0;
            var step = 100;
            if (lines.Length > 1)
            {
                for (int j = 1; j < lines.Length; j += step)
                {
                    var xml = new StringBuilder("<DetailItems>");
                    var k = Math.Min(j + step, lines.Length);
                    for (int i = j; i < k; i++)
                    {
                        var fields = lines[i].Split(new[] { '	' }, StringSplitOptions.RemoveEmptyEntries);
                        var priceOffset = fields[2].Equals("--") ? "0" : fields[2];
                        DateTimeOffset dateTime;
                        DateTimeOffset.TryParse($"{dealDate} {fields[0]}", out dateTime);
                        xml.Append($@"<Item>
        <DateTime>{dateTime}</DateTime>
        <Code>{code}</Code>
        <Price>{fields[1]}</Price>
        <PriceOffset>{priceOffset}</PriceOffset>
        <Volume>{int.Parse(fields[3]) * 100}</Volume>
        <Amount>{fields[4]}</Amount>
        <Direction>{fields[5]}</Direction>
        </Item>");

                        //rows++;
                    }
                    xml.Append("</DetailItems>");
                    using (StockEntities stockEntities = new StockEntities())
                    {
                        lock (objectLock)
                        {
                            rows += stockEntities.usp_SaveStockDetail(xml.ToString());
                        }
                    }
                }
                //xml.Append("</DetailItems>");
                //using (StockEntities stockEntities = new StockEntities())
                //{
                //    lock (objectLock)
                //    {
                //        return Task.FromResult<int>(stockEntities.usp_SaveStockDetail(xml.ToString()));
                //    }
                //}
            }
            return Task.FromResult<int>(rows);
        }
        */
        #endregion

        #endregion GetDetail
    }
}
