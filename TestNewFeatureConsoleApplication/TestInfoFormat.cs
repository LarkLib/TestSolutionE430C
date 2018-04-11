using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace TestNewFeatureConsoleApplication
{
    class TestInfoFormat : INewFeature
    {
        public void ExecuteTest()
        {
            TestMethodEscapeSequenceCharacter();
            TestMethodRealLiterals();
            TestMethodTextInfo();
            TestGetCurrenfPath();
            TestMethodRound();
            TestMethodFormatDateTime();
            TestMethodFormatNember();
            TestMethodBigInteger();
            TestKeywords();
            TestMethodGetXmlSchema();
        }

        private void TestGetCurrenfPath()
        {
            //获取当前进程的完整路径，包含文件名(进程名)。
            string str = this.GetType().Assembly.Location;
            //result: X:\xxx\xxx\xxx.exe(.exe文件所在的目录 +.exe文件名)

            //获取新的 Process 组件并将其与当前活动的进程关联的主模块的完整路径，包含文件名(进程名)。
            str = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            //result: X:\xxx\xxx\xxx.exe(.exe文件所在的目录 +.exe文件名)

            //获取和设置当前目录（即该进程从中启动的目录）的完全限定路径。
            str = System.Environment.CurrentDirectory;
            //result: X:\xxx\xxx(.exe文件所在的目录)

            //获取当前 Thread 的当前应用程序域的基目录，它由程序集冲突解决程序用来探测程序集。
            str = System.AppDomain.CurrentDomain.BaseDirectory;
            //result: X:\xxx\xxx\ (.exe文件所在的目录 + "\")

            //获取和设置包含该应用程序的目录的名称。(推荐)
            str = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //result: X:\xxx\xxx\ (.exe文件所在的目录 + "\")

            //获取启动了应用程序的可执行文件的路径，不包括可执行文件的名称。
            str = System.Windows.Forms.Application.StartupPath;
            //result: X:\xxx\xxx(.exe文件所在的目录)

            //获取启动了应用程序的可执行文件的路径，包括可执行文件的名称。
            str = System.Windows.Forms.Application.ExecutablePath;
            //result: X:\xxx\xxx\xxx.exe(.exe文件所在的目录 +.exe文件名)

            //获取应用程序的当前工作目录(不可靠)。
            str = System.IO.Directory.GetCurrentDirectory();
            //result: X:\xxx\xxx(.exe文件所在的目录)
        }

        private void TestMethodTextInfo()
        {
            string title = "this is my converted string";
            Console.WriteLine("String Class");
            Console.WriteLine("------------");

            //Convert string to uppercase.
            Console.WriteLine(title.ToUpper());
            //Convert string to lowercase.
            Console.WriteLine(title.ToLower());

            Console.WriteLine();
            Console.WriteLine("TextInfo Class");
            Console.WriteLine("--------------");

            //Get the culture property of the thread.
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            //Create TextInfo object.
            TextInfo textInfo = cultureInfo.TextInfo;

            //Convert to uppercase.
            Console.WriteLine(textInfo.ToUpper(title));
            //Convert to lowercase.
            Console.WriteLine(textInfo.ToLower(title));
            //Convert to title case.
            Console.WriteLine(textInfo.ToTitleCase(title));
        }

        //Real literals are used to write values of types float, double, and decimal
        //real-type-suffix:  one of F f  D d  M m
        private void TestMethodRealLiterals()
        {
            var doubleValued = default(double);
            var doubleValueD = default(double);
            var floatValuef = default(float);
            var floatValueF = default(float);
            var decimalValued = default(decimal);
            var decimalValueD = default(decimal);

            doubleValued = 100.999d;
            doubleValueD = 100.999D;
            floatValuef = 100.999f;
            floatValueF = 100.999F;
            decimalValued = 1000.999M;
            decimalValueD = 1000.999m;

            Console.WriteLine($@"
doubleValued = 100.999d;
doubleValueD = 100.999D;
floatValuef = 100.999f;
floatValueF = 100.999F;
decimalValued = 1000.999m;
decimalValueD = 1000.999m;
{doubleValued} {doubleValueD} {floatValuef} {floatValueF} {decimalValued} {decimalValueD}");
        }

        /*
        Escape sequence	Character name	Unicode encoding
        \'	Single quote	0x0027
        \"	Double quote	0x0022
        \\	Backslash	0x005C
        \0	Null	0x0000
        \a	Alert	0x0007
        \b	Backspace	0x0008
        \f	Form feed	0x000C
        \n	New line	0x000A
        \r	Carriage return	0x000D
        \t	Horizontal tab	0x0009
        \v	Vertical tab	0x000B
        */
        private void TestMethodEscapeSequenceCharacter()
        {
            Console.WriteLine("Escape sequence	Character: \\' \\\" \\\\ \\0 \\a \\b \\f \\n \\r \\t \\v");
        }

        private void TestMethodRound()
        {
            string str = (string)null;
            Console.WriteLine("(string)null==null is {0}", string.IsNullOrEmpty(str).ToString());
            double fA = 3.5;
            double fB = 4.5;
            Console.WriteLine("fA = 3.5; fB = 4.5;");
            Console.WriteLine("int(fA)={0},int(fB)={1}", (int)fA, (int)fB);
            Console.WriteLine("int(fA)/1*1={0},int(fB)/1*1={1}", (int)fA / 1 * 1, (int)fB / 1 * 1);
            Console.WriteLine("Convert.ToInt32(fA)={0},Convert.ToInt32(fB)={1}", Convert.ToInt32(fA), Convert.ToInt32(fB));
            Console.WriteLine("Math.Round(fA)={0},Math.Round(fB)={1}", Math.Round(fA), Math.Round(fB));
            Console.WriteLine("Math.Round(fA,0,AwayFromZero)={0},Math.Round(fB,0,AwayFromZero)={1}", Math.Round(fA, 0, MidpointRounding.AwayFromZero), Math.Round(fB, 0, MidpointRounding.AwayFromZero));
            Console.WriteLine("Math.Round(fA,0,ToEven)={0},Math.Round(fB,0,ToEven)={1}", Math.Round(fA, 0, MidpointRounding.ToEven), Math.Round(fB, 0, MidpointRounding.ToEven));
            BigInteger bigInt = 12;
        }
        private void TestMethodBigInteger()
        {
            BigInteger bigInt = BigInteger.Pow(2, 2048);
            Console.WriteLine($"{nameof(TestMethodBigInteger)} {bigInt}");
        }

        private void TestMethodFormatNember()
        {
            Console.WriteLine("TestMethodFormatNember: See the code, content is from msdn");
            /*Standard Numeric Format Strings
           Format specifier	Name	Description	Examples
           "C" or "c"	Currency	Result: A currency value.	123.456 ("C", en-US) -> $123.46

                   Supported by: All numeric types.	123.456 ("C", fr-FR) -> 123,46 €

                   Precision specifier: Number of decimal digits.	123.456 ("C", ja-JP) -> ¥123

                   Default precision specifier: Defined by NumberFormatInfo.CurrencyDecimalDigits.	-123.456 ("C3", en-US) -> ($123.456)

                   More information: The Currency ("C") Format Specifier.	-123.456 ("C3", fr-FR) -> -123,456 €

                       -123.456 ("C3", ja-JP) -> -¥123.456
           "D" or "d"	Decimal	Result: Integer digits with optional negative sign.	1234 ("D") -> 1234

                   Supported by: Integral types only.	-1234 ("D6") -> -001234

                   Precision specifier: Minimum number of digits.	

                   Default precision specifier: Minimum number of digits required.	

                   More information: The Decimal("D") Format Specifier.	
           "E" or "e"	Exponential (scientific)	Result: Exponential notation.	1052.0329112756 ("E", en-US) -> 1.052033E+003

                   Supported by: All numeric types.	1052.0329112756 ("e", fr-FR) -> 1,052033e+003

                   Precision specifier: Number of decimal digits.	-1052.0329112756 ("e2", en-US) -> -1.05e+003

                   Default precision specifier: 6.	-1052.0329112756 ("E2", fr_FR) -> -1,05E+003

                   More information: The Exponential ("E") Format Specifier.	
           "F" or "f"	Fixed-point	Result: Integral and decimal digits with optional negative sign.	1234.567 ("F", en-US) -> 1234.57

                   Supported by: All numeric types.	1234.567 ("F", de-DE) -> 1234,57

                   Precision specifier: Number of decimal digits.	1234 ("F1", en-US) -> 1234.0

                   Default precision specifier: Defined by NumberFormatInfo.NumberDecimalDigits.	1234 ("F1", de-DE) -> 1234,0

                   More information: The Fixed-Point ("F") Format Specifier.	-1234.56 ("F4", en-US) -> -1234.5600

                       -1234.56 ("F4", de-DE) -> -1234,5600
           "G" or "g"	General	Result: The more compact of either fixed-point or scientific notation.	-123.456 ("G", en-US) -> -123.456

                   Supported by: All numeric types.	-123.456 ("G", sv-SE) -> -123,456

                   Precision specifier: Number of significant digits.	123.4546 ("G4", en-US) -> 123.5

                   Default precision specifier: Depends on numeric type.	123.4546 ("G4", sv-SE) -> 123,5

                   More information: The General ("G") Format Specifier.	-1.234567890e-25 ("G", en-US) -> -1.23456789E-25

                       -1.234567890e-25 ("G", sv-SE) -> -1,23456789E-25
           "N" or "n"	Number	Result: Integral and decimal digits, group separators, and a decimal separator with optional negative sign.	1234.567 ("N", en-US) -> 1,234.57

                   Supported by: All numeric types.	1234.567 ("N", ru-RU) -> 1 234,57

                   Precision specifier: Desired number of decimal places.	1234 ("N1", en-US) -> 1,234.0

                   Default precision specifier: Defined by NumberFormatInfo.NumberDecimalDigits.	1234 ("N1", ru-RU) -> 1 234,0

                   More information: The Numeric ("N") Format Specifier.	-1234.56 ("N3", en-US) -> -1,234.560

                       -1234.56 ("N3", ru-RU) -> -1 234,560
           "P" or "p"	Percent	Result: Number multiplied by 100 and displayed with a percent symbol.	1 ("P", en-US) -> 100.00 %

                   Supported by: All numeric types.	1 ("P", fr-FR) -> 100,00 %

                   Precision specifier: Desired number of decimal places.	-0.39678 ("P1", en-US) -> -39.7 %

                   Default precision specifier: Defined by NumberFormatInfo.PercentDecimalDigits.	-0.39678 ("P1", fr-FR) -> -39,7 %

                   More information: The Percent ("P") Format Specifier.	
           "R" or "r"	Round-trip	Result: A string that can round-trip to an identical number.	123456789.12345678 ("R") -> 123456789.12345678

                   Supported by: Single, Double, and BigInteger.	-1234567890.12345678 ("R") -> -1234567890.1234567

                   Precision specifier: Ignored.	

                   More information: The Round-trip ("R") Format Specifier.	
           "X" or "x"	Hexadecimal	Result: A hexadecimal string.	255 ("X") -> FF

                   Supported by: Integral types only.	-1 ("x") -> ff

                   Precision specifier: Number of digits in the result string.	255 ("x4") -> 00ff

                   More information: The HexaDecimal ("X") Format Specifier.	-1 ("X4") -> 00FF
           Any other single character	Unknown specifier	Result: Throws a FormatException at run time.	   

           */

            /*Custom Numeric Format Strings
             Format specifier	Name	Description	Examples
            "0"	Zero placeholder	Replaces the zero with the corresponding digit if one is present; otherwise, zero appears in the result string.	1234.5678 ("00000") -> 01235

                    More information: The "0" Custom Specifier.	0.45678 ("0.00", en-US) -> 0.46

                        0.45678 ("0.00", fr-FR) -> 0,46
            "#"	Digit placeholder	Replaces the "#" symbol with the corresponding digit if one is present; otherwise, no digit appears in the result string.	1234.5678 ("#####") -> 1235

                    Note that no digit appears in the result string if the corresponding digit in the input string is a non-significant 0. For example, 0003 ("####") -> 3.	0.45678 ("#.##", en-US) -> .46

                    More information: The "#" Custom Specifier.	0.45678 ("#.##", fr-FR) -> ,46
            "."	Decimal point	Determines the location of the decimal separator in the result string.	0.45678 ("0.00", en-US) -> 0.46

                    More information: The "." Custom Specifier.	0.45678 ("0.00", fr-FR) -> 0,46
            ","	Group separator and number scaling	Serves as both a group separator and a number scaling specifier. As a group separator, it inserts a localized group separator character between each group. As a number scaling specifier, it divides a number by 1000 for each comma specified.	Group separator specifier:

                    More information: The "," Custom Specifier.	2147483647 ("##,#", en-US) -> 2,147,483,647

                        2147483647 ("##,#", es-ES) -> 2.147.483.647

                        Scaling specifier:

                        2147483647 ("#,#,,", en-US) -> 2,147

                        2147483647 ("#,#,,", es-ES) -> 2.147
            "%"	Percentage placeholder	Multiplies a number by 100 and inserts a localized percentage symbol in the result string.	0.3697 ("%#0.00", en-US) -> %36.97

                    More information: The "%" Custom Specifier.	0.3697 ("%#0.00", el-GR) -> %36,97

                        0.3697 ("##.0 %", en-US) -> 37.0 %

                        0.3697 ("##.0 %", el-GR) -> 37,0 %
            "‰"	Per mille placeholder	Multiplies a number by 1000 and inserts a localized per mille symbol in the result string.	0.03697 ("#0.00‰", en-US) -> 36.97‰

                    More information: The "‰" Custom Specifier.	0.03697 ("#0.00‰", ru-RU) -> 36,97‰
            "E0"	Exponential notation	If followed by at least one 0 (zero), formats the result using exponential notation. The case of "E" or "e" indicates the case of the exponent symbol in the result string. The number of zeros following the "E" or "e" character determines the minimum number of digits in the exponent. A plus sign (+) indicates that a sign character always precedes the exponent. A minus sign (-) indicates that a sign character precedes only negative exponents.	987654 ("#0.0e0") -> 98.8e4

            "E+0"		More information: The "E" and "e" Custom Specifiers.	1503.92311 ("0.0##e+00") -> 1.504e+03

            "E-0"			1.8901385E-16 ("0.0e+00") -> 1.9e-16

            "e0"			

            "e+0"			

            "e-0"			
            \	Escape character	Causes the next character to be interpreted as a literal rather than as a custom format specifier.	987654 ("\###00\#") -> #987654#

                    More information: The "\" Escape Character.	
            'string'	Literal string delimiter	Indicates that the enclosed characters should be copied to the result string unchanged.	68 ("# ' degrees'") -> 68 degrees

            "string"			68 ("#' degrees'") -> 68 degrees
            ;	Section separator	Defines sections with separate format strings for positive, negative, and zero numbers.	12.345 ("#0.0#;(#0.0#);-\0-") -> 12.35

                    More information: The ";" Section Separator.	0 ("#0.0#;(#0.0#);-\0-") -> -0-

                        -12.345 ("#0.0#;(#0.0#);-\0-") -> (12.35)

                        12.345 ("#0.0#;(#0.0#)") -> 12.35

                        0 ("#0.0#;(#0.0#)") -> 0.0

                        -12.345 ("#0.0#;(#0.0#)") -> (12.35)
            Other	All other characters	The character is copied to the result string unchanged.	68 ("# °") -> 68 °

         */
        }
        private void TestMethodFormatDateTime()
        {
            DateTime dateValue = new DateTime(2009, 6, 1, 4, 37, 0);
            CultureInfo[] cultures = { new CultureInfo("en-US"),
                                 new CultureInfo("fr-FR"),
                                 new CultureInfo("it-IT"),
                                 new CultureInfo("de-DE") };
            foreach (CultureInfo culture in cultures)
                Console.WriteLine("{0}: {1}", culture.Name, dateValue.ToString(culture));

            Console.WriteLine("TestMethodFormatDateTime: See the code from more info, content is from msdn");
            /*
            Character literals				
            The following characters in a custom date and time format string are reserved and are always interpreted as formatting characters or, in the case of ", ', /, and \, as special 				

            characters.				

            F	H	K	M	d
            f	g	h	m	s
            t	y	z	%	:
            /	"	'	\	
            */
            /*Standard Date and Time Format Strings
            Format specifier	Description	Examples
            "d"	Short date pattern.	2009-06-15T13:45:30 -> 6/15/2009 (en-US)

                More information:The Short Date ("d") Format Specifier.	2009-06-15T13:45:30 -> 15/06/2009 (fr-FR)

                    2009-06-15T13:45:30 -> 2009/06/15 (ja-JP)
            "D"	Long date pattern.	2009-06-15T13:45:30 -> Monday, June 15, 2009 (en-US)

                More information:The Long Date ("D") Format Specifier.	2009-06-15T13:45:30 -> 15 июня 2009 г. (ru-RU)

                    2009-06-15T13:45:30 -> Montag, 15. Juni 2009 (de-DE)
            "f"	Full date/time pattern (short time).	2009-06-15T13:45:30 -> Monday, June 15, 2009 1:45 PM (en-US)

                More information: The Full Date Short Time ("f") Format Specifier.	2009-06-15T13:45:30 -> den 15 juni 2009 13:45 (sv-SE)

                    2009-06-15T13:45:30 -> Δευτέρα, 15 Ιουνίου 2009 1:45 μμ (el-GR)
            "F"	Full date/time pattern (long time).	2009-06-15T13:45:30 -> Monday, June 15, 2009 1:45:30 PM (en-US)

                More information: The Full Date Long Time ("F") Format Specifier.	2009-06-15T13:45:30 -> den 15 juni 2009 13:45:30 (sv-SE)

                    2009-06-15T13:45:30 -> Δευτέρα, 15 Ιουνίου 2009 1:45:30 μμ (el-GR)
            "g"	General date/time pattern (short time).	2009-06-15T13:45:30 -> 6/15/2009 1:45 PM (en-US)

                More information: The General Date Short Time ("g") Format Specifier.	2009-06-15T13:45:30 -> 15/06/2009 13:45 (es-ES)

                    2009-06-15T13:45:30 -> 2009/6/15 13:45 (zh-CN)
            "G"	General date/time pattern (long time).	2009-06-15T13:45:30 -> 6/15/2009 1:45:30 PM (en-US)

                More information: The General Date Long Time ("G") Format Specifier.	2009-06-15T13:45:30 -> 15/06/2009 13:45:30 (es-ES)

                    2009-06-15T13:45:30 -> 2009/6/15 13:45:30 (zh-CN)
            "M", "m"	Month/day pattern.	2009-06-15T13:45:30 -> June 15 (en-US)

                More information: The Month ("M", "m") Format Specifier.	2009-06-15T13:45:30 -> 15. juni (da-DK)

                    2009-06-15T13:45:30 -> 15 Juni (id-ID)
            "O", "o"	Round-trip date/time pattern.	DateTime values:

                More information: The Round-trip ("O", "o") Format Specifier.	2009-06-15T13:45:30 (DateTimeKind.Local) --> 2009-06-15T13:45:30.0000000-07:00

                    2009-06-15T13:45:30 (DateTimeKind.Utc) --> 2009-06-15T13:45:30.0000000Z

                    2009-06-15T13:45:30 (DateTimeKind.Unspecified) --> 2009-06-15T13:45:30.0000000

                    DateTimeOffset values:

                    2009-06-15T13:45:30-07:00 --> 2009-06-15T13:45:30.0000000-07:00
            "R", "r"	RFC1123 pattern.	2009-06-15T13:45:30 -> Mon, 15 Jun 2009 20:45:30 GMT

                More information: The RFC1123 ("R", "r") Format Specifier.	
            "s"	Sortable date/time pattern.	2009-06-15T13:45:30 (DateTimeKind.Local) -> 2009-06-15T13:45:30

                More information: The Sortable ("s") Format Specifier.	2009-06-15T13:45:30 (DateTimeKind.Utc) -> 2009-06-15T13:45:30
            "t"	Short time pattern.	2009-06-15T13:45:30 -> 1:45 PM (en-US)

                More information: The Short Time ("t") Format Specifier.	2009-06-15T13:45:30 -> 13:45 (hr-HR)

                    2009-06-15T13:45:30 -> 01:45 م (ar-EG)
            "T"	Long time pattern.	2009-06-15T13:45:30 -> 1:45:30 PM (en-US)

                More information: The Long Time ("T") Format Specifier.	2009-06-15T13:45:30 -> 13:45:30 (hr-HR)

                    2009-06-15T13:45:30 -> 01:45:30 م (ar-EG)
            "u"	Universal sortable date/time pattern.	With a DateTime value: 2009-06-15T13:45:30 -> 2009-06-15 13:45:30Z
            */

            /*Custom Numeric Format Strings
            Format specifier	Description	Examples
            "d"	The day of the month, from 1 through 31.	2009-06-01T13:45:30 -> 1

                More information: The "d" Custom Format Specifier.	2009-06-15T13:45:30 -> 15
            "dd"	The day of the month, from 01 through 31.	2009-06-01T13:45:30 -> 01

                More information: The "dd" Custom Format Specifier.	2009-06-15T13:45:30 -> 15
            "ddd"	The abbreviated name of the day of the week.	2009-06-15T13:45:30 -> Mon (en-US)

                More information: The "ddd" Custom Format Specifier.	2009-06-15T13:45:30 -> Пн (ru-RU)

                    2009-06-15T13:45:30 -> lun. (fr-FR)
            "dddd"	The full name of the day of the week.	2009-06-15T13:45:30 -> Monday (en-US)

                More information: The "dddd" Custom Format Specifier.	2009-06-15T13:45:30 -> понедельник (ru-RU)

                    2009-06-15T13:45:30 -> lundi (fr-FR)
            "f"	The tenths of a second in a date and time value.	2009-06-15T13:45:30.6170000 -> 6

                More information: The "f" Custom Format Specifier.	2009-06-15T13:45:30.05 -> 0
            "ff"	The hundredths of a second in a date and time value.	2009-06-15T13:45:30.6170000 -> 61

                More information: The "ff" Custom Format Specifier.	2009-06-15T13:45:30.0050000 -> 00
            "fff"	The milliseconds in a date and time value.	6/15/2009 13:45:30.617 -> 617

                More information: The "fff" Custom Format Specifier.	6/15/2009 13:45:30.0005 -> 000
            "ffff"	The ten thousandths of a second in a date and time value.	2009-06-15T13:45:30.6175000 -> 6175

                More information: The "ffff" Custom Format Specifier.	2009-06-15T13:45:30.0000500 -> 0000
            "fffff"	The hundred thousandths of a second in a date and time value.	2009-06-15T13:45:30.6175400 -> 61754

                More information: The "fffff" Custom Format Specifier.	6/15/2009 13:45:30.000005 -> 00000
            "ffffff"	The millionths of a second in a date and time value.	2009-06-15T13:45:30.6175420 -> 617542

                More information: The "ffffff" Custom Format Specifier.	2009-06-15T13:45:30.0000005 -> 000000
            "fffffff"	The ten millionths of a second in a date and time value.	2009-06-15T13:45:30.6175425 -> 6175425

                More information: The "fffffff" Custom Format Specifier.	2009-06-15T13:45:30.0001150 -> 0001150
            "F"	If non-zero, the tenths of a second in a date and time value.	2009-06-15T13:45:30.6170000 -> 6

                More information: The "F" Custom Format Specifier.	2009-06-15T13:45:30.0500000 -> (no output)
            "FF"	If non-zero, the hundredths of a second in a date and time value.	2009-06-15T13:45:30.6170000 -> 61

                More information: The "FF" Custom Format Specifier.	2009-06-15T13:45:30.0050000 -> (no output)
            "FFF"	If non-zero, the milliseconds in a date and time value.	2009-06-15T13:45:30.6170000 -> 617

                More information: The "FFF" Custom Format Specifier.	2009-06-15T13:45:30.0005000 -> (no output)
            "FFFF"	If non-zero, the ten thousandths of a second in a date and time value.	2009-06-15T13:45:30.5275000 -> 5275

                More information: The "FFFF" Custom Format Specifier.	2009-06-15T13:45:30.0000500 -> (no output)
            "FFFFF"	If non-zero, the hundred thousandths of a second in a date and time value.	2009-06-15T13:45:30.6175400 -> 61754

                More information: The "FFFFF" Custom Format Specifier.	2009-06-15T13:45:30.0000050 -> (no output)
            "FFFFFF"	If non-zero, the millionths of a second in a date and time value.	2009-06-15T13:45:30.6175420 -> 617542

                More information: The "FFFFFF" Custom Format Specifier.	2009-06-15T13:45:30.0000005 -> (no output)
            "FFFFFFF"	If non-zero, the ten millionths of a second in a date and time value.	2009-06-15T13:45:30.6175425 -> 6175425

                More information: The "FFFFFFF" Custom Format Specifier.	2009-06-15T13:45:30.0001150 -> 000115
            "g", "gg"	The period or era.	2009-06-15T13:45:30.6170000 -> A.D.

                More information: The "g" or "gg" Custom Format Specifier.	
            "h"	The hour, using a 12-hour clock from 1 to 12.	2009-06-15T01:45:30 -> 1

                More information: The "h" Custom Format Specifier.	2009-06-15T13:45:30 -> 1
            "hh"	The hour, using a 12-hour clock from 01 to 12.	2009-06-15T01:45:30 -> 01

                More information: The "hh" Custom Format Specifier.	2009-06-15T13:45:30 -> 01
            "H"	The hour, using a 24-hour clock from 0 to 23.	2009-06-15T01:45:30 -> 1

                More information: The "H" Custom Format Specifier.	2009-06-15T13:45:30 -> 13
            "HH"	The hour, using a 24-hour clock from 00 to 23.	2009-06-15T01:45:30 -> 01

                More information: The "HH" Custom Format Specifier.	2009-06-15T13:45:30 -> 13
            "K"	Time zone information.	With DateTime values:

                More information: The "K" Custom Format Specifier.	2009-06-15T13:45:30, Kind Unspecified ->

                    2009-06-15T13:45:30, Kind Utc -> Z

                    2009-06-15T13:45:30, Kind Local -> -07:00 (depends on local computer settings)

                    With DateTimeOffset values:

                    2009-06-15T01:45:30-07:00 --> -07:00

                    2009-06-15T08:45:30+00:00 --> +00:00
            "m"	The minute, from 0 through 59.	2009-06-15T01:09:30 -> 9

                More information: The "m" Custom Format Specifier.	2009-06-15T13:29:30 -> 29
            "mm"	The minute, from 00 through 59.	2009-06-15T01:09:30 -> 09

                More information: The "mm" Custom Format Specifier.	2009-06-15T01:45:30 -> 45
            "M"	The month, from 1 through 12.	2009-06-15T13:45:30 -> 6

                More information: The "M" Custom Format Specifier.	
            "MM"	The month, from 01 through 12.	2009-06-15T13:45:30 -> 06

                More information: The "MM" Custom Format Specifier.	
            "MMM"	The abbreviated name of the month.	2009-06-15T13:45:30 -> Jun (en-US)

                More information: The "MMM" Custom Format Specifier.	2009-06-15T13:45:30 -> juin (fr-FR)

                    2009-06-15T13:45:30 -> Jun (zu-ZA)
            "MMMM"	The full name of the month.	2009-06-15T13:45:30 -> June (en-US)

                More information: The "MMMM" Custom Format Specifier.	2009-06-15T13:45:30 -> juni (da-DK)

                    2009-06-15T13:45:30 -> uJuni (zu-ZA)
            "s"	The second, from 0 through 59.	2009-06-15T13:45:09 -> 9

                More information: The "s" Custom Format Specifier.	
            "ss"	The second, from 00 through 59.	2009-06-15T13:45:09 -> 09

                More information: The "ss" Custom Format Specifier.	
            "t"	The first character of the AM/PM designator.	2009-06-15T13:45:30 -> P (en-US)

                More information: The "t" Custom Format Specifier.	2009-06-15T13:45:30 -> 午 (ja-JP)

                    2009-06-15T13:45:30 -> (fr-FR)
            "tt"	The AM/PM designator.	2009-06-15T13:45:30 -> PM (en-US)

                More information: The "tt" Custom Format Specifier.	2009-06-15T13:45:30 -> 午後 (ja-JP)

                    2009-06-15T13:45:30 -> (fr-FR)
            "y"	The year, from 0 to 99.	0001-01-01T00:00:00 -> 1

                More information: The "y" Custom Format Specifier.	0900-01-01T00:00:00 -> 0

                    1900-01-01T00:00:00 -> 0

                    2009-06-15T13:45:30 -> 9

                    2019-06-15T13:45:30 -> 19
            "yy"	The year, from 00 to 99.	0001-01-01T00:00:00 -> 01

                More information: The "yy" Custom Format Specifier.	0900-01-01T00:00:00 -> 00

                    1900-01-01T00:00:00 -> 00

                    2019-06-15T13:45:30 -> 19
            "yyy"	The year, with a minimum of three digits.	0001-01-01T00:00:00 -> 001

                More information: The "yyy" Custom Format Specifier.	0900-01-01T00:00:00 -> 900

                    1900-01-01T00:00:00 -> 1900

                    2009-06-15T13:45:30 -> 2009
            "yyyy"	The year as a four-digit number.	0001-01-01T00:00:00 -> 0001

                More information: The "yyyy" Custom Format Specifier.	0900-01-01T00:00:00 -> 0900

                    1900-01-01T00:00:00 -> 1900

                    2009-06-15T13:45:30 -> 2009
            "yyyyy"	The year as a five-digit number.	0001-01-01T00:00:00 -> 00001

                More information: The "yyyyy" Custom Format Specifier.	2009-06-15T13:45:30 -> 02009
            "z"	Hours offset from UTC, with no leading zeros.	2009-06-15T13:45:30-07:00 -> -7

                More information: The "z" Custom Format Specifier.	
            "zz"	Hours offset from UTC, with a leading zero for a single-digit value.	2009-06-15T13:45:30-07:00 -> -07

                More information: The "zz" Custom Format Specifier.	
            "zzz"	Hours and minutes offset from UTC.	2009-06-15T13:45:30-07:00 -> -07:00

                More information: The "zzz" Custom Format Specifier.	
            ":"	The time separator.	2009-06-15T13:45:30 -> : (en-US)

                More information: The ":" Custom Format Specifier.	2009-06-15T13:45:30 -> . (it-IT)

                    2009-06-15T13:45:30 -> : (ja-JP)
            "/"	The date separator.	2009-06-15T13:45:30 -> / (en-US)

                More Information: The "/" Custom Format Specifier.	2009-06-15T13:45:30 -> - (ar-DZ)

                    2009-06-15T13:45:30 -> . (tr-TR)
            "string"	Literal string delimiter.	2009-06-15T13:45:30 ("arr:" h:m t) -> arr: 1:45 P

            'string'	More information: Character literals.	2009-06-15T13:45:30 ('arr:' h:m t) -> arr: 1:45 P
            %	Defines the following character as a custom format specifier.	2009-06-15T13:45:30 (%h) -> 1

                More information:Using Single Custom Format Specifiers.	
            \	The escape character.	2009-06-15T13:45:30 (h \h) -> 1 h

                More information: Character literals and Using the Escape Character.	
            Any other character	The character is copied to the result string unchanged.	2009-06-15T01:45:30 (arr hh:mm t) -> arr 01:45 A

                More information: Character literals.	
                     */
        }

        private void TestKeywords()
        {
            Console.WriteLine(@"
Keywrok List:
abstract as base bool 
break byte case catch 
char checked class const 
continue decimal default delegate 
do double else enum 
event explicit extern false 
finally fixed float for 
foreach goto if implicit 
in in (generic modifier) int interface 
internal is lock long 
namespace new null object 
operator out out (generic modifier) override 
params private protected public 
readonly ref return sbyte 
sealed short sizeof stackalloc 
static string struct switch 
this throw true try 
typeof uint ulong unchecked 
unsafe ushort using virtual 
void volatile while  

Contextual Keywords
--------------------------------------------------------------------------------
A contextual keyword is used to provide a specific meaning in the code, but it is not a reserved word in C#. 
Some contextual keywords, such as partial and where, have special meanings in two or more contexts.

add alias ascending 
async await descending 
dynamic from get 
global group into 
join let orderby 
partial (type) partial (method) remove 
select set value 
var where (generic type constraint) where (query clause) 
yield 
");
        }

        private void TestMethodGetXmlSchema()
        {
            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.xml");
            using (var reader = XmlReader.Create(file))
            {
                XmlSchemaInference schemaInference = new XmlSchemaInference();
                var schemaSet = schemaInference.InferSchema(reader);
                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    var schemaContentBiuder = new StringBuilder();
                    var writer = new StringWriter(schemaContentBiuder);
                    schema.Write(writer);
                    Console.WriteLine(schema.Namespaces);
                    //Console.WriteLine(schemaContentBiuder.ToString());
                }
            }
        }

    }
}
