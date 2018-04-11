using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    class NewFeature5 : INewFeature
    {
        async void TestMethod()
        {
            for (int i = 0; i < 8; i++)
            {
                var result = await GetTimeStringAsync().ConfigureAwait(false);//donen't need to run in original thread
                Console.WriteLine($"{nameof(TestMethod)} {i,6} {result}");
                Thread.Sleep(1000);
            }
            //var array = new[] { 1, 2, 3 };
            //var one = from n in array select n;
        }

        private async Task<string> GetTimeStringAsync()
        {
            var resultString = DateTime.Now.ToString();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"{nameof(GetTimeStringAsync)} {i,6} {resultString}");
                var task = await Task.Factory.StartNew(() => { Console.WriteLine("Thread in GetTimeStringAsync"); return 1; });
                Thread.Sleep(1000);
            }
            return resultString;
        }

        private async void TestWebRequest(object sender, EventArgs e)
        {
            var request = WebRequest.Create("http://localhost:8090/home.htm");
            var content = new MemoryStream();
            Task<WebResponse> responseTask = request.GetResponseAsync();
            using (var response = await responseTask)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    Task copyTask = responseStream.CopyToAsync(content);
                    //await operator to supends the excution of the method
                    //until the task is completed. In the meantime,
                    //the control is returned the UI thread.
                    await copyTask;
                }
            }
            string txtResult = content.Length.ToString();

        }

        private void TestTask()
        {

            Thread.Sleep(2000);
            Console.WriteLine($"{nameof(TestTask)}");
        }
        public void ExecuteTest()
        {
            //var task = new Task(TestTask);
            //task.Start();
            //Console.WriteLine($"{nameof(ExecuteTest)}");
            //return;
            for (int i = 0; i < 5; i++)
            {
                TestMethod();
                Console.WriteLine($"{nameof(ExecuteTest)}  {i,6}  {DateTime.Now}");
                Thread.Sleep(1000);
            }

        }
    }
}
