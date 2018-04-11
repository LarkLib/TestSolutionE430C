using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace TesWebPageDownloadtConsoleApplication
{
    class WebPageOperation
    {
        internal void WebClientTestGetWebPages()
        {
            var stopwatch = new Stopwatch();
            var clientIndex = new WebDownload();
            var indexPage = "http://www.zhuaji.org/read/2471/";
            var indexContent = clientIndex.DownloadString(indexPage);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(indexContent);
            var dl = htmlDocument.DocumentNode.SelectNodes("//dl");
            var ddElements = dl.Nodes().Where((node) => node.Name == "dd").ToArray();
            var lastChapter = 214;
            foreach (var node in ddElements)
            {
                var a = node.Element("a");
                if (a == null || !a.InnerText.StartsWith("第")) continue;
                var chapter = int.Parse(a.InnerText.Substring(1, a.InnerText.IndexOf("章") - 1));
                if (chapter <= lastChapter) continue;
                File.AppendAllText("d:\\麻衣神算子.txt", $"{a.InnerText}{Environment.NewLine}");
                Console.WriteLine(a.InnerText);
                var clientContent = new WebDownload();
                var content = clientContent.DownloadString($"http://www.zhuaji.org/read/2471/{a.Attributes["href"].Value}");
                var contentDocument = new HtmlDocument();
                contentDocument.LoadHtml(content);
                var divContent = contentDocument.GetElementbyId("content");
                var divA = divContent.SelectNodes("a")?.ToArray();
                if (divA != null && divA.Any()) foreach (var aItem in divA) aItem.Remove();
                var temContent = divContent.InnerHtml.Replace("&nbsp;", " ").Replace("<br><br>", "<br>").Replace("<br>", Environment.NewLine);
                var textContent = Regex.Replace(temContent, "&#\\w+;", "");
                File.AppendAllText("d:\\麻衣神算子.txt", $"{textContent}{Environment.NewLine}");
            }
            return;
            //Parallel.ForEach(dbList, new ParallelOptions() { MaxDegreeOfParallelism = MaxDbThreadCount }, db =>
            //{
            //    var codeList = GetCodeListByDatabase(db);
            //    Parallel.ForEach(codeList, new ParallelOptions() { MaxDegreeOfParallelism = MaxCodeThreadCount }, code =>
            //    {
            //        {
            //            var stopwatch = new Stopwatch();
            //            stopwatch.Start();
            //            string stockContent = null;
            //            for (int i = 0; i < 5; i++)
            //            {
            //                try
            //                {
            //                    var client = new WebDownload(5 * 60 * 1000) { Encoding = Encoding.GetEncoding(936) };
            //                    var stockAddress = string.Format("http://quotes.money.163.com/service/chddata.html?code={0}{1}&start=19021231&end=20180526&fields=TCLOSE;HIGH;LOW;TOPEN;LCLOSE;CHG;PCHG;TURNOVER;VOTURNOVER;VATURNOVER;TCAP;MCAP", code.StartsWith("sh") ? 0 : 1, code.Substring(2, 6));
            //                    stockContent = client.DownloadString(stockAddress);
            //                    break;
            //                }
            //                catch (WebException)
            //                {
            //                    Thread.Sleep(15000);
            //                }
            //            }
            //            var rows = 0;
            //            if (!string.IsNullOrEmpty(stockContent))
            //            {
            //                rows = SaveStockDaily163ContentToDB(db, code, stockContent);
            //            }
            //            Console.WriteLine($"{db},GetDaily163,{code},{rows},{(int)stopwatch.ElapsedMilliseconds}ms");
            //        }
            //    }
            //    );
            //}
            //);
        }

    }
    #region WebDownload
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
    #endregion
}
