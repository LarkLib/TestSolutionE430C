using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebBrowserControlDialogs;

namespace QnyDownloader
{
    public partial class WebBrowserForm : Form
    {
        public WebBrowserForm()
        {
            InitializeComponent();

            // Subscribe to Event(s) with the WindowsInterop Class
            WindowsInterop.SecurityAlertDialogWillBeShown += new GenericDelegate<Boolean, Boolean>(this.WindowsInterop_SecurityAlertDialogWillBeShown);
        }

        private void WebBrowserForm_Load(object sender, EventArgs e)
        {
            AddressTextBox.Focus();
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            TestWebBrowser.Navigate(AddressTextBox.Text);
        }
        private void AddressTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) TestWebBrowser.Navigate(AddressTextBox.Text);
        }


        private void TestWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            InfoTextBox.Text += e.Url + "\r\n";
            if (e.Url == TestWebBrowser.Url)
            {
                if (TestWebBrowser.ReadyState == WebBrowserReadyState.Complete || TestWebBrowser.ReadyState == WebBrowserReadyState.Interactive)
                {
                    var htmlDocument = TestWebBrowser.Document;
                    var outerHtml = TestWebBrowser.Document.Body.OuterHtml;
                    var loginTextBox = htmlDocument.GetElementById("login");
                    var passwordTextBox = htmlDocument.GetElementById("password");
                    loginTextBox.InnerText = "shqnymy";
                    passwordTextBox.InnerText = "xx6601";
                }
            }
        }

        private void ProcessDocument()
        {
            Thread.Sleep(10 * 1000);
            var doc = TestWebBrowser.Document;
            var login = TestWebBrowser.Document.GetElementById("login");
        }

        private Boolean WindowsInterop_SecurityAlertDialogWillBeShown(Boolean blnIsSSLDialog)
        {
            // Return true to ignore and not show the 
            // "Security Alert" dialog to the user
            return true;
        }

    }
}
