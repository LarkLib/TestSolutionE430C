using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataExportMinute
{
    class Program
    {
        static void Main(string[] args)
        {
            new ExportFileOperation().ExportData();
            Task.WaitAll();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
