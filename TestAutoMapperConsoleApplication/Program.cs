using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAutoMapperConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            new TestAutoMapper().Execute();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
