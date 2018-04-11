using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    class TestLinq : INewFeature
    {
        public void ExecuteTest()
        {
            TestMethodLinqZip();
        }

        private void TestMethodLinqZip()
        {
            var numbers = new[] { 1, 2, 3, 4 };
            var chars = new[] { 'A', 'B', 'C', 'D' };
            var zip = numbers.Zip<int, char, string>(chars, (n, c) => $"{n}={c}, done");
            foreach (var z in zip)
            {
                Console.WriteLine(z);
            }
        }
    }
}
