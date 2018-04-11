using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static Common.Utilities.ConfigurationHelper;
using static Common.Utilities.Extensions;

namespace StockDataDownloaderConsoleApplication
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
        public static int MaxFailCount
        {
            get
            {
                return GetIntegerConfigurationItem(nameof(MaxFailCount), false);
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
        public static double ItervalSecond
        {
            get
            {
                double itervalSecond = default(double);

                return double.TryParse(ConfigurationManager.AppSettings["ItervalSecond"], out itervalSecond) ? itervalSecond : 3.0d;
            }
        }
        public static string StockConnectionString
        {
            get
            {

                return GetConnectionStringConfigurationItem("StockConnectionString", false);
            }
        }

        #region TradeTime
        private static readonly string amStartTime = GetStringConfigurationItem("AMStartTime", true);
        private static TimeSpan AMStartTime
        {
            get
            {
                TimeSpan timeSpan;
                TimeSpan.TryParse(amStartTime ?? "9:15", out timeSpan);
                return timeSpan;
            }
        }
        private static TimeSpan AMEndTime
        {
            get
            {
                TimeSpan timeSpan;
                TimeSpan.TryParse(GetStringConfigurationItem("AMEndTime", true) ?? "11:30", out timeSpan);
                return timeSpan;
            }
        }
        private static TimeSpan PMStartTime
        {
            get
            {
                TimeSpan timeSpan;
                TimeSpan.TryParse(GetStringConfigurationItem("PMStartTime", true) ?? "13:00", out timeSpan);
                return timeSpan;
            }
        }
        private static TimeSpan PMEndTime
        {
            get
            {
                TimeSpan timeSpan;
                TimeSpan.TryParse(GetStringConfigurationItem("PMEndTime", true) ?? "15:00", out timeSpan);
                return timeSpan;
            }
        }
        public static bool isTradeTime
        {
            get
            {
                TimeSpan currentTime = DateTime.Now.TimeOfDay;

                return (currentTime >= AMStartTime && currentTime < AMEndTime)
                    || (currentTime >= PMStartTime && currentTime < PMEndTime);
            }
        }
        public static bool isDayClosed
        {
            get
            {
                return DateTime.Now.TimeOfDay > PMEndTime;
            }
        }

        public static TimeSpan NexttTradeTimeTimeSpan
        {
            get
            {
                return DateTime.Now.TimeOfDay < AMStartTime ? AMStartTime - DateTime.Now.TimeOfDay : PMStartTime - DateTime.Now.TimeOfDay;
            }
        }
        #endregion TradeTime

    }
}
