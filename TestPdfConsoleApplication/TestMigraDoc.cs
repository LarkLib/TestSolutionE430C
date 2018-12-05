using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.IO;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPdfConsoleApplication
{
    class TestMigraDoc
    {
        internal void Execute()
        {
            var fileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\Hello.pdf";
            CreateReceivingNote(fileName);
            //TestPrintDocument();
            //TestTable(fileName);
            //TestMain();
        }

        private void CreateReceivingNote(string fileName = null)
        {
            Font simHeiFont = new Font("SimHei");

            var document = new Document();
            //document.DefaultPageSetup.TopMargin = "1.0mm";
            document.AddSection();
            //var textFrame = document.LastSection.AddTextFrame();
            //textFrame.Left = "0.0cm";
            //textFrame.Top = "0.0cm";
            //textFrame.Width = "100.0mm"; ;//document.DefaultPageSetup.PageWidth;
            //textFrame.Height = "1.0cm";
            //var titleParagraph = textFrame.AddParagraph();
            var titleParagraph = document.LastSection.AddParagraph();
            titleParagraph.Format.Font = simHeiFont.Clone();
            titleParagraph.Format.Font.Size = "0.6cm";
            titleParagraph.Format.Alignment = ParagraphAlignment.Center;
            titleParagraph.AddFormattedText("收货单", TextFormat.Bold);

            document.LastSection.AddParagraph();
            document.LastSection.AddParagraph();
            document.LastSection.AddParagraph();
            var poParagraph = document.LastSection.AddParagraph();
            poParagraph.Format.Font = simHeiFont.Clone();
            //poParagraph.Format.Font.Size = "1.0cm";
            poParagraph.Format.Alignment = ParagraphAlignment.Left;
            poParagraph.AddFormattedText("采购单号：");
            poParagraph.AddFormattedText("CG201812020228", TextFormat.Bold);
            poParagraph.AddFormattedText("    采购单时间：");
            poParagraph.AddFormattedText("2018-12-02 20:13)", TextFormat.Bold);

            var infoParagraph = document.LastSection.AddParagraph();
            infoParagraph.Format.Font = simHeiFont.Clone();
            //infoParagraph.Format.Font.Size = "1.0cm";
            infoParagraph.Format.Alignment = ParagraphAlignment.Left;
            infoParagraph.AddFormattedText("门店名称：");
            infoParagraph.AddFormattedText("小象生鲜（无锡广益店）", TextFormat.Bold);
            infoParagraph.AddFormattedText("    门店联系人：");
            infoParagraph.AddFormattedText("付明亮", TextFormat.Bold);
            infoParagraph.AddFormattedText("    门店联系电话：");
            infoParagraph.AddFormattedText("15961756029", TextFormat.Bold);

            document.LastSection.AddParagraph();
            document.LastSection.AddParagraph().AddFormattedText("采购详情", simHeiFont.Clone());

            var table = new Table();
            table.Borders.Width = 0.75;
            table.Borders.Color = Colors.Black;

            table.AddColumn(Unit.FromCentimeter(2.5));
            table.AddColumn(Unit.FromCentimeter(7));
            table.AddColumn(Unit.FromCentimeter(2.5));
            table.AddColumn(Unit.FromCentimeter(2.5));
            table.AddColumn(Unit.FromCentimeter(2.5));

            var row = table.AddRow();
            row.Format.Alignment = ParagraphAlignment.Center;
            //row.Shading.Color = Colors.PaleGoldenrod;
            row.Cells[0].AddParagraph().AddFormattedText("skuId");
            row.Cells[1].AddParagraph().AddFormattedText("商品名称", simHeiFont.Clone());
            row.Cells[2].AddParagraph().AddFormattedText("规格", simHeiFont.Clone());
            row.Cells[3].AddParagraph().AddFormattedText("单位", simHeiFont.Clone());
            row.Cells[4].AddParagraph().AddFormattedText("订单数量", simHeiFont.Clone());
            for (int i = 0; i < 100; i++)
            {
                row = table.AddRow();
                row.Format.Alignment = ParagraphAlignment.Left;
                //row.Shading.Color = Colors.PaleGoldenrod;
                row.Cells[0].AddParagraph().AddFormattedText("17412");
                row.Cells[1].AddParagraph().AddFormattedText("鲜活 罗氏沼虾40-60头/500g", simHeiFont.Clone());
                row.Cells[2].AddParagraph().AddFormattedText("40-60头", simHeiFont.Clone());
                row.Cells[3].AddParagraph().AddFormattedText("kg", simHeiFont.Clone());
                row.Cells[4].AddParagraph().AddFormattedText("100", simHeiFont.Clone());
            }

            document.LastSection.Add(table);

            var shopName = "小象生鲜（无锡广益店）";
            var index = shopName.IndexOf('（');
            index = index == -1 ? shopName.IndexOf('(') : index;
            index = index == -1 ? 0 : index + 1;
            var shop = shopName.Substring(index, shopName.Length - index - 1);

            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();
            renderer.PdfDocument.Save(fileName);
        }

        private void TestTable(string fileName)
        {
            var document = new Document();
            document.AddSection();
            var textFrame = document.LastSection.AddTextFrame();
            textFrame.Left = "5.0cm";
            textFrame.Top = "1.0cm";
            textFrame.Width = "10.0cm";
            textFrame.Height = "5.0cm";
            var lineFormat = new LineFormat();
            lineFormat.Color = Colors.Blue;
            lineFormat.Width = "0.08cm";
            textFrame.LineFormat = lineFormat;

            textFrame.AddParagraph().AddFormattedText("AddFormattedText", TextFormat.Bold | TextFormat.Italic);

            document.LastSection.AddParagraph("Simple Tables", "Heading2");
            document.LastSection.AddParagraph("Simple Tables2", "Heading2");
            document.LastSection.AddParagraph("Simple Tables3", "Heading2");


            var addressFrame = document.LastSection.AddTextFrame();
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "7.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "7.0cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            var paragraph = addressFrame.AddParagraph();
            paragraph.Format.LineSpacing = "5.25cm";
            paragraph.Format.LineSpacingRule = LineSpacingRule.Exactly;
            // And now the paragraph with text.
            paragraph.Format.SpaceBefore = 0;
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("INVOICE", TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText("Cologne, ");
            paragraph.AddDateField("dd.MM.yyyy");

            // Add the notes paragraph.
            paragraph = document.LastSection.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.SpaceBefore = "1cm";
            paragraph.Format.Borders.Width = 0.75;
            paragraph.Format.Borders.Distance = 3;
            //paragraph.Format.Borders.Color = TableBorder;
            //paragraph.Format.Shading.Color = TableGray;

            // Create the text frame for the address.

            var table = new Table();
            table.Borders.Width = 0.75;
            table.Borders.Color = Colors.DarkCyan;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 1.5;
            table.Borders.Right.Width = 2.5;
            table.Rows.LeftIndent = 0;

            var column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));

            table.AddColumn(Unit.FromCentimeter(5));

            var row = table.AddRow();
            row.Shading.Color = Colors.PaleGoldenrod;
            var cell = row.Cells[0];
            cell.AddParagraph("Itemus");
            cell = row.Cells[1];
            cell.AddParagraph("Descriptum");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("1");
            cell = row.Cells[1];
            cell.AddParagraph(FillerText.ShortText);

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("2");
            cell = row.Cells[1];
            cell.AddParagraph(FillerText.Text);

            table.SetEdge(0, 0, 3, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Red);

            document.LastSection.Add(table);

            //var ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);
            DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;

            renderer.RenderDocument();

            // Save the document...
            renderer.PdfDocument.Save(fileName);

            //MigraDocPrintDocument printDocument = new MigraDocPrintDocument();

            // ...and start a viewer.
            //Process.Start(filename);
        }

        private void TestMain()
        {
            // Create a MigraDoc document.
            var document = CreateDocument();

            //var ddl = MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToString(document);
            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;

            renderer.RenderDocument();

            // Save the document...
#if DEBUG
            var filename = Guid.NewGuid().ToString("N").ToUpper() + ".pdf";
#else
            var filename = "HelloMigraDoc.pdf";
#endif
            renderer.PdfDocument.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }
        private static Document CreateDocument()
        {
            // Create a new MigraDoc document.
            var document = new Document();
            document.Info.Title = "Hello, MigraDoc";
            document.Info.Subject = "Demonstrates an excerpt of the capabilities of MigraDoc.";
            document.Info.Author = "Stefan Lange";
            document.AddSection();
            //Styles.DefineStyles(document);
            //Cover.DefineCover(document);
            //TableOfContents.DefineTableOfContents(document);
            //DefineContentSection(document);
            //Paragraphs.DefineParagraphs(document);
            //Tables.DefineTables(document);
            //Charts.DefineCharts(document);
            Tables.DefineTables(document);
            return document;
        }
        #region Test Print pdf file
        private void TestPrintDocument()
        {

            PrinterSettings printerSettings = new PrinterSettings();

            using (System.Windows.Forms.PrintDialog dialog = new System.Windows.Forms.PrintDialog())
            {
                dialog.PrinterSettings = printerSettings;
                dialog.AllowSelection = true;
                dialog.AllowSomePages = true;
                //var result = dialog.ShowDialog();

                //if (result == System.Windows.Forms.DialogResult.OK)
                {
                    PrintDemo(printerSettings);
                }
            }
        }

        private void PrintDemo(PrinterSettings printerSettings)
        {
            var document = MakeDemoDocument();

#if WPF
            // MigraDocPrintDocument uses the GDI build of PDFsharp/MigraDoc.
            // The Document class known to MigraDocPrintDocument is not the Document class we use here (it is from the WPF build).
            // To bridge the gap, we have to use MigraDocPrintDocumentEx and pass an MDDDL string.
            MigraDocPrintDocumentEx printDocument = new MigraDocPrintDocumentEx(DdlWriter.WriteToString(document));
#else
            // Creates a PrintDocument that simplyfies printing of MigraDoc documents
            // Code that uses the GDI build of PDFsharp/MigraDoc can use the MigraDocPrintDocument class and pass the object of Document class directly.
            // Using an MDDDL will work with all builds, but will lose a few nanoseconds.
            MigraDocPrintDocument printDocument = new MigraDocPrintDocument(document);
#endif

            // Attach the current printer settings
            printDocument.PrinterSettings = printerSettings;

            if (printerSettings.PrintRange == PrintRange.Selection)
                throw new NotImplementedException();

            // Print the document
            printDocument.Print();
        }

        private Document MakeDemoDocument()
        {
            // Create a new MigraDoc document
            Document document = new Document();

            // Add a section to the document
            Section section = document.AddSection();

            // Add a paragraph to the section
            Paragraph paragraph = section.AddParagraph();

            paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);

            // Add some text to the paragraph
            paragraph.AddFormattedText("Hello, World!", TextFormat.Bold);

            return document;
        }

        #endregion
    }
    public class Tables
    {
        public static void DefineTables(Document document)
        {
            var paragraph = document.LastSection.AddParagraph("Table Overview", "Heading1");
            paragraph.AddBookmark("Tables");

            DemonstrateSimpleTable(document);
            DemonstrateAlignment(document);
            DemonstrateCellMerge(document);
        }

        public static void DemonstrateSimpleTable(Document document)
        {
            document.LastSection.AddParagraph("Simple Tables", "Heading2");

            var table = new Table();
            table.Borders.Width = 0.75;

            var column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));

            var row = table.AddRow();
            row.Shading.Color = Colors.PaleGoldenrod;
            var cell = row.Cells[0];
            cell.AddParagraph("Itemus");
            cell = row.Cells[1];
            cell.AddParagraph("Descriptum");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("1");
            cell = row.Cells[1];
            cell.AddParagraph(FillerText.ShortText);

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("2");
            cell = row.Cells[1];
            cell.AddParagraph(FillerText.Text);

            table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }

        public static void DemonstrateAlignment(Document document)
        {
            document.LastSection.AddParagraph("Cell Alignment", "Heading2");

            var table = document.LastSection.AddTable();
            table.Borders.Visible = true;
            table.Format.Shading.Color = Colors.LavenderBlush;
            table.Shading.Color = Colors.Salmon;
            table.TopPadding = 5;
            table.BottomPadding = 5;

            var column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Right;

            table.Rows.Height = 35;

            var row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Top;
            row.Cells[0].AddParagraph("Text");
            row.Cells[1].AddParagraph("Text");
            row.Cells[2].AddParagraph("Text");

            row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Center;
            row.Cells[0].AddParagraph("Text");
            row.Cells[1].AddParagraph("Text");
            row.Cells[2].AddParagraph("Text");

            row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].AddParagraph("Text");
            row.Cells[1].AddParagraph("Text");
            row.Cells[2].AddParagraph("Text");
        }

        public static void DemonstrateCellMerge(Document document)
        {
            document.LastSection.AddParagraph("Cell Merge", "Heading2");

            var table = document.LastSection.AddTable();
            table.Borders.Visible = true;
            table.TopPadding = 5;
            table.BottomPadding = 5;

            var column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Right;

            table.Rows.Height = 35;

            var row = table.AddRow();
            row.Cells[0].AddParagraph("Merge Right");
            row.Cells[0].MergeRight = 1;

            row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].MergeDown = 1;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].AddParagraph("Merge Down");

            table.AddRow();
        }
    }
    public class FillerText
    {
        public static string ShortText
        {
            get { return "Andigna cons nonsectem accummo diamet nis diat."; }
        }

        public static string Text
        {
            get
            {
                return "Loboreet autpat, quis adigna conse dipit la consed exeril et utpatetuer autat, voloboreet, consequamet ilit nos aut in henit ullam, sim doloreratis dolobore tat, venim quissequat. " +
                    "Nisci tat laor ametumsan vulla feuisim ing eliquisi tatum autat, velenisit iustionsed tis dunt exerostrud dolore verae.";
            }
        }

        public static string MediumText
        {
            get
            {
                return "Incinibh ecte dionsent am, sisl ute magna faccum ing eu feugait ulla consequismod tetum zzrilluptat. Ut velis accum dit la corper inci essequat, venis nisl dolutat. Sandipit esequisit autpat. " +
                    "Magnibh et laortie feugiamcommy nulluptat dolorpero euipis nonum augait wis dit, quamcon sequipit at vel il eui blaorper si tat ipit at nis nullan hent num dunt irit, sum dolendio consendigna consent " +
                    "lan ut illan etue miniam dolenisis nonsenim inim quat, conulla orercinisim vel inci ent illam quat volore veliquam amconsequat. Ut lut incincipit nullaor iriurercip et luptat erat illamco mmoluptat.\n" +
                    "Ut iriusciduis nonsed do el dolut ea autem il dolore verci blam, quatue el ute facilis cidunt dit alisl ut lut num vercinc illaore del ilisi blandre commodit, quamcon sequipsusto dunt ver illaorperit utpat, " +
                    "velisci lisciniam vent alis nostisi et, quisit, con eu facipit vulputpat.";
            }
        }

        public static string LargeText
        {
            get
            {
                return "Enim vulput ea am, conulput wisi endio ex ent ut velit nosting eugait nonullut nonse modolorperat vulla acipsuscil ut augue tet verilis modiate commodo lesequis eugue esto eugiam in esto corperosto " +
                    "dipiscipis dunt acil do dolutpatummy nos eugiam nonum ea alisit delit, vendio od tatumsan henim nullamconse minci tem delenit iusto eummolore magna consent ea am aliquis adigna con er sent ad mincidunt num vulla " +
                    "autat la alit alit am, volutate dolortin ullut alit wis adit verostrud tisi.\n" +
                    "Ad estionulla feu faci tinit atie modipsuscip essi.\n" +
                    "Suscinibh el ex euguer autem iure dolum doluptat laoreros am velenia mconulla corem nos ea facilisl et eugiat ecte minisit wis er in utat ip exeratie mincidunt wiscilis nisci essecte eriliquip et illutem duisim " +
                    "velestrud tat ilisisis ese molesenim vercil el ute magniam augue min erosto con volorper adignim dolorer ostrud exerostio odo doloreet ex et utat. Ud tat. Andipsum erosto odolum dolesequat vent augiamcon ullam " +
                    "euipis duis adignis autem nullamet, velisi. Tet doloreet venibh ex et ut nullamet adipisis accum ipis acing exer sed et, vero odoloreet, veros atie dolore dolorem quat, se velit velit wismodit er sum iure " +
                    "te facidunt doloreraesto do dolorem nos nos erat at nit in utpat adigna feugiamet aliquissi blaore dui blan utat vero dolessim zzriustrud digniat, volobor secte core feuis dolore vero odip et ad mod te conulla " +
                    "feugait volor sum iusciduismod dunt vendigna ad del ut dunt alit augue eliquam ip el ipit in veliqui ssenisci te tis ercing";
            }
        }
    }
}
