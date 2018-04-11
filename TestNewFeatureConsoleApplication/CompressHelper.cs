using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestNewFeatureConsoleApplication
{
    /// <summary>  
    /// 简单的压缩  
    /// </summary>  
    public static class CompressHelper
    {
        /// <summary>  
        /// 压缩字符串  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static byte[] CompressString(string str)
        {
            return CompressBytes(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>  
        /// 压缩二进制  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static byte[] CompressBytes(byte[] str)
        {
            var ms = new MemoryStream(str) { Position = 0 };
            var outms = new MemoryStream();
            using (var deflateStream = new DeflateStream(outms, CompressionMode.Compress, true))
            {
                var buf = new byte[1024];
                int len;
                while ((len = ms.Read(buf, 0, buf.Length)) > 0)
                    deflateStream.Write(buf, 0, len);
            }
            return outms.ToArray();
        }
        /// <summary>  
        /// 解压字符串  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static string DecompressString(byte[] str)
        {
            return Encoding.UTF8.GetString(DecompressBytes(str));
        }
        /// <summary>  
        /// 解压二进制  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static byte[] DecompressBytes(byte[] str)
        {
            var ms = new MemoryStream(str) { Position = 0 };
            var outms = new MemoryStream();
            using (var deflateStream = new DeflateStream(ms, CompressionMode.Decompress, true))
            {
                var buf = new byte[1024];
                int len;
                while ((len = deflateStream.Read(buf, 0, buf.Length)) > 0)
                    outms.Write(buf, 0, len);
            }
            return outms.ToArray();
        }
    }
}
