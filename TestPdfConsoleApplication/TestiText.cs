using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.Printing;

namespace TestPdfConsoleApplication
{
    class TestiText
    {
        internal void Execute()
        {
            TestPrint3();
            //TestPrint2();
            TestPrint();
        }

        private void TestPrint3()
        {
            PrintQueueCollection printQueues = null;
            List<PrinterDescription> printerDescriptions = null;

            // Get a list of available printers.
            var printServer = new PrintServer();
            printQueues = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            printerDescriptions = new List<PrinterDescription>();

            foreach (PrintQueue printQueue in printQueues)
            {
                // The OneNote printer driver causes crashes in 64bit OSes so for now just don't include it.
                // Also redirected printer drivers cause crashes for some printers. Another WPF issue that cannot be worked around.
                if (printQueue.Name.ToUpperInvariant().Contains("ONENOTE") || printQueue.Name.ToUpperInvariant().Contains("REDIRECTED"))
                {
                    continue;
                }

                string status = printQueue.QueueStatus.ToString();

                PrinterDescription printerDescription = new PrinterDescription()
                {
                    Name = printQueue.Name,
                    FullName = printQueue.FullName,
                    //Status = status == Strings.Printing_PrinterStatus_NoneTxt ? Strings.Printing_PrinterStatus_ReadyTxt : status,
                    Status = status,
                    ClientPrintSchemaVersion = printQueue.ClientPrintSchemaVersion,
                    DefaultPrintTicket = printQueue.DefaultPrintTicket,
                    PrintCapabilities = printQueue.GetPrintCapabilities(),
                    PrintQueue = printQueue
                };

                printerDescriptions.Add(printerDescription);
            }
        }

        private void TestPrint2()
        {
            string query = string.Format("SELECT * from Win32_Printer ");
            var searcher = new ManagementObjectSearcher(query);
            var printers = searcher.Get();

            foreach (var printer in printers)
            {
                Console.WriteLine(printer.Properties["Name"].Value);
                foreach (var property in printer.Properties)
                {
                    Console.WriteLine(string.Format("\t{0}: {1}", property.Name, property.Value));
                }
                Console.WriteLine();
            }
        }

        private void TestPrint()
        {
            string fileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\Hello.pdf";
            ////Initialize PDF writer
            //PdfWriter writer = new PdfWriter(fileName);
            ////Initialize PDF document
            //PdfDocument pdf = new PdfDocument(writer);
            //// Initialize document
            //Document document = new Document(pdf);
            ////Add paragraph to the document
            //document.Add(new Paragraph("Hello World! 中文测试"));
            ////Close document
            //document.Close();

            //iText.IO.Util.ResourceUtil.AddToResourceSearch("itext.font_asian.dll");
            var writer = new PdfWriter(fileName);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var sysfonts = new System.Drawing.Text.InstalledFontCollection().Families;
            string fontsfolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            //var ds= PdfFontFactory.RegisterSystemDirectories();

            // Create a PdfFont
            var fonts = PdfFontFactory.GetRegisteredFonts();
            fonts = PdfFontFactory.GetRegisteredFamilies();
            var font = PdfFontFactory.CreateFont(FontConstants.TIMES_ROMAN);
            //font = PdfFontFactory.createRegisteredFont(fontname, PdfEncodings.IDENTITY_H);
            font = PdfFontFactory.CreateFont(@"c:\windows\fonts\SIMHEI.TTF", PdfEncodings.IDENTITY_H, embedded: false);
            //https://github.com/itext/itext7-dotnet/tree/master/itext/itext.font-asian
            font = PdfFontFactory.CreateFont("STSong-Light", "UniGB-UCS2-H", true);
            // Add a Paragraph
            document.Add(new Paragraph("iText is:").SetFont(font));
            // Create a List
            List list = new List()
                .SetSymbolIndent(12)
                .SetListSymbol("。")
                .SetFont(font);
            // Add ListItem objects
            list.Add(new ListItem("Never gonna give you up"))
                .Add(new ListItem("Never gonna let you down"))
                .Add(new ListItem("Never gonna run around and desert you"))
                .Add(new ListItem("Never gonna make you cry"))
                .Add(new ListItem("Never gonna say goodbye"))
                .Add(new ListItem("中文测试"))
                .Add(new ListItem("Never gonna tell a lie and hurt you"));
            // Add the list
            document.Add(list);
            document.Close();

            //Thread.Sleep(5000);

            List<string> PrinterFound = new List<string>();
            var printer = new System.Drawing.Printing.PrinterSettings();
            foreach (var item in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                PrinterFound.Add(item.ToString());
            }
            var currentPinter = PrinterFound[1];
            printer.PrinterName = currentPinter;
            printer.PrintFileName = fileName;

            var PrintDoc = new System.Drawing.Printing.PrintDocument();

            PrintDoc.DocumentName = fileName;
            PrintDoc.PrinterSettings.PrinterName = currentPinter;

            //PrintDoc.Print();
            //Thread.Sleep(5000);
        }
    }

    internal class PrinterDescription
    {
        public int ClientPrintSchemaVersion { get; internal set; }
        public PrintTicket DefaultPrintTicket { get; internal set; }
        public string FullName { get; internal set; }
        public string Name { get; internal set; }
        public PrintCapabilities PrintCapabilities { get; internal set; }
        public PrintQueue PrintQueue { get; internal set; }
        public object Status { get; internal set; }
    }
}
