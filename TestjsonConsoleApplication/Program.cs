using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestjsonConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestJson();
            TestGetWo();
            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }

        private static void TestGetWo()
        {
            var json = "{\"ResultCode\":\"200\",\"ErrorMsg\":\"\",\"Data\":{\"data\":[{\"CHANGE\":{},\"ALL\":{\"WO\":\"M181224001\",\"ReceiveQty\":1200,\"Barcode\":\"FGP1901003\",\"ProdDesc\":\"测试 CS-0001A 台湾大香蕉纸箱 香蕉10kg (外径 400*400*430 H1/H2/H3 :B)SCS-0001A\",\"WOByBOM\":\"M1812240011\",\"BOM\":\"CS-0001A\",\"Location\":\"A区\",\"SOCategory\":\"正常\",\"CustID\":\"CS\",\"ProdQty\":1200,\"CustName\":\"测试有限公司\",\"GroupManagementDistributedFactoryID\":\"\",\"ReceiveDate\":\"2019-01-21 00:00:00.000\",\"ProdDescShort\":\"台湾大香蕉纸箱 外径 (400*400*430)\",\"CustShortName\":\"测试\",\"LocSub\":\"A1\"}}]},\"ReceivedParam\":{\"CustNo\":\"A001\",\"EncryptStr\":\"5C9D1EC5A5AB4DFD8688D7AA6707BD09\",\"params\":{\"ActionID\":\"ODIStoreFGPreReceiveQuery\",\"GroupManagementDistributedFactoryID\":\"\",\"Data\":{\"Barcode\":\"FGP1901003\"}},\"Timestamp\":\"2019-01-22 09:57:03\",\"WEComConfigParams\":{\"WEDCNameMap\":{\"DI\":\"\"},\"functionID\":\"ODIAction\"},\"GroupManagementDistributedFactoryID\":\"\"}}";
            var jObject = (JObject)JsonConvert.DeserializeObject(json);
            var dataArray = (JArray)((JObject)jObject.GetValue("Data")).GetValue("data");
            foreach (var item in dataArray)
            {
                var all= ((JObject)item).GetValue("ALL");
                var wo = all.Value<string>("WO");
            }
        }
        #region Test Json

        private static void TestJson()
        {
            var json = "[{\"Id\":0,\"Name\":\"待确认\",\"FinalStatus\":0},{\"Id\":1,\"Name\":\"待收货\",\"FinalStatus\":0},{\"Id\":2,\"Name\":\"完成收货\",\"FinalStatus\":1},{\"Id\":3,\"Name\":\"生成中\",\"FinalStatus\":0},{\"Id\":4,\"Name\":\"超时关闭\",\"FinalStatus\":1},{\"Id\":5,\"Name\":\"生成失败\",\"FinalStatus\":1},{\"Id\":7,\"Name\":\"部分收货\",\"FinalStatus\":0},{\"Id\":8,\"Name\":\"已取消\",\"FinalStatus\":1},{\"Id\":9,\"Name\":\"取消中\",\"FinalStatus\":0},{\"Id\":10,\"Name\":\"取消失败\",\"FinalStatus\":1},{\"Id\":11,\"Name\":\"待审核\",\"FinalStatus\":0}]";
            var status = JsonConvert.DeserializeObject<IEnumerable<Status>>(json);

            Product product = new Product();

            product.Name = "Apple";
            product.ExpiryDate = new DateTime(2008, 12, 28);
            product.Price = 3.99M;
            product.Sizes = new string[] { "Small", "Medium", "Large" };

            string output = JsonConvert.SerializeObject(product);
            Console.WriteLine(output);
            //{"Name": "Apple","ExpiryDate": "2008-12-28T00:00:00","Price": 3.99,"Sizes": ["Small","Medium","Large"]}
            Product deserializedProduct = JsonConvert.DeserializeObject<Product>(output);
            Console.WriteLine(deserializedProduct.ToString());

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "json.txt")))
            using (JsonWriter writer = new JsonTextWriter(sw))
            //using (StreamWriter swConsole = new StreamWriter(Console.OpenStandardOutput()))
            //using (JsonWriter writerConsole = new JsonTextWriter(swConsole))
            {
                //swConsole.AutoFlush = true;
                //Console.SetOut(swConsole);
                //serializer.Serialize(writerConsole, product);
                serializer.Serialize(writer, product);
                // {"ExpiryDate":new Date(1230375600000),"Price":0}
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            //var jsonString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "houfuquan.js"));
            var jsonString = new WebClient().DownloadString("http://finance.sina.com.cn/realstock/company/sh600020/houfuquan.js");
            dynamic jArray = (JArray)JsonConvert.DeserializeObject(jsonString);
            dynamic jArray2 = JArray.Parse(jsonString);
            Console.WriteLine(jArray[0].total);
            foreach (var item in jArray[0].data)
            {
                Console.WriteLine($"{DateTimeOffset.ParseExact(item.Name, "_yyyy_MM_dd", null)}={item.Value}");
                break;
            }
            var houfaquanObj = JsonConvert.DeserializeObject<IList<Houfuquan>>(jsonString);
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            var client = new WebClient();
            var stockContent = client.DownloadString("http://api.finance.ifeng.com/akdaily/?code=sh600020&type=last");
            dynamic jArrayAkdaily = (JObject)JsonConvert.DeserializeObject(stockContent);
            JObject jArrayAkdaily2 = JObject.Parse(stockContent);
            DataTable dt = new DataTable("akdaily");
            var code = "sh600020";
            dt.Columns.AddRange(
                new[]
                {
                    new DataColumn("DateTime", typeof(DateTimeOffset)),
                    new DataColumn("Code", typeof(string)),
                    new DataColumn("OpenPrice", typeof(decimal)),
                    new DataColumn("HighestPrice", typeof(decimal)),
                    new DataColumn("ClosePrice", typeof(decimal)),
                    new DataColumn("LowestPrice", typeof(decimal)),
                    new DataColumn("Volume", typeof(long)),
                    new DataColumn("PriceChange", typeof(decimal)),
                    new DataColumn("PercentChange", typeof(decimal)),
                    new DataColumn("MA5", typeof(decimal)),
                    new DataColumn("MA10", typeof(decimal)),
                    new DataColumn("MA20", typeof(decimal)),
                    new DataColumn("VolumeMA5", typeof(decimal)),
                    new DataColumn("VolumeMA10", typeof(decimal)),
                    new DataColumn("VolumeMA20", typeof(decimal)),
                    new DataColumn("Turnover", typeof(decimal)),
                });

            string password = jArrayAkdaily2.Value<string>("password");
            foreach (JArray item in jArrayAkdaily2["record"])
            {
                var dataRow = dt.NewRow();
                dt.Rows.Add(new object[] { (DateTimeOffset)item[0], code, (decimal)item[1], (decimal)item[2], (decimal)item[3], (decimal)item[4], (long)((decimal)item[5]) * 100, (decimal)item[6], (decimal)item[7], (decimal)item[8], (decimal)item[9], (decimal)item[10], (decimal)item[11], (decimal)item[12], (decimal)item[13], (decimal)item[14] });
            }
            var bulkCopy = new SqlBulkCopy("");
            foreach (DataColumn item in dt.Columns)
            {
                bulkCopy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
            }
            //bulkCopy.ColumnMappings.Add("DateTime", "DateTime");
            //bulkCopy.ColumnMappings.Add("Code", "Code");
            //bulkCopy.ColumnMappings.Add("OpenPrice", "OpenPrice");
            //bulkCopy.ColumnMappings.Add("HighestPrice", "HighestPrice");
            //bulkCopy.ColumnMappings.Add("LowestPrice", "LowestPrice");
            //bulkCopy.ColumnMappings.Add("ClosePrice", "ClosePrice");
            //bulkCopy.ColumnMappings.Add("PriceChange", "PriceChange");
            //bulkCopy.ColumnMappings.Add("PercentChange", "PercentChange");
            //bulkCopy.ColumnMappings.Add("TurnoverRate", "TurnoverRate");
            //bulkCopy.ColumnMappings.Add("Volume", "Volume");
            //bulkCopy.ColumnMappings.Add("MA5", "MA5");
            //bulkCopy.ColumnMappings.Add("MA10", "MA10");
            //bulkCopy.ColumnMappings.Add("MA20", "MA20");
            //bulkCopy.ColumnMappings.Add("VolumeMA5", "VolumeMA5");
            //bulkCopy.ColumnMappings.Add("VolumeMA10", "VolumeMA10");
            //bulkCopy.ColumnMappings.Add("VolumeMA20", "VolumeMA20");

        }

        public class Houfuquan
        {
            //https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonPropertyAttribute.htm
            [JsonProperty("total", Order = 1)]
            public int Total { get; set; }
            [JsonProperty("data", Order = 2)]
            public Dictionary<string, decimal> Data { get; set; }
        }
        public class Product
        {
            //mast be public
            public string Name { get; set; }
            public DateTime ExpiryDate { get; set; }
            public decimal Price { get; set; }
            public string[] Sizes { get; set; }

            public override string ToString()
            {
                var sizeString = string.Empty;
                var joinString = string.Join(",", from s in Sizes where !s.StartsWith("m", StringComparison.OrdinalIgnoreCase) select s);
                Console.WriteLine(joinString);
                foreach (var item in Sizes)
                {
                    sizeString += $"{item},";
                }
                return $"Name={Name},ExpiryDate={ExpiryDate},Price={Price},Sizes={sizeString.TrimEnd(new[] { ',' })}";
            }
        }

        public partial class Status
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Nullable<int> FinalStatus { get; set; }
        }

        #endregion

    }
}
