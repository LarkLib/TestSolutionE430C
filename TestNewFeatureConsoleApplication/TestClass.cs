using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    public class TestClass : INewFeature, IComparable<TestClass>
    {
        public TestClass()
        {
            Id = UuidHelper.NewUuid();
        }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public int CompareTo(TestClass other)
        {
            return this.Id.CompareTo(other.Id);
        }

        public void ExecuteTest()
        {
            var list = new List<TestClass>() { new TestClass() { Name = "aa" }, new TestClass() { Name = "bb" } };
            list.Sort();
            list.Sort(new CompareTestClassByName());
        }
    }

    class CompareTestClassByName : IComparer<TestClass>
    {
        public int Compare(TestClass x, TestClass y)
        {
            return string.Compare(x.Name, y.Name);
        }
    }
    public static class Extensions
    {
        //I want to call the method in this fashion:  var instance = MyClass.ParseJson(text); 
        //It cannot be done; extension methods need to work on an instance of something.
        public static Guid NewSequentialGuid(this Guid guid)
        {
            return UuidHelper.NewUuid();
        }
    }
}
