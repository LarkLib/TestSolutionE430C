using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            new NewFeature4().ExecuteTest();
            new NewFeature5().ExecuteTest();
            new NewFeature6().ExecuteTest();
            new TestAsync().ExecuteTest();
            new TestSerializable().ExecuteTest();
            new TestTimer().ExecuteTest();
            new TestStructLayout().ExecuteTest();
            new TestMultipleThreads().ExecuteTest();
            new TestInfoFormat().ExecuteTest();
            new TestLinq().ExecuteTest();
            new TestSomething().ExecuteTest();
            new TestGeneric().ExecuteTest();
            new TestExpression().ExecuteTest();
            new TestSomething().ExecuteTest();

            Console.Write("Press ENTER to exit... ");
            Console.ReadKey();
        }

    }
}
