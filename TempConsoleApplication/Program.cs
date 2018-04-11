using HtmlAgilityPack;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TempConsoleApplication
{
    class Program
    {
        private static long count = 0;
        private static SemaphoreSlim semaphore;
        static void Main(string[] args)
        {
            //var stockInfo = new StockInfoOps();
            //stockInfo.WebClientTestGetStockInfo(false);
            //TestThread();
            //TestThreadParallel();
            //WinApi.TestWinApi();
            //TestMehtodEPPlus();
            Console.ReadKey();
        }
        private static void TestMehtodEPPlus()
        {
            var outputDir = AppDomain.CurrentDomain.BaseDirectory;
            FileInfo newFile = new FileInfo(Path.Combine(outputDir + @"sample6.xlsx"));

            ExcelPackage pck = new ExcelPackage(newFile);
            //Add the Content sheet
            var ws = pck.Workbook.Worksheets.Add("Content");
            ws.View.ShowGridLines = false;

            //Then create an outline for column 4 and 5 and hide them.
            ws.Column(4).OutlineLevel = 1;
            ws.Column(4).Collapsed = true;
            ws.Column(5).OutlineLevel = 1;
            ws.Column(5).Collapsed = true;
            ws.OutLineSummaryRight = true;

            //Set the header values and set font style to bold.
            //Headers
            ws.Cells["B1"].Value = "Name";
            ws.Cells["C1"].Value = "Size";
            ws.Cells["D1"].Value = "Created";
            ws.Cells["E1"].Value = "Last modified";
            ws.Cells["B1:E1"].Style.Font.Bold = true;

            //Add the subtotal formula...
            //ws.Cells[prevRow, 3].Formula = string.Format("SUBTOTAL(9, {0})", ExcelCellBase.GetAddress(prevRow + 1, 3, row - 1, 3));

            //Now we add the shape containing the text and sets a bunch of properties.
            //Add the textbox
            var shape = ws.Drawings.AddShape("txtDesc", eShapeStyle.Rect);
            shape.SetPosition(0, 5, 5, 5);
            shape.SetSize(400, 200);

            shape.Text = "This example demonstrates how to create various drawing objects like Pictures, Shapes and charts.\n\r\n\rThe first sheet...";
            shape.Fill.Style = eFillStyle.SolidFill;
            shape.Fill.Color = Color.DarkSlateGray;
            shape.Fill.Transparancy = 20;
            shape.Border.Fill.Style = eFillStyle.SolidFill;
            shape.Border.LineStyle = eLineStyle.LongDash;
            shape.Border.Width = 1;
            shape.Border.Fill.Color = Color.Black;
            shape.Border.LineCap = eLineCap.Round;
            shape.TextAnchoring = eTextAnchoringType.Top;
            shape.TextVertical = eTextVerticalType.Horizontal;
            shape.TextAnchoringControl = false;

            //Add a HyperLink to the statistics sheet. 
            var namedStyle = pck.Workbook.Styles.CreateNamedStyle("HyperLink");   //This one is language dependent
            namedStyle.Style.Font.UnderLine = true;
            namedStyle.Style.Font.Color.SetColor(Color.Blue);
            ws.Cells["K12"].Hyperlink = new ExcelHyperLink("Statistics!A1", "Statistics");
            ws.Cells["K12"].StyleName = "HyperLink";
            pck.Save();
        }
        private static void TestThreadParallel()
        {
            string name = "name";
            string code = "code";
            ParallelOptions options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 3;
            Parallel.For(0, 30, options,
                index =>
                {
                    Thread.Sleep(5678);
                    Console.WriteLine($"{index},{DateTime.Now},{code},{name}");
                });
        }
        private static void TestThread()
        {
            semaphore = new SemaphoreSlim(3, 5);
            string name = "name";
            string code = "code";
            for (int i = 0; i < 30; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    //semaphore.Wait();
                    TaskThread(name, code);
                    //semaphore.Release();
                });
            }
        }
        private static void TaskThread(string name, string code)
        {
            //Interlocked.Increment(ref count);
            //Console.WriteLine(Interlocked.Read(ref count));
            //return;

            var id = Thread.CurrentContext.ContextID;
            Console.WriteLine($"{id},Enter TaskThread");
            semaphore.Wait();
            Console.WriteLine($"{id},After Wait TaskThread:");
            Interlocked.Increment(ref count);
            Console.WriteLine($"{id},{DateTime.Now},{code},{name},{Interlocked.Read(ref count)}");
            Thread.Sleep(12345);
            Console.WriteLine($"{id},After Sleep TaskThread");
            //Interlocked.Decrement(ref count);
            semaphore.Release();
            Console.WriteLine($"{id},After Release TaskThread");

        }
    }
    #region StockInfo
    //class StockInfoOps
    //{
    //    //private static SemaphoreSlim semaphore;
    //    private static long reds = 0;
    //    internal void WebClientTestGetStockInfo(bool isLocal)
    //    {
    //        var htmlDocument = new HtmlDocument();
    //        htmlDocument.Load(@"D:\stockdata\StockCode.html", Encoding.UTF8);
    //        var list = htmlDocument.GetElementbyId("stockcode");
    //        var dataRows = list.Descendants("a").ToArray();
    //        //semaphore = new SemaphoreSlim(5, 15);
    //        var stopwatch = new Stopwatch();
    //        stopwatch.Start();
    //        foreach (var item in dataRows)
    //        {
    //            var arr = item.InnerText.Split(new[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
    //            var code = arr[1];
    //            var name = arr[0];
    //            if (!code.StartsWith("60") && !code.StartsWith("00") && !code.StartsWith("30")) continue;
    //            Interlocked.Increment(ref reds);
    //            var task = Task.Factory.StartNew(() => PrecessStockInfo(code, name, isLocal));
    //            var threadCount = Interlocked.Read(ref reds);
    //            Console.WriteLine($"threadCount = {threadCount}");
    //            while (Interlocked.Read(ref reds) >= 20)
    //            {
    //                Thread.Sleep(3000);
    //            }
    //            //PrecessStockInfo(code, name, isLocal);
    //            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")},{(int)stopwatch.ElapsedMilliseconds},{item.Name},{item.InnerText},{Path.GetFileNameWithoutExtension(item.GetAttributeValue("href", null))}");
    //            stopwatch.Restart();

    //            if (isLocal) break;
    //            //if (stopwatch.ElapsedMilliseconds > 1000 * 30) break;
    //        }
    //        Console.Write("Press any key to exit...");
    //        Console.ReadKey();
    //    }
    //    private void PrecessStockInfo(string code, string name, bool isLocal)
    //    {
    //        //Interlocked.Increment(ref reds);
    //        //semaphore.Wait();
    //        string categoryContent = null;
    //        string companyInfoContent = null;
    //        HtmlNode categoryDiv = null;
    //        HtmlNode companyInfoTable = null;
    //        var categoryLink = "http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_CorpOtherInfo/stockid/{0}/menu_num/2.phtml";
    //        var companyInfoLink = "http://vip.stock.finance.sina.com.cn/corp/go.php/vCI_CorpInfo/stockid/{0}.phtml";
    //        if (isLocal)
    //        {
    //            categoryLink = "http://localhost:8090/601766_2.html";
    //            companyInfoLink = "http://localhost:8090/601766.html";
    //        }

    //        StockEntities context = new StockEntities();
    //        WebClient client = new WebClient() { Encoding = Encoding.GetEncoding(936) };

    //        try
    //        {
    //            client.Encoding = Encoding.GetEncoding(936);
    //            categoryContent = client.DownloadString(string.Format(categoryLink, code));
    //            companyInfoContent = client.DownloadString(string.Format(companyInfoLink, code));
    //        }
    //        catch (Exception)
    //        {
    //            //throw;
    //        }

    //        string listingDateString = null;

    //        if (!string.IsNullOrEmpty(categoryContent))
    //        {
    //            var categoryHemlDocument = new HtmlDocument() { OptionOutputAsXml = true };
    //            categoryHemlDocument.LoadHtml(categoryContent);
    //            categoryDiv = categoryHemlDocument.GetElementbyId("con02-0");
    //        }
    //        if (!string.IsNullOrEmpty(companyInfoContent))
    //        {
    //            var companyInfoHtmlDocument = new HtmlDocument() { OptionOutputAsXml = true };
    //            companyInfoHtmlDocument.LoadHtml(companyInfoContent);
    //            companyInfoTable = companyInfoHtmlDocument.GetElementbyId("comInfo1");
    //            listingDateString = companyInfoTable?.Descendants("a")?.First()?.InnerText;
    //        }
    //        DateTimeOffset listingDate;
    //        context.StockInfoes.Add(new StockInfo() { Code = code, Name = name, Category = categoryDiv?.OuterHtml, CompanyInfo = companyInfoTable?.OuterHtml, ListingDate = DateTimeOffset.TryParse(listingDateString, out listingDate) ? (DateTimeOffset?)listingDate : null });
    //        context.SaveChanges();
    //        //semaphore.Release();
    //        Interlocked.Decrement(ref reds);
    //    }
    //}
    #endregion StockInfo

}
