using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockDataCollector
{
    sealed class Logger
    {
        public static readonly Logger Instance = new Logger();
        private string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
        private Logger()
        {

            lock (lockObject)
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
            }
        }
        private string LogFileFullName
        {
            get
            {
                return Path.Combine(filePath, "{0}.txt".FormatInvariantCulture(DateTime.Now.ToString("yyyy-MM-dd")));
            }
        }
        private object lockObject = new object();

        public void LogTextMessage(string message, bool withDateTime = true)
        {
            var currentTicks = DateTime.Now.Ticks;
            lock (lockObject)
            {
                File.AppendAllLines(LogFileFullName, new[] { withDateTime ? $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - {message}" : message });
            }
            var passedTicks = DateTime.Now.Ticks - currentTicks;
            var ms = passedTicks / (decimal)TimeSpan.TicksPerMillisecond;
            //if (Writer != null)
            //{
            //    lock (lockObject)
            //    {
            //        Writer.WriteLine("{0}", message);
            //        // keep the object alive until WriteLine method complete
            //        GC.KeepAlive(this);
            //        writer.Flush();
            //    }
            //}
        }
        public void LogConsoleMessage(string message, bool withDateTime = true)
        {
            Console.WriteLine(withDateTime ? $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - {message}" : message);
        }
        public void LogAll(string message, bool withDateTime = true)
        {
            LogConsoleMessage(message, withDateTime);
            LogTextMessage(message, withDateTime);
        }

    }
}
