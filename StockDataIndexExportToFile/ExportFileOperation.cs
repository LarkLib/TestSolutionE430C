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
using System.Xml.Linq;

namespace StockDataIndexExportToFile
{
    class ExportFileOperation
    {
        private static readonly string Separator = ConfigurationManager.AppSettings["Separator"];
        private static readonly string DateTimeFormat = ConfigurationManager.AppSettings["DateTimeFormat"];
        private static readonly string ExportPath = ConfigurationManager.AppSettings["ExportPath"];
        private static readonly string ExtensionNane = ConfigurationManager.AppSettings["ExtensionNane"];
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["StockEntities"].ConnectionString;
        private static readonly int TimeStep = int.Parse(ConfigurationManager.AppSettings["TimeStep"]);
        private static readonly int Timeout = int.Parse(ConfigurationManager.AppSettings["Timeout"]);
        private static readonly DateTimeOffset startDate = DateTimeOffset.Parse("2012-03-01 00:00:00.0000000 +08:00");
        private static readonly DateTimeOffset endDate = DateTimeOffset.Parse("2017-03-01 00:00:00.0000000 +08:00");

        internal void ExportData()
        {
            var targetDir = Path.Combine(ExportPath, "Index");
            if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
            var fileName = Path.Combine(targetDir, $"IndexAllDaily.{ExtensionNane}");
            if (File.Exists(fileName)) File.Delete(fileName);
            File.WriteAllText(fileName, "DateTime,Volume,Amount,FlowingAmount,FlowingAmount100K,FlowingAmount500K,MarketAmountOpenPrice,MarketAmountHighestPrice,MarketAmountClosePrice,MarketAmountLowestPrice\r\n");
            for (DateTimeOffset date = startDate; date < endDate; date = date.AddMonths(TimeStep))
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var data = GetDataList(date);
                File.AppendAllLines(fileName, data);
                Console.WriteLine($"{date.ToString("yyyy-MM-dd")} {data.Count} {(int)stopwatch.ElapsedMilliseconds}ms");
                data = null;
            }
        }
        private IList<string> GetDataList(DateTimeOffset date)
        {
            var dataList = new List<string>();
            var endDate = date.AddMonths(TimeStep);
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandTimeout = Timeout;
                cmd.CommandText = "usp_GetDailyIndexExport";
                cmd.CommandType = CommandType.StoredProcedure;
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
            DateTimeOffset date;
            if (reader != null)
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var type = reader.GetDataTypeName(i);
                    if (i == 0)
                    {
                        date = reader.GetDateTimeOffset(0);
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
