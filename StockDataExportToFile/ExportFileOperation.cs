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

namespace StockDataExportToFile
{
    class ExportFileOperation
    {
        enum EnumExportType { Minutely, Daliy }
        private static readonly EnumExportType ExportType = (EnumExportType)Enum.Parse(typeof(EnumExportType), ConfigurationManager.AppSettings["ExportType"]);
        private static readonly string Separator = ConfigurationManager.AppSettings["Separator"];
        private static readonly string DateTimeFormat = ConfigurationManager.AppSettings["DateTimeFormat"];
        private static readonly string ExportPath = ConfigurationManager.AppSettings["ExportPath"];
        private static readonly string ExtensionNane = ConfigurationManager.AppSettings["ExtensionNane"];
        private static readonly string DataBaseList = ConfigurationManager.AppSettings["DataBaseList"];
        private static readonly string StockList = ConfigurationManager.AppSettings["StockList"];
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["StockEntities"].ConnectionString;
        private static readonly int MaxDbThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxDbThreadCount"]);
        private static readonly int MaxCodeThreadCount = int.Parse(ConfigurationManager.AppSettings["MaxCodeThreadCount"]);
        private static readonly string ConnectionFormatString = ConfigurationManager.AppSettings["ConnectionFormatString"] as string;
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
            var targetDir = Path.Combine(ExportPath, ExportType.ToString());
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
            var dbList = string.IsNullOrEmpty(DataBaseList) || DataBaseListArray == null || !DataBaseListArray.Any() ? GetDbList() : DataBaseListArray;
            //foreach (var db in dbList)
            Parallel.ForEach(dbList, new ParallelOptions() { MaxDegreeOfParallelism = MaxDbThreadCount }, db =>
            {
                var codeList = GetCodeList(db);
                if (StockListArray != null && StockListArray.Any())
                {
                    codeList = codeList.Where(code => StockListArray.Contains(code)).ToList();
                }
                //foreach (var code in codeList)
                Parallel.ForEach(codeList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, code =>
                {
                    var dateList = GetDateList(db, code);
                    var fileName = Path.Combine(targetDir, $"{code}.{ExtensionNane}");
                    if (!File.Exists(fileName))
                    {
                        File.WriteAllText(fileName, "date,y1,y2,y3,y4,y5,y6,y7,y8,y9,y10,y11,y12,y13,y14,y15,y16,y17,y18,y19,y20,y21,y22,y23,y24,y25,y26,y27,y28,y29\r\n".Replace(",", Separator));
                        foreach (var date in dateList)
                        //Parallel.ForEach(dateList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, date =>
                        {
                            var stopwatch = new Stopwatch();
                            stopwatch.Start();
                            var data = GetDataList(db, code, date);
                            File.AppendAllLines(fileName, data);
                            Console.WriteLine($"{code}_{ExportType}_{date.ToString("yyyy-MM-dd")} {data.Count} {(int)stopwatch.ElapsedMilliseconds}ms");
                            data = null;
                            if (stopwatch.ElapsedMilliseconds>5000)
                            {
                                Thread.Sleep((int)(stopwatch.ElapsedMilliseconds / 5));
                            }
                        }
                        //);
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
                cmd.CommandText = "select distinct [Date] from FileInfo with(nolock) where status=1 and db=@db and code=@code order by [Date]";
                cmd.Parameters.Add(new SqlParameter("@db", db) { SqlDbType = SqlDbType.NVarChar, Size = 50 });
                cmd.Parameters.Add(new SqlParameter("@code", code) { SqlDbType = SqlDbType.NVarChar, Size = 50 });
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var date = reader.GetDateTimeOffset(0).Date;
                    var dateYear = date.AddDays(1 - date.DayOfYear);
                    var dateItem = ExportType == EnumExportType.Daliy ? dateYear : date;
                    if (!list.Contains(dateItem))
                    {
                        list.Add(dateItem);
                    }
                }
                connection.Close();
            }
            return list;
        }
        private IList<string> GetDataList(string db, string code, DateTimeOffset date)
        {
            var dataList = new List<string>();
            var endDate = ExportType == EnumExportType.Minutely ? date.AddDays(1) : date.AddYears(1);
            using (var connection = new SqlConnection(string.Format(ConnectionFormatString, db)))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandTimeout = 600;
                cmd.CommandText = ExportType == EnumExportType.Minutely ? "usp_GetMinutelyData" : "usp_GetDailyData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@code", code) { SqlDbType = SqlDbType.NVarChar, Size = 50 });
                cmd.Parameters.Add(new SqlParameter("@beginDate", date) { SqlDbType = SqlDbType.DateTimeOffset });
                cmd.Parameters.Add(new SqlParameter("@endDate", endDate) { SqlDbType = SqlDbType.DateTimeOffset });
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dataList.Add(GetData(reader));
                    //dataList.Add($"{DateTimeOffset.Parse(reader.GetString(0)).ToString(DateTimeFormat)}{Separator}{reader.GetDecimal(1):#.00}{Separator}{reader.GetDecimal(2):#.00}{Separator}{reader.GetDouble(3):#.00}{Separator}{reader.GetInt32(4)}{Separator}{reader.GetDecimal(5):#.00}{Separator}{reader.GetInt32(6)}");
                }
                connection.Close();
            }
            return dataList;
        }

        private string GetData(SqlDataReader reader)
        {
            var dataBuilder = new StringBuilder();
            if (reader != null)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var type = reader.GetDataTypeName(i);
                    if (i == 0)
                    {
                        var date = reader.GetDateTimeOffset(0);
                        dataBuilder.Append(date.ToString(DateTimeFormat));
                    }
                    else
                    {
                        switch (type)
                        {
                            case "decimal":
                                dataBuilder.Append(Separator);
                                dataBuilder.Append(reader.GetDecimal(i).ToString("#.00"));
                                break;
                            case "float":
                                dataBuilder.Append(Separator);
                                dataBuilder.Append(reader.GetFloat(i).ToString("#.00"));
                                break;
                            case "double":
                                dataBuilder.Append(Separator);
                                dataBuilder.Append(reader.GetDouble(i).ToString("#.00"));
                                break;
                            default:
                                dataBuilder.Append(Separator);
                                dataBuilder.Append(reader[i].ToString());
                                break;
                        }
                    }
                }
            }
            return dataBuilder.ToString();
        }
    }
}
