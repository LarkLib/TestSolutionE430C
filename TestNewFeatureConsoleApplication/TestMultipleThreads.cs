using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    partial class TestMultipleThreads : INewFeature
    {
        public void ExecuteTest()
        {
            //TestVolatileWorkerThread.TestMethodVolatile();
            //TestSpinWait.TestMethodSpinWait();
            //TestBarrier.TestMethodBarrier();
            //TestReaderWriterLockSlim.TestMethodReaderWriterLockSlim();
            //TestReaderWriterLock.TestMehtodReaderWriterLock();
            //TestSpinLock.TestMethodSpinLock();
            //TestCountdownEvent.TestMethodCountdownEvent();
            //TestManualResetEvent.TestMethodManualResetEvent();
            //TestManualResetEventSlim.TestMethodManualResetEventSlim();
            //TestSemaphore.TestMethodSemaphore();
            //TestSemaphoreSlim.TestMethodSemaphoreSlim();
            //TestMonitor.TestMethodMonitor();
            //TestMonitor2.TestMethodMonitor();
            //TestMutex.TesMethodtMutex();
            TestMehtodPartial();
        }
    }

    public class TestReaderWriterLockSlim
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private Dictionary<int, string> innerCache = new Dictionary<int, string>();

        public int Count
        { get { return innerCache.Count; } }
        public static void TestMethodReaderWriterLockSlim()
        {
            var sc = new TestReaderWriterLockSlim();
            var tasks = new List<Task>();
            int itemsWritten = 0;

            // Execute a writer.
            tasks.Add(Task.Run(() =>
            {
                String[] vegetables = { "broccoli", "cauliflower",
                                                          "carrot", "sorrel", "baby turnip",
                                                          "beet", "brussel sprout",
                                                          "cabbage", "plantain",
                                                          "spinach", "grape leaves",
                                                          "lime leaves", "corn",
                                                          "radish", "cucumber",
                                                          "raddichio", "lima beans" };
                for (int ctr = 1; ctr <= vegetables.Length; ctr++)
                    sc.Add(ctr, vegetables[ctr - 1]);

                itemsWritten = vegetables.Length;
                Console.WriteLine("Task {0} wrote {1} items\n",
                                  Task.CurrentId, itemsWritten);
            }));
            // Execute two readers, one to read from first to last and the second from last to first.
            for (int ctr = 0; ctr <= 1; ctr++)
            {
                bool desc = Convert.ToBoolean(ctr);
                tasks.Add(Task.Run(() =>
                {
                    int start, last, step;
                    int items;
                    do
                    {
                        String output = String.Empty;
                        items = sc.Count;
                        if (!desc)
                        {
                            start = 1;
                            step = 1;
                            last = items;
                        }
                        else
                        {
                            start = items;
                            step = -1;
                            last = 1;
                        }

                        for (int index = start; desc ? index >= last : index <= last; index += step)
                            output += String.Format("[{0}] ", sc.Read(index));

                        Console.WriteLine("Task {0} read {1} items: {2}\n",
                                          Task.CurrentId, items, output);
                    } while (items < itemsWritten | itemsWritten == 0);
                }));
            }
            // Execute a red/update task.
            tasks.Add(Task.Run(() =>
            {
                Thread.Sleep(100);
                for (int ctr = 1; ctr <= sc.Count; ctr++)
                {
                    String value = sc.Read(ctr);
                    if (value == "cucumber")
                        if (sc.AddOrUpdate(ctr, "green bean") != TestReaderWriterLockSlim.AddOrUpdateStatus.Unchanged)
                            Console.WriteLine("Changed 'cucumber' to 'green bean'");
                }
            }));

            // Wait for all three tasks to complete.
            Task.WaitAll(tasks.ToArray());

            // Display the final contents of the cache.
            Console.WriteLine();
            Console.WriteLine("Values in synchronized cache: ");
            for (int ctr = 1; ctr <= sc.Count; ctr++)
                Console.WriteLine("   {0}: {1}", ctr, sc.Read(ctr));

        }

        public string Read(int key)
        {
            cacheLock.EnterReadLock();
            try
            {
                return innerCache[key];
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        public void Add(int key, string value)
        {
            cacheLock.EnterWriteLock();
            try
            {
                innerCache.Add(key, value);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public bool AddWithTimeout(int key, string value, int timeout)
        {
            if (cacheLock.TryEnterWriteLock(timeout))
            {
                try
                {
                    innerCache.Add(key, value);
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public AddOrUpdateStatus AddOrUpdate(int key, string value)
        {
            cacheLock.EnterUpgradeableReadLock();
            try
            {
                string result = null;
                if (innerCache.TryGetValue(key, out result))
                {
                    if (result == value)
                    {
                        return AddOrUpdateStatus.Unchanged;
                    }
                    else
                    {
                        cacheLock.EnterWriteLock();
                        try
                        {
                            innerCache[key] = value;
                        }
                        finally
                        {
                            cacheLock.ExitWriteLock();
                        }
                        return AddOrUpdateStatus.Updated;
                    }
                }
                else
                {
                    cacheLock.EnterWriteLock();
                    try
                    {
                        innerCache.Add(key, value);
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                    return AddOrUpdateStatus.Added;
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }

        public void Delete(int key)
        {
            cacheLock.EnterWriteLock();
            try
            {
                innerCache.Remove(key);
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        public enum AddOrUpdateStatus
        {
            Added,
            Updated,
            Unchanged
        };

        ~TestReaderWriterLockSlim()
        {
            if (cacheLock != null) cacheLock.Dispose();
        }
    }

    public class TestReaderWriterLock
    {
        static ReaderWriterLock rwl = new ReaderWriterLock();
        // Define the shared resource protected by the ReaderWriterLock.
        static int resource = 0;

        const int numThreads = 26;
        static bool running = true;
        static Random rnd = new Random();

        // Statistics.
        static int readerTimeouts = 0;
        static int writerTimeouts = 0;
        static int reads = 0;
        static int writes = 0;

        public static void TestMehtodReaderWriterLock()
        {
            // Start a series of threads to randomly read from and
            // write to the shared resource.
            Thread[] t = new Thread[numThreads];
            for (int i = 0; i < numThreads; i++)
            {
                t[i] = new Thread(new ThreadStart(ThreadProc));
                t[i].Name = new String(Convert.ToChar(i + 65), 1);
                t[i].Start();
                if (i > 10)
                    Thread.Sleep(300);
            }

            // Tell the threads to shut down and wait until they all finish.
            running = false;
            for (int i = 0; i < numThreads; i++)
                t[i].Join();

            // Display statistics.
            Console.WriteLine("\n{0} reads, {1} writes, {2} reader time-outs, {3} writer time-outs.",
                  reads, writes, readerTimeouts, writerTimeouts);
        }

        static void ThreadProc()
        {
            // Randomly select a way for the thread to read and write from the shared
            // resource.
            while (running)
            {
                double action = rnd.NextDouble();
                if (action < .8)
                    ReadFromResource(10);
                else if (action < .81)
                    ReleaseRestore(50);
                else if (action < .90)
                    UpgradeDowngrade(100);
                else
                    WriteToResource(100);
            }
        }

        // Request and release a reader lock, and handle time-outs.
        static void ReadFromResource(int timeOut)
        {
            try
            {
                rwl.AcquireReaderLock(timeOut);
                try
                {
                    // It is safe for this thread to read from the shared resource.
                    Display("reads resource value " + resource);
                    Interlocked.Increment(ref reads);
                }
                finally
                {
                    // Ensure that the lock is released.
                    rwl.ReleaseReaderLock();
                }
            }
            catch (ApplicationException)
            {
                // The reader lock request timed out.
                Interlocked.Increment(ref readerTimeouts);
            }
        }

        // Request and release the writer lock, and handle time-outs.
        static void WriteToResource(int timeOut)
        {
            try
            {
                rwl.AcquireWriterLock(timeOut);
                try
                {
                    // It's safe for this thread to access from the shared resource.
                    resource = rnd.Next(500);
                    Display("writes resource value " + resource);
                    Interlocked.Increment(ref writes);
                }
                finally
                {
                    // Ensure that the lock is released.
                    rwl.ReleaseWriterLock();
                }
            }
            catch (ApplicationException)
            {
                // The writer lock request timed out.
                Interlocked.Increment(ref writerTimeouts);
            }
        }

        // Requests a reader lock, upgrades the reader lock to the writer
        // lock, and downgrades it to a reader lock again.
        static void UpgradeDowngrade(int timeOut)
        {
            try
            {
                rwl.AcquireReaderLock(timeOut);
                try
                {
                    // It's safe for this thread to read from the shared resource.
                    Display("reads resource value " + resource);
                    Interlocked.Increment(ref reads);

                    // To write to the resource, either release the reader lock and
                    // request the writer lock, or upgrade the reader lock. Upgrading
                    // the reader lock puts the thread in the write queue, behind any
                    // other threads that might be waiting for the writer lock.
                    try
                    {
                        LockCookie lc = rwl.UpgradeToWriterLock(timeOut);
                        try
                        {
                            // It's safe for this thread to read or write from the shared resource.
                            resource = rnd.Next(500);
                            Display("writes resource value " + resource);
                            Interlocked.Increment(ref writes);
                        }
                        finally
                        {
                            // Ensure that the lock is released.
                            rwl.DowngradeFromWriterLock(ref lc);
                        }
                    }
                    catch (ApplicationException)
                    {
                        // The upgrade request timed out.
                        Interlocked.Increment(ref writerTimeouts);
                    }

                    // If the lock was downgraded, it's still safe to read from the resource.
                    Display("reads resource value " + resource);
                    Interlocked.Increment(ref reads);
                }
                finally
                {
                    // Ensure that the lock is released.
                    rwl.ReleaseReaderLock();
                }
            }
            catch (ApplicationException)
            {
                // The reader lock request timed out.
                Interlocked.Increment(ref readerTimeouts);
            }
        }

        // Release all locks and later restores the lock state.
        // Uses sequence numbers to determine whether another thread has
        // obtained a writer lock since this thread last accessed the resource.
        static void ReleaseRestore(int timeOut)
        {
            int lastWriter;

            try
            {
                rwl.AcquireReaderLock(timeOut);
                try
                {
                    // It's safe for this thread to read from the shared resource,
                    // so read and cache the resource value.
                    int resourceValue = resource;     // Cache the resource value.
                    Display("reads resource value " + resourceValue);
                    Interlocked.Increment(ref reads);

                    // Save the current writer sequence number.
                    lastWriter = rwl.WriterSeqNum;

                    // Release the lock and save a cookie so the lock can be restored later.
                    LockCookie lc = rwl.ReleaseLock();

                    // Wait for a random interval and then restore the previous state of the lock.
                    Thread.Sleep(rnd.Next(250));
                    rwl.RestoreLock(ref lc);

                    // Check whether other threads obtained the writer lock in the interval.
                    // If not, then the cached value of the resource is still valid.
                    if (rwl.AnyWritersSince(lastWriter))
                    {
                        resourceValue = resource;
                        Interlocked.Increment(ref reads);
                        Display("resource has changed " + resourceValue);
                    }
                    else
                    {
                        Display("resource has not changed " + resourceValue);
                    }
                }
                finally
                {
                    // Ensure that the lock is released.
                    rwl.ReleaseReaderLock();
                }
            }
            catch (ApplicationException)
            {
                // The reader lock request timed out.
                Interlocked.Increment(ref readerTimeouts);
            }
        }

        // Helper method briefly displays the most recent thread action.
        static void Display(string msg)
        {
            Console.Write("Thread {0} {1}.       \r", Thread.CurrentThread.Name, msg);
        }
    }
    class TestBarrier
    {
        // Demonstrates:
        //      Barrier constructor with post-phase action
        //      Barrier.AddParticipants()
        //      Barrier.RemoveParticipant()
        //      Barrier.SignalAndWait(), incl. a BarrierPostPhaseException being thrown
        public static void TestMethodBarrier()
        {
            int count = 0;

            // Create a barrier with three participants
            // Provide a post-phase action that will print out certain information
            // And the third time through, it will throw an exception
            Barrier barrier = new Barrier(3, (b) =>
            {
                Console.WriteLine("Post-Phase action: count={0}, phase={1}", count, b.CurrentPhaseNumber);
                if (b.CurrentPhaseNumber == 2) throw new Exception("D'oh!");
            });

            // Nope -- changed my mind.  Let's make it five participants.
            barrier.AddParticipants(2);

            // Nope -- let's settle on four participants.
            barrier.RemoveParticipant();


            // This is the logic run by all participants
            Action action = () =>
            {
                Interlocked.Increment(ref count);
                barrier.SignalAndWait(); // during the post-phase action, count should be 4 and phase should be 0
                Interlocked.Increment(ref count);
                barrier.SignalAndWait(); // during the post-phase action, count should be 8 and phase should be 1

                // The third time, SignalAndWait() will throw an exception and all participants will see it
                Interlocked.Increment(ref count);
                try
                {
                    barrier.SignalAndWait();
                }
                catch (BarrierPostPhaseException bppe)
                {
                    Console.WriteLine("Caught BarrierPostPhaseException: {0}", bppe.Message);
                }

                // The fourth time should be hunky-dory
                Interlocked.Increment(ref count);
                barrier.SignalAndWait(); // during the post-phase action, count should be 16 and phase should be 3
            };

            // Now launch 4 parallel actions to serve as 4 participants
            Parallel.Invoke(action, action, action, action);

            // This (5 participants) would cause an exception:
            // Parallel.Invoke(action, action, action, action, action);
            //      "System.InvalidOperationException: The number of threads using the barrier
            //      exceeded the total number of registered participants."

            // It's good form to Dispose() a barrier when you're done with it.
            barrier.Dispose();

        }

    }

    class TestInterlocked
    {
        //0 for false, 1 for true.
        private static int usingResource = 0;

        private const int numThreadIterations = 5;
        private const int numThreads = 10;

        static void TestMethodInterlocked()
        {
            Thread myThread;
            Random rnd = new Random();

            for (int i = 0; i < numThreads; i++)
            {
                myThread = new Thread(new ThreadStart(MyThreadProc));
                myThread.Name = String.Format("Thread{0}", i + 1);

                //Wait a random amount of time before starting next thread.
                Thread.Sleep(rnd.Next(0, 1000));
                myThread.Start();
            }
        }

        private static void MyThreadProc()
        {
            for (int i = 0; i < numThreadIterations; i++)
            {
                UseResource();

                //Wait 1 second before next attempt.
                Thread.Sleep(1000);
            }
        }

        //A simple method that denies reentrancy.
        static bool UseResource()
        {
            //0 indicates that the method is not in use.
            if (0 == Interlocked.Exchange(ref usingResource, 1))
            {
                Console.WriteLine("{0} acquired the lock", Thread.CurrentThread.Name);

                //Code to access a resource that is not thread safe would go here.

                //Simulate some work
                Thread.Sleep(500);

                Console.WriteLine("{0} exiting lock", Thread.CurrentThread.Name);

                //Release the lock
                Interlocked.Exchange(ref usingResource, 0);
                return true;
            }
            else
            {
                Console.WriteLine("   {0} was denied the lock", Thread.CurrentThread.Name);
                return false;
            }
        }

    }

    /*
     For an example of how to use a Spin Lock, see How to: Use SpinLock for Low-Level Synchronization.
    Spin locks can be used for leaf-level locks where the object allocation implied by using a Monitor, in size or due to garbage collection pressure, is overly expensive.A spin lock can be useful in to avoid blocking; however, if you expect a significant amount of blocking, you should probably not use spin locks due to excessive spinning.Spinning can be beneficial when locks are fine-grained and large in number(for example, a lock per node in a linked list) and also when lock hold-times are always extremely short. In general, while holding a spin lock, one should avoid any of these actions:
    •blocking, 
    •calling anything that itself may block,
    •holding more than one spin lock at once,
    •making dynamically dispatched calls(interface and virtuals), 
    •making statically dispatched calls into any code one doesn't own, or 
    •allocating memory.
    SpinLock should only be used after you have been determined that doing so will improve an application's performance. It is also important to note that SpinLock is a value type, for performance reasons. For this reason, you must be very careful not to accidentally copy a SpinLock instance, as the two instances (the original and the copy) would then be completely independent of one another, which would likely lead to erroneous behavior of the application. If a SpinLock instance must be passed around, it should be passed by reference rather than by value. 
    Do not store SpinLock instances in readonly fields.
    */
    public class TestSpinWait<T>
    {
        private volatile Node m_head;

        private class Node { public Node Next; public T Value; }

        public void Push(T item)
        {
            var spin = new SpinWait();
            Node node = new Node { Value = item }, head;
            while (true)
            {
                head = m_head;
                node.Next = head;
                if (Interlocked.CompareExchange(ref m_head, node, head) == head) break;
                spin.SpinOnce();
            }
        }

        public bool TryPop(out T result)
        {
            result = default(T);
            var spin = new SpinWait();

            Node head;
            while (true)
            {
                head = m_head;
                if (head == null) return false;
                if (Interlocked.CompareExchange(ref m_head, head.Next, head) == head)
                {
                    result = head.Value;
                    return true;
                }
                spin.SpinOnce();
            }
        }
    }

    class TestSpinWait
    {
        private static int _count = 1000;
        private static int _timeout_ms = 10;

        public static void TestMethodSpinWait()
        {
            //NoSleep();  
            ThreadSleepInThread();
            SpinWaitInThread();
        }

        private static void NoSleep()
        {
            Thread thread = new Thread(() =>
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < _count; i++)
                {

                }
                Console.WriteLine("No Sleep Consume Time:{0}", sw.Elapsed.ToString());
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private static void ThreadSleepInThread()
        {
            Thread thread = new Thread(() =>
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < _count; i++)
                {
                    Thread.Sleep(_timeout_ms);
                }
                Console.WriteLine("Thread Sleep Consume Time:{0}", sw.Elapsed.ToString());
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private static void SpinWaitInThread()
        {
            Thread thread = new Thread(() =>
            {
                var sw = Stopwatch.StartNew();
                for (int i = 0; i < _count; i++)
                {
                    SpinWait.SpinUntil(() => false, _timeout_ms);
                }
                Console.WriteLine("SpinWait Consume Time:{0}", sw.Elapsed.ToString());
            });
            thread.IsBackground = true;
            thread.Start();
        }
    }


    /* 
     In this example, the critical section performs a minimal amount of work, which makes it a good candidate for a SpinLock. 
     Increasing the work a small amount increases the performance of the SpinLock compared to a standard lock. However,
     there is a point at which a SpinLock becomes more expensive than a standard lock. 
     You can use the concurrency profiling functionality in the profiling tools to see
     which type of lock provides better performance in your program. For more information, see Concurrency Visualizer.
     SpinLock might be useful when a lock on a shared resource is not going to be held for very long. In such cases, on multi-core computers it can be efficient for the blocked thread to spin for a few cycles until the lock is released. By spinning, the thread does not become blocked, which is a CPU-intensive process. SpinLock will stop spinning under certain conditions to prevent starvation of logical processors or priority inversion on systems with Hyper-Threading.
    This example uses the System.Collections.Generic.Queue<T> class, which requires user synchronization for multi-threaded access. In applications that target the .NET Framework version 4, another option is to use the System.Collections.Concurrent.ConcurrentQueue<T>, which does not require any user locks.
    Note the use of false (False in Visual Basic) in the call to Exit. This provides the best performance. Specify true (True)on IA64 architectures to use the memory fence, which flushes the write buffers to ensure that the lock is now available for other threads to exit.
     */
    class TestSpinLock
    {
        const int N = 100000;
        static Queue<Data> _queue = new Queue<Data>();
        static object _lock = new Object();
        static SpinLock _spinlock = new SpinLock();

        class Data
        {
            public string Name { get; set; }
            public double Number { get; set; }
        }
        public static void TestMethodSpinLock()
        {

            // First use a standard lock for comparison purposes.
            UseLock();
            _queue.Clear();
            UseSpinLock();
        }

        private static void UpdateWithSpinLock(Data d, int i)
        {
            bool lockTaken = false;
            try
            {
                _spinlock.Enter(ref lockTaken);
                _queue.Enqueue(d);
            }
            finally
            {
                if (lockTaken) _spinlock.Exit(false);
            }
        }

        private static void UseSpinLock()
        {

            Stopwatch sw = Stopwatch.StartNew();

            Parallel.Invoke(
                    () =>
                    {
                        for (int i = 0; i < N; i++)
                        {
                            UpdateWithSpinLock(new Data() { Name = i.ToString(), Number = i }, i);
                        }
                    },
                    () =>
                    {
                        for (int i = 0; i < N; i++)
                        {
                            UpdateWithSpinLock(new Data() { Name = i.ToString(), Number = i }, i);
                        }
                    }
                );
            sw.Stop();
            Console.WriteLine("elapsed ms with spinlock: {0}", sw.ElapsedMilliseconds);
        }

        static void UpdateWithLock(Data d, int i)
        {
            lock (_lock)
            {
                _queue.Enqueue(d);
            }
        }

        private static void UseLock()
        {
            Stopwatch sw = Stopwatch.StartNew();

            Parallel.Invoke(
                    () =>
                    {
                        for (int i = 0; i < N; i++)
                        {
                            UpdateWithLock(new Data() { Name = i.ToString(), Number = i }, i);
                        }
                    },
                    () =>
                    {
                        for (int i = 0; i < N; i++)
                        {
                            UpdateWithLock(new Data() { Name = i.ToString(), Number = i }, i);
                        }
                    }
                );
            sw.Stop();
            Console.WriteLine("elapsed ms with lock: {0}", sw.ElapsedMilliseconds);
        }
    }

    class TestCountdownEvent
    {
        public static void TestMethodCountdownEvent()
        {
            // Initialize a queue and a CountdownEvent
            ConcurrentQueue<int> queue = new ConcurrentQueue<int>(Enumerable.Range(0, 10000));
            CountdownEvent cde = new CountdownEvent(10000); // initial count = 10000

            // This is the logic for all queue consumers
            Action consumer = () =>
            {
                int local;
                // decrement CDE count once for each element consumed from queue
                while (queue.TryDequeue(out local)) cde.Signal();
            };

            // Now empty the queue with a couple of asynchronous tasks
            Task t1 = Task.Factory.StartNew(consumer);
            Task t2 = Task.Factory.StartNew(consumer);

            // And wait for queue to empty by waiting on cde
            cde.Wait(); // will return when cde count reaches 0

            Console.WriteLine("Done emptying queue.  InitialCount={0}, CurrentCount={1}, IsSet={2}",
                cde.InitialCount, cde.CurrentCount, cde.IsSet);

            // Proper form is to wait for the tasks to complete, even if you that their work
            // is done already.
            Task.WaitAll(t1, t2);

            // Resetting will cause the CountdownEvent to un-set, and resets InitialCount/CurrentCount
            // to the specified value
            cde.Reset(10);

            // AddCount will affect the CurrentCount, but not the InitialCount
            cde.AddCount(2);

            Console.WriteLine("After Reset(10), AddCount(2): InitialCount={0}, CurrentCount={1}, IsSet={2}",
                cde.InitialCount, cde.CurrentCount, cde.IsSet);

            // Now try waiting with cancellation
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Cancel(); // cancels the CancellationTokenSource
            try
            {
                cde.Wait(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("cde.Wait(preCanceledToken) threw OCE, as expected");
            }
            finally
            {
                cts.Dispose();
            }
            // It's good for to release a CountdownEvent when you're done with it.
            cde.Dispose();
        }
    }

    public class TestManualResetEvent
    {
        // mre is used to block and release threads manually. It is
        // created in the unsignaled state.
        private static ManualResetEvent mre = new ManualResetEvent(false);

        public static void TestMethodManualResetEvent()
        {
            Console.WriteLine("\nStart 3 named threads that block on a ManualResetEvent:\n");

            for (int i = 0; i <= 2; i++)
            {
                Thread t = new Thread(ThreadProc);
                t.Name = "Thread_" + i;
                t.Start();
            }

            Thread.Sleep(500);
            Console.WriteLine("\nWhen all three threads have started, press Enter to call Set()" +
                              "\nto release all the threads.\n");
            Console.ReadLine();

            mre.Set();

            Thread.Sleep(500);
            Console.WriteLine("\nWhen a ManualResetEvent is signaled, threads that call WaitOne()" +
                              "\ndo not block. Press Enter to show this.\n");
            Console.ReadLine();

            for (int i = 3; i <= 4; i++)
            {
                Thread t = new Thread(ThreadProc);
                t.Name = "Thread_" + i;
                t.Start();
            }

            Thread.Sleep(500);
            Console.WriteLine("\nPress Enter to call Reset(), so that threads once again block" +
                              "\nwhen they call WaitOne().\n");
            Console.ReadLine();

            mre.Reset();

            // Start a thread that waits on the ManualResetEvent.
            Thread t5 = new Thread(ThreadProc);
            t5.Name = "Thread_5";
            t5.Start();

            Thread.Sleep(500);
            Console.WriteLine("\nPress Enter to call Set() and conclude the demo.");
            Console.ReadLine();

            mre.Set();

            // If you run this example in Visual Studio, uncomment the following line:
            //Console.ReadLine();
        }


        private static void ThreadProc()
        {
            string name = Thread.CurrentThread.Name;

            Console.WriteLine(name + " starts and calls mre.WaitOne()");

            mre.WaitOne();

            Console.WriteLine(name + " ends.");
        }
    }

    class TestManualResetEventSlim
    {

        public static void TestMethodManualResetEventSlim()
        {
            MRES_SetWaitReset();
            MRES_SpinCountWaitHandle();
        }
        // Demonstrates:
        //      ManualResetEventSlim construction
        //      ManualResetEventSlim.Wait()
        //      ManualResetEventSlim.Set()
        //      ManualResetEventSlim.Reset()
        //      ManualResetEventSlim.IsSet
        static void MRES_SetWaitReset()
        {
            ManualResetEventSlim mres1 = new ManualResetEventSlim(false); // initialize as unsignaled
            ManualResetEventSlim mres2 = new ManualResetEventSlim(false); // initialize as unsignaled
            ManualResetEventSlim mres3 = new ManualResetEventSlim(true);  // initialize as signaled

            // Start an asynchronous Task that manipulates mres3 and mres2
            var observer = Task.Factory.StartNew(() =>
            {
                mres1.Wait();
                Console.WriteLine("observer sees signaled mres1!");
                Console.WriteLine("observer resetting mres3...");
                mres3.Reset(); // should switch to unsignaled
                Console.WriteLine("observer signalling mres2");
                mres2.Set();
            });

            Console.WriteLine("main thread: mres3.IsSet = {0} (should be true)", mres3.IsSet);
            Console.WriteLine("main thread signalling mres1");
            mres1.Set(); // This will "kick off" the observer Task
            mres2.Wait(); // This won't return until observer Task has finished resetting mres3
            Console.WriteLine("main thread sees signaled mres2!");
            Console.WriteLine("main thread: mres3.IsSet = {0} (should be false)", mres3.IsSet);

            // It's good form to Dispose() a ManualResetEventSlim when you're done with it
            observer.Wait(); // make sure that this has fully completed
            mres1.Dispose();
            mres2.Dispose();
            mres3.Dispose();
        }

        // Demonstrates:
        //      ManualResetEventSlim construction w/ SpinCount
        //      ManualResetEventSlim.WaitHandle
        static void MRES_SpinCountWaitHandle()
        {
            // Construct a ManualResetEventSlim with a SpinCount of 1000
            // Higher spincount => longer time the MRES will spin-wait before taking lock
            ManualResetEventSlim mres1 = new ManualResetEventSlim(false, 1000);
            ManualResetEventSlim mres2 = new ManualResetEventSlim(false, 1000);

            Task bgTask = Task.Factory.StartNew(() =>
            {
                // Just wait a little
                Thread.Sleep(100);

                // Now signal both MRESes
                Console.WriteLine("Task signalling both MRESes");
                mres1.Set();
                mres2.Set();
            });

            // A common use of MRES.WaitHandle is to use MRES as a participant in 
            // WaitHandle.WaitAll/WaitAny.  Note that accessing MRES.WaitHandle will
            // result in the unconditional inflation of the underlying ManualResetEvent.
            WaitHandle.WaitAll(new WaitHandle[] { mres1.WaitHandle, mres2.WaitHandle });
            Console.WriteLine("WaitHandle.WaitAll(mres1.WaitHandle, mres2.WaitHandle) completed.");

            // Clean up
            bgTask.Wait();
            mres1.Dispose();
            mres2.Dispose();
        }
    }


    public class TestSemaphore
    {
        // A semaphore that simulates a limited resource pool.
        //
        private static Semaphore _pool;

        // A padding interval to make the output more orderly.
        private static int _padding;

        public static void TestMethodSemaphore()
        {
            // Create a semaphore that can satisfy up to three
            // concurrent requests. Use an initial count of zero,
            // so that the entire semaphore count is initially
            // owned by the main program thread.
            //
            _pool = new Semaphore(0, 3);

            // Create and start five numbered threads. 
            //
            for (int i = 1; i <= 5; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Worker));

                // Start the thread, passing the number.
                //
                t.Start(i);
            }

            // Wait for half a second, to allow all the
            // threads to start and to block on the semaphore.
            //
            Thread.Sleep(500);

            // The main thread starts out holding the entire
            // semaphore count. Calling Release(3) brings the 
            // semaphore count back to its maximum value, and
            // allows the waiting threads to enter the semaphore,
            // up to three at a time.
            //
            Console.WriteLine("Main thread calls Release(3).");
            _pool.Release(3);

            Console.WriteLine("Main thread exits.");
        }

        private static void Worker(object num)
        {
            // Each worker thread begins by requesting the
            // semaphore.
            Console.WriteLine("Thread {0} begins " +
                "and waits for the semaphore.", num);
            _pool.WaitOne();

            // A padding interval to make the output more orderly.
            int padding = Interlocked.Add(ref _padding, 100);

            Console.WriteLine("Thread {0} enters the semaphore.", num);

            // The thread's "work" consists of sleeping for 
            // about a second. Each thread "works" a little 
            // longer, just to make the output more orderly.
            //
            Thread.Sleep(1000 + padding);

            Console.WriteLine("Thread {0} releases the semaphore.", num);
            Console.WriteLine("Thread {0} previous semaphore count: {1}",
                num, _pool.Release());
        }
    }

    public class TestSemaphoreSlim
    {
        private static SemaphoreSlim semaphore;
        // A padding interval to make the output more orderly.
        private static int padding;

        public static void TestMethodSemaphoreSlim()
        {
            // Create the semaphore.
            semaphore = new SemaphoreSlim(0, 3);
            Console.WriteLine("{0} tasks can enter the semaphore.",
                              semaphore.CurrentCount);
            Task[] tasks = new Task[5];

            // Create and start five numbered tasks.
            for (int i = 0; i <= 4; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    // Each task begins by requesting the semaphore.
                    Console.WriteLine("Task {0} begins and waits for the semaphore.",
                                      Task.CurrentId);
                    semaphore.Wait();

                    Interlocked.Add(ref padding, 100);

                    Console.WriteLine("Task {0} enters the semaphore.", Task.CurrentId);

                    // The task just sleeps for 1+ seconds.
                    Thread.Sleep(1000 + padding);

                    Console.WriteLine("Task {0} releases the semaphore; previous count: {1}.",
                                      Task.CurrentId, semaphore.Release());
                });
            }

            // Wait for half a second, to allow all the tasks to start and block.
            Thread.Sleep(500);

            // Restore the semaphore count to its maximum value.
            Console.Write("Main thread calls Release(3) --> ");
            semaphore.Release(3);
            Console.WriteLine("{0} tasks can enter the semaphore.",
                              semaphore.CurrentCount);
            // Main thread waits for the tasks to complete.
            Task.WaitAll(tasks);

            Console.WriteLine("Main thread exits.");
        }
    }

    public class TestMonitor
    {
        //public static void TestMethodMonitor()
        //{

        //    int nTasks = 0;
        //    List<Task> tasks = new List<Task>();
        //    try
        //    {
        //        for (int ctr = 0; ctr < 10; ctr++)
        //            tasks.Add(Task.Run(() => { // Instead of doing some work, just sleep.
        //                Thread.Sleep(250);
        //                // Increment the number of tasks.
        //                Monitor.Enter(nTasks);
        //                try
        //                {
        //                    nTasks += 1;
        //                }
        //                finally
        //                {
        //                    Monitor.Exit(nTasks);
        //                }
        //            }));
        //        Task.WaitAll(tasks.ToArray());
        //        Console.WriteLine("{0} tasks started and executed.", nTasks);
        //    }
        //    catch (AggregateException e)
        //    {
        //        String msg = String.Empty;
        //        foreach (var ie in e.InnerExceptions)
        //        {
        //            Console.WriteLine("{0}", ie.GetType().Name);
        //            if (!msg.Contains(ie.Message))
        //                msg += ie.Message + Environment.NewLine;
        //        }
        //        Console.WriteLine("\nException Message(s):");
        //        Console.WriteLine(msg);
        //    }
        //}

        public static void TestMethodMonitor()
        {

            int nTasks = 0;
            object o = nTasks;
            List<Task> tasks = new List<Task>();

            try
            {
                for (int ctr = 0; ctr < 10; ctr++)
                    tasks.Add(Task.Run(() =>
                    { // Instead of doing some work, just sleep.
                        Thread.Sleep(250);
                        // Increment the number of tasks.
                        Monitor.Enter(o);
                        try
                        {
                            nTasks++;
                        }
                        finally
                        {
                            Monitor.Exit(o);
                        }
                    }));
                Task.WaitAll(tasks.ToArray());
                Console.WriteLine("{0} tasks started and executed.", nTasks);
            }
            catch (AggregateException e)
            {
                String msg = String.Empty;
                foreach (var ie in e.InnerExceptions)
                {
                    Console.WriteLine("{0}", ie.GetType().Name);
                    if (!msg.Contains(ie.Message))
                        msg += ie.Message + Environment.NewLine;
                }
                Console.WriteLine("\nException Message(s):");
                Console.WriteLine(msg);
            }
        }

    }

    internal class SyncResource
    {
        // Use a monitor to enforce synchronization.
        public void Access()
        {
            lock (this)
            {
                Console.WriteLine("Staring synchronized resource access on thread #{0}",
                                  Thread.CurrentThread.ManagedThreadId);
                if (Thread.CurrentThread.ManagedThreadId % 2 == 0)
                    Thread.Sleep(2000);

                Thread.Sleep(200);
                Console.WriteLine("Stopping synchronized resource access on thread #{0}",
                                  Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
    internal class UnSyncResource
    {
        // Do not enforce synchronization.
        public void Access()
        {
            Console.WriteLine("Starting unsynchronized esource access on Thread #{0}",
                              Thread.CurrentThread.ManagedThreadId);
            if (Thread.CurrentThread.ManagedThreadId % 2 == 0)
                Thread.Sleep(2000);

            Thread.Sleep(200);
            Console.WriteLine("Stopping unsynchronized resource access on thread #{0}",
                              Thread.CurrentThread.ManagedThreadId);
        }
    }
    public class TestMonitor2
    {
        private static int numOps;
        private static AutoResetEvent opsAreDone = new AutoResetEvent(false);
        private static SyncResource SyncRes = new SyncResource();
        private static UnSyncResource UnSyncRes = new UnSyncResource();
        public static void TestMethodMonitor()
        {
            // Set the number of synchronized calls.
            numOps = 5;
            for (int ctr = 0; ctr <= 4; ctr++)
                ThreadPool.QueueUserWorkItem(new WaitCallback(SyncUpdateResource));

            // Wait until this WaitHandle is signaled.
            opsAreDone.WaitOne();
            Console.WriteLine("\t\nAll synchronized operations have completed.\n");

            // Reset the count for unsynchronized calls.
            numOps = 5;
            for (int ctr = 0; ctr <= 4; ctr++)
                ThreadPool.QueueUserWorkItem(new WaitCallback(UnSyncUpdateResource));

            // Wait until this WaitHandle is signaled.
            opsAreDone.WaitOne();
            Console.WriteLine("\t\nAll unsynchronized thread operations have completed.\n");
        }
        static void SyncUpdateResource(Object state)
        {
            // Call the internal synchronized method.
            SyncRes.Access();

            // Ensure that only one thread can decrement the counter at a time.
            if (Interlocked.Decrement(ref numOps) == 0)
                // Announce to Main that in fact all thread calls are done.
                opsAreDone.Set();
        }
        static void UnSyncUpdateResource(Object state)
        {
            // Call the unsynchronized method.
            UnSyncRes.Access();

            // Ensure that only one thread can decrement the counter at a time.
            if (Interlocked.Decrement(ref numOps) == 0)
                // Announce to Main that in fact all thread calls are done.
                opsAreDone.Set();
        }
    }


    class TestMutex
    {
        // Create a new Mutex. The creating thread does not own the mutex.
        private static Mutex mut = new Mutex();
        private const int numIterations = 1;
        private const int numThreads = 3;

        public static void TesMethodtMutex()
        {
            TestMutex ex = new TestMutex();
            ex.StartThreads();
        }

        private void StartThreads()
        {
            // Create the threads that will use the protected resource.
            for (int i = 0; i < numThreads; i++)
            {
                Thread newThread = new Thread(new ThreadStart(ThreadProc));
                newThread.Name = String.Format("Thread{0}", i + 1);
                newThread.Start();
            }

            // The main thread returns to Main and exits, but the application continues to
            // run until all foreground threads have exited.
        }

        private static void ThreadProc()
        {
            for (int i = 0; i < numIterations; i++)
            {
                UseResource();
            }
        }

        // This method represents a resource that must be synchronized
        // so that only one thread at a time can enter.
        private static void UseResource()
        {
            // Wait until it is safe to enter, and do not enter if the request times out.
            Console.WriteLine("{0} is requesting the mutex", Thread.CurrentThread.Name);
            if (mut.WaitOne(1000))
            {
                Console.WriteLine("{0} has entered the protected area",
                    Thread.CurrentThread.Name);

                // Place code to access non-reentrant resources here.

                // Simulate some work.
                Thread.Sleep(5000);

                Console.WriteLine("{0} is leaving the protected area",
                    Thread.CurrentThread.Name);

                // Release the Mutex.
                mut.ReleaseMutex();
                Console.WriteLine("{0} has released the mutex",
                                  Thread.CurrentThread.Name);
            }
            else
            {
                Console.WriteLine("{0} will not acquire the mutex",
                                  Thread.CurrentThread.Name);
            }
        }

        ~TestMutex()
        {
            mut.Dispose();
        }
    }

    #region TestVolatile
    /*
    http://m.blog.csdn.net/article/details?id=5796471
    http://stackoverflow.com/questions/19382705/c-sharp-volatile-keyword-usage-vs-lock
    I've used volatile where I'm not sure it is necessary.

    Let me be very clear on this point:

    If you are not 100% clear on what volatile means in C# then do not use it. It is a sharp tool that is meant to be used by experts only. If you cannot describe what all the possible reorderings of memory accesses are allowed by a weak memory model architecture when two threads are reading and writing two different volatile fields then you do not know enough to use volatile safely and you will make mistakes, as you have done here, and write a program that is extremely brittle.


    I was pretty sure a lock would be overkill in my situation

    First off, the best solution is to simply not go there. If you don't write multithreaded code that tries to share memory then you don't have to worry about locking, which is hard to get correct.

    If you must write multithreaded code that shares memory, then the best practice is to always use locks. Locks are almost never overkill. The price of an uncontended lock is on the order of ten nanoseconds. Are you really telling me that ten extra nanoseconds will make a difference to your user? If so, then you have a very, very fast program and a user with unusually high standards. 

    The price of a contended lock is of course arbitrarily high if the code inside the lock is expensive. Do not do expensive work inside a lock, so that the probability of contention is low.

    Only when you have a demonstrated performance problem with locks that cannot be solved by removing contention should you even begin to consider a low-lock solution.


    I added "volatile" to make sure that there is no misalignment occurring: reading only 32 bits of the variable and the other 32 bits on another fetch which can be broken in two by a write in the middle from another thread.

    This sentence tells me that you need to stop writing multithreaded code right now. Multithreaded code, particularly low-lock code, is for experts only. You have to understand how the system actually works before you start writing multithreaded code again. Get a good book on the subject and study hard.

    Your sentence is nonsensical because:

    First off, integers already are only 32 bits.

    Second, int accesses are guaranteed by the specification to be atomic! If you want atomicity, you've already got it.

    Third, yes, it is true that volatile accesses are always atomic, but that is not because C# makes all volatile accesses into atomic accesses! Rather, C# makes it illegal to put volatile on a field unless the field is already atomic. 

    Fourth, the purpose of volatile is to prevent the C# compiler, jitter and CPU from making certain optimizations that would change the meaning of your program in a weak memory model. Volatile in particular does not make ++ atomic. (I work for a company that makes static analyzers; I will use your code as a test case for our "incorrect non-atomic operation on volatile field" checker. It is very helpful to me to get real-world code that is full of realistic mistakes; we want to make sure that we are actually finding the bugs that people write, so thanks for posting this.)

    Looking at your actual code: volatile is, as Hans pointed out, totally inadequate to make your code correct. The best thing to do is what I said before: do not allow these methods to be called on any thread other than the main thread. That the counter logic is wrong should be the least of your worries. What makes the serialization thread safe if code on another thread is modifying the fields of the object while it is being serialized? That is the problem you should be worried about first.
    */

    public class TestVolatile
    {
        // This method is called when the thread is started.
        public void DoWork()
        {
            while (!_shouldStop)
            {
                Console.WriteLine("Worker thread: working...");
            }
            Console.WriteLine("Worker thread: terminating gracefully.");
        }
        public void RequestStop()
        {
            _shouldStop = true;
        }
        // Keyword volatile is used as a hint to the compiler that this data
        // member is accessed by multiple threads.
        private volatile bool _shouldStop;
    }
    public class TestVolatileWorkerThread
    {
        public static void TestMethodVolatile()
        {
            // Create the worker thread object. This does not start the thread.
            TestVolatile workerObject = new TestVolatile();
            Thread workerThread = new Thread(workerObject.DoWork);

            // Start the worker thread.
            workerThread.Start();
            Console.WriteLine("Main thread: starting worker thread...");

            // Loop until the worker thread activates.
            while (!workerThread.IsAlive) ;

            // Put the main thread to sleep for 1 millisecond to
            // allow the worker thread to do some work.
            Thread.Sleep(1);

            // Request that the worker thread stop itself.
            workerObject.RequestStop();

            // Use the Thread.Join method to block the current thread 
            // until the object's thread terminates.
            workerThread.Join();
            Console.WriteLine("Main thread: worker thread has terminated.");
        }
        // Sample output:
        // Main thread: starting worker thread...
        // Worker thread: working...
        // Worker thread: working...
        // Worker thread: working...
        // Worker thread: working...
        // Worker thread: working...
        // Worker thread: working...
        // Worker thread: terminating gracefully.
        // Main thread: worker thread has terminated.
    }
    #endregion TestVolatile

    class TestHashtable
    {
        Hashtable ht = Hashtable.Synchronized(new Hashtable());
    }
}
