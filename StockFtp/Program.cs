using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFtp
{
    class Program
    {
        static void Main(string[] args)
        {
            var operation = new StockFtpOperation();
            operation.ExecuteOperation();
            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
