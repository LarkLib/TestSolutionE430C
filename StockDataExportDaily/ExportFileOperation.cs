using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockDataExportDaily
{
    class ExportFileOperation
    {
        private static readonly string DataBaseList = ConfigurationManager.AppSettings["DataBaseList"];
        private static readonly string StockList = ConfigurationManager.AppSettings["StockList"];
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["StockEntities"].ConnectionString;
        private static readonly int MaxDbThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxDbThreadCount"]);
        private static readonly int MaxCodeThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxCodeThreadCount"]);
        private static readonly int TimeStep = int.Parse(ConfigurationManager.AppSettings["TimeStep"]);
        private static readonly string ConnectionFormatString = ConfigurationManager.AppSettings["ConnectionFormatString"] as string;
        private static readonly int Timeout = 600;
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

        internal void ExportData()
        {
            var endDate = DateTime.Parse("2017-04-01");
            var dbList = string.IsNullOrEmpty(DataBaseList) || DataBaseListArray == null || !DataBaseListArray.Any() ? GetDbList() : DataBaseListArray;
            //foreach (var db in dbList)
            Parallel.ForEach(dbList, new ParallelOptions() { MaxDegreeOfParallelism = MaxDbThreadCount }, db =>
            {
                var codeItemList = GetDateList(db);
                if (StockListArray != null && StockListArray.Any())
                {
                    codeItemList = codeItemList.Where(item => StockListArray.Contains(item.Code)).ToList();
                }
                Parallel.ForEach(codeItemList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, codeItem =>
                {
                    var date = codeItem.StartDate.Date;
                    for (var t = date; t < endDate; t = t.AddDays(TimeStep))
                    {
                        var sleepMs = 0;
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        var dataTable = GetData(db, codeItem.Code, t);
                        SaveDataTableToDb(dataTable, db);
                        var elapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds;
                        stopwatch.Stop();
                        if (elapsedMilliseconds > 5000)
                        {
                            sleepMs = elapsedMilliseconds / 5;
                            Thread.Sleep(sleepMs);
                        }
                        Console.WriteLine($"{db},{codeItem.Code},{t.ToString("yyyy-MM-dd")},{dataTable.Rows.Count},{elapsedMilliseconds}ms,{sleepMs}");
                    }
                }
                );
            }
            );
        }
        private IList<string> GetDbList()
        {
            var list = new List<string>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"select distinct db from CodeInfo with(nolock) order by db";
                cmd.CommandTimeout = Timeout;
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
                cmd.CommandText = "select distinct Code from CodeInfo  with(nolock) where status=1 and db=@db";
                cmd.CommandTimeout = Timeout;
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
        private IList<CodeItem> GetDateList(string database)
        {
            var list = new List<CodeItem>();
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, database)))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = Timeout;
                cmd.CommandText = $@"
                        select 
                        ci.code,
                        coalesce(
                            ri.datetime,
                            CASE WHEN si.ListingDate > cast('2012-03-01 00:00:00.0000000 +08:00' as datetimeoffset) 
                            THEN si.ListingDate 
                            ELSE cast('2012-03-01 00:00:00.0000000 +08:00' as datetimeoffset) END) datetime 
                        from  stockinfo..codeinfo ci with(nolock)
                        left join (select code, max(dateadd(d,1,datetime)) datetime from [Daily] with(nolock) group by code) as ri on ci.code = ri.code
                        join stockinfo..stockinfo si on ci.code= si.code
                        where ci.db = db_name()
                        order by ci.code, datetime";
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
        private DataTable GetData(string db, string code, DateTimeOffset date)
        {
            var dataTable = new DataTable("Daily");
            var endDate = date.AddDays(TimeStep);
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, db)))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "usp_GetDailyDataExport";
                cmd.CommandTimeout = Timeout;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@code", code) { SqlDbType = SqlDbType.NVarChar, Size = 50 });
                cmd.Parameters.Add(new SqlParameter("@beginDate", date) { SqlDbType = SqlDbType.DateTimeOffset });
                cmd.Parameters.Add(new SqlParameter("@endDate", endDate) { SqlDbType = SqlDbType.DateTimeOffset });
                connection.Open();
                var reader = cmd.ExecuteReader();
                dataTable.Load(reader);
                connection.Close();
            }
            return dataTable;
        }
        private DataTable GetDataTable()
        {
            var dataTable = new DataTable("Daily");
            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn("DateTime", typeof(DateTimeOffset)),
                    new DataColumn("Code", typeof(string)),
                    new DataColumn("Price", typeof(decimal)),
                    new DataColumn("PriceOffset", typeof(decimal)),
                    new DataColumn("TickCount", typeof(int)),
                    new DataColumn("Volume", typeof(int)),
                    new DataColumn("Amount", typeof(decimal)),
                    new DataColumn("Direction", typeof(string)),
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
                });
            return dataTable;
        }
        private void SaveDataTableToDb(DataTable dataTable, string db)
        {
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, db)))
            {
                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.BulkCopyTimeout = Timeout;
                bulkCopy.ColumnMappings.Add("DateTime", "DateTime");
                bulkCopy.ColumnMappings.Add("Code", "Code");
                bulkCopy.ColumnMappings.Add("OpenPrice", "OpenPrice");
                bulkCopy.ColumnMappings.Add("HighestPrice", "HighestPrice");
                bulkCopy.ColumnMappings.Add("ClosePrice", "ClosePrice");
                bulkCopy.ColumnMappings.Add("LowestPrice", "LowestPrice");
                bulkCopy.ColumnMappings.Add("Volume", "Volume");
                bulkCopy.ColumnMappings.Add("Amount", "Amount");
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
                bulkCopy.ColumnMappings.Add("FlowingAmount", "FlowingAmount");
                bulkCopy.ColumnMappings.Add("FlowingAmount100K", "FlowingAmount100K");
                bulkCopy.ColumnMappings.Add("FlowingAmount500K", "FlowingAmount500K");
                if (connection.State != ConnectionState.Open) connection.Open();
                bulkCopy.DestinationTableName = dataTable.TableName;
                bulkCopy.WriteToServer(dataTable);
                if (connection.State != ConnectionState.Closed) connection.Close();
            }
        }
        private class CodeItem
        {
            public DateTimeOffset StartDate { get; set; }
            public string Code { get; set; }
        }

    }
}
