using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace LarkLib.Common.Utilities
{
    public static class Uuid
    {
        [DllImport("rpcrt4.dll", SetLastError = true)]
        private static extern int UuidCreateSequential(out Guid guid);

        public static Guid NewUuid()
        {
            Guid result;

            lock (typeof(Uuid))
            {
                int hr = UuidCreateSequential(out result);

                if (hr == 0)
                    result = Guid.NewGuid();

                //Thread.Sleep(1);
                SpinWait.SpinUntil(() => false, 1);
            }

            return result;
        }

        public static string NewUuidString()
        {
            Guid result = NewUuid();

            byte[] guidBytes = result.ToByteArray();

            for (int i = 0; i < 8; i++)
            {
                byte t = guidBytes[15 - i];
                guidBytes[15 - i] = guidBytes[i];
                guidBytes[i] = t;
            }

            return new Guid(guidBytes).ToString();
        }
    }
}
