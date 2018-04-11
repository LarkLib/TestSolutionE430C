using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    public class QueryPerfCounterTest
    {
        public void ExecuteTest()
        {
            RunTestValidateQueryPerfCounter();
            RunTestBoxingOverhead();
            RunTestStringConcatenation(10);
            RunTestStringConcatenation(100);
        }

        public static void RunTestValidateQueryPerfCounter()
        {
            int iterations = 5;

            // Call the object and methods to JIT before the test run
            QueryPerfCounter myTimer = new QueryPerfCounter();
            myTimer.Start();
            myTimer.Stop();

            // Time the overall test duration
            DateTime dtStartTime = DateTime.Now;

            // Use QueryPerfCounters to get the average time per iteration
            myTimer.Start();

            for (int i = 0; i < iterations; i++)
            {
                // Method to time
                System.Threading.Thread.Sleep(1000);
            }
            myTimer.Stop();

            // Calculate time per iteration in nanoseconds
            double result = myTimer.Duration(iterations);

            // Show the average time per iteration results
            Console.WriteLine("Iterations: {0}", iterations);
            Console.WriteLine("Average time per iteration: ");
            Console.WriteLine(result / 1000000000 + " seconds");
            Console.WriteLine(result / 1000000 + " milliseconds");
            Console.WriteLine(result + " nanoseconds");

            // Show the overall test duration results
            DateTime dtEndTime = DateTime.Now;
            Double duration = ((TimeSpan)(dtEndTime - dtStartTime)).TotalMilliseconds;
            Console.WriteLine();
            Console.WriteLine("Duration of test run: ");
            Console.WriteLine(duration / 1000 + " seconds");
            Console.WriteLine(duration + " milliseconds");
            Console.ReadLine();
        }
        public static void RunTestBoxingOverhead()
        {
            int iterations = 10000;

            // Call the object and methods to JIT before the test run
            QueryPerfCounter myTimer = new QueryPerfCounter();
            myTimer.Start();
            myTimer.Stop();

            // variables used for boxing/unboxing
            object obj = null;
            int value1 = 12;
            int value2 = 0;

            // Measure without boxing
            myTimer.Start();

            for (int i = 0; i < iterations; i++)
            {
                // a simple value copy of an integer to another integer
                value2 = value1;
            }
            myTimer.Stop();

            // Calculate time per iteration in nanoseconds
            double result = myTimer.Duration(iterations);
            Console.WriteLine("int to int (no boxing): " + result + " nanoseconds");

            // Measure boxing
            myTimer.Start();

            for (int i = 0; i < iterations; i++)
            {
                // point the object to a copy of the integer
                obj = value1;
            }
            myTimer.Stop();

            // Calculate time per iteration in nanoseconds
            result = myTimer.Duration(iterations);
            Console.WriteLine("int to object (boxing): " + result + " nanoseconds");

            // Measure unboxing
            myTimer.Start();

            for (int i = 0; i < iterations; i++)
            {
                // copy the integer value from the object to a second integer
                value2 = (int)obj;
            }
            myTimer.Stop();

            // Calculate time per iteration in nanoseconds
            result = myTimer.Duration(iterations);
            Console.WriteLine("object to int (unboxing): " + result + " nanoseconds");
            Console.ReadLine();
        }
        public static void RunTestStringConcatenation(int iterations)
        {
            // Call the object and methods to JIT before the test run
            QueryPerfCounter myTimer = new QueryPerfCounter();
            myTimer.Start();
            myTimer.Stop();


            Console.WriteLine("");
            Console.WriteLine("Iterations = " + iterations.ToString());
            Console.WriteLine("(Time shown is in nanoseconds)");

            // Measure StringBuilder performance
            StringBuilder sb = new StringBuilder("");
            myTimer.Start();
            for (int i = 0; i < iterations; i++)
            {
                sb.Append(i.ToString());
            }

            myTimer.Stop();

            // Pass in 1 for iterations to calculate overall duration
            double result = myTimer.Duration(1);
            Console.WriteLine(result + " StringBuilder version");

            // Measure string concatenation
            string s = string.Empty;
            myTimer.Start();
            for (int i = 0; i < iterations; i++)
            {
                s += i.ToString();
            }

            myTimer.Stop();

            // Pass in 1 for iterations to calculate overall duration
            result = myTimer.Duration(1);
            Console.WriteLine(result + " string concatenation version");
            Console.ReadLine();
        }
    }
}
