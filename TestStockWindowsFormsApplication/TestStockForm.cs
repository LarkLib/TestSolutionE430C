using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestStockWindowsFormsApplication
{
    public partial class TestStockForm : Form
    {
        public IList<string> DateList { get; set; }
        private IList<PriceItem> PriceItemList { get; set; }
        private IList<PriceItem> PriceList = null;
        private string Code = "sz000060";
        private decimal OpenPrice { get; set; }
        private Graphics ContentPanelGraphics { get; set; }
        string StartDate
        {
            get
            {
                return StartDatePicker.Value.ToString("yyyyMMdd");
            }
        }
        public TestStockForm()
        {
            InitializeComponent();
        }

        private void TestStockForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            StartDatePicker.Value = DateTime.Parse("2014-01-24");
            ContentPanelGraphics = ContentPanel.CreateGraphics();
        }

        private void StartDatePicker_ValueChanged(object sender, EventArgs e)
        {
            //TestStock.StartDate = StartDate;
            DateList = TestStock.GetDateList(StartDate, Code);
            DatecomboBox.DataSource = DateList;
        }

        private void TestStockButton_Click(object sender, EventArgs e)
        {
            RunTest();
        }
        private void RunTest()
        {
            RunTest(new TimeSpan(9, 30, 0), new TimeSpan(15, 0, 0));
        }
        private void RunTest(TimeSpan startTime, TimeSpan endTime, bool refershPriceList = true)
        {
            //Application.DoEvents();
            OpenPrice = TestStock.GetOpenPrice(DatecomboBox.SelectedValue as string, Code);
            PriceList = refershPriceList || PriceList == null ? TestStock.GetPriceList(OpenPrice) : PriceList;
            PriceItemList = TestStock.GetPriceItemList(DatecomboBox.SelectedValue as string, Code);
            ContentPanelGraphics.Clear(ContentPanel.BackColor);
            DrawCoord(startTime, endTime);
            DrawPriceItem();
            DrawPrice(startTime, endTime, Color.Blue);
            DrawPrice(startTime, endTime, Color.DarkOrange, 2, 10);
            TestStock.BuyCount = 0;
            TestStock.SaleCount = 0;
        }
        int width = 1300;
        int hight = 660;
        int startX = 60;
        int startY = 660;
        int priceOffset = 350;
        int xStep = 100;
        int timeStep = 0;

        private void DrawCoord(TimeSpan startTime, TimeSpan endTime)
        {
            ContentPanelGraphics.DrawLine(Pens.Red, new Point(startX, startY), new Point(startX + width, startY));
            ContentPanelGraphics.DrawLine(Pens.Red, new Point(startX, startY), new Point(startX, startY - hight));
            var time = startTime;
            timeStep = (endTime - startTime).TotalMinutes > 60d ? 30 : 5;
            for (int i = 0; time <= endTime; i++)
            {
                var X = startX + (int)((time - startTime).TotalSeconds * (xStep / timeStep / 60d));
                ContentPanelGraphics.DrawString($"{time.Hours:D2}:{time.Minutes:D2}", DefaultFont, Brushes.Black, startX + i * xStep, startY);
                ContentPanelGraphics.DrawLine(Pens.Red, new Point(startX + i * xStep, startY), new Point(startX + i * xStep, startY - 5));
                time = time.Add(new TimeSpan(0, timeStep, 0));
            }
        }
        private void DrawPriceItem()
        {
            if (!PriceList.Any()) return;
            int textXOffset = 30;
            int textYOffset = 5;
            var orderPriceList = PriceList.OrderBy(p => p.Price);
            //ContentPanelGraphics.DrawLine(Pens.Black, new Point(startX, startY - priceOffset), new Point(startX + width, startY - priceOffset));
            //ContentPanelGraphics.DrawString($"{OpenPrice:F2}", new Font(DefaultFont, FontStyle.Bold), Brushes.Red, startX - textXOffset, startY - priceOffset - textYOffset);
            foreach (var item in orderPriceList)
            {
                Pen pen = item.Action == StockAction.Buy ? pen = Pens.Green : (item.Action == StockAction.Sale ? pen = Pens.Red : pen = Pens.Black);
                var Yoffset = priceOffset + (int)((item.Price - OpenPrice) * 100m);
                var brush = item.Price == OpenPrice ? Brushes.Red : Brushes.Black;
                ContentPanelGraphics.DrawLine(pen, new Point(startX, startY - Yoffset), new Point(startX + width, startY - Yoffset));
                ContentPanelGraphics.DrawString($"{item.Price:F2}", DefaultFont, brush, startX - textXOffset, startY - Yoffset - textYOffset);
            }
        }
        private void DrawPrice(TimeSpan startTime, TimeSpan endTime, Color color, float width = 1f, int sleepTime = 0)
        {
            if (!PriceItemList.Any()) return;
            var priceItemListByTime = from item in PriceItemList
                                      where item.PriceDateTime.TimeOfDay >= startTime && item.PriceDateTime.TimeOfDay < endTime
                                      orderby item.PriceDateTime
                                      select item;
            var perPoint = new Point(-1, -1);
            foreach (var item in priceItemListByTime)
            {
                Application.DoEvents();
                var pen = new Pen(color, width);
                var X = startX + (int)((item.PriceDateTime.TimeOfDay - startTime).TotalSeconds * (xStep / timeStep / 60d));
                var Y = startY - priceOffset - (int)((item.Price - OpenPrice) * 100m);
                if (perPoint.X < 0)
                {
                    perPoint.X = X;
                    perPoint.Y = Y;
                }
                ContentPanelGraphics.DrawLine(pen, new Point(perPoint.X, perPoint.Y), new Point(X, Y));
                if (sleepTime > 0)
                {
                    //if (X >= X - (X % 100) && perPoint.X < X - (X % 100))
                    //{
                    //    StockToolTip.Show($"X={X}", this, X, Y, 50);
                    //}
                    var hitOne = TestStock.CheckPrice(Code, item, ref PriceList, OpenPrice);
                    if (hitOne != null)
                    {
                        //Thread.Sleep(200);
                        DrawPriceItem();
                        //StockToolTip.Show($"Price:{item.Price},{text}", this, X, Y, 100);
                        ContentPanelGraphics.DrawRectangle(Pens.Red, new Rectangle(X - 5, Y - 5, 10, 10));
                        ContentPanelGraphics.DrawString($"{item.Price},{hitOne.Action.ToString()}", DefaultFont, Brushes.Black, X + 5, Y - 15);
                        InfoListBox.Items.Insert(0, $"{item.PriceDateTime.ToString("yyyyMMdd HH:mm:ss")},{item.Price},{hitOne.Action}");
                        InfoListBox.Refresh();
                        //Thread.Sleep(5000);
                    }
                    //Thread.Sleep(sleepTime);
                    SpinWait.SpinUntil(() => false, sleepTime);
                }
                perPoint.X = X;
                perPoint.Y = Y;
            }
        }
        private void PerDayButton_Click(object sender, EventArgs e)
        {
            DatecomboBox.SelectedIndex = DatecomboBox.SelectedIndex > 0 ? DatecomboBox.SelectedIndex - 1 : 0;
            RunTest();
        }

        private void NextDayButton_Click(object sender, EventArgs e)
        {
            DatecomboBox.SelectedIndex = DatecomboBox.SelectedIndex < DatecomboBox.Items.Count ? DatecomboBox.SelectedIndex + 1 : DatecomboBox.Items.Count;
            RunTest();
        }

        private void PM930Button_Click(object sender, EventArgs e)
        {
            RunTest(new TimeSpan(9, 30, 0), new TimeSpan(10, 30, 0), false);
        }

        private void PM1030Button_Click(object sender, EventArgs e)
        {
            RunTest(new TimeSpan(10, 30, 0), new TimeSpan(11, 30, 0), false);
        }

        private void AM1300Button_Click(object sender, EventArgs e)
        {
            RunTest(new TimeSpan(13, 0, 0), new TimeSpan(14, 0, 0), false);
        }

        private void AM1400Button_Click(object sender, EventArgs e)
        {
            RunTest(new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0), false);
        }


        private void ContentPanel_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = PointToClient(MousePosition);
            int x = point.X;
            int y = point.Y;
            ContentPanelGraphics.Clear(this.BackColor);
            ContentPanelGraphics.DrawLine(Pens.Black, 0, y, ContentPanel.Width, y);
            ContentPanelGraphics.DrawLine(Pens.Black, x, 0, x, ContentPanel.Height);
        }
    }
}
