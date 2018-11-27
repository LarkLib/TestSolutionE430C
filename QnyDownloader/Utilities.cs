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
            var pmsPoSummary = GetSupplierPmsPoPage(bsid, 200, 100);
            for (int i = 0; i < pmsPoSummary.total; i += 100)
            {
                var pageList = pmsPoSummary.pmsPoList;
                foreach (var item in pageList)
                {
                    SavePmsPoToDb(item);
                    var detail = GetSupplierPmsPoDetail(bsid, item.poNo);
                    SavePmsPoDetailToDb(detail);
                    System.Threading.Thread.Sleep(5000);
                    SaveSkuToDb(detail.poNo, detail.skuList);
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
    }
}
