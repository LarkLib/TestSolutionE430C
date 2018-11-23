using AutoIt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace TestUIAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            //test1(); //test start process
            //test2(); //test find process by name
            //test3(); //test autoitx
            test4(); //test autoitx with get string
        }
        static void test1()
        {
            var windir = Environment.GetEnvironmentVariable("windir");
            var appPath = string.Empty;
            appPath = $@"{windir}\\system32\calc.exe";
            Process process = Process.Start(appPath);
            int processId = process.Id;
            var winHandle = process.Handle;
            var mainWindowHandle = process.MainWindowHandle;
            AutomationElement element = FindElementById(processId, "139");
            Thread.Sleep(2000);
            process.CloseMainWindow();
            //process.Kill();
            Thread.Sleep(2000);
        }
        static void test2()
        {
            //Process[] processes = Process.GetProcessesByName("智远一户通V3.69");
            //Process[] processes = Process.GetProcesses();
            Process[] processes = Process.GetProcessesByName("TdxW"); //ProcessName(TdxW.exe remove .ext) find by foreach process and print info
            foreach (var item in processes.OrderBy(e => e.ProcessName))
            {
                Console.WriteLine($"ProcessName:{item.ProcessName},MainWindowTitle:{item.MainWindowTitle}");
            }
            var process = processes[0];
            int processId = process.Id;
            Console.WriteLine($"process.id:{processId}");
            AutomationElement element = FindElementById(processId, "236");
            Console.WriteLine($"process.id:{element.Current.ProcessId},AutomationElement:{element.Current.AutomationId}");

            Thread.Sleep(2000);
            //process.CloseMainWindow();
            //process.Kill();
            //Thread.Sleep(2000);
        }
        static void test3()
        {
            var windir = Environment.GetEnvironmentVariable("windir");
            var appPath = string.Empty;
            appPath = $@"{windir}\\system32";
            var processId = AutoItX.Run("notepad.exe", appPath);
            var waitActiveId = AutoItX.WinWaitActive("Untitled");
            AutoItX.Send("I'm in notepad");
            Thread.Sleep(3000);
            IntPtr winHandle = AutoItX.WinGetHandle("Untitled");
            AutoItX.WinKill(winHandle);
        }
        static void test4()
        {
            //Open Notepad
            IntPtr processId = (IntPtr)AutoItX.Run("notepad.exe", "", 1);

            //Wait for Notepad to open with a timeout of 10 seconds
            //AutoItX.WinWait("[CLASS:Notepad]", "", 10);
            AutoItX.WinWait(processId, 3);
            var process = Process.GetProcessById((int)processId);
            var mainWindowHandle = process.MainWindowHandle;
            var handle = process.Handle;
            //Send text example
            //AutoItX.ControlSend("[CLASS:Notepad]", "", "Edit1", "autoitsourcode.blogspot.com", 0);
            IntPtr textControlId = (IntPtr)AutoItX.ControlGetHandle(mainWindowHandle, "Edit1");
            AutoItX.ControlSend(mainWindowHandle, textControlId, "this is a testing string, send by autoidx");

            //Get the texts
            string strReturnText = AutoItX.ControlGetText("[CLASS:Notepad]", "", "Edit1");
            Console.WriteLine($"strReturnText:{strReturnText}");
            Thread.Sleep(3000);
            IntPtr winHandle = AutoItX.WinGetHandle("Untitled");
            //AutoItX.WinKill(winHandle);
            AutoItX.WinKill(mainWindowHandle);
            //Process.GetProcessById((int)processId).Kill();
        }
        public static AutomationElement FindElementById(int processId, string automationId)
        {
            AutomationElement aeForm = FindWindowByProcessId(processId);
            AutomationElement tarFindElement = aeForm.FindFirst(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.AutomationIdProperty, automationId));
            return tarFindElement;
        }

        public static AutomationElement FindWindowByProcessId(int processId)
        {
            AutomationElement targetWindow = null;
            int count = 0;
            try
            {
                Process p = Process.GetProcessById(processId);
                targetWindow = AutomationElement.FromHandle(p.MainWindowHandle);
                return targetWindow;
            }
            catch (Exception ex)
            {
                count++;
                StringBuilder sb = new StringBuilder();
                string message = sb.AppendLine(string.Format("Target window is not existing.try #{0}", count)).ToString();
                if (count > 5)
                {
                    throw new InvalidProgramException(message, ex);
                }
                else
                {
                    return FindWindowByProcessId(processId);
                }
            }
        }

        public static InvokePattern GetInvokePattern(AutomationElement element)
        {
            object currentPattern;
            if (!element.TryGetCurrentPattern(InvokePattern.Pattern, out currentPattern))
            {
                throw new Exception(string.Format("Element with AutomationId '{0}' and Name '{1}' does not support the InvokePattern.",
                    element.Current.AutomationId, element.Current.Name));
            }
            return currentPattern as InvokePattern;
        }
    }
}
