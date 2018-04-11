using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Utilities.ConfigurationHelper;

namespace StockDataCollector
{
    class Constants
    {
        public static readonly string StockAddress = @"http://hq.sinajs.cn/list={0}";
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
        public static int BranchNumber
        {
            get
            {
                int branchNumber = default(int);
                return int.TryParse(ConfigurationManager.AppSettings["BranchNumber"], out branchNumber) ? branchNumber : 300;
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
