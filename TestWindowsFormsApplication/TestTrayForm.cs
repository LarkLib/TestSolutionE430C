using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWindowsFormsApplication
{
    public partial class TestTrayForm : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public TestTrayForm()
        {
            InitializeComponent();
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            if (e is MouseEventArgs && ((MouseEventArgs)e).Button == MouseButtons.Right)
            {
                contextMenuStripTray.Show();
            }
        }

        private void MaxWindowsMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.ShowDialog();
        }

        private void CloseWindowsMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TestTrayForm_Load(object sender, EventArgs e)
        {

            notifyIcon.Visible = false;
            AddressTextBox.Text = "http://localhost:8090/home.htm";


            long capacity = 1 << 4;
            using (var mmf = MemoryMappedFile.OpenExisting("testMmf"))

            {
                MemoryMappedViewAccessor viewAccessor = mmf.CreateViewAccessor(0, capacity);
                var c = viewAccessor.ReadChar(8);
                //读取字符长度
                int strLength = viewAccessor.ReadInt32(0);
                char[] charsInMMf = new char[strLength];

                //读取字符
                viewAccessor.ReadArray<char>(4, charsInMMf, 0, strLength);
                var resultString = new string(charsInMMf);
            }

        }

        private void TestTrayForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon.Visible = true;
            }
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            //TestWebBrowser.Url = new Uri(AddressTextBox.Text);
            //AddressTextBox.Text = "http://finance.sina.com.cn/realstock/company/sh600900/nc.shtml";
            //AddressTextBox.Text = "http://vip.stock.finance.sina.com.cn/corp/go.php/vMS_MarketHistory/stockid/600900.phtml";
            TestWebBrowser.Navigate(AddressTextBox.Text);
            while (this.TestWebBrowser.ReadyState != WebBrowserReadyState.Interactive && this.TestWebBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }

            var htmlDocument = TestWebBrowser.Document;
            var testTextBox = htmlDocument.GetElementById("price");
            //testTextBox.InnerText = "ttttttttttt";
            var document = TestWebBrowser.DocumentText;
            var price = TestWebBrowser.Document.GetElementById("price");
            //price.InnerText = "100.99";
            var slect = TestWebBrowser.Document.GetElementsByTagName("select");
            var inputs = TestWebBrowser.Document.GetElementsByTagName("input");
            var div = TestWebBrowser.Document.GetElementById("con02-4");
            document = null;
            foreach (HtmlElement item in TestWebBrowser.Document.All)
            {
                if (item.GetAttribute("name") == "year")
                {
                    var year = item;
                    //price.InnerText = "100.99";

                }
            }
        }

        private void TestWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url == TestWebBrowser.Url)
            {
                if (TestWebBrowser.ReadyState == WebBrowserReadyState.Complete)
                {
                    var htmlDocument = TestWebBrowser.Document;
                    //((WebBrowser)sender).Print();
                    var forms = TestWebBrowser.Document.GetElementsByTagName("form");
                    foreach (HtmlElement item in TestWebBrowser.Document.All)
                    {
                        if (item.GetAttribute("name") == "year")
                        {
                            var year = item;
                            var options = year.GetElementsByTagName("option");
                            foreach (HtmlElement option in options)
                            {
                                if (option.GetAttribute("value") == "2006")
                                {
                                    option.SetAttribute("selected", "true");
                                }
                                var form = forms[2];
                                var inputs = form.GetElementsByTagName("input");
                                foreach (HtmlElement input in inputs)
                                {
                                    if (input.GetAttribute("type") == "submit" && input.GetAttribute("value") == "查询")
                                    {
                                        input.InvokeMember("click");
                                    }
                                }
                            }

                        }
                    }
                    var window = FindWindow(null, "TestTray");
                }
            }
        }

        private void Input_Click(object sender, HtmlElementEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
