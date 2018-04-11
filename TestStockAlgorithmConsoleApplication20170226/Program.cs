using System;

namespace TestStockAlgorithmConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            TestStock test = new TestStock();
            test.ProcessTest();
            Console.Write("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
