using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//[assembly: CLSCompliant(true)]
namespace TestNewFeatureConsoleApplication
{
    class TestSomething : INewFeature
    {
        public void ExecuteTest()
        {
            //TestMehtodReadOnlyCollection();
            //TestMethodImmutableStack();
            //TestMethodCompressing();
            //TestMehtodString();
            //TestMehtodWeakReference();
            //TestMehtodEnum();
            //TestMehtodSortedList();
            //TestMehtodStringStringBuilderDiff();
            //TestMehtodGetObjectAddress();
            TestMehtodObjectIDGenerator();
        }
        private void TestMehtodReadOnlyCollection()
        {
            ;
            IList<string> readOnlyList = new List<string> { "dog", "cat", "forg" };
            ReadOnlyCollection<string> readOnlyCollection = new ReadOnlyCollection<string>(readOnlyList);
            IList<string> animals = new List<string> { "dog", "cat", "forg" }.AsReadOnly();
            readOnlyList.Add("lion");
            try
            {
                animals.Add("lion");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                animals[0] = "tiger";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            foreach (var item in readOnlyCollection)
            {
                Console.WriteLine(item);
            }
        }
        private void TestMethodImmutableStack()
        {
            IStack<string> stringStack = PersistentStack<string>.Empty;
            stringStack = stringStack.Push("A").Push("B");
            IStack<object> objectStack = stringStack.Push<object>(42);
            while (!objectStack.IsEmpty)
            {
                Console.WriteLine(objectStack.Peek());
                objectStack = objectStack.Pop();
            }

            AtomicStack<string> atomigStack = new AtomicStack<string>();
            atomigStack.Push("AA");
            atomigStack.Push("BB");
            Console.WriteLine(atomigStack.Pop("AA"));
            Console.WriteLine(atomigStack.Peek());
        }
        private void TestMethodCompressing()
        {
            var content = @"
                <summary>Holds a reference to an immutable class and updates it atomically.</summary>
                <typeparam name=""T"">An immutable class to reference.</typeparam>";
            var resultBits = CompressHelper.CompressString(content);
            Console.WriteLine($"content.Length={content.Length}; resultBits.Length={resultBits?.Length}");
        }
        private void TestMethodExpression()
        {
            var content = @"
                <summary>Holds a reference to an immutable class and updates it atomically.</summary>
                <typeparam name=""T"">An immutable class to reference.</typeparam>";
            var resultBits = CompressHelper.CompressString(content);
            Console.WriteLine($"content.Length={content.Length}; resultBits.Length={resultBits?.Length}");

        }
        private void TestMehtodString()
        {
            /*
           The first method, Intern, takes a String and looks it up in the internal hash table.If the
           string exists, a reference to the already existing String object is returned.If the application
           no longer holds a reference to the original String object, the garbage collector is able to
           free the memory of that string.
           Like the Intern method, the IsInterned method takes a String and looks it up in the internal
           hash table.If the string is in the hash table, IsInterned returns a reference to the interned
           string object. If the string isn’t in the hash table, however, IsInterned returns null; it
           doesn’t add the string to the hash table.
           The C# compiler uses the IsInterned method to allow switch/case statements to work
           efficiently on strings.
           */
            String s1 = "Hello";
            String s2 = "Hel";
            String s3 = s2 + "lo";
            String s4 = "Helllo4";
            Console.WriteLine(Object.ReferenceEquals(s1, s3));
            s3 = String.Intern(s3);
            Console.WriteLine(Object.ReferenceEquals(s1, s3));
            Console.WriteLine(s1.Equals(s3));
            var s5 = string.IsInterned("Helllo4");
            Console.WriteLine(Object.ReferenceEquals(s4, s5));
            s5 = string.IsInterned("Helllo6");
            Console.WriteLine(Object.ReferenceEquals(s4, s5));

            /*
            Clone Instance Returns a reference to thesame object (this). This is OK because String objectsare immutable. This methodimplements String’sICloneable interface.
            Copy Static Returns a new string that is a duplicate of the specified string. This method is rarely
            */
            s5 = s4.Clone() as string;
            s4 = "Helllo4+1";
            Console.WriteLine($"s4={s4} s5={s5}");

            s5 = string.Copy(s4);
            s4 = "Helllo4+2";
            Console.WriteLine($"s4={s4} s5={s5}");

            //todo:GetPreamble
            var s6 = Encoding.UTF8.GetPreamble();
            var s7 = Convert.ToBase64String(Encoding.UTF8.GetBytes(s4));
            var array7 = Convert.FromBase64String(s7);
            Console.WriteLine($"string:{Encoding.UTF8.GetString(array7)} base64:{s7} ");

            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in cultures)
            {
                Console.WriteLine($"{culture.Name:-18}{culture.DisplayName}{culture.EnglishName}");
            }
        }
        private void TestMehtodWeakReference()
        {
            /*
            When a root points to an object, the object can’t be collected because the application’s codecan reach the object.When a root points to an object,
            a strong reference to the object is saidto exist.However, the garbage collector also supports weak references.
            Weak referencesallow the garbage collector to collect the object but also allow the application to access theobject.It all comes down to timing.
            If only weak references to an object exist and the garbage collector runs, the object iscollected, and when the application later attempts to access the object, 
            the access will fail.On the other hand, to access a weakly referenced object, the application must obtain astrong reference to the object.
            If the application obtains this strong reference before thegarbage collector collects the object, 
            the garbage collector can’t collect the object because astrong reference to the object exists.
            */
            // Create a strong reference to a new Object.
            Object o = new Object();
            // Create a strong reference to a short WeakReference object.
            // The WeakReference object tracks the Object’s lifetime.
            WeakReference wr = new WeakReference(o);
            o = null; // Remove the strong reference to the object.
            o = wr.Target;
            if (o == null)
            {
                Console.WriteLine("A garbage collection occurred and Object’s memory was reclaimed");
            }
            else
            {
                Console.WriteLine("A garbage collection did not occur and I can successfully access the Object using o.");
            }
            while (true)
            {
                if (o != null)
                {
                    Console.WriteLine("A garbage collection occurred and Object’s memory was reclaimed");
                    break;
                }
                else
                {
                    Console.WriteLine("A garbage collection occurred and Object’s memory was reclaimed");
                }
            }
        }
        private void TestMehtodObjectIDGenerator()
        {

            //Generates IDs for objects.
            /*
            Remarks
            --------------------------------------------------------------------------------
            The ObjectIDGenerator keeps track of previously identified objects. When you ask for the ID of an object, the ObjectIDGenerator knows whether to return the existing ID, or generate and remember a new ID.
            The IDs are unique for the life of the ObjectIDGenerator instance.Generally, a ObjectIDGenerator life lasts as long as the Formatter that created it.Object IDs have meaning only within a given serialized stream, and are used for tracking which objects have references to others within the serialized object graph.
            Using a hash table, the ObjectIDGenerator retains which ID is assigned to which object.The object references, which uniquely identify each object, are addresses in the runtime garbage - collected heap.Object reference values can change during serialization, but the table is updated automatically so the information is correct.
            Object IDs are 64 - bit numbers.Allocation starts from one, so zero is never a valid object ID.A formatter can choose a zero value to represent an object reference whose value is null.
            */
            var obj1 = new object();
            var generator = new ObjectIDGenerator();
            var id = 0l;
            var firstTime = false;
            id = generator.GetId(obj1, out firstTime);
            Console.WriteLine(id);
            id = generator.GetId(obj1, out firstTime);
            Console.WriteLine(id);
            var obj2 = new object();
            id = generator.GetId(obj2, out firstTime);
            Console.WriteLine(id);
            obj2 = obj1;
            id = generator.GetId(obj2, out firstTime);
            Console.WriteLine(id);
        }
        private void TestMehtodStringStringBuilderDiff()
        {
            {
                //If you say I’m not convinced. let us check these behaviours using  C# code snippet. For that I am using C# class ObjectIDGenerator(in System.Runtime.Serialization Namespace). Actually it will return an unique integer value for instances that we created in our programs.With the help of this class we can check whether new instance is created or not for various operations on string and stringbuilder .Consider following program
                ObjectIDGenerator idGenerator = new ObjectIDGenerator();
                bool blStatus = new bool();
                //just ignore this blStatus Now.
                String str = "My first string was ";
                Console.WriteLine("str = {0}", str);
                Console.WriteLine("Instance Id : {0}", idGenerator.GetId(str, out blStatus));
                //here blStatus get True for new instace otherwise it will be false
                Console.WriteLine("this instance is new : {0}\n", blStatus);
                str += "Hello World";
                Console.WriteLine("str = {0}", str);
                Console.WriteLine("Instance Id : {0}", idGenerator.GetId(str, out blStatus));
                Console.WriteLine("this instance is new : {0}\n", blStatus);
                //Now str="My first string was Hello World"
                StringBuilder sbr = new StringBuilder("My Favourate Programming Font is ");
                Console.WriteLine("sbr = {0}", sbr);
                Console.WriteLine("Instance Id : {0}", idGenerator.GetId(sbr, out blStatus));
                Console.WriteLine("this instance is new : {0}\n", blStatus);
                sbr.Append("Inconsolata");
                Console.WriteLine("sbr = {0}", sbr);
                Console.WriteLine("Instance Id : {0}", idGenerator.GetId(sbr, out blStatus));
                Console.WriteLine("this instance is new : {0}\n", blStatus);
                //Now sbr="My Favourate Programming Font is Inconsolata"
            }
            {
                //Who Run Faster
                //This output clearly shows their performance difference.StringBuilder is about 70X faster than String in my laptop. it might be different in your case but generally speaking stringbuilder gives 10x times speed than string.
                Stopwatch Mytimer = new Stopwatch();
                string str = string.Empty;
                Mytimer.Start();
                for (int i = 0; i < 10000; i++)
                {
                    str += i.ToString();
                }
                Mytimer.Stop();
                Console.WriteLine("Time taken by string : {0}", Mytimer.Elapsed);
                StringBuilder sbr = new StringBuilder(string.Empty);
                //restart timer from zero
                Mytimer.Restart();
                for (int i = 0; i < 10000; i++)
                {
                    sbr.Append(i.ToString());
                }
                Mytimer.Stop();
                Console.WriteLine("Time taken by stringbuilder : {0}", Mytimer.Elapsed);
            }
            {
                //One more thing,If you do an operation on a string variable, Creation of new instance occurs only when it’s current value changes.try following code,
                ObjectIDGenerator idGenerator = new ObjectIDGenerator();
                bool blStatus = new bool();
                string str = "Fashion Fades,Style Remains Same";
                Console.WriteLine("initial state");
                Console.WriteLine("str = {0}", str);
                Console.WriteLine("instance id : {0}", idGenerator.GetId(str, out blStatus));
                Console.WriteLine("this is new instance : {0}", blStatus);
                //a series of operations that won't change value of str
                str += "";
                //try to replace character 'x' which is not present in str so no change
                str = str.Replace('x', 'Q');
                //trim removes whitespaces from both ends so no change
                str = str.Trim();
                str = str.Trim();
                Console.WriteLine("\nfinal state");
                Console.WriteLine("str = {0}", str);
                Console.WriteLine("instance id : {0}", idGenerator.GetId(str, out blStatus));
                Console.WriteLine("this is new instance : {0}", blStatus);
            }
        }
        private void TestMehtodGetObjectAddress()
        {
            //object obj = new object();
            //obj = "This is an object";
            //GCHandle gch = GCHandle.Alloc(obj, GCHandleType.Pinned);
            //IntPtr pObj = gch.AddrOfPinnedObject();
            //Console.WriteLine(pObj.ToString());
            object obj = new object();
            obj = "This is an object";
            Console.WriteLine($"obj address: {obj.GetIntPtr()}");
            obj = obj + " and add something";
            Console.WriteLine($"obj add somthing address: {obj.GetIntPtr()}");
            var objIntent = new object();
            objIntent = string.IsInterned("This is an object and add something");
            Console.WriteLine($"objobjIntent address: {obj.GetIntPtr(),20}");
            var objAssign = new object();
            objAssign = "This is an object and add something";
            Console.WriteLine($"objAssign address: {obj.GetIntPtr(),20}");
        }

        [Flags]
        enum MyEnum
        {
            a = 1 << 0,
            b = 1 << 1,
            c = 1 << 2,
            d = 1 << 3
        }
        private void TestMehtodEnum()
        {
            var names = Enum.GetNames(typeof(CultureTypes));
            var values = Enum.GetValues(typeof(CultureTypes));
            for (int i = 0; i < names.Length; i++)
            {
                Console.WriteLine($"{names[i]}={(int)values.GetValue(i)}");
            }
            /*In the .NET Framework version 2.0, 
            * the Array class implements the System.Collections.Generic.IList<T>, System.Collections.Generic.ICollection<T>, and System.Collections.Generic.IEnumerable<T> generic interfaces.
            * The implementations are provided to arrays at run time, and therefore are not visible to the documentation build tools. 
            * As a result, the generic interfaces do not appear in the declaration syntax for the Array class, 
            * and there are no reference topics for interface members that are accessible only by casting an array to the generic interface type (explicit interface implementations). */
            var enumerValues = (IEnumerable<string>)new[] { "aa", "bb" };
            var queryableNames = names.AsEnumerable<string>();
            var concatValues = queryableNames.Concat(enumerValues);
            var zipValues = names.Zip<string, int, string>((IEnumerable<int>)values, (n, v) => $"{n} {v}");

            int[] x = new int[] { 1, 2, 3 };
            int[] y = new int[] { 4, 5 };
            var z = new int[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length);
            foreach (var v in z)
            {
                Console.Write($"{v,3}");
            }
            Console.Write($"{100,13}{200,9}{500,12}");
            Console.Write($"{{90012,9:D2}}={90012,9:D2}");
        }

        private void TestMehtodSortedList()
        {
            var sortedList = new SortedList<string, ConsoleColor>();
            sortedList.Add("aaa", ConsoleColor.Black);
            sortedList.Add("zzz", ConsoleColor.Yellow);
            sortedList.Add("fff", ConsoleColor.Magenta);
            //sortedList.Add("fff", ConsoleColor.Green); //Additional information: An entry with the same key already exists.
            sortedList.Add("eee", ConsoleColor.White);
            sortedList.Add("ttt", ConsoleColor.DarkYellow);
            foreach (var item in sortedList)
            {
                Console.WriteLine(item);
            }

            var sortedSet = new SortedSet<string>();
            sortedSet.Add("Encoding.UTF32");
            sortedSet.Add("Encoding.UTF8");
            sortedSet.Add("Encoding.UTF8");
            sortedSet.Add("Encoding.GetEncoding(936)");
            sortedSet.Add("Encoding.ASCII");
            foreach (var item in sortedSet)
            {
                Console.WriteLine(item);
            }
            var sortedItemSet = new SortedSet<TestSortedItem>();
            sortedItemSet.Add(new TestSortedItem(100.1m));
            sortedItemSet.Add(new TestSortedItem(99.9m) { Name = "111" });
            sortedItemSet.Add(new TestSortedItem(99.9m) { Name = "222" });
            sortedItemSet.Add(new TestSortedItem(200.2m));
            foreach (var item in sortedItemSet)
            {
                Console.WriteLine(item);
            }
            var list = new List<string>();
            list.Add("Encoding.UTF32");
            list.Add("Encoding.UTF8");
            list.Add("Encoding.UTF8");
            list.Add("Encoding.ASCII");
            list.Sort();
            list.Add("Encoding.GetEncoding(936)");
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }

        }
        private class TestSortedItem : IComparable<TestSortedItem>
        {
            public TestSortedItem(decimal price)
            {
                this.Price = price;
            }
            internal Guid Id { get; set; } = Guid.NewGuid();
            internal string Name { get; set; } = $"TN{new Random().Next(10000, 99999)}";
            internal ConsoleColor Color { get; set; } = ConsoleColor.DarkGray;
            internal decimal Price { get; set; }

            public int CompareTo(TestSortedItem other)
            {
                if (other == null || this.Price < other.Price)
                {
                    return -1;
                }
                else if (this.Price > other.Price)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            public override string ToString()
            {
                return $"Name:{Name},Color:{Color},Price:{Price},Id:{Id.GetHashCode()}";
            }
        }
    }
    #region TestMethodImmutableStack
    //http://blog.slaks.net/2013-06-28/covariant-immutable-stack/
    public interface IStack<out T>
    {
        IStack<T> Pop();
        T Peek();
        bool IsEmpty { get; }
    }
    public static class PersistentStackExtensions
    {
        public static IStack<TNew> Push<TNew>(this IStack<TNew> stack, TNew element)
        {
            return new PersistentStack<TNew>.LinkNode(stack, element);
        }
    }
    public abstract class PersistentStack<T> : IStack<T>
    {
        public static readonly PersistentStack<T> Empty = new EmptyNode();

        private class EmptyNode : PersistentStack<T>
        {
            public override IStack<T> Pop()
            {
                throw new InvalidOperationException("Stack is empty");
            }
            public override T Peek()
            {
                throw new InvalidOperationException("Stack is empty");
            }
            public override bool IsEmpty { get { return true; } }
        }

        internal class LinkNode : PersistentStack<T>
        {
            readonly IStack<T> previous;
            readonly T element;

            public LinkNode(IStack<T> previous, T element)
            {
                this.previous = previous;
                this.element = element;
            }

            public override IStack<T> Pop()
            {
                return previous;
            }
            public override T Peek() { return element; }
            public override bool IsEmpty { get { return false; } }
        }
        public abstract IStack<T> Pop();
        public abstract T Peek();
        public abstract bool IsEmpty { get; }
    }
    /// <summary>Holds a reference to an immutable class and updates it atomically.</summary>
    /// <typeparam name="T">An immutable class to reference.</typeparam>
    class AtomicReference<T> where T : class
    {
        private volatile T value;

        public AtomicReference(T initialValue)
        {
            this.value = initialValue;
        }

        /// <summary>Gets the current value of this instance.</summary>
        public T Value { get { return value; } }

        /// <summary>Atomically updates the value of this instance.</summary>
        /// <param name="mutator">A pure function to compute a new value based on the current value of the instance.
        /// This function may be called more than once.</param>
        /// <returns>The previous value that was used to generate the resulting new value.</returns>
        public T Mutate(Func<T, T> mutator)
        {
            T baseVal = value;
            while (true)
            {
                T newVal = mutator(baseVal);
#pragma warning disable 420
                T currentVal = Interlocked.CompareExchange(ref value, newVal, baseVal);
#pragma warning restore 420

                if (currentVal == baseVal)
                    return baseVal;         // Success!
                else
                    baseVal = currentVal;   // Try again
            }
        }
    }
    class AtomicStack<T>
    {
        private readonly AtomicReference<IStack<T>> stack
            = new AtomicReference<IStack<T>>(PersistentStack<T>.Empty);

        public void Push(T item)
        {
            stack.Mutate(s => s.Push(item));
        }
        public T Pop(T item)
        {
            var oldStack = stack.Mutate(s => s.Pop());
            return oldStack.Peek();
        }
        public T Peek()
        {
            return stack.Value.Peek();
        }
    }

    #endregion TestMethodImmutableStack

}