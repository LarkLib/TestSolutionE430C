using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QnyDownloader
{
    class Utilities
    {
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
    }
}
