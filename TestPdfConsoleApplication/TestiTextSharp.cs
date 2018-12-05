using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPdfConsoleApplication
{
    class TestiTextSharp
    {
        internal void Execute()
        {
            string fileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\Hello.pdf";
            CreateFile(fileName);
            var result = ReadFile(fileName);
        }
        private void CreateFile(string fileName)
        {
            ////创建一个pdf文档的对象，设置纸张大小为A4，页边距为0
            ////PageSize.A4.Rotate();当需要把PDF纸张设置为横向时，使用PageSize.A4.Rotate()
            //Document docPDF = new Document(PageSize.A4, 0, 0, 0, 0);
            //PdfWriter write = PdfWriter.GetInstance(doc, new FileStream(@"E:\pdffile.pdf", FileMode.OpenOrCreate, FileAccess.Write));
            //BaseFont bsFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            //Font font = new Font(bsFont);//在这里需要注意的是，itextsharp不支持中文字符，想要显示中文字符的话需要自己设置字体 
            //docPDF.Open();//打开
            //docPDF.Add(new Paragraph("第一个PDF文件", font));//将一句短语写入PDF中

            //docPDF.Close();//关闭

            //定义一个Document，并设置页面大小为A4，竖向 
            //当需要把PDF纸张设置为横向时，使用PageSize.A4.Rotate()
            Document docPdf = new Document(PageSize.A4, 10, 10, 10, 10);
            //写实例 
            PdfWriter.GetInstance(docPdf, new FileStream(fileName, FileMode.Create));
            #region 设置PDF的头信息，一些属性设置，在Document.Open 之前完成
            docPdf.AddAuthor("作者幻想Zerow");
            docPdf.AddCreationDate();
            docPdf.AddCreator("创建人幻想Zerow");
            docPdf.AddSubject("Dot Net 使用 itextsharp 类库创建PDF文件的例子");
            docPdf.AddTitle("此PDF由幻想Zerow创建，嘿嘿");
            docPdf.AddKeywords("ASP.NET,PDF,iTextSharp,幻想Zerow");
            //自定义头 
            docPdf.AddHeader("Expires", "0");
            #endregion //打开document
            docPdf.Open();
            //载入字体 
            //BaseFont.AddToResourceSearch("iTextAsian.dll");
            //BaseFont.AddToResourceSearch("iTextAsianCmaps.dll");
            //"UniGB-UCS2-H" "UniGB-UCS2-V"是简体中文，分别表示横向字 和 // 纵向字 //" STSong-Light"是字体名称 
            BaseFont baseFT = BaseFont.CreateFont(@"c:\windows\fonts\SIMHEI.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font font = new iTextSharp.text.Font(baseFT); //写入一个段落, Paragraph 
            docPdf.Add(new Paragraph("您好， PDF !", font));

            //docPdf.NewPage();//新的一页显示
            docPdf.Add(new Paragraph());
            var table = new PdfPTable(3);
            PdfPCell cellClospan = new PdfPCell(new Phrase("Row 1 , Col 1, Col 2 and col 3, 行合并"));
            cellClospan.Colspan = 3;
            cellClospan.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cellClospan);

            for (int i = 0; i < 27; i++)//表示创建一个3列9行的表格
            {
                var cell1 = new PdfPCell(new Paragraph(i.ToString(), font));

                // tablerow1.AddCell(
                table.AddCell(cell1);//将单元格添加到表格中

            }
            PdfPCell cell = new PdfPCell(new Phrase("Row 1, Col 1"));
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Row 1, Col 2"));
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Row 1, Col 3"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Row 2 ,Col 1"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Row 2 and row 3, Col 2 and Col 3"));
            cell.Rowspan = 2;
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Row 3, Col 1"));

            //PdfPCell 
            cell = new PdfPCell(new Phrase("Row 1, Col 1"));
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Row 1, Col 2"));
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Row 1, Col 3"));
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Row 2 ,Col 1"));
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Row 2 and row 3, Col 2 and Col 3"));
            cell.Rowspan = 2;
            cell.Colspan = 2;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Row 3, Col 1"));
            table.AddCell(cell);


            //PdfPCell 
            cell = new PdfPCell(new Phrase("Row 1, Col 1"));
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Row 1, Col 2"));
            cell.Rotation = 90;
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("Row 1, Col 3 Rotation = -90"));
            cell.Rotation = -90;
            table.AddCell(cell);
            docPdf.Add(table);//将表格添加到pdf文档中   

            //关闭document 
            docPdf.Close();
            //打开PDF，看效果 

        }
        private string ReadFile(string fileName)
        {
            StringBuilder text = new StringBuilder();

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string currentText = PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    text.Append(currentText);
                }
                pdfReader.Close();
            }
            return text.ToString();
        }
    }
}
