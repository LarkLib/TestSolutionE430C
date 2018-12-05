using MigraDoc.DocumentObjectModel;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPdfConsoleApplication
{
    class TestPdfSharp
    {
        internal void Execute()
        {
            var fileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\Hello.pdf";
            CreateFile(fileName);
            ReadFile(fileName);
        }
        private void CreateFile(string fileName)
        {
            // Create a new PDF document
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Created with PDFsharp";
            // Create an empty page
            PdfPage page = document.AddPage();

            // Get an XGraphics object for drawing
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Create a font
            //XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
            PrivateFontCollection pfcFonts = new PrivateFontCollection();
            string strFontPath = @"C:/Windows/Fonts/simhei.ttf";//字体设置为微软雅黑
            pfcFonts.AddFontFile(strFontPath);

            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            XFont font = new XFont(pfcFonts.Families[0], 15, XFontStyle.Regular, options);

            // Draw the text
            gfx.DrawString("Hello, World!", font, XBrushes.Black, new XRect(0, 0, page.Width, 100 /*page.Height*/), XStringFormats.Center);
            gfx.DrawString("中文测试", font, XBrushes.Black, new XRect(0, 50, page.Width, 100 /*page.Height*/), XStringFormats.Center);

            // Save the document...
            document.Save(fileName);
        }
        private void ReadFile(string fileName)
        {

        }
    }
}
