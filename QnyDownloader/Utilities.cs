using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QnyDownloader
{
    class Utilities
    {
        private static string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
        private static string AccessKeyId = ConfigurationManager.AppSettings["AccessKeyId"];
        private static string AccessKeySecret = ConfigurationManager.AppSettings["AccessKeySecret"];
        internal static void UpdateWebBrowserControlRegistryKey(string appName, uint version = 11001)
        {
            var subKey = @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
            using (var registryKey = Registry.CurrentUser.OpenSubKey(subKey, true))
            {
                if (registryKey == null) return;
                dynamic value = registryKey.GetValue(appName);
                if (value == null) registryKey.SetValue(appName, version, RegistryValueKind.DWord);
            }
        }

        internal void GetSupplierPmsPoList(string bsid)
        {
            var sentMsmPoiList = new List<int>();
            var pmsPoSummary = GetSupplierPmsPoPage(bsid, 0, 20);
            var total = pmsPoSummary.total;
            var lastId = pmsPoSummary.pmsPoList[0].id;
            var maxId = GetMaxPmsPoIdFromDb();
            for (int i = 20; i < total && lastId > maxId; i += 20)
            {
                var pageList = pmsPoSummary.pmsPoList;
                foreach (var item in pageList)
                {
                    lastId = item.id;
                    if (lastId <= maxId)
                    {
                        break;
                    }
                    var poiId = int.Parse(item.poiId);
                    if (!sentMsmPoiList.Contains(poiId))
                    {
                        sentMsmPoiList.Add(poiId);
                    }
                    SavePmsPoToDb(item);
                    var detail = GetSupplierPmsPoDetail(bsid, item.poNo);
                    SavePmsPoDetailToDb(detail);
                    SaveSkuToDb(detail.poNo, detail.skuList);
                }
                pmsPoSummary = GetSupplierPmsPoPage(bsid, i, 20);
            }
            foreach (var item in sentMsmPoiList)
            {
                var phoneNumber = PhoneConfig[item];
                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    SendSms(phoneNumber);
                }
            }
        }

        private PmsPoSummary GetSupplierPmsPoPage(string bsid, int offset, int limite)
        {
            string url = "https://vss.baobaoaichi.cn/thrift/vss/SupplierPMSTService/querySupplierPmsPoList";
            string postDataString = $@"
{{
    ""bizAccountId"": ""NodeJs"",
	""pmsPoListQueryParam"": {{
		""poNo"": """",
		""status"": -1,
		""poiId"": -1,
		""preArrivalStartTime"": -1,
		""preArrivalEndTime"": -1,
		""paging"": {{
			""offset"": {offset},
			""limit"": {limite}
		}}
	}}
}}
";
            byte[] postData = Encoding.UTF8.GetBytes(postDataString);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.Host = "vss.baobaoaichi.cn";
            request.ContentLength = postData.Length;
            //request.Connection = "Keep-Alive";
            request.KeepAlive = true;
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Cookie", $"BSID={bsid}; msid=shqnymy");
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ServerCertificateValidationCallback +=
                    (sender, cert, chain, error) =>
                    {
                        return true;
                    };

            string responseContent = null;
            using (var writer = request.GetRequestStream())
            {

                writer.Write(postData, 0, postData.Length);
                writer.Close();
            }
            var response = (HttpWebResponse)request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                using (var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress))
                {

                    using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
                    {
                        responseContent = responseContent = reader.ReadToEnd();
                    }
                }
            }
            var result = JsonConvert.DeserializeObject<PmsPoRoot>(responseContent);

            return result.data;
        }
        private PmsPoDetailItem GetSupplierPmsPoDetail(string bsid, string poNo)
        {
            string url = "https://vss.baobaoaichi.cn/thrift/vss/SupplierPMSTService/querySupplierPmsPoDetail";
            string postDataString = $@"{{""pmsPoQueryParam"":{{""poNo"":""{poNo}""}},""bizAccountId"":""NodeJs""}}";
            byte[] postData = Encoding.UTF8.GetBytes(postDataString);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            request.Host = "vss.baobaoaichi.cn";
            request.ContentLength = postData.Length;
            //request.Connection = "Keep-Alive";
            request.KeepAlive = true;
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Cookie", $"BSID={bsid}; msid=shqnymy");
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ServerCertificateValidationCallback +=
                    (sender, cert, chain, error) =>
                    {
                        return true;
                    };

            string responseContent = null;
            using (var writer = request.GetRequestStream())
            {
                writer.Write(postData, 0, postData.Length);
                writer.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                using (var gzipStream = new GZipStream(responseStream, CompressionMode.Decompress))
                {

                    using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
                    {
                        responseContent = responseContent = reader.ReadToEnd();
                    }
                }
            }

            var result = JsonConvert.DeserializeObject<PmsPoDetailRoot>(responseContent);
            return result.data;
        }
        private void SavePmsPoToDb(PmsPoItem pmsPo)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $@"
IF NOT EXISTS(SELECT ID FROM PMSPO WHERE ID={pmsPo.id})
    INSERT INTO [dbo].[PmsPo]([id],[poNo],[supplierId],[poiId],[preArrivalTime],[arrivalTime],[status],[categoryName],[totalSku],[totalPrepoAmount],[expairTime],[skuPriceType],[creator],[ctime],[utime],[operator])
    VALUES('{pmsPo.id}','{pmsPo.poNo}','{pmsPo.supplierId}','{pmsPo.poiId}','{pmsPo.preArrivalTime}','{pmsPo.arrivalTime}','{pmsPo.status}','{pmsPo.categoryName}','{pmsPo.totalSku}','{pmsPo.totalPrepoAmount}','{pmsPo.expairTime}','{pmsPo.skuPriceType}','{pmsPo.creator}','{pmsPo.ctime}','{pmsPo.utime}','{pmsPo.@operator}')
";
                connection.Open();
                var reader = cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        private void SavePmsPoDetailToDb(PmsPoDetailItem pmsPoDetail)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $@"
IF NOT EXISTS(SELECT [poNo] FROM PMSPODETAIL WHERE [poNo]='{pmsPoDetail.poNo}')
    INSERT INTO [dbo].[PmsPoDetail]([supplierId],[supplierName],[poNo],[cTime],[poiId],[preArrivalTime],[arrivalTime],[creator],[poiType],[supplierCode],[supplierPrimaryContactPhone],[supplierPrimaryContactName],[poiName],[poiAddress],[poiServicePhone],[status],[poiContactName],[skuPriceType],[remark],[supplyType],[areaId],[areaName])
    VALUES('{pmsPoDetail.supplierId}','{pmsPoDetail.supplierName}','{pmsPoDetail.poNo}','{pmsPoDetail.cTime}','{pmsPoDetail.poiId}','{pmsPoDetail.preArrivalTime}','{pmsPoDetail.arrivalTime}','{pmsPoDetail.creator}','{pmsPoDetail.poiType}','{pmsPoDetail.supplierCode}','{pmsPoDetail.supplierPrimaryContactPhone}','{pmsPoDetail.supplierPrimaryContactName}','{pmsPoDetail.poiName}','{pmsPoDetail.poiAddress}','{pmsPoDetail.poiServicePhone}','{pmsPoDetail.status}','{pmsPoDetail.poiContactName}','{pmsPoDetail.skuPriceType}','{pmsPoDetail.remark}','{pmsPoDetail.supplyType}','{pmsPoDetail.areaId}','{pmsPoDetail.areaName}')
";
                connection.Open();
                var reader = cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        private void SaveSkuToDb(string poNo, IList<SkuItem> skuList)
        {
            var values = string.Empty;
            foreach (var sku in skuList)
            {
                values += $"('{sku.skuId}','{poNo}','{sku.categoryName}','{sku.spuName}','{sku.skuMallCode}','{sku.skuCode}','{sku.storageTemperatureLevel}','{sku.guaranteePeriod}','{sku.skuSpec}','{sku.skuCostPrice}','{sku.skuDictUnitName}','{sku.skuBoxQuantity}','{sku.prePoAmount}','{sku.poAmount}','{sku.productionDate}','{sku.sumPrePoPrice}','{sku.sumPoPrice}','{sku.unitId}','{sku.packageType}','{sku.categoryId}','{sku.tax}','{sku.guaranteePeriodType}','{sku.availableQuantity}','{sku.availablePoPrice}'),";
            }
            values = values.TrimEnd(new[] { ',' });

            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = $@"
IF NOT EXISTS(SELECT [poNo] FROM Sku WHERE [poNo]='{poNo}')
    INSERT INTO [dbo].[Sku]([skuId],[poNo],[categoryName],[spuName],[skuMallCode],[skuCode],[storageTemperatureLevel],[guaranteePeriod],[skuSpec],[skuCostPrice],[skuDictUnitName],[skuBoxQuantity],[prePoAmount],[poAmount],[productionDate],[sumPrePoPrice],[sumPoPrice],[unitId],[packageType],[categoryId],[tax],[guaranteePeriodType],[availableQuantity],[availablePoPrice])
    VALUES {values}
";
                connection.Open();
                var reader = cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        private long GetMaxPmsPoIdFromDb()
        {
            var id = -1L;
            using (var connection = new SqlConnection(ConnectionString))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT COALESCE(MAX(ID),-1) FROM PMSPO";
                connection.Open();
                id = (long)cmd.ExecuteScalar();
                connection.Close();
            }
            return id;
        }
        private static Dictionary<int, string> PhoneConfig { get; set; }

        private void GetPhoneConfigFromDb()
        {
            if (PhoneConfig == null)
            {
                PhoneConfig = new Dictionary<int, string>();
                using (var connection = new SqlConnection(ConnectionString))
                {
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "SELECT poiid,phonenumber FROM PoiConfig";
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var poiid = reader.GetInt32(0);
                            var phoneNumber = reader.GetString(1);
                            if (!PhoneConfig.ContainsKey(poiid))
                                PhoneConfig.Add(poiid, phoneNumber);
                        }
                    }
                    connection.Close();
                }
            }
        }
        internal void SendSms(string phoneNumber, string templateParam = "{\"name\":\"name\",\"remark\":\"remark\"}", string signName = "天风", string templateCode = "SMS_126865429")
        {
            String product = "Dysmsapi";//短信API产品名称
            String domain = "dysmsapi.aliyuncs.com";//短信API产品域名
            String accessKeyId = AccessKeyId;//你的accessKeyId
            String accessKeySecret = AccessKeySecret;//你的accessKeySecret

            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", accessKeyId, accessKeySecret);
            //IAcsClient client = new DefaultAcsClient(profile);
            // SingleSendSmsRequest request = new SingleSendSmsRequest();

            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            IAcsClient acsClient = new DefaultAcsClient(profile);
            SendSmsRequest request = new SendSmsRequest();
            //try
            //{
            //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为20个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
            request.PhoneNumbers = phoneNumber;
            //必填:短信签名-可在短信控制台中找到
            request.SignName = signName;
            //必填:短信模板-可在短信控制台中找到
            request.TemplateCode = templateCode;
            //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
            request.TemplateParam = templateParam;
            //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
            request.OutId = "21212121211";
            //请求失败这里会抛ClientException异常
            SendSmsResponse sendSmsResponse = acsClient.GetAcsResponse(request);

            //System.Console.WriteLine(sendSmsResponse.Message);


            //}
            //catch (ServerException e)
            //{
            //    System.Console.WriteLine("Hello World!");
            //}
            //catch (ClientException e)
            //{
            //    System.Console.WriteLine("Hello World!");
            //}
        }

    }
}
