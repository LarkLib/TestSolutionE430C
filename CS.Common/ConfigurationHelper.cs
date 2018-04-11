using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LarkLib.Common.Utilities
{
    public class ConfigurationHelper
    {
        #region helper
        public static IEnumerable<string> GetStringArrayConfigurationItem(string arrayString, char[] sepataor = null, bool isTrimItem = true, bool allowNull = false)
        {
            var configItem = GetStringConfigurationItem(arrayString, allowNull);
            var items = configItem.IsNullOrWhiteSpace() ? null : configItem.Split(sepataor.IsNullOrEmpty() ? new[] { ',' } : sepataor, StringSplitOptions.RemoveEmptyEntries);
            return items.IsNullOrEmpty() ? null : (from item in items select item.IsNullOrWhiteSpace() ? null : item.Trim());

        }
        public static IEnumerable<int> GetIntArrayConfigurationItem(string arrayString, char[] sepataor = null, bool isTrimItem = true, bool allowNull = false)
        {
            var items = GetStringArrayConfigurationItem(arrayString, sepataor, isTrimItem, allowNull);
            return items.IsNullOrEmpty() ? null : items.Select(int.Parse);
        }
        public static int GetIntegerConfigurationItem(string configItemName, bool allowNull = false, int defaultValue = default(int))
        {
            int configItem;
            if (!int.TryParse(GetStringConfigurationItem(configItemName, allowNull), out configItem))
            {
                configItem = defaultValue;
            }
            return configItem;
        }
        public static string GetStringConfigurationItem(string configItemName, bool allowNull = true)
        {
            var configItem = ConfigurationManager.AppSettings[configItemName];
            if (configItem.IsNullOrWhiteSpace() && !allowNull)
            {
                throw new ArgumentNullException(configItemName);
            }
            return configItem;
        }
        public static double GetDoubleConfigurationItem(string configItemName, bool allowNull = false, double defaultValue = default(double))
        {
            double configItem;
            if (!double.TryParse(GetStringConfigurationItem(configItemName, allowNull), out configItem))
            {
                configItem = defaultValue;
            }
            return configItem;
        }
        public static bool GetBooleanConfigurationItem(string configItemName, bool allowNull = false, bool defaultValue = default(bool))
        {
            bool configItem;
            if (!bool.TryParse(GetStringConfigurationItem(configItemName, allowNull), out configItem))
            {
                configItem = defaultValue;
            }
            return configItem;
        }
        public static string GetConnectionStringConfigurationItem(string configItemName, bool allowNull = true)
        {
            var configItem = ConfigurationManager.ConnectionStrings[configItemName];
            if ((configItem == null || configItem.ConnectionString.IsNullOrWhiteSpace()) && !allowNull)
            {
                throw new ArgumentNullException(configItemName);
            }
            return configItem.ConnectionString;
        }
        public static string GetCallerName([CallerMemberName] string name = null) => name;//return MethodBase.GetCurrentMethod().Name;
        #endregion helper
    }
}
