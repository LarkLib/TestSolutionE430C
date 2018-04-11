using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestUnityConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Microsoft.Practices.Unity.Configuration.AliasElement t;
            TestMehtodTestUnity();
            TestMehtodTestUnityInjection();
            TestMehtodConfiguration();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        private static void TestMehtodTestUnity()
        {
            {
                //To register a mapping for an interface or class to a concrete type 
                //1.Create a new instance of the UnityContainer class or use a reference to an existing instance. To create a new instance, you can use the "new" operator. 
                IUnityContainer myContainer = new UnityContainer();
                //2.If you want to use a child container nested within an existing container, call the CreateChildContainer method of the parent container. For more details about using nested containers, see Using Container Hierarchies. 
                IUnityContainer childCtr = myContainer.CreateChildContainer();
                //3.Call the RegisterType method of the container in which you want to register a mapping or a type. Specify the registered type as an interface or object type and the target type you want returned in response to a query for that type. The target type must implement the interface, or inherit from the class, that you specify as the registered type. The following code creates a default (un-named) mapping using an interface as the dependency key. 
                myContainer.RegisterType<IMyService, CustomerService>();
                //4.If you want to map a class or object type to a more specific class that inherits from it, use the same syntax but provide the inherited object as the registered type, as shown in this code. 
                myContainer.RegisterType<MyServiceBase, DataService>();
                //5.If you want to create more than one registration or mapping for the same type, you can create a named (non-default) mapping by specifying a name as a parameter, as shown in this code. 
                myContainer.RegisterType<IMyService, CustomerService>("Customers");
            }
            {
                //To register a class or type as a singleton instance
                //By default, the object instances returned by the container when you use the RegisterType method have a transient lifetime. The container does not store a reference to the object, and you will get a new instance of the type each time you call the Resolve method. If you want to manage the lifetime of the object, you specify a lifetime manager when you call the RegisterType method. The most common scenario is to create an instance that behaves like a singleton, so that the container creates the object the first time you call Resolve and then returns the same instance for all subsequent calls to Resolve for as long as the container is in scope.
                //1.Create or obtain a reference to a container as shown in steps 1 and 2 of the preceding procedure, "To register a mapping for an interface or class to a concrete type."
                IUnityContainer myContainer = new UnityContainer();
                //2.If you want to specify a mapping between an interface or base class and a concrete type, call the RegisterType method of the container in which you want to register a mapping.Specify as the registration type an interface or object type and the target type you want returned in response to a query for that registered type.The target type must implement the interface, or inherit from the class, that you specify as the registered type.Include an instance of the ContainerControlledLifetimeManager class in the parameters to the RegisterType method to instruct the container to register a singleton mapping.The following code creates a default (un-named) singleton mapping using an interface as the registration type.
                myContainer.RegisterType<IMyService, CustomerService>(new ContainerControlledLifetimeManager());
                //3.If you want to map a class or object type to a more specific class that inherits from it, use the same syntax but provide the base class as the registration type, as shown in this code. 
                myContainer.RegisterType<MyServiceBase, EmailService>(new ContainerControlledLifetimeManager());
                //4.If you do not want to specify a mapping between an interface or base class and a concrete type, but just register a specific type as a singleton, call the RegisterType method of the container in which you want to register the singleton. Specify as the registration type the concrete type you want returned in response to a query for that type. Include an instance of the ContainerControlledLifetimeManager class in the parameters to the RegisterType method. The following code creates a default (un-named) singleton registration for the type CustomerService. 
                myContainer.RegisterType<CustomerService>(new ContainerControlledLifetimeManager());
                //5.If you want to create more than one registration using the same registered type, you can create a named (non - default) registration by specifying a name as a parameter, as shown in this code.
                myContainer.RegisterType<CustomerService>("Customers", new ContainerControlledLifetimeManager());
                //6.In the same way, if you want to register more than one mapping for an object type that will return a singleton using the registered type, you can create a named (non-default) registration by specifying a name as a parameter, as shown in this code. 
                myContainer.RegisterType<IMyService, CustomerService>("Customers", new ContainerControlledLifetimeManager());
            }
            {
                //To register an existing object as a singleton instance 
                //1.Create or obtain a reference to a container as shown in steps 1 and 2 of the earlier procedure, "To register a mapping for an interface or class to a concrete type."
                IUnityContainer myContainer = new UnityContainer();
                //2.Call the RegisterInstance method of the container in which you want to register the existing object.Specify as the registration type an interface that the object implements, an object type from which the target object inherits, or the concrete type of the object. The following code creates a default (un-named) registration for an object of type EmailService that implements the IMyService interface. 
                EmailService myEmailService = new EmailService();
                myContainer.RegisterInstance<IMyService>(myEmailService);
                //3.If you do not want to map the existing object type to an interface or base class type, you can specify the actual concrete type for the registered type, as shown in this code.
                myContainer.RegisterInstance<EmailService>(myEmailService);
                //4.If you want to create more than one existing object registration using the same registered type, you can create named (non-default) mappings by specifying a name as a parameter, as shown in this code. 
                myContainer.RegisterInstance<EmailService>("Email", myEmailService);
            }
            {
                //To register an existing object instance as an externally controlled instance 
                //The default lifetime for objects registered using the RegisterInstance method is use of the ContainerControlledLifetimeManager, which causes them to behave like a singleton for the lifetime of the container. However, sometimes you may need to maintain control of the lifetime of existing objects or allow some other mechanism to control the lifetime. You can create your own custom lifetime managers for specific scenarios, but Unity includes the ExternallyControlledLifetimeManager class that provides generic support for externally managed lifetimes. It causes the container to maintain only a weak reference to the object when you register it, allowing other code to maintain the object in memory or dispose it.
                //1.Create or obtain a reference to a container as shown in steps 1 and 2 of the earlier procedure, "To register a mapping for an interface or class to a concrete type."
                IUnityContainer myContainer = new UnityContainer();
                //2.Call the RegisterInstance method of the container in which you want to register the existing object.Specify as the registration type an interface that the object implements, an object type from which the target object inherits, or the concrete type of the object. Provide a reference to the existing object and an instance of the ExternallyControlledLifetimeManager class as parameters of the method.The following code creates a default (un-named) externally managed registration using an interface as the registered type.
                EmailService myEmailService = new EmailService();
                myContainer.RegisterInstance<IMyService>(myEmailService, new ExternallyControlledLifetimeManager());
                //3.If you want to register the object using the object type, or a type from which the target object inherits, use the same syntax but provide the actual or the inherited object type as the registration type, as shown in this code. 
                myContainer.RegisterInstance<EmailService>(myEmailService, new ExternallyControlledLifetimeManager());
                //4.If you want to create more than one existing object registration using the same registered type, you can create named (non-default) mappings by specifying a name as a parameter, as shown in this code. 
                myContainer.RegisterInstance<MyServiceBase>("Email", myEmailService, new ExternallyControlledLifetimeManager());
            }
            {
                //To use the fluent interface of the container 
                //The API for the Unity container also provides a fluent interface. This means that you can use a chain of methods in one statement. 
                //1.To use the fluent interface, call all the methods you want one after the other in a single statement, as shown in this code. 
                EmailService myEmailService = new EmailService();
                IUnityContainer myContainer = new UnityContainer()
                   .RegisterType<IMyService, DataService>("Data")
                   .RegisterType<IMyService, EmailService>()
                   .RegisterType<MyServiceBase, CustomerService>()
                   .RegisterType<IMyUtilities, DataConversions>()
                   .RegisterInstance<IMyService>(myEmailService)
                   .RegisterInstance<IMyService>("Customers", myEmailService);


                //To retrieve an object from the container using the default mapping
                //1.Using a reference to the container, call the Resolve method and specify the required object type (the type specified when registering the object). If the registration was an interface of type IMyService, mapped to the concrete type EmailService, the following code will retrieve an instance of EmailService.
                IMyService result1 = myContainer.Resolve<IMyService>();
                //2.If the registration was an object of type MyServiceBase, mapped to the concrete type CustomerService, the following code will retrieve an instance of CustomerService.
                MyServiceBase result2 = myContainer.Resolve<MyServiceBase>();
                //3.If you know what the actual returned type will be, you might decide to specify this as the return type, as shown in the following code.However, this is likely to cause an error if you later change the mappings in the container. 
                CustomerService result3 = (CustomerService)myContainer.Resolve<MyServiceBase>();

                //To retrieve an object from the container using a named mapping 
                //1.Using a reference to the container, call the Resolve method and specify the required object type (the type specified when registering the object) and the registration name. If the registration was an interface of type IMyService, mapped to the concrete type DataService and registered with the name "Data" the following code will retrieve an instance of DataService. 
                IMyService result4 = myContainer.Resolve<IMyService>("Data");
                //2.If the registration was an object of type MyServiceBase, mapped to the concrete type CustomerService and registered with the name "Customers" the following code will retrieve an instance of CustomerService. 
                MyServiceBase result5 = myContainer.Resolve<MyServiceBase>("Customers");
                //3.If you know what the actual returned type will be, you might decide to specify this as the return type, as shown in the following code. However, this is likely to cause an error if you later change the mappings in the container. 
                EmailService result6 = (EmailService)myContainer.Resolve<IMyService>("Customers");

                //To retrieve a list of registered objects for a specified registered type 
                //1.Using a reference to the container, call the ResolveAll method and specify the required object type (the type specified when registering the object). The following code assumes you have registered one or more named mappings for the type IMyObject.
                IEnumerable<IMyService> objects = myContainer.ResolveAll<IMyService>();
                //2.Iterate over the list performing any required tasks with each object in the list. The following code assumes that the registered non-default mappings for IMyObject return objects of type MyRealObject. 
                foreach (IMyService foundObject in objects)
                {
                    // convert the object reference to the "real" type
                    IMyService theObject = foundObject as IMyService;
                    if (null != theObject)
                    // work with the object
                    {
                        Console.WriteLine(theObject.GetType());
                    }
                }

                //To apply dependency injection to an existing object instance 
                //1.Using a reference to the container, call the BuildUp method and specify the concrete type of the existing object that contains dependency attributes within its class definition. You do not need to register a mapping for this. The following code shows how to apply dependency injection to an object of type CustomerService for which you have a reference to an existing instance in the variable myService. The returned object is (or is type-compatible with) the type CustomerService. 
                var myService = new CustomerService();
                CustomerService result7 = myContainer.BuildUp<CustomerService>(myService);
                //2.You can also use the BuildUp method to apply dependency injection to an existing object where an extension uses the name to control the behavior of that extension. The following code shows how you can apply dependency injection to an object of type CustomerService for which you have a reference to an existing instance in the variable myService, using the name "Customers" to control extension behavior. 
                CustomerService result = myContainer.BuildUp<CustomerService>(myService, "Customers");
            }
        }
        private static void TestMehtodTestUnityInjection()
        {
            #region constructor injection
            /*
            Annotating Objects for Constructor Injection
            Retired Content
            This content is outdated and is no longer being maintained. It is provided as a courtesy for individuals who are still using these technologies. This page may contain URLs that were valid when originally published, but now link to sites or pages that no longer exist.

            The latest Unity Application Block information can be found at the Unity Application Block site. 
            The Unity Application Block supports automatic dependency injection and dependency injection specified through attributes applied to members of the target class. You can use the Unity container to generate instances of dependent objects and wire up the target class with these instances. 
            Typical Goals
            --------------------------------------------------------------------------------

            In this scenario, you use both the automatic constructor injection mechanism and an attribute applied to the constructor of a class to define the dependency injection requirements of that class. The attribute can also specify parameters that the constructor will pass to the dependent object that the container generates.
            Solution
            --------------------------------------------------------------------------------

            To perform injection of dependent classes into objects you create through the Unity container, you can use the following two techniques:
            • Single-constructor automatic injection. With this technique, you allow the Unity container to satisfy any constructor dependencies defined in parameters of the constructor automatically. You use this technique when there is a single constructor in the target class.
            • Multiple-constructor injection using an attribute. With this technique, you apply attributes to the class constructor(s) that specify the dependencies. You use this technique when there is more than one constructor in the target class.
            Constructor injection is a form of mandatory injection of dependent objects, provided developers use the Unity container to generate the target object. The dependent object instance is generated when the Unity container creates an instance of the target class using the constructor. 
            For more information, see Notes on Using Constructor Injection.

            Single-Constructor Automatic Injection
            For automatic constructor injection, you simply specify as parameters of the constructor the dependent object types. You can specify the concrete type, or specify an interface or base class for which the Unity container contains a registered mapping.
            To use automatic single-constructor injection to create dependent objects  
            1.Define a constructor in the target class that takes as a parameter the concrete type of the dependent class. For example, the following code shows a target class named MyObject containing a constructor that has a dependency on a class named MyDependentClass. 
            Copy
            public class MyObject
            {
              public MyObject(MyDependentClass myInstance)
              { 
                // work with the dependent instance
                myInstance.SomeProperty = "SomeValue";
                // or assign it to a class-level variable
              }
            } 
            2.In your run-time code, use the Resolve method of the container to create an instance of the target class. The Unity container will instantiate the dependent concrete class and inject it into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing a constructor that has a dependency on a class named MyDependentClass. 
            Copy
            IUnityContainer uContainer = new UnityContainer();
            MyObject myInstance = uContainer.Resolve<MyObject>();
            3.Alternatively, you can define a target class that contains more than one dependency defined in constructor parameters. The Unity container will instantiate and inject an instance of each one. For example, the following code shows a target class named MyObject containing a constructor that has dependencies on two classes named DependentClassA and DependentClassB. 
            Copy
            public class MyObject
            {
              public MyObject(DependentClassA depA, DependentClassB depB)
              { 
                // work with the dependent instances
                depA.SomeClassAProperty = "SomeValue";
                depB.SomeClassBProperty = "AnotherValue";
                // or assign them to class-level variables
              }
            } 
            4.In your run-time code, use the Resolve method of the container to create an instance of the target class. The Unity container will create an instance of each of the dependent concrete classes and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing a constructor that has constructor dependencies. 
            Copy
            IUnityContainer uContainer = new UnityContainer();
            MyObject myInstance = uContainer.Resolve<MyObject>();
            5.In addition to using concrete types as parameters of the target object constructor, you can use interfaces or base class types and then register mappings in the Unity container to translate these types into the correct concrete types. Define a constructor in the target class that takes as parameters the interface or base types of the dependent class. For example, the following code shows a target class named MyObject containing a constructor that has a dependency on a class that implements the interface named IMyInterface and a class that inherits from MyBaseClass. 
            Copy
            public class MyObject
            {
              public MyObject(IMyInterface interfaceObj, MyBaseClass baseObj)
              { 
                // work with the concrete dependent instances
                // or assign them to class-level variables
              }
            } 
            6.In your run-time code, register the mappings you require for the interface and base class types, and then use the Resolve method of the container to create an instance of the target class. The Unity container will instantiate an instance of each of the mapped concrete types for the dependent classes and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing a constructor that has a dependency on the two objects of type IMyInterface and MyBaseClass. 
            Copy
            IUnityContainer uContainer = new UnityContainer()
               .RegisterType<IMyInterface, FirstObject>()
               .RegisterType<MyBaseClass, SecondObject>();
            MyObject myInstance = uContainer.Resolve<MyObject>();

            Multiple-Constructor Injection Using an Attribute
            When a target class contains more than one constructor with the same number of parameters, you must apply the InjectionConstructor attribute to the constructor that the Unity container will use to indicate which constructor the container should use. As with automatic constructor injection, you can specify the constructor parameters as a concrete type, or you can specify an interface or base class for which the Unity container contains a registered mapping.
            To use attributed constructor injection when there is more than one constructor  
            1.Apply the InjectionConstructor attribute to the constructor in the target class that you want the container to use. In the simplest case, the target constructor takes as a parameter the concrete type of the dependent class. For example, the following code shows a target class named MyObject containing two constructors, one of which has a dependency on a class named MyDependentClass and has the InjectionConstructor attribute applied. 
            Copy
            public class MyObject
            {
              public MyObject(SomeOtherClass myObjA)
              { 
                ...
              }
              [InjectionConstructor]
              public MyObject(MyDependentClass myObjB)
              { 
                ...
              }
            } 
            2.In your run-time code, use the Resolve method of the container to create an instance of the target class. The Unity container will instantiate the dependent concrete class defined in the attributed constructor and inject it into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing an attributed constructor that has a dependency on a class named MyDependentClass. 
            Copy
            IUnityContainer uContainer = new UnityContainer();
            MyObject myInstance = uContainer.Resolve<MyObject>();
            3.Alternatively, you can define a multiple-constructor target class that contains more than one dependency defined in the target constructor parameters. The Unity container will instantiate and inject an instance of each one. For example, the following code shows a target class named MyObject containing an attributed constructor that has dependencies on two classes named DependentClassA and DependentClassB. 
            Copy
            public class MyObject
            {
              public MyObject(SomeClassA objA, SomeClassB objB)
              { 
                ...
              }
              [InjectionConstructor]
              public MyObject(DependentClassA depA, DependentClassB depB)
              { 
                ...
              }
            } 
            4.In your run-time code, use the Resolve method of the container to create an instance of the target class. The Unity container will create an instance of each of the dependent concrete classes defined in the attributed constructor and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing a constructor that has constructor dependencies 
            Copy
            IUnityContainer uContainer = new UnityContainer();
            MyObject myInstance = uContainer.Resolve<MyObject>();
            5.In addition to using concrete types as parameters of the target object constructor, you can use interfaces or base class types, and then register mappings in the Unity container to translate these types into the correct concrete types. For details, see steps 5 and 6 of the procedure Single-Constructor Automatic Injection.
            Notes on Using Constructor Injection
            --------------------------------------------------------------------------------

            The following notes will help you to get the most benefit from using constructor injection with the Unity Application Block.

            How Unity Resolves Target Constructors and Parameters
            When a target class contains more than one constructor, Unity will use the one that has the InjectionConstructor attribute applied. If there is more than one constructor, and none carries the InjectionConstructor attribute, Unity will use the constructor with the most parameters. If there is more than one such constructor (more than one of the "longest" with the same number of parameters), Unity will raise an exception. 

            Constructor Injection with Existing Objects
            If you use the RegisterInstance method to register an existing object, constructor injection does not take place on that object because it has already been created outside of the influence of the Unity container. Even if you call the BuildUp method of the container and pass it the existing object, constructor injection will never take place because the constructor will not execute. Instead, mark the constructor parameter containing the object you want to inject with the Dependency attribute to force property injection to take place on that object, and then call the BuildUp method. This is a similar process to property (setter) injection. It ensures that the dependent object can generate any dependent objects it requires. For more details, see Annotating Objects for Property (Setter) Injection.

            Avoiding Circular References
            Dependency injection mechanisms can cause application errors if there are circular references between objects that the container will create. For more details, see Circular References with Dependency Injection.

            When to Use Constructor Injection
            You should consider using constructor injection in the following situations:
            •You want to instantiate dependent objects automatically when your instantiate the parent object.
            •You want a simple approach that makes it easy to see in the code what the dependencies are for each class.
            •The parent object does not require a large number of constructors that forward to each other.
            •The parent object constructors do not require a large number of parameters.
            •You want to be able to hide field values from view in the application code by not exposing them as properties or methods.
            •You want to control which objects are injected by editing the code of the dependent object instead of the parent object or application.
            If you are not sure which type of injection to use, the recommendation is that you use constructor injection. This is likely to satisfy almost all general requirements.
            Ff650802.note(en-us,PandP.10).gifNote:
            You can also apply constructor injection at run time using the configuration API of the Unity container. For more information, see Configuring Containers at Run Time. 
            */
            //To use automatic single-constructor injection to create dependent objects  
            //1.Define a constructor in the target class that takes as a parameter the concrete type of the dependent class. For example, the following code shows a target class named MyObject containing a constructor that has a dependency on a class named MyDependentClass.
            /*
            public MyObject(MyDependentClass myInstance)
            {
                // work with the dependent instance
                myInstance.SomeProperty = "SomeValue";
                // or assign it to a class-level variable
            }
            */
            //2.In your run-time code, use the Resolve method of the container to create an instance of the target class. The Unity container will instantiate the dependent concrete class and inject it into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing a constructor that has a dependency on a class named MyDependentClass. 
            {
                IUnityContainer uContainer = new UnityContainer();
                MyObject0 myInstance = uContainer.Resolve<MyObject0>();
            }
            //3.Alternatively, you can define a target class that contains more than one dependency defined in constructor parameters. The Unity container will instantiate and inject an instance of each one. For example, the following code shows a target class named MyObject containing a constructor that has dependencies on two classes named DependentClassA and DependentClassB. 
            /*
            public class MyObject
            {
              public MyObject(DependentClassA depA, DependentClassB depB)
              { 
                // work with the dependent instances
                depA.SomeClassAProperty = "SomeValue";
                depB.SomeClassBProperty = "AnotherValue";
                // or assign them to class-level variables
              }
            } 
            */
            {
                //4.In your run-time code, use the Resolve method of the container to create an instance of the target class. The Unity container will create an instance of each of the dependent concrete classes and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing a constructor that has constructor dependencies. 
                IUnityContainer uContainer = new UnityContainer();
                MyObjectB myInstance = uContainer.Resolve<MyObjectB>();
            }
            {
                //5.In addition to using concrete types as parameters of the target object constructor, you can use interfaces or base class types and then register mappings in the Unity container to translate these types into the correct concrete types. Define a constructor in the target class that takes as parameters the interface or base types of the dependent class. For example, the following code shows a target class named MyObject containing a constructor that has a dependency on a class that implements the interface named IMyInterface and a class that inherits from MyBaseClass. 
                /*
                public class MyObject
                        {
                            public MyObject(IMyInterface interfaceObj, MyBaseClass baseObj)
                            {
                                // work with the concrete dependent instances
                                // or assign them to class-level variables
                            }
                        } 

                */
                //6.In your run-time code, register the mappings you require for the interface and base class types, and then use the Resolve method of the container to create an instance of the target class. The Unity container will instantiate an instance of each of the mapped concrete types for the dependent classes and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing a constructor that has a dependency on the two objects of type IMyInterface and MyBaseClass.
                IUnityContainer uContainer = new UnityContainer()
                    .RegisterType<IMyInterface, FirstObject>()
                    .RegisterType<MyBaseClass, SecondObject>();
                MyObjectC myInstance = uContainer.Resolve<MyObjectC>();
            }
            {
                //To use attributed constructor injection when there is more than one constructor  
                //1.Apply the InjectionConstructor attribute to the constructor in the target class that you want the container to use.In the simplest case, the target constructor takes as a parameter the concrete type of the dependent class. For example, the following code shows a target class named MyObject containing two constructors, one of which has a dependency on a class named MyDependentClass and has the InjectionConstructor attribute applied.
                //2.In your run-time code, use the Resolve method of the container to create an instance of the target class. The Unity container will instantiate the dependent concrete class defined in the attributed constructor and inject it into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing an attributed constructor that has a dependency on a class named MyDependentClass. 
                //IUnityContainer uContainer = new UnityContainer();
                //MyObjectD myInstance = uContainer.Resolve<MyObjectD>();
                //3.Alternatively, you can define a multiple-constructor target class that contains more than one dependency defined in the target constructor parameters. The Unity container will instantiate and inject an instance of each one. For example, the following code shows a target class named MyObject containing an attributed constructor that has dependencies on two classes named DependentClassA and DependentClassB. 
                //4.In your run-time code, use the Resolve method of the container to create an instance of the target class. The Unity container will create an instance of each of the dependent concrete classes defined in the attributed constructor and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing a constructor that has constructor dependencies 
                IUnityContainer uContainer = new UnityContainer();
                MyObjectD myInstance = uContainer.Resolve<MyObjectD>();
                //5.In addition to using concrete types as parameters of the target object constructor, you can use interfaces or base class types, and then register mappings in the Unity container to translate these types into the correct concrete types. For details, see steps 5 and 6 of the procedure Single-Constructor Automatic Injection.
            }
            /*
             How Unity Resolves Target Constructors and Parameters

            When a target class contains more than one constructor, Unity will use the one that has the InjectionConstructor attribute applied. If there is more than one constructor, and none carries the InjectionConstructor attribute, Unity will use the constructor with the most parameters. If there is more than one such constructor (more than one of the "longest" with the same number of parameters), Unity will raise an exception. 

            Constructor Injection with Existing Objects

            If you use the RegisterInstance method to register an existing object, constructor injection does not take place on that object because it has already been created outside of the influence of the Unity container. Even if you call the BuildUp method of the container and pass it the existing object, constructor injection will never take place because the constructor will not execute. Instead, mark the constructor parameter containing the object you want to inject with the Dependency attribute to force property injection to take place on that object, and then call the BuildUp method. This is a similar process to property (setter) injection. It ensures that the dependent object can generate any dependent objects it requires. For more details, see Annotating Objects for Property (Setter) Injection.

            Avoiding Circular References

            Dependency injection mechanisms can cause application errors if there are circular references between objects that the container will create. For more details, see Circular References with Dependency Injection.

            When to Use Constructor Injection

            You should consider using constructor injection in the following situations:
            •You want to instantiate dependent objects automatically when your instantiate the parent object.
            •You want a simple approach that makes it easy to see in the code what the dependencies are for each class.
            •The parent object does not require a large number of constructors that forward to each other.
            •The parent object constructors do not require a large number of parameters.
            •You want to be able to hide field values from view in the application code by not exposing them as properties or methods.
            •You want to control which objects are injected by editing the code of the dependent object instead of the parent object or application.

            If you are not sure which type of injection to use, the recommendation is that you use constructor injection. This is likely to satisfy almost all general requirements.
            */
            #endregion constructor injection

            #region  Property (Setter) Injection
            /*
            Annotating Objects for Property (Setter) Injection
            Retired Content
            This content is outdated and is no longer being maintained. It is provided as a courtesy for individuals who are still using these technologies. This page may contain URLs that were valid when originally published, but now link to sites or pages that no longer exist.

            The latest Unity Application Block information can be found at the Unity Application Block site. 
            The Unity Application Block supports dependency injection specified through attributes applied to members of the target class. You can use the Unity container to generate instances of dependent objects and wire up the target class with these instances. 
            Typical Goals
            --------------------------------------------------------------------------------

            In this scenario, you use an attribute that is applied to one or more property declarations of a class to define the dependency injection requirements of that class. The attribute can specify parameters for the attribute to control its behavior, such as the name of a registered mapping.
            Solution
            --------------------------------------------------------------------------------

            To perform injection of dependent classes into objects you create through the Unity container, you apply attributes to the classes that specify these dependencies. For property (or setter) injection, you apply the Dependency attribute to the property declarations of a class. The Unity container will create an instance of the dependent class within the scope of the target object (the object you specify in a Resolve method call) and assign this dependent object to the attributed property of the target object. 
            Property injection is a form of optional injection of dependent objects, provided developers use the Unity container to generate the target object. The dependent object instance is generated before the container returns the target object. In addition, unlike constructor injection, you must apply the appropriate attribute in the target class to initiate property injection.
            To use property (setter) injection to create dependent objects for a class 
            1.Define a property in the target class and apply the Dependency attribute to it to indicate that the type defined and exposed by the property is a dependency of the class. The following code demonstrates property injection for a class named MyObject that exposes as a property a reference an instance of another class named SomeOtherObject (not defined in this code). 
            Copy
            public class MyObject
            {
              private SomeOtherObject _dependentObject;
              [Dependency]
              public SomeOtherObject DependentObject 
              {
                get { return _dependentObject; }
                set { _dependentObject = value; }
              }
            } 
            2.In your run-time code, use the Resolve method of the container to create an instance of the target class, and then reference the property containing the dependent object. The Unity container will instantiate the dependent concrete class defined in the attributed property and inject it into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing an attributed property that has a dependency on a class named SomeOtherObject and then retrieve the dependent object from the DependentObject property. 
            Copy
            IUnityContainer uContainer = new UnityContainer();
            MyObject myInstance = uContainer.Resolve<MyObject>();
            // now access the property containing the dependency
            SomeOtherObject depObj = myInstance.DependentObject;
            3.In addition to using concrete types for the dependencies in target object properties, you can use interfaces or base class types, and then register mappings in the Unity container to translate these types into the correct concrete types. Define a property in the target class as an interface or base type. For example, the following code shows a target class named MyObject containing properties named InterfaceObject and BaseObject that have dependencies on a class that implements the interface named IMyInterface and on a class that inherits from MyBaseClass. 
            Copy
            public class MyObject
            {
              private IMyInterface _interfaceObj;
              private MyBaseClass _baseObj;
              [Dependency]
              public IMyInterface InterfaceObject
              {
                get { return _interfaceObj; }
                set { _interfaceObj = value; }
              }
              [Dependency]
              public MyBaseClass BaseObject
              {
                get { return _baseObj; }
                set { _baseObj = value; }
              }
            } 
            4.In your run-time code, register the mappings you require for the interface and base class types, and then use the Resolve method of the container to create an instance of the target class. The Unity container will create an instance of each of the mapped concrete types for the dependent classes and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing two properties that have dependencies on the two classes named FirstObject and SecondObject. 
            Copy
            IUnityContainer uContainer = new UnityContainer()
               .RegisterType<IMyInterface, FirstObject>()
               .RegisterType<MyBaseClass, SecondObject>();
            MyObject myInstance = uContainer.Resolve<MyObject>();
            // now access the properties containing the dependencies
            IMyInterface depObjA = myInstance.InterfaceObject;
            MyBaseClass depObjB = myInstance.BaseObject;
            5.You can register multiple named mappings with the container for each dependency type, if required, and then use a parameter of the Dependency attribute to specify the mapping you want to use to resolve the dependent object type. For example, the following code specifies the mapping names for the Key property of the Dependency attribute for two properties of the same type (in this case, an interface) in the class MyObject. 
            Copy
            public class MyObject
            {
              private IMyInterface _objA, _objB;
              [Dependency("MapTypeA")]
              public IMyInterface ObjectA
              {
                get { return _objA; }
                set { _objA = value; }
              }
              [Dependency("MapTypeB")]
              public IMyInterface ObjectB
              {
                get { return _objB; }
                set { _objB = value; }
              }
            } 
            6.In your run-time code, register the named (non-default) mappings you require for the two concrete types that the properties will depend on, and then use the Resolve method of the container to create an instance of the target class. The Unity container will instantiate an instance of each of the mapped concrete types for the dependent classes and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing two properties that have dependencies on the two classes named FirstObject and SecondObject. 
            Copy
            IUnityContainer uContainer = new UnityContainer()
               .RegisterType<IMyInterface, FirstObject>("MapTypeA")
               .RegisterType<IMyInterface, SecondObject>("MapTypeB");
            MyObject myInstance = uContainer.Resolve<MyObject>();
            // now access the properties containing the dependencies
            IMyInterface depObjA = myInstance.ObjectA;
            IMyInterface depObjB = myInstance.ObjectB;
            Notes on Using Property (Setter) Injection
            --------------------------------------------------------------------------------

            Remember that property injection is optional and that you must apply the Dependency attribute to target class properties if you want property injection of dependent types to occur. 

            Using Property Injection with Constructor Parameters
            You can also apply the Dependency attribute to the parameters of the constructor of a class if you only require access to these dependent objects within the constructor—though you can use code in the constructor to save the newly created dependent object instances in class-level variables if required. 
            This approach implements mandatory injection of dependent objects, providing that developers use the Unity container to generate the original (target) object. The dependent object instance is generated when the Unity container creates an instance of the target class using the attributed constructor.
            For example, the following extracts from the StoplightPresenter class of the StopLight QuickStart sample show how the property declaration for the Schedule property defines a dependency on the StoplightSchedule class.
            Copy
            [Dependency]
            public StoplightSchedule Schedule
            {
              get { return _schedule; }
              set { _schedule = value; }
            }
            The constructor of the StoplightSchedule class also contains a Dependency attribute that specifies a dependency on a concrete implementation of the IStoplightTimer class.
            Copy
            public StoplightSchedule([Dependency] IStoplightTimer timer)
            {
              this._timer = timer;
            }
            The following code located in the main program defines a registered mapping between the IStopLightTimer interface and the concrete implementation named RealTimeTimer.
            Copy
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IStoplightTimer, RealTimeTimer>();
            Therefore, when the main program accesses the Schedule property of the StoplightPresenter class, the Unity container will create an instance of the StoplightSchedule class. The Dependency attribute in the parameters of the StoplightSchedule class constructor will cause the Unity container to create an instance of the RealTimeTimer class.

            Property Injection with Existing Objects
            If you use the RegisterInstance method to register an existing object, property (setter) injection does not take place on that object because it has already been created outside of the influence of the Unity container. However, you can call the BuildUp method of the container and pass it the existing object to force property injection to take place on that object.

            Avoiding Circular References
            Dependency injection mechanisms can cause application errors if there are circular references between objects that the container will create. For more details, see Circular References with Dependency Injection.

            When to Use Property (Setter) Injection
            You should consider using property injection in the following situations:
            •You want to instantiate dependent objects automatically when you instantiate the parent object.
            •You want a simple approach that makes it easy to see in the code what the dependencies are for each class.
            •The parent object requires a large number of constructors that forward to each other, making debugging and maintenance difficult.
            •The parent object constructors require a large number of parameters, especially if they are of similar types and the only way to identify them is by position.
            •You want to make it easier for users to see what settings and objects are available, which is not possible using constructor injection.
            •You want to control which objects are injected by editing the code of the dependent object instead of the parent object or application.
            If you are not sure which type of injection to use, the recommendation is that you use constructor injection. This is likely to satisfy almost all general requirements.
            Ff649447.note(en-us,PandP.10).gifNote:
            You can also apply property injection at run time using the configuration API of the Unity container. For more information, see Configuring Containers at Run Time. 
            */
            //1.Define a property in the target class and apply the Dependency attribute to it to indicate that the type defined and exposed by the property is a dependency of the class. The following code demonstrates property injection for a class named MyObject that exposes as a property a reference an instance of another class named SomeOtherObject(not defined in this code). 
            //2.In your run-time code, use the Resolve method of the container to create an instance of the target class, and then reference the property containing the dependent object. The Unity container will instantiate the dependent concrete class defined in the attributed property and inject it into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing an attributed property that has a dependency on a class named SomeOtherObject and then retrieve the dependent object from the DependentObject property.
            //IUnityContainer uContainer = new UnityContainer();
            //MyObject myInstance = uContainer.Resolve<MyObject>();
            //// now access the property containing the dependency
            //SomeOtherObject depObj = myInstance.DependentObject;//no code to instance it and unity do it.
            {
                //3.In addition to using concrete types for the dependencies in target object properties, you can use interfaces or base class types, and then register mappings in the Unity container to translate these types into the correct concrete types. Define a property in the target class as an interface or base type. For example, the following code shows a target class named MyObject containing properties named InterfaceObject and BaseObject that have dependencies on a class that implements the interface named IMyInterface and on a class that inherits from MyBaseClass. 
                //4.In your run-time code, register the mappings you require for the interface and base class types, and then use the Resolve method of the container to create an instance of the target class. The Unity container will create an instance of each of the mapped concrete types for the dependent classes and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing two properties that have dependencies on the two classes named FirstObject and SecondObject.
                IUnityContainer uContainer = new UnityContainer()
                .RegisterType<IMyInterface, FirstObject>()
                .RegisterType<MyBaseClass, SecondObject>()
                .RegisterType<IMyInterface, FirstObject>("MapTypeA")
                .RegisterType<IMyInterface, SecondObject>("MapTypeB");
                MyObject myInstance = uContainer.Resolve<MyObject>();
                // now access the properties containing the dependencies
                IMyInterface depObjA = myInstance.InterfaceObject;
                MyBaseClass depObjB = myInstance.BaseObject;
            }
            {
                //5.You can register multiple named mappings with the container for each dependency type, if required, and then use a parameter of the Dependency attribute to specify the mapping you want to use to resolve the dependent object type. For example, the following code specifies the mapping names for the Key property of the Dependency attribute for two properties of the same type(in this case, an interface) in the class MyObject. 
                //6.In your run-time code, register the named (non-default) mappings you require for the two concrete types that the properties will depend on, and then use the Resolve method of the container to create an instance of the target class. The Unity container will instantiate an instance of each of the mapped concrete types for the dependent classes and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing two properties that have dependencies on the two classes named FirstObject and SecondObject. 
                //Therefore, when the main program accesses the Schedule property of the StoplightPresenter class, the Unity container will create an instance of the StoplightSchedule class. The Dependency attribute in the parameters of the StoplightSchedule class constructor will cause the Unity container to create an instance of the RealTimeTimer class.
                IUnityContainer uContainer = new UnityContainer()
                    .RegisterType<IMyInterface, FirstObject>()
                    .RegisterType<MyBaseClass, SecondObject>()
                    .RegisterType<IMyInterface, FirstObject>("MapTypeA")
                    .RegisterType<IMyInterface, SecondObject>("MapTypeB");
                MyObject myInstance = uContainer.Resolve<MyObject>();

                // now access the properties containing the dependencies
                IMyInterface depObjA = myInstance.ObjectA;
                IMyInterface depObjB = myInstance.ObjectB;
            }
            {
                //Using Property Injection with Constructor Parameters
                //For example, the following extracts from the StoplightPresenter class of the StopLight QuickStart sample show how the property declaration for the Schedule property defines a dependency on the StoplightSchedule class.
                // [Dependency]
                //public StoplightSchedule Schedule
                //{
                //    get { return _schedule; }
                //    set { _schedule = value; }
                //}
                //The constructor of the StoplightSchedule class also contains a Dependency attribute that specifies a dependency on a concrete implementation of the IStoplightTimer class.
                //public StoplightSchedule([Dependency] IStoplightTimer timer)
                //{
                //    this._timer = timer;
                //}
                //The following code located in the main program defines a registered mapping between the IStopLightTimer interface and the concrete implementation named RealTimeTimer.
                IUnityContainer container = new UnityContainer();
                container.RegisterType<IStoplightTimer, RealTimeTimer>();
                /*
                 Property Injection with Existing Objects

                If you use the RegisterInstance method to register an existing object, property (setter)injection does not take place on that object because it has already been created outside of the influence of the Unity container.However, you can call the BuildUp method of the container and pass it the existing object to force property injection to take place on that object.

                Avoiding Circular References

                Dependency injection mechanisms can cause application errors if there are circular references between objects that the container will create. For more details, see Circular References with Dependency Injection.


                When to Use Property (Setter)Injection

                You should consider using property injection in the following situations:
                •You want to instantiate dependent objects automatically when you instantiate the parent object.
                •You want a simple approach that makes it easy to see in the code what the dependencies are for each class.
                •The parent object requires a large number of constructors that forward to each other, making debugging and maintenance difficult.
                •The parent object constructors require a large number of parameters, especially if they are of similar types and the only way to identify them is by position.
                •You want to make it easier for users to see what settings and objects are available, which is not possible using constructor injection.
                •You want to control which objects are injected by editing the code of the dependent object instead of the parent object or application.

                If you are not sure which type of injection to use, the recommendation is that you use constructor injection.This is likely to satisfy almost all general requirements.
                */
            }
            #endregion  Property (Setter) Injection

            #region Method Call Injection
            /*
            1.Define a method in the target class and apply the InjectionMethod attribute to it to indicate that any types defined in parameters of the method are dependencies of the class. The following code demonstrates the most common scenario, saving the dependent object instance in a class-level variable, for a class named MyObject that exposes a method named Initialize that takes as a parameter a reference an instance of another class named SomeOtherObject (not defined in this code). 
            public class MyObject
            {
              public SomeOtherObject dependentObject;
              [InjectionMethod]
              public void Initialize(SomeOtherObject dep) 
              {
                // assign the dependent object to a class-level variable
                dependentObject = dep;
              }
            } 
            2.In your run-time code, use the Resolve method of the container to create an instance of the target class. The Unity container will instantiate the dependent concrete class defined in the attributed method, inject it into the target class, and execute the method. For example, the following code shows how you can instantiate the example target class named MyObject containing an attributed method that has a dependency on a class named SomeOtherObject and then reference the injected object. 
            IUnityContainer uContainer = new UnityContainer();
            MyObject myInstance = uContainer.Resolve<MyObject>();
            // access the dependent object
            myInstance.dependentObject.SomeProperty = "Some value";
            3.In addition to using concrete types for the dependencies in target object methods, you can use interfaces or base class types and then register mappings in the Unity container to translate these types into the appropriate concrete types. Define a method in the target class that takes as parameters interfaces or base types. For example, the following code shows a target class named MyObject containing a method named Initialize that takes as parameters an object named interfaceObj that implements the interface named IMyInterface and an object named baseObj that inherits from the class MyBaseClass, respectively. 
            public class MyObject
            {
              public IMyInterface depObjectA;
              public MyBaseClass depObjectB;
              [InjectionMethod]
              public void Initialize(IMyInterface interfaceObj, MyBaseClass baseObj) 
              {
                depObjectA = interfaceObj;
                depObjectB = baseObj;
              }
            } 
            4.In your run-time code, register the mappings you require for the interface and base class types, and then use the Resolve method of the container to create an instance of the target class. The Unity container will instantiate an instance of each of the mapped concrete types for the dependent classes and inject them into the target class. For example, the following code shows how you can instantiate the example target class named MyObject containing an attributed method that has dependencies on the two classes named FirstObject and SecondObject. 
            IUnityContainer uContainer = new UnityContainer()
               .RegisterType<IMyInterface, FirstObject>()
               .RegisterType<MyBaseClass, SecondObject>();
            MyObject myInstance = uContainer.Resolve<MyObject>();
            // now access the public variables containing the dependencies
            IMyInterface depObjA = myInstance.depObjectA;
            MyBaseClass depObjB = myInstance.depObjectB;

            Notes on Using Method Call Injection
            --------------------------------------------------------------------------------
            The following notes will help you to get the most benefit from using method call injection with the Unity Application Block.
            Method Call Injection with Existing Objects
            If you use the RegisterInstance method to register an existing object, method call injection does not take place on that object because it has already been created outside of the influence of the Unity container. However, you can call the BuildUp method of the container and pass it the existing object to force method call injection to take place on that object.
            Avoiding Circular References
            Dependency injection mechanisms can cause application errors if there are circular references between objects that the container will create. For more details, see Circular References with Dependency Injection.

            When to Use Method Call Injection
            You should consider using method call injection in the following situations:
            •You want to instantiate dependent objects automatically when your instantiate the parent object.
            •You want a simple approach that makes it easy to see in the code what the dependencies are for each class.
            •The parent object requires a large number of constructors that forward to each other, making debugging and maintenance difficult.
            •The parent object constructors require a large number of parameters, especially if they are of similar types and the only way to identify them is by position. 
            •You want to hide the dependent objects by not exposing them as properties.
            •You want to control which objects are injected by editing the code of the dependent object instead of the parent object or application.
            If you are not sure which type of injection to use, the recommendation is that you use constructor injection. This is likely to satisfy almost all general requirements.
            */
            {
                IUnityContainer uContainer = new UnityContainer()
                    .RegisterType<IMyInterface, FirstObject>()
                    .RegisterType<MyBaseClass, SecondObject>()
                    .RegisterType<IMyInterface, FirstObject>("MapTypeA")
                    .RegisterType<IMyInterface, SecondObject>("MapTypeB")
                    .RegisterType<IMyInterface, FirstObject>()
                    .RegisterType<MyBaseClass, SecondObject>();
                MyObject myInstance = uContainer.Resolve<MyObject>();

                // now access the public variables containing the dependencies
                IMyInterface depObjA = myInstance.depObjectA;
                MyBaseClass depObjB = myInstance.depObjectB;
            }
            #endregion Method Call Injection
        }

        private static void TestMehtodConfiguration()
        {
            IUnityContainer container = new UnityContainer();
            UnityConfigurationSection configuration = (UnityConfigurationSection)ConfigurationManager.GetSection(UnityConfigurationSection.SectionName);
            configuration.Configure(container);
            var logger = container.Resolve<ILogger>();
        }
    }

    #region helper class
    internal class SecondObject : MyBaseClass
    {
    }

    internal class FirstObject : MyBaseClass
    {
    }
    public class MyObject0
    {
        public MyObject0(MyDependentClass myInstance)
        {
            // work with the dependent instance
            myInstance.SomeProperty = "SomeValue";
            // or assign it to a class-level variable
        }
    }

    internal class MyObject
    {
        public MyObject(MyDependentClass myInstance)
        {
            // work with the dependent instance
            myInstance.SomeProperty = "SomeValue";
            // or assign it to a class-level variable
        }

        private SomeOtherObject _dependentObject;

        [Dependency]
        public SomeOtherObject DependentObject
        {
            get { return _dependentObject; }
            set { _dependentObject = value; }
        }
        private IMyInterface _interfaceObj;
        private MyBaseClass _baseObj;

        [Dependency]
        public IMyInterface InterfaceObject
        {
            get { return _interfaceObj; }
            set { _interfaceObj = value; }
        }

        [Dependency]
        public MyBaseClass BaseObject
        {
            get { return _baseObj; }
            set { _baseObj = value; }
        }
        private IMyInterface _objA, _objB;

        [Dependency("MapTypeA")]
        public IMyInterface ObjectA
        {
            get { return _objA; }
            set { _objA = value; }
        }

        [Dependency("MapTypeB")]
        public IMyInterface ObjectB
        {
            get { return _objB; }
            set { _objB = value; }
        }
        public SomeOtherObject dependentObject;

        [InjectionMethod]
        public void Initialize(SomeOtherObject dep)
        {
            // assign the dependent object to a class-level variable
            dependentObject = dep;
        }
        public IMyInterface depObjectA;
        public MyBaseClass depObjectB;

        [InjectionMethod]
        public void Initialize(IMyInterface interfaceObj, MyBaseClass baseObj)
        {
            depObjectA = interfaceObj;
            depObjectB = baseObj;
        }
    }
    public class MyObjectB
    {
        public MyObjectB(DependentClassA depA, DependentClassB depB)
        {
            // work with the dependent instances
            depA.SomeClassAProperty = "SomeValue";
            depB.SomeClassBProperty = "AnotherValue";
            // or assign them to class-level variables
        }
    }
    public class MyObjectC
    {
        public MyObjectC(IMyInterface interfaceObj, MyBaseClass baseObj)
        {
            // work with the concrete dependent instances
            // or assign them to class-level variables
        }
    }
    public class MyObjectD
    {
        public MyObjectD(SomeOtherClass myObjA)
        {
        }
        [InjectionConstructor]
        public MyObjectD(DependentClassA depA, DependentClassB depB)
        {
        }
    }

    public class SomeOtherClass
    {
    }

    public interface IMyInterface
    {
    }

    public class MyBaseClass : IMyInterface
    {
    }

    public class DependentClassB
    {
        public string SomeClassBProperty { get; internal set; }
    }

    public class DependentClassA
    {
        public string SomeClassAProperty { get; internal set; }
    }

    public class MyDependentClass
    {
        public string SomeProperty { get; internal set; }
    }

    internal class DataConversions : IMyUtilities
    {
    }

    internal interface IMyUtilities
    {
    }

    internal class EmailService : MyServiceBase
    {
    }

    internal class DataService : MyServiceBase
    {
    }

    internal class MyServiceBase : IMyService
    {
    }

    internal class CustomerService : MyServiceBase
    {
    }

    internal interface IMyService
    {
    }

    internal class RealTimeTimer : IStoplightTimer
    {
    }
    internal class StoplightSchedule
    {
        private StoplightSchedule _schedule;
        private IStoplightTimer _timer;

        [Dependency]
        public StoplightSchedule Schedule
        {
            get { return _schedule; }
            set { _schedule = value; }
        }
        public StoplightSchedule([Dependency] IStoplightTimer timer)
        {
            this._timer = timer;
        }
    }

    internal interface IStoplightTimer
    {
    }

    internal class SomeOtherObject
    {
    }
    internal interface ILogger
    {

    }
    internal class SpecialLogger : ILogger
    {

    }
    internal class PageAdminLogger : ILogger
    {

    }
    internal class EventLogLogger : ILogger
    {

    }

    internal class NormalLogger : ILogger
    {
        public NormalLogger(string port, ConsoleColor color)
        {
            this.Port = port;
            this.Color = color;
        }
        public string Port { get; set; }
        public int Connections { get; set; }
        public ConsoleColor Color { get; set; }
        public string Settings { get; set; }
        public bool Initialize(StringBuilder loggerSettings)
        {
            return true;
        }
    }
    #endregion helper class
}
