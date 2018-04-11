using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Utilities;
namespace StockDataCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            var useTestData = args != null && args.Any() && args[0].ToLowerInvariant().Equals("usetestdata");
            var isDebug = args != null && args.Any() && args[0].ToLowerInvariant().Equals("debug") || useTestData;
            SoundPlayer player = new SoundPlayer(@"提示.wav");          //声音
            player.Play();
            var stockOperation = new StockOperation() { UseTestData = useTestData };
            Logger.Instance.LogAll("DataCollector start running...");
            Logger.Instance.LogTextMessage($"StockList:{Constants.StockList}");
            TimeSpan expendTime;
            while (!Constants.isDayClosed || isDebug)
            {
                try
                {
                    Logger.Instance.LogTextMessage("Processing realtime data-----------------------------------------");
                    DateTime beginDateTime = DateTime.Now;
                    double itervalTime = 0;
                    if (Constants.isTradeTime || isDebug)
                    {
                        stockOperation.ProcessStockInfo();
                        expendTime = DateTime.Now - beginDateTime;
                        //Logger.Instance.LogTextMessage($"{nameof(Main)} {expendTime.GetIntTotalMilliseconds()}ms");
                        Logger.Instance.LogConsoleMessage($"Expend {expendTime.GetIntTotalMilliseconds()}ms, Waitting {Constants.ItervalSecond}s");
                        expendTime = DateTime.Now - beginDateTime;
                        itervalTime = (Constants.ItervalSecond * 1000) - expendTime.GetIntTotalMilliseconds();
                        if (itervalTime < 0)
                        {
                            itervalTime = 0;
                            Logger.Instance.LogAll("This process is more then 3s, ...");
                        }

                    }
                    else
                    {
                        var totalMilliseconds = Constants.NexttTradeTimeTimeSpan.GetIntTotalMilliseconds();
                        Logger.Instance.LogAll($"Not trade time, watting {totalMilliseconds / 1000}s");
                        itervalTime = totalMilliseconds;
                    }
                    Logger.Instance.LogTextMessage("Processed realtime data");
                    Thread.Sleep((int)itervalTime);

                }
                catch (Exception e)
                {

                    Logger.Instance.LogAll($"error: {e.Message}");
                    break;
                }
            }
            Logger.Instance.LogAll("DataCollector stoped...");
        }
    }
}
