using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    class TestEnumerable : INewFeature
    {
        public void ExecuteTest()
        {
            throw new NotImplementedException();
        }
    }

    class TestEnumerableClass : IEnumerable<TestEnumerableClass>
    {
        public IEnumerator<TestEnumerableClass> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
