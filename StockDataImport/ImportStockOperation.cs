using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataImport
{
    internal class ImportStockOperation
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["StockEntities"].ConnectionString;
        private static string importPath = ConfigurationManager.AppSettings["ImportPath"];
        private static string BackuptPath = ConfigurationManager.AppSettings["BackuptPath"];
        private static int maxThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxThreadCount"]);
        internal void ImportData()
        {
            var dirsL1 = Directory.EnumerateDirectories(importPath);
            foreach (var dirL1 in dirsL1)
            {
                var dirsL2 = Directory.EnumerateDirectories(dirL1);
                foreach (var dirL2 in dirsL2)
                {
                    var files = Directory.GetFiles(dirL2, "*.csv").AsParallel();
                    Parallel.ForEach(files, file =>
                    {
                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        var lines = File.ReadAllLines(file, Encoding.GetEncoding(936));
                        var rows = SaveStockDetailContentToDB(lines);
                        Console.WriteLine($"{Path.GetFileName(file)}\trows:{rows,-4}\t{(int)stopwatch.ElapsedMilliseconds}ms");
                        var backupFile = file.Replace(importPath, BackuptPath);
                        var backupDir = Path.GetDirectoryName(backupFile);
                        if (!Directory.Exists(backupDir))
                        {
                            Directory.CreateDirectory(backupDir);
                        }
                        File.Move(file, backupFile);
                    });
                }
            }

        }
        private int SaveStockDetailContentToDB(string[] lines)
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
            var rows = lines.Length - 1;
            if (lines != null && lines.Length > 1 && lines[0][0].Equals('市'))
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
                using (var connection = new SqlConnection(connectionString))
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
                    if (connection.State != ConnectionState.Open)
                        connection.Open();
                    bulkCopy.DestinationTableName = dataTable.TableName;
                    bulkCopy.WriteToServer(dataTable);
                    if (connection.State != ConnectionState.Closed)
                        connection.Close();
                }
            }
            return rows;
        }
    }
}
