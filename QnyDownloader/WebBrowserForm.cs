using mshtml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
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
        private static Utilities utilities = new Utilities();
        private static string Login = ConfigurationManager.AppSettings["Login"];
        private static string Password = ConfigurationManager.AppSettings["Password"];
        private static int Interval = int.Parse(ConfigurationManager.AppSettings["Interval"] ?? "180") * 1000;
        private static bool IsLogin = false;
        private string LoginUrlHeader = "https://epassport.meituan.com/account/unitivelogin";
        private string LoginUrl = "https://epassport.meituan.com/account/unitivelogin?bg_source=14&continue=https:%2F%2Fvss.baobaoaichi.cn%2Fauth%2Flogin%3Ftype%3DLOGIN&leftBottomLink=https:%2F%2Fvss.baobaoaichi.cn%2Fsignup.html&part_type=0&rightBottomLink=https:%2F%2Fvss.baobaoaichi.cn%2Frecover.html&service=com.sankuai.mall.fe.vss";
        private string AutoLoginUrlHeader = "https://vss.baobaoaichi.cn/auth/login";
        private string PurchaseListUrl = "https://vss.baobaoaichi.cn/purchase/list.html";
        private string Bsid = null;
        public WebBrowserForm()
        {
            InitializeComponent();

            // Subscribe to Event(s) with the WindowsInterop Class
            WindowsInterop.SecurityAlertDialogWillBeShown += new GenericDelegate<Boolean, Boolean>(this.WindowsInterop_SecurityAlertDialogWillBeShown);
        }

        private void WebBrowserForm_Load(object sender, EventArgs e)
        {
            AddressTextBox.Focus();
            AddressTextBox.Text = LoginUrl;
            TestWebBrowser.Navigate(LoginUrl);
            Task.Run(() => CheckLogin());
        }

        private void GoButton_Click(object sender, EventArgs e)
        {
            GoButton.Enabled = false;
            TestWebBrowser.Visible = true;
            TestWebBrowser.Navigate(AddressTextBox.Text);
            GoButton.Enabled = true;
        }
        private void AddressTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                AddressTextBox.Refresh();
                TestWebBrowser.Navigate(AddressTextBox.Text);
            }
        }

        private void TestWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var currentUrl = e.Url.AbsoluteUri;
            if (currentUrl.Contains(LoginUrlHeader))
            {
                #region Login
                Utilities.LogInfo("Running auto login");
                if (TestWebBrowser.ReadyState == WebBrowserReadyState.Complete || TestWebBrowser.ReadyState == WebBrowserReadyState.Interactive)
                {
                    var htmlDocument = TestWebBrowser.Document;
                    var outerHtml = TestWebBrowser.Document.Body.OuterHtml;
                    var loginTextBox = htmlDocument.GetElementById("login");
                    var passwordTextBox = htmlDocument.GetElementById("password");
                    if (loginTextBox != null && passwordTextBox != null)
                    {
                        loginTextBox.InvokeMember("focus");
                        SendKeys.SendWait($"({Login})");
                        passwordTextBox.InvokeMember("focus");
                        SendKeys.SendWait($"({Password})");
                        var elements = htmlDocument.GetElementsByTagName("button");
                        foreach (HtmlElement element in elements)
                        {
                            if (element.InnerText.Equals("登录"))
                            {
                                element.InvokeMember("click");
                            }
                        }
                    }
                }
                #endregion
            }
            else if (currentUrl.Contains(AutoLoginUrlHeader))
            {
                #region AutoLogin
                Utilities.LogInfo("Auto login done");
                IsLogin = true;
                var parameters = currentUrl.Split(new[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in parameters)
                {
                    if (item.Contains("BSID="))
                    {
                        Bsid = item.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    }
                }
                #endregion
            }
            else if (currentUrl.Contains(PurchaseListUrl))
            {
                #region PurchaseListUrl
                if (TestWebBrowser.ReadyState == WebBrowserReadyState.Complete || TestWebBrowser.ReadyState == WebBrowserReadyState.Interactive)
                {
                    Utilities.LogInfo("redirct to list page");
                    //Task.Run(() => GetSupplierPmsPoList());
                    GetSupplierPmsPoList();
                    TestWebBrowser.Visible = false;
                }
                #endregion
            }
        }

        private void GetSupplierPmsPoList()
        {
            SyncTimer.Interval = Interval;
            SyncTimer.Enabled = true;
            SyncTimer.Start();
        }
        private Boolean WindowsInterop_SecurityAlertDialogWillBeShown(Boolean blnIsSSLDialog)
        {
            // Return true to ignore and not show the 
            // "Security Alert" dialog to the user
            return true;
        }
        private void SyncTimer_Tick(object sender, EventArgs e)
        {
            SyncTimer.Enabled = false;
            Utilities.CheckTimeWithExit();
            utilities.GetSupplierPmsPoList(Bsid);
            SyncTimer.Enabled = true;
        }
        private void CheckLogin()
        {
            Utilities.LogInfo("CheckLogin");
            Thread.Sleep(3 * 60 * 1000);
            if (!IsLogin)
            {
                utilities.AdminSendSms();
                IsLogin = true;
                Utilities.LogInfo("CheckLogin:Auto Login Error.");
            }
        }
    }
}
