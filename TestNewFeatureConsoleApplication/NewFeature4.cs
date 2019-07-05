using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TestNewFeatureConsoleApplication
{
    class NewFeature4 : INewFeature
    {
        public void ExecuteTest()
        {
            //TestMethodDynamic();
            TestMethodExpandoObject("Patrick Hines", address: "address", age: 20);
            TestMethodDynamicParser();
            TestMethodDynamicDictionary();
            TestMethodDynamicReadOnlyFile();
        }

        private void TestMethodDynamic()
        {
            TestDynamicClass staticInstanse = new TestDynamicClass();
            dynamic dynamicInstanse = staticInstanse;
            int num = 100000; //次数
            var expressionFun = staticInstanse.GetExpressionCallFunction("TestMethod");
            var emitFun = staticInstanse.GetEmitCallFunction("TestMethod");
            var method = staticInstanse.GetType().GetMethod("TestMethod");
            Console.WriteLine("开始对比测试(连续执行{0}次空方法):", num);
            Stopwatch watch = new Stopwatch();
            //测试直接调用
            watch.Start();
            for (int i = 0; i < num; i++)
            {
                staticInstanse.TestMethod();
            }
            watch.Stop();
            Console.WriteLine("直接调用耗时:{0} ms", watch.Elapsed.TotalMilliseconds);
            //测试反射调用
            watch.Reset();
            watch.Start();
            for (int i = 0; i < num; i++)
            {
                method.Invoke(staticInstanse, null);
            }
            watch.Stop();
            Console.WriteLine("使用反射调用耗时:{0} ms", watch.Elapsed.TotalMilliseconds);
            //测试包含首次调用的dynamic调用
            watch.Reset();
            watch.Start();
            for (int i = 0; i < num; i++)
            {
                dynamicInstanse.TestMethod();
            }
            watch.Stop();
            Console.WriteLine("使用包含首次调用的dynamic调用耗时:{0} ms", watch.Elapsed.TotalMilliseconds);
            //测试出去首次调用后的dynamic调用
            watch.Reset();
            watch.Start();
            for (int i = 0; i < num; i++)
            {
                dynamicInstanse.TestMethod();
            }
            watch.Stop();
            Console.WriteLine("使用去掉首次调用的dynamic调用耗时:{0} ms", watch.Elapsed.TotalMilliseconds);
            //测试expression调用
            watch.Reset();
            watch.Start();
            for (int i = 0; i < num; i++)
            {
                expressionFun(staticInstanse);
            }
            watch.Stop();
            Console.WriteLine("使用Expression调用耗时:{0} ms", watch.Elapsed.TotalMilliseconds);
            //测试emit调用
            watch.Reset();
            watch.Start();
            for (int i = 0; i < num; i++)
            {
                emitFun(staticInstanse);
            }
            watch.Stop();
            Console.WriteLine("使用Emit调用耗时:{0} ms", watch.Elapsed.TotalMilliseconds);

            /*
            开始对比测试(连续执行100000次空方法):
            直接调用耗时:0.6045 ms
            使用反射调用耗时:34.5875 ms
            使用包含首次调用的dynamic调用耗时:74.0094 ms
            使用去掉首次调用的dynamic调用耗时:4.3026 ms
            使用Expression调用耗时:2.6293 ms
            使用Emit调用耗时:2.7793 ms
             */
        }
        string TestMethodExpandoObject(string name, string address = null, int age = 18)
        {
            Contract.Ensures(name != null);
            dynamic contact = new ExpandoObject();
            contact.Name = name;
            contact.Phone = "206-555-0144";
            contact.Address = new ExpandoObject();
            contact.Address.Street = "123 Main St";
            contact.Address.City = "Mercer Island";
            contact.Address.State = "WA";
            contact.Address.Postal = "68402";

            XElement e = new XElement("Test", (IDictionary<string, object>)contact);
            Console.WriteLine(e.ToString());

            //var contentBuilder = new StringBuilder();
            //XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            //ns.Add("", "");
            //var serializer = new XmlSerializer(typeof(ExpandoObject));
            //using (var writer = new StringWriter(contentBuilder))
            //{
            //    serializer.Serialize(writer, contact, ns);
            //}
            //var xmlContent = contentBuilder.ToString();
            //Console.WriteLine(xmlContent);

            //XmlSerializer xs = new XmlSerializer(typeof(ExpandoObject));
            //Stream stream = new FileStream(@"c:\t.xml", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            //xs.Serialize(stream, dNodes);
            //stream.Close();


#if TestModule
            Console.WriteLine("TestModule");
#endif
            return null;
        }
        private void TestMethodLazyClass()
        {
            Lazy<TestDynamicClass> lazyCls = new Lazy<TestDynamicClass>();
            bool isv = lazyCls.IsValueCreated;
        }
        private void TestMethodDynamicParser()
        {
            dynamic xmlParser = new DynamicXmlParser(@".\TestDynamic.xml");
            dynamic csvParser = new DynamicCSVParser(@".\TestDynamic.csv");
            Console.WriteLine(xmlParser.customer.name);
            Console.WriteLine(csvParser.Product);
        }
        private static void TestMethodDynamicDictionary()
        {
            // Creating a dynamic dictionary.
            dynamic person = new DynamicDictionary();

            // Adding new dynamic properties. 
            // The TrySetMember method is called.
            person.FirstName = "Ellen";
            person.LastName = "Adams";

            // Getting values of the dynamic properties.
            // The TryGetMember method is called.
            // Note that property names are case-insensitive.
            Console.WriteLine(person.firstname + " " + person.lastname);

            // Getting the value of the Count property.
            // The TryGetMember is not called, 
            // because the property is defined in the class.
            Console.WriteLine("Number of dynamic properties:" + person.Count);

            // The following statement throws an exception at run time.
            // There is no "address" property,
            // so the TryGetMember method returns false and this causes a
            // RuntimeBinderException.
            // Console.WriteLine(person.address);
        }
        private void TestMethodDynamicReadOnlyFile()
        {
            dynamic rFile = new DynamicReadOnlyFile(@".\TestDynamic.txt");
            foreach (string line in rFile.Customer)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("----------------------------");
            foreach (string line in rFile.Customer(StringSearchOption.Contains, true))
            {
                Console.WriteLine(line);
            }
        }
    }
    #region TestDynamicClass
    class TestDynamicClass
    {
        internal string Name { get; set; }
        internal string Phone { get; set; }
        public void TestMethod()
        {
            //这是一个空方法
        }
    }
    static class TestExtensions
    {
        public static Action<TestDynamicClass> GetExpressionCallFunction(this TestDynamicClass testClass, string methodName)
        {
            Contract.Ensures(methodName != null);
            var parameter = Expression.Parameter(typeof(TestDynamicClass), "e");
            var method = testClass.GetType().GetMethod(methodName);
            var callExpression = Expression.Call(parameter, method);
            Expression<Action<TestDynamicClass>> lambdaExpression = Expression.Lambda<Action<TestDynamicClass>>(callExpression, parameter);
            return lambdaExpression.Compile();
        }

        public static Action<TestDynamicClass> GetEmitCallFunction(this TestDynamicClass testClass, string methodName)
        {
            Contract.Ensures(methodName != null);
            var callMethod = testClass.GetType().GetMethod(methodName);
            DynamicMethod method = new DynamicMethod("EmitCallable", null, new Type[] { typeof(TestDynamicClass) }, callMethod.DeclaringType.Module);
            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, callMethod);
            il.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof(Action<TestDynamicClass>)) as Action<TestDynamicClass>;
        }
    }
    #endregion TestDynamicClass
    class DynamicXmlParser : DynamicObject
    {
        XElement element;
        public DynamicXmlParser(string filename)
        {
            element = XElement.Load(filename);
        }
        private DynamicXmlParser(XElement el)
        {
            element = el;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (element == null)
            {
                result = null;
                return false;
            }
            XElement sub = element.Element(binder.Name);
            if (sub == null)
            {
                result = null;
                return false;
            }
            else
            {
                result = new DynamicXmlParser(sub);
                return true;
            }
        }
        public override string ToString()
        {
            if (element != null)
            {
                return element.Value;
            }
            else
            {
                return string.Empty;
            }
        }
        public string this[string attr]
        {
            get
            {
                if (element == null)
                {
                    return string.Empty;
                }
                return element.Attribute(attr).Value;
            }
        }
    }
    class DynamicCSVParser : DynamicObject
    {
        List<string> columns;
        StreamReader sr;
        string[] currentLine;
        public DynamicCSVParser(string file)
        {
            sr = new StreamReader(file);
            string columnLine = sr.ReadLine();
            columns = columnLine.Split(',').ToList<string>();
        }
        public bool Read()
        {
            if (sr.EndOfStream)
            {
                sr.Close();
                return false;
            }
            else
            {
                currentLine = sr.ReadLine().Split(',');
                return true;
            }
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            int index = columns.FindIndex(col => col == binder.Name);
            if (index == -1 || !Read())
            {
                result = null;
                return false;
            }

            result = this.currentLine[index];
            return true;
        }
    }
    public class DynamicDictionary : DynamicObject
    {
        // The inner dictionary.
        Dictionary<string, object> dictionary
            = new Dictionary<string, object>();

        // This property returns the number of elements
        // in the inner dictionary.
        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name.ToLower();

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return dictionary.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            dictionary[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }
    public enum StringSearchOption
    {
        StartsWith,
        Contains,
        EndsWith
    }
    class DynamicReadOnlyFile : DynamicObject
    {
        // Store the path to the file and the initial line count value.
        private string p_filePath;

        // Public constructor. Verify that file exists and store the path in 
        // the private variable.
        public DynamicReadOnlyFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File path does not exist.");
            }

            p_filePath = filePath;
        }
        public List<string> GetPropertyValue(string propertyName, StringSearchOption StringSearchOption = StringSearchOption.StartsWith, bool trimSpaces = true)
        {
            StreamReader sr = null;
            List<string> results = new List<string>();
            string line = "";
            string testLine = "";

            try
            {
                sr = new StreamReader(p_filePath);

                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();

                    // Perform a case-insensitive search by using the specified search options.
                    testLine = line.ToUpper();
                    if (trimSpaces) { testLine = testLine.Trim(); }

                    switch (StringSearchOption)
                    {
                        case StringSearchOption.StartsWith:
                            if (testLine.StartsWith(propertyName.ToUpper())) { results.Add(line); }
                            break;
                        case StringSearchOption.Contains:
                            if (testLine.Contains(propertyName.ToUpper())) { results.Add(line); }
                            break;
                        case StringSearchOption.EndsWith:
                            if (testLine.EndsWith(propertyName.ToUpper())) { results.Add(line); }
                            break;
                    }
                }
            }
            catch
            {
                // Trap any exception that occurs in reading the file and return null.
                results = null;
            }
            finally
            {
                if (sr != null) { sr.Close(); }
            }

            return results;
        }
        // Implement the TryGetMember method of the DynamicObject class for dynamic member calls.
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetPropertyValue(binder.Name);
            return result == null ? false : true;
        }
        // Implement the TryInvokeMember method of the DynamicObject class for 
        // dynamic member calls that have arguments.
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            StringSearchOption StringSearchOption = StringSearchOption.StartsWith;
            bool trimSpaces = true;

            try
            {
                if (args.Length > 0) { StringSearchOption = (StringSearchOption)args[0]; }
            }
            catch
            {
                throw new ArgumentException("StringSearchOption argument must be a StringSearchOption enum value.");
            }

            try
            {
                if (args.Length > 1) { trimSpaces = (bool)args[1]; }
            }
            catch
            {
                throw new ArgumentException("trimSpaces argument must be a Boolean value.");
            }

            result = GetPropertyValue(binder.Name, StringSearchOption, trimSpaces);

            return result == null ? false : true;
        }
    }
}
