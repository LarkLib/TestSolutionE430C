using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    public static class ObjectExtension
    {
        public static T CopyObject<T>(this object objSource)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, objSource);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
        public static IntPtr GetIntPtr(this object objSource)
        {
            var ptr = (IntPtr)0;
            if (objSource != null)
            {
                GCHandle gch = GCHandle.Alloc(objSource, GCHandleType.Pinned);
                ptr = gch == null ? (IntPtr)0 : gch.AddrOfPinnedObject();
            }
            return ptr;
        }
    }
}
