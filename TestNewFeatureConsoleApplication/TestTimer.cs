using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    class TestTimer : INewFeature
    {
        public void ExecuteTest()
        {
            TestAccurateTimer();
            TestMethodQueryPerfCounter();
            TestMethodTimer();
            TestMethodStopwatch();
        }

        #region TestTimer
        [DllImport("kernel32")]
        static extern uint GetTickCount();

        //常用于多媒体定时器中，与GetTickCount类似，也是返回操作系统启动到现在所经过的毫秒数，精度为1毫秒。
        //一般默认的精度不止1毫秒（不同操作系统有所不同），需要调用timeBeginPeriod与timeEndPeriod来设置精度
        //缺点：与GetTickCount一样，受返回值的最大位数限制。
        //The timeBeginPeriod function requests a minimum resolution for periodic timers
        [DllImport("winmm")]
        static extern uint timeGetTime();
        [DllImport("winmm")]
        static extern void timeBeginPeriod(int t);
        [DllImport("winmm")]
        static extern void timeEndPeriod(int t);


        //用于得到高精度计时器（如果存在这样的计时器）的值。微软对这个API解释就是每秒钟某个计数器增长的数值。
        //如果安装的硬件不支持高精度计时器,函数将返回false需要配合另一个API函数QueryPerformanceFrequency。
        //精度为百万分之一秒。而且由于是long型，所以不存在上面几个API位数不够的问题。
        //缺点：在一篇文章看到，该API在节能模式的时候结果偏慢，超频模式的时候又偏快，而且用电池和接电源的时候效果还不一样（笔记本）
        //原文地址：http://delphi.xcjc.net/viewthread.php?tid=1570
        //未经过超频等测试，如果是真的，那该API出来的结果就可能不准。
        [DllImport("kernel32.dll ")]
        static extern bool QueryPerformanceCounter(ref long lpPerformanceCount);
        [DllImport("kernel32")]
        static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);


        private void TestMethodTimer()
        {
            Console.WriteLine($"Number of milliseconds elapsed since the system started:{System.Environment.TickCount}");//返回值是uint,最大值是2的32次方，因此如果服务器连续开机大约49天以后，该方法取得的返回值会归零

            uint start = 0;
            for (int i = 0; i < 5; i++)
            {
                timeBeginPeriod(1);//
                start = timeGetTime();
                Thread.Sleep(1000);
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")} {i} Total Run Time(ms):{timeGetTime() - start}");  //单位毫秒
                timeEndPeriod(1);
                /*
                Result:
                2017 - 01 - 22 04:04:36.469 Total Run Time(ms):1000
                2017 - 01 - 22 04:04:37.469 Total Run Time(ms):1000
                2017 - 01 - 22 04:04:38.469 Total Run Time(ms):1000
                2017 - 01 - 22 04:04:39.469 Total Run Time(ms):1000
                2017 - 01 - 22 04:04:40.469 Total Run Time(ms):1000
                2017 - 01 - 22 04:04:41.469 Total Run Time(ms):1000
                2017 - 01 - 22 04:04:42.469 Total Run Time(ms):1000
                2017 - 01 - 22 04:04:43.470 Total Run Time(ms):1000
                2017 - 01 - 22 04:04:44.470 Total Run Time(ms):1000
                */
            }


            long elapsed = 0;
            long conterBegin = 0;
            long conterEnd = 0;
            QueryPerformanceFrequency(ref elapsed);
            QueryPerformanceCounter(ref conterBegin);
            Thread.Sleep(1);
            QueryPerformanceCounter(ref conterEnd);
            Console.WriteLine((conterEnd - conterBegin) / (decimal)elapsed);  //单位秒
        }
        private void TestAccurateTimer()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            AccurateTimer.AccurateSleep(12);
            stopwatch.Stop();
            Console.WriteLine("AccurateTimer Sleep Time(ms):" + stopwatch.ElapsedMilliseconds);
            Console.WriteLine("AccurateTimer Sleep Time(ticks):" + stopwatch.ElapsedTicks);

        }
        #endregion TestTimer


        private void TestMethodQueryPerfCounter()
        {
            int iterations = 5;
            QueryPerfCounter myTimer = new QueryPerfCounter();
            // Measure without boxing
            myTimer.Start();
            for (int i = 0; i < iterations; i++)
            {
                // do some work to time
            }
            myTimer.Stop();
            // Calculate time per iteration in nanoseconds
            double result = myTimer.Duration(iterations);

            new QueryPerfCounterTest().ExecuteTest();
        }
        private void TestMethodStopwatch()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Thread.Sleep(1000);
            Console.WriteLine("Total Run Time(ms)" + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
            Thread.Sleep(1000);
            stopwatch.Stop();

            Console.WriteLine("Total Run Time:" + stopwatch.Elapsed);
            Console.WriteLine("Total Run Time(ms):" + stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Total Run Time(ticks):" + stopwatch.ElapsedTicks);
            Console.WriteLine("Number of ticks per second:" + Stopwatch.Frequency);//Gets the frequency of the timer as the number of ticks per second
            Console.WriteLine("Whether based on a high-resolution performance counter:" + Stopwatch.IsHighResolution);//Indicates whether the timer is based on a high-resolution performance counter
            Console.WriteLine("Timer isRunning:" + stopwatch.IsRunning.ToString());
        }

    }
}


