using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestjsonConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            TestJson();
            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }
        #region Test Json
        private static void TestJson()
        {
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
            var jsonString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "houfuquan.js"));
            dynamic jArray = (JArray)JsonConvert.DeserializeObject(jsonString);
            dynamic jArray2 = JArray.Parse(jsonString);
            Console.WriteLine(jArray[0].total);
            foreach (var item in jArray[0].data)
            {
                Console.WriteLine($"{item.Name}={item.Value}");
            }
            var houfaquanObj = JsonConvert.DeserializeObject<IList<Houfuquan>>(jsonString);
        }

        public class Houfuquan
        {
            public int total { get; set; }
            public Dictionary<string, decimal> data { get; set; }
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
        #endregion

    }
}
