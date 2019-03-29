using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QnyWin
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //var data = new[] {"aaa","bbb","ccc" };
            //DataTable dt = new DataTable();//创建表
            //dt.Columns.Add("Id", typeof(Int32));//添加列
            //dt.Columns.Add("Name", typeof(String));
            //dt.Columns.Add("Age", typeof(Int32));
            //dt.Rows.Add(new object[] { 1, "张三", 20 });//添加行
            //dt.Rows.Add(new object[] { 2, "李四", 25 });
            //dt.Rows.Add(new object[] { 3, "王五", 30 });
            //RnDataGridView.DataSource = dt;
            //RnDataGridView.Columns[0].ReadOnly = true;
            test();
        }


        public async Task<bool> test(string userName = "admin", string password = "admin123")
        {
            var identityDbContext = new IdentityDbContext("Data Source=.;Initial Catalog=ShQny;Integrated Security=True");
            var usermanager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(identityDbContext));
            var user = await usermanager.FindAsync(userName, password);
            return user != null;
        }

        private async void test6()
        {
            var token = await GetBearerToken("http://localhost:16483/", "admin", "admin123");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var response = await client.GetAsync("http://localhost:10536/api/qny/GetRnItems?cDate=2018-12-13");
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync();
                    //var data = response.Content.ReadAsAsync<IEnumerable<string>>();
                    //return Json(data.Result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        internal async Task<string> GetBearerToken(string siteUrl, string Username, string Password)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(siteUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            HttpContent requestContent = new StringContent("grant_type=password&username=" + Username + "&password=" + Password, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage responseMessage = await client.PostAsync("Token", requestContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                //TokenResponseModel response = await responseMessage.Content.ReadAsAsync<TokenResponseModel>();
                var response = await responseMessage.Content.ReadAsStringAsync();
                return "";// response.AccessToken;
            }
            return "";
        }
        private async void test5()
        {
            var httpClientHandler = new HttpClientHandler()
            {
                Credentials = new NetworkCredential("admin", "admin123"),
            };

            var httpClient = new HttpClient(httpClientHandler);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var result = await httpClient.GetStringAsync("http://localhost:10536/api/qny/GetRnItems?cDate=2018-12-13");
        }

        private void test4()
        {
            var url = "http://localhost:10536/api/qny/GetRnItems?cDate=2018-12-13";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Get";
            request.KeepAlive = true;
            request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            request.UseDefaultCredentials = true;
            request.Credentials = new NetworkCredential("admin", "admin123");
            request.ContentType = "application/json";
            //request.ContentType = "application/x-www-form-urlencoded";

            //get cookie from Web API
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            foreach (Cookie cookieValue in response.Cookies)
            {
                Console.Write("Cookie: " + cookieValue.ToString());
                //store in your winform application
            }
            //get content
            string myResponse = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                myResponse = sr.ReadToEnd();
            }
        }

        private async void test3()
        {
            var url = "http://localhost:10536/api/qny/GetRnItems?cDate=2018-12-13";
            var client = new HttpClient();
            var username = "admin";
            var password = "admin123";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetStringAsync(url);
        }

        private void test2()
        {
            //var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());

            //var url = "http://goservices.cn/api/qny/GetRnItems?cDate=2018-12-13";
            var url = "http://localhost:10536/api/qny/GetRnItems?cDate=2018-12-13";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "Get";
            request.KeepAlive = false;
            //request.Credentials = new NetworkCredential("admin", "admin123");
            //request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("admin:admin123"));

            //request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("admin:admin123"));
            //request.ContentType = "application/json";
            //request.Accept = "application/json";
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            //request.Host = "vss.baobaoaichi.cn";
            //request.ContentLength = postData.Length;
            //request.Connection = "Keep-Alive";
            //request.KeepAlive = true;
            //request.Headers.Add("Cache-Control", "no-cache");
            //request.Headers.Add("Accept-Encoding", "gzip, deflate");
            //request.Headers.Add("Cookie", $"BSID={bsid}; msid=shqnymy");
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ServerCertificateValidationCallback +=
                    (sender, cert, chain, error) =>
                    {
                        return true;
                    };

            string responseContent = null;
            //using (var writer = request.GetRequestStream())
            //{

            //    writer.Write(postData, 0, postData.Length);
            //    writer.Close();
            //}
            var response = (HttpWebResponse)request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                //using (var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress))
                //{

                //using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
                using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    responseContent = responseContent = reader.ReadToEnd();
                }
                //}
            }
            var result = JsonConvert.DeserializeObject<string>(responseContent);

        }


    }
}
