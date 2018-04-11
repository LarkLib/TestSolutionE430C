using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace StockDataImportToMultiDb
{
    internal class ImportStockToMultiDbOperation
    {
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["StockEntities"].ConnectionString;
        private static int MaxDbThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxDbThreadCount"]);
        private static int MaxCodeThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxCodeThreadCount"]);
        private static string ConnectionFormatString = ConfigurationManager.AppSettings["ConnectionFormatString"] as string;
        internal void ImportData()
        {
            var dbList = GetDbList();
            //foreach (var db in dbList)
            Parallel.ForEach(dbList, new ParallelOptions() { MaxDegreeOfParallelism = MaxDbThreadCount }, db =>
            {
                var codeList = GetCodeList(db);
                //foreach (var code in codeList)
                Parallel.ForEach(codeList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, code =>
                {
                    var dateList = GetDateList(db, code);
                    foreach (var date in dateList)
                    {
                        var fileDictionary = GetFileDictionary(db, code, date);
                        var dataTable = GetDataTable();
                        foreach (var file in fileDictionary)
                        {
                            var infoString = $"{db} {code} {date.ToString("yyyy-MM")} ";
                            var stopwatch = new Stopwatch();
                            stopwatch.Start();
                            var totalMs = 0L;
                            dataTable.Clear();
                            AddRowsToDataTable(ref dataTable, File.ReadAllLines(file.Key));
                            infoString += $"Rows:{dataTable.Rows.Count,-6}Read:{(int)stopwatch.ElapsedMilliseconds,-5}";
                            totalMs += stopwatch.ElapsedMilliseconds;
                            stopwatch.Restart();
                            using (var scop = new TransactionScope())
                            {
                                SaveDataTableToDb(dataTable, db);
                                UpdateFileInfoStatusToDb(file.Value, dataTable.Rows.Count);
                                totalMs += stopwatch.ElapsedMilliseconds;
                                infoString += $"SaveToDB:{stopwatch.ElapsedMilliseconds,-5}Total:{totalMs,-6}";
                                scop.Complete();
                            }
                            Console.WriteLine(infoString);
                        }
                        dataTable = null;
                    }
                }
                );
            }
            );
        }
        internal void ImportData2()
        {
            var dbList = GetDbList().Take(1);
            //foreach (var db in dbList)
            Parallel.ForEach(dbList, new ParallelOptions() { MaxDegreeOfParallelism = MaxDbThreadCount }, db =>
            {
                var codeList = GetCodeList(db);
                //foreach (var code in codeList)
                Parallel.ForEach(codeList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, code =>
                {
                    var dateList = GetDateList(db, code);
                    foreach (var date in dateList)
                    {
                        var infoString = $"{db} {code} {date.ToString("yyyy-MM")} ";
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        var totalMs = 0l;
                        var fileDictionary = GetFileDictionary(db, code, date);
                        var dataTable = GetDataTable();
                        foreach (var file in fileDictionary.Keys)
                        {
                            AddRowsToDataTable(ref dataTable, File.ReadAllLines(file));
                        }
                        infoString += $"Rows:{dataTable.Rows.Count,-6}Read:{(int)stopwatch.ElapsedMilliseconds,-5}";
                        totalMs += stopwatch.ElapsedMilliseconds;
                        stopwatch.Restart();
                        using (var scop = new TransactionScope())
                        {
                            SaveDataTableToDb(dataTable, db);
                            UpdateFileInfoStatusToDb(fileDictionary.Values);
                            totalMs += stopwatch.ElapsedMilliseconds;
                            infoString += $"SaveToDB:{stopwatch.ElapsedMilliseconds,-5}Total:{totalMs,-6}";
                            scop.Complete();
                        }
                        dataTable = null;
                        Console.WriteLine(infoString);
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
                cmd.CommandText = "select distinct db from FileInfo with(nolock) where status=0 order by db";
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
                cmd.CommandText = "select distinct Code from FileInfo  with(nolock) where status=0 and db=@db";
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
        private IList<DateTimeOffset> GetDateList(string db, string code)
        {
            var list = new List<DateTimeOffset>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select distinct [Date] from FileInfo with(nolock) where status=0 and db=@db and code=@code order by [Date]";
                cmd.Parameters.Add(new SqlParameter("@db", db) { SqlDbType = SqlDbType.NVarChar, Size = 50 });
                cmd.Parameters.Add(new SqlParameter("@code", code) { SqlDbType = SqlDbType.NVarChar, Size = 50 });
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var date = reader.GetDateTimeOffset(0);
                    var dateMonth = date.AddDays(1 - date.Day);
                    if (!list.Contains(dateMonth))
                    {
                        list.Add(dateMonth);
                    }
                }
                connection.Close();
            }
            return list;
        }
        private IDictionary<string, Guid> GetFileDictionary(string db, string code, DateTimeOffset date)
        {
            var dictionary = new Dictionary<string, Guid>();
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select FullName,id from FileInfo with(nolock) where status=0 and db=@db  and code=@code and date>=@beginDate and date<@endDate order by [FullName]";
                cmd.Parameters.Add(new SqlParameter("@db", db) { SqlDbType = SqlDbType.NVarChar, Size = 50 });
                cmd.Parameters.Add(new SqlParameter("@code", code) { SqlDbType = SqlDbType.NVarChar, Size = 50 });
                cmd.Parameters.Add(new SqlParameter("@beginDate", date) { SqlDbType = SqlDbType.DateTimeOffset });
                cmd.Parameters.Add(new SqlParameter("@endDate", date.AddMonths(1)) { SqlDbType = SqlDbType.DateTimeOffset });
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dictionary.Add(reader.GetString(0), reader.GetGuid(1));
                }
                connection.Close();
            }
            return dictionary;
        }
        private DataTable GetDataTable()
        {
            var dataTable = new DataTable("Detail");
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
        private void AddRowsToDataTable(ref DataTable dataTable, string[] lines)
        {
            if (lines != null && lines.Length > 1)
            {
                DateTimeOffset dateTime;
                decimal price = default(decimal);
                decimal priceOffset = default(decimal);
                decimal volume = default(decimal);
                decimal amount = default(decimal);
                var perPrice = default(decimal);
                for (int i = 1; i < lines.Length; i++)
                {
                    var fields = lines[i].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (fields.Length != 28) continue;
                    DateTimeOffset.TryParse($"{fields[2]}", out dateTime);
                    decimal.TryParse(fields[3], out price);
                    priceOffset = perPrice == default(decimal) ? 0m : perPrice - price;
                    perPrice = price;
                    decimal.TryParse(fields[6], out volume);
                    decimal.TryParse(fields[5], out amount);
                    dataTable.Rows.Add(new object[]
                    {
                        dateTime,                           //DateTime    
                        $"{fields[0]}{fields[1]}",          //Code        
                        price,                              //Price       
                        priceOffset,                        //PriceOffset 
                        int.Parse(fields[4]),               //TickCount      
                        (int)(volume * 100),                //Volume      
                        amount,                             //Amount      
                        fields[7],                          //Direction   
                        decimal.Parse(fields[8]),           //BuyPrice1   
                        decimal.Parse(fields[9]),           //BuyPrice2   
                        decimal.Parse(fields[10]),          //BuyPrice3   
                        decimal.Parse(fields[11]),          //BuyPrice4   
                        decimal.Parse(fields[12]),          //BuyPrice5   
                        decimal.Parse(fields[13]),          //SalePrice1  
                        decimal.Parse(fields[14]),          //SalePrice2  
                        decimal.Parse(fields[15]),          //SalePrice3  
                        decimal.Parse(fields[16]),          //SalePrice4  
                        decimal.Parse(fields[17]),          //SalePrice5  
                        int.Parse(fields[18]),              //BuyVolume1  
                        int.Parse(fields[19]),              //BuyVolume2  
                        int.Parse(fields[20]),              //BuyVolume3  
                        int.Parse(fields[21]),              //BuyVolume4  
                        int.Parse(fields[22]),              //BuyVolume5  
                        int.Parse(fields[23]),              //SaleVolume1 
                        int.Parse(fields[24]),              //SaleVolume2 
                        int.Parse(fields[25]),              //SaleVolume3
                        int.Parse(fields[26]),              //SaleVolume4
                        int.Parse(fields[27])               //SaleVolume5
                    });
                }
            }
        }
        private void SaveDataTableToDb(DataTable dataTable, string db)
        {
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, db)))
            {
                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.ColumnMappings.Add("DateTime", "DateTime");
                bulkCopy.ColumnMappings.Add("Code", "Code");
                bulkCopy.ColumnMappings.Add("Price", "Price");
                bulkCopy.ColumnMappings.Add("PriceOffset", "PriceOffset");
                bulkCopy.ColumnMappings.Add("TickCount", "TickCount");
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
                if (connection.State != ConnectionState.Open) connection.Open();
                bulkCopy.DestinationTableName = dataTable.TableName;
                bulkCopy.WriteToServer(dataTable);
                if (connection.State != ConnectionState.Closed) connection.Close();
            }
        }
        private void UpdateFileInfoStatusToDb(IEnumerable<Guid> ids)
        {
            var idList = string.Empty;
            foreach (var id in ids)
            {
                idList += $"'{id}',";
            }
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"update fileinfo set status=1 where id in({idList.TrimEnd(new char[] { ',' })})";
                if (connection.State != ConnectionState.Open) connection.Open();
                cmd.ExecuteNonQuery();
                if (connection.State != ConnectionState.Closed) connection.Close();
            }
        }
        private void UpdateFileInfoStatusToDb(Guid id, int rows)
        {
            var idList = string.Empty;
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"update fileinfo set status=1,rows=@rows where id=@id";
                cmd.Parameters.Add(new SqlParameter("@id", id) { SqlDbType = SqlDbType.UniqueIdentifier });
                cmd.Parameters.Add(new SqlParameter("@rows", rows) { SqlDbType = SqlDbType.Int });
                if (connection.State != ConnectionState.Open) connection.Open();
                cmd.ExecuteNonQuery();
                if (connection.State != ConnectionState.Closed) connection.Close();
            }
        }
    }
}
