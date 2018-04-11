using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDataDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.BufferHeight = 30;
            StockDataOperation.Instance.Execute();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
