using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebBrowserControlDialogs;

namespace QnyDownloader
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            var appName = AppDomain.CurrentDomain.FriendlyName;
            Utilities.UpdateWebBrowserControlRegistryKey(appName);
            appName = Path.GetFileName(Application.ExecutablePath);
            Utilities.UpdateWebBrowserControlRegistryKey(appName);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Tell the WidowsInterop to Hook messages
            WindowsInterop.Hook();
            Application.Run(new WebBrowserForm());
            // Tell the WidowsInterop to Unhook
            WindowsInterop.Unhook();
        }
    }
}
