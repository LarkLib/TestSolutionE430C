using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 100;
            TestStock test = new TestStock();
            test.ProcessTest();
            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
