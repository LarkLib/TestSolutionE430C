using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    class TestGeneric : INewFeature
    {
        public void ExecuteTest()
        {
            TestMehtodGeneric();
            TestMehtodGenericList();
            GenericTypeWithReflectionEmit.TestMehtodGenericTypeWithReflectionEmit();
            GenericMethodBuilder.TestMethodGenericMethodBuilder();
            TestGenericInfo.TestMehtodGenericInfo();
            ArraySegmentExample.TestMethodArraySegment1();
            ArraySegmentExample.TestMethodArraySegment2();
            ArraySegmentExample.TestMethodArraySegment3();
            TestCovarianceAndContravariance.TestMethodCovarianceAndContravariance();
        }
        private void TestMehtodGeneric()
        {
            var bmw = new BMW<Whell<ConsoleColor>, ConsoleColor, string, int>();
            var whell = bmw.CreateInstance();
            whell.Name = "BMW 350";
            whell.Color = ConsoleColor.Green;
            Console.WriteLine(whell.ToString());
            Console.ForegroundColor = whell.Color;
            //var doubleWhell = whell + whell;
            var array = new[] { 100, 69, 88, 99, 20, 1000 };
            bmw.BubbleSort<int>(array);
            Console.WriteLine($"T type is {bmw.GetTType()}");
            //System.Type
            var bmwType = bmw.GetType();
            var genericTypeDefinition = bmwType.GetGenericTypeDefinition();
            var genericArguments = bmwType.GetGenericArguments();
            //var genericParameterPosition = genericArguments?[0].GenericParameterPosition;
            //var generiParameterConstraints = bmwType.GetGenericParameterConstraints();

            //System.Reflection.MemberInfo 
            var memberInfos = bmwType.GetMembers();
            //var genericParameters = from member in memberInfos where member.IsGenericMethod() select member;
        }
        private void TestMehtodGenericList()
        {
            // int is the type argument
            GenericList<int> list = new GenericList<int>();

            for (int x = 0; x < 10; x++)
            {
                list.AddHead(x);
            }
            foreach (int i in list)
            {
                Console.Write("{0}", i);
            }
            Console.WriteLine("\nDone");
        }

    }
    #region Test Generic
    class BMW<T, G, M, P> : ICar<T>
        where T : Whell<G>, new()//new约束只能是无参数的，所以也要求相应的类必须有一个无参构造函数
        where M : class
        where P : struct
        where G : struct
    {
        public string Name { get; set; }

        public T Type { get; set; }

        public string Vendor { get; set; }
        internal string GetTType()
        {
            return typeof(T)?.Name;
        }
        internal void BubbleSort<X>(X[] array) where X : IComparable<X>
        {
            int length = array.Length;
            for (int i = 0; i <= length - 2; i++)
            {
                for (int j = length - 1; j >= 1; j--)
                {
                    if (array[j].CompareTo(array[j - 1]) < 0)
                    {
                        X temp = array[j];
                        array[j] = array[j - 1];
                        array[j - 1] = temp;
                    }
                }
            }
            foreach (var item in array)
            {
                Console.Write($"{item}  ");
            }
            Console.Write(Environment.NewLine);
        }
        internal T CreateInstance()
        {
            return Activator.CreateInstance<T>();
        }
    }
    class Whell<T> : IWhell<T>
        /*
        where T: struct The type argument must be a value type. Any value type except Nullable can be specified. See Using Nullable Types for more information. 
        where T : class The type argument must be a reference type; this applies also to any class, interface, delegate, or array type. 
        where T : new() The type argument must have a public parameterless constructor. When used together with other constraints, the new() constraint must be specified last. 
        where T : <base class name> The type argument must be or derive from the specified base class. 
        where T : <interface name> The type argument must be or implement the specified interface. Multiple interface constraints can be specified. The constraining interface can also be generic. 
        where T : U The type argument supplied for T must be or derive from the argument supplied for U. 
        */
        where T : struct
    {
        public Whell()
        {

        }
        public T Color { get; set; }

        public string Name { get; set; }
        public override string ToString()
        {
            return $"Name:{Name}, color:{Color}";
        }
        //public static T operator +(Whell<T> a, Whell<T> b)
        //{
        //    return a + b;
        //}
    }
    interface ICar<T>
    {
        string Name { get; }
        string Vendor { get; }
        T Type { get; }

    }
    interface IWhell<G>
    {
        string Name { get; }
        G Color { get; }
    }
    // type parameter T in angle brackets
    public class GenericList<T>
    {
        // The nested class is also generic on T.
        private class Node
        {
            // T used in non-generic constructor.
            public Node(T t)
            {
                next = null;
                data = t;
            }

            private Node next;
            public Node Next
            {
                get { return next; }
                set { next = value; }
            }

            // T as private member data type.
            private T data;

            // T as return type of property.
            public T Data
            {
                get { return data; }
                set { data = value; }
            }
        }

        private Node head;

        // constructor
        public GenericList()
        {
            head = null;
        }

        // T as method parameter type:
        public void AddHead(T t)
        {
            Node n = new Node(t);
            n.Next = head;
            head = n;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node current = head;

            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }
        // The following method returns the data value stored in the last node in
        // the list. If the list is empty, the default value for type T is
        // returned.
        public T GetLast()
        {
            // The value of temp is returned as the value of the method. 
            // The following declaration initializes temp to the appropriate 
            // default value for type T. The default value is returned if the 
            // list is empty.
            T temp = default(T);

            Node current = head;
            while (current != null)
            {
                temp = current.Data;
                current = current.Next;
            }
            return temp;
        }

    }
    #endregion Test Generic
    #region How to: Define a Generic Type with Reflection Emit
    //https://msdn.microsoft.com/en-us/library/4xxf1410.aspx
    // Define a trivial base class and two trivial interfaces 
    // to use when demonstrating constraints.
    //
    public class ExampleBase { }

    public interface IExampleA { }

    public interface IExampleB { }

    // Define a trivial type that can substitute for type parameter 
    // TSecond.
    //
    public class ExampleDerived : ExampleBase, IExampleA, IExampleB { }

    class GenericTypeWithReflectionEmit
    {
        internal static void TestMehtodGenericTypeWithReflectionEmit()
        {
            // Define a dynamic assembly to contain the sample type. The
            // assembly will not be run, but only saved to disk, so
            // AssemblyBuilderAccess.Save is specified.
            //
            AppDomain myDomain = AppDomain.CurrentDomain;
            AssemblyName myAsmName = new AssemblyName("GenericEmitExample1");
            AssemblyBuilder myAssembly =
                myDomain.DefineDynamicAssembly(myAsmName,
                    AssemblyBuilderAccess.RunAndSave);

            // An assembly is made up of executable modules. For a single-
            // module assembly, the module name and file name are the same 
            // as the assembly name. 
            //
            ModuleBuilder myModule =
                myAssembly.DefineDynamicModule(myAsmName.Name,
                   myAsmName.Name + ".dll");

            // Get type objects for the base class trivial interfaces to
            // be used as constraints.
            //
            Type baseType = typeof(ExampleBase);
            Type interfaceA = typeof(IExampleA);
            Type interfaceB = typeof(IExampleB);

            // Define the sample type.
            //
            TypeBuilder myType =
                myModule.DefineType("Sample", TypeAttributes.Public);

            Console.WriteLine("Type 'Sample' is generic: {0}",
                myType.IsGenericType);

            // Define type parameters for the type. Until you do this, 
            // the type is not generic, as the preceding and following 
            // WriteLine statements show. The type parameter names are
            // specified as an array of strings. To make the code
            // easier to read, each GenericTypeParameterBuilder is placed
            // in a variable with the same name as the type parameter.
            // 
            string[] typeParamNames = { "TFirst", "TSecond" };
            GenericTypeParameterBuilder[] typeParams =
                myType.DefineGenericParameters(typeParamNames);

            GenericTypeParameterBuilder TFirst = typeParams[0];
            GenericTypeParameterBuilder TSecond = typeParams[1];

            Console.WriteLine("Type 'Sample' is generic: {0}",
                myType.IsGenericType);

            // Apply constraints to the type parameters.
            //
            // A type that is substituted for the first parameter, TFirst,
            // must be a reference type and must have a parameterless
            // constructor.
            TFirst.SetGenericParameterAttributes(
                GenericParameterAttributes.DefaultConstructorConstraint |
                GenericParameterAttributes.ReferenceTypeConstraint);

            // A type that is substituted for the second type
            // parameter must implement IExampleA and IExampleB, and
            // inherit from the trivial test class ExampleBase. The
            // interface constraints are specified as an array 
            // containing the interface types.
            TSecond.SetBaseTypeConstraint(baseType);
            Type[] interfaceTypes = { interfaceA, interfaceB };
            TSecond.SetInterfaceConstraints(interfaceTypes);

            // The following code adds a private field named ExampleField,
            // of type TFirst.
            FieldBuilder exField =
                myType.DefineField("ExampleField", TFirst,
                    FieldAttributes.Private);

            // Define a static method that takes an array of TFirst and 
            // returns a List<TFirst> containing all the elements of 
            // the array. To define this method it is necessary to create
            // the type List<TFirst> by calling MakeGenericType on the
            // generic type definition, List<T>. (The T is omitted with
            // the typeof operator when you get the generic type 
            // definition.) The parameter type is created by using the
            // MakeArrayType method. 
            //
            Type listOf = typeof(List<>);
            Type listOfTFirst = listOf.MakeGenericType(TFirst);
            Type[] mParamTypes = { TFirst.MakeArrayType() };

            MethodBuilder exMethod =
                myType.DefineMethod("ExampleMethod",
                    MethodAttributes.Public | MethodAttributes.Static,
                    listOfTFirst,
                    mParamTypes);

            // Emit the method body. 
            // The method body consists of just three opcodes, to load 
            // the input array onto the execution stack, to call the 
            // List<TFirst> constructor that takes IEnumerable<TFirst>,
            // which does all the work of putting the input elements into
            // the list, and to return, leaving the list on the stack. The
            // hard work is getting the constructor.
            // 
            // The GetConstructor method is not supported on a 
            // GenericTypeParameterBuilder, so it is not possible to get 
            // the constructor of List<TFirst> directly. There are two
            // steps, first getting the constructor of List<T> and then
            // calling a method that converts it to the corresponding 
            // constructor of List<TFirst>.
            //
            // The constructor needed here is the one that takes an
            // IEnumerable<T>. Note, however, that this is not the 
            // generic type definition of IEnumerable<T>; instead, the
            // T from List<T> must be substituted for the T of 
            // IEnumerable<T>. (This seems confusing only because both
            // types have type parameters named T. That is why this example
            // uses the somewhat silly names TFirst and TSecond.) To get
            // the type of the constructor argument, take the generic
            // type definition IEnumerable<T> (expressed as 
            // IEnumerable<> when you use the typeof operator) and 
            // call MakeGenericType with the first generic type parameter
            // of List<T>. The constructor argument list must be passed
            // as an array, with just one argument in this case.
            // 
            // Now it is possible to get the constructor of List<T>,
            // using GetConstructor on the generic type definition. To get
            // the constructor of List<TFirst>, pass List<TFirst> and
            // the constructor from List<T> to the static
            // TypeBuilder.GetConstructor method.
            //
            ILGenerator ilgen = exMethod.GetILGenerator();

            Type ienumOf = typeof(IEnumerable<>);
            Type TfromListOf = listOf.GetGenericArguments()[0];
            Type ienumOfT = ienumOf.MakeGenericType(TfromListOf);
            Type[] ctorArgs = { ienumOfT };

            ConstructorInfo ctorPrep = listOf.GetConstructor(ctorArgs);
            ConstructorInfo ctor =
                TypeBuilder.GetConstructor(listOfTFirst, ctorPrep);

            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Newobj, ctor);
            ilgen.Emit(OpCodes.Ret);

            // Create the type and save the assembly. 
            Type finished = myType.CreateType();
            myAssembly.Save(myAsmName.Name + ".dll");

            // Invoke the method.
            // ExampleMethod is not generic, but the type it belongs to is
            // generic, so in order to get a MethodInfo that can be invoked
            // it is necessary to create a constructed type. The Example 
            // class satisfies the constraints on TFirst, because it is a 
            // reference type and has a default constructor. In order to
            // have a class that satisfies the constraints on TSecond, 
            // this code example defines the ExampleDerived type. These
            // two types are passed to MakeGenericMethod to create the
            // constructed type.
            //
            Type[] typeArgs = { typeof(GenericTypeWithReflectionEmit), typeof(ExampleDerived) };
            Type constructed = finished.MakeGenericType(typeArgs);
            MethodInfo mi = constructed.GetMethod("ExampleMethod");

            // Create an array of Example objects, as input to the generic
            // method. This array must be passed as the only element of an 
            // array of arguments. The first argument of Invoke is 
            // null, because ExampleMethod is static. Display the count
            // on the resulting List<Example>.
            // 
            GenericTypeWithReflectionEmit[] input = { new GenericTypeWithReflectionEmit(), new GenericTypeWithReflectionEmit() };
            object[] arguments = { input };

            List<GenericTypeWithReflectionEmit> listX =
                (List<GenericTypeWithReflectionEmit>)mi.Invoke(null, arguments);

            Console.WriteLine(
                "\nThere are {0} elements in the List<Example>.",
                listX.Count);

            DisplayGenericParameters(finished);
        }

        private static void DisplayGenericParameters(Type t)
        {
            if (!t.IsGenericType)
            {
                Console.WriteLine("Type '{0}' is not generic.");
                return;
            }
            if (!t.IsGenericTypeDefinition)
            {
                t = t.GetGenericTypeDefinition();
            }

            Type[] typeParameters = t.GetGenericArguments();
            Console.WriteLine("\nListing {0} type parameters for type '{1}'.",
                typeParameters.Length, t);

            foreach (Type tParam in typeParameters)
            {
                Console.WriteLine("\r\nType parameter {0}:", tParam.ToString());

                foreach (Type c in tParam.GetGenericParameterConstraints())
                {
                    if (c.IsInterface)
                    {
                        Console.WriteLine("    Interface constraint: {0}", c);
                    }
                    else
                    {
                        Console.WriteLine("    Base type constraint: {0}", c);
                    }
                }

                ListConstraintAttributes(tParam);
            }
        }

        // List the constraint flags. The GenericParameterAttributes
        // enumeration contains two sets of attributes, variance and
        // constraints. For this example, only constraints are used.
        //
        private static void ListConstraintAttributes(Type t)
        {
            // Mask off the constraint flags. 
            GenericParameterAttributes constraints =
                t.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;

            if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint)
                != GenericParameterAttributes.None)
            {
                Console.WriteLine("    ReferenceTypeConstraint");
            }

            if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint)
                != GenericParameterAttributes.None)
            {
                Console.WriteLine("    NotNullableValueTypeConstraint");
            }

            if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint)
                != GenericParameterAttributes.None)
            {
                Console.WriteLine("    DefaultConstructorConstraint");
            }
        }
    }
    /*
    Type 'Sample' is generic: False
    Type 'Sample' is generic: True

    There are 2 elements in the List<Example>.

    Listing 2 type parameters for type 'Sample[TFirst,TSecond]'.

    Type parameter TFirst:
        ReferenceTypeConstraint
        DefaultConstructorConstraint

    Type parameter TSecond:
        Interface constraint: TestNewFeatureConsoleApplication.IExampleA
        Interface constraint: TestNewFeatureConsoleApplication.IExampleB
        Base type constraint: TestNewFeatureConsoleApplication.ExampleBase
    */
    #endregion How to: Define a Generic Type with Reflection Emit
    #region How to: Define a Generic Method with Reflection Emit
    //https://msdn.microsoft.com/en-us/library/ms228971.aspx
    // Declare a generic delegate that can be used to execute the 
    // finished method.
    //
    public delegate TOut D<TIn, TOut>(TIn[] input);

    class GenericMethodBuilder
    {
        // This method shows how to declare, in Visual Basic, the generic
        // method this program emits. The method has two type parameters,
        // TInput and TOutput, the second of which must be a reference type
        // (class), must have a parameterless constructor (new()), and must
        // implement ICollection<TInput>. This interface constraint
        // ensures that ICollection<TInput>.Add can be used to add
        // elements to the TOutput object the method creates. The method 
        // has one formal parameter, input, which is an array of TInput. 
        // The elements of this array are copied to the new TOutput.
        //
        public static TOutput Factory<TInput, TOutput>(TInput[] tarray)
            where TOutput : class, ICollection<TInput>, new()
        {
            TOutput ret = new TOutput();
            ICollection<TInput> ic = ret;

            foreach (TInput t in tarray)
            {
                ic.Add(t);
            }
            return ret;
        }

        internal static void TestMethodGenericMethodBuilder()
        {
            // The following shows the usage syntax of the C#
            // version of the generic method emitted by this program.
            // Note that the generic parameters must be specified 
            // explicitly, because the compiler does not have enough 
            // context to infer the type of TOutput. In this case, TOutput
            // is a generic List containing strings.
            // 
            string[] arr = { "a", "b", "c", "d", "e" };
            List<string> list1 =
                GenericMethodBuilder.Factory<string, List<string>>(arr);
            Console.WriteLine("The first element is: {0}", list1[0]);


            // Creating a dynamic assembly requires an AssemblyName
            // object, and the current application domain.
            //
            AssemblyName asmName = new AssemblyName("DemoMethodBuilder1");
            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyBuilder demoAssembly =
                domain.DefineDynamicAssembly(asmName,
                    AssemblyBuilderAccess.RunAndSave);

            // Define the module that contains the code. For an 
            // assembly with one module, the module name is the 
            // assembly name plus a file extension.
            ModuleBuilder demoModule =
                demoAssembly.DefineDynamicModule(asmName.Name,
                    asmName.Name + ".dll");

            // Define a type to contain the method.
            TypeBuilder demoType =
                demoModule.DefineType("DemoType", TypeAttributes.Public);

            // Define a public static method with standard calling
            // conventions. Do not specify the parameter types or the
            // return type, because type parameters will be used for 
            // those types, and the type parameters have not been
            // defined yet.
            //
            MethodBuilder factory =
                demoType.DefineMethod("Factory",
                    MethodAttributes.Public | MethodAttributes.Static);

            // Defining generic type parameters for the method makes it a
            // generic method. To make the code easier to read, each
            // type parameter is copied to a variable of the same name.
            //
            string[] typeParameterNames = { "TInput", "TOutput" };
            GenericTypeParameterBuilder[] typeParameters =
                factory.DefineGenericParameters(typeParameterNames);

            GenericTypeParameterBuilder TInput = typeParameters[0];
            GenericTypeParameterBuilder TOutput = typeParameters[1];

            // Add special constraints.
            // The type parameter TOutput is constrained to be a reference
            // type, and to have a parameterless constructor. This ensures
            // that the Factory method can create the collection type.
            // 
            TOutput.SetGenericParameterAttributes(
                GenericParameterAttributes.ReferenceTypeConstraint |
                GenericParameterAttributes.DefaultConstructorConstraint);

            // Add interface and base type constraints.
            // The type parameter TOutput is constrained to types that
            // implement the ICollection<T> interface, to ensure that
            // they have an Add method that can be used to add elements.
            //
            // To create the constraint, first use MakeGenericType to bind 
            // the type parameter TInput to the ICollection<T> interface,
            // returning the type ICollection<TInput>, then pass
            // the newly created type to the SetInterfaceConstraints
            // method. The constraints must be passed as an array, even if
            // there is only one interface.
            //
            Type icoll = typeof(ICollection<>);
            Type icollOfTInput = icoll.MakeGenericType(TInput);
            Type[] constraints = { icollOfTInput };
            TOutput.SetInterfaceConstraints(constraints);

            // Set parameter types for the method. The method takes
            // one parameter, an array of type TInput.
            Type[] parms = { TInput.MakeArrayType() };
            factory.SetParameters(parms);

            // Set the return type for the method. The return type is
            // the generic type parameter TOutput.
            factory.SetReturnType(TOutput);

            // Generate a code body for the method. 
            // -----------------------------------
            // Get a code generator and declare local variables and
            // labels. Save the input array to a local variable.
            //
            ILGenerator ilgen = factory.GetILGenerator();

            LocalBuilder retVal = ilgen.DeclareLocal(TOutput);
            LocalBuilder ic = ilgen.DeclareLocal(icollOfTInput);
            LocalBuilder input = ilgen.DeclareLocal(TInput.MakeArrayType());
            LocalBuilder index = ilgen.DeclareLocal(typeof(int));

            Label enterLoop = ilgen.DefineLabel();
            Label loopAgain = ilgen.DefineLabel();

            ilgen.Emit(OpCodes.Ldarg_0);
            ilgen.Emit(OpCodes.Stloc_S, input);

            // Create an instance of TOutput, using the generic method 
            // overload of the Activator.CreateInstance method. 
            // Using this overload requires the specified type to have
            // a parameterless constructor, which is the reason for adding 
            // that constraint to TOutput. Create the constructed generic
            // method by passing TOutput to MakeGenericMethod. After
            // emitting code to call the method, emit code to store the
            // new TOutput in a local variable. 
            //
            MethodInfo createInst =
                typeof(Activator).GetMethod("CreateInstance", Type.EmptyTypes);
            MethodInfo createInstOfTOutput =
                createInst.MakeGenericMethod(TOutput);

            ilgen.Emit(OpCodes.Call, createInstOfTOutput);
            ilgen.Emit(OpCodes.Stloc_S, retVal);

            // Load the reference to the TOutput object, cast it to
            // ICollection<TInput>, and save it.
            //
            ilgen.Emit(OpCodes.Ldloc_S, retVal);
            ilgen.Emit(OpCodes.Box, TOutput);
            ilgen.Emit(OpCodes.Castclass, icollOfTInput);
            ilgen.Emit(OpCodes.Stloc_S, ic);

            // Loop through the array, adding each element to the new
            // instance of TOutput. Note that in order to get a MethodInfo
            // for ICollection<TInput>.Add, it is necessary to first 
            // get the Add method for the generic type defintion,
            // ICollection<T>.Add. This is because it is not possible
            // to call GetMethod on icollOfTInput. The static overload of
            // TypeBuilder.GetMethod produces the correct MethodInfo for
            // the constructed type.
            //
            MethodInfo mAddPrep = icoll.GetMethod("Add");
            MethodInfo mAdd = TypeBuilder.GetMethod(icollOfTInput, mAddPrep);

            // Initialize the count and enter the loop.
            ilgen.Emit(OpCodes.Ldc_I4_0);
            ilgen.Emit(OpCodes.Stloc_S, index);
            ilgen.Emit(OpCodes.Br_S, enterLoop);

            // Mark the beginning of the loop. Push the ICollection
            // reference on the stack, so it will be in position for the
            // call to Add. Then push the array and the index on the 
            // stack, get the array element, and call Add (represented
            // by the MethodInfo mAdd) to add it to the collection.
            //
            // The other ten instructions just increment the index
            // and test for the end of the loop. Note the MarkLabel
            // method, which sets the point in the code where the 
            // loop is entered. (See the earlier Br_S to enterLoop.)
            //
            ilgen.MarkLabel(loopAgain);

            ilgen.Emit(OpCodes.Ldloc_S, ic);
            ilgen.Emit(OpCodes.Ldloc_S, input);
            ilgen.Emit(OpCodes.Ldloc_S, index);
            ilgen.Emit(OpCodes.Ldelem, TInput);
            ilgen.Emit(OpCodes.Callvirt, mAdd);

            ilgen.Emit(OpCodes.Ldloc_S, index);
            ilgen.Emit(OpCodes.Ldc_I4_1);
            ilgen.Emit(OpCodes.Add);
            ilgen.Emit(OpCodes.Stloc_S, index);

            ilgen.MarkLabel(enterLoop);
            ilgen.Emit(OpCodes.Ldloc_S, index);
            ilgen.Emit(OpCodes.Ldloc_S, input);
            ilgen.Emit(OpCodes.Ldlen);
            ilgen.Emit(OpCodes.Conv_I4);
            ilgen.Emit(OpCodes.Clt);
            ilgen.Emit(OpCodes.Brtrue_S, loopAgain);

            ilgen.Emit(OpCodes.Ldloc_S, retVal);
            ilgen.Emit(OpCodes.Ret);

            // Complete the type.
            Type dt = demoType.CreateType();
            // Save the assembly, so it can be examined with Ildasm.exe.
            demoAssembly.Save(asmName.Name + ".dll");

            // To create a constructed generic method that can be
            // executed, first call the GetMethod method on the completed 
            // type to get the generic method definition. Call MakeGenericType
            // on the generic method definition to obtain the constructed
            // method, passing in the type arguments. In this case, the
            // constructed method has string for TInput and List<string>
            // for TOutput. 
            //
            MethodInfo m = dt.GetMethod("Factory");
            MethodInfo bound =
                m.MakeGenericMethod(typeof(string), typeof(List<string>));

            // Display a string representing the bound method.
            Console.WriteLine(bound);


            // Once the generic method is constructed, 
            // you can invoke it and pass in an array of objects 
            // representing the arguments. In this case, there is only
            // one element in that array, the argument 'arr'.
            //
            object o = bound.Invoke(null, new object[] { arr });
            List<string> list2 = (List<string>)o;

            Console.WriteLine("The first element is: {0}", list2[0]);


            // You can get better performance from multiple calls if
            // you bind the constructed method to a delegate. The 
            // following code uses the generic delegate D defined 
            // earlier.
            //
            Type dType = typeof(D<string, List<string>>);
            D<string, List<string>> test;
            test = (D<string, List<string>>)
                Delegate.CreateDelegate(dType, bound);

            List<string> list3 = test(arr);
            Console.WriteLine("The first element is: {0}", list3[0]);
        }
    }
    /* This code example produces the following output:

    The first element is: a
    System.Collections.Generic.List`1[System.String] Factory[String,List`1](System.String[])
    The first element is: a
    The first element is: a
     */
    #endregion How to: Define a Generic Method with Reflection Emit
    #region TestGenericInfo
    public class TestGenericInfo
    {
        private static void DisplayGenericTypeInfo(Type t)
        {
            Console.WriteLine("\r\n{0}", t);

            Console.WriteLine("\tIs this a generic type definition? {0}",
                t.IsGenericTypeDefinition);

            Console.WriteLine("\tIs it a generic type? {0}",
                t.IsGenericType);

            if (t.IsGenericType)
            {
                // If this is a generic type, display the type arguments.
                //
                Type[] typeArguments = t.GetGenericArguments();

                Console.WriteLine("\tList type arguments ({0}):",
                    typeArguments.Length);

                foreach (Type tParam in typeArguments)
                {
                    // If this is a type parameter, display its
                    // position.
                    //
                    if (tParam.IsGenericParameter)
                    {
                        Console.WriteLine("\t\t{0}\t(unassigned - parameter position {1})",
                            tParam,
                            tParam.GenericParameterPosition);
                    }
                    else
                    {
                        Console.WriteLine("\t\t{0}", tParam);
                    }
                }
            }
        }

        internal static void TestMehtodGenericInfo()
        {
            Console.WriteLine("\r\n--- Display information about a constructed type, its");
            Console.WriteLine("    generic type definition, and an ordinary type.");

            // Create a Dictionary of Test objects, using strings for the
            // keys.       
            Dictionary<string, TestGenericInfo> d = new Dictionary<string, TestGenericInfo>();

            // Display information for the constructed type and its generic
            // type definition.
            DisplayGenericTypeInfo(d.GetType());
            DisplayGenericTypeInfo(d.GetType().GetGenericTypeDefinition());

            // Display information for an ordinary type.
            DisplayGenericTypeInfo(typeof(string));

            DisplayGenericTypeInfo(new BMW<Whell<ConsoleColor>, ConsoleColor, string, int>().GetType());

        }
    }

    /* This example produces the following output:

    --- Display information about a constructed type, its
        generic type definition, and an ordinary type.

    System.Collections.Generic.Dictionary[System.String,Test]
            Is this a generic type definition? False
            Is it a generic type? True
            List type arguments (2):
                    System.String
                    Test

    System.Collections.Generic.Dictionary[TKey,TValue]
            Is this a generic type definition? True
            Is it a generic type? True
            List type arguments (2):
                    TKey    (unassigned - parameter position 0)
                    TValue  (unassigned - parameter position 1)

    System.String
            Is this a generic type definition? False
            Is it a generic type? False
     */
    #endregion TestGenericInfo
    #region ArraySegmentExample
    public class ArraySegmentExample
    {
        private const int segmentSize = 10;

        public static void TestMethodArraySegment1()
        {
            List<Task> tasks = new List<Task>();

            // Create array.
            int[] arr = new int[50];
            for (int ctr = 0; ctr <= arr.GetUpperBound(0); ctr++)
                arr[ctr] = ctr + 1;

            // Handle array in segments of 10.
            for (int ctr = 1; ctr <= Math.Ceiling(((double)arr.Length) / segmentSize); ctr++)
            {
                int multiplier = ctr;
                int elements = (multiplier - 1) * 10 + segmentSize > arr.Length ?
                                arr.Length - (multiplier - 1) * 10 : segmentSize;
                ArraySegment<int> segment = new ArraySegment<int>(arr, (ctr - 1) * 10, elements);
                tasks.Add(Task.Run(() =>
                {
                    IList<int> list = (IList<int>)segment;
                    for (int index = 0; index < list.Count; index++)
                        list[index] = list[index] * multiplier;
                }));
            }
            try
            {
                Task.WaitAll(tasks.ToArray());
                int elementsShown = 0;
                foreach (var value in arr)
                {
                    Console.Write("{0,3} ", value);
                    elementsShown++;
                    if (elementsShown % 18 == 0)
                        Console.WriteLine();
                }
            }
            catch (AggregateException e)
            {
                Console.WriteLine("Errors occurred when working with the array:");
                foreach (var inner in e.InnerExceptions)
                    Console.WriteLine("{0}: {1}", inner.GetType().Name, inner.Message);
            }
        }
        // The example displays the following output:
        //      1   2   3   4   5   6   7   8   9  10  22  24  26  28  30  32  34  36
        //     38  40  63  66  69  72  75  78  81  84  87  90 124 128 132 136 140 144
        //    148 152 156 160 205 210 215 220 225 230 235 240 245 250


        public static void TestMethodArraySegment2()
        {
            String[] names = { "Adam", "Bruce", "Charles", "Daniel",
                         "Ebenezer", "Francis", "Gilbert",
                         "Henry", "Irving", "John", "Karl",
                         "Lucian", "Michael" };
            var partNames = new ArraySegment<String>(names, 2, 5);

            // Cast the ArraySegment object to an IList<String> and enumerate it.
            var list = (IList<String>)partNames;
            for (int ctr = 0; ctr <= list.Count - 1; ctr++)
                Console.WriteLine(list[ctr]);
        }
        // The example displays the following output:
        //    Charles
        //    Daniel
        //    Ebenezer
        //    Francis
        //    Gilbert

        public static void TestMethodArraySegment3()
        {

            // Create and initialize a new string array.
            String[] myArr = { "The", "quick", "brown", "fox", "jumps", "over", "the", "lazy", "dog" };

            // Display the initial contents of the array.
            Console.WriteLine("The original array initially contains:");
            PrintIndexAndValues(myArr);

            // Define an array segment that contains the entire array.
            ArraySegment<String> myArrSegAll = new ArraySegment<String>(myArr);

            // Display the contents of the ArraySegment.
            Console.WriteLine("The first array segment (with all the array's elements) contains:");
            PrintIndexAndValues(myArrSegAll);

            // Define an array segment that contains the middle five values of the array.
            ArraySegment<String> myArrSegMid = new ArraySegment<String>(myArr, 2, 5);

            // Display the contents of the ArraySegment.
            Console.WriteLine("The second array segment (with the middle five elements) contains:");
            PrintIndexAndValues(myArrSegMid);

            // Modify the fourth element of the first array segment myArrSegAll.
            myArrSegAll.Array[3] = "LION";

            // Display the contents of the second array segment myArrSegMid.
            // Note that the value of its second element also changed.
            Console.WriteLine("After the first array segment is modified, the second array segment now contains:");
            PrintIndexAndValues(myArrSegMid);

        }

        public static void PrintIndexAndValues(ArraySegment<String> arrSeg)
        {
            for (int i = arrSeg.Offset; i < (arrSeg.Offset + arrSeg.Count); i++)
            {
                Console.WriteLine("   [{0}] : {1}", i, arrSeg.Array[i]);
            }
            Console.WriteLine();
        }

        public static void PrintIndexAndValues(String[] myArr)
        {
            for (int i = 0; i < myArr.Length; i++)
            {
                Console.WriteLine("   [{0}] : {1}", i, myArr[i]);
            }
            Console.WriteLine();
        }

        /* 
        This code produces the following output.

        The original array initially contains:
           [0] : The
           [1] : quick
           [2] : brown
           [3] : fox
           [4] : jumps
           [5] : over
           [6] : the
           [7] : lazy
           [8] : dog

        The first array segment (with all the array's elements) contains:
           [0] : The
           [1] : quick
           [2] : brown
           [3] : fox
           [4] : jumps
           [5] : over
           [6] : the
           [7] : lazy
           [8] : dog

        The second array segment (with the middle five elements) contains:
           [2] : brown
           [3] : fox
           [4] : jumps
           [5] : over
           [6] : the

        After the first array segment is modified, the second array segment now contains:
           [2] : brown
           [3] : LION
           [4] : jumps
           [5] : over
           [6] : the

        */


    }

    #endregion ArraySegmentExample
    #region Covariance and Contravariance in Generics
    /*
    https://msdn.microsoft.com/en-us/library/dd799517.aspx
    Covariance and contravariance are terms that refer to the ability to use a less derived(less specific) or more derived type(more specific) than originally specified.Generic type parameters support covariance and contravariance to provide greater flexibility in assigning and using generic types.When you are referring to a type system, covariance, contravariance, and invariance have the following definitions.The examples assume a base class named Base and a derived class named Derived.
    •Covariance
    Enables you to use a more derived type than originally specified.
    You can assign an instance of IEnumerable<Derived> (IEnumerable(Of Derived) in Visual Basic) to a variable of type IEnumerable<Base>.
    •Contravariance
    Enables you to use a more generic (less derived) type than originally specified.
    You can assign an instance of IEnumerable<Base> (IEnumerable(Of Base) in Visual Basic) to a variable of type IEnumerable<Derived>.
    •Invariance
    Means that you can use only the type originally specified; so an invariant generic type parameter is neither covariant nor contravariant.
    You cannot assign an instance of IEnumerable<Base> (IEnumerable(Of Base) in Visual Basic) to a variable of type IEnumerable<Derived> or vice versa.
    
        
    List of Variant Generic Interface and Delegate Types
    In the .NET Framework 4, the following interface and delegate types have covariant and/or contravariant type parameters.
    
    Type	                                                                                                Covariant type parameters	Contravariant type parameters
    Action<T> to Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>	                No	                        Yes
    Comparison<T>		                                                                                        No	                        Yes
    Converter<TInput, TOutput>		                                                                            Yes	                        Yes
    Func<TResult>			                                                                                    Yes	                        No
    Func<T, TResult> to Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>	Yes	                        Yes
    IComparable<T>			                                                                                    No	                        Yes
    Predicate<T>			                                                                                    No	                        Yes
    IComparer<T>			                                                                                    No	                        Yes
    IEnumerable<T>			                                                                                    Yes	                        No
    IEnumerator<T>			                                                                                    Yes	                        No
    IEqualityComparer<T>			                                                                            No	                        Yes
    IGrouping<TKey, TElement>			                                                                        Yes                         No
    IOrderedEnumerable<TElement>			                                                                    Yes                         No
    IOrderedQueryable<T>			                                                                            Yes                         No
    IQueryable<T>			                                                                                    Yes                         No
    */

    class TestCovarianceAndContravariance
    {
        public static void TestMethodCovarianceAndContravariance()
        {
            TestCovariance.TestMethodCovariance();
            TestContravariant.TestMethodContravariant();
            TestVariant.TestMethodVariant();

            //http://www.cnblogs.com/LoveJenny/archive/2012/03/13/2392747.html
            ITestCovarianceAndContravarianceOut<Dog, Animal> dog = new TestAnimail<Dog, Animal>();
            ITestCovarianceAndContravarianceOut<Animal/*Covariance*/, Dog/*Contravariance*/> animal = dog;
            Console.WriteLine("interface ITestCovarianceAndContravarianceOut<out T/*Covariance*/,in G/*Contravariance*/>");
        }
        private class Base
        {
            public static void PrintBases(IEnumerable<Base> bases)
            {
                foreach (Base b in bases)
                {
                    Console.WriteLine(b);
                }
            }
        }

        private class TestCovariance : Base
        {
            public static void TestMethodCovariance()
            {
                List<TestCovariance> dlist = new List<TestCovariance>();

                TestCovariance.PrintBases(dlist);
                IEnumerable<Base> bIEnum = dlist;
            }
        }

        private abstract class Shape
        {
            public virtual double Area { get { return 0; } }
        }

        private class Circle : Shape
        {
            private double r;
            public Circle(double radius) { r = radius; }
            public double Radius { get { return r; } }
            public override double Area { get { return Math.PI * r * r; } }
        }

        private class ShapeAreaComparer : IComparer<Shape>
        {
            int IComparer<Shape>.Compare(Shape a, Shape b)
            {
                if (a == null) return b == null ? 0 : -1;
                return b == null ? 1 : a.Area.CompareTo(b.Area);
            }
        }

        private class TestContravariant
        {
            public static void TestMethodContravariant()
            {
                // You can pass ShapeAreaComparer, which implements IComparer<Shape>,
                // even though the constructor for SortedSet<Circle> expects 
                // IComparer<Circle>, because type parameter T of IComparer<T> is
                // contravariant.
                SortedSet<Circle> circlesByArea =
                    new SortedSet<Circle>(new ShapeAreaComparer())
                        { new Circle(7.2), new Circle(100), null, new Circle(.01) };

                foreach (Circle c in circlesByArea)
                {
                    Console.WriteLine(c == null ? "null" : "Circle with area " + c.Area);
                }
            }
        }

        /* This code example produces the following output:

        null
        Circle with area 0.000314159265358979
        Circle with area 162.860163162095
        Circle with area 31415.9265358979
         */


        private class Type1 { }
        private class Type2 : Type1 { }
        private class Type3 : Type2 { }

        private class TestVariant
        {
            public static Type3 MyMethod(Type1 t)
            {
                Console.WriteLine("TestVariant: t as Type3 ?? new Type3() is {0}", t as Type3 ?? new Type3());
                return t as Type3 ?? new Type3();
            }

            public static void TestMethodVariant()
            {
                Func<Type2, Type2> f1 = MyMethod;

                // Covariant return type and contravariant parameter type.
                Func<Type3, Type1> f2 = f1;
                Type1 t1 = f2(new Type3());
            }
        }

        private class Animal
        {
            public string Name
            {
                get
                {
                    return this.GetType().Name;
                }
            }
        }
        private class Dog : Animal
        {

        }
        private class TestAnimail<T,G> : ITestCovarianceAndContravarianceOut<T,G>
        {
        }
    }
    interface ITestCovarianceAndContravarianceOut<out T/*Covariance*/,in G/*Contravariance*/>
    {
    }
    #endregion Covariance and Contravariance in Generics
}
