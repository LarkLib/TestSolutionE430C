using HtmlAgilityPack;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Media;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //PayrollTest();
            //FsetRegExp();
            //WebClientTestGetStock();
            //InvokeJsTest();
            //WebClientTestGetStockDetail();
            //WebClientTestGetStockSummary();
            //ParseDetailDataTDB();
            //ParseSummaryDataTDB();
            //TestTread();
            //TestShareMemory();
            //TestTimer();
            WebClientTestGetMiaoJiangGuShi2();
            //TestMethodIronPython();
            //new StockInfoOps().WebClientTestGetStockInfo(false);
            //new StockInfoOps().WebClientTestGetStockCapitalStructure();
        }

        private static Timer threadingTimer;
        private static void TestTimer()
        {
            var count = 0;
            threadingTimer = new Timer((state) =>
            {
                if (count > 20)
                {
                    threadingTimer.Change(-1, -1);
                }
                count++;
                Console.WriteLine(DateTime.Now.TimeOfDay.ToString());
            }, null, 0, 1000);
            Console.ReadKey();
        }

        private static void InvokeJsTest()
        {
            MSScriptControl.ScriptControl scc = new MSScriptControl.ScriptControl();
            scc.Language = "javascript";
            scc.AddCode("var m=6;;");
            var result = scc.Eval("var w=10;t=2;p=w/t*m");
            Console.WriteLine(result);


            //Warning CS0618  'VsaEngine' is obsolete: 'Use of this type is not recommended because it is being deprecated in Visual Studio 2005; 
            //there will be no replacement for this feature.Please see the ICodeCompiler documentation for additional help.
#pragma warning disable CS0618
            Microsoft.JScript.Vsa.VsaEngine vsaEngine = Microsoft.JScript.Vsa.VsaEngine.CreateEngine();
#pragma warning restore CS0618
            result = Microsoft.JScript.Eval.JScriptEvaluate("var w=10;var t=2;var p=w/t", vsaEngine);
            Console.WriteLine(result);

            object[] para = new object[] { 10, 5 };
            string jscript = @"
                package pag
                {
                    public class JScript
                    {
                        public static function test(a,b)
                        {
                            return a/b;
                        }
                    }
                }";
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            CodeDomProvider provider = new Microsoft.JScript.JScriptCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, jscript);
            Assembly assembly = results.CompiledAssembly;
            Type evaluateType = assembly.GetType("pag.JScript");
            var resultObj = evaluateType.InvokeMember("test", BindingFlags.InvokeMethod, null, null, para);
            Console.WriteLine(resultObj);

        }
        private static void WebClientTestGetStock()
        {
            //string stockAddress = "http://hq.sinajs.cn/list=sh166105,sh201000,sh201001,sh201002,sh201003,sh201004,sh201005,sh201008,sh201009,sh201010,sh202001,sh202003,sh202007,sh203007,sh203008,sh203009,sh203016,sh203017,sh203018,sh203019,sh203020,sh203021,sh203031,sh203032,sh203033,sh203040,sh203041,sh203042,sh203043,sh203044,sh203045,sh203049,sh203050,sh203051,        ,sh203053,sh203054,sh204001,sh204002,sh204003,sh204004,sh204007,sh204014,sh204028,sh204091,sh204182,sh500001,sh500002,sh500003,sh500005,sh500006,sh500007,sh500008,sh500009,sh500011,sh500015,sh500018,sh500029,sh500038,sh500056,sh500058,sh501000,sh501001,sh501002,sh501005,sh501015,sh501017,sh501018,sh501021,sh501022,sh501023,sh501025,sh501026,sh501050,sh502000,sh502001,sh502002,sh502003,sh502004,sh502005,sh502006,sh502007,sh502008,sh502010,sh502011,sh502012,sh502013,sh502014,sh502015,sh502016,sh502017,sh502018,sh502020,sh502021,sh502022,sh502023,sh502024,sh502025,sh502026,sh502027,sh502028,sh502031,sh502032,sh502036,sh502037,sh502038,sh502040,sh502041,sh502042,sh502048,sh502049,sh502050,sh502053,sh502054,sh502055,sh502056,sh502057,sh502058,sh505888,sh510010,sh510020,sh510030,sh510050,sh510060,sh510061,sh510070,sh510090,sh510110,sh510120,sh510130,sh510150,sh510160,sh510170,sh510180,sh510190,sh510210,sh510220,sh510230,sh510260,sh510270,sh510280,sh510290,sh510300,sh510310,sh510330,sh510360,sh510410,sh510420,sh510430,sh510440,sh510450,sh510500,sh510510,sh510520,sh510560,sh510580,sh510610,sh510620,sh510630,sh510650,sh510660,sh510680,sh510700,sh510710,sh510810,sh510880,sh510900,sh511010,sh511210,sh511220,sh511600,sh511660,sh511680,sh511690,sh511700,sh511760,sh511800,sh511810,sh511820,sh511830,sh511850,sh511860,sh511880,sh511890,sh511900,sh511910,sh511920,sh511930,sh511950,sh511960,sh511970,sh511980,sh511990,sh512000,sh512010,sh512070,sh512100,sh512110,sh512120,sh512210,sh512220,sh512230,sh512300,sh512310,sh512330,sh512340,sh512500,sh512510,sh512600,sh512610,sh512640,sh512680,sh512810,sh512990,sh513030,sh513100,sh513500,sh513600,sh513660,sh518800,sh518880,sh580012,sh580013,sh580014,sh580016,sh580017,sh580019,sh580020,sh580021,sh580022,sh580023,sh580024,sh580025,sh580026,sh580027,sh600000,sh600001,sh600002,sh600003,sh600004,sh600005,sh600006,sh600007,sh600008,sh600009,sh600010,sh600011,sh600012,sh600015,sh600016,sh600017,sh600018,sh600019,sh600020,sh600021,sh600022,sh600023,sh600026,sh600027,sh600028,sh600029,sh600030,sh600031,sh600033,sh600035,sh600036,sh600037,sh600038,sh600039,sh600048,sh600050,sh600051,sh600052,sh600053,sh600054,sh600055,sh600056,sh600057,sh600058,sh600059,sh600060,sh600061,sh600062,sh600063,sh600064,sh600065,sh600066,sh600067,sh600068,sh600069,sh600070,sh600071,sh600072,sh600073,sh600074,sh600075,sh600076,sh600077,sh600078,sh600079,sh600080,sh600081,sh600082,sh600083,sh600084,sh600085,sh600086,sh600087,sh600088,sh600089,sh600090,sh600091,sh600092,sh600093,sh600094,sh600095,sh600096,sh600097,sh600098,sh600099,sh600100,sh600101,sh600102,sh600103,sh600104,sh600105,sh600106,sh600107,sh600108,sh600109,sh600110,sh600111,sh600112,sh600113,sh600114,sh600115,sh600116,sh600117,sh600118,sh600119,sh600120,sh600121,sh600122,sh600123,sh600125,sh600126,sh600127,sh600128,sh600129,sh600130,sh600131,sh600132,sh600133,sh600135,sh600136,sh600137,sh600138,sh600139,sh600141,sh600143,sh600145,sh600146,sh600148,sh600149,sh600150,sh600151,sh600152,sh600153,sh600155,sh600156,sh600157,sh600158,sh600159,sh600160,sh600161,sh600162,sh600163,sh600165,sh600166,sh600167,sh600168,sh600169,sh600170,sh600171,sh600172,sh600173,sh600175,sh600176,sh600177,sh600178,sh600179,sh600180,sh600181,sh600182,sh600183,sh600184,sh600185,sh600186,sh600187,sh600188,sh600189,sh600190,sh600191,sh600192,sh600193,sh600195,sh600196,sh600197,sh600198,sh600199,sh600200,sh600201,sh600202,sh600203,sh600205,sh600206,sh600207,sh600208,sh600209,sh600210,sh600211,sh600212,sh600213,sh600215,sh600216,sh600217,sh600218,sh600219,sh600220,sh600221,sh600222,sh600223,sh600225,sh600226,sh600227,sh600228,sh600229,sh600230,sh600231,sh600232,sh600233,sh600234,sh600235,sh600236,sh600237,sh600238,sh600239,sh600240,sh600241,sh600242,sh600243,sh600246,sh600247,sh600248,sh600249,sh600250,sh600251,sh600252,sh600253,sh600255,sh600256,sh600257,sh600258,sh600259,sh600260,sh600261,sh600262,sh600263,sh600265,sh600266,sh600267,sh600268,sh600269,sh600270,sh600271,sh600272,sh600273,sh600275,sh600276,sh600277,sh600278,sh600279,sh600280,sh600281,sh600282,sh600283,sh600284,sh600285,sh600286,sh600287,sh600288,sh600289,sh600290,sh600291,sh600292,sh600293,sh600295,sh600296,sh600297,sh600298";
            //string stockAddress = "http://hq.sinajs.cn/list=sh601003,sh601001";
            //string stockAddress = "http://hq.sinajs.cn/list=s_sh000001";
            string stockAddress = "http://hq.sinajs.cn/list=sz399001,sz399101,sz399102,sh000001";

            WebClient client = new WebClient();
            SoundPlayer player = new SoundPlayer(@"提示.wav");          //声音
            player.Play();
            var stockContentBuilder = new StringBuilder();
            for (var i = 0; i < 20; i++)
            {
                var beginDateTime = DateTime.Now;
                Console.WriteLine(beginDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                var stockContent = client.DownloadString(stockAddress);
                //stockContentBuilder.AppendLine(stockContent);
                File.AppendAllText(@"C:\Users\admin\Desktop\Web\pages\stockData\testdata.txt", stockContent + Environment.NewLine);

                var endDateTime = DateTime.Now;
                Console.WriteLine(endDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Console.WriteLine((endDateTime - beginDateTime).Milliseconds);
                Thread.Sleep(1000);
            }

            //Console.WriteLine(stockContent);
            //for (int i = 0; i < 3; i++)
            //{
            //    System.Threading.Thread.Sleep(2000);
            //    player.Play();
            //}
            player.Play();
            Console.ReadKey();
            //WebRequest request =WebRequest.Create(stockAddress);

        }
        private static void WebClientTestGetStockDetail()
        {
            WebClient client = new WebClient();
            SoundPlayer player = new SoundPlayer(@"提示.wav");          //声音
            player.Play();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var stockCode = "sh000001";
            for (var t = DateTime.Parse("2010-01-01"); t <= DateTime.Now; t = t.AddDays(1))
            {
                var tString = t.ToString("yyyy-MM-dd");
                string stockAddress = string.Format("http://market.finance.sina.com.cn/downxls.php?date={0}&symbol={1}", tString, stockCode);
                var stockContent = client.DownloadString(stockAddress);
                if (!stockContent.Contains("当天没有数据"))
                {
                    var fileName = string.Format(@"C:\Users\admin\Desktop\Web\pages\stockData\detail\{1}_{0}.xls", tString, stockCode);
                    File.WriteAllText(fileName, stockContent);
                    Console.WriteLine(string.Format("{0} --- {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), t.ToString("yyyy-MM-dd")));
                }
                else
                {
                    Console.WriteLine(string.Format("{0} --- {1} 当天没有数据", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), t.ToString("yyyy-MM-dd")));
                }

            }
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //Console.WriteLine(stockContent);
            //for (int i = 0; i < 3; i++)
            //{
            //    System.Threading.Thread.Sleep(2000);
            //    player.Play();
            //}
            player.Play();
            Console.ReadKey();
            //WebRequest request =WebRequest.Create(stockAddress);

        }
        private static void WebClientTestGetStockSummary()
        {
            string stockAddress = "http://market.finance.sina.com.cn/downxls.php?date=2016-12-03&symbol=sz000802";
            //string stockAddress = "http://market.finance.sina.com.cn/downxls.php?date=2016-12-30&symbol=sz000802";
            WebClient client = new WebClient();
            SoundPlayer player = new SoundPlayer(@"提示.wav");          //声音
            player.Play();
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //client.DownloadFile(stockAddress,string.Format( @"C:\Users\admin\Desktop\Web\pages\{0}_sh000802.xls", DateTime.Now.ToString("yyyy-MM-dd")));
            for (var t = DateTime.Parse("2017-01-01"); t < DateTime.Parse("2017-01-03"); t = t.AddYears(1))
            {
                for (int i = 1; i <= 4; i++)
                {
                    var tString = t.ToString("yyyy-MM-dd");
                    stockAddress = string.Format("http://vip.stock.finance.sina.com.cn/corp/go.php/vMS_MarketHistory/stockid/000802.phtml?year={0}&jidu={1}", t.Year, i);
                    var stockContent = client.DownloadString(stockAddress);
                    if (stockContent.Length > 57000)
                    {
                        var fileName = string.Format(@"C:\Users\admin\Desktop\Web\pages\stockData\summary\sh000802_{0}_0{1}.html", t.Year, i);
                        File.WriteAllText(fileName, stockContent, Encoding.UTF8);
                        Console.WriteLine(string.Format("{0} --- {1}-{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), t.Year, i));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("{0} --- {1}-{2} 当天没有数据", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), t.Year, i));
                    }

                }

            }
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //Console.WriteLine(stockContent);
            //for (int i = 0; i < 3; i++)
            //{
            //    System.Threading.Thread.Sleep(2000);
            //    player.Play();
            //}
            player.Play();
            Console.ReadKey();
            //WebRequest request =WebRequest.Create(stockAddress);

        }

        #region StockInfo
        class StockInfoOps
        {
            private static long reds = 0;
            internal void WebClientTestGetStockInfo(bool isLocal)
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.Load(@"D:\stockdata\StockCode.html", Encoding.UTF8);
                var list = htmlDocument.GetElementbyId("stockcode");
                var dataRows = list.Descendants("a").ToArray();
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach (var item in dataRows)
                {
                    var arr = item.InnerText.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                    var code = arr[1];
                    var name = arr[0];
                    if (!code.StartsWith("60") && !code.StartsWith("00") && !code.StartsWith("30")) continue;
                    var task = Task.Factory.StartNew(() => PrecessStockInfo(code, name, isLocal));
                    while (Interlocked.Read(ref reds) > 5)
                    {
                        Thread.Sleep(3000);
                    }
                    //PrecessStockInfo(code, name, isLocal);
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")},{(int)stopwatch.ElapsedMilliseconds},{item.Name},{item.InnerText},{Path.GetFileNameWithoutExtension(item.GetAttributeValue("href", null))}");
                    stopwatch.Restart();

                    if (isLocal) break;
                    //if (stopwatch.ElapsedMilliseconds > 1000 * 30) break;
                }
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }
            private void PrecessStockInfo(string code, string name, bool isLocal)
            {
                Interlocked.Increment(ref reds);
                string categoryContent = null;
                string companyInfoContent = null;
                HtmlNode categoryDiv = null;
                HtmlNode companyInfoTable = null;
                var categoryLink = "http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_CorpOtherInfo/stockid/{0}/menu_num/2.phtml";
                var companyInfoLink = "http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_CorpInfo/stockid/{0}.phtml";
                if (isLocal)
                {
                    categoryLink = "http://localhost:8090/601766_2.html";
                    companyInfoLink = "http://localhost:8090/601766.html";
                }

                StockEntities context = new StockEntities();
                WebClient client = new WebClient() { Encoding = Encoding.GetEncoding(936) };

                try
                {
                    client.Encoding = Encoding.GetEncoding(936);
                    categoryContent = client.DownloadString(string.Format(categoryLink, code));
                    companyInfoContent = client.DownloadString(string.Format(companyInfoLink, code));
                }
                catch (Exception)
                {
                    //throw;
                }

                string listingDateString = null;

                if (!string.IsNullOrEmpty(categoryContent))
                {
                    var categoryHemlDocument = new HtmlDocument() { OptionOutputAsXml = true };
                    categoryHemlDocument.LoadHtml(categoryContent);
                    categoryDiv = categoryHemlDocument.GetElementbyId("con02-0");
                }
                if (!string.IsNullOrEmpty(companyInfoContent))
                {
                    var companyInfoHtmlDocument = new HtmlDocument() { OptionOutputAsXml = true };
                    companyInfoHtmlDocument.LoadHtml(companyInfoContent);
                    companyInfoTable = companyInfoHtmlDocument.GetElementbyId("comInfo1");
                    listingDateString = companyInfoTable?.Descendants("a")?.First()?.InnerText;
                }
                DateTimeOffset listingDate;
                context.StockInfoes.Add(new StockInfo() { Code = code, Name = name, Category = categoryDiv?.OuterHtml, CompanyInfo = companyInfoTable?.OuterHtml, ListingDate = DateTimeOffset.TryParse(listingDateString, out listingDate) ? (DateTimeOffset?)listingDate : null });
                context.SaveChanges();
                Interlocked.Decrement(ref reds);
            }

            internal void WebClientTestGetStockCapitalStructure()
            {
                var link = "http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_StockStructure/stockid/{0}.phtml";

                var context = new StockEntities();
                var codeIdList = context.StockInfoes.Where(info => info.CapitalStructure == null).Select(info => new { info.Id, info.Code }).ToArray();
                context.Dispose();
                Parallel.ForEach(codeIdList, new ParallelOptions() { MaxDegreeOfParallelism = 50 }, item =>
                  {
                      var code = item.Code;
                      var client = new WebDownload(5 * 60 * 1000) { Encoding = Encoding.GetEncoding(936) };
                      var content = client.DownloadString(string.Format(link, code.Substring(2)));
                      if (!string.IsNullOrEmpty(content))
                      {
                          var htmlDocument = new HtmlDocument() { OptionOutputAsXml = true };
                          htmlDocument.LoadHtml(content);
                          var div = htmlDocument.GetElementbyId("con02-1");
                          using (var stock = new StockEntities())
                          {
                              var connection = stock.Database.Connection;
                              var cmd = connection.CreateCommand();
                              cmd.CommandType = CommandType.Text;
                              cmd.CommandText = $"update stockinfo set CapitalStructure=@content where Id='{item.Id}'";
                              cmd.Parameters.Add(new SqlParameter("@content", SqlDbType.Xml) { Value = div?.OuterHtml });
                              connection.Open();
                              cmd.ExecuteNonQuery();
                              connection.Close();
                          }
                          htmlDocument = null;
                          div = null;
                          Console.WriteLine($"{code} done");
                      }
                      else
                      {
                          Console.WriteLine($"{code} null");
                      }
                      content = null;
                      client.Dispose();

                  });
            }

            public class WebDownload : WebClient
            {
                /// <summary>
                /// Time in milliseconds
                /// </summary>
                public int Timeout { get; set; }

                public WebDownload() : this(60000) { }

                public WebDownload(int timeout)
                {
                    this.Timeout = timeout;
                }

                protected override WebRequest GetWebRequest(Uri address)
                {
                    var request = base.GetWebRequest(address);
                    if (request != null)
                    {
                        request.Timeout = this.Timeout;
                    }
                    return request;
                }
            }

        }
        #endregion StockInfo
        private static void ParseDetailDataTDB()
        {

            var files = Directory.GetFiles(@"C:\Users\admin\Desktop\Web\pages\stockData\detail", "*.xls", SearchOption.TopDirectoryOnly).OrderBy(f => f);
            if (!files.Any())
            {
                return;
            }
            var rows = 0;
            var consoleLines = 0;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
            cmd.CommandType = CommandType.Text;
            conn.Open();
            var tempDateTeim = DateTime.Now;
            var logBuilder = new StringBuilder();
            try
            {
                foreach (var file in files)
                {
                    logBuilder.Append($"{Path.GetFileNameWithoutExtension(file)},");
                    consoleLines++;
                    tempDateTeim = DateTime.Now;
                    var lines = File.ReadLines(file).ToArray();
                    logBuilder.Append($"Read:{(int)(DateTime.Now - tempDateTeim).TotalMilliseconds},");
                    tempDateTeim = DateTime.Now;
                    var fileArray = Path.GetFileNameWithoutExtension(file).Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    var code = fileArray[0];
                    var dealDate = fileArray[1];
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
                                //Console.WriteLine($"{value}");
                                rows++;
                            }
                            //logBuilder.Append($"Prepare:{(int)(DateTime.Now - tempDateTeim).TotalMilliseconds},");
                            //tempDateTeim = DateTime.Now;

                            cmd.CommandText = $"{sqlHeader.ToString()}{sqlValues.ToString()}".TrimEnd(',');   //sql语句
                            cmd.ExecuteNonQuery();
                        }
                        logBuilder.Append($"Excute:{(int)(DateTime.Now - tempDateTeim).TotalMilliseconds},");
                        tempDateTeim = DateTime.Now;
                    }
                    //if (consoleLines % 20 == 0) Console.Clear();
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} --  {Path.GetFileNameWithoutExtension(file)}, rows:{rows}");
                    logBuilder.Append($"Conlose:{(int)(DateTime.Now - tempDateTeim).TotalMilliseconds},");
                    tempDateTeim = DateTime.Now;
                    File.Move(file, file.Replace("detail\\", "detail\\Backup\\"));
                    logBuilder.Append($"Move:{(int)(DateTime.Now - tempDateTeim).TotalMilliseconds},lines:{lines.Length}");
                    //File.AppendAllLines(@"C:\Users\admin\Desktop\Web\pages\stockData\log.txt", new[] { logBuilder.ToString() });
                    logBuilder.Clear();
                    /*
                    for (int i = 1; i < lines.Length; i++)
                    {
                        SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
                        cmd.CommandType = CommandType.Text;

                        var fields = lines[i].Split(new[] { '	' }, StringSplitOptions.RemoveEmptyEntries);
                        cmd.CommandText = @"
                       INSERT INTO [Detail]
                                ([DateTime]
                               ,[Code]
                               ,[Price]
                               ,[PriceOffset]
                               ,[Volume]
                               ,[Amount]
                               ,[Direction])
                         VALUES
                               (@DateTime
                               ,@Code
                               ,@Price
                               ,@PriceOffset
                               ,@Volume
                               ,@Amount
                               ,@Direction)";   //sql语句
                        cmd.Parameters.Add(new SqlParameter("@DateTime", SqlDbType.DateTimeOffset) { Value = DateTimeOffset.Parse($"{dealDate} {fields[0]}") });  //给参数sql语句的参数赋值
                        cmd.Parameters.Add(new SqlParameter("@Code", SqlDbType.NVarChar) { Value = code });
                        cmd.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal) { Value = fields[1] });
                        cmd.Parameters.Add(new SqlParameter("@PriceOffset", SqlDbType.Decimal) { Value = fields[2].Equals("--") ? 0 : decimal.Parse(fields[2]) });
                        cmd.Parameters.Add(new SqlParameter("@Volume", SqlDbType.Int) { Value = int.Parse(fields[3]) * 100 });
                        cmd.Parameters.Add(new SqlParameter("@Amount", SqlDbType.Decimal) { Value = decimal.Parse(fields[4]) });
                        cmd.Parameters.Add(new SqlParameter("@Direction", SqlDbType.NVarChar) { Value = fields[5] });
                        cmd.ExecuteNonQuery();
                        rows += lines.Length - 1;
                        //Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} --  Lines:{i}/{lines.Length - 1}, rows:{rows}");
                    }
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} --  {Path.GetFileNameWithoutExtension(file)}, rows:{rows}");
                    */
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"error:{e.Message}");
            }
            finally
            {
                conn.Close();
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} --  End");
            }

        }
        private static void ParseSummaryDataTDB()
        {
            var files = Directory.GetFiles(@"C:\Users\admin\Desktop\Web\pages\stockData\summary", "*.html", SearchOption.TopDirectoryOnly).OrderBy(f => f);
            if (!files.Any())
            {
                return;
            }
            var rows = 0;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            conn.Open();

            try
            {
                foreach (var file in files)
                {
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.Load(file);
                    htmlDocument.GetElementbyId("FundHoldSharesTable");
                    var table = htmlDocument.GetElementbyId("FundHoldSharesTable");
                    var dataRows = table.Elements("tr").ToArray();

                    var fileArray = Path.GetFileNameWithoutExtension(file).Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    var code = fileArray[0];
                    var sql = new StringBuilder(@"
                        INSERT INTO [Summary]
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
                        //Console.WriteLine($"{value}");
                        rows++;
                    }

                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} --  {Path.GetFileNameWithoutExtension(file)}, rows:{rows}");
                    sql.Remove(sql.Length - 1, 1);
                    SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql.ToString();   //sql语句
                    cmd.ExecuteNonQuery();
                    File.Move(file, file.Replace("summary\\", "summary\\Backup\\"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"error:{e.Message}");

            }
            finally
            {
                conn.Close();
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} --  End");
            }

        }
        private static void WebClientTestGetMiaoJiangGuShi2()
        {
            var lastChapter = 0;
            var files = Directory.GetFiles(@"D:\LenovoDrivers\html", "*.html", SearchOption.TopDirectoryOnly).OrderBy(f => f);
            if (files.Any())
            {
                int pageId = 0;
                lastChapter = (from file in files where int.TryParse(Path.GetFileNameWithoutExtension(file).Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)?[0], out pageId) select pageId).Max();
            }
            WebClient client = new WebClient() { Encoding = Encoding.UTF8 };
            var mainPageContent = client.DownloadString("http://www.miaojianggushi2.com/");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(mainPageContent);
            //htmlDocument.Load(files.First());
            var div = htmlDocument.GetElementbyId("wrap");
            var links = div.Descendants("a").ToArray();
            foreach (var link in links)
            {

                if (link.Attributes == null || !link.Attributes.Any() || !link.OuterHtml.Contains("/book/")) continue;
                var bookLink = link.Attributes.First().Value.Replace(Environment.NewLine, "");
                var title = link.InnerText.Replace(Environment.NewLine, "");
                var field = bookLink.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
                var linkNumber = field.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).First();
                //if (string.CompareOrdinal(linkNumber, "1113") <= 0) continue;
                if (Convert.ToInt32(linkNumber) <= lastChapter) continue;
                var content = client.DownloadString(bookLink);
                var fileName = string.Format(@"D:\LenovoDrivers\html\{0}_{1}.html", linkNumber, title);
                File.WriteAllText(fileName, content);
                Console.WriteLine(string.Format("{0} --- {1}-{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), title, field));
                Thread.Sleep(500);
            }
        }

        private static void TestTread()
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["StockConnectionString"].ToString());
            SqlCommand cmd = conn.CreateCommand();              //创建SqlCommand对象
            cmd.CommandType = CommandType.Text;
            conn.Open();
            cmd.CommandText = @"
                  SELECT  CONVERT(VARCHAR, [DateTime],112) as [date]
                  FROM [Detail] 
                  WHERE CONVERT(VARCHAR, [DateTime],112)>'20160101'
                  GROUP BY CONVERT(VARCHAR, [DateTime],112)
                  ORDER BY [date]";
            var dateListReader = cmd.ExecuteReader();
            var dateList = new List<string>();
            while (dateListReader.Read())
            {
                dateList.Add(dateListReader.GetString(0));
            }
            dateListReader.Close();
            foreach (var date in dateList)
            {
                cmd.CommandText = $@"
                      SELECT [DateTime],[Price]
                      FROM [Stock].[Detail]
                      WHERE [DateTime] > '{date}' 
                      ORDER BY [DateTime]";
                var perPrice = default(decimal);
                try
                {

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var dt = reader.GetDateTimeOffset(0);
                        var price = reader.GetDecimal(1);
                        if (perPrice <= default(decimal))
                        {
                            perPrice = price;
                            continue;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
                finally
                {
                    conn.Close();
                }

            }
        }
        private static void TestShareMemory()
        {
            long capacity = 1 << 6;//64
            var memoryMappedFileSecurity = new MemoryMappedFileSecurity();
            using (var mmf = MemoryMappedFile.CreateOrOpen("testMmf", capacity, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.DelayAllocatePages, memoryMappedFileSecurity, HandleInheritability.None))
            {
                //通过MemoryMappedFile的CreateViewAccssor方法获得共享内存的访问器
                var viewAccessor = mmf.CreateViewAccessor(0, capacity);
                string input = "197.390";
                //向共享内存开始位置写入字符串的长度
                viewAccessor.Write(0, input.Length);
                //向共享内存4位置写入字符
                viewAccessor.WriteArray<char>(4, input.ToArray(), 0, input.Length);
                Thread.Sleep(30000);
            }
            //Console.ReadKey();

        }
        private static void TestMethodClay()
        {
            //dynamic New = new ClayFactory();
            //var person = New.Person();
            //person.FirstName = "Louis";
            //person.LastName = "Dejardin";
        }
        private static void TestMethodIronPython()
        {

            Console.WriteLine("Loading helloworld.py...");
            ScriptRuntime py = Python.CreateRuntime();
            dynamic helloworld = py.UseFile("helloworld.py");
            Console.WriteLine("helloworld.py loaded!");
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(helloworld.welcome("Employee #{0}"), i);
            }
            Console.ReadLine();
        }

        #region PayrollTest

        static void FsetRegExp()
        {
            var excutePath = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}FsetRegExp\";
            var xDoc = XElement.Load($"{excutePath}eFiles.xml");
            var eFileItems = from eFile in xDoc.Elements() select eFile;
            var resultBuilder = new StringBuilder(0);
            resultBuilder.AppendLine("State,Form,Field,RegExp");

            foreach (var eFileItem in eFileItems)
            {
                var state = eFileItem.Element("State")?.Value;
                var efileForm = eFileItem.Element("Form")?.Value;
                var fields = from field in eFileItem.Descendants("Field") where field.Element("RegExp") != null select field;
                foreach (var field in fields)
                {
                    var name = GetFullName(field); //field.Element("Name")?.Value;
                    var regExp = field.Element("RegExp")?.Value;
                    resultBuilder.AppendLine($"\"{state}\",\"{efileForm}\",\"{name}\",\"{regExp}\"");
                }
                File.WriteAllText($@"{excutePath}\result.txt", resultBuilder.ToString());
            }

        }
        static string GetFullName(XElement field)
        {
            var fullName = string.Empty;
            while (!field.Name.LocalName.Equals("Fields"))
            {
                if (field.Name.LocalName.Equals("CollectionItem"))
                {
                    field = field.Parent;
                    continue;
                }
                fullName = field.Element("Name")?.Value + "/" + fullName;
                field = field.Parent;
            }
            return fullName.TrimEnd('/');
        }

        static void PayrollTest()
        {
            var excutePath = $@"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}PayrollTest\";
            var testFilePath = @"D:\GitSource\Payroll\project\Paycycle.EForms.StateFilingGenerations.Test\FilingGenerationsTest";
            var xDoc = XElement.Load($"{excutePath}PayrollTest.xml");
            var eFileItems = from eFile in xDoc.Elements() where eFile.Element("Finished").Value == "No" select eFile;
            foreach (var eFileItem in eFileItems)
            {
                var state = eFileItem.Element("State");
                var statePath = $@"{testFilePath}\{state.Value}";
                if (!Directory.Exists(statePath))
                {
                    Directory.CreateDirectory(statePath);
                }
                var forms = eFileItem.Elements("Form");
                var isFirstForm = true;
                var inputJson = File.ReadAllText($"{excutePath}$FormCode$.input.json");
                var testSetup = File.ReadAllText($"{excutePath}TestSetup.cs.txt");
                var testCaseTemplate = File.ReadAllText($"{excutePath}FormTestCaseTemplate.txt");
                var testCase = string.Empty;
                foreach (var form in forms)
                {
                    var formCode = form.Element("FormCode");
                    var depositGroupCode = form.Element("DepositGroupCode");
                    var taxObligationType = form.Element("TaxObligationType");
                    var taxDepositFrequency = form.Element("TaxDepositFrequency");
                    if (isFirstForm)
                    {
                        inputJson = inputJson
                            .Replace("$State$", state.Value)
                            .Replace("$FormCode$", formCode?.Value)
                            .Replace("$DepositGroupCode$", depositGroupCode?.Value)
                            .Replace("$TaxObligationType$", taxObligationType.Value.Equals("Filing") ? "2" : "1");
                        var inputJsonPath = "$FormCode$.input.json".Replace("$FormCode$", formCode?.Value);
                        File.WriteAllText($@"{statePath}\{inputJsonPath}", inputJson);
                        testSetup = testSetup
                                .Replace("$State$", state.Value)
                                .Replace("$FormCode$", formCode?.Value);
                        isFirstForm = false;
                    }
                    var outputJsonPath = "$FormCode$.output.json".Replace("$FormCode$", formCode?.Value);
                    File.Copy($"{excutePath}$FormCode$.output.json", $@"{statePath}\{outputJsonPath}", true);
                    testCase += testCaseTemplate
                           .Replace("$State$", state.Value)
                           .Replace("$TaxDepositFrequency$", taxDepositFrequency.Value)
                           .Replace("$FormCode$", formCode?.Value);
                }
                testSetup = testSetup.Replace("$TestCaseTemplate$", testCase);
                File.WriteAllText($@"{statePath}\TestSetup.cs", testSetup);
            }

        }

        static void ParseContent()
        {
            var excutePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            var xDoc = XElement.Load(string.Format("{0}Template.xml", excutePath));
            var content = File.ReadAllLines(string.Format("{0}Content.txt", excutePath));
            var xResult = new XElement("Result");
            for (int i = 0; i < content.Length; i += 2)
            {
                var rw = content[i];
                var rs = content[i + 1];
                var template = XElement.Parse(xDoc.ToString());
                var values = from node in template.Descendants() where node.Name == "Value" select node;
                foreach (var item in values)
                {
                    var fields = item.Value.Split(',');
                    var startIndex = int.Parse(fields[1]) - 1;
                    var length = int.Parse(fields[2]);
                    var line = fields[0].Equals("RW") ? rw : rs;
                    var value = line.Substring(startIndex, length).Trim();
                    if (fields.Length == 4 && fields[3].Equals("Date") && !string.IsNullOrEmpty(value))
                    {
                        value = string.Format("{0}-{1}-{2}", value.Substring(4, 4), value.Substring(0, 2), value.Substring(2, 2));
                    }
                    else if (fields.Length == 4 && fields[3].Equals("Number"))
                    {
                        value = value.TrimStart('0');
                        if (string.IsNullOrEmpty(value)) value = "000";
                        value = value.Insert(value.Length - 2, ".");
                    }
                    item.Value = value;
                }
                xResult.Add(template);
            }
            xResult.Save(string.Format("{0}result.xml", excutePath));
        }
        #endregion PayrollTest
    }
}
