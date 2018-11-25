using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace TestWebBrowserWindowsFormsApplication
{
    public partial class WebBrowserForm : Form
    {
        public ChromiumWebBrowser chromeBrowser;

        public void InitializeChromium()
        {
            CefSettings settings = new CefSettings();

            // Initialize cef with the provided settings
            Cef.Initialize(settings);

            // Create a browser component
            chromeBrowser = new ChromiumWebBrowser("https://vss.baobaoaichi.cn/login.html");
            // Add it to the form and fill it to the form window.
            this.WebBrowserPanel.Controls.Add(chromeBrowser);
            chromeBrowser.Dock = DockStyle.Fill;
            chromeBrowser.FrameLoadEnd += ChromeBrowser_FrameLoadEnd;
            
        }

        private void ChromeBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            MessageLabel.Text = "ChromeBrowser_FrameLoadEnd";
            if (e.Frame.IsMain)
            {
                //chromeBrowser.ViewSource();
                chromeBrowser.GetSourceAsync().ContinueWith(taskHtml =>
                {
                    var html = taskHtml.Result;
                    ContentTextBox.Text = html;
                });
            }
        }

        public WebBrowserForm()
        {
            InitializeComponent();
            InitializeChromium();
        }

        private void WebBrowserForm_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void WebBrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            //chromeBrowser = new ChromiumWebBrowser("https://vss.baobaoaichi.cn/login.html");
            //chromeBrowser = new ChromiumWebBrowser(UrlTextBox.Text);
            //this.WebBrowserPanel.Controls.Add(chromeBrowser);
            //chromeBrowser.Dock = DockStyle.Fill;

            //获得页面源代码
            String html = chromeBrowser.GetSourceAsync().Result;
            ContentTextBox.Text = html;

            //chromeBrowser.ExecuteScriptAsync("alert('hello!')");
            //chromeBrowser.ExecuteScriptAsync("document.getElementById('login');");
            chromeBrowser.ExecuteScriptAsync("document.getElementById('login').value='shqnymy';");

            // Get Document Height
            //var task = chromeBrowser.GetMainFrame().EvaluateScriptAsync("(function() { var body = document.body, html = document.documentElement; return  Math.max( body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight ); })();", null);
            var task = chromeBrowser.GetMainFrame().EvaluateScriptAsync("(function() { return 'a'+'b';})();", null);
            task.Wait();

            var response = task.Result;
            var result = response.Success ? (response.Result ?? "null") : response.Message;
            ContentTextBox.Text = result as string;

            //task.ContinueWith(t =>
            //{
            //    if (!t.IsFaulted)
            //    {
            //        var response = t.Result;
            //        string EvaluateJavaScriptResult = response.Success ? (response.Result ?? "null") : response.Message;
            //    }
            //}, TaskScheduler.FromCurrentSynchronizationContext());


        }
    }
}
