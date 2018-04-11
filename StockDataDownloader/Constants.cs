using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static LarkLib.Common.Utilities.ConfigurationHelper;
using static LarkLib.Common.Utilities.Extensions;
using System.Xml.Linq;

namespace StockDataDownloader
{
    class Constants
    {
        public static string StockDetailAddress
        {
            get
            {
                return GetStringConfigurationItem(nameof(StockDetailAddress), false);
            }
        }
        public static string StockSummaryAddress
        {
            get
            {
                return GetStringConfigurationItem(nameof(StockSummaryAddress), false);
            }
        }
        public static int MaxThreadCount
        {
            get
            {
                return GetIntegerConfigurationItem(GetCallerName(), false);
            }
        }
        public static bool SaveSummaryContentToFile
        {
            get
            {
                return GetBooleanConfigurationItem(GetCallerName(), true, false);
            }
        }
        public static bool SaveDetailContentToFile
        {
            get
            {
                return GetBooleanConfigurationItem(GetCallerName(), true, false);
            }
        }

        private static int maxFailCount = default(int);
        public static int MaxFailCount
        {
            get
            {
                if (maxFailCount == default(int))
                {
                    maxFailCount = GetIntegerConfigurationItem(nameof(MaxFailCount), false);
                }
                return maxFailCount;
            }
        }
        public static int FailSleepMilliseconds
        {
            get
            {
                return GetIntegerConfigurationItem(nameof(FailSleepMilliseconds), false);
            }
        }
        public static string StockDetailDataPath
        {
            get
            {
                var dir = GetStringConfigurationItem(nameof(StockDetailDataPath), false);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }
        public static string StockSummaryDataPath
        {
            get
            {
                var dir = GetStringConfigurationItem(nameof(StockSummaryDataPath), false);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                return dir;
            }
        }
        public static string StockList
        {
            get
            {
                return ConfigurationManager.AppSettings["StockList"];
            }
        }
        public static IList<string> StockListArray
        {
            get
            {
                return GetStringArrayConfigurationItem("StockList").ToList<string>();
            }
        }

        public static string StockListXml
        {
            get
            {
                var ids = StockListArray;
                var idXelemnt = ids == null ? null : new XElement("IdList", from id in ids select new XElement("Item", new XAttribute("Id", id)));
                return idXelemnt == null ? null : idXelemnt.ToString();
            }
        }
        public static double ItervalSecond
        {
            get
            {
                double itervalSecond = default(double);

                return double.TryParse(ConfigurationManager.AppSettings["ItervalSecond"], out itervalSecond) ? itervalSecond : 3.0d;
            }
        }
    }
}
