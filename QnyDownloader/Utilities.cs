﻿using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QnyDownloader
{
    class Utilities
    {
        private static string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
        private static string AccessKeyId = ConfigurationManager.AppSettings["AccessKeyId"];
        private static string AccessKeySecret = ConfigurationManager.AppSettings["AccessKeySecret"];
        private static string SignName = ConfigurationManager.AppSettings["SignName"];
        private static string TemplateCode = ConfigurationManager.AppSettings["TemplateCode"];
        private static string AdminTemplateCode = ConfigurationManager.AppSettings["AdminTemplateCode"];
        private static string AdminPhone = ConfigurationManager.AppSettings["AdminPhone"];
        private static TimeSpan StarTime = TimeSpan.Parse(ConfigurationManager.AppSettings["StarTime"]);
        private static TimeSpan StopTime = TimeSpan.Parse(ConfigurationManager.AppSettings["StopTime"]);
        private static bool IgnoreTimeCheck = string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["IgnoreTimeCheck"]) ? false : bool.Parse(ConfigurationManager.AppSettings["IgnoreTimeCheck"]);
        private static string ReceivingNotePath
        {
            get
            {
                var path = ConfigurationManager.AppSettings["ReceivingNotePath"];
                var defaultPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\ReceivingNote";
                if (!Directory.Exists(defaultPath))
                {
                    Directory.CreateDirectory(defaultPath);
                }
                return string.IsNullOrWhiteSpace(path) ? defaultPath : path;
            }
        }
        private static readonly string LogFile = $@"{AppDomain.CurrentDomain.BaseDirectory}\Log.txt";
        private static Mutex QnyDownloaderMutex;
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
        internal static void CheckProcess()
        {
            //Guid ownGUID = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute))).Value);
            //Guid processGUID;
            //int currentProcessId = Process.GetCurrentProcess().Id;
            //int processId;

            //foreach (Process process in Process.GetProcesses())
            //{
            //    try
            //    {
            //        processGUID = new Guid(((GuidAttribute)Attribute.GetCustomAttribute(Assembly.LoadFile(process.MainModule.FileName), typeof(GuidAttribute))).Value);
            //        processId = process.Id;
            //        if (processGUID.Equals(ownGUID) && processId != currentProcessId)
            //        {
            //            process.Kill();
            //            process..WaitForExit();
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        //throw;
            //    }
            string mutexName = "QnyDownloader";
            //验证程序是否为 “单实例” 启动
            Boolean isAppRunning = false;
            QnyDownloaderMutex = new Mutex(
                true,
                mutexName,
                out isAppRunning);
            LogInfo($"isAppRunning={isAppRunning}");
            if (!isAppRunning)
            {
                var currentProcess = Process.GetCurrentProcess();
                LogInfo($"currentProcess.ProcessName={currentProcess.ProcessName}");
                // 结束原来的进程
                Process[] mainApps = Process.GetProcessesByName(currentProcess.ProcessName);
                LogInfo($"mainApps.Length={mainApps.Length}");
                if (mainApps.Length > 1)
                {

                    foreach (Process p in mainApps)
                    {
                        if (p.Id != currentProcess.Id)
                        {
                            p.Kill();
                            p.WaitForExit();
                        }
                    }
                }

                // 释放原来的互斥变量
                QnyDownloaderMutex.ReleaseMutex();

                // 重新申请本次启动的全局互斥变量
                QnyDownloaderMutex = new Mutex(
                    true,
                    mutexName,
                    out isAppRunning);
            }
        }
        internal static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        private string GetUnixTimeStampString(double unixTimeStamp, string format = "yyyy-MM-dd HH:mm:ss.fff")
        {
            return unixTimeStamp > 0 ? "'" + UnixTimeStampToDateTime(unixTimeStamp).ToString(format) + "'" : "null";
        }
        internal static bool CheckTime(bool ignoreChank = false)
        {
            TimeSpan currentSpan = DateTime.Now - DateTime.Now.Date;
            return (currentSpan >= StarTime && currentSpan < StopTime) || ignoreChank || IgnoreTimeCheck;
        }
        internal static void CheckTimeWithExit(bool ignoreChank = false, [CallerMemberName] string methodName = "")
        {
            var isInTime = CheckTime(ignoreChank);
            if (!isInTime)
            {
                Utilities.LogInfo($"{methodName}:CheckTime=false and exit.");
                Environment.Exit(0);
            }
            Utilities.LogInfo($"{methodName}:CheckTime=true");
        }

        internal void GetSupplierPmsPoList(string bsid)
        {
            LogInfo("GetSupplierPmsPoList runing");
            var sentMsmPoiList = new List<int>();
            var pmsPoSummary = GetSupplierPmsPoPage(bsid, 0, 20);
            var total = pmsPoSummary.total;
            var lastId = pmsPoSummary.pmsPoList[0].id;
            var maxId = GetMaxPmsPoIdFromDb();
            LogInfo($"total={total}, lastId ={lastId}, maxId={maxId}, lastId > maxId={lastId > maxId}");
            for (int i = 20; i < total && lastId > maxId; i += 20)
            {
                var pageList = pmsPoSummary.pmsPoList;
                foreach (var item in pageList)
                {
                    lastId = item.id;
                    if (lastId <= maxId)
                    {
                        LogInfo("No new Po");
                        break;
                    }
                    LogInfo($"poNo={item.poNo})");
                    var poiId = int.Parse(item.poiId);
                    SavePmsPoToDb(item);
                    LogInfo("SavePmsPoToDb");
                    var detail = GetSupplierPmsPoDetail(bsid, item.poNo);
                    SavePmsPoDetailToDb(detail);
                    LogInfo("SavePmsPoDetailToDb");
                    WriteReceivingNoteToPdf(detail, item.categoryName);
                    LogInfo("WriteReceivingNoteToPdf");
                    SaveSkuToDb(detail.poNo, detail.skuList);
                    LogInfo("SaveSkuToDb");
                    var phoneNumber = PhoneConfig[poiId];
                    LogInfo($"phoneNumber ={phoneNumber}");
                    if (!string.IsNullOrWhiteSpace(phoneNumber))
                    {
                        var templateParam = new SmsTemplateParam()
                        {
                            poNo = detail.poNo,
                            creator = detail.creator,
                            poiName = GetShopName(detail.poiName),
                            //preArrivalTime = UnixTimeStampToDateTime(detail.preArrivalTime).ToString("yyyy-MM-dd")
                        };
                        string smsResult = SendSms(phoneNumber, JsonConvert.SerializeObject(templateParam), TemplateCode, SignName);
                        LogInfo($"sent sms: phoneNumber ={phoneNumber}, poNo={item.poNo}");
                        LogInfo($"smsResult: {smsResult}");
                    }
                }
                pmsPoSummary = GetSupplierPmsPoPage(bsid, i, 20);
            }
            LogInfo("GetSupplierPmsPoList end");
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
    INSERT INTO [dbo].[PmsPo]([id],[poNo],[supplierId],[poiId],[preArrivalTime],[arrivalTime],[status],[categoryName],[totalSku],[totalPrepoAmount],[expairTime],[skuPriceType],[creator],[ctime],[utime],[operator],
                [preArrivalTime2],[arrivalTime2],[expairTime2],[ctime2],[utime2])
    VALUES('{pmsPo.id}','{pmsPo.poNo}','{pmsPo.supplierId}','{pmsPo.poiId}','{pmsPo.preArrivalTime}','{pmsPo.arrivalTime}','{pmsPo.status}','{pmsPo.categoryName}','{pmsPo.totalSku}','{pmsPo.totalPrepoAmount}','{pmsPo.expairTime}','{pmsPo.skuPriceType}','{pmsPo.creator}','{pmsPo.ctime}','{pmsPo.utime}','{pmsPo.@operator}',
            {GetUnixTimeStampString(pmsPo.preArrivalTime)},{GetUnixTimeStampString(pmsPo.arrivalTime)},{GetUnixTimeStampString(pmsPo.expairTime)},{GetUnixTimeStampString(pmsPo.ctime)},{GetUnixTimeStampString(pmsPo.utime)})
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
    INSERT INTO [dbo].[PmsPoDetail]([supplierId],[supplierName],[poNo],[cTime],[poiId],[preArrivalTime],[arrivalTime],[creator],[poiType],[supplierCode],[supplierPrimaryContactPhone],[supplierPrimaryContactName],[poiName],[poiAddress],[poiServicePhone],[status],[poiContactName],[skuPriceType],[remark],[supplyType],[areaId],[areaName],
                      [cTime2],[preArrivalTime2],[arrivalTime2])
    VALUES('{pmsPoDetail.supplierId}','{pmsPoDetail.supplierName}','{pmsPoDetail.poNo}','{pmsPoDetail.cTime}','{pmsPoDetail.poiId}','{pmsPoDetail.preArrivalTime}','{pmsPoDetail.arrivalTime}','{pmsPoDetail.creator}','{pmsPoDetail.poiType}','{pmsPoDetail.supplierCode}','{pmsPoDetail.supplierPrimaryContactPhone}','{pmsPoDetail.supplierPrimaryContactName}','{pmsPoDetail.poiName}','{pmsPoDetail.poiAddress}','{pmsPoDetail.poiServicePhone}','{pmsPoDetail.status}','{pmsPoDetail.poiContactName}','{pmsPoDetail.skuPriceType}','{pmsPoDetail.remark}','{pmsPoDetail.supplyType}','{pmsPoDetail.areaId}','{pmsPoDetail.areaName}',
            {GetUnixTimeStampString(pmsPoDetail.cTime)},{GetUnixTimeStampString(pmsPoDetail.preArrivalTime)},{GetUnixTimeStampString(pmsPoDetail.arrivalTime)})
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
        private Dictionary<int, string> PhoneConfig { get { return GetPhoneConfigFromDb(); } }
        private static Dictionary<int, string> phoneConfig = null;
        private Dictionary<int, string> GetPhoneConfigFromDb()
        {
            if (phoneConfig == null)
            {
                phoneConfig = new Dictionary<int, string>();
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
                            var phoneNumber = reader.IsDBNull(1) ? null : reader.GetString(1);
                            if (!phoneConfig.ContainsKey(poiid))
                                phoneConfig.Add(poiid, phoneNumber);
                        }
                    }
                    connection.Close();
                }
            }
            return phoneConfig;
        }
        internal string SendSms(string phoneNumber, string templateParam, string templateCode, string signName)
        {
            LogInfo("Send a Sms message!");
            //您有新采购单(${poNo})，下单人mis账号(${creator})，收货方(${poiName})，约定到货(${preArrivalTime})，请于小象系统中确认。
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
            var result = string.Empty;
            try
            {
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

                result = $"RequestId={sendSmsResponse.RequestId}, Code={sendSmsResponse.Code}, BizId={sendSmsResponse.BizId}, Message={sendSmsResponse.Message}";
            }
            catch (ServerException e)
            {
                result = e.Message;
            }
            catch (ClientException e)
            {
                result = e.Message;
            }
            return result;
        }
        internal string AdminSendSms()
        {
            return SendSms(AdminPhone, AdminTemplateCode, null, SignName);
        }
        internal static void LogInfo(string content)
        {
            File.AppendAllText(LogFile, $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")},{content}{Environment.NewLine}");
        }
        internal void WriteReceivingNoteToPdf(PmsPoDetailItem pmsPoDetail, string categoryName)
        {
            Font simHeiFont = new Font("SimHei");

            var document = new Document();
            document.DefaultPageSetup.TopMargin = "1cm";
            document.DefaultPageSetup.BottomMargin = "1cm";
            document.AddSection();
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
            //poParagraph.AddFormattedText("CG201812020228", TextFormat.Bold);
            poParagraph.AddFormattedText(pmsPoDetail.poNo, TextFormat.Bold);
            poParagraph.AddFormattedText("    采购单时间：");
            //poParagraph.AddFormattedText("2018-12-02 20:13)", TextFormat.Bold);
            poParagraph.AddFormattedText(UnixTimeStampToDateTime(pmsPoDetail.cTime).ToString("yyyy-MM-dd"), TextFormat.Bold);

            var infoParagraph = document.LastSection.AddParagraph();
            infoParagraph.Format.Font = simHeiFont.Clone();
            //infoParagraph.Format.Font.Size = "1.0cm";
            infoParagraph.Format.Alignment = ParagraphAlignment.Left;
            infoParagraph.AddFormattedText("门店名称：");
            //infoParagraph.AddFormattedText("小象生鲜（无锡广益店）", TextFormat.Bold);
            infoParagraph.AddFormattedText(pmsPoDetail.poiName, TextFormat.Bold);
            infoParagraph.AddFormattedText("    门店联系人：");
            //infoParagraph.AddFormattedText("付明亮", TextFormat.Bold);
            infoParagraph.AddFormattedText(pmsPoDetail.poiContactName, TextFormat.Bold);
            infoParagraph.AddFormattedText("    门店联系电话：");
            //infoParagraph.AddFormattedText("15961756029", TextFormat.Bold);
            infoParagraph.AddFormattedText(pmsPoDetail.poiServicePhone, TextFormat.Bold);

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

            foreach (var sku in pmsPoDetail.skuList)
            {
                row = table.AddRow();
                row.Format.Alignment = ParagraphAlignment.Left;
                //row.Shading.Color = Colors.PaleGoldenrod;
                //row.Cells[0].AddParagraph().AddFormattedText("17412");
                row.Cells[0].AddParagraph().AddFormattedText(sku.skuId.ToString());
                //row.Cells[1].AddParagraph().AddFormattedText("鲜活 罗氏沼虾40-60头/500g", simHeiFont.Clone());
                row.Cells[1].AddParagraph().AddFormattedText(sku.spuName, simHeiFont.Clone());
                //row.Cells[2].AddParagraph().AddFormattedText("40-60头", simHeiFont.Clone());
                row.Cells[2].AddParagraph().AddFormattedText(sku.skuSpec, simHeiFont.Clone());
                //row.Cells[3].AddParagraph().AddFormattedText("kg", simHeiFont.Clone());
                row.Cells[3].AddParagraph().AddFormattedText(sku.skuDictUnitName, simHeiFont.Clone());
                //row.Cells[4].AddParagraph().AddFormattedText("100", simHeiFont.Clone());
                row.Cells[4].AddParagraph().AddFormattedText(sku.availableQuantity, simHeiFont.Clone());
            }

            document.LastSection.Add(table);


            var renderer = new PdfDocumentRenderer(true);
            renderer.Document = document;
            renderer.RenderDocument();

            var fileName = Path.Combine(ReceivingNotePath, $"{pmsPoDetail.poNo}_{GetShopName(pmsPoDetail.poiName)}_{categoryName}.pdf");
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            renderer.PdfDocument.Save(fileName);
        }
        private string GetShopName(string shopName)
        {
            var index = shopName.IndexOf('（');
            index = index == -1 ? shopName.IndexOf('(') : index;
            index = index == -1 ? 0 : index + 1;
            return shopName.Substring(index, shopName.Length - index - 1);

        }
    }
}
