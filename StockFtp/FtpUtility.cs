using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StockFtp
{
    static class FtpUtility
    {
        public static List<string> GetFileList(string ftpPath, string username, string password, string mask = null)
        {
            List<string> fileList = new List<string>();
            try
            {
                string uri = ftpPath;
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(username, password);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                string line = reader.ReadLine();
                while (line != null)
                {
                    if (!string.IsNullOrWhiteSpace(mask) && !mask.Trim().Equals("*.*"))
                    {
                        string maskChar = mask.Substring(0, mask.IndexOf("*"));
                        if (line.Substring(0, maskChar.Length) != maskChar)
                        {
                            continue;
                        }
                    }
                    fileList.Add($"{uri}/{line}");
                    Console.WriteLine(line);
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                return fileList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get file list error：" + ex.Message);
            }
            return fileList;
        }

    }
}
