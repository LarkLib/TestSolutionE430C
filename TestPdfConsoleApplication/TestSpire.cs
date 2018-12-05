using Spire.Pdf;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPdfConsoleApplication
{
    class TestSpire
    {
        internal void Excute()
        {
            var fileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\Hello.pdf";
            CreateFile(fileName);
            ReadFile(fileName);
        }
        private void HelloPdf()
        {
            //https://www.e-iceblue.com/
            //Create a pdf document.
            PdfDocument doc = new PdfDocument();
            // Create one page
            PdfPageBase page = doc.Pages.Add();
            //Draw the text
            page.Canvas.DrawString(
                "Hello, I'm Created By SPIRE.PDF! \r\n支持中文测试!",
                //new Spire.Pdf.Graphics.PdfFont(PdfFontFamily.TimesRoman, 30f),
                new PdfCjkStandardFont(PdfCjkFontFamily.SinoTypeSongLight, 30f),
                new PdfSolidBrush(Color.Black), 10, 10);
            //Save pdf file.
            doc.SaveToFile($"{AppDomain.CurrentDomain.BaseDirectory}\\Hello.pdf");
            doc.Close();
        }

        private void CreateFile(string fileName)
        {
            PdfDocument doc = new PdfDocument();
            PdfPageBase page = doc.Pages.Add();

            //添加文本  
            page.Canvas.DrawString("Demo of extract text and imgae from PDF!",
            new PdfFont(PdfFontFamily.Helvetica, 20f),
            new PdfSolidBrush(Color.Black), 10, 10);

            //添加图片
            PdfImage image = PdfImage.FromFile("pdf.png");
            float width = image.Width * 0.75f;
            float height = image.Height * 0.75f;
            float x = (page.Canvas.ClientSize.Width - width) / 2;
            page.Canvas.DrawImage(image, x, 60, width, height);

            //PdfImage image2 = PdfImage.FromFile("image.jpg");
            //width = image2.Width * 0.75f;
            //height = image2.Height * 0.75f;
            //page.Canvas.DrawImage(image2, x - 100, 220, width, height);
            doc.SaveToFile(fileName);
        }

        private void ReadFile(string fileName)
        {
            PdfDocument doc = new PdfDocument();
            doc.LoadFromFile(fileName);
            IList<Image> images = new List<Image>();
            foreach (PdfPageBase page in doc.Pages)
            {
                if (page.ExtractImages() != null)
                {
                    foreach (Image image in page.ExtractImages())
                    {
                        images.Add(image);
                    }
                }
            }
            doc.Close();
            int index = 0;
            foreach (Image image in images)
            {
                String imageFileName = String.Format("Image-{0}.png", index++);
                image.Save(imageFileName, ImageFormat.Png);
            }
        }

    }
}
