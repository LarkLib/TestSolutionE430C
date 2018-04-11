using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    class TestStructLayout : INewFeature
    {
        public void ExecuteTest()
        {
            TestPack();
            TestMethodStructLayout();
        }
        private void TestMethodStructLayout()
        {
            OneTime oneTime = new OneTime() { wYear = 2017, wMonth = 1, wDayOfYear = 22 };
            Console.WriteLine($"OneTime.wDayOfYear={oneTime.wDayOfYear} wWeekOfYear={oneTime.wWeekOfYear}");
            oneTime.wWeekOfYear = 3;
            Console.WriteLine($"OneTime.wDayOfYear={oneTime.wDayOfYear} wWeekOfYear={oneTime.wWeekOfYear}");

            TwoTime twoTime = new TwoTime() { wYear = 2017, wMonth = 1, wDayOfYear = 22 };
            Console.WriteLine($"TwoTime.wDayOfYear={twoTime.wDayOfYear} wWeekOfYear={twoTime.wWeekOfYear}");
            twoTime.wWeekOfYear = 3;
            Console.WriteLine($"TwoTime.wDayOfYear={twoTime.wDayOfYear} wWeekOfYear={twoTime.wWeekOfYear}");
        }



        //1.Open the project's Properties page.
        //2.Click the Build property page.
        //3.Select the Allow Unsafe Code check box.
        public unsafe static void TestPack()
        {

            ExampleStruct ex = new ExampleStruct();
            byte* addr = (byte*)&ex;
            Console.WriteLine("Size:      {0}", sizeof(ExampleStruct));
            Console.WriteLine("b1 Offset: {0}", &ex.b1 - addr);
            Console.WriteLine("b2 Offset: {0}", &ex.b2 - addr);
            Console.WriteLine("i3 Offset: {0}", (byte*)&ex.i3 - addr);
        }

    }


    //If the CharSet field is set to CharSet.Unicode, all string arguments are converted to Unicode characters (LPWSTR) before they are passed to the unmanaged implementation. 
    //If the field is set to CharSet.Ansi, the strings are converted to ANSI strings (LPSTR). 
    //If the CharSet field is set to CharSet.Auto, the conversion is platform-dependent (ANSI on Windows 98 and Windows Me, and Unicode on later versions).
    //The Pack field controls the alignment of a type's fields in memory. It affects both LayoutKind.Sequential and LayoutKind.Explicit. By default, the value is 0, indicating the default packing size for the current platform. The value of Pack must be 0, 1, 2, 4, 8, 16, 32, 64, or 128:
    //The fields of a type instance are aligned by using the following rules:
    //•The alignment of the type is the size of its largest element(1, 2, 4, 8, etc., bytes) or the specified packing size, whichever is smaller.
    //•Each field must align with fields of its own size(1, 2, 4, 8, etc., bytes) or the alignment of the type, whichever is smaller.Because the default alignment of the type is the size of its largest element, which is greater than or equal to all other field lengths, this usually means that fields are aligned by their size.For example, even if the largest field in a type is a 64-bit (8-byte) integer or the Pack field is set to 8, Byte fields align on 1 - byte boundaries, Int16 fields align on 2 - byte boundaries, and Int32 fields align on 4 - byte boundaries.
    //•Padding is added between fields to satisfy the alignment requirements.
    //Pack:https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.structlayoutattribute.pack(v=vs.110).aspx
    [StructLayout(LayoutKind.Explicit, Size = 6, CharSet = CharSet.Ansi)]
    public class OneTime
    {
        [FieldOffset(0)]
        public ushort wYear;
        [FieldOffset(2)]
        public ushort wMonth;
        [FieldOffset(4)]
        public ushort wDayOfYear;
        [FieldOffset(4)]
        public ushort wWeekOfYear;
    }

    [StructLayout(LayoutKind.Sequential, Size = 6, CharSet = CharSet.Ansi)]
    public class TwoTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfYear;
        public ushort wWeekOfYear;
    }



    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    struct ExampleStruct
    {
        public byte b1;
        public byte b2;
        public int i3;
    }

    struct Employeestruct
    {
        //错误消息告诉我们，自动属性背后的字段还没有初始化；并且给了我们一个建议，那就是在构造器之前调用默认构造器。
        public StringBuilder Builder { get; set; }
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int age;
        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        private int gongzhi;
        public int Gongzhi
        {
            get { return gongzhi; }
            //set { gongzhi = value; }
        }
        //有参数构造函数
        public Employeestruct(string _name, int _age, int _gongzhi, StringBuilder builder) : this()
        {
            //如果要在结构中使用构造函数则必须给所有的变量赋值（在构造函数中赋值）
            this.name = _name;
            this.age = _age;
            this.gongzhi = _gongzhi;
            this.Builder = builder;
        }
    }
}
