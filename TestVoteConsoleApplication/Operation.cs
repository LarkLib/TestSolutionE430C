using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestVoteConsoleApplication
{
    class Operation
    {
        private static readonly string CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string LogFile = $@"{AppDomain.CurrentDomain.BaseDirectory}\Log.txt";
        private static readonly string LastRowNumberFileName = Path.Combine(CurrentPath, $"LastRowNumber_{DateTime.Now.ToString("yyyyMMdd")}.txt");
        private static string[] agentList = null;
        private static string[] AgentList
        {
            get
            {
                if (agentList == null)
                {
                    agentList = new[]
                    {
                     //"Mozilla/5.0 (Linux; Android 4.1.1; Nexus 7 Build/JRO03D) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166  Safari/535.19",
                     //"Mozilla/5.0 (Linux; U; Android 4.0.4; en-gb; GT-I9300 Build/IMM76D) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30",
                     //"Mozilla/5.0 (Linux; U; Android 2.2; en-gb; GT-P1000 Build/FROYO) AppleWebKit/533.1 (KHTML, like Gecko) Version/4.0 Mobile Safari/533.1",
                     //"Mozilla/5.0 (Android; Mobile; rv:14.0) Gecko/14.0 Firefox/14.0",
                     //"Mozilla/5.0 (Android; Tablet; rv:14.0) Gecko/14.0 Firefox/14.0",
                     //"Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:21.0) Gecko/20130331 Firefox/21.0",
                     //"Mozilla/5.0 (Windows NT 6.2; WOW64; rv:21.0) Gecko/20100101 Firefox/21.0",
                     //"Mozilla/5.0 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19",
                     //"Mozilla/5.0 (Linux; Android 4.1.2; Nexus 7 Build/JZ054K) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Safari/535.19",
                     //"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.93 Safari/537.36",
                     //"Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/535.11 (KHTML, like Gecko) Ubuntu/11.10 Chromium/27.0.1453.93 Chrome/27.0.1453.93 Safari/537.36",
                     //"Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.94 Safari/537.36",
                     "Mozilla/5.0 (iPhone; CPU iPhone OS 6_1_4 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) CriOS/27.0.1453.10.{0} Mobile/10B350 Safari/8536.25",
                     //"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0)",
                     //"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",
                     //"Mozilla/5.0 (compatible; WOW64; MSIE 10.0; Windows NT 6.2)",
                     "mozilla/5.0 (linux; u; android 4.1.2; zh-cn; mi-one plus build/jzo54k) applewebkit/534.30 (khtml, like gecko) version/4.0 mobile safari/534.30.{0} micromessenger/5.0.1.352",
                     "mozilla/5.0 (iphone; cpu iphone os 5_1_1 like mac os x) applewebkit/534.46.{0} (khtml, like gecko) mobile/9b206 micromessenger/5.0",
                     "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63.{0} Safari/537.36 360Browser",
                     "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63.{0} Safari/537.36 360Browser",
                     "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Orbitum/56.0.2924.92.{0} Chrome/27.0.1453.94 360Browser",
                     "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; AS; rv:11.0.{0}) like Gecko",
                     "Mozilla/5.0 (compatible, MSIE 11, Windows NT 6.3; Trident/7.0; rv:11.0.{0}) like Gecko",
                     "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.110.{0} Safari/537.36",
                     "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML like Gecko) Chrome/44.0.2403.155.{0} Safari/537.36"
                    };
                }
                return agentList;
            }
        }
        private static CookieContainer cookies = null;


        internal void Execute()
        {
            //CheckProxy();
            Votes();
            //TestGetNextTime();
        }

        private void TestGetNextTime()
        {
            var time = DateTime.Now;
            var c = GetNextTime(time);
            Console.WriteLine($"{time.ToString()}, {(c / 1000 / 60).ToString()}");
            time = DateTime.Now.Date.AddHours(6);
            c = GetNextTime(time);
            Console.WriteLine($"{time.ToString()}, {(c / 1000 / 60).ToString()}");
            time = DateTime.Now.Date.AddHours(22);
            c = GetNextTime(time);
            Console.WriteLine($"{time.ToString()}, {(c / 1000 / 60).ToString()}");
        }

        private void Votes()
        {
            int i = 0;
            while (true)
            {
                var spenTime = GetNextTime(DateTime.Now);
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},Sleep:{spenTime / 1000 } s,Vote Time:{DateTime.Now.AddMilliseconds(spenTime)}");
                Thread.Sleep(spenTime);
                //var uniqueMark = Convert.ToString((int)(DateTime.Now - DateTime.Now.Date.AddHours(7)).TotalSeconds, 16);
                var uniqueMark = (int)(DateTime.Now - DateTime.Now.Date.AddHours(7)).TotalSeconds;
                var uniqueUserAgent = string.Format(GetUserAgent(), uniqueMark);
                Vote(null, uniqueUserAgent);
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},Vote Count:{++i}");
            }
            //var voteList = GetVoteList();
            //var lastRowNumber = int.Parse(File.ReadAllText(LastRowNumberFileName));
            //foreach (var vote in voteList)
            //{
            //    var voteFields = vote.Split(new[] { '#' });
            //    var rowNumber = int.Parse(voteFields[0]);
            //    if (rowNumber <= lastRowNumber) continue;
            //    var ip = voteFields[1];
            //    var port = int.Parse(voteFields[2]);
            //    var userAgent = voteFields[3];
            //    var webProxy = new WebProxy(ip, port);
            //    if (CheckProxy(webProxy))
            //    {
            //        LogInfo($"vote:{vote}");
            //        Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},vote:{vote}");
            //        Vote(webProxy, userAgent);
            //    }
            //    File.WriteAllText(LastRowNumberFileName, rowNumber.ToString());
            //}
        }

        private void Vote(WebProxy webProxy, string userAgent)
        {
            var userId = 1016; //10
            var questionResult = new Random().Next(0, 100);
            var url = "http://www.cydxnb.com/eduvote/search?search_key=1016";
            var contenUrl = $"http://www.cydxnb.com/eduvote/search?search_key={userId}";
            url = " http://www.cydxnb.com/EduVote/VoteAutoMac";
            var result = GetVoteContent(contenUrl, userAgent, webProxy);
            var sessionId = (string)result[0];
            var keymc = (string)result[1];
            var sssvalue = (string)result[2];


            string postDataString = $@"id={userId}&h{userId}={questionResult}&keymc={keymc}&sssvalue={sssvalue}";
            byte[] postData = Encoding.UTF8.GetBytes(postDataString);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = cookies;
            request.Proxy = webProxy;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = userAgent;
            request.Referer = string.Empty;
            request.ContentLength = postData.Length;
            request.KeepAlive = true;
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Cookie", $"ASP.NET_SessionId={sessionId}");
            request.ServerCertificateValidationCallback +=
                    (sender, cert, chain, error) =>
                    {
                        return true;
                    };

            string responseContent = null;
            using (var writer = request.GetRequestStream())
            {
                writer.Write(postData, 0, postData.Length);
                writer.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    responseContent = responseContent = reader.ReadToEnd();
                    var info = $"Post:keymc={keymc},sssvalue={sssvalue},questionReqult={questionResult},sessionId={sessionId}";
                    LogInfo(info);
                    //Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},{info}");
                }
            }
            request = null;
            response.Close();
        }
        private IList<string> GetVoteContent(string url, string userAgent, WebProxy webProxy)
        {
            var resultList = new List<string>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            cookies = new CookieContainer();
            request.CookieContainer = cookies;
            request.AllowAutoRedirect = false;
            request.Proxy = webProxy;
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = userAgent;
            request.KeepAlive = true;
            request.ServerCertificateValidationCallback +=
                    (sender, cert, chain, error) =>
                    {
                        return true;
                    };

            string responseContent = null;
            var response = (HttpWebResponse)request.GetResponse();
            var sessionId = response.Headers["Set-Cookie"].Split(new[] { ';' })[0]; // "ASP.NET_SessionId=c1cisgywavifbucqlhbwhvqo; path=/; HttpOnly"  string
            string keymc = null;
            string sssvalue = null;
            using (Stream responseStream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    responseContent = reader.ReadToEnd();
                    var responseHtmlDocumet = new HtmlDocument();
                    responseHtmlDocumet.LoadHtml(responseContent);
                    var inputs = responseHtmlDocumet.DocumentNode.Descendants("input");
                    foreach (var input in inputs)
                    {
                        if (input.GetAttributeValue("id", string.Empty).Equals("keymc"))
                        {
                            keymc = input.GetAttributeValue("value", string.Empty);
                        }
                        else if (input.GetAttributeValue("id", string.Empty).Equals("sssvalue"))
                        {
                            sssvalue = input.GetAttributeValue("value", string.Empty);
                        }
                    }
                    var info = $"Get:keymc={keymc},sssvalue={sssvalue},sessionId={sessionId}";
                    LogInfo(info);
                    //Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},{info}");
                }
            }
            request = null;
            response.Close();
            resultList.Add(sessionId);
            resultList.Add(keymc);
            resultList.Add(sssvalue);
            return resultList;
        }
        private IList<string> GetVoteList()
        {
            var voteListFileName = Path.Combine(CurrentPath, $"VoteList_{DateTime.Now.ToString("yyyyMMdd")}.txt");
            CreateVoteList(voteListFileName);
            var voteList = File.ReadAllLines(voteListFileName);
            return voteList;
        }

        private void CreateVoteList(string voteListFileName)
        {
            if (File.Exists(voteListFileName)) return;
            File.WriteAllText(LastRowNumberFileName, "-1");
            var proxyList = GetProxyList();
            var voteList = new List<string>();
            int rowNumber = 0;

            for (int i = 0; i < 3; i++)
            {

                foreach (var proxy in proxyList)
                {
                    var vote = $"{rowNumber++}#{proxy.Key}#{proxy.Value}#{GetUserAgent()}";
                    if (!voteList.Contains(vote))
                    {
                        voteList.Add(vote);
                    }
                }
            }
            File.WriteAllLines(voteListFileName, voteList);
        }
        private void CheckProxy()
        {
            //https://www.xicidaili.com/
            var url = "http://myip.kkcha.com/";
            url = "https://ip.cn/";
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(Path.Combine(CurrentPath, "IpList.txt"));
            var proxyList = htmlDocument.GetElementbyId("ip_list").Elements("tr").ToArray();

            string postDataString = $@"id=9";
            byte[] postData = Encoding.UTF8.GetBytes(postDataString);
            var continueFlag = false;
            foreach (var proxy in proxyList)
            {
                var lastIp = "60.169.192.99";
                var ip = proxy.Elements("td").ToArray()[1].InnerText;
                continueFlag = continueFlag != true ? ip.Equals(lastIp) : true;
                if (!continueFlag || ip.Equals(lastIp))
                {
                    continue;
                }
                var port = proxy.Elements("td").ToArray()[2].InnerText;
                var webProxy = new WebProxy(ip, int.Parse(port));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = webProxy;
                request.Method = "GET";
                //request.ContentType = "application/json";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
                //request.Host = "vss.baobaoaichi.cn";
                //request.ContentLength = postData.Length;
                //request.Connection = "Keep-Alive";
                request.KeepAlive = true;
                //request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Add("Accept-Encoding", "gzip, deflate");
                //request.Headers.Add("Cookie", $"BSID={bsid}; msid=shqnymy");
                //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.ServerCertificateValidationCallback +=
                        (sender, cert, chain, error) =>
                        {
                            return true;
                        };

                string responseContent = null;
                //try
                //{
                //    using (var writer = request.GetRequestStream())
                //    {
                //        writer.Write(postData, 0, postData.Length);
                //        writer.Close();
                //    }
                //}
                //catch(WebException we)
                //{

                //}
                //catch (Exception e)
                //{

                //    LogInfo($"Error:{e.Message}");
                //}
                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress))
                        {

                            using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
                            {
                                responseContent = responseContent = reader.ReadToEnd();
                                var responseHtmlDocumet = new HtmlDocument();
                                responseHtmlDocumet.LoadHtml(responseContent);
                                var currentIp = responseHtmlDocumet.GetElementbyId("result").Element("div").Element("p").Element("code").InnerText;
                                LogInfo($"{ip},{port},{currentIp}");
                                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},{ip},{port},{currentIp}");
                            }
                        }
                    }

                }
                catch (WebException)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},Error IP:{ip},{port}");

                }
                catch (Exception e)
                {

                    LogInfo($"Error:{e.Message}");
                }
            }
        }
        private bool CheckProxy(WebProxy webProxy)
        {
            //https://www.xicidaili.com/
            var url = "http://myip.kkcha.com/";
            url = "https://ip.cn/";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = webProxy;
            request.Method = "GET";
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.KeepAlive = true;
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.ServerCertificateValidationCallback +=
                    (sender, cert, chain, error) =>
                    {
                        return true;
                    };

            string responseContent = null;
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress))
                    {

                        using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
                        {
                            responseContent = responseContent = reader.ReadToEnd();
                            var responseHtmlDocumet = new HtmlDocument();
                            responseHtmlDocumet.LoadHtml(responseContent);
                            var currentIp = responseHtmlDocumet.GetElementbyId("result").Element("div").Element("p").Element("code").InnerText;
                            return true;
                        }
                    }
                }
            }
            catch (WebException)
            {
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private string GetUserAgent()
        {
            //http://useragentstring.com/
            Random random = new Random();
            var i = random.Next(0, AgentList.Length - 1);
            return AgentList[i];

        }
        private IDictionary<string, string> GetProxyList()
        {
            var proxyList = new Dictionary<string, string>();
            var proxyRows = File.ReadAllLines(Path.Combine(CurrentPath, "ProxyList.txt"));
            foreach (var row in proxyRows)
            {
                var fields = row.Split(new[] { ',' });
                var ip = fields[1];
                var port = fields[2];
                if (!proxyList.Keys.Contains(ip))
                    proxyList.Add(ip, port);
            }
            proxyRows = null;
            //proxyList.OrderBy(p => p.Key);
            return proxyList;
        }
        private int GetNextTime(DateTime dateTime)
        {
            var currenTimeSpan = dateTime - dateTime.Date;
            var currentHours = currenTimeSpan.Hours;
            var random = new Random();
            var i = 0;
            var sleepTime = 0;
            if (currentHours < 7) sleepTime = (int)(dateTime.Date.AddHours(7) - dateTime).TotalMilliseconds + random.Next(5, 10) * 60 * 1000;
            //else if (currentHours >= 7 && currentHours < 8)   sleepTime = random.Next(6 * 60 * 1000, 15 * 60 * 1000);
            //else if (currentHours >= 8 && currentHours < 9)   sleepTime = random.Next(6 * 60 * 1000, 11 * 60 * 1000);
            //else if (currentHours >= 9 && currentHours < 11)  sleepTime = random.Next(4 * 60 * 1000, 9 * 60 * 1000);
            //else if (currentHours >= 11 && currentHours < 12) sleepTime = random.Next(6 * 60 * 1000, 11 * 60 * 1000);
            //else if (currentHours >= 12 && currentHours < 14) sleepTime = random.Next(6 * 60 * 1000, 15 * 60 * 1000);
            //else if (currentHours >= 14 && currentHours < 18) sleepTime = random.Next(6 * 60 * 1000, 11 * 60 * 1000);
            //else if (currentHours >= 18 && currentHours < 21) sleepTime = random.Next(6 * 60 * 1000, 15 * 60 * 1000);
            else if (currentHours >= 7 && currentHours <= 21)
            {
                //sleepTime = random.Next(1 * 60 * 1000, 25 * 60 * 1000);
                //sleepTime = sleepTime > 1000 * 60 * 5 && new Random().Next(1, 100) % 5 != 0 ? (int)(sleepTime * 0.5) : sleepTime;
                //sleepTime = sleepTime > 1000 * 60 * 5 && new Random().Next(1, 100) % 3 != 0 ? (int)(sleepTime * 0.5) : sleepTime;
                //sleepTime = sleepTime > 1000 * 60 * 3 && new Random().Next(1, 100) % 3 != 0 ? (int)(sleepTime * 0.5) : sleepTime;
                //sleepTime = sleepTime > 1000 * 60 * 3 && new Random().Next(1, 100) % 3 != 0 ? random.Next(1 * 60 * 1000, 3 * 60 * 1000) : sleepTime;
                sleepTime = new Random().Next(1, 1000) % 10 != 0 ? random.Next(30 * 1000, 2 * 60 * 1000) : random.Next(1 * 60 * 1000, 18 * 60 * 1000);
                sleepTime = sleepTime > 1000 * 60 * 3 && new Random().Next(1, 100) % 5 != 0 ? random.Next(30 * 1000, 2 * 60 * 1000) : sleepTime;

            }
            else sleepTime = (int)(dateTime.Date.AddDays(1).AddHours(7) - dateTime).TotalMilliseconds + random.Next(0 * 60 * 1000, 5 * 60 * 1000);
            return sleepTime;
        }
        internal static void LogInfo(string content)
        {
            File.AppendAllText(LogFile, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},{content}{Environment.NewLine}");
        }

    }
}
