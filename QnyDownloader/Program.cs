using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QnyDownloader
{
    class Program
    {
        private static CookieContainer cookies = new CookieContainer();
        static void Main(string[] args)
        {
            string url = "https://epassport.meituan.com/api/account/login?service=com.sankuai.mall.fe.vss&bg_source=14&part_type=0&loginContinue=https:%2F%2Fvss.baobaoaichi.cn%2Fauth%2Flogin%3Ftype%3DLOGIN&loginType=account";
            string jsonbody = "{\"login\":\"shqnymy\",\"part_key\":\"\",\"password\":\"xx6601\",\"error\":\"\",\"success\":\"\",\"isFetching\":false,\"loginType\":\"account\",\"verifyRequestCode\":\"\",\"verifyResponseCode\":\"\",\"captchaCode\":\"\",\"verifyType\":null,\"rohrToken\":\"eJy91FtP2zAUAOD/Yql9MqljJ2lSqZroGNCRFljDxAYIJZmbRs1NtlPaIv47joNSP/SB7WGSpX499jk6Obm8Ajb9A0avYJmFCRiZCCGLvEGwoQyMgGkgwwEQCC63bAu7toOQh4kNQazHCHYxgiBiP8/A6IEQCxIHPTWBH/L/g0kcBw4d9wl+kEhiS67mzFQeASshKj4aDGgVcl6VTBg5TUUdFkZc5oMwjsu6EIO6SEW6oVmZpMWXKHnmZc1iOjatflwWIi1qOlaFeuS0h8/l2nBuRGEpV5jGq9SICxkMa7GSP6pKj5yLXUV75My/vpjO+xldikkpRJn7abH+TDWeJkVdGSuRZ/0qZOK5qTdGfZYmq78sxWhcysG3tThlm1RenLx+g4fFug5TIw+zzFhSQ2bLuwLk9PKgmZ5pY8OGpoVk9B9NPM1DzY5mSzPWrNXBrmatDrY1E2WijDWbB5ue5rampdz2Yyu3/TjKWDM6GA01a+dR28NQuc11ldtcT9pTLTio4bAZ9loN+5HB6fzmLmg2O6FOzhHdH9Fj9D9pflJXnfxOvzrNjsTmnW47XXZafEhOL1TTO7GRC0+wbcHJXRBcz9unonkWD904FnS9Q6JoEpsCM/lJkvvNGydFv2+DHc9u9u52v+cu28UvLIju51df19WVHy3Mnct88dtf8JfLkgcze7qdbeitO4iDxTrZevyizE4nIv42dMfg7R1o91Jo\"}";
            //Post(url, "{\"login\":\"1\",\"part_key\":\"\",\"password\":\"1\",\"error\":\"\",\"success\":\"\",\"isFetching\":false,\"loginType\":\"account\",\"verifyRequestCode\":\"\",\"verifyResponseCode\":\"\",\"captchaCode\":\"\",\"verifyType\":null\"}");
            var bsid = PostMoths(url, jsonbody);
            PostMoths2(bsid);
        }
        private static bool PostWebRequest()
        {
            CookieContainer cc = new CookieContainer();
            string postData = "login=shqnymy&password=xx6601";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData); // 转化
            HttpWebRequest webRequest2 = (HttpWebRequest)WebRequest.Create(new Uri("https://vss.baobaoaichi.cn/login.html"));
            webRequest2.CookieContainer = cc;
            webRequest2.Method = "POST";
            webRequest2.ContentType = "application/json;charset=UTF-8";
            webRequest2.Accept = "application/json";
            //webRequest2.Connection = "keep-alive";
            webRequest2.KeepAlive = true;
            webRequest2.ContentLength = byteArray.Length;
            Stream newStream = webRequest2.GetRequestStream();
            // Send the data.
            newStream.Write(byteArray, 0, byteArray.Length); //写入参数
            newStream.Close();
            HttpWebResponse response2 = (HttpWebResponse)webRequest2.GetResponse();
            StreamReader sr2 = new StreamReader(response2.GetResponseStream(), Encoding.Default);
            string text2 = sr2.ReadToEnd();
            return false;
        }
        public static string Post(string Url, string jsonParas)
        {
            string strURL = Url;

            //创建一个HTTP请求  
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            //Post请求方式  
            request.Method = "POST";
            //内容类型
            request.ContentType = "application/json;charset=UTF-8";

            //设置参数，并进行URL编码 

            string paraUrlCoded = jsonParas;//System.Web.HttpUtility.UrlEncode(jsonParas);   

            byte[] payload;
            //将Json字符串转化为字节  
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的ContentLength   
            request.ContentLength = payload.Length;
            //发送请求，获得请求流 

            Stream writer;
            try
            {
                writer = request.GetRequestStream();//获取用于写入请求数据的Stream对象
            }
            catch (Exception)
            {
                writer = null;
                Console.Write("连接服务器失败!");
            }
            //将请求参数写入流
            writer.Write(payload, 0, payload.Length);
            writer.Close();//关闭请求流

            String strValue = "";//strValue为http响应所返回的字符流
            HttpWebResponse response;
            try
            {
                //获得响应流
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }

            Stream s = response.GetResponseStream();


            //Stream postData = Request.InputStream;
            StreamReader sRead = new StreamReader(s);
            string postContent = sRead.ReadToEnd();
            sRead.Close();


            return postContent;//返回Json数据
        }

        public static string PostMoths(string url, string param)
        {
            string strURL = url;
            HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.CookieContainer = new CookieContainer();
            request.ContentType = "application/json;charset=UTF-8";
            request.Accept = "application/json";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.Host = "epassport.meituan.com";
            //request.Connection = "Keep-Alive";
            request.KeepAlive = true;
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("DNT", "1");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            string paraUrlCoded = param;
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            dynamic loginReturn = (JObject)JsonConvert.DeserializeObject(strValue);
            JObject loginReturn2 = JObject.Parse(strValue);
            var bsid = loginReturn.bsid;
            cookies = request.CookieContainer;

            return bsid;
        }
        private static void PostMoths2(string bsid)
        {
            string strURL = "https://vss.baobaoaichi.cn/thrift/vss/SupplierPMSTService/querySupplierPmsPoList";
            string paraUrlCoded = "{\"bizAccountId\":\"NodeJs\",\"pmsPoListQueryParam\":{\"poNo\":\"\",\"status\":-1,\"poiId\":-1,\"preArrivalStartTime\":-1,\"preArrivalEndTime\":-1,\"paging\":{\"offset\":60,\"limit\":20}}}";
            //string cookiestr = $"BSID={bsid}; msid=shqnymy";
            string cookiestr = "BSID=hNOseozkOrkrysHe4LTuCd5YPaZZplHj7vb1f2F80ghY36rwBPu9nV6BIJZ8kjwqM0pHo8PLgvSbVHHYjGHP_g; msid=shqnymy";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.CookieContainer = new CookieContainer();
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.Host = "epassport.meituan.com";
            //request.Connection = "Keep-Alive";
            request.KeepAlive = true;
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("DNT", "1");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            //request2.CookieContainer = cookies;
            //request2.Headers.Add("Cookie", cookiestr);
            bool supportsCookieContainer = request.SupportsCookieContainer;
            var cookieCookieContainer = new CookieContainer();
            Cookie bsidCookie = new Cookie("BSID", "hNOseozkOrkrysHe4LTuCd5YPaZZplHj7vb1f2F80ghY36rwBPu9nV6BIJZ8kjwqM0pHo8PLgvSbVHHYjGHP_g");
            request.CookieContainer.Add(new Uri(strURL), new Cookie("BSID", "hNOseozkOrkrysHe4LTuCd5YPaZZplHj7vb1f2F80ghY36rwBPu9nV6BIJZ8kjwqM0pHo8PLgvSbVHHYjGHP_g"));
            request.CookieContainer.Add(new Uri(strURL), new Cookie("msid", "shqnymy"));
            //cookieCookieContainer.Add(new Uri(strURL), bsidCookie);
            request.ServerCertificateValidationCallback +=
                    (sender, cert, chain, error) =>
                    {
                        return true;
                    };
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            var writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            var response = (HttpWebResponse)request.GetResponse();
            Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            var Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return;
        }
    }
}
