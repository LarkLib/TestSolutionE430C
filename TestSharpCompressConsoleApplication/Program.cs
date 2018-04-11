using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSharpCompressConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            new SharpCompressOperation().Execute();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
