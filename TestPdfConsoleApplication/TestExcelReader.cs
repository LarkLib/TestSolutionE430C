using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPdfConsoleApplication
{
    class TestExcelReader
    {
        private static readonly string ConnectionString = "data source=.;initial catalog=ShQny;integrated security=True;";
        internal void Execute()
        {
            ImportExcel();
        }
        private void ImportExcel()
        {
            //var tempOutFile = @"D:\MySpace\Work\HHY\购销明细\fields.txt";
            //if (File.Exists(tempOutFile)) File.Delete(tempOutFile);
            var files = Directory.GetFiles(@"D:\MySpace\Work\HHY\购销明细\", "*明细表.xls*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var dataTable = GetDataTable();
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith("~$")) continue;
                var filePath = file;
                //var filePath = @"D:\MySpace\Work\HHY\购销明细\8月份\8月15日进出明细表.xls";
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Choose one of either 1 or 2:

                        // 1. Use the reader methods
                        //do
                        //{
                        //    while (reader.Read())
                        //    {
                        //        // reader.GetDouble(0);
                        //    }
                        //} while (reader.NextResult());

                        // 2. Use the AsDataSet extension method
                        var result = reader.AsDataSet();
                        var tables = result.Tables.Count;
                        // The result of each spreadsheet is in result.Tables
                        foreach (DataTable table in result.Tables)
                        {
                            var rows = table.Rows;
                            var poNoRows = from row in table.AsEnumerable() where row.ItemArray.Contains("采购单号") select row;
                            foreach (DataRow row in poNoRows)
                            {
                                var index = table.Rows.IndexOf(row);
                                var itemList = row.ItemArray.ToList();
                                var poNo = itemList[itemList.IndexOf("采购单号") + 1];
                                var poiName = itemList[itemList.IndexOf("门店") + 1];
                                var cTime2 = DateTime.Parse(rows[index + 1].ItemArray[1].ToString());
                                var preArrivalTime2 = DateTime.Parse(rows[index + 1].ItemArray[7].ToString());
                                var i = 3;
                                while (index + i < rows.Count)
                                {
                                    if (rows[index + i].ItemArray[0] == null || 
                                        string.IsNullOrWhiteSpace(rows[index + i].ItemArray[0].ToString())||
                                        rows[index + i].ItemArray[0].ToString().StartsWith("总记")) break;

                                    var rowItem = dataTable.NewRow();
                                    rowItem["poNo"] = poNo;
                                    rowItem["poiName"] = poiName;
                                    rowItem["cTime2"] = cTime2;
                                    rowItem["preArrivalTime2"] = preArrivalTime2;
                                    rowItem["spuName"] = rows[index + i].ItemArray[0] as string;
                                    rowItem["skuSpec"] = rows[index + i].ItemArray[1] as string;
                                    rowItem["buyAmount"] = rows[index + i].ItemArray[2] as string;
                                    decimal buyPrice;
                                    rowItem["buyPrice"] = decimal.TryParse(rows[index + i].ItemArray[3].ToString(), out buyPrice) ? buyPrice : 0;
                                    decimal unitPrice;
                                    rowItem["unitPrice"] = decimal.TryParse(rows[index + i].ItemArray[4].ToString(), out unitPrice) ? unitPrice : 0;
                                    rowItem["skuDictUnitName"] = rows[index + i].ItemArray[5] as string;
                                    decimal availableQuantity;
                                    rowItem["availableQuantity"] = decimal.TryParse(rows[index + i].ItemArray[6].ToString(), out availableQuantity) ? availableQuantity : 0;

                                    decimal poAmount;
                                    rowItem["poAmount"] = decimal.TryParse(rows[index + i].ItemArray[7].ToString(), out poAmount) ? poAmount : 0;
                                    decimal sumPoPrice;
                                    rowItem["sumPoPrice"] = decimal.TryParse(rows[index + i].ItemArray[8].ToString(), out sumPoPrice) ? sumPoPrice : 0;
                                    rowItem["remark"] = rows[index + i].ItemArray[9] as string;
                                    rowItem["fileName"] = fileName;
                                    rowItem["tableName"] = table.TableName;
                                    dataTable.Rows.Add(rowItem);
                                    i++;
                                }
                                //if (row.ItemArray.Contains("商品名称"))
                                //{
                                //    var text = string.Empty;
                                //    foreach (var item in row.ItemArray)
                                //    {
                                //        text += $"{item},";
                                //    }
                                //    //File.AppendAllText(tempOutFile, $"{ fileName.PadRight(20)},{table.TableName.PadRight(20)},{text}{ Environment.NewLine}");
                                //}
                            }
                        }
                    }
                }
                SaveDetailContentToDB(dataTable);
                Console.WriteLine($"{fileName},rows:{dataTable.Rows.Count}");
                dataTable = null;
            }
        }
        private DataTable GetDataTable()
        {
            var dataTable = new DataTable("PoSkuRn");
            dataTable.Columns.AddRange(
                new[]
                {
                    new DataColumn("poNo", typeof(string)),
                    new DataColumn("spuName", typeof(string)),
                    new DataColumn("poiName", typeof(string)),
                    new DataColumn("cTime2", typeof(DateTime)),
                    new DataColumn("preArrivalTime2", typeof(DateTime)),
                    new DataColumn("skuSpec", typeof(string)),
                    new DataColumn("buyAmount", typeof(string)),
                    new DataColumn("buyPrice", typeof(decimal)),
                    new DataColumn("unitPrice", typeof(decimal)),
                    new DataColumn("skuDictUnitName", typeof(string)),
                    new DataColumn("availableQuantity", typeof(decimal)),
                    new DataColumn("poAmount", typeof(decimal)),
                    new DataColumn("sumPoPrice", typeof(decimal)),
                    new DataColumn("remark", typeof(string)),
                    new DataColumn("fileName", typeof(string)),
                    new DataColumn("tableName", typeof(string))
                });
            return dataTable;
        }
        private int SaveDetailContentToDB(DataTable dataTable)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.ColumnMappings.Add("poNo", "poNo");
                bulkCopy.ColumnMappings.Add("spuName", "spuName");
                bulkCopy.ColumnMappings.Add("poiName", "poiName");
                bulkCopy.ColumnMappings.Add("cTime2", "cTime2");
                bulkCopy.ColumnMappings.Add("preArrivalTime2", "preArrivalTime2");
                bulkCopy.ColumnMappings.Add("skuSpec", "skuSpec");
                bulkCopy.ColumnMappings.Add("buyAmount", "buyAmount");
                bulkCopy.ColumnMappings.Add("buyPrice", "buyPrice");
                bulkCopy.ColumnMappings.Add("unitPrice", "unitPrice");
                bulkCopy.ColumnMappings.Add("skuDictUnitName", "skuDictUnitName");
                bulkCopy.ColumnMappings.Add("availableQuantity", "availableQuantity");
                bulkCopy.ColumnMappings.Add("poAmount", "poAmount");
                bulkCopy.ColumnMappings.Add("sumPoPrice", "sumPoPrice");
                bulkCopy.ColumnMappings.Add("remark", "remark");
                bulkCopy.ColumnMappings.Add("fileName", "fileName");
                bulkCopy.ColumnMappings.Add("tableName", "tableName");
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                bulkCopy.DestinationTableName = dataTable.TableName;
                bulkCopy.BatchSize = 500;
                bulkCopy.WriteToServer(dataTable);
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
            }
            return 0;
        }
    }
}

