using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    class TestAsync : INewFeature
    {
        public void ExecuteTest()
        {
            TestTaskQueue();
            TestAsyncMethod();
        }

        #region ececute task by order
        private ConcurrentQueue<Task> taskQueue = new ConcurrentQueue<Task>();
        private string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private object lockObject = new object();
        void TestTaskQueue()
        {
            Console.WriteLine("TestTaskQueue enter");
            Task.Factory.StartNew(ExecuteTaskQueue);
            for (int i = 0; i < 10; i++)
            {
                int j = i; //why? hehe, try it use i and i always 10, 1/18/2017 22:46:15 -- 10
                var task = new Task(() => WriteText(j.ToString()));
                taskQueue.Enqueue(task);
                //taskQueue.Enqueue(new Task(() => WriteText(i.ToString())));
            }
            Console.WriteLine("TestTaskQueue exit");
        }
        void ExecuteTaskQueue()
        {
            Thread.Sleep(2000);
            Task task;
            while (taskQueue.TryDequeue(out task))
            {
                task.Start();
                task.Wait();
                //await Task.Yield();
            }
        }
        void WriteText(string message)
        {
            lock (lockObject)
            {
                using (var writer = new StreamWriter(new FileStream($"{Path.Combine(CurrentDirectory, "TextFile.txt")}", FileMode.Append, FileAccess.Write, FileShare.None), Encoding.Default))
                {
                    writer.WriteLine($"{DateTime.Now.ToString()} -- {message}");
                }
            }
            Thread.Sleep(1000);
            Console.WriteLine($"{DateTime.Now.ToString()} -- {message}");
        }
        #endregion ececute task by order

        #region TestAsyncMethod
        void TestAsyncMethod()
        {
            Console.WriteLine("ExecuteTest enter");
            // Create task and start it.
            // ... Wait for it to complete.
            Task task = new Task(ProcessDataAsync);
            task.Start();
            task.Wait();
            Console.WriteLine("ExecuteTest exit");
            Console.ReadLine();
        }
        async void ProcessDataAsync()
        {
            // Start the HandleFile method.
            Task<int> task = HandleFileAsync($"{Path.Combine(CurrentDirectory, "License-LGPL.txt")}");

            // Control returns here before HandleFileAsync returns.
            // ... Prompt the user.
            Thread.Sleep(3000);
            Console.WriteLine("Please wait patiently while I do something important.");

            // Wait for the HandleFile task to complete.
            // ... Display its results.
            int x = await task;
            Console.WriteLine("Count: " + x);
        }
        async Task<int> HandleFileAsync(string file)
        {
            Console.WriteLine("HandleFile enter");
            int count = 0;

            // Read in the specified file.
            // ... Use async StreamReader method.
            using (StreamReader reader = new StreamReader(file))
            {
                string v = await reader.ReadToEndAsync();

                // ... Process the file data somehow.
                count += v.Length;

                // ... A slow-running computation.
                //     Dummy code.
                for (int i = 0; i < 10000; i++)
                {
                    int x = v.GetHashCode();
                    if (x == 0)
                    {
                        count--;
                    }
                }
            }
            Thread.Sleep(5000);
            Console.WriteLine("HandleFile exit");
            return count;
        }
        #endregion TestAsyncMethod
    }

}
