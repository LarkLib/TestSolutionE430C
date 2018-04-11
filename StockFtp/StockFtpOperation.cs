using FluentFTP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFtp
{
    class StockFtpOperation
    {
        private static readonly string UserName = ConfigurationManager.AppSettings["UserName"];
        private static readonly string Password = ConfigurationManager.AppSettings["Password"];
        private static readonly string FtpPathSh = ConfigurationManager.AppSettings["FtpPathSh"];
        private static readonly string FtpPathSz = ConfigurationManager.AppSettings["FtpPathSz"];
        private static readonly string Host = ConfigurationManager.AppSettings["Host"];
        private static readonly string DownloadToPath = ConfigurationManager.AppSettings["DownloadToPath"];

        private static readonly string ExecutePaht = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string DataDirectory = $"{ExecutePaht}\\Data";
        private static readonly string LastDownlaodFileSh = $"{DataDirectory}\\LastDownlaodFileSh.txt";
        private static readonly string LastDownlaodFileSz = $"{DataDirectory}\\LastDownlaodFileSz.txt";

        internal void ExecuteOperation()
        {
            if (!Directory.Exists(DownloadToPath)) Directory.CreateDirectory(DownloadToPath);
            if (!Directory.Exists(DataDirectory)) Directory.CreateDirectory(DataDirectory);
            var client = GetFtpClient();
            client.Connect();

            DownloadFiles(client, FtpPathSh, LastDownlaodFileSh, DownloadToPath);
            DownloadFiles(client, FtpPathSz, LastDownlaodFileSz, DownloadToPath);
            //Task.Factory.StartNew(() => DownloadFiles(client, FtpPathSh, LastDownlaodFileSh, DownloadToPath));
            //Task.Factory.StartNew(() => DownloadFiles(client, FtpPathSz, LastDownlaodFileSz, DownloadToPath));
            //Task.WaitAll();
            return;
        }

        private FtpClient GetFtpClient()
        {
            return GetFtpClient(Host, UserName, Password);
        }
        private FtpClient GetFtpClient(string host, string userName, string password)
        {
            return new FtpClient(host, userName, password);
        }
        private IList<string> GetFileList(FtpClient client, string ftpPath)
        {
            IList<string> fileList = new List<string>();
            foreach (FtpListItem item in client.GetListing(ftpPath))
            {
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    fileList.Add(item.Name);
                    //Console.WriteLine($"{item.FullName},{item.Created},{item.Modified}");
                }
            }
            return fileList;
        }
        private IList<string> GetDownloadFileList(FtpClient client, string ftpPath, string lastDownloadFile)
        {
            var fileList = GetFileList(client, ftpPath);
            var lastDownloadFileName = GetLastDownlaodFileName(lastDownloadFile);
            return fileList != null && fileList.Any() ? (from fileName in fileList where (string.IsNullOrWhiteSpace(lastDownloadFileName) ? true : fileName.CompareTo(lastDownloadFileName) > 0) && fileName.EndsWith(".rar") orderby fileName select fileName).ToList<string>() : null;
        }
        private void DownloadFiles(FtpClient client, string ftpPath, string lastDownloadFile, string downloadToPath)
        {
            var fileList = GetDownloadFileList(client, ftpPath, lastDownloadFile);
            foreach (var fileName in fileList)
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd ss:mm:ss.fff")}, {fileName} start downloading...");
                var isDownloaded = client.DownloadFile(localPath: $"{downloadToPath}\\{fileName}", remotePath: $"/{ftpPath}/{fileName}".Replace("//", "/"), overwrite: true);
                File.WriteAllText(lastDownloadFile, fileName);
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd ss:mm:ss.fff")}, {fileName} downloaded, Elapsed:{stopwatch.ElapsedMilliseconds / 1000}s");
            }
        }
        private string GetLastDownlaodFileName(string fileName)
        {
            string content = null;
            if (File.Exists(fileName))
            {
                content = File.ReadAllText(fileName);
            }
            return content;
        }
    }
}
