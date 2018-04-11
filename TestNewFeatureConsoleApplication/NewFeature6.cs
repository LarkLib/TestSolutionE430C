using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.DayOfWeek;
using static System.Math;
using static System.Linq.Enumerable; // The type, not the namespace
using System.Runtime.CompilerServices;
using S = System.String;
using System.ComponentModel;

namespace TestNewFeatureConsoleApplication
{
    class NewFeature6 : INewFeature
    {
        public NewFeature6()
        {
            //As with the read-only fields, you can also initialize a read-only auto-property in the constructor
            MaxScope = 99;
        }

        //-------------------------------------------
        //Initializers for Auto-Properties
        public string FullName { get; set; } = "FullName";

        //Read-Only Auto-Properties
        public int MaxScope { get; } = 100;

        //-------------------------------------------
        //Expression Bodied Method-Like Members
        //For methods whose return type is void (or Task for asynchronous methods), the lambda arrow (=>) syntax still applies but the subsequent expression must be a statement (this is similar to what already happens with lambdas):
        public int DoubleAdd(int a, int b) => a + 2 * b;
        public void Print() => Console.WriteLine("return type is void");

        //-------------------------------------------
        //Expression Bodied Property-Like Function Members
        //Expression bodies can also be used to define the body of properties and an indexers
        public string AnimalName => "Dog";
        public int this[int id] => DoubleAdd(id, id);

        //-------------------------------------------
        //The ‘using static’ Directive 
        //using static System.DayOfWeek;
        //using static System.Math;
        void TestUsingStatic() => Console.WriteLine($"{nameof(TestUsingStatic)}: Friday - Monday={Friday - Monday},Sqrt(3 * 3 + 4 * 4)={Sqrt(3 * 3 + 4 * 4)}");

        void TestMethod()
        {
            //-------------------------------------------
            //Extension Methods
            //Extension methods are invoked like regular static methods, (See: https://msdn.microsoft.com/en-gb/magazine/dn879355.aspx) but they are called as if they were instance methods on the extended type. Instead of bringing these methods to the current scope, the static import functionality makes these methods available as extension methods without the need to import all extension methods in a namespace like before:
            //using static System.Linq.Enumerable; // The type, not the namespace
            var range = Range(5, 10);                // Ok: not extension
            //var odd = Where(range, i => i % 2 == 1); // Error, not in scope
            var even = range.Where(i => i % 2 == 0); // Ok
            Console.Write($"{nameof(TestMethod)}: range= ");
            foreach (var item in range) Console.Write($"{item},");
            Console.Write($" even= ");
            foreach (var item in even) Console.Write($"{item},");
            Console.WriteLine();

            //-------------------------------------------
            //Null-Conditional Operator
            string treeName = null;
            IList<string> list = new List<string>();
            list?.Add("firstLength");
            list?.Add(null);
            var length = treeName?.Length ?? 0;
            var price = list?[1] ?? "30";
            int? firstLength = list?[0]?.ToString()?.Count();

            //-------------------------------------------
            //String Interpolation, Formattable Strings
            Console.Write($@"String Interpolation:{nameof(TestMethod),10} \ {list?[0],-5}  ");
            IFormattable christmas = $"{new DateTime(2015, 12, 25):f}";
            Console.WriteLine(christmas);


            //-------------------------------------------
            //‘nameof’ Expressions
            //It is not allowed to use primitive types (int, long, char, bool, string, etc.) in nameof expressions because they are not expressions and the argument of nameof is an expression
            //Source Code vs.Metadata
            //The names used by the compiler are the source names and not the metadata names of the artifacts, and so the following code
            //using S = System.String;
            S s = null;
            Console.WriteLine($@"'nameof' Expressions:{nameof(TestMethod),10} {nameof(S)} {nameof(s)}");//TeseMethod S s

            //-------------------------------------------
            //Add Extension Methods in Collection Initializers 
            //Index Initializers
            var numbers = new Dictionary<int, string>
            {
                [7] = "sete",
                [9] = "nove",
                [13] = "treze"
            };
            Console.WriteLine($@"Add Extension Methods in Collection Initializers:{nameof(numbers),10} {numbers[7]}");

            //-------------------------------------------
            //Exception Filters
            try
            {
                var div0 = 0;
                var div = 5 / div0;
            }
            catch (Exception e) when (e.Message.Contains("zero"))
            {

                Console.WriteLine($@"Exception Filters(when(e.Message.Contains(""zero"")):{e.Message}");
            }

            //-------------------------------------------
            //‘await’ in ‘catch’ and ‘finally’ blocks
            //Resource res = null;
            //try
            //{
            //    res = await Resource.OpenAsync();

            //}
            //catch (ResourceException e)
            //{
            //    await Resource.LogAsync(res, e);
            //}
            //finally
            //{
            //    if (res != null) await res.CloseAsync();
            //}
            Console.WriteLine("‘await’ in ‘catch’ and ‘finally’ blocks");

            var movie = new Movie();
            movie.PropertyChanged += (sender, e) => { Console.WriteLine("PropertyChangedEventHandler"); };
            movie.Title = "good movid";
            movie.Rating = 9.99;

        }

        public class Movie : INotifyPropertyChanged
        {
            string oldTitle = default(string);
            public string Title
            {
                get
                {
                    return oldTitle;
                }
                set
                {
                    if (oldTitle != value)
                    {
                        oldTitle = value;
                        OnPropertyChanged();
                    }
                }
            }
            public double Rating { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName]string name = null)
            {
                PropertyChangedEventHandler handler = PropertyChanged;

                /* Null propagation syntax */
                handler?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public void ExecuteTest()
        {
            var s = MaxScope.ToString() ?? "aaa";
            Console.WriteLine($"{nameof(MaxScope)}={MaxScope}");//MaxScope = 99
            Console.WriteLine($"{nameof(DoubleAdd)}(80,10)={DoubleAdd(80, 10)}");
            TestUsingStatic();
            TestMethod();
        }

    }
}
